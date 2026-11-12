using Godot;
using Hollowlight.Presentation;

namespace Hollowlight.Tools;

public partial class ArtBenchmark : Node2D
{
    private readonly System.Collections.Generic.List<ReactiveFern> _ferns = new();
    private readonly System.Collections.Generic.List<CharacterBody2D> _actors = new();
    private bool _walking = true;
    private double _clock;
    private bool _captured;
    public override void _Ready()
    {
        AddChild(new ColorRect { Size = new Vector2(384, 216), Color = new Color("202832"), MouseFilter = Control.MouseFilterEnum.Ignore });
        Text("HOLLOWLIGHT / FOUR-ASSET BENCHMARK", new Vector2(8, 5), 11);
        Text("S / N / W / E", new Vector2(14, 25), 8);
        Text("MACHINE", new Vector2(134, 25), 8);
        Text("FERN", new Vector2(200, 25), 8);
        Text("WALL", new Vector2(279, 25), 8);
        BuildRow(75, false);
        BuildRow(159, true);
        Text("SPACE brush fern   1 walk / idle   Compare at 1x and 3x", new Vector2(8, 199), 8);
    }
    private void Text(string text, Vector2 position, int size)
    {
        var label = new Label { Text = text, Position = position };
        label.AddThemeFontSizeOverride("font_size", size);
        label.AddThemeColorOverride("font_color", new Color("cdd0ae"));
        AddChild(label);
    }
    private void BuildRow(int y, bool warm)
    {
        AddChild(new ColorRect { Position = new Vector2(7, y - 38), Size = new Vector2(370, 68),
            Color = new Color(warm ? "303842" : "283a40"), MouseFilter = Control.MouseFilterEnum.Ignore });
        var group = new Node2D { Position = new Vector2(0, y) };
        if (!warm) group.Material = new CanvasItemMaterial { LightMode = CanvasItemMaterial.LightModeEnum.Unshaded };
        AddChild(group);
        var directions = new[] { Vector2.Down, Vector2.Up, Vector2.Left, Vector2.Right };
        for (int i = 0; i < 4; i++)
        {
            var actor = new CharacterBody2D { Position = new Vector2(24 + i * 23, 0), Velocity = directions[i] * 80,
                CollisionLayer = 0, CollisionMask = 0, UseParentMaterial = true };
            actor.AddChild(new CharacterArt { UseParentMaterial = true });
            group.AddChild(actor);
            _actors.Add(actor);
        }
        group.AddChild(new BenchmarkSprite { Position = new Vector2(153, 0), UseParentMaterial = true });
        var fern = new ReactiveFern { Position = new Vector2(211, 0), UseParentMaterial = true };
        group.AddChild(fern);
        _ferns.Add(fern);
        group.AddChild(new WallSample { Position = new Vector2(265, -30), UseParentMaterial = true });
        if (warm)
        {
            var gradient = new Gradient { Colors = new[] { Colors.White, new Color(1, 1, 1, 0) } };
            group.AddChild(new PointLight2D { Position = new Vector2(154, -12), Color = new Color("d4ad73"), Energy = 0.3f,
                TextureScale = 2.4f, Texture = new GradientTexture2D {
                    Width = 128, Height = 128, Gradient = gradient, Fill = GradientTexture2D.FillEnum.Radial,
                    FillFrom = new Vector2(0.5f, 0.5f), FillTo = new Vector2(1, 0.5f)
                } });
        }
        Text(warm ? "AMBER LIGHT" : "NEUTRAL / SOURCE COLORS", new Vector2(14, y + 9), 8);
    }
    public override async void _Process(double delta)
    {
        _clock += delta;
        if (!_captured && _clock > 1 && System.Array.Exists(OS.GetCmdlineUserArgs(), arg => arg == "--capture-benchmark"))
        {
            _captured = true;
            await ToSignal(RenderingServer.Singleton, RenderingServer.SignalName.FramePostDraw);
            var error = GetViewport().GetTexture().GetImage().SavePng("user://art-benchmark.png");
            GD.Print($"Benchmark capture: {ProjectSettings.GlobalizePath("user://art-benchmark.png")} ({error})");
            GetTree().Quit(error == Error.Ok ? 0 : 1);
        }
    }
    public override void _UnhandledKeyInput(InputEvent @event)
    {
        if (@event is not InputEventKey key || !key.Pressed || key.Echo) return;
        if (key.PhysicalKeycode == Key.Space) foreach (var fern in _ferns) fern.Brush(1);
        if (key.PhysicalKeycode == Key.Key1)
        {
            _walking = !_walking;
            var directions = new[] { Vector2.Down, Vector2.Up, Vector2.Left, Vector2.Right };
            for (int i = 0; i < _actors.Count; i++) _actors[i].Velocity = _walking ? directions[i % 4] * 80 : Vector2.Zero;
        }
    }
}

public partial class WallSample : Node2D
{
    public override void _Ready() => TextureFilter = TextureFilterEnum.Nearest;
    public override void _Draw()
    {
        var atlas = GD.Load<Texture2D>("res://assets/benchmark/wall.png");
        for (int y = 0; y < 2; y++)
        for (int x = 0; x < 4; x++)
        {
            int tile = x % 3 + y * 3;
            DrawTextureRectRegion(atlas, new Rect2(x * 16, y * 16, 16, 16), new Rect2(tile % 4 * 16, tile / 4 * 16, 16, 16));
        }
    }
}
