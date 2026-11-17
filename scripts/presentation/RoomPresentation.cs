using System.Collections.Generic;
using System.Linq;
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
    public WorldVisuals Visuals {get;private set;}=null!;
    private CanvasModulate _ambient = null!;
    private readonly List<PointLight2D> _lights = new();
    private bool _neutral;
    private readonly List<PointLight2D> _lamps = new();
    private PointLight2D? _workshop, _generator;
    public override void _Ready()
    {
        var actor = World.GetNode<PlayerCharacter>("Player");
        actor.GetNode<Camera2D>("Camera2D").Zoom = Vector2.One * 2;
        World.GetNode<ThresholdHud>("HUD").Scale = Vector2.One * 2;
        World.GetNode<ThresholdHud>("HUD").UseResponsiveChrome();
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
                    if(prop.Kind==PixelSprite.Motif.Generator)_generator=light;
                    if (prop.Kind == PixelSprite.Motif.Lamp)
                    { _lamps.Add(light); light.SetMeta("power_consumer","lighting"); light.Position = new Vector2(0, -24); light.TextureScale *= LampRadiusScale; light.Energy = LampEnergy; }
                }
            }
            string? asset = AssetFor(prop.Kind);
            if (prop.Kind == PixelSprite.Motif.Garden && World.FarmingEnabled)
                prop.AddChild(new Sprite2D { Texture = GD.Load<Texture2D>("res://assets/farming/bed.png"), Centered = false,
                    Position = new Vector2(-18, -25), Scale = Vector2.One * 0.5f, TextureFilter = CanvasItem.TextureFilterEnum.Nearest });
            else if (asset != null) prop.AddChild(new RoomSprite { Name = "RoomArt", Asset = asset, Reactive = prop.Kind == PixelSprite.Motif.Fern });
        }
        if(map.Region==null){Terrain = new RoomTerrain { Name = "RoomTerrain", Layout = map.Layout };map.AddChild(Terrain);}
        else
        {
            map.AddChild(new MountainTerrain {Name="MountainTerrain",Region=map.Region});
            if(map.Region.Garage!=null)map.AddChild(new GarageEnvironment{Plan=map.Region.Garage});
            if(map.Region.Definition.Hall!=null)map.AddChild(new HallEnvironment{Plan=map.Region.Definition.Hall});
            foreach(var placement in map.Region.Decorations)map.AddChild(new RegionProp {Placement=placement,Position=new Vector2(placement.X,placement.Y)});
        }
        foreach (var child in World.GetChildren())
        {
            if (child is not Interactable target) continue;
            target.Art.Hide();
            if (target.IsFiniteSource) continue;
            target.AddChild(new RoomSprite { Name = "RoomArt", Asset = target.IsStorage ? "storage" : target.PresentationAsset, InteractionOwner = target });
        }
        if(World.Config.PowerEnabled)
        {
            var bench=System.Linq.Enumerable.First(map.GetChildren().OfType<SanctuaryProp>(),p=>p.Kind==PixelSprite.Motif.Bench);
            _workshop=new PointLight2D { Texture=_lamps[0].Texture,TextureScale=.25f,Energy=.4f,Color=new Color("efbb79"),
                Position=new Vector2(0,-12),RangeZMin=-10,RangeZMax=10 };
            bench.AddChild(_workshop);_lights.Add(_workshop);
        }
        _ambient = new CanvasModulate { Color = Colors.White };
        World.AddChild(_ambient);
        Visuals=new WorldVisuals{Name="WorldVisuals",World=World,CaveAmbient=AmbientTint};World.AddChild(Visuals);
    }
    public override void _Process(double delta)
    {
        Visuals.Neutral=_neutral;
        var power=World.Electrical;
        if(power==null)return;
        bool Powered(string id)=>power.Consumers.Any(c=>c.Id==id&&c.Powered);
        foreach(var lamp in _lamps)lamp.Visible=!_neutral&&Powered("lighting");
        if(_workshop!=null)_workshop.Visible=!_neutral&&Powered("workbench");
        if(_generator!=null)_generator.Visible=!_neutral&&power.Running;
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
        Visuals.Neutral=_neutral;
        foreach (var light in _lights) light.Visible = !_neutral;
    }
    public override void _UnhandledKeyInput(InputEvent @event)
    {
        if (@event is InputEventKey key && key.Pressed && !key.Echo && key.PhysicalKeycode == Key.L)
        { ToggleLighting(); GetViewport().SetInputAsHandled(); }
    }
}
