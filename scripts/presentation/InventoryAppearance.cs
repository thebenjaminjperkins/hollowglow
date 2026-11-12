using System;
using System.Collections.Generic;
using System.Text.Json;
using Godot;

namespace Hollowlight.Presentation;

// Presentation dimensions and icon regions never determine inventory capacity.
public sealed class InventoryAppearance
{
    public int BackpackColumns { get; set; } = 3;
    public int ChestColumns { get; set; } = 6;
    public int ChestRows { get; set; } = 4;
    public int CellSize { get; set; } = 24;
    public Dictionary<string, IconRegion> Icons { get; set; } = new();
    private readonly Dictionary<string, Texture2D> _textures = new();
    public sealed class IconRegion
    {
        public string Path { get; set; } = "";
        public int[] Region { get; set; } = Array.Empty<int>();
    }
    public static InventoryAppearance Load()
    {
        var result = JsonSerializer.Deserialize<InventoryAppearance>(FileAccess.GetFileAsString("res://data/inventory_ui.json"),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? throw new InvalidOperationException("Missing inventory appearance");
        if (result.BackpackColumns < 1 || result.BackpackColumns > 6 || result.ChestColumns < 1 || result.ChestRows < 1 || result.CellSize < 20)
            throw new InvalidOperationException("Inventory grids need positive dimensions and cells of at least 20 units");
        int width = (result.BackpackColumns + result.ChestColumns) * result.CellSize + 40;
        int height = Math.Max((6 + result.BackpackColumns - 1) / result.BackpackColumns, result.ChestRows) * result.CellSize + 74;
        if (width > 368 || height > 196) throw new InvalidOperationException("Inventory presentation must fit the 384x216 interface canvas");
        return result;
    }
    public Texture2D? Icon(string resource)
    {
        if (_textures.TryGetValue(resource, out var texture)) return texture;
        if (!Icons.TryGetValue(resource, out var entry)) return null;
        if (entry.Region.Length != 4) throw new InvalidOperationException("Icon region needs x,y,width,height");
        var atlas = GD.Load<Texture2D>(entry.Path);
        if (entry.Region[0] < 0 || entry.Region[1] < 0 || entry.Region[2] < 1 || entry.Region[3] < 1 ||
            entry.Region[0] + entry.Region[2] > atlas.GetWidth() || entry.Region[1] + entry.Region[3] > atlas.GetHeight() ||
            entry.Region[2] > (CellSize - 4) * 2 || entry.Region[3] > (CellSize - 4) * 2)
            throw new InvalidOperationException("Icon region must fit its source and slot at native pixel density");
        texture = new AtlasTexture { Atlas = atlas,
            Region = new Rect2(entry.Region[0], entry.Region[1], entry.Region[2], entry.Region[3]) };
        _textures.Add(resource, texture);
        return texture;
    }
}
