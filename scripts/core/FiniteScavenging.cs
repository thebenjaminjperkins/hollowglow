using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Hollowlight.Core;

public sealed record SourceType(string Id, string Name, string Asset, int NearCount, int FarCount,
    int NearMin, int NearMax, int FarMin, int FarMax);
public sealed record ScavengingSetup(int EntranceX, int EntranceY, int SurfaceMinX, int NearMinDistance,
    int NearMaxDistance, int FarMinDistance, int FarMaxDistance, int MinimumSpacing, SourceType[] Types, string ResourceId = "food", string Domain = "food")
{
    public static ScavengingSetup Parse(string json) => JsonSerializer.Deserialize<ScavengingSetup>(json,
        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? throw new JsonException("Missing scavenging setup.");
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(ResourceId) || string.IsNullOrWhiteSpace(Domain) || NearMinDistance < 16 || NearMaxDistance < NearMinDistance || FarMinDistance <= NearMaxDistance ||
            FarMaxDistance < FarMinDistance || MinimumSpacing < 16 || Types.Length == 0 ||
            Types.Select(t => t.Id).Distinct().Count() != Types.Length || Types.Any(t => string.IsNullOrWhiteSpace(t.Id) ||
                string.IsNullOrWhiteSpace(t.Name) || string.IsNullOrWhiteSpace(t.Asset) || t.NearCount < 0 || t.FarCount < 0 ||
                t.NearMin < 1 || t.FarMin < 1 || t.NearMax < t.NearMin || t.FarMax < t.FarMin ||
                t.NearMax > SharedSupplies.StackLimit || t.FarMax > SharedSupplies.StackLimit) ||
            Types.Sum(t => t.NearCount + t.FarCount) is < 1 or > 32)
            throw new ArgumentException("Invalid finite source configuration.");
    }
}
public sealed record GeneratedSourceSite(GatheringSite Site, string Type, string Band, int PathDistance, int ApproachX, int ApproachY);
public sealed record SourceWorld(int Seed, IReadOnlyList<GeneratedSourceSite> Sources);

// Resource placement on supplied terrain, not a terrain generator. No player-count
// input, clock, inventory mutation, retries on collection, or respawn timer.
public static class FiniteScavenging
{
    private readonly record struct Cell(int X, int Y);
    public static SourceWorld Generate(ScavengingSetup setup, CaveLayout layout, IEnumerable<GatheringSite> existing, int seed, IEnumerable<GatheringSite>? groundSources = null)
    {
        setup.Validate();
        var oldSites = existing.ToArray();
        var ground = (groundSources ?? Array.Empty<GatheringSite>()).ToArray();
        bool Walkable(int x, int y)
        {
            // Actor feet are 8x6. Check their clearance, including thin prop bodies.
            foreach (int dx in new[] { -4, 4 }) foreach (int dy in new[] { -3, 3 })
                if (!layout.IsFloor((x + dx) / 16, (y + dy) / 16)) return false;
            foreach (var p in layout.Props)
                if (p.Width > 0 && p.Height > 0 && Math.Abs(x - p.X) < p.Width / 2.0 + 4 && Math.Abs(y - p.Y) < p.Height / 2.0 + 3) return false;
            return oldSites.All(p => Math.Abs(x - p.X) >= 10 || Math.Abs(y - p.Y - 1) >= 6.5);
        }
        bool EdgeClear(Cell a, Cell b)
        {
            for (int i = 0; i <= 4; i++)
                if (!Walkable(a.X * 16 + 8 + (b.X - a.X) * i * 4, a.Y * 16 + 8 + (b.Y - a.Y) * i * 4)) return false;
            return true;
        }
        var entrance = new Cell(setup.EntranceX / 16, setup.EntranceY / 16);
        if (!Walkable(entrance.X * 16 + 8, entrance.Y * 16 + 8)) throw new ArgumentException("Source placement needs a clear entrance.");
        var distances = new Dictionary<Cell, int> { [entrance] = 0 }; var queue = new Queue<Cell>(); queue.Enqueue(entrance);
        var directions = new[] { new Cell(1, 0), new Cell(0, 1), new Cell(-1, 0), new Cell(0, -1) };
        while (queue.Count > 0)
        {
            var cell = queue.Dequeue();
            foreach (var d in directions)
            {
                var next = new Cell(cell.X + d.X, cell.Y + d.Y);
                if (next.X * 16 < setup.SurfaceMinX || distances.ContainsKey(next) || !EdgeClear(cell, next)) continue;
                distances.Add(next, distances[cell] + 16); queue.Enqueue(next);
            }
        }
        var candidates = distances.Keys.OrderBy(c => c.Y).ThenBy(c => c.X).Where(c => {
            int x = c.X * 16 + 8, y = c.Y * 16 + 8;
            var approach = new Cell(c.X, c.Y + 1);
            if (!distances.ContainsKey(approach) || !EdgeClear(c, approach)) return false;
            // Leave a quiet visual envelope and avoid existing interactive/decorative props.
            if (!layout.IsFloor(c.X, c.Y - 1) || !layout.IsFloor(c.X - 1, c.Y) || !layout.IsFloor(c.X + 1, c.Y)) return false;
            return layout.Props.All(p => Square(x - p.X) + Square(y - p.Y) >= 28 * 28) &&
                oldSites.All(p => Square(x - p.X) + Square(y - p.Y) >= 48 * 48) &&
                ground.All(p => Square(x - p.X) + Square(y - p.Y) >= Square(setup.MinimumSpacing));
        }).ToList();
        var random = new SeedStream(seed, setup.Domain + "-placement-v1");
        for (int i = candidates.Count - 1; i > 0; i--) { int j = random.Next(i + 1); (candidates[i], candidates[j]) = (candidates[j], candidates[i]); }
        var slots = new List<(SourceType Type, string Band, int Ordinal)>();
        foreach (string band in new[] { "near", "far" }) foreach (var type in setup.Types)
            for (int n = 0; n < (band == "near" ? type.NearCount : type.FarCount); n++) slots.Add((type, band, n));
        var placed = new List<GeneratedSourceSite>();
        bool Place(int index)
        {
            if (index == slots.Count) return true;
            var (type, band, ordinal) = slots[index]; bool near = band == "near";
            foreach (var cell in candidates)
            {
                int distance = distances[cell], x = cell.X * 16 + 8, y = cell.Y * 16 + 8;
                if (distance < (near ? setup.NearMinDistance : setup.FarMinDistance) || distance > (near ? setup.NearMaxDistance : setup.FarMaxDistance) ||
                    placed.Any(p => Square(p.Site.X - x) + Square(p.Site.Y - y) < Square(setup.MinimumSpacing))) continue;
                string id = $"{setup.Domain}-{band}-{type.Id}-{ordinal}";
                var quantities = new SeedStream(seed, setup.Domain + "-quantity-v1:" + id);
                int min = near ? type.NearMin : type.FarMin, max = near ? type.NearMax : type.FarMax;
                var site = new GatheringSite(id, type.Name, setup.ResourceId, min + quantities.Next(max - min + 1), x, y, type.Asset);
                placed.Add(new(site, type.Id, band, distance, x, y + 12));
                if (Place(index + 1)) return true;
                placed.RemoveAt(placed.Count - 1);
            }
            return false;
        }
        if (!Place(0)) throw new ArgumentException("Terrain cannot fit the configured sources with safe approaches and spacing.");
        return new(seed, Array.AsReadOnly(placed.ToArray()));
    }
    // Roll quantities without terrain placement. Domains/IDs match compact fixtures exactly.
    public static IReadOnlyList<GeneratedSourceSite> RollBudget(ScavengingSetup setup,int seed)
    {
        setup.Validate();var result=new List<GeneratedSourceSite>();
        foreach(string band in new[]{"near","far"})foreach(var type in setup.Types)
        for(int ordinal=0;ordinal<(band=="near"?type.NearCount:type.FarCount);ordinal++)
        {
            bool near=band=="near";string id=$"{setup.Domain}-{band}-{type.Id}-{ordinal}";
            var stream=new SeedStream(seed,setup.Domain+"-quantity-v1:"+id);
            int min=near?type.NearMin:type.FarMin,max=near?type.NearMax:type.FarMax;
            result.Add(new(new GatheringSite(id,type.Name,setup.ResourceId,min+stream.Next(max-min+1),0,0,type.Asset),type.Id,band,0,0,0));
        }
        return result.AsReadOnly();
    }
    private static long Square(int n) => (long)n * n;
    // Specified SplitMix64 stream + FNV-1a domain labels, independent of runtime
    // Random implementation, decoration, player count and quantity/placement draws.
    internal sealed class SeedStream
    {
        private ulong _value;
        public SeedStream(int seed, string domain)
        {
            ulong hash = 14695981039346656037UL;
            foreach (char c in domain) hash = unchecked((hash ^ c) * 1099511628211UL);
            _value = hash ^ unchecked((ulong)(long)seed);
        }
        private ulong NextValue()
        {
            _value = unchecked(_value + 0x9e3779b97f4a7c15UL); ulong z = _value;
            z = unchecked((z ^ (z >> 30)) * 0xbf58476d1ce4e5b9UL);
            z = unchecked((z ^ (z >> 27)) * 0x94d049bb133111ebUL);
            return z ^ (z >> 31);
        }
        public int Next(int bound)
        {
            ulong size = (ulong)bound, threshold = unchecked(0UL - size) % size, value;
            do { value = NextValue(); } while (value < threshold);
            return (int)(value % size);
        }
    }
}
