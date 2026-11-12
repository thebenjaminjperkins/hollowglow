using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Hollowlight.Core;

public sealed record ResourceDefinition(string Id, string Name, string ShortName);
public sealed record ResourceNodeDefinition(string Id, string ResourceId, int Quantity);
public sealed record ItemStack(string ResourceId, int Quantity);
public readonly record struct SupplyVersion(Guid Session, long Revision);
public enum SupplyStatus { Applied, Stale, InvalidPlayer, UnknownResource, UnknownNode, InvalidQuantity, NoCapacity, Empty, InvalidSlot, Incompatible, NoChange, AccessDenied }
public enum InventoryKind { Personal, Storage }
public readonly record struct SlotAddress(InventoryKind Container, int Index);
public enum SlotAction { Move, QuickTransfer, DepositAll }
public sealed record SlotRequest(SlotAction Action, SlotAddress Source, SlotAddress Destination, int Quantity, SupplyVersion Version);
public sealed record SupplyResult(SupplyStatus Status, int Amount, SupplyVersion Version);
public sealed record InventorySnapshot(IReadOnlyList<ItemStack?> Slots)
{
    public int Capacity => Slots.Count;
    public int UsedSlots => Slots.Count(slot => slot != null);
    public int Total => Slots.Sum(slot => slot?.Quantity ?? 0);
    public int Count(string resource) => Slots.Where(slot => slot?.ResourceId == resource).Sum(slot => slot!.Quantity);
}
public sealed record SuppliesSnapshot(SupplyVersion Version, InventorySnapshot Personal,
    InventorySnapshot Storage, IReadOnlyDictionary<string, int> Remaining);

// One authoritative owner. All validation and both sides of a mutation share one lock.
// A session/revision token makes competing or retried commands reject stale state.
// Networking/authentication is deliberately outside this engine-independent model.
public sealed class SharedSupplies
{
    public const int PlayerSlots = 6;
    public const int StackLimit = 10;
    private readonly object _gate = new();
    private readonly Guid _session = Guid.NewGuid();
    private long _revision;
    private readonly Dictionary<string, ResourceDefinition> _resources;
    private readonly Dictionary<string, ResourceNodeDefinition> _nodes;
    private readonly Dictionary<string, int> _remaining;
    private readonly Dictionary<int, Inventory> _players = new();
    private readonly Inventory _storage;
    public IReadOnlyList<ResourceDefinition> Resources { get; }
    private SupplyVersion Version => new(_session, _revision);

    public SharedSupplies(IEnumerable<ResourceDefinition> resources, IEnumerable<ResourceNodeDefinition> nodes,
        IEnumerable<int> playerIds, int storageSlots = 24)
    {
        var definitions = resources.ToArray();
        if (definitions.Length == 0 || definitions.Any(r => string.IsNullOrWhiteSpace(r.Id) ||
            string.IsNullOrWhiteSpace(r.Name) || string.IsNullOrWhiteSpace(r.ShortName)))
            throw new ArgumentException("Resources need unique IDs and readable names.");
        _resources = definitions.ToDictionary(r => r.Id, StringComparer.Ordinal);
        Resources = Array.AsReadOnly(definitions);
        _nodes = nodes.ToDictionary(n => n.Id, StringComparer.Ordinal);
        if (_nodes.Values.Any(n => string.IsNullOrWhiteSpace(n.Id) || n.Quantity <= 0 || !_resources.ContainsKey(n.ResourceId)))
            throw new ArgumentException("Nodes need positive finite quantities and defined resources.");
        _remaining = _nodes.ToDictionary(n => n.Key, n => n.Value.Quantity);
        _storage = new Inventory(storageSlots);
        foreach (int id in playerIds)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(playerIds));
            _players.Add(id, new Inventory(PlayerSlots));
        }
        if (_players.Count == 0) throw new ArgumentException("At least one actor is required.");
    }

    public SuppliesSnapshot Snapshot(int playerId)
    {
        lock (_gate)
        {
            if (!_players.TryGetValue(playerId, out var player)) throw new ArgumentOutOfRangeException(nameof(playerId));
            return new(Version, player.Snapshot(), _storage.Snapshot(),
                new ReadOnlyDictionary<string, int>(new Dictionary<string, int>(_remaining)));
        }
    }

    public SupplyResult Gather(int playerId, string nodeId, int requested, SupplyVersion expected)
    {
        lock (_gate)
        {
            if (expected != Version) return Result(SupplyStatus.Stale);
            if (!_players.TryGetValue(playerId, out var player)) return Result(SupplyStatus.InvalidPlayer);
            if (requested <= 0) return Result(SupplyStatus.InvalidQuantity);
            if (!_nodes.TryGetValue(nodeId, out var node)) return Result(SupplyStatus.UnknownNode);
            int available = _remaining[nodeId];
            if (available == 0) return Result(SupplyStatus.Empty);
            int amount = Math.Min(requested, Math.Min(available, player.RoomFor(node.ResourceId)));
            if (amount == 0) return Result(SupplyStatus.NoCapacity);
            player.Add(node.ResourceId, amount);
            _remaining[nodeId] -= amount;
            return Applied(amount);
        }
    }

    public SupplyResult Transfer(int playerId, string resourceId, int requested, bool withdraw, SupplyVersion expected)
    {
        lock (_gate)
        {
            if (expected != Version) return Result(SupplyStatus.Stale);
            if (!_players.TryGetValue(playerId, out var player)) return Result(SupplyStatus.InvalidPlayer);
            if (requested <= 0) return Result(SupplyStatus.InvalidQuantity);
            if (!_resources.ContainsKey(resourceId)) return Result(SupplyStatus.UnknownResource);
            Inventory source = withdraw ? _storage : player, destination = withdraw ? player : _storage;
            if (source.Count(resourceId) == 0) return Result(SupplyStatus.Empty);
            int amount = Math.Min(requested, Math.Min(source.Count(resourceId), destination.RoomFor(resourceId)));
            if (amount == 0) return Result(SupplyStatus.NoCapacity);
            source.Remove(resourceId, amount);
            destination.Add(resourceId, amount);
            return Applied(amount);
        }
    }

    public SupplyResult DepositAll(int playerId, SupplyVersion expected)
    {
        lock (_gate)
        {
            if (expected != Version) return Result(SupplyStatus.Stale);
            if (!_players.TryGetValue(playerId, out var player)) return Result(SupplyStatus.InvalidPlayer);
            int amount = 0;
            foreach (var resource in Resources)
            {
                int moved = Math.Min(player.Count(resource.Id), _storage.RoomFor(resource.Id));
                player.Remove(resource.Id, moved);
                _storage.Add(resource.Id, moved);
                amount += moved;
            }
            if (amount > 0) return Applied(amount);
            return Result(player.Total == 0 ? SupplyStatus.Empty : SupplyStatus.NoCapacity);
        }
    }

    public SupplyResult ApplySlots(int playerId, SlotRequest request)
    {
        lock (_gate)
        {
            if (request.Version != Version) return Result(SupplyStatus.Stale);
            if (!_players.TryGetValue(playerId, out var player)) return Result(SupplyStatus.InvalidPlayer);
            if (request.Action == SlotAction.DepositAll) return DepositAll(playerId, request.Version);
            Inventory? source = Container(player, request.Source.Container);
            if (source == null || !source.Valid(request.Source.Index)) return Result(SupplyStatus.InvalidSlot);
            ItemStack? stack = source.At(request.Source.Index);
            if (stack == null) return Result(SupplyStatus.Empty);
            if (request.Quantity <= 0) return Result(SupplyStatus.InvalidQuantity);
            int wanted = Math.Min(request.Quantity, stack.Quantity);
            if (request.Action == SlotAction.QuickTransfer)
            {
                Inventory other = request.Source.Container == InventoryKind.Personal ? _storage : player;
                int moved = Math.Min(wanted, other.RoomFor(stack.ResourceId));
                if (moved == 0) return Result(SupplyStatus.NoCapacity);
                source.Take(request.Source.Index, moved);
                other.Add(stack.ResourceId, moved);
                return Applied(moved);
            }
            if (request.Action != SlotAction.Move) return Result(SupplyStatus.InvalidSlot);
            Inventory? destination = Container(player, request.Destination.Container);
            if (destination == null || !destination.Valid(request.Destination.Index)) return Result(SupplyStatus.InvalidSlot);
            if (source == destination && request.Source.Index == request.Destination.Index) return Result(SupplyStatus.NoChange);
            var target = destination.At(request.Destination.Index);
            if (target != null && target.ResourceId != stack.ResourceId)
            {
                if (wanted != stack.Quantity) return Result(SupplyStatus.Incompatible);
                source.Set(request.Source.Index, target);
                destination.Set(request.Destination.Index, stack);
                return Applied(wanted);
            }
            int amount = Math.Min(wanted, StackLimit - (target?.Quantity ?? 0));
            if (amount == 0) return Result(SupplyStatus.NoCapacity);
            source.Take(request.Source.Index, amount);
            destination.Set(request.Destination.Index, new(stack.ResourceId, (target?.Quantity ?? 0) + amount));
            return Applied(amount);
        }
    }

    private Inventory? Container(Inventory player, InventoryKind kind) => kind switch {
        InventoryKind.Personal => player, InventoryKind.Storage => _storage, _ => null
    };

    private SupplyResult Result(SupplyStatus status) => new(status, 0, Version);
    private SupplyResult Applied(int amount) { _revision++; return new(SupplyStatus.Applied, amount, Version); }

    private sealed class Inventory
    {
        private readonly ItemStack?[] _slots;
        public Inventory(int slots)
        {
            if (slots < 1 || slots > 256) throw new ArgumentOutOfRangeException(nameof(slots), "Use 1–256 storage slots.");
            _slots = new ItemStack?[slots];
        }
        public bool Valid(int index) => index >= 0 && index < _slots.Length;
        public ItemStack? At(int index) => _slots[index];
        public void Set(int index, ItemStack? value) => _slots[index] = value;
        public void Take(int index, int amount)
        {
            var slot = _slots[index]!;
            _slots[index] = amount == slot.Quantity ? null : slot with { Quantity = slot.Quantity - amount };
        }
        public int Total => _slots.Sum(s => s?.Quantity ?? 0);
        public int Count(string id) => _slots.Where(s => s?.ResourceId == id).Sum(s => s!.Quantity);
        public int RoomFor(string id) => _slots.Sum(s => s == null ? StackLimit : s.ResourceId == id ? StackLimit - s.Quantity : 0);
        public InventorySnapshot Snapshot() => new(Array.AsReadOnly((ItemStack?[])_slots.Clone()));
        public void Add(string id, int amount)
        {
            // Capacity was computed under the same owner lock. Fill partial stacks first.
            for (int pass = 0; pass < 2 && amount > 0; pass++)
                for (int i = 0; i < _slots.Length && amount > 0; i++)
                {
                    var slot = _slots[i];
                    if (pass == 0 ? slot?.ResourceId != id : slot != null) continue;
                    int moved = Math.Min(amount, StackLimit - (slot?.Quantity ?? 0));
                    _slots[i] = new(id, (slot?.Quantity ?? 0) + moved);
                    amount -= moved;
                }
        }
        public void Remove(string id, int amount)
        {
            for (int i = _slots.Length - 1; i >= 0 && amount > 0; i--)
            {
                var slot = _slots[i];
                if (slot?.ResourceId != id) continue;
                int moved = Math.Min(amount, slot.Quantity);
                _slots[i] = moved == slot.Quantity ? null : slot with { Quantity = slot.Quantity - moved };
                amount -= moved;
            }
        }
    }
}
