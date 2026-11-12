using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;

namespace Hollowlight.Core;

public sealed record CropDefinition(string Id, string Name, string SeedResource, double GrowthDays,
    IReadOnlyDictionary<string, int> Yields, int SeedCost = 1, double GrowingAt = 0.35);
public sealed record PlotOffset(float X, float Y);
public sealed record AgricultureSetup(ResourceDefinition[] Resources, CropDefinition[] Crops, Dictionary<string, int> StartingStorage, PlotOffset[] PlotOffsets)
{
    public static AgricultureSetup Parse(string json) => JsonSerializer.Deserialize<AgricultureSetup>(json,
        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? throw new JsonException("Missing agriculture definition.");
}
public enum CropStage { Empty, Seedling, Growing, Mature }
public enum FarmStatus { Planted, Harvested, Stale, InvalidPlayer, UnknownPlot, UnknownCrop, Occupied, Empty, NoSeed, NotMature, NoCapacity, AccessDenied }
public sealed record FarmResult(FarmStatus Status, SupplyVersion PlotVersion);
public sealed record PlotSnapshot(string Id, string? CropId, long PlantedTicks, long ReadyAtTicks,
    SupplyVersion Version, CropStage Stage, double Progress, long RemainingTicks);

// Passive, timestamp-based growth; no per-plot timers, inventory ownership or renderer.
internal sealed class Agriculture
{
    internal sealed class Plot
    {
        public string? CropId;
        public long PlantedTicks;
        public long Revision;
    }
    public readonly Dictionary<string, Plot> Plots;
    private readonly Dictionary<string, CropDefinition> _crops;
    public IReadOnlyList<CropDefinition> Crops { get; }
    public readonly GameClock Clock;
    public Agriculture(AgricultureSetup setup, GameClock clock, IEnumerable<string> plotIds)
    {
        Clock = clock;
        var definitions = setup.Crops.Select(c => c with {
            Yields = new ReadOnlyDictionary<string, int>(c.Yields.ToDictionary(p => p.Key, p => p.Value)) }).ToArray();
        _crops = definitions.ToDictionary(c => c.Id);
        if (definitions.Length == 0 || definitions.Any(c => string.IsNullOrWhiteSpace(c.Id) || string.IsNullOrWhiteSpace(c.Name) ||
            string.IsNullOrWhiteSpace(c.SeedResource) || c.SeedCost < 1 || !double.IsFinite(c.GrowthDays) || c.GrowthDays <= 0 ||
            !double.IsFinite(c.GrowingAt) || c.GrowingAt <= 0 || c.GrowingAt >= 1 || c.Yields.Count == 0 || c.Yields.Values.Any(q => q < 1)))
            throw new ArgumentException("Invalid crop definition.");
        foreach (var crop in definitions) if (Duration(crop) < 1) throw new ArgumentException("Crop duration must be positive.");
        Crops = Array.AsReadOnly(definitions);
        Plots = plotIds.ToDictionary(id => id, _ => new Plot());
        if (Plots.Count == 0 || Plots.Keys.Any(string.IsNullOrWhiteSpace)) throw new ArgumentException("Plots need stable unique IDs.");
    }
    public bool TryCrop(string id, out CropDefinition crop) => _crops.TryGetValue(id, out crop!);
    public long Duration(CropDefinition crop) => checked((long)Math.Round(crop.GrowthDays * Clock.CycleTicks));
    public PlotSnapshot Snapshot(string id, Guid session)
    {
        var plot = Plots[id];
        if (plot.CropId == null) return new(id, null, 0, 0, new(session, plot.Revision), CropStage.Empty, 0, 0);
        var crop = _crops[plot.CropId];
        long duration = Duration(crop), ready = checked(plot.PlantedTicks + duration);
        double progress = Math.Clamp((double)(Clock.ElapsedTicks - plot.PlantedTicks) / duration, 0, 1);
        return new(id, crop.Id, plot.PlantedTicks, ready, new(session, plot.Revision),
            progress >= 1 ? CropStage.Mature : progress >= crop.GrowingAt ? CropStage.Growing : CropStage.Seedling,
            progress, Math.Max(0, ready - Clock.ElapsedTicks));
    }
}
