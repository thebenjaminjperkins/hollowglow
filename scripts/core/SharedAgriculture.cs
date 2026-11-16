using System;
using System.Collections.Generic;
using System.Linq;

namespace Hollowlight.Core;

public sealed partial class SharedSupplies
{
    private Agriculture? _agriculture;
    private Agriculture Farm => _agriculture ?? throw new InvalidOperationException("Farming is not initialized.");
    public void InitializeAgriculture(AgricultureSetup setup, GameClock clock, IEnumerable<string> plotIds)
    {
        lock (_gate)
        {
            if (_agriculture != null) throw new InvalidOperationException("Farming is initialized once per session.");
            if (_mealClock != null && !ReferenceEquals(_mealClock, clock))
                throw new ArgumentException("Farming and meals must share the session clock.");
            if (_powerClock != null && !ReferenceEquals(_powerClock, clock)) throw new ArgumentException("Farming must share the power clock.");
            var candidate = new Agriculture(setup, clock, plotIds);
            foreach (var crop in candidate.Crops)
                if (!_resources.ContainsKey(crop.SeedResource) || crop.Yields.Keys.Any(id => !_resources.ContainsKey(id)))
                    throw new ArgumentException("Crops refer to undefined resources.");
            if (setup.StartingStorage.Any(p => !_resources.ContainsKey(p.Key) || p.Value < 1))
                throw new ArgumentException("Invalid starter stock.");
            var stock = setup.StartingStorage.Select(p => new ItemStack(p.Key, p.Value)).ToArray();
            if (!_storage.CanAddAll(stock)) throw new ArgumentException("Starter seeds do not fit communal storage; farming needs at least two slots with the default setup.");
            foreach (var item in stock) _storage.Add(item.ResourceId, item.Quantity);
            _agriculture = candidate;
            if (stock.Length > 0) _revision++;
        }
    }
    public IReadOnlyList<CropDefinition> Crops { get { lock (_gate) return Farm.Crops; } }
    public PlotSnapshot Plot(string id) { lock (_gate) return Farm.Snapshot(id, _session); }
    public IReadOnlyList<PlotSnapshot> Plots()
    {
        lock (_gate) return Array.AsReadOnly(Farm.Plots.Keys.Select(id => Farm.Snapshot(id, _session)).ToArray());
    }
    public FarmResult Plant(int playerId, string plotId, string cropId, SupplyVersion expected)
    {
        lock (_gate)
        {
            if (RunCompleted) return new(FarmStatus.AccessDenied,expected);
            if (!_players.TryGetValue(playerId, out var player)) return new(FarmStatus.InvalidPlayer, expected);
            if (!Farm.Plots.TryGetValue(plotId, out var plot)) return new(FarmStatus.UnknownPlot, expected);
            var version = new SupplyVersion(_session, plot.Revision);
            if (expected != version) return new(FarmStatus.Stale, version);
            if (plot.CropId != null) return new(FarmStatus.Occupied, version);
            if (!Farm.TryCrop(cropId, out var crop)) return new(FarmStatus.UnknownCrop, version);
            if (player.Count(crop.SeedResource) < crop.SeedCost) return new(FarmStatus.NoSeed, version);
            _ = checked(Farm.Clock.ElapsedTicks + Farm.Duration(crop));
            player.Remove(crop.SeedResource, crop.SeedCost);
            plot.CropId = crop.Id; plot.PlantedTicks = Farm.Clock.ElapsedTicks; plot.Revision++;
            _revision++;
            return new(FarmStatus.Planted, new(_session, plot.Revision));
        }
    }
    public FarmResult Harvest(int playerId, string plotId, SupplyVersion expected)
    {
        lock (_gate)
        {
            if (RunCompleted) return new(FarmStatus.AccessDenied,expected);
            if (!_players.TryGetValue(playerId, out var player)) return new(FarmStatus.InvalidPlayer, expected);
            if (!Farm.Plots.TryGetValue(plotId, out var plot)) return new(FarmStatus.UnknownPlot, expected);
            var state = Farm.Snapshot(plotId, _session);
            if (expected != state.Version) return new(FarmStatus.Stale, state.Version);
            if (state.CropId == null) return new(FarmStatus.Empty, state.Version);
            if (state.Stage != CropStage.Mature) return new(FarmStatus.NotMature, state.Version);
            Farm.TryCrop(state.CropId, out var crop);
            var yield = crop.Yields.Select(p => new ItemStack(p.Key, p.Value)).ToArray();
            if (!player.CanAddAll(yield)) return new(FarmStatus.NoCapacity, state.Version);
            foreach (var item in yield) player.Add(item.ResourceId, item.Quantity);
            plot.CropId = null; plot.PlantedTicks = 0; plot.Revision++; _revision++;
            return new(FarmStatus.Harvested, new(_session, plot.Revision));
        }
    }
}
