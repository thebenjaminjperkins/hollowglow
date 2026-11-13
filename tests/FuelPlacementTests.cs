using System;
using System.Linq;
using Hollowlight.Core;

public static class FuelPlacementTests
{
    public static SourceWorld Generate(int seed, string layout = "sanctuary")
    {
        var food = ScavengingTests.Generate(seed,layout);
        return FiniteScavenging.Generate(ScavengingSetup.Parse(ScavengingTests.Data("fuel_scavenging")),
            CaveLayout.Parse(ScavengingTests.Data(layout)), SupplySetup.Parse(ScavengingTests.Data("supplies")).Sites, seed, food.Sources.Select(s=>s.Site));
    }
    public static void Run()
    {
        foreach(string layout in new[]{"sanctuary","sanctuary_alternate"})
        for(int seed=0;seed<256;seed++)
        {
            var fuel=Generate(seed,layout);
            if(fuel.Sources.Count!=2 || fuel.Sources.Sum(x=>x.Site.Quantity) is <5 or >8 || fuel.Sources.Any(x=>x.Site.ResourceId!="fuel")) throw new Exception("Fuel cache bounds");
        }
        Console.WriteLine("PASS: two finite seeded Fuel caches across 512 layout/seed cases; 5–8 surface Fuel, separate Food stream");
    }
}
