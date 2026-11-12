using System;
using System.Text.Json;

namespace Hollowlight.Core;

public sealed class CaveLayout
{
    public const int TileSize = 16;
    public string Name { get; set; } = "";
    public string[] Rows { get; set; } = Array.Empty<string>();
    public PropPlacement[] Props { get; set; } = Array.Empty<PropPlacement>();
    public bool IsFloor(int x, int y) => y >= 0 && y < Rows.Length && x >= 0 &&
        x < Rows[y].Length && Rows[y][x] == '.';
    public bool CutsCorner(int x, int y, int dx, int dy) =>
        IsFloor(x, y) && !IsFloor(x + dx, y) && !IsFloor(x, y + dy);

    public static CaveLayout Parse(string json)
    {
        var layout = JsonSerializer.Deserialize<CaveLayout>(json) ?? throw new ArgumentException("Empty cave layout");
        if (layout.Rows.Length != 22) throw new ArgumentException("Threshold layouts must have 22 rows");
        for (int y = 0; y < layout.Rows.Length; y++)
        {
            if (layout.Rows[y].Length != 40) throw new ArgumentException("Threshold layouts must have 40 columns");
            for (int x = 0; x < 40; x++)
            {
                char cell = layout.Rows[y][x];
                if (cell != '.' && cell != '#') throw new ArgumentException("Use only floor (.) and rock (#)");
                if ((x == 0 || x == 39 || y == 0 || y == 21) && cell != '#')
                    throw new ArgumentException("Layouts require a closed outer rock boundary");
            }
        }
        return layout;
    }
}

public sealed class PropPlacement
{
    public string Kind { get; set; } = "Fern";
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public int Variant { get; set; }
    public float Radius { get; set; }
    public string Color { get; set; } = "efbb79";
    public float Energy { get; set; } = 0.5f;
}
