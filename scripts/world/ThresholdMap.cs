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
    public MountainRegion? Region { get; private set; }
    public CaveLayout Layout { get; private set; } = null!;
    public override void _Ready()
    {
        YSortEnabled = true;
        Config=GetParent<ThresholdWorld>().Config;
        if(Config.LayoutProfile is "mountain-region" or "garage-expedition" or "sanctuary-hall-review")
        {
            var definition=RegionDefinition.Parse(FileAccess.GetFileAsString("res://data/regions/mountain.json"));
            if(Config.LayoutProfile=="sanctuary-hall-review")definition=SanctuaryPlan.Parse(FileAccess.GetFileAsString("res://data/regions/sanctuary_hall.json")).Apply(definition);
            Region=MountainRegion.Generate(definition,
                SupplySetup.Parse(FileAccess.GetFileAsString("res://data/supplies.json")),
                ScavengingSetup.Parse(FileAccess.GetFileAsString("res://data/food_scavenging.json")),
                ScavengingSetup.Parse(FileAccess.GetFileAsString("res://data/fuel_scavenging.json")),Config.WorldSeed);
            if(Config.LayoutProfile!="mountain-region")Region.InstallGarage(GarageDefinition.Parse(FileAccess.GetFileAsString("res://data/locations/garage.json")));
            Layout=Region.Layout;BuildRegionBodies();BuildProps();return;
        }
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
        BuildProps();
    }
    private void BuildRegionBodies()
    {
        // Merge horizontal solid runs. Collision comes from the same ground mask as rendering/navigation.
        for(int y=0;y<Region!.Height;y++)
        for(int x=0;x<Region.Width;)
        {
            if(Layout.IsFloor(x,y)){x++;continue;}
            int first=x;while(x<Region.Width&&!Layout.IsFloor(x,y))x++;
            var body=new StaticBody2D {Position=new Vector2((first+x)*8,y*16+8),CollisionLayer=1,CollisionMask=0};
            body.AddChild(new CollisionShape2D {Shape=new RectangleShape2D {Size=new Vector2((x-first)*16,16)}});AddChild(body);
        }
        foreach(var decor in Region.Decorations)
        {
            if(decor.Width<=0||decor.Height<=0)continue;
            var body=new StaticBody2D {Position=new Vector2(decor.X,decor.Y),CollisionLayer=1,CollisionMask=0};
            body.AddChild(new CollisionShape2D {Shape=new RectangleShape2D {Size=new Vector2(decor.Width,decor.Height)}});AddChild(body);
        }
        if(Region.Garage!=null)foreach(var o in Region.Garage.Objects)
        {
            var body=new StaticBody2D {Position=new Vector2(o.Foot.WorldX,o.Foot.WorldY+o.Rule.OffsetY),CollisionLayer=1,CollisionMask=0};
            body.AddChild(new CollisionShape2D {Shape=new RectangleShape2D {Size=new Vector2(o.Rule.Width,o.Rule.Height)}});AddChild(body);
        }
        if(Region.Definition.Hall is {} hall)foreach(var fixture in hall.Fixtures)
        {
            var p=hall.FixtureFoot(fixture);var body=new StaticBody2D{Position=new(p.WorldX,p.WorldY),CollisionLayer=1,CollisionMask=0};
            body.AddChild(new CollisionShape2D{Shape=new RectangleShape2D{Size=new(fixture.Width,fixture.Height)}});AddChild(body);
        }
    }
    private void BuildProps()
    {
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
