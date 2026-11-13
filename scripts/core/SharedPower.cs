using System;
using System.Collections.Generic;
using System.Linq;

namespace Hollowlight.Core;

public sealed partial class SharedSupplies
{
    private PowerDefinition? _power;
    private GameClock? _powerClock;
    private ConsumerDefinition[] _consumers = Array.Empty<ConsumerDefinition>();
    private readonly HashSet<string> _enabledConsumers = new(StringComparer.Ordinal);
    private decimal _tankCredits, _consumedCredits, _creditsPerFuel;
    private long _lastPowerTicks, _powerRevision;
    private bool _running;
    private int Demand => _consumers.Where(c => _enabledConsumers.Contains(c.Id)).Sum(c => c.Demand);
    private SupplyVersion PowerVersion => new(_session, _powerRevision);
    private decimal BurnRate => _power!.IdleFuelPerDay + _power.FuelPerPuDay * Demand;

    public void InitializePower(PowerDefinition definition, GameClock clock)
    {
        lock (_gate)
        {
            if (_power != null) throw new InvalidOperationException("Power initializes once per session.");
            definition.Validate();
            if ((_agriculture != null && !ReferenceEquals(Farm.Clock, clock)) || (_mealClock != null && !ReferenceEquals(_mealClock, clock)))
                throw new ArgumentException("Power must share the session clock.");
            if (!_resources.TryGetValue(definition.FuelResource, out var fuel) || fuel.Edible)
                throw new ArgumentException("Generator Fuel must be a defined non-edible resource.");
            if (_storage.RoomFor(fuel.Id) < definition.StartingFuel) throw new ArgumentException("Starter Fuel must fit communal storage.");
            clock.BindAuthority(_gate, AdvancePower);
            _power = definition; _powerClock = clock; _consumers = definition.Consumers.ToArray();
            _creditsPerFuel = (decimal)clock.CycleTicks * 1000000;
            _lastPowerTicks = clock.ElapsedTicks;
            _storage.Add(fuel.Id, definition.StartingFuel);
            if (definition.StartingFuel > 0) _revision++;
        }
    }
    public PowerSnapshot Power()
    {
        lock (_gate)
        {
            if (_power == null) throw new InvalidOperationException("Power is not initialized.");
            long? runtime = _running ? (long)Math.Min((decimal)long.MaxValue, decimal.Floor(_tankCredits / (BurnRate * 1000000))) : null;
            return new(PowerVersion, _running, _power.Capacity, Demand, _tankCredits / _creditsPerFuel,
                _power.TankCapacity, _consumedCredits / _creditsPerFuel, _running ? BurnRate : 0, runtime,
                Array.AsReadOnly(_consumers.Select(c => new ConsumerSnapshot(c.Id, c.Name, c.Demand,
                    _enabledConsumers.Contains(c.Id), _running && _enabledConsumers.Contains(c.Id))).ToArray()));
        }
    }
    public PowerFuelState PowerFuel()
    { lock (_gate) return new(_tankCredits, _consumedCredits, _creditsPerFuel, _lastPowerTicks); }
    public PowerResult ApplyPower(int playerId, PowerRequest request)
    {
        lock (_gate)
        {
            if (_power == null) throw new InvalidOperationException("Power is not initialized.");
            PowerResult Result(PowerStatus status, int amount = 0, int? demand = null) => new(status, amount, demand ?? Demand, _power.Capacity);
            if (!_players.TryGetValue(playerId, out var player)) return Result(PowerStatus.InvalidPlayer);
            if (request.Version != PowerVersion) return Result(PowerStatus.Stale);
            switch (request.Action)
            {
                case PowerAction.LoadFuel:
                    if (request.InventoryVersion != Version) return Result(PowerStatus.Stale);
                    if (request.Quantity < 1) return Result(PowerStatus.InvalidRequest);
                    int room = (int)decimal.Floor((_power.TankCapacity * _creditsPerFuel - _tankCredits) / _creditsPerFuel);
                    if (room == 0) return Result(PowerStatus.TankFull);
                    int amount = Math.Min(request.Quantity, Math.Min(room, player.Count(_power.FuelResource)));
                    if (amount == 0) return Result(PowerStatus.NoFuel);
                    player.Remove(_power.FuelResource, amount); _tankCredits += amount * _creditsPerFuel;
                    _revision++; _powerRevision++;
                    return Result(PowerStatus.Applied, amount);
                case PowerAction.SetRunning:
                    if (request.Enabled == _running) return Result(PowerStatus.NoChange);
                    if (request.Enabled && _tankCredits == 0) return Result(PowerStatus.NoFuel);
                    _running = request.Enabled; _powerRevision++; return Result(PowerStatus.Applied);
                case PowerAction.SetConsumer:
                    var consumer = _consumers.FirstOrDefault(c => c.Id == request.Consumer);
                    if (consumer == null) return Result(PowerStatus.UnknownConsumer);
                    if (_enabledConsumers.Contains(consumer.Id) == request.Enabled) return Result(PowerStatus.NoChange);
                    int wanted = Demand + (request.Enabled ? consumer.Demand : -consumer.Demand);
                    if (request.Enabled && wanted > _power.Capacity) return Result(PowerStatus.OverCapacity, demand: wanted);
                    if (request.Enabled && !_running) return Result(PowerStatus.NoSupply, demand: wanted);
                    if (request.Enabled) _enabledConsumers.Add(consumer.Id); else _enabledConsumers.Remove(consumer.Id);
                    _powerRevision++; return Result(PowerStatus.Applied);
                default: return Result(PowerStatus.InvalidRequest);
            }
        }
    }
    private void AdvancePower(long ticks)
    {
        long elapsed = ticks - _lastPowerTicks; _lastPowerTicks = ticks;
        if (!_running || elapsed == 0) return;
        decimal rate = BurnRate * 1000000;
        decimal burned = elapsed >= decimal.Ceiling(_tankCredits / rate) ? _tankCredits : (decimal)elapsed * rate;
        _tankCredits -= burned; _consumedCredits += burned;
        if (_tankCredits == 0) { _running = false; _powerRevision++; }
        // Burn never mutates item slots/revisions. A fuel-empty transition invalidates
        // power requests, while ordinary continuous burn leaves controls usable.
    }
}
