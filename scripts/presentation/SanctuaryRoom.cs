using System;
using System.Threading.Tasks;
using Godot;
using Hollowlight.World;
using Hollowlight.Inputs;

namespace Hollowlight.Presentation;

public partial class SanctuaryRoom : Node
{
    [Export] public Color AmbientTint { get; set; } = new(0.78f, 0.80f, 0.84f);
    [Export(PropertyHint.Range, "0,2,0.05")] public float LampEnergy { get; set; } = 0.9f;
    [Export(PropertyHint.Range, "0.5,2,0.05")] public float LampRadiusScale { get; set; } = 1.15f;
    public bool ScavengingEnabled { get; set; } = true;
    public int WorldSeed { get; set; } = 1701;
    public bool CraftingEnabled { get; set; } = true;
    public bool RescueEnabled { get; set; } = true;
    public bool PowerEnabled { get; set; } = true;
    public bool HungerEnabled { get; set; } = true;
    public PlayerInput? InputOverride { get; set; }
    public ThresholdWorld World => _world;
    public RoomPresentation Presentation => _presentation;
    private ThresholdWorld _world = null!;
    private PlayerCharacter _actor = null!;
    private RoomPresentation _presentation = null!;
    private SceneTree _tree = null!;
    public static void ConfigureWindow(Window window)
    {
        window.ContentScaleSize = new Vector2I(768, 432);
        window.ContentScaleMode = Window.ContentScaleModeEnum.CanvasItems;
        window.ContentScaleStretch = Window.ContentScaleStretchEnum.Integer;
        window.Size = new Vector2I(1536, 864);
    }
    public override async void _Ready()
    {
        _tree = GetTree();
        try
        {
            ConfigureWindow(GetWindow());
            bool capture = Array.Exists(OS.GetCmdlineUserArgs(), arg => arg == "--capture-room");
            _world = GD.Load<PackedScene>("res://scenes/Threshold.tscn").Instantiate<ThresholdWorld>();
            _world.Config = (WorldConfig)_world.Config.Duplicate();
            _world.Config.CraftingEnabled = CraftingEnabled;
            _world.Config.RescueEnabled = RescueEnabled;
            _world.Config.PowerEnabled = PowerEnabled;
            _world.Config.HungerEnabled = HungerEnabled;
            _world.Config.ScavengingEnabled = ScavengingEnabled;
            _world.Config.WorldSeed = WorldSeed;
            _actor = _world.GetNode<PlayerCharacter>("Player");
            PlayerInput? input = InputOverride ?? (capture ? new RoomCaptureInput() : null);
            if (input != null)
            {
                input.Name = "PresentationInput";
                _actor.AddChild(input);
                _actor.InputPath = new NodePath(input.Name);
            }
            AddChild(_world);
            _presentation = new RoomPresentation { World = _world, AmbientTint = AmbientTint,
                LampEnergy = LampEnergy, LampRadiusScale = LampRadiusScale };
            AddChild(_presentation);
            if (capture) await CaptureRoom();
        }
        catch (Exception error) { GD.PushError($"Sanctuary room: {error}"); _tree.Quit(1); }
    }
    private async Task CaptureRoom()
    {
        await ToSignal(_tree.CreateTimer(6.2), SceneTreeTimer.SignalName.Timeout);
        _actor.Position = new Vector2(144, 144);
        await Capture("room");
        _presentation.ToggleLighting();
        await Capture("neutral");
        _presentation.ToggleLighting();
        _actor.Position = new Vector2(136, 224);
        await Capture("home");
        _actor.Position = new Vector2(88, 242);
        await Capture("storage");
        GetWindow().Size = new Vector2I(768, 432);
        _actor.Position = new Vector2(144, 144);
        await Capture("native");
        var camera = _actor.GetNode<Camera2D>("Camera2D");
        Vector2 coverage = GetViewport().GetVisibleRect().Size / camera.Zoom;
        if (coverage.DistanceTo(new Vector2(384, 216)) > 0.01f) throw new InvalidOperationException("Unexpected camera coverage");
        GD.Print($"PASS: native TileMapLayer room captures, world coverage {coverage}, adjustable lighting");
        _tree.Quit();
    }
    private async Task Capture(string name)
    {
        for (int i = 0; i < 6; i++) await ToSignal(_tree, SceneTree.SignalName.ProcessFrame);
        await ToSignal(RenderingServer.Singleton, RenderingServer.SignalName.FramePostDraw);
        DirAccess.MakeDirRecursiveAbsolute(ProjectSettings.GlobalizePath("user://sanctuary-room"));
        Error error = GetViewport().GetTexture().GetImage().SavePng($"user://sanctuary-room/{name}.png");
        if (error != Error.Ok) throw new InvalidOperationException($"Could not save {name}: {error}");
        GD.Print($"Capture: {ProjectSettings.GlobalizePath($"user://sanctuary-room/{name}.png")}");
    }
}

public partial class RoomCaptureInput : PlayerInput
{
    public override Vector2 ReadMovement() => Vector2.Zero;
    public override bool InteractPressed() => false;
}
