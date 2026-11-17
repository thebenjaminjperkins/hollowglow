using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Hollowlight.Core;

public sealed record GarageObjectRule(string Id,string Name,string Kind,string Asset,int X,int Y,string FromBudget,int Quantity,
    int Width,int Height,int OffsetY=0,int[]? Stages=null,double Seconds=.8);
public sealed record GarageDefinition(int Width,int Height,int OffsetX,int OffsetY,int[] Entry,string[] Rows,GarageObjectRule[] Objects)
{
    public static GarageDefinition Parse(string json)=>JsonSerializer.Deserialize<GarageDefinition>(json,
        new JsonSerializerOptions{PropertyNameCaseInsensitive=true})??throw new ArgumentException("Missing garage template.");
}
public sealed record GarageObject(GarageObjectRule Rule,GridPoint Foot,GeneratedSourceSite Source,ExplorationDefinition Definition)
{
    public string Id=>Definition.Id;
    public GridPoint Approach=>new(Foot.X,Foot.Y+1);
}
public sealed class GaragePlan
{
    public const string Tool="salvage_tool";
    public GarageDefinition Definition{get;}
    public GridPoint Origin{get;}
    public GridPoint Entry=>new(Origin.X+Definition.Entry[0],Origin.Y+Definition.Entry[1]);
    public IReadOnlyList<GarageObject> Objects{get;}
    private readonly int _left,_right,_top,_bottom;
    public bool Contains(int x,int y,int margin=0)=>x>=Origin.X-margin&&y>=Origin.Y-margin&&x<Origin.X+Definition.Width+margin&&y<Origin.Y+Definition.Height+margin;
    public bool Building(int x,int y)=>x>=Origin.X+_left&&x<=Origin.X+_right&&y>=Origin.Y+_top&&y<=Origin.Y+_bottom;
    public bool Wall(int x,int y)=>Contains(x,y)&&Definition.Rows[y-Origin.Y][x-Origin.X]=='#';
    public GaragePlan(GarageDefinition definition,GridPoint center,IReadOnlyList<GeneratedSourceSite> sources)
    {
        Definition=definition;Origin=new(center.X+definition.OffsetX,center.Y+definition.OffsetY);
        if(definition.Rows.Length!=definition.Height||definition.Rows.Any(r=>r.Length!=definition.Width))throw new ArgumentException("Garage rows must match dimensions.");
        var walls=definition.Rows.SelectMany((row,y)=>row.Select((c,x)=>(c,x,y))).Where(p=>p.c=='#').ToArray();
        if(walls.Length==0)throw new ArgumentException("Garage needs a building boundary.");
        _left=walls.Min(p=>p.x);_right=walls.Max(p=>p.x);_top=walls.Min(p=>p.y);_bottom=walls.Max(p=>p.y);
        foreach(var group in definition.Objects.Where(o=>o.FromBudget!="").GroupBy(o=>o.FromBudget))
        {
            int total=sources.Single(s=>s.Site.Id==group.Key).Site.Quantity;
            if(group.Sum(o=>o.Quantity==0?total:o.Quantity)!=total)throw new ArgumentException("Garage must conserve every allocated source budget.");
        }
        Objects=definition.Objects.Select(o=>
        {
            var foot=new GridPoint(Origin.X+o.X,Origin.Y+o.Y);
            var budget=o.FromBudget==""?null:sources.Single(s=>s.Site.Id==o.FromBudget);
            string resource=budget?.Site.ResourceId??Tool;
            int quantity=o.Quantity==0?budget!.Site.Quantity:o.Quantity;
            string id="garage-"+o.Id;
            var site=new GatheringSite(id,o.Name,resource,quantity,foot.WorldX,foot.WorldY+12,o.Asset);
            return new GarageObject(o,foot,new(site,o.Kind,"garage",0,foot.WorldX,foot.WorldY+16),
                new(id,o.Name,o.Kind,id,resource,o.Stages??Array.Empty<int>(),o.Stages!=null?Tool:"",o.Seconds));
        }).ToArray();
    }
}
