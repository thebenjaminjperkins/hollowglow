using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Hollowlight.Core;

public static class FarmingTests
{
    private static AgricultureSetup Setup => AgricultureSetup.Parse(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "data/farming.json")));
    private static (SharedSupplies, GameClock) Create()
    {
        var supplies = SupplySetup.Parse(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "data/supplies.json")));
        var clock = new GameClock(new()); var setup = Setup;
        var state = supplies.CreateState(new[] { 1, 2 }, 24, setup.Resources);
        state.InitializeAgriculture(setup, clock, new[] { "a", "b", "c", "d" });
        return (state, clock);
    }
    private static void Check(bool value, string message) { if (!value) throw new Exception(message); }
    private static void Take(SharedSupplies state, int actor, string resource, int amount) =>
        Check(state.Transfer(actor, resource, amount, true, state.Snapshot(actor).Version).Amount == amount, "Seed transfer from communal supply");
    private static FarmResult Plant(SharedSupplies state, int actor, string plot, string crop) => state.Plant(actor, plot, crop, state.Plot(plot).Version);
    public static void Run()
    {
        var (s, clock) = Create();
        Check(s.Snapshot(1).Storage.Count("potato_seed") == 2 && s.Snapshot(1).Storage.Count("glowcap_seed") == 2,
            "Exactly one shared starter supply");
        Check(Plant(s, 1, "a", "cave_potato").Status == FarmStatus.NoSeed, "Shared seeds cannot be spent without personal withdrawal");
        Take(s, 1, "potato_seed", 2); Take(s, 1, "glowcap_seed", 2);
        var old = s.Plot("a").Version;
        Check(Plant(s, 1, "a", "cave_potato").Status == FarmStatus.Planted && s.Snapshot(1).Personal.Count("potato_seed") == 1,
            "Plant consumes one personal seed");
        Check(s.Plant(1, "a", "cave_potato", old).Status == FarmStatus.Stale && s.Snapshot(1).Personal.Count("potato_seed") == 1,
            "Replayed planting cannot consume another seed");
        Check(Plant(s, 1, "a", "glowcap").Status == FarmStatus.Occupied, "Occupied plot cannot be overwritten");
        Check(s.Plot("a").Stage == CropStage.Seedling, "Fresh planting is seedling");
        Check(s.Harvest(1, "a", s.Plot("a").Version).Status == FarmStatus.NotMature, "Premature harvest rejected");
        clock.Advance(420);
        Check(s.Plot("a").Stage == CropStage.Growing, "Configured growing threshold");
        clock.Advance(779.9); Check(s.Plot("a").Stage == CropStage.Growing, "Does not mature early at calendar rollover");
        clock.Advance(0.1); Check(s.Plot("a").Stage == CropStage.Mature, "Potato matures after two complete elapsed cycles");
        var ripe = s.Plot("a").Version;
        Check(s.Harvest(1, "a", ripe).Status == FarmStatus.Harvested && s.Snapshot(1).Personal.Count("food") == 3 &&
            s.Snapshot(1).Personal.Count("potato_seed") == 2 && s.Plot("a").Stage == CropStage.Empty, "Exact potato yield and returned seed");
        Check(s.Harvest(2, "a", ripe).Status == FarmStatus.Stale, "Another actor cannot repeat accepted harvest");
        Check(Plant(s, 1, "a", "cave_potato").Status == FarmStatus.Planted, "Harvested plot can be replanted");
        Check(Plant(s, 1, "b", "glowcap").Status == FarmStatus.Planted, "Glowcap plants independently");
        clock.Advance(1200); Check(s.Plot("b").Stage == CropStage.Growing, "Glowcap grows more slowly than potato");
        clock.Advance(600);
        Check(s.Harvest(1, "b", s.Plot("b").Version).Status == FarmStatus.Harvested && s.Snapshot(1).Personal.Count("food") == 5 &&
            s.Snapshot(1).Personal.Count("organic") == 1 && s.Snapshot(1).Personal.Count("glowcap_seed") == 2, "Exact Glowcap Food/Organic/seed yield");
        var serialized = JsonSerializer.Serialize(s.Plots());
        Check(JsonSerializer.Deserialize<PlotSnapshot[]>(serialized)![0].PlantedTicks == s.Plot("a").PlantedTicks,
            "Plot IDs/timestamps/revisions serialize without renderer references");

        var (full, fullClock) = Create(); Take(full, 1, "potato_seed", 1);
        Plant(full, 1, "a", "cave_potato");
        full.Gather(1, "radio-shell", 12, full.Snapshot(1).Version);
        full.Gather(1, "machine-housing", 14, full.Snapshot(1).Version);
        full.Gather(1, "tool-case", 8, full.Snapshot(1).Version);
        // Spread existing Scrap into every free personal slot; all six are occupied.
        full.ApplySlots(1, new(SlotAction.Move, new(InventoryKind.Personal, 0), new(InventoryKind.Personal, 4), 1, full.Snapshot(1).Version));
        full.ApplySlots(1, new(SlotAction.Move, new(InventoryKind.Personal, 0), new(InventoryKind.Personal, 5), 1, full.Snapshot(1).Version));
        fullClock.Advance(1200); var before = full.Snapshot(1); var mature = full.Plot("a");
        Check(full.Harvest(1, "a", mature.Version).Status == FarmStatus.NoCapacity && full.Plot("a") == mature &&
            full.Snapshot(1).Version == before.Version && full.Snapshot(1).Personal.Total == before.Personal.Total,
            "Full pack leaves complete harvest and crop intact without changing versions");
        full.DepositAll(1, full.Snapshot(1).Version);
        Check(full.Harvest(1, "a", mature.Version).Status == FarmStatus.Harvested, "Same mature crop harvests after making room");

        // One spare slot fits Food, but not the returned seed: reject the entire bundle.
        var (seedRoom, seedClock) = Create(); Take(seedRoom, 1, "potato_seed", 1); Plant(seedRoom, 1, "a", "cave_potato");
        seedRoom.Gather(1, "radio-shell", 12, seedRoom.Snapshot(1).Version);
        for (int i = 2; i < 5; i++) seedRoom.ApplySlots(1, new(SlotAction.Move, new(InventoryKind.Personal, 0), new(InventoryKind.Personal, i), 1, seedRoom.Snapshot(1).Version));
        seedClock.Advance(1200);
        Check(seedRoom.Snapshot(1).Personal.UsedSlots == 5 && seedRoom.Harvest(1, "a", seedRoom.Plot("a").Version).Status == FarmStatus.NoCapacity &&
            seedRoom.Snapshot(1).Personal.Count("food") == 0, "Returned seed capacity is included before any Food is added");
        var (merge, mergeClock) = Create(); Take(merge, 1, "potato_seed", 2); Plant(merge, 1, "a", "cave_potato");
        mergeClock.Advance(1200); merge.Harvest(1, "a", merge.Plot("a").Version); Plant(merge, 1, "a", "cave_potato");
        merge.Gather(1, "radio-shell", 12, merge.Snapshot(1).Version); merge.Gather(1, "tool-case", 8, merge.Snapshot(1).Version);
        merge.ApplySlots(1, new(SlotAction.Move, new(InventoryKind.Personal, 2), new(InventoryKind.Personal, 5), 1, merge.Snapshot(1).Version));
        mergeClock.Advance(1200);
        Check(merge.Snapshot(1).Personal.UsedSlots == 6 && merge.Harvest(1, "a", merge.Plot("a").Version).Status == FarmStatus.Harvested &&
            merge.Snapshot(1).Personal.Count("food") == 6, "Full slot grid can harvest when every yield fits existing partial stacks");
        var (plantRace, _) = Create(); Take(plantRace, 1, "potato_seed", 1); Take(plantRace, 2, "glowcap_seed", 1);
        FarmResult? plantA = null, plantB = null; var empty = plantRace.Plot("a").Version;
        Parallel.Invoke(() => plantA = plantRace.Plant(1, "a", "cave_potato", empty), () => plantB = plantRace.Plant(2, "a", "glowcap", empty));
        Check(new[] { plantA!, plantB! }.Count(r => r.Status == FarmStatus.Planted) == 1 &&
            new[] { plantA!, plantB! }.Count(r => r.Status == FarmStatus.Stale) == 1 &&
            plantRace.Snapshot(1).Personal.Total + plantRace.Snapshot(2).Personal.Total + plantRace.Snapshot(1).Storage.Total == 3,
            "Competing planting consumes exactly one seed and preserves the loser's stock");
        var baseSupply = SupplySetup.Parse(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "data/supplies.json")));
        var tooSmall = baseSupply.CreateState(new[] { 1 }, 1, Setup.Resources);
        try { tooSmall.InitializeAgriculture(Setup, new GameClock(new()), new[] { "a" }); throw new Exception("Starter seed overflow accepted"); }
        catch (ArgumentException) { }
        Check(tooSmall.Snapshot(1).Storage.Total == 0, "Starter allocation is atomic when configured storage is too small");
        for (int round = 0; round < 32; round++)
        {
            var (race, raceClock) = Create(); Take(race, 1, "potato_seed", 1); Take(race, 2, "potato_seed", 1);
            var a = race.Plot("a").Version; var b = race.Plot("b").Version;
            FarmResult? x = null, y = null;
            Parallel.Invoke(() => x = race.Plant(1, "a", "cave_potato", a), () => y = race.Plant(2, "b", "cave_potato", b));
            Check(x!.Status == FarmStatus.Planted && y!.Status == FarmStatus.Planted, "Different plots accept concurrent actors without inventory-version conflicts");
            raceClock.Advance(1200); a = race.Plot("a").Version;
            Parallel.Invoke(() => x = race.Harvest(1, "a", a), () => y = race.Harvest(2, "a", a));
            Check(new[] { x!, y! }.Count(r => r.Status == FarmStatus.Harvested) == 1 &&
                race.Snapshot(1).Personal.Count("food") + race.Snapshot(2).Personal.Count("food") == 3,
                "Competing harvest has one winner and one complete yield");
        }
        var (fresh, _) = Create();
        Check(fresh.Plots().All(p => p.Stage == CropStage.Empty) && fresh.Snapshot(1).Personal.Total == 0 && fresh.Snapshot(1).Storage.Total == 4,
            "New session resets plots, personal stock and starter seeds");
        Check(fresh.Plant(1, "a", "cave_potato", old).Status == FarmStatus.Stale, "Pre-reset plot command rejected");
        Check(fresh.Plant(99, "a", "cave_potato", fresh.Plot("a").Version).Status == FarmStatus.InvalidPlayer &&
            Plant(fresh, 1, "a", "unknown").Status == FarmStatus.UnknownCrop, "Invalid actors/crops cannot mutate plots");
        Console.WriteLine("PASS: shared seeds, planting costs, stages, exact elapsed maturity/yields, full-pack/seed capacity safety, replant/reset, and 32 concurrent planting/harvest races");
    }
}
