using System;
using System.Threading.Tasks;
using Godot;
using Hollowlight.Core;
using Hollowlight.Presentation;
using Hollowlight.World;

namespace Hollowlight.Tools;

// Drives real commands for graphical review, including a one-slot chest fixture.
public partial class SuppliesReview : Node
{
    private ThresholdWorld _world = null!;
    private int Stored => _world.StoredTotal - _world.StartingStoredTotal;
    private PlayerCharacter _actor = null!;
    private readonly SmokeInput _input = new() { Name = "ReviewInput" };
    public override async void _Ready()
    {
        try
        {
            bool small = Array.Exists(OS.GetCmdlineUserArgs(), arg => arg == "--small-storage");
            if (small)
            {
                SanctuaryRoom.ConfigureWindow(GetWindow());
                _world = GD.Load<PackedScene>("res://scenes/Threshold.tscn").Instantiate<ThresholdWorld>();
                _world.Config = (WorldConfig)_world.Config.Duplicate();
                _world.Config.StorageSlots = 1;
                _world.Config.FarmingEnabled = false;
                _world.Config.PowerEnabled = false;
                _world.Config.HungerEnabled = false;
                _world.Config.ScavengingEnabled = false;
                _actor = _world.GetNode<PlayerCharacter>("Player");
                _actor.AddChild(_input);
                _actor.InputPath = new NodePath("ReviewInput");
                AddChild(_world);
                AddChild(new RoomPresentation { World = _world });
            }
            else
            {
                string entry = ProjectSettings.GetSetting("application/run/main_scene").AsString();
                var room = GD.Load<PackedScene>(entry).Instantiate<SanctuaryRoom>();
                room.PowerEnabled = false;
                room.HungerEnabled = false;
                room.ScavengingEnabled = false;
                room.InputOverride = _input;
                AddChild(room);
                _world = room.World;
                _actor = _world.GetNode<PlayerCharacter>("Player");
            }
            await Frames(4);
            await InteractAt(new Vector2(432, 178));
            Check(_world.CarriedBy(1) == 12, "Gather twelve Scrap into two stacks");
            if (small)
            {
                await InteractAt(new Vector2(88, 242));
                _world.InventoryPanel.StoreAll(); await Frames(2);
                Check(Stored == 10 && _world.CarriedBy(1) == 2, "One-slot chest accepts ten and leaves two carried");
                await InteractAt(new Vector2(528, 130));
                await InteractAt(new Vector2(88, 242));
                _world.InventoryPanel.StoreAll(); await Frames(2);
                Check(Stored == 10 && _world.CarriedBy(1) == 10, "Full chest preserves both carried resources");
                await Capture("full-storage");
                var version = _world.SuppliesFor(1).Version;
                Check(_world.TransferSupplies(_actor, "scrap", 50, true, version)?.Amount == 10, "Partial withdrawal limited by actual chest stock");
                Check(_world.TransferSupplies(_actor, "organic", 20, false, _world.SuppliesFor(1).Version)?.Amount == 8,
                    "Other resource can use emptied chest slot");
                Check(_world.TransferSupplies(_actor, "scrap", 1, false, _world.SuppliesFor(1).Version)?.Status == SupplyStatus.NoCapacity,
                    "Partially filled Organic stack cannot accept Scrap");
            }
            else
            {
                await InteractAt(new Vector2(528, 130));
                await InteractAt(new Vector2(512, 274));
                SendKey(Key.Tab);
                await Capture("personal");
                await InteractAt(new Vector2(88, 242));
                _world.InventoryPanel.StoreAll(); await Frames(2);
                Check(Stored == 34 && _world.CarriedBy(1) == 0, "All finite supplies deposited");
                await Capture("storage");
                _world.TransferSupplies(_actor, "organic", 10, true, _world.SuppliesFor(1).Version);
                await Frames(2);
                Check(_world.SuppliesFor(1).Personal.Count("organic") == 8 && Stored == 26,
                    "Withdrawal partially supplies requested ten Organic");
                await Capture("withdrawal");
                _actor.Position = new Vector2(136, 208);
                await Frames(2);
                Check(_world.TransferSupplies(_actor, "organic", 1, true, _world.SuppliesFor(1).Version) == null, "Closed chest denies old transfer API");
                await Frames(2);
                Check(_world.CarriedBy(1) == 8 && Stored == 26, "Open panel cannot withdraw after leaving chest");
            }
            GD.Print("PASS: graphical supplies review and actual command checks");
            GetTree().Quit();
        }
        catch (Exception error) { GD.PushError(error.ToString()); GetTree().Quit(1); }
    }
    private async Task InteractAt(Vector2 position)
    {
        _world.CloseInventory();
        _actor.Position = position;
        await Frames(2);
        _input.Tap = true;
        await Frames(2);
    }
    private void SendKey(Key key, bool shift = false) => _world._UnhandledKeyInput(
        new InputEventKey { PhysicalKeycode = key, Pressed = true, ShiftPressed = shift });
    private async Task Frames(int count)
    {
        for (int i = 0; i < count; i++)
        {
            await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);
            await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        }
    }
    private async Task Capture(string name)
    {
        await Frames(3);
        await ToSignal(RenderingServer.Singleton, RenderingServer.SignalName.FramePostDraw);
        string folder = ProjectSettings.GlobalizePath("user://supplies-review");
        DirAccess.MakeDirRecursiveAbsolute(folder);
        Check(GetViewport().GetTexture().GetImage().SavePng($"{folder}/{name}.png") == Error.Ok, $"Capture {name}");
    }
    private static void Check(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
        GD.Print($"PASS: {message}");
    }
}
