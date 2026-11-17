using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Hollowlight.Core;

public sealed record HallReservation(string Id,int X,int Y,int Width,int Height);
public sealed record HallFixture(string Id,string Asset,int X,int Y,int Width,int Height);
// A reusable floor-mask input to sanctuary generation. It does not replace surface graph generation.
public sealed record SanctuaryPlan(int[] Offset,string[] Rows,Dictionary<string,int[]> Anchors,PropPlacement[] Props,
    HallReservation[] Reservations,HallFixture[] Fixtures,int[] Entrance)
{
    public static SanctuaryPlan Parse(string json)=>JsonSerializer.Deserialize<SanctuaryPlan>(json,new JsonSerializerOptions{PropertyNameCaseInsensitive=true})??throw new ArgumentException("Missing sanctuary plan.");
    public GridPoint Cell(int x,int y)=>new(Offset[0]+x,Offset[1]+y);
    public GridPoint FixtureFoot(HallFixture f)=>Cell(f.X,f.Y);
    public RegionDefinition Apply(RegionDefinition region)
    {
        if(Offset.Length!=2||Rows.Length==0||Rows.Any(r=>r.Length!=Rows[0].Length)||Entrance.Length!=4||Entrance[2]!=6||
            Rows.Any(r=>r.Any(c=>c is not ('.' or '#')))||Offset[0]<1||Offset[1]<1||Offset[0]+Rows[0].Length>=region.SanctuaryWidth||Offset[1]+Rows.Length>=region.SanctuaryHeight)
            throw new ArgumentException("Invalid sanctuary mask, bounds or reserved entrance.");
        return region with{Hall=this,Anchors=Anchors.ToDictionary(p=>p.Key,p=>new[]{Offset[0]+p.Value[0],Offset[1]+p.Value[1]}),Props=Props,
            Chambers=new[]{new ChamberRule("hall","Sanctuary hall",Offset[0]+16,Offset[1]+16,12,12)},ChamberLinks=Array.Empty<string[]>()};
    }
}
