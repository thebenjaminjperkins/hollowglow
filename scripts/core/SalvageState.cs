using System;
using System.Collections.Generic;

namespace Hollowlight.Core;

// Owned by the world, never by input or presentation. Object IDs make repeat pickup idempotent.
public sealed class SalvageState
{
    private readonly Dictionary<string, int> _remaining;
    private readonly Dictionary<int, int> _carried = new();
    public int Stored { get; private set; }

    public SalvageState(IReadOnlyDictionary<string, int> objects)
    {
        _remaining = new Dictionary<string, int>();
        foreach (var pair in objects)
        {
            if (pair.Value <= 0) throw new ArgumentOutOfRangeException(nameof(objects));
            _remaining.Add(pair.Key, pair.Value);
        }
    }

    public int CarriedBy(int playerId) => _carried.GetValueOrDefault(playerId);
    public bool IsAvailable(string objectId) => _remaining.ContainsKey(objectId);

    public bool TryCollect(int playerId, string objectId)
    {
        if (!_remaining.TryGetValue(objectId, out int amount)) return false;
        int total = checked(CarriedBy(playerId) + amount);
        _remaining.Remove(objectId);
        _carried[playerId] = total;
        return true;
    }

    public int Deposit(int playerId)
    {
        int amount = CarriedBy(playerId);
        Stored = checked(Stored + amount);
        _carried.Remove(playerId);
        return amount;
    }
}
