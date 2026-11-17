using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Hollowlight.Core;

public static class RegionTests
{
    public static RegionDefinition Definition=>RegionDefinition.Parse(File.ReadAllText(Path.Combine(AppContext.BaseDirectory,"data/regions/mountain.json")));
    public static MountainRegion Generate(int seed,RegionDefinition? definition=null)=>MountainRegion.Generate(definition??Definition,
        SupplySetup.Parse(ScavengingTests.Data("supplies")),ScavengingSetup.Parse(ScavengingTests.Data("food_scavenging")),
        ScavengingSetup.Parse(ScavengingTests.Data("fuel_scavenging")),seed);
    private static void Check(bool value,string message){if(!value)throw new Exception(message);}
    private static string Signature(MountainRegion region)=>Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(
        string.Join("\n",region.Layout.Rows)+string.Join(",",region.Ground.Cast<GroundKind>())+string.Join(";",region.Landmarks)+string.Join(";",region.Routes.Select(r=>r.Id+string.Join(",",r.Cells)))+string.Join(";",region.Decorations)+string.Join(";",region.Layout.Props.Select(p=>p.ToString()))+string.Join(";",region.Sources))));
    public static void Run()
    {
        var variants=new System.Collections.Generic.HashSet<string>();var routeVariants=new System.Collections.Generic.HashSet<int>();
        for(int seed=0;seed<32;seed++)
        {
            var region=Generate(seed);var connected=region.Distances(region.Entrance.Cell);
            Check(region.Width==272&&region.Height==144&&region.Definition.Chambers.Length==5,"Approved surface/interior dimensions and five chambers");
            Check(region.Landmarks.All(n=>connected.ContainsKey(n.Cell)),"All graph landmarks connected in actual walkable geography");
            Check(region.Sources.Count==11&&region.Sources.Select(s=>s.Site.Id).Distinct().Count()==11,"Exact finite source count and stable unique IDs");
            Check(region.Sources.Where(s=>s.Site.ResourceId=="food").Sum(s=>s.Site.Quantity)==ScavengingTests.Generate(seed).Sources.Sum(s=>s.Site.Quantity),"Terrain generation does not reroll Food quantities");
            Check(region.Sources.Where(s=>s.Site.ResourceId=="fuel").Sum(s=>s.Site.Quantity)==FuelPlacementTests.Generate(seed).Sources.Sum(s=>s.Site.Quantity),"Terrain generation does not reroll Fuel quantities");
            Check(region.Sources.Where(s=>s.Site.ResourceId=="scrap").Sum(s=>s.Site.Quantity)==26&&region.Sources.Where(s=>s.Site.ResourceId=="organic").Sum(s=>s.Site.Quantity)==8,"Original material budget preserved");
            foreach(var source in region.Sources)Check(connected.ContainsKey(MountainRegion.CellAt(source.ApproachX,source.ApproachY)),"Every resource has a reachable approach");
            var home=region.Anchor("home");var local=region.Distances(home);
            Check(local.ContainsKey(region.Anchor("insidePortal"))&&local.ContainsKey(region.Anchor("insideLanding"))&&local.ContainsKey(region.Anchor("expansion")),"Interior home, entrance and fifth-plot socket connected");
            foreach(var prop in region.Layout.Props.Where(p=>p.Kind is "Bench" or "Generator" or "Garden" or "Shelf"))
                Check(local.ContainsKey(MountainRegion.CellAt(prop.X,prop.Y+12)),"All sanctuary work/farm/power interaction anchors reachable");
            Check(local[region.Anchor("insidePortal")]/80<12,"Sanctuary entrance within twelve seconds of home");
            variants.Add(Signature(region));routeVariants.Add(region.Routes.Count);
            if(seed==0)Check(Signature(region)==Signature(Generate(seed)),"Same seed reproduces complete layout, sources and route graph");
        }
        Check(variants.Count==32&&routeVariants.Count==2,"Seeds vary meaningful landmark positions and optional route connectivity");
        var original=Generate(1701);var flipped=Generate(1701,Definition with {Orientations=new[]{"mirror-x"}});
        foreach(var node in original.Landmarks)
        {
            var other=flipped.Landmarks.Single(n=>n.Id==node.Id);
            Check(other.Cell.X==Definition.SurfaceOffsetX+Definition.SurfaceWidth-1-(node.Cell.X-Definition.SurfaceOffsetX)&&other.Cell.Y==node.Cell.Y,
                "Alternate geography orientation needs only profile data, not special coordinate assumptions");
        }
        var moved=Definition with {Landmarks=Definition.Landmarks.Select(n=>n.Kind=="yard"?n with {MinX=.78,MaxX=.82,MinY=.43,MaxY=.48}:n).ToArray(),
            Routes=Definition.Routes.Where(r=>r.Id!="ridge-trail").ToArray()};
        var changed=Generate(1701,moved);
        Check(changed.Distances(changed.Entrance.Cell).ContainsKey(changed.Landmarks.Single(n=>n.Kind=="yard").Cell),"Moving a landmark and removing a route are supported by data-driven graph generation");
        Console.WriteLine("PASS: 32 seeded regions, reproducibility, landmark/route variation, data-only geography changes, five connected chambers, all resource approaches and unchanged Food/Fuel/material budgets");
    }
}
