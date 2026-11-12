using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Godot;
using Hollowlight.Presentation;
using Hollowlight.World;

namespace Hollowlight.Tools;

// Separate process-local presentation trial. No project settings or production scenes are saved.
public partial class MachineryTrial : Node
{
    private ThresholdWorld _world = null!;
    private PlayerCharacter _actor = null!;
    private Camera2D _camera = null!;
    private SanctuaryProp _machine = null!;
    private BenchmarkSprite _original = null!;
    private TrialMachineArt _candidate = null!;
    private Label _caption = null!;
    private bool _showCandidate = true;
    private bool _capture;
    private SceneTree _tree = null!;
    private readonly Dictionary<string, object> _audit = new();

    public override async void _Ready()
    {
        _tree = GetTree();
        try
        {
            var window = GetWindow();
            window.ContentScaleSize = new Vector2I(768, 432);
            window.ContentScaleMode = Window.ContentScaleModeEnum.CanvasItems;
            window.ContentScaleStretch = Window.ContentScaleStretchEnum.Integer;
            window.Size = new Vector2I(1536, 864);
            _capture = Array.Exists(OS.GetCmdlineUserArgs(), arg => arg == "--capture-machinery");
            _world = GD.Load<PackedScene>("res://scenes/Threshold.tscn").Instantiate<ThresholdWorld>();
            _actor = _world.GetNode<PlayerCharacter>("Player");
            SmokeInput? input = null;
            if (_capture)
            {
                input = new SmokeInput { Name = "TrialInput" };
                _actor.AddChild(input);
                _actor.InputPath = new NodePath("TrialInput");
            }
            AddChild(_world);
            _camera = _actor.GetNode<Camera2D>("Camera2D");
            _camera.Zoom = Vector2.One * 2;
            _world.GetNode<ThresholdHud>("HUD").Scale = Vector2.One * 2;
            foreach (var child in _world.GetNode<ThresholdMap>("Map").GetChildren())
                if (child is SanctuaryProp prop && prop.Kind == PixelSprite.Motif.Generator) _machine = prop;
            if (_machine == null) throw new InvalidOperationException("Expected existing machine prop");
            foreach (var child in _machine.GetChildren()) if (child is BenchmarkSprite sprite) _original = sprite;
            _candidate = new TrialMachineArt();
            _machine.AddChild(_candidate);
            var overlay = new CanvasLayer { Layer = 10 };
            _caption = new Label { Position = new Vector2(16, 406) };
            _caption.AddThemeFontSizeOverride("font_size", 14);
            _caption.AddThemeColorOverride("font_color", new Color("f1dfad"));
            _caption.AddThemeColorOverride("font_shadow_color", new Color("202832"));
            _caption.AddThemeConstantOverride("shadow_offset_y", 2);
            overlay.AddChild(_caption);
            AddChild(overlay);
            SetCandidate(true);
            Check(_actor.Config.MoveSpeed == 80 && _actor.Config.InteractionRange == 22,
                "Movement speed 80 and interaction range 22 remain world-space values");
            var feet = (RectangleShape2D)_actor.GetNode<CollisionShape2D>("CollisionShape2D").Shape;
            Check(feet.Size == new Vector2(8, 6), "Original player feet collider preserved");
            Check(_machine.Footprint == new Vector2(26, 9), "Original machine footprint 26x9 preserved");
            _actor.Position = new Vector2(144, 112);
            _actor.Velocity = Vector2.Zero;
            _camera.ResetSmoothing();
            if (_capture && input != null) await CaptureComparison(input);
        }
        catch (Exception error) { GD.PushError($"Machinery trial: {error}"); _tree.Quit(1); }
    }

    private void SetCandidate(bool show)
    {
        _showCandidate = show;
        _original.Visible = !show;
        _candidate.Visible = show;
        _caption.Text = (show ? "CANDIDATE 64px" : "ORIGINAL 32px") +
            "  |  T compare  N 1x/2x  |  WASD move  E interact  R reset";
    }

    private async Task CaptureComparison(SmokeInput input)
    {
        await Frames(3);
        Vector2 coverage = GetViewport().GetVisibleRect().Size / _camera.Zoom;
        Check(coverage.DistanceTo(new Vector2(384, 216)) < 0.01f, "Camera still covers 384x216 world units");
        _audit["world_coverage"] = new[] { coverage.X, coverage.Y };
        _audit["native_render_grid"] = new[] { 768, 432 };
        _audit["display_scale"] = 2;
        _audit["camera_zoom"] = 2;
        _audit["candidate_world_draw_rect"] = new[] { -16, -29, 32, 32 };
        _actor.Position = new Vector2(136, 208);
        input.Movement = Vector2.Right;
        await Frames(60);
        input.Movement = Vector2.Zero;
        Check(Mathf.Abs(_actor.Position.X - 216) < 2, "Horizontal movement speed unchanged in higher-resolution trial");
        _actor.Position = new Vector2(136, 208);
        input.Movement = Vector2.One;
        await Frames(30);
        input.Movement = Vector2.Zero;
        Check(Mathf.Abs(_actor.Position.DistanceTo(new Vector2(136, 208)) - 40) < 2, "Normalized diagonal travel unchanged");
        _actor.Position = new Vector2(144, 112);
        _actor.Velocity = Vector2.Zero;
        _camera.ResetSmoothing();
        await ToSignal(_tree.CreateTimer(6.2), SceneTreeTimer.SignalName.Timeout);
        SetCandidate(false);
        await Capture("original");
        SetCandidate(true);
        await Capture("candidate");
        GetWindow().Size = new Vector2I(768, 432);
        SetCandidate(false);
        await Capture("original-native");
        SetCandidate(true);
        await Capture("candidate-native");
        Check((GetViewport().GetVisibleRect().Size / _camera.Zoom).DistanceTo(new Vector2(384, 216)) < 0.01f,
            "Native-window comparison retains the same world coverage");
        Check(_world.CarriedBy(1) == 0 && _world.StoredTotal == _world.StartingStoredTotal, "Presentation switching does not mutate resource counts");
        FileAccess file = FileAccess.Open("user://machinery-trial/audit.json", FileAccess.ModeFlags.Write);
        file.StoreString(JsonSerializer.Serialize(_audit, new JsonSerializerOptions { WriteIndented = true }));
        file.Dispose();
        GD.Print("PASS: separate machinery comparison, world measurements and captures");
        _tree.Quit();
    }

    private async Task Frames(int count)
    {
        for (int i = 0; i < count; i++)
        {
            await ToSignal(_tree, SceneTree.SignalName.PhysicsFrame);
            await ToSignal(_tree, SceneTree.SignalName.ProcessFrame);
        }
    }

    private async Task Capture(string name)
    {
        await Frames(3);
        await ToSignal(RenderingServer.Singleton, RenderingServer.SignalName.FramePostDraw);
        var image = GetViewport().GetTexture().GetImage();
        DirAccess.MakeDirRecursiveAbsolute(ProjectSettings.GlobalizePath("user://machinery-trial"));
        Error result = image.SavePng($"user://machinery-trial/{name}.png");
        Check(result == Error.Ok, $"Saved {name} at intended gameplay zoom");
        var logicalAnchor = _machine.GetGlobalTransformWithCanvas().Origin;
        float displayScale = image.GetWidth() / GetViewport().GetVisibleRect().Size.X;
        _audit[name + "_machine_screen_anchor"] = new[] { logicalAnchor.X * displayScale, logicalAnchor.Y * displayScale };
        _audit[name + "_image_size"] = new[] { image.GetWidth(), image.GetHeight() };
        _audit[name + "_actor_position"] = new[] { _actor.Position.X, _actor.Position.Y };
        _audit[name + "_display_scale"] = displayScale;
        var center = _camera.GetScreenCenterPosition();
        _audit[name + "_camera_center"] = new[] { center.X, center.Y };
    }

    private void Check(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
        _audit[message] = true;
        GD.Print($"PASS: {message}");
    }

    public override void _UnhandledKeyInput(InputEvent @event)
    {
        if (@event is not InputEventKey key || !key.Pressed || key.Echo) return;
        if (key.PhysicalKeycode == Key.T) { SetCandidate(!_showCandidate); GetViewport().SetInputAsHandled(); }
        if (key.PhysicalKeycode == Key.N)
        {
            GetWindow().Size = GetWindow().Size.X >= 1536 ? new Vector2I(768, 432) : new Vector2I(1536, 864);
            GetViewport().SetInputAsHandled();
        }
    }
}

public partial class TrialMachineArt : Node2D
{
    private Texture2D _texture = null!;
    public override void _Ready()
    {
        TextureFilter = TextureFilterEnum.Nearest;
        _texture = GD.Load<Texture2D>("res://assets/trials/machinery/candidate.png");
    }
    public override void _Draw()
    {
        DrawRect(new Rect2(-12, -1, 24, 4), new Color("20283266"));
        // 64 source pixels occupy the same 32x32 world rectangle as the old PNG.
        DrawTextureRect(_texture, new Rect2(-16, -29, 32, 32), false);
    }
}
