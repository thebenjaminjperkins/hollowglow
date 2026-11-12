using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Hollowlight.Core;

public static class HungerTests
{
    private static void Check(bool value, string message) { if (!value) throw new Exception(message); }
    private static (SharedSupplies s, GameClock c) Create(HungerDefinition? def = null)
    {
        var s = new SharedSupplies(new[] { new ResourceDefinition("food", "Food", "Food", true),
            new ResourceDefinition("ration", "Fixture ration", "Ration", true), new ResourceDefinition("seed", "Seed", "Seed") },
            new[] { new ResourceNodeDefinition("rations", "ration", 100), new ResourceNodeDefinition("seeds", "seed", 10) }, new[] { 1, 2 });
        var c = new GameClock(new()); s.InitializeHunger(def ?? new(), c); return (s, c);
    }
    private static EatRequest Request(SharedSupplies s, int id, int slot = 0) => new(slot, s.Snapshot(id).Version, s.Hunger(id).Day);
    private static void Add(SharedSupplies s, int id, string node, int amount) => s.Gather(id, node, amount, s.Snapshot(id).Version);
    public static void Run()
    {
        var (s, c) = Create();
        Check(s.Snapshot(1).Personal.Count("food") == 2 && s.Snapshot(2).Personal.Count("food") == 2 && s.Hunger(1).Stage == HungerStage.Fed,
            "Two personal starter Food per actor; initially fed without a credited meal");
        var request = Request(s, 1);
        Check(s.Eat(1, request).Status == EatStatus.Eaten && s.Eat(1, request).Status == EatStatus.Stale, "Eating is atomic and replay-safe");
        Check(s.Eat(1, Request(s, 1)).Status == EatStatus.Satisfied && s.Snapshot(1).Personal.Count("food") == 1, "No wasted extra food or future banking");
        c.Advance(600);
        Check(s.Snapshot(1).Personal.Count("food") == 1 && s.Snapshot(2).Personal.Count("food") == 1 && s.Hunger(1).Consumed == 1,
            "Manual meal prevents dawn double consumption; other actor eats automatically");
        c.Advance(600);
        Check(s.Hunger(1).Consumed == 2 && s.Hunger(2).Consumed == 2 && s.Hunger(1).Eaten == 0, "Exactly one personal Food per day");
        c.Advance(600); Check(s.Hunger(1).Stage == HungerStage.Hungry && s.Hunger(1).SpeedMultiplier == 1, "First missed meal warns only");
        c.Advance(600); Check(s.Hunger(1).Stage == HungerStage.Starving && s.Hunger(1).SpeedMultiplier == .9, "Second miss slows ten percent");
        c.Advance(600); Check(s.Hunger(1).Stage == HungerStage.Critical && s.Hunger(1).SpeedMultiplier == .8, "Third miss slows twenty percent");
        Add(s, 1, "rations", 4);
        Check(s.Eat(1, Request(s, 1)).Status == EatStatus.Eaten && s.Hunger(1).MissedMeals == 2 && s.Hunger(1).Stage == HungerStage.Starving,
            "A different edible resource recovers only one accumulated miss");
        c.Advance(600);
        Check(s.Hunger(1).MissedMeals == 2 && s.Hunger(1).Consumed == 3, "Dawn cannot recover again from an already credited meal");
        c.Advance(600); Check(s.Hunger(1).MissedMeals == 1 && s.Hunger(2).MissedMeals == 5, "Automatic recovery is gradual and player-independent");
        c.Advance(600); Check(s.Hunger(1).MissedMeals == 0 && s.Hunger(1).Stage == HungerStage.Fed, "Multiple daily meals restore Fed");
        Check(s.Eat(99, Request(s, 1)).Status == EatStatus.InvalidPlayer && s.Eat(1, Request(s, 1, 100)).Status == EatStatus.InvalidSlot,
            "Invalid actor and slot rejected");
        Add(s, 1, "seeds", 1);
        int seed = s.Snapshot(1).Personal.Slots.ToList().FindIndex(x => x?.ResourceId == "seed");
        Check(s.Eat(1, Request(s, 1, seed)).Status == EatStatus.NotEdible, "Seeds are not edible");
        var saved = JsonSerializer.Deserialize<HungerSnapshot>(JsonSerializer.Serialize(s.Hunger(1)));
        Check(saved == s.Hunger(1), "Immutable hunger state serializes without engine references");

        var (shared, sharedClock) = Create(); shared.DepositAll(1, shared.Snapshot(1).Version); var token = Request(shared, 1);
        sharedClock.Advance(600);
        Check(shared.Hunger(1).MissedMeals == 1 && shared.Snapshot(1).Storage.Count("food") == 2, "Never auto-withdraw shared Food");
        Check(shared.Eat(1, token).Status == EatStatus.Stale, "Old-day eating request rejected even without a personal mutation");
        var (partial, pc) = Create(new(DailyFood: 2, StartingFood: 1));
        pc.Advance(600); Check(partial.Hunger(1).Consumed == 1 && partial.Hunger(1).MissedMeals == 1, "Partial personal supply consumed once; incomplete requirement misses");
        Add(partial, 1, "rations", 3); partial.Eat(1, Request(partial, 1));
        Check(partial.Hunger(1).MissedMeals == 1 && partial.Hunger(1).Eaten == 1, "Partial manual meal grants no premature recovery");
        pc.Advance(600); Check(partial.Hunger(1).MissedMeals == 0 && partial.Hunger(1).Consumed == 3, "Dawn fills only remaining requirement across edible types");
        var (fast, fc) = Create(new(RecoveryPerMeal: 2, StartingFood: 0)); fc.Advance(1800); Add(fast, 1, "rations", 1); fast.Eat(1, Request(fast, 1));
        Check(fast.Hunger(1).MissedMeals == 1, "Recovery rate is configurable");
        var (jump, jc) = Create(); var (step, sc) = Create(); jc.Advance(60000); for (int i = 0; i < 6000; i++) sc.Advance(10);
        Check(jump.Hunger(1) == step.Hunger(1) && jump.Hunger(1).Consumed == 2 && jump.Hunger(1).MissedMeals == 98,
            "Large day skips process every deadline identically to normal updates; penalty capped");
        var (fresh, _) = Create(); Check(fresh.Hunger(1).MissedMeals == 0 && fresh.Hunger(1).Day == 1 && fresh.Hunger(1).Consumed == 0 &&
            fresh.Eat(1, request).Status == EatStatus.Stale, "Reset clears debt/time and rejects prior session requests");
        try { fresh.InitializeHunger(new(), new(new())); throw new Exception("Repeated initialization accepted"); } catch (InvalidOperationException) { }
        try { new HungerDefinition(RecoveryPerMeal: 0).Validate(); throw new Exception("Invalid recovery accepted"); } catch (ArgumentException) { }
        var (empty, ec) = Create(new(StartingFood: 0));
        Check(empty.Eat(1, Request(empty, 1)).Status == EatStatus.Empty && empty.Hunger(1).Consumed == 0,
            "Empty personal pack cannot manufacture a meal");
        var untouched = empty.Snapshot(1).Version; ec.Advance(600);
        Check(empty.Snapshot(1).Version == untouched && empty.Eat(1, new(0, untouched, 1)).Status == EatStatus.Stale,
            "Day guard rejects old input even when no inventory version changed");
        var blocked = new SharedSupplies(new[] { new ResourceDefinition("food", "Food", "Food", true), new ResourceDefinition("filler", "Filler", "Filler") },
            new[] { new ResourceNodeDefinition("fill", "filler", 60) }, new[] { 1, 2 });
        Add(blocked, 2, "fill", 60); var blockedVersion = blocked.Snapshot(1).Version;
        try { blocked.InitializeHunger(new(), new(new())); throw new Exception("Starter overflow accepted"); } catch (ArgumentException) { }
        Check(blocked.Snapshot(1).Personal.Total == 0 && blocked.Snapshot(2).Personal.Total == 60 && blocked.Snapshot(1).Version == blockedVersion,
            "Starter allocation failure leaves every actor untouched");
        for (int n = 0; n < 64; n++)
        {
            var (race, rc) = Create(); var r = Request(race, 1); EatResult? a = null, b = null;
            Parallel.Invoke(() => a = race.Eat(1, r), () => b = race.Eat(1, r));
            Check(new[] { a!, b! }.Count(x => x.Status == EatStatus.Eaten) == 1 && race.Hunger(1).Consumed == 1, "Competing eat consumes once");
            var (timeRace, tc) = Create(); r = Request(timeRace, 1);
            Parallel.Invoke(() => timeRace.Eat(1, r), () => tc.Advance(600));
            Check(timeRace.Hunger(1).Consumed == 1 && timeRace.Snapshot(1).Personal.Count("food") == 1, "Eating at dawn cannot double consume");
            var (transferRace, _) = Create(); r = Request(transferRace, 1);
            Parallel.Invoke(() => transferRace.Eat(1, r), () => transferRace.DepositAll(1, r.Version));
            var snap = transferRace.Snapshot(1);
            Check(snap.Personal.Count("food") + snap.Storage.Count("food") + transferRace.Hunger(1).Consumed == 2, "Transfer/eat conflict conserves Food");
        }
        Console.WriteLine("PASS: hunger deadlines, manual/automatic meals, gradual/configurable recovery, partial meals, edible validation, reset, time skips, conservation and 64 concurrent eat/time/transfer races");
    }
}
