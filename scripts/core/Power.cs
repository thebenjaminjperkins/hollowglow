using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Hollowlight.Core;

public sealed record ConsumerDefinition(string Id, string Name, int Demand);
public sealed record PowerDefinition(int Capacity, int TankCapacity, int StartingFuel, string FuelResource,
    decimal IdleFuelPerDay, decimal FuelPerPuDay, ConsumerDefinition[] Consumers)
{
    public static PowerDefinition Parse(string json) => JsonSerializer.Deserialize<PowerDefinition>(json,
        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? throw new JsonException("Missing power configuration.");
    public void Validate()
    {
        if (Capacity is < 1 or > 100 || TankCapacity is < 1 or > 100 || StartingFuel is < 0 or > 2560 ||
            string.IsNullOrWhiteSpace(FuelResource) || IdleFuelPerDay <= 0 || FuelPerPuDay < 0 ||
            IdleFuelPerDay > 100 || FuelPerPuDay > 100 || IdleFuelPerDay * 1000000 != decimal.Truncate(IdleFuelPerDay * 1000000) ||
            FuelPerPuDay * 1000000 != decimal.Truncate(FuelPerPuDay * 1000000) || Consumers.Length is < 1 or > 16 ||
            Consumers.Select(c => c.Id).Distinct().Count() != Consumers.Length || Consumers.Any(c =>
                string.IsNullOrWhiteSpace(c.Id) || string.IsNullOrWhiteSpace(c.Name) || c.Demand < 1 || c.Demand > Capacity))
            throw new ArgumentException("Invalid power definition.");
    }
}
public sealed record ConsumerSnapshot(string Id, string Name, int Demand, bool Enabled, bool Powered);
public sealed record PowerSnapshot(SupplyVersion Version, bool Running, int Capacity, int Demand, decimal Fuel,
    decimal TankCapacity, decimal Consumed, decimal RatePerDay, long? RuntimeTicks, IReadOnlyList<ConsumerSnapshot> Consumers)
{
    public int Available => Running ? Capacity - Demand : 0;
}
public enum PowerAction { SetRunning, SetConsumer, LoadFuel }
public sealed record PowerRequest(PowerAction Action, SupplyVersion Version, SupplyVersion InventoryVersion,
    bool Enabled = false, string Consumer = "", int Quantity = 1);
public enum PowerStatus { Applied, NoChange, Stale, InvalidPlayer, InvalidRequest, UnknownConsumer, NoFuel, TankFull, NoSupply, OverCapacity, AccessDenied }
public sealed record PowerResult(PowerStatus Status, int Amount, int RequestedDemand, int Capacity);
// Exact integer credit counters, suitable for serialization. One Fuel = cycle ticks * 1,000,000 credits.
public sealed record PowerFuelState(decimal TankCredits, decimal ConsumedCredits, decimal CreditsPerFuel, long LastTicks);
