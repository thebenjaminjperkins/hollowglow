using System.Collections.Generic;
using Godot;
using Hollowlight.World;

namespace Hollowlight.Presentation;

// Approved playable visual assembly. All original bodies, inputs, rules and resource quantities stay owned by World.
public partial class RoomPresentation : Node
{
    public ThresholdWorld World { get; set; } = null!;
    public Color AmbientTint { get; set; } = new(0.78f, 0.80f, 0.84f);
    public float LampEnergy { get; set; } = 0.9f;
    public float LampRadiusScale { get; set; } = 1.15f;
    public RoomTerrain Terrain { get; private set; } = null!;
    private CanvasModulate _ambient = null!;
    private readonly List<PointLight2D> _lights = new();
    private bool _neutral;
    public override void _Ready()
    {
        var actor = World.GetNode<PlayerCharacter>("Player");
        actor.GetNode<Camera2D>("Camera2D").Zoom = Vector2.One * 2;
        World.GetNode<ThresholdHud>("HUD").Scale = Vector2.One * 2;
        actor.GetNode<CanvasItem>("Art").Hide();
        actor.AddChild(new RoomCharacter { Name = "RoomCharacter" });
        var map = World.GetNode<ThresholdMap>("Map");
        foreach (var child in map.GetChildren())
        {
            if (child is CaveTerrain oldTerrain) oldTerrain.Hide();
            if (child is not SanctuaryProp prop) continue;
            foreach (var old in prop.GetChildren())
            {
                if (old is PixelSprite or BenchmarkSprite or ReactiveFern) ((CanvasItem)old).Hide();
                if (old is PointLight2D light)
                {
                    _lights.Add(light);
                    if (prop.Kind == PixelSprite.Motif.Lamp)
                    { light.Position = new Vector2(0, -24); light.TextureScale *= LampRadiusScale; light.Energy = LampEnergy; }
                }
            }
            string? asset = AssetFor(prop.Kind);
            if (asset != null) prop.AddChild(new RoomSprite { Name = "RoomArt", Asset = asset, Reactive = prop.Kind == PixelSprite.Motif.Fern });
        }
        Terrain = new RoomTerrain { Name = "RoomTerrain", Layout = map.Layout };
        map.AddChild(Terrain);
        foreach (var child in World.GetChildren())
        {
            if (child is not Interactable target) continue;
            target.Art.Hide();
            target.AddChild(new RoomSprite { Name = "RoomArt", Asset = target.IsStorage ? "storage" : target.PresentationAsset, InteractionOwner = target });
        }
        _ambient = new CanvasModulate { Color = AmbientTint };
        World.AddChild(_ambient);
    }
    private static string? AssetFor(PixelSprite.Motif kind) => kind switch
    {
        PixelSprite.Motif.Generator => "machine",
        PixelSprite.Motif.Shelf => "shelf",
        PixelSprite.Motif.Bench => "workbench",
        PixelSprite.Motif.Garden => "garden",
        PixelSprite.Motif.Lamp => "lamp",
        PixelSprite.Motif.Bedroll => "bedroll",
        PixelSprite.Motif.Fern or PixelSprite.Motif.Plant => "fern",
        PixelSprite.Motif.Mushrooms => "mushrooms",
        PixelSprite.Motif.Vine => "vine",
        PixelSprite.Motif.Stones or PixelSprite.Motif.Ruin => "stones",
        _ => null // Old non-solid cables/rugs/marks omitted to keep the room visually quiet.
    };
    public void ToggleLighting()
    {
        _neutral = !_neutral;
        _ambient.Color = _neutral ? Colors.White : AmbientTint;
        foreach (var light in _lights) light.Visible = !_neutral;
    }
    public override void _UnhandledKeyInput(InputEvent @event)
    {
        if (@event is InputEventKey key && key.Pressed && !key.Echo && key.PhysicalKeycode == Key.L)
        { ToggleLighting(); GetViewport().SetInputAsHandled(); }
    }
}
