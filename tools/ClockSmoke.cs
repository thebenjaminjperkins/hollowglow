using System;
using System.Threading.Tasks;
using Godot;
using Hollowlight.Core;
using Hollowlight.Presentation;
using Hollowlight.World;

namespace Hollowlight.Tools;

public partial class ClockSmoke : Node
{
    private static bool _resetRequested;
    public override async void _Ready()
    {
        try
        {
            var input = new SmokeInput();
            var room = GD.Load<PackedScene>(ProjectSettings.GetSetting("application/run/main_scene").AsString()).Instantiate<SanctuaryRoom>();
            room.HungerEnabled = false;
            room.ScavengingEnabled = false;
            room.InputOverride = input; AddChild(room);
            var world = room.World; var actor = world.GetNode<PlayerCharacter>("Player");
            await Frames(3);
            if (_resetRequested)
            {
                Check(world.Clock.Snapshot().DayNumber == 1 && world.Clock.Snapshot().TotalSeconds < 1 && !world.InventoryVisible,
                    "Reset restarts clock and clears modal input");
                GD.Print("PASS: live clock continuity, exploration/inventory time, developer skips and reset"); GetTree().Quit(); return;
            }
            long before = world.Clock.ElapsedTicks;
            input.Movement = Vector2.Right;
            await Frames(60); input.Movement = Vector2.Zero;
            Check(Math.Abs((double)(world.Clock.ElapsedTicks - before) / TimeSpan.TicksPerSecond - 1) < 0.02,
                "Time advances at physics rate during exploration");
            actor.Position = new Vector2(432, 178); await Frames(2); input.Tap = true; await Frames(2);
            SendKey(world, Key.Tab); await Frames(2);
            before = world.Clock.ElapsedTicks;
            var version = world.SuppliesFor(1).Version;
            world.InventoryPanel.Activate(new(InventoryKind.Personal, 0));
            await Frames(120);
            Check(Math.Abs((double)(world.Clock.ElapsedTicks - before) / TimeSpan.TicksPerSecond - 2) < 0.02 && actor.Velocity == Vector2.Zero,
                "Clock keeps advancing while inventory owner is stationary");
            long day = world.Clock.Snapshot().DayNumber;
            SendKey(world, Key.F5); await Frames(2);
            Check(world.Clock.Snapshot().DayNumber == day + 1 && world.SuppliesFor(1).Version == version && world.InventoryPanel.Selected != null,
                "Day skip advances time without invalidating inventory previews or supplies");
            before = world.Clock.ElapsedTicks;
            SendKey(world, Key.F4); await Frames(2);
            Check(Math.Abs((double)(world.Clock.ElapsedTicks - before) / TimeSpan.TicksPerSecond - 60) < 0.05,
                "Minute developer control works while managing inventory");
            day = world.Clock.Snapshot().DayNumber;
            world._UnhandledKeyInput(new InputEventKey { PhysicalKeycode = Key.F5, Pressed = true, ShiftPressed = true }); await Frames(2);
            Check(world.Clock.Snapshot().DayNumber == day + 3, "Three-day developer skip");
            world.CloseInventory(); actor.Position = new Vector2(136, 208);
            long inCycle = world.Clock.ElapsedTicks % world.Clock.CycleTicks;
            world.Clock.Advance(360 - (double)inCycle / TimeSpan.TicksPerSecond); await Frames(2);
            Check(world.Clock.Snapshot().Phase == DayPhase.Dusk, "Live dusk boundary");
            await Capture("dusk");
            world.Clock.Advance(60); await Frames(2);
            Check(world.Clock.Snapshot().Phase == DayPhase.Night, "Live night boundary");
            await Capture("night");
            _resetRequested = true; SendKey(world, Key.R);
        }
        catch (Exception error) { GD.PushError(error.ToString()); GetTree().Quit(1); }
    }
    private static void SendKey(ThresholdWorld world, Key key) => world._UnhandledKeyInput(new InputEventKey { PhysicalKeycode = key, Pressed = true });
    private async Task Frames(int count)
    {
        for (int i = 0; i < count; i++)
        { await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame); await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame); }
    }
    private async Task Capture(string name)
    {
        if (!Array.Exists(OS.GetCmdlineUserArgs(), arg => arg == "--capture-clock")) return;
        await Frames(3); await ToSignal(RenderingServer.Singleton, RenderingServer.SignalName.FramePostDraw);
        string folder = ProjectSettings.GlobalizePath("user://clock"); DirAccess.MakeDirRecursiveAbsolute(folder);
        Check(GetViewport().GetTexture().GetImage().SavePng($"{folder}/{name}.png") == Error.Ok, $"Capture {name}");
    }
    private static void Check(bool value, string message)
    { if (!value) throw new InvalidOperationException(message); GD.Print($"PASS: {message}"); }
}
