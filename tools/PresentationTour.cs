using System;
using System.Threading.Tasks;
using Godot;
using Hollowlight.World;

namespace Hollowlight.Tools;

// Reproducible captures of the live renderer, without capturing desktop contents.
public partial class PresentationTour : Node
{
    private PlayerCharacter _actor = null!;
    private SmokeInput _input = null!;
    private SceneTree _tree = null!;
    public override async void _Ready()
    {
        _tree = GetTree();
        try
        {
            var world = GD.Load<PackedScene>("res://scenes/Threshold.tscn").Instantiate<ThresholdWorld>();
            _actor = world.GetNode<PlayerCharacter>("Player");
            _input = new SmokeInput { Name = "TourInput" };
            _actor.AddChild(_input);
            _actor.InputPath = new NodePath("TourInput");
            AddChild(world);
            await ToSignal(_tree.CreateTimer(6.2), SceneTreeTimer.SignalName.Timeout);
            await Capture("refuge", new Vector2(136, 208));
            await Capture("workshop", new Vector2(144, 112));
            await Capture("storage", new Vector2(88, 242));
            _input.Movement = Vector2.Right;
            await Capture("threshold", new Vector2(288, 176));
            _input.Movement = Vector2.Left;
            await Capture("surface", new Vector2(480, 176));
            await Capture("salvage", new Vector2(432, 178));
            GD.Print("PASS: presentation tour captures saved to user://presentation");
            _tree.Quit();
        }
        catch (Exception error) { GD.PushError(error.ToString()); _tree.Quit(1); }
    }
    private async Task Capture(string name, Vector2 position)
    {
        _actor.Position = position;
        for (int i = 0; i < 12; i++) await ToSignal(_tree, SceneTree.SignalName.ProcessFrame);
        _input.Movement = Vector2.Zero;
        await ToSignal(RenderingServer.Singleton, RenderingServer.SignalName.FramePostDraw);
        DirAccess.MakeDirRecursiveAbsolute(ProjectSettings.GlobalizePath("user://presentation"));
        bool alternate = Array.Exists(OS.GetCmdlineUserArgs(), arg => arg == "--sanctuary-variant");
        string path = $"user://presentation/{(alternate ? "alternate-" : "")}{name}.png";
        Error result = GetViewport().GetTexture().GetImage().SavePng(path);
        if (result != Error.Ok) throw new InvalidOperationException($"Capture failed: {result}");
        GD.Print($"Capture: {ProjectSettings.GlobalizePath(path)}");
    }
}
