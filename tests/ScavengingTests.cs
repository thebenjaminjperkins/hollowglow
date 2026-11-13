using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Hollowlight.Core;

public static class ScavengingTests
{
    public static string Data(string name) => File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "data", name + ".json"));
    public static ScavengingSetup Setup => ScavengingSetup.Parse(Data("food_scavenging"));
    public static SourceWorld Generate(int seed, string layout = "sanctuary") => FiniteScavenging.Generate(Setup, CaveLayout.Parse(Data(layout)),
        SupplySetup.Parse(Data("supplies")).Sites, seed);
    public static SharedSupplies State(SourceWorld world, int[]? players = null)
    {
        var setup = SupplySetup.Parse(Data("supplies"));
        return new(setup.Resources, setup.Sites.Concat(world.Sources.Select(s => s.Site)).Select(s =>
            new ResourceNodeDefinition(s.Id, s.ResourceId, s.Quantity)), players ?? new[] { 1, 2 });
    }
    private static void Check(bool value, string message) { if (!value) throw new Exception(message); }
    public static void Run()
    {
        var unique = new System.Collections.Generic.HashSet<string>();
        foreach (string layout in new[] { "sanctuary", "sanctuary_alternate" })
        for (int seed = 0; seed < 256; seed++)
        {
            var world = Generate(seed, layout); string signature = JsonSerializer.Serialize(world);
            Check(signature == JsonSerializer.Serialize(Generate(seed, layout)), "Same seed/layout/config reproduces all node IDs, placement and quantities");
            unique.Add(signature);
            Check(world.Sources.Count == 6 && world.Sources.Sum(s => s.Site.Quantity) is >= 8 and <= 17, "Exactly six finite sources with approved 8–17 Food bounds");
            Check(world.Sources.Count(s => s.Band == "near" && s.Type == "wild_plant") == 2 &&
                world.Sources.Count(s => s.Band == "far" && s.Type == "container") == 2, "Near/far source composition fixed");
            foreach (var source in world.Sources)
            {
                var type = Setup.Types.Single(t => t.Id == source.Type); bool near = source.Band == "near";
                Check(source.Site.Quantity >= (near ? type.NearMin : type.FarMin) && source.Site.Quantity <= (near ? type.NearMax : type.FarMax), "Per-type/band quantity bounds");
                Check(source.PathDistance >= (near ? 32 : 160) && source.PathDistance <= (near ? 112 : 288) &&
                    source.Site.X >= 320 && source.ApproachY == source.Site.Y + 12, "Surface-only placement respects path bands and a reachable interaction approach");
                foreach (var other in world.Sources.Where(o => o != source))
                    Check(Math.Pow(source.Site.X - other.Site.X, 2) + Math.Pow(source.Site.Y - other.Site.Y, 2) >= 32 * 32, "Sources do not crowd one another");
            }
            if (seed == 0) Console.WriteLine($"PLACEMENT {layout}: {string.Join("; ", world.Sources.Select(x => $"{x.Site.Id} ({x.Site.X},{x.Site.Y}) d{x.PathDistance} q{x.Site.Quantity}"))}");
        }
        Check(unique.Count == 256, "Seeds produce meaningful placement and quantity variation on both layouts");
        var generated = Generate(1701); var s = State(generated); var first = generated.Sources.First(x => x.Site.Quantity >= 2);
        var version = s.Snapshot(1).Version;
        var result = s.Gather(1, first.Site.Id, 1, version);
        Check(result.Amount == 1 && s.Snapshot(1).Remaining[first.Site.Id] == first.Site.Quantity - 1, "Partial source collection leaves remainder");
        Check(s.Gather(2, first.Site.Id, 10, version).Status == SupplyStatus.Stale, "Replayed/conflicting gather cannot duplicate Food");
        foreach (var node in generated.Sources) s.Gather(1, node.Site.Id, int.MaxValue, s.Snapshot(1).Version);
        int total = generated.Sources.Sum(x => x.Site.Quantity);
        Check(s.Snapshot(1).Personal.Count("food") == total && generated.Sources.All(n => s.Snapshot(1).Remaining[n.Site.Id] == 0), "All finite Food conserved on depletion");
        Check(s.Gather(1, first.Site.Id, 1, s.Snapshot(1).Version).Status == SupplyStatus.Empty, "Depleted sources stay empty");
        var reset = State(Generate(1701));
        Check(reset.Snapshot(1).Remaining[first.Site.Id] == first.Site.Quantity && reset.Gather(1, first.Site.Id, 1, version).Status == SupplyStatus.Stale,
            "New run restores same seeded supply and invalidates old-session commands");
        var filled = State(generated);
        // Six occupied slots and a Food stack at nine: only one unit can fit.
        filled.Gather(1, "radio-shell", 12, filled.Snapshot(1).Version);
        for (int i = 2; i < 5; i++) filled.ApplySlots(1, new(SlotAction.Move, new(InventoryKind.Personal, 0), new(InventoryKind.Personal, i), 1, filled.Snapshot(1).Version));
        int food = 0;
        foreach (var node in generated.Sources)
        {
            int want = Math.Min(9 - food, node.Site.Quantity); if (want <= 0) break;
            food += filled.Gather(1, node.Site.Id, want, filled.Snapshot(1).Version).Amount;
        }
        Check(food == 9 && filled.Snapshot(1).Personal.UsedSlots == 6, "Capacity fixture has nine Food and no spare slot");
        var remainingNode = generated.Sources.First(n => filled.Snapshot(1).Remaining[n.Site.Id] > 0);
        Check(filled.Gather(1, remainingNode.Site.Id, 10, filled.Snapshot(1).Version).Amount == 1, "Existing Food stack partially fills to ten");
        var before = filled.Snapshot(1);
        remainingNode = generated.Sources.First(n => before.Remaining[n.Site.Id] > 0);
        Check(filled.Gather(1, remainingNode.Site.Id, 10, before.Version).Status == SupplyStatus.NoCapacity && filled.Snapshot(1).Version == before.Version &&
            filled.Snapshot(1).Remaining[remainingNode.Site.Id] == before.Remaining[remainingNode.Site.Id], "Full pack changes neither source nor inventory");
        filled.DepositAll(1, before.Version);
        Check(filled.Gather(1, remainingNode.Site.Id, 10, filled.Snapshot(1).Version).Status == SupplyStatus.Applied, "Remainder collectible after storing supplies");
        for (int round = 0; round < 32; round++)
        {
            var race = State(generated); var v = race.Snapshot(1).Version;
            Parallel.Invoke(() => race.Gather(1, first.Site.Id, 10, v), () => race.Gather(2, first.Site.Id, 10, v));
            Check(race.Snapshot(1).Personal.Count("food") + race.Snapshot(2).Personal.Count("food") == first.Site.Quantity &&
                race.Snapshot(1).Remaining[first.Site.Id] == 0, "Two actors cannot duplicate a finite source");
        }
        var altered = Setup with { Types = Setup.Types.Select(t => t with { NearMin = 1, NearMax = 1, FarMin = 1, FarMax = 1 }).ToArray() };
        var samePositions = FiniteScavenging.Generate(altered, CaveLayout.Parse(Data("sanctuary")), SupplySetup.Parse(Data("supplies")).Sites, 1701);
        Check(generated.Sources.Select(s => (s.Site.X,s.Site.Y)).SequenceEqual(samePositions.Sources.Select(s => (s.Site.X,s.Site.Y))), "Quantity configuration does not perturb placement stream");
        try { FiniteScavenging.Generate(Setup with { NearMaxDistance = 16 }, CaveLayout.Parse(Data("sanctuary")), Array.Empty<GatheringSite>(), 1); throw new Exception("Invalid bands accepted"); }
        catch (ArgumentException) { }
        Console.WriteLine("PASS: 512 seeded layout cases, stable independent streams, approved quantities/bands, spacing, partial/full collection, conservation, depletion, reset and 32 concurrent gathers");
    }
}
