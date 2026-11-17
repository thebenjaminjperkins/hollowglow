using System;
using System.IO;
using System.Linq;
using Hollowlight.Core;

public static class HallTests
{
    public static SanctuaryPlan Plan=>SanctuaryPlan.Parse(File.ReadAllText(Path.Combine(AppContext.BaseDirectory,"data/regions/sanctuary_hall.json")));
    private static void Check(bool value,string message){if(!value)throw new Exception(message);}
    public static void Run()
    {
        var plan=Plan;
        for(int seed=0;seed<16;seed++)
        {
            var old=RegionTests.Generate(seed);old.InstallGarage(GarageTests.Definition);
            var hall=RegionTests.Generate(seed,plan.Apply(RegionTests.Definition));hall.InstallGarage(GarageTests.Definition);
            for(int y=0;y<old.Height;y++)for(int x=old.Definition.SurfaceOffsetX;x<old.Width;x++)
                Check(old.Ground[x,y]==hall.Ground[x,y],"Hall profile cannot change surface geography");
            Check(old.Sources.SequenceEqual(hall.Sources),"Exact generated locations/quantities remain the same");
            var reachable=hall.Distances(hall.Anchor("home"));
            foreach(var p in hall.Definition.Anchors.Where(p=>p.Key!="chest").Select(p=>p.Value))Check(reachable.ContainsKey(new(p[0],p[1])),"Every hall gameplay anchor reachable");
            Check(reachable.ContainsKey(new(hall.Anchor("chest").X,hall.Anchor("chest").Y+1)),"Chest has a reachable front approach");
            foreach(var prop in hall.Layout.Props.Where(p=>p.Kind is "Generator" or "Bench" or "Shelf" or "Garden"))
                Check(reachable.ContainsKey(MountainRegion.CellAt(prop.X,prop.Y+12)),"Station/farm approach reachable");
            Check(hall.Definition.Props.Count(p=>p.Kind=="Garden")==2,"Exactly four starting plots, not eight installed plots");
            for(int x=0;x<6;x++)Check(hall.Walkable(plan.Cell(plan.Entrance[0]+x,plan.Entrance[1])),"All six entrance tiles remain open");
            Check(plan.Reservations.Single(r=>r.Id=="garden-eight").Width>=7&&plan.Reservations.Single(r=>r.Id=="utilities").Width>=6,"Future production space reserved without installations");
            foreach(var area in plan.Reservations)
                for(int y=area.Y;y<area.Y+area.Height;y++)for(int x=area.X;x<area.X+area.Width;x++)
                {var p=plan.Cell(x,y);Check(hall.Layout.IsFloor(p.X,p.Y),"Reserved area is actual floor: "+area.Id);}
            Check(hall.Definition.Props.Count(p=>p.Kind=="Generator")==1&&hall.Definition.Props.Count(p=>p.Kind=="Bench")==1,"Inert damaged chamber adds no generator/workbench");
        }
        Console.WriteLine("PASS: 16 hall seeds, identical surface and supplies, all station/plot/entrance approaches, six-tile opening, four starting plots and future-only reservations");
    }
}
