using System;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Hollowlight.Core;
using Hollowlight.Presentation;
using Hollowlight.World;

namespace Hollowlight.Tools;

public partial class HungerSmoke : Node
{
    private static bool _reset;
    private readonly SmokeInput _input = new();
    private ThresholdWorld _world = null!;
    private PlayerCharacter _actor = null!;
    private SuppliesPanel Panel => _world.InventoryPanel;
    private HungerSnapshot Meal => _world.HungerFor(1)!;
    private SuppliesSnapshot Stock => _world.SuppliesFor(1);
    public override async void _Ready()
    {
        try
        {
            var room = GD.Load<PackedScene>(ProjectSettings.GetSetting("application/run/main_scene").AsString()).Instantiate<SanctuaryRoom>();
            room.InputOverride = _input; AddChild(room); _world = room.World; _actor = _world.GetNode<PlayerCharacter>("Player");
            await Frames(3);
            Check(Meal.Stage == HungerStage.Fed && Meal.Eaten == 0 && Stock.Personal.Count("food") == 2 && _world.GrowingPlots.Count == 4,
                "Integrated defaults: two personal Food, Fed, four unchanged growing plots");
            if (_reset)
            {
                Check(Meal.Consumed == 0 && Meal.MissedMeals == 0 && Meal.Day == 1 && !_world.InventoryVisible && _actor.HungerSpeedMultiplier == 1,
                    "Reset restores hunger, clock, Food and movement");
                GD.Print("PASS: integrated hunger UI, inputs, automatic meals, stale gestures, movement, farming, time and reset"); GetTree().Quit(); return;
            }
            _actor.Position = new Vector2(400, 208);
            _input.Movement = Vector2.Right; await Frames(3); _input.Movement = Vector2.Zero;
            Check(Math.Abs(_actor.Velocity.Length() - 80) < .01, "Fed movement preserves original speed");
            await PressKey(Key.Tab); Panel.FocusSlot(new(InventoryKind.Personal, 0)); await Frames(2);
            Check(_world.InventoryVisible && !Panel.ChestOpen, "Food can be eaten from backpack away from communal chest");
            await Capture("backpack");
            _world.CloseInventory(); await InteractAt(new Vector2(88, 242));
            Panel.FocusSlot(new(InventoryKind.Personal, 0)); await Capture("chest");
            _world.CloseInventory(); _actor.Position = new Vector2(400, 208); await PressKey(Key.Tab);
            await PressKey(Key.F);
            Check(Meal.Eaten == 1 && Stock.Personal.Count("food") == 1, "Keyboard F eats selected personal Food");
            await PressKey(Key.F); Check(Stock.Personal.Count("food") == 1, "Repeated Eat cannot waste tomorrow's Food");
            await PressKey(Key.F5); Check(Meal.Consumed == 1 && Meal.Day == 2 && _actor.Velocity == Vector2.Zero,
                "Time advances while inventory open without double eating");
            await Joy(JoyButton.RightStick); Check(Meal.Eaten == 1 && Stock.Personal.Count("food") == 0, "Assigned controller RS eats without changing transfer bindings");
            await PressKey(Key.F5); Check(Meal.Consumed == 2 && Meal.MissedMeals == 0, "Controller meal credits daily check");
            _world._UnhandledKeyInput(new InputEventKey { PhysicalKeycode = Key.F5, Pressed = true, ShiftPressed = true }); await Frames(3);
            Check(Meal.MissedMeals == 3 && Meal.Stage == HungerStage.Critical && Math.Abs(_actor.HungerSpeedMultiplier - .8f) < .001,
                "Developer skips settle every missed deadline and apply capped penalty");
            _world.CloseInventory(); _actor.Position = new Vector2(400, 208);
            _input.Movement = Vector2.One.Normalized(); await Frames(3);
            Check(Math.Abs(_actor.Velocity.Length() - 64) < .01 && _actor.Velocity.X > 0 && _actor.Velocity.Y > 0,
                "Critical penalty preserves normalized diagonal movement");
            _input.Movement = Vector2.Zero;
            // Integrated farm: withdraw starter seeds, plant, explore, harvest, eat.
            await InteractAt(new Vector2(88, 242));
            Check(_world.TransferSupplies(_actor, "potato_seed", 1, true, Stock.Version)?.Amount == 1, "Hunger preserves chest seed withdrawals");
            _world.CloseInventory();
            var plot = _world.GrowingPlots[0]; _actor.Position = plot.GlobalPosition + new Vector2(0, 12); await Frames(3);
            Check(_world.UsePlot(_actor, plot.PlotId, "cave_potato", _world.Plot(plot.PlotId).Version).Status == FarmStatus.Planted, "Hunger preserves planting seed cost");
            _actor.Position = new Vector2(400, 208); _world.Clock.Advance(1201); await Frames(3);
            Check(_world.Plot(plot.PlotId).Stage == CropStage.Mature && Meal.MissedMeals == 5, "Direct clock advancement grows crops and settles hunger during exploration");
            _actor.Position = plot.GlobalPosition + new Vector2(0, 12); await Frames(3);
            Check(_world.UsePlot(_actor, plot.PlotId, "cave_potato", _world.Plot(plot.PlotId).Version).Status == FarmStatus.Harvested && Stock.Personal.Count("food") == 3,
                "Unchanged three-Food potato harvest fits personal inventory");
            await PressKey(Key.Tab);
            int foodSlot = Stock.Personal.Slots.ToList().FindIndex(x => x?.ResourceId == "food"); Panel.FocusSlot(new(InventoryKind.Personal, foodSlot)); await Frames(2);
            await Capture("critical");
            var eatButton = Descendants(Panel).OfType<Button>().Single(b => b.Text == "Eat  F / RS");
            // Pointer-down captures identity/version. A deadline before release must reject it.
            await Mouse(eatButton, true); _world.Clock.Advance(600); await Frames(2); await Mouse(eatButton, false);
            Check(Meal.Eaten == 0 && Stock.Personal.Count("food") == 2, "Dawn invalidates a held mouse Eat gesture instead of consuming twice");
            Panel.FocusSlot(new(InventoryKind.Personal, foodSlot)); await Frames(2);
            await Mouse(eatButton, true); await Mouse(eatButton, false);
            Check(Meal.Eaten == 1 && Stock.Personal.Count("food") == 1 && Meal.MissedMeals == 3, "Mouse eating recovers one miss after automatic recovery; prolonged hunger remains");
            await Capture("recovery");
            long debt = Meal.MissedMeals; _world.Clock.Advance(600); await Frames(2);
            Check(Meal.MissedMeals == debt && Meal.Eaten == 0, "Settling credited meal does not grant a second recovery");
            await PressKey(Key.F); Check(Meal.MissedMeals == 2 && Meal.Stage == HungerStage.Starving && Math.Abs(_actor.HungerSpeedMultiplier - .9f) < .001,
                "Following daily meal steps down to Starving without instantly clearing streak");
            _world.CloseInventory(); await Capture("world");
            _reset = true; _world._UnhandledKeyInput(new InputEventKey { PhysicalKeycode = Key.R, Pressed = true });
        }
        catch (Exception error) { GD.PushError(error.ToString()); GetTree().Quit(1); }
    }
    private static System.Collections.Generic.IEnumerable<Node> Descendants(Node node)
    { foreach (var child in node.GetChildren()) { yield return child; foreach (var nested in Descendants(child)) yield return nested; } }
    private async Task Mouse(Button button, bool pressed)
    {
        var point = GetViewport().GetFinalTransform() * (button.GetGlobalTransformWithCanvas() * (button.Size / 2));
        Input.ParseInputEvent(new InputEventMouseMotion { Position = point, GlobalPosition = point });
        await Frames(1);
        Input.ParseInputEvent(new InputEventMouseButton { Position = point, GlobalPosition = point, ButtonIndex = MouseButton.Left,
            Pressed = pressed, ButtonMask = pressed ? MouseButtonMask.Left : 0 }); await Frames(2);
    }
    private async Task InteractAt(Vector2 position) { _actor.Position = position; await Frames(3); _input.Tap = true; await Frames(3); }
    private async Task PressKey(Key key)
    { Input.ParseInputEvent(new InputEventKey { PhysicalKeycode = key, Pressed = true }); await Frames(2);
      Input.ParseInputEvent(new InputEventKey { PhysicalKeycode = key, Pressed = false }); await Frames(2); }
    private async Task Joy(JoyButton button)
    { Input.ParseInputEvent(new InputEventJoypadButton { Device = 0, ButtonIndex = button, Pressed = true }); await Frames(2);
      Input.ParseInputEvent(new InputEventJoypadButton { Device = 0, ButtonIndex = button, Pressed = false }); await Frames(2); }
    private async Task Frames(int count)
    { for (int i = 0; i < count; i++) { await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame); await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame); } }
    private async Task Capture(string name)
    {
        if (!Array.Exists(OS.GetCmdlineUserArgs(), arg => arg == "--capture-hunger")) return;
        await Frames(3); await ToSignal(RenderingServer.Singleton, RenderingServer.SignalName.FramePostDraw);
        string folder = ProjectSettings.GlobalizePath("user://hunger"); DirAccess.MakeDirRecursiveAbsolute(folder);
        Check(GetViewport().GetTexture().GetImage().SavePng($"{folder}/{name}.png") == Error.Ok, $"Capture {name}");
    }
    private static void Check(bool value, string message)
    { if (!value) throw new InvalidOperationException(message); GD.Print($"PASS: {message}"); }
}
