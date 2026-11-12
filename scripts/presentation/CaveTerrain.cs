using Godot;
using Hollowlight.Core;

namespace Hollowlight.Presentation;

// Neighbour-aware 16px terrain kit: faces, exposed edges, chamfered corners and floor variants.
public partial class CaveTerrain : Node2D
{
    public CaveLayout Layout { get; set; } = null!;
    public int DecorationSeed { get; set; }
    private Texture2D _wall = null!;
    public override void _Ready()
    {
        _wall = GD.Load<Texture2D>("res://assets/benchmark/wall.png");
        TextureFilter = TextureFilterEnum.Nearest;
    }
    private void Box(float x, float y, float w, float h, string color) => DrawRect(new Rect2(x, y, w, h), new Color(color));
    public static Vector2[] FloorPolygon(CaveLayout layout, int x, int y)
    {
        int a = layout.CutsCorner(x, y, -1, -1) ? 4 : 0;
        int b = layout.CutsCorner(x, y, 1, -1) ? 4 : 0;
        int c = layout.CutsCorner(x, y, 1, 1) ? 4 : 0;
        int d = layout.CutsCorner(x, y, -1, 1) ? 4 : 0;
        // Duplicate vertices at square corners are removed before drawing/physics use.
        var points = new System.Collections.Generic.List<Vector2>(8);
        foreach (var point in new[] { new Vector2(a, 0), new Vector2(16 - b, 0), new Vector2(16, b),
            new Vector2(16, 16 - c), new Vector2(16 - c, 16), new Vector2(d, 16), new Vector2(0, 16 - d), new Vector2(0, a) })
            if (points.Count == 0 || points[^1] != point) points.Add(point);
        if (points.Count > 1 && points[0] == points[^1]) points.RemoveAt(points.Count - 1);
        return points.ToArray();
    }

    public override void _Draw()
    {
        Box(0, 0, 640, 352, "252a30");
        for (int y = 0; y < 22; y++)
        for (int x = 0; x < 40; x++)
        {
            int px = x * 16, py = y * 16;
            bool cave = x < 20;
            uint hash = unchecked((uint)(x * 73856093 ^ y * 19349663 ^ DecorationSeed));
            int variant = (int)(hash % 3);
            if (Layout.IsFloor(x, y))
            {
                var polygon = FloorPolygon(Layout, x, y);
                for (int i = 0; i < polygon.Length; i++) polygon[i] += new Vector2(px, py);
                string floor = cave ? (variant == 0 ? "48433b" : "464139") : (variant == 0 ? "667267" : "657166");
                DrawColoredPolygon(polygon, new Color(floor));
                int dx = 4 + (int)(hash % 6), dy = 5 + (int)((hash >> 4) % 5);
                Box(px + dx, py + dy, 4, 1, cave ? "565045" : "788474");
                if (variant == 0) Box(px + dx + 2, py + dy + 2, 2, 1, cave ? "3d3b35" : "56675e");
                if (!Layout.IsFloor(x, y - 1))
                {
                    Box(px + 4, py, 8, 2, cave ? "302f31" : "46594f");
                    Box(px + 6, py + 2, 5, 1, cave ? "3a3934" : "586d59");
                }
                if (!Layout.IsFloor(x - 1, y)) Box(px, py + 4, 2, 8, cave ? "363633" : "526558");
                if (!Layout.IsFloor(x + 1, y)) Box(px + 14, py + 4, 2, 8, cave ? "353533" : "526558");
                continue;
            }
            bool near = Layout.IsFloor(x + 1, y) || Layout.IsFloor(x - 1, y) || Layout.IsFloor(x, y + 1) || Layout.IsFloor(x, y - 1);
            if (!near) { if (variant == 0) Box(px + 4, py + 5, 6, 2, "2c3034"); continue; }
            if (cave)
            {
                int tile = Layout.IsFloor(x, y + 1) ? 3 + variant
                    : Layout.IsFloor(x + 1, y) ? 6 : Layout.IsFloor(x - 1, y) ? 7 : variant;
                Box(px, py, 16, 16, "303842");
                DrawTextureRectRegion(_wall, new Rect2(px, py, 16, 16), new Rect2(tile % 4 * 16, tile / 4 * 16, 16, 16));
                continue;
            }
            Box(px, py, 16, 16, cave ? "39393b" : "3c4e48");
            DrawColoredPolygon(new[] { new Vector2(px + 2, py + 1), new Vector2(px + 11, py),
                new Vector2(px + 16, py + 5), new Vector2(px + 14, py + 13), new Vector2(px + 7, py + 16),
                new Vector2(px, py + 11) }, new Color(cave ? "514b47" : "52645a"));
            Box(px + 4, py + 3, 7, 2, cave ? "6c6053" : "7c8b73");
            Box(px + 2, py + 7, 3, 4, cave ? "5c544b" : "607565");
            Box(px + 9, py + 11, 5, 3, cave ? "3c3b3d" : "3e534d");
            if (Layout.IsFloor(x, y + 1))
            {
                Box(px, py + 10, 16, 6, cave ? "665b4e" : "596d5d");
                Box(px + 2, py + 10, 12, 2, cave ? "88735a" : "8b9b7d");
                Box(px + 3, py + 14, 8, 1, cave ? "4e4842" : "425b50");
                if (variant == 1) Box(px + 10, py + 12, 2, 4, "4a6855");
            }
        }
    }
}
