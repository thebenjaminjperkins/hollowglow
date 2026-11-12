using Godot;
using Hollowlight.Core;

namespace Hollowlight.Presentation;

// Native TileMapLayer assembly of texture tiles. Existing simulation owns collision.
public partial class RoomTerrain : Node2D
{
    public CaveLayout Layout { get; set; } = null!;
    public TileMapLayer Floor { get; private set; } = null!;
    public TileMapLayer Walls { get; private set; } = null!;
    public override void _Ready()
    {
        var field = Layer("Ceiling", "wall", 4, 2, -6);
        field.Modulate = new Color(0.22f, 0.24f, 0.27f);
        Floor = Layer("Floor", "floor", 32, 16, -5);
        Walls = Layer("Walls", "wall", 4, 2, -4);
        for (int y = 0; y < 22; y++)
        for (int x = 0; x < 40; x++)
        {
            var cell = new Vector2I(x, y);
            int patch = (x & 1) + (y & 1) * 2;
            if (Layout.IsFloor(x, y))
            {
                int mask = (Layout.CutsCorner(x, y, -1, -1) ? 1 : 0) |
                    (Layout.CutsCorner(x, y, 1, -1) ? 2 : 0) |
                    (Layout.CutsCorner(x, y, 1, 1) ? 4 : 0) |
                    (Layout.CutsCorner(x, y, -1, 1) ? 8 : 0);
                bool damp = x >= 20;
                int floorPatch = (x & 3) + (y & 3) * 4;
                int index = ((damp ? 16 : 0) + floorPatch) * 16 + mask;
                Floor.SetCell(cell, 0, new Vector2I(index % 32, index / 32));
            }
            else
            {
                field.SetCell(cell, 0, new Vector2I(patch, 1));
                bool edge = Layout.IsFloor(x - 1, y) || Layout.IsFloor(x + 1, y) || Layout.IsFloor(x, y - 1) || Layout.IsFloor(x, y + 1);
                if (edge)
                {
                    int index = (Layout.IsFloor(x, y + 1) ? 0 : 4) + patch;
                    Walls.SetCell(cell, 0, new Vector2I(index % 4, index / 4));
                }
            }
        }
    }
    private TileMapLayer Layer(string name, string image, int columns, int rows, int z)
    {
        var source = new TileSetAtlasSource { Texture = GD.Load<Texture2D>($"res://assets/sanctuary/{image}.png"), TextureRegionSize = new Vector2I(32, 32) };
        for (int y = 0; y < rows; y++)
        for (int x = 0; x < columns; x++) source.CreateTile(new Vector2I(x, y));
        var set = new TileSet { TileSize = new Vector2I(32, 32) };
        set.AddSource(source, 0);
        var layer = new TileMapLayer { Name = name, TileSet = set, Scale = Vector2.One * 0.5f,
            ZIndex = z, CollisionEnabled = false, NavigationEnabled = false, OcclusionEnabled = false,
            TextureFilter = TextureFilterEnum.Nearest };
        AddChild(layer);
        return layer;
    }
}
