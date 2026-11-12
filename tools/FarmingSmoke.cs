using System;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Hollowlight.Core;
using Hollowlight.Presentation;
using Hollowlight.World;

namespace Hollowlight.Tools;

public partial class FarmingSmoke : Node
{
    private static bool _reset;
    private ThresholdWorld _world = null!;
    private PlayerCharacter _actor = null!;
    private readonly SmokeInput _input = new();
    public override async void _Ready()
    {
        try
        {
            var room = GD.Load<PackedScene>(ProjectSettings.GetSetting("application/run/main_scene").AsString()).Instantiate<SanctuaryRoom>();
            room.HungerEnabled = false;
            room.ScavengingEnabled = false;
            room.InputOverride = _input; AddChild(room);
            _world = room.World; _actor = _world.GetNode<PlayerCharacter>("Player");
            await Frames(3);
            Check(_world.GrowingPlots.Count == 4, "Four growing plots in existing garden beds");
            if (_reset)
            {
                Check(_world.GrowingPlots.All(p => _world.Plot(p.PlotId).Stage == CropStage.Empty) && _world.CarriedBy(1) == 0 &&
                    _world.SuppliesFor(1).Storage.Count("potato_seed") == 2 && _world.SuppliesFor(1).Storage.Count("glowcap_seed") == 2 &&
                    _world.Clock.Snapshot().DayNumber == 1, "Reset restores empty plots, shared starter seeds, personal inventory and clock");
                GD.Print("PASS: complete farming loop, contextual crop selection, passive growth, exact harvests, full-pack safety, replant and reset");
                GetTree().Quit(); return;
            }
            await AtPlot(0);
            Input.ParseInputEvent(new InputEventJoypadButton { Device = 0, ButtonIndex = JoyButton.RightShoulder, Pressed = true });
            await Frames(2);
            Check(_world.GetNode<ThresholdHud>("HUD").GetChildren().OfType<Label>().Any(l => l.Text.StartsWith("E / A Plant Glowcap")),
                "Controller shoulder selects crop contextually");
            Input.ParseInputEvent(new InputEventJoypadButton { Device = 0, ButtonIndex = JoyButton.RightShoulder, Pressed = false });
            Input.ParseInputEvent(new InputEventJoypadButton { Device = 0, ButtonIndex = JoyButton.LeftShoulder, Pressed = true });
            await Frames(2);
            Input.ParseInputEvent(new InputEventJoypadButton { Device = 0, ButtonIndex = JoyButton.LeftShoulder, Pressed = false });
            var version = _world.Plot(_world.GrowingPlots[0].PlotId).Version;
            Check(_world.UsePlot(_actor, _world.GrowingPlots[0].PlotId, "cave_potato", version).Status == FarmStatus.NoSeed,
                "Planting requires personal seeds, not shared stock");
            await InteractAt(new Vector2(88, 242));
            foreach (string resource in new[] { "potato_seed", "glowcap_seed" })
                Check(_world.TransferSupplies(_actor, resource, 2, true, _world.SuppliesFor(1).Version)?.Amount == 2, "Withdraw shared starter seeds");
            _world.CloseInventory();
            for (int i = 0; i < 4; i++)
            {
                await AtPlot(i);
                if (i == 2) _world._UnhandledKeyInput(new InputEventKey { PhysicalKeycode = Key.C, Pressed = true });
                _input.Tap = true; await Frames(2);
                Check(_world.Plot(_world.GrowingPlots[i].PlotId).CropId == (i < 2 ? "cave_potato" : "glowcap"), "Contextual E planting and C crop selection");
            }
            Check(_world.SuppliesFor(1).Personal.Count("potato_seed") == 0 && _world.SuppliesFor(1).Personal.Count("glowcap_seed") == 0,
                "All four planting costs consumed exactly");
            await Capture("seedlings");
            var first = _world.GrowingPlots[0].PlotId;
            Check(_world.UsePlot(_actor, _world.GrowingPlots[3].PlotId, "glowcap", _world.Plot(_world.GrowingPlots[3].PlotId).Version).Status == FarmStatus.NotMature,
                "Live premature harvest rejected");
            _world._UnhandledKeyInput(new InputEventKey { PhysicalKeycode = Key.Tab, Pressed = true });
            long before = _world.Clock.ElapsedTicks; double progress = _world.Plot(first).Progress;
            await Frames(120);
            Check(_world.Clock.ElapsedTicks > before && _world.Plot(first).Progress > progress && _world.InventoryVisible,
                "Crops grow during inventory management");
            _world.CloseInventory(); _actor.Position = new Vector2(368, 176); _input.Movement = Vector2.Right;
            progress = _world.Plot(first).Progress; await Frames(60); _input.Movement = Vector2.Zero;
            Check(_actor.Position.X > 440 && _world.Plot(first).Progress > progress, "Crops grow while exploring the surface");
            _world.Clock.Advance(660); await Frames(2);
            Check(_world.GrowingPlots.All(p => _world.Plot(p.PlotId).Stage == CropStage.Growing), "Both crops show growing stage");
            await Capture("growing");
            AdvanceToAge(first, 1201); await Frames(2);
            Check(_world.Plot(first).Stage == CropStage.Mature && _world.Plot(_world.GrowingPlots[2].PlotId).Stage == CropStage.Growing,
                "Potatoes mature before Glowcaps");
            await Capture("potato-mature"); await AtPlot(0);
            var ripe = _world.Plot(first).Version;
            Check(_world.UsePlot(_actor, first, "cave_potato", ripe).Status == FarmStatus.Harvested &&
                _world.SuppliesFor(1).Personal.Count("food") == 3 && _world.SuppliesFor(1).Personal.Count("potato_seed") == 1,
                "Live potato harvest gives three Food and one seed");
            Check(_world.UsePlot(_actor, first, "cave_potato", ripe).Status == FarmStatus.Stale, "Repeated farm request cannot duplicate yield or replant");
            Check(_world.UsePlot(_actor, first, "cave_potato", _world.Plot(first).Version).Status == FarmStatus.Planted,
                "Returned seed supports replanting");
            string glow = _world.GrowingPlots[2].PlotId;
            AdvanceToAge(glow, 1801); await Frames(2); await Capture("glowcap-mature"); await AtPlot(2);
            Check(_world.UsePlot(_actor, glow, "glowcap", _world.Plot(glow).Version).Status == FarmStatus.Harvested &&
                _world.SuppliesFor(1).Personal.Count("food") == 5 && _world.SuppliesFor(1).Personal.Count("organic") == 1 &&
                _world.SuppliesFor(1).Personal.Count("glowcap_seed") == 1, "Live Glowcap harvest gives Food, Organic and seed");
            await InteractAt(new Vector2(432, 178)); await InteractAt(new Vector2(528, 130)); await InteractAt(new Vector2(512, 274));
            Check(_world.SuppliesFor(1).Personal.UsedSlots == 6, "Existing finite gathering fills remaining pack slots");
            await AtPlot(1); string blocked = _world.GrowingPlots[1].PlotId; var original = _world.Plot(blocked);
            Check(_world.UsePlot(_actor, blocked, "cave_potato", original.Version).Status == FarmStatus.NoCapacity &&
                _world.Plot(blocked) == original && _world.SuppliesFor(1).Personal.Count("food") == 5,
                "Full inventory preserves entire live harvest and returned seed");
            await InteractAt(new Vector2(88, 242)); _world.InventoryPanel.StoreAll(); await Frames(2); _world.CloseInventory();
            await AtPlot(1);
            Check(_world.UsePlot(_actor, blocked, "cave_potato", original.Version).Status == FarmStatus.Harvested,
                "Unchanged mature crop harvests after storage transfer frees room");
            _reset = true;
            _world._UnhandledKeyInput(new InputEventKey { PhysicalKeycode = Key.R, Pressed = true });
        }
        catch (Exception error) { GD.PushError(error.ToString()); GetTree().Quit(1); }
    }
    private void AdvanceToAge(string id, double seconds)
    {
        long target = _world.Plot(id).PlantedTicks + (long)(seconds * TimeSpan.TicksPerSecond);
        if (target > _world.Clock.ElapsedTicks) _world.Clock.Advance((double)(target - _world.Clock.ElapsedTicks) / TimeSpan.TicksPerSecond);
    }
    private async Task AtPlot(int index)
    {
        _world.CloseInventory(); _actor.Position = _world.GrowingPlots[index].GlobalPosition + new Vector2(0, 10); await Frames(2);
    }
    private async Task InteractAt(Vector2 pos)
    {
        _world.CloseInventory(); _actor.Position = pos; await Frames(2); _input.Tap = true; await Frames(2);
    }
    private async Task Frames(int count)
    {
        for (int i = 0; i < count; i++)
        { await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame); await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame); }
    }
    private async Task Capture(string name)
    {
        if (!Array.Exists(OS.GetCmdlineUserArgs(), arg => arg == "--capture-farm")) return;
        Vector2 original = _actor.Position;
        _actor.Position = new Vector2(200, 168); await Frames(4);
        await ToSignal(RenderingServer.Singleton, RenderingServer.SignalName.FramePostDraw);
        string folder = ProjectSettings.GlobalizePath("user://farming"); DirAccess.MakeDirRecursiveAbsolute(folder);
        Check(GetViewport().GetTexture().GetImage().SavePng($"{folder}/{name}.png") == Error.Ok, $"Capture {name}");
        _actor.Position = original;
    }
    private static void Check(bool value, string message) { if (!value) throw new InvalidOperationException(message); GD.Print($"PASS: {message}"); }
}
