using System;
using System.Linq;
using System.Threading.Tasks;
using Hollowlight.Core;

public static class SlotOperationsTests
{
    private static readonly SlotAddress P0 = new(InventoryKind.Personal, 0), P1 = new(InventoryKind.Personal, 1), P2 = new(InventoryKind.Personal, 2);
    private static SharedSupplies Create(int capacity = 24) => new(
        new[] { new ResourceDefinition("scrap", "Scrap", "Scrap"), new ResourceDefinition("organic", "Organic", "Organic") },
        new[] { new ResourceNodeDefinition("metal", "scrap", 200), new ResourceNodeDefinition("growth", "organic", 200) }, new[] { 1, 2 }, capacity);
    private static void Check(bool value, string message) { if (!value) throw new Exception(message); }
    private static SupplyResult Move(SharedSupplies s, SlotAddress src, SlotAddress dst, int quantity, int actor = 1) =>
        s.ApplySlots(actor, new(SlotAction.Move, src, dst, quantity, s.Snapshot(actor).Version));
    private static void Gather(SharedSupplies s, string node, int quantity, int actor = 1) => s.Gather(actor, node, quantity, s.Snapshot(actor).Version);
    private static void Conservation(SharedSupplies s)
    {
        var a = s.Snapshot(1); var b = s.Snapshot(2);
        foreach (string id in new[] { "scrap", "organic" })
        {
            int left = a.Remaining[id == "scrap" ? "metal" : "growth"];
            Check(left + a.Personal.Count(id) + b.Personal.Count(id) + a.Storage.Count(id) == 200, "Slot operations conserve every resource");
        }
        foreach (var slot in a.Personal.Slots.Concat(b.Personal.Slots).Concat(a.Storage.Slots))
            Check(slot == null || slot.Quantity > 0 && slot.Quantity <= 10, "Slot operations keep stack bounds");
    }
    public static void Run()
    {
        var state = Create(); Gather(state, "metal", 12);
        Check(Move(state, P0, P2, 4).Amount == 4 && state.Snapshot(1).Personal.Slots[0]?.Quantity == 6, "Split into chosen empty slot");
        Check(Move(state, P2, P1, 4).Amount == 4 && state.Snapshot(1).Personal.Slots[2] == null, "Merge complete source into matching stack");
        Check(Move(state, P1, P0, int.MaxValue).Amount == 4 && state.Snapshot(1).Personal.Slots[1]?.Quantity == 2, "Partial merge retains source remainder");
        Gather(state, "growth", 8);
        var before = state.Snapshot(1).Version;
        Check(Move(state, P1, P2, 1).Status == SupplyStatus.Incompatible && state.Snapshot(1).Version == before, "Partial different-resource swap rejected atomically");
        Check(Move(state, P1, P2, 2).Amount == 2 && state.Snapshot(1).Personal.Slots[1]?.ResourceId == "organic", "Whole stacks swap");
        Check(Move(state, P0, P0, 10).Status == SupplyStatus.NoChange, "Same slot is a no-op");
        Check(Move(state, P0, new(InventoryKind.Personal, 6), 1).Status == SupplyStatus.InvalidSlot, "Out-of-range destination rejected");
        Check(Move(state, new(InventoryKind.Personal, -1), P0, 1).Status == SupplyStatus.InvalidSlot, "Invalid source rejected");
        Check(Move(state, P0, new((InventoryKind)99, 0), 1).Status == SupplyStatus.InvalidSlot, "Unknown container rejected");
        Check(Move(state, P0, P2, 0).Status == SupplyStatus.InvalidQuantity, "Zero quantity rejected");
        Check(Move(state, new(InventoryKind.Personal, 5), P2, 1).Status == SupplyStatus.Empty, "Empty source rejected");
        var chest0 = new SlotAddress(InventoryKind.Storage, 0);
        Check(Move(state, P1, chest0, 3).Amount == 3, "Split to chosen storage slot");
        Check(Move(state, P0, chest0, 10).Amount == 10 && state.Snapshot(1).Personal.Slots[0]?.ResourceId == "organic", "Cross-container swap preserves both ends");
        var token = state.Snapshot(1).Version;
        var request = new SlotRequest(SlotAction.Move, P0, new(InventoryKind.Personal, 5), 1, token);
        Check(state.ApplySlots(1, request).Amount == 1 && state.ApplySlots(1, request).Status == SupplyStatus.Stale, "Accepted request cannot replay");
        Conservation(state);

        var full = Create(1); Gather(full, "metal", 12);
        Check(full.ApplySlots(1, new(SlotAction.QuickTransfer, P0, default, 10, full.Snapshot(1).Version)).Amount == 10, "Quick transfer uses selected stack");
        Check(full.ApplySlots(1, new(SlotAction.QuickTransfer, P1, default, 10, full.Snapshot(1).Version)).Status == SupplyStatus.NoCapacity, "Full chest leaves quick-transfer source intact");
        Check(Move(full, chest0, P0, 7).Amount == 7, "Targeted withdrawal splits storage stack");
        Check(full.ApplySlots(1, new(SlotAction.QuickTransfer, P0, default, 10, full.Snapshot(1).Version)).Amount == 7, "Quick transfer fills partial matching stack first");
        Conservation(full);

        for (int i = 0; i < 32; i++)
        {
            var race = Create(); Gather(race, "metal", 10); race.DepositAll(1, race.Snapshot(1).Version);
            var version = race.Snapshot(1).Version; SupplyResult? a = null, b = null;
            Parallel.Invoke(() => a = race.ApplySlots(1, new(SlotAction.Move, chest0, P0, 7, version)),
                () => b = race.ApplySlots(2, new(SlotAction.Move, chest0, P0, 7, version)));
            Check(new[] { a!, b! }.Count(r => r.Status == SupplyStatus.Applied) == 1 && new[] { a!, b! }.Count(r => r.Status == SupplyStatus.Stale) == 1,
                "Conflicting split withdrawals have one winner");
            Conservation(race);
        }
        var random = new Random(611); var mixed = Create(4);
        Gather(mixed, "metal", 45); Gather(mixed, "growth", 10); Gather(mixed, "growth", 55, 2);
        for (int i = 0; i < 3000; i++)
        {
            int actor = random.Next(1, 3);
            var src = new SlotAddress((InventoryKind)random.Next(2), random.Next(6));
            var dst = new SlotAddress((InventoryKind)random.Next(2), random.Next(6));
            mixed.ApplySlots(actor, new((SlotAction)random.Next(3), src, dst, random.Next(1, 16), mixed.Snapshot(actor).Version));
            Conservation(mixed);
        }
        var fresh = Create();
        Check(fresh.ApplySlots(1, request).Status == SupplyStatus.Stale, "Reset invalidates slot previews and commands");
        Console.WriteLine("PASS: slot moves, partial merges, splits, whole-stack swaps, quick transfers, invalid requests, stale/replayed commands, 32 split races and 3000 conservation checks");
    }
}
