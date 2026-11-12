using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Hollowlight.Core;

public static class SuppliesTests
{
    private static readonly ResourceDefinition[] Resources = {
        new("scrap", "Scrap", "Scrap"), new("organic", "Organic Material", "Organic"), new("component", "Component", "Part") };
    private static SharedSupplies State(int slots = 24) => new(Resources,
        new[] { new ResourceNodeDefinition("metal", "scrap", 200), new ResourceNodeDefinition("growth", "organic", 200),
            new ResourceNodeDefinition("part", "component", 1) }, new[] { 1, 2 }, slots);
    private static void Check(bool condition, string message) { if (!condition) throw new Exception(message); }
    private static SupplyResult Gather(SharedSupplies s, int player, string node, int amount) => s.Gather(player, node, amount, s.Snapshot(player).Version);
    private static SupplyResult Move(SharedSupplies s, int player, string resource, int amount, bool take = false) =>
        s.Transfer(player, resource, amount, take, s.Snapshot(player).Version);
    private static void Invariants(SharedSupplies state, IReadOnlyDictionary<string, int> initial)
    {
        var a = state.Snapshot(1); var b = state.Snapshot(2);
        foreach (var inventory in new[] { a.Personal, b.Personal, a.Storage })
            foreach (var slot in inventory.Slots)
                Check(slot == null || slot.Quantity > 0 && slot.Quantity <= 10, "Every occupied slot holds 1–10 units");
        Check(a.Personal.Capacity == 6 && b.Personal.Capacity == 6, "Each actor has exactly six slots");
        foreach (var pair in initial)
        {
            int remaining = pair.Key == "scrap" ? a.Remaining["metal"] : pair.Key == "organic" ? a.Remaining["growth"] : a.Remaining["part"];
            Check(remaining >= 0 && remaining + a.Personal.Count(pair.Key) + b.Personal.Count(pair.Key) + a.Storage.Count(pair.Key) == pair.Value,
                $"Conservation for {pair.Key}");
        }
    }

    public static void Run()
    {
        var state = State();
        Check(Gather(state, 1, "metal", 59).Amount == 59, "Fill partial sixth slot");
        var saved = state.Snapshot(1);
        Check(saved.Personal.UsedSlots == 6 && saved.Personal.Slots[5]?.Quantity == 9, "Stack packing");
        Check(Gather(state, 1, "growth", 4).Status == SupplyStatus.NoCapacity, "Partial scrap slot cannot hold another resource");
        Check(Gather(state, 1, "metal", 9).Amount == 1 && state.Snapshot(1).Remaining["metal"] == 140, "Partial gathering leaves overflow in node");
        Check(Gather(state, 1, "metal", 1).Status == SupplyStatus.NoCapacity, "Full personal inventory rejects gathering");
        Check(saved.Personal.Total == 59 && saved.Remaining["metal"] == 141, "Snapshots remain immutable after mutations");
        Check(Gather(state, 2, "growth", 8).Amount == 8 && state.Snapshot(2).Personal.Count("scrap") == 0, "Inventories belong to actors");
        Check(Gather(state, 2, "part", 1).Amount == 1, "Definitions support a third resource without special-case inventory code");
        Check(Gather(state, 2, "part", 1).Status == SupplyStatus.Empty, "Depleted node rejects fresh attempts");
        Invariants(state, new Dictionary<string, int> { ["scrap"] = 200, ["organic"] = 200, ["component"] = 1 });

        var small = State(1);
        Gather(small, 1, "metal", 15);
        Check(small.DepositAll(1, small.Snapshot(1).Version).Amount == 10, "Bulk deposit partially fits configured storage");
        Check(small.Snapshot(1).Personal.Total == 5 && small.Snapshot(1).Storage.Total == 10, "Deposit remainder stays personal");
        Gather(small, 1, "growth", 8);
        var full = small.Snapshot(1);
        Check(small.DepositAll(1, full.Version).Status == SupplyStatus.NoCapacity, "Full storage rejects deposit without loss");
        Check(Move(small, 1, "organic", 1).Status == SupplyStatus.NoCapacity && small.Snapshot(1).Version == full.Version, "Failed transfer does not mutate revision");
        Check(Move(small, 1, "scrap", int.MaxValue, true).Amount == 10, "Withdrawal limited to available stock");
        Check(Move(small, 1, "organic", 7).Amount == 7, "Deposit selected quantity");
        Check(Move(small, 1, "scrap", 1).Status == SupplyStatus.NoCapacity, "Resources cannot mix in one slot");
        Check(Move(small, 1, "organic", 10).Amount == 1 && small.Snapshot(1).Storage.Count("organic") == 8, "Source-limited partial transfer");
        Gather(small, 1, "growth", 8);
        Check(Move(small, 1, "organic", 10).Amount == 2 && small.Snapshot(1).Personal.Count("organic") == 6,
            "Partial selected deposit fills matching chest stack and retains remainder");
        Invariants(small, new Dictionary<string, int> { ["scrap"] = 200, ["organic"] = 200, ["component"] = 1 });

        var partial = State();
        Gather(partial, 1, "metal", 60); partial.DepositAll(1, partial.Snapshot(1).Version);
        Gather(partial, 2, "growth", 55);
        Check(Move(partial, 2, "scrap", 20, true).Status == SupplyStatus.NoCapacity, "Full recipient rejects withdrawal");
        Move(partial, 2, "organic", 10);
        Check(Move(partial, 2, "scrap", 20, true).Amount == 10 && partial.Snapshot(2).Storage.Count("scrap") == 50, "Partial withdrawal preserves chest remainder");
        var old = partial.Snapshot(2).Version;
        Check(Move(partial, 2, "scrap", 1).Amount == 1, "Single-unit deposit");
        Check(partial.Transfer(2, "scrap", 1, false, old).Status == SupplyStatus.Stale, "Retried transfer cannot apply twice");
        Check(partial.Transfer(99, "scrap", 1, true, partial.Snapshot(1).Version).Status == SupplyStatus.InvalidPlayer, "Unknown actor rejected");
        Check(Move(partial, 1, "missing", 1).Status == SupplyStatus.UnknownResource, "Unknown resource rejected");
        Check(Move(partial, 1, "scrap", 0).Status == SupplyStatus.InvalidQuantity && Move(partial, 1, "scrap", -1).Status == SupplyStatus.InvalidQuantity, "Nonpositive transfers rejected");
        Check(Gather(partial, 1, "missing", 1).Status == SupplyStatus.UnknownNode && Gather(partial, 1, "metal", 0).Status == SupplyStatus.InvalidQuantity, "Invalid gathering rejected");
        Invariants(partial, new Dictionary<string, int> { ["scrap"] = 200, ["organic"] = 200, ["component"] = 1 });

        for (int round = 0; round < 32; round++)
        {
            var race = State(); var version = race.Snapshot(1).Version;
            SupplyResult? first = null, second = null;
            Parallel.Invoke(() => first = race.Gather(1, "part", 1, version), () => second = race.Gather(2, "part", 1, version));
            Check(new[] { first!, second! }.Count(r => r.Status == SupplyStatus.Applied) == 1 &&
                new[] { first!, second! }.Count(r => r.Status == SupplyStatus.Stale) == 1, "Conflicting gather has exactly one winner");
            Gather(race, 1, "metal", 20); race.DepositAll(1, race.Snapshot(1).Version);
            version = race.Snapshot(1).Version;
            Parallel.Invoke(() => first = race.Transfer(1, "scrap", 15, true, version), () => second = race.Transfer(2, "scrap", 15, true, version));
            Check(new[] { first!, second! }.Count(r => r.Status == SupplyStatus.Applied) == 1, "Conflicting withdrawal has exactly one winner");
            int loser = first!.Status == SupplyStatus.Stale ? 1 : 2;
            Check(Move(race, loser, "scrap", 15, true).Amount == 5, "Loser can retry against fresh stock and gets only remainder");
            Invariants(race, new Dictionary<string, int> { ["scrap"] = 200, ["organic"] = 200, ["component"] = 1 });
        }

        var competingDeposits = State(1);
        Gather(competingDeposits, 1, "metal", 8); Gather(competingDeposits, 2, "growth", 8);
        var depositVersion = competingDeposits.Snapshot(1).Version;
        SupplyResult? depositA = null, depositB = null;
        Parallel.Invoke(() => depositA = competingDeposits.DepositAll(1, depositVersion),
            () => depositB = competingDeposits.DepositAll(2, depositVersion));
        Check(new[] { depositA!, depositB! }.Count(r => r.Status == SupplyStatus.Applied) == 1, "Competing deposits cannot overfill chest");
        int rejected = depositA!.Status == SupplyStatus.Stale ? 1 : 2;
        Check(competingDeposits.DepositAll(rejected, competingDeposits.Snapshot(rejected).Version).Status == SupplyStatus.NoCapacity,
            "Conflicting resource stays with its actor when last chest slot is taken");
        Invariants(competingDeposits, new Dictionary<string, int> { ["scrap"] = 200, ["organic"] = 200, ["component"] = 1 });

        var random = new Random(1701); var mixed = State(3);
        for (int i = 0; i < 2000; i++)
        {
            int actor = random.Next(1, 3), amount = random.Next(1, 90);
            string resource = random.Next(2) == 0 ? "scrap" : "organic";
            switch (random.Next(4))
            {
                case 0: Gather(mixed, actor, resource == "scrap" ? "metal" : "growth", amount); break;
                case 1: Move(mixed, actor, resource, amount); break;
                case 2: Move(mixed, actor, resource, amount, true); break;
                case 3: mixed.DepositAll(actor, mixed.Snapshot(actor).Version); break;
            }
            Invariants(mixed, new Dictionary<string, int> { ["scrap"] = 200, ["organic"] = 200, ["component"] = 1 });
        }
        var fresh = State();
        Check(fresh.Snapshot(1).Personal.Total == 0 && fresh.Snapshot(1).Storage.Total == 0 && fresh.Snapshot(1).Remaining["metal"] == 200, "New session restores all finite stock");
        Check(fresh.Transfer(1, "scrap", 1, true, mixed.Snapshot(1).Version).Status == SupplyStatus.Stale, "Pre-reset command cannot mutate a fresh session");
        Check(Move(fresh, 1, "scrap", 1, true).Status == SupplyStatus.Empty, "Reset chest cannot supply old stock");
        Check(fresh.Gather(1, "metal", 1, State().Snapshot(1).Version).Status == SupplyStatus.Stale, "Session token rejects old commands even at equal revision zero");
        foreach (int invalid in new[] { 0, -1, 257 })
        {
            try { State(invalid); throw new Exception("Invalid storage capacity accepted"); }
            catch (ArgumentOutOfRangeException) { }
        }
        var setup = SupplySetup.Parse(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "data/supplies.json")));
        Check(setup.Resources.Length == 3 && setup.CreateState(new[] { 1, 2 }, 24).Resources.Count == 3, "Actual catalog includes Scrap, Organic and edible Food");
        Console.WriteLine("PASS: six-slot/ten-stack limits, typed resources, partial transfers, full storage, finite gathering, resets, stale/replayed commands, 32 two-actor races, 2000 conservation checks");
    }
}
