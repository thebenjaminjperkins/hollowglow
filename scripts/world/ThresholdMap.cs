using System;
using Godot;
using Hollowlight.Core;
using Hollowlight.Presentation;

namespace Hollowlight.World;

public partial class ThresholdMap : Node2D
{
    public const int Tile = CaveLayout.TileSize;
    public const int Width = 40;
    public const int Height = 22;
    public const int SurfaceStart = 320;
    [Export] public WorldConfig Config { get; set; } = null!;
    [Export(PropertyHint.File, "*.json")] public string LayoutPath { get; set; } = "res://data/sanctuary.json";
    public CaveLayout Layout { get; private set; } = null!;
    public override void _Ready()
    {
        YSortEnabled = true;
        if (Array.Exists(OS.GetCmdlineUserArgs(), arg => arg == "--sanctuary-variant"))
            LayoutPath = "res://data/sanctuary_alternate.json";
        Layout = CaveLayout.Parse(FileAccess.GetFileAsString(LayoutPath));
        AddChild(new CaveTerrain { Layout = Layout, DecorationSeed = Config.DecorationSeed, ZIndex = -5 });
        for (int y = 0; y < Height; y++)
        for (int x = 0; x < Width; x++)
        {
            if (!Layout.IsFloor(x, y))
            {
                var body = new StaticBody2D { Position = new Vector2(x * Tile + 8, y * Tile + 8), CollisionLayer = 1, CollisionMask = 0 };
                body.AddChild(new CollisionShape2D { Shape = new RectangleShape2D { Size = new Vector2(Tile, Tile) } });
                AddChild(body);
                continue;
            }
            // Cut corner triangles use the same neighbour rule as the visible terrain edge.
            foreach (int dx in new[] { -1, 1 })
            foreach (int dy in new[] { -1, 1 })
                if (Layout.CutsCorner(x, y, dx, dy))
                {
                    float cx = dx < 0 ? 0 : 16, cy = dy < 0 ? 0 : 16;
                    var body = new StaticBody2D { Position = new Vector2(x * Tile, y * Tile), CollisionLayer = 1, CollisionMask = 0 };
                    body.AddChild(new CollisionShape2D { Shape = new ConvexPolygonShape2D { Points = new[] {
                        new Vector2(cx, cy), new Vector2(cx - dx * 4, cy), new Vector2(cx, cy - dy * 4)
                    } } });
                    AddChild(body);
                }
        }
        var prefab = GD.Load<PackedScene>("res://scenes/SanctuaryProp.tscn");
        foreach (var placement in Layout.Props)
        {
            var prop = prefab.Instantiate<SanctuaryProp>();
            prop.Position = new Vector2(placement.X, placement.Y);
            prop.Kind = Enum.Parse<PixelSprite.Motif>(placement.Kind);
            if (prop.Kind is PixelSprite.Motif.Cable or PixelSprite.Motif.Root or PixelSprite.Motif.Rug or PixelSprite.Motif.Deck)
                prop.ZIndex = -2;
            prop.Variant = placement.Variant;
            prop.Footprint = new Vector2(placement.Width, placement.Height);
            prop.GlowRadius = placement.Radius;
            prop.GlowColor = new Color(placement.Color);
            prop.GlowEnergy = placement.Energy;
            AddChild(prop);
        }
    }
}
