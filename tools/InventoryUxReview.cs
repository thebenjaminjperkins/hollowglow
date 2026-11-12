using System;
using System.Threading.Tasks;
using Godot;
using Hollowlight.Core;
using Hollowlight.Presentation;
using Hollowlight.World;

namespace Hollowlight.Tools;

public partial class InventoryUxReview : Node
{
    private ThresholdWorld _world = null!;
    private int Stored => _world.StoredTotal - _world.StartingStoredTotal;
    private PlayerCharacter _actor = null!;
    private SuppliesPanel _panel = null!;
    private readonly SmokeInput _input = new();
    private static SlotAddress P(int i) => new(InventoryKind.Personal, i);
    public override async void _Ready()
    {
        try
        {
            var room = GD.Load<PackedScene>(ProjectSettings.GetSetting("application/run/main_scene").AsString()).Instantiate<SanctuaryRoom>();
            room.InputOverride = _input; AddChild(room);
            _world = room.World; _actor = _world.GetNode<PlayerCharacter>("Player"); _panel = _world.InventoryPanel;
            await Frames(3);
            await InteractAt(new Vector2(432, 178)); await InteractAt(new Vector2(528, 130));
            await KeyPress(Key.Tab); await Frames(2);
            Check(_world.InventoryVisible && !_panel.ChestOpen && _actor.InventoryOpen, "Backpack opens anywhere without chest access");
            Check(_panel.Appearance.BackpackColumns == 3 && _panel.Appearance.ChestColumns == 6 && _panel.Appearance.ChestRows == 4,
                "Configured 3x2 backpack and 6x4 chest presentation");
            await KeyPress(Key.Right); Check(_panel.Focused == P(1), "Keyboard focus follows grid");
            await KeyPress(Key.Left); await KeyPress(Key.Enter); await KeyPress(Key.Escape);
            Check(_panel.Selected == null && _world.CarriedBy(1) == 20 && _world.InventoryVisible, "Escape cancels selection without closing or moving items");
            await Click(P(0), MouseButton.Right);
            Check(_panel.Splitting && _panel.SplitQuantity == 5, "Mouse right-click opens half-stack selector");
            await KeyPress(Key.Left); await KeyPress(Key.Enter); await KeyPress(Key.Right); await KeyPress(Key.Down); await KeyPress(Key.Enter);
            await Frames(2);
            Check(_world.SuppliesFor(1).Personal.Slots[4]?.Quantity == 4 && _world.CarriedBy(1) == 20, "Keyboard quantity and destination split conserves items");
            _panel.BeginSplit(P(0)); _panel.ConfirmSplit();
            await Drag(P(0), Center(P(3)));
            Check(_world.SuppliesFor(1).Personal.Slots[0]?.Quantity == 3 && _world.SuppliesFor(1).Personal.Slots[3]?.Quantity == 3,
                "Mouse drag honors selected split quantity and chosen destination");
            _panel.Activate(P(3)); _panel.Activate(P(0)); await Frames(2);
            await Joy(JoyButton.DpadLeft); await Joy(JoyButton.DpadUp);
            Check(_panel.Focused == P(0), "Controller D-pad navigates visual slots");
            await Joy(JoyButton.A); await Joy(JoyButton.B);
            Check(_panel.Selected == null && _world.InventoryVisible, "Controller B cancels preview without losing a stack");
            await Joy(JoyButton.Y); await Joy(JoyButton.DpadRight); await Joy(JoyButton.B);
            Check(!_panel.Splitting && _world.CarriedBy(1) == 20, "Controller quantity selector cancellation preserves items");
            Input.ParseInputEvent(new InputEventJoypadMotion { Device = 0, Axis = JoyAxis.LeftX, AxisValue = 1 });
            await Frames(2);
            Input.ParseInputEvent(new InputEventJoypadMotion { Device = 0, Axis = JoyAxis.LeftX, AxisValue = 0 });
            Check(_panel.Focused != P(0), "Controller stick navigates with repeat threshold");
            var focused = _panel.Focused;
            Input.ParseInputEvent(new InputEventJoypadButton { Device = 1, ButtonIndex = JoyButton.DpadRight, Pressed = true });
            Check(_panel.Focused == focused, "Other controller cannot navigate this actor's inventory");
            _actor.Position = new Vector2(136, 208);
            var other = GD.Load<PackedScene>("res://scenes/Player.tscn").Instantiate<PlayerCharacter>();
            other.PlayerId = 2; other.Position = new Vector2(176, 208);
            other.AddChild(new SmokeInput { Name = "OtherInput", Movement = Vector2.Right }); other.InputPath = new NodePath("OtherInput");
            _world.AddChild(other);
            Vector2 ownStart = _actor.Position, otherStart = other.Position;
            _input.Movement = Vector2.Right; await Frames(30); _input.Movement = Vector2.Zero;
            Check(_actor.Position.DistanceTo(ownStart) < 0.1f && Math.Abs(other.Position.DistanceTo(otherStart) - 40) < 2 && !GetTree().Paused,
                "Only inventory owner stands still; another actor and world continue");
            Check(_actor.CollisionLayer == 2 && _actor.CollisionMask == 1, "Inventory does not disable actor collision");
            other.QueueFree(); await Frames(2);
            await Capture("backpack");
            await Drag(P(4), new Vector2(5, 5));
            Check(_world.CarriedBy(1) == 20 && _panel.Selected == null, "Dropping outside cancels drag without removing source items");
            await KeyPress(Key.Tab); await Frames(2);
            await InteractAt(new Vector2(88, 242));
            Check(_world.ChestAccessible && _panel.ChestOpen && Stored == 0, "Chest interaction opens both panels without auto-deposit");
            await Click(P(0), MouseButton.Left, true);
            Check(Stored == 6 && _world.CarriedBy(1) == 14, "Mouse shift-click quick-transfers selected stack");
            _panel.FocusSlot(P(1)); await Joy(JoyButton.X); await Frames(2);
            Check(Stored == 8, "Controller X quick-transfers selected stack");
            _panel.FocusSlot(P(2)); await KeyPress(Key.Q); await Frames(2);
            Check(Stored == 16, "Keyboard Q quick-transfers selected stack");
            _panel.StoreAll(); await Frames(2);
            Check(Stored == 20 && _world.CarriedBy(1) == 0, "Explicit Store all preserves contents");
            await Capture("chest");
            var blocker = new StaticBody2D { Position = new Vector2(88, 233), CollisionLayer = 1 };
            blocker.AddChild(new CollisionShape2D { Shape = new RectangleShape2D { Size = new Vector2(16, 2) } });
            _world.AddChild(blocker); await Frames(2);
            Check(_world.ChestAccessible, "Minor line-of-sight obstruction does not close established chest session");
            _panel.QuickTransfer(ScrapSlot()); await Frames(2);
            Check(_world.CarriedBy(1) == 10, "Established chest session permits transfer through changed line of sight");
            blocker.QueueFree(); await Frames(2);
            _panel.Activate(P(0));
            var old = _world.SuppliesFor(1).Version;
            var concurrent = new SlotRequest(SlotAction.Move, P(0), P(1), 1, old);
            Check(_world.ApplyInventoryCommand(_actor, concurrent).Amount == 1, "Concurrent state mutation accepted");
            _panel.Activate(P(3));
            Check(_panel.Selected == null && _world.SuppliesFor(1).Personal.Slots[3] == null && _world.CarriedBy(1) == 10,
                "Stale selection rejected without moving wrong slot or losing items");
            await Frames(2);
            _panel.BeginSplit(P(0)); await Capture("split");
            await KeyPress(Key.Escape);
            _actor.Position = new Vector2(136, 208); await Frames(2);
            Check(!_world.InventoryVisible && !_actor.InventoryOpen, "Leaving range closes chest and releases movement");
            Check(_world.ApplyInventoryCommand(_actor, concurrent with { Version = _world.SuppliesFor(1).Version }).Status == SupplyStatus.AccessDenied,
                "Closed chest rejects old UI commands");
            await InteractAt(new Vector2(88, 242));
            Interactable chest = null!;
            foreach (var child in _world.GetChildren()) if (child is Interactable item && item.IsStorage) chest = item;
            chest.Hide(); await Frames(2);
            Check(!_world.InventoryVisible, "Unavailable chest closes session");
            chest.Show(); await Frames(2);
            _panel.Appearance.ChestRows = 2;
            await InteractAt(new Vector2(88, 242));
            await Joy(JoyButton.RightShoulder);
            Check(_panel.Page == 1 && _panel.PageCount == 2 && _world.SuppliesFor(1).Storage.Capacity == 24, "Configurable pages preserve storage capacity");
            _panel.Appearance.ChestRows = 4;
            await Joy(JoyButton.Back); await Frames(2);
            Check(!_world.InventoryVisible && !_actor.InventoryOpen, "Controller View exits session");
            GD.Print("PASS: inventory UX mouse, keyboard, controller events, modal ownership, LOS tolerance, access revocation, configurable paging, cancellation and stale-state handling");
            GetTree().Quit();
        }
        catch (Exception error) { GD.PushError(error.ToString()); GetTree().Quit(1); }
    }
    private async Task KeyPress(Key key)
    {
        Input.ParseInputEvent(new InputEventKey { PhysicalKeycode = key, Keycode = key, Pressed = true });
        await Frames(1);
        Input.ParseInputEvent(new InputEventKey { PhysicalKeycode = key, Keycode = key, Pressed = false });
        await Frames(1);
    }
    private async Task Joy(JoyButton button)
    {
        Input.ParseInputEvent(new InputEventJoypadButton { Device = 0, ButtonIndex = button, Pressed = true });
        await Frames(1);
        Input.ParseInputEvent(new InputEventJoypadButton { Device = 0, ButtonIndex = button, Pressed = false });
        await Frames(1);
    }
    private Vector2 Center(SlotAddress address)
    {
        var cell = _panel.Cell(address) ?? throw new InvalidOperationException("Missing visible slot");
        return GetViewport().GetFinalTransform() * (cell.GetGlobalTransformWithCanvas() * (cell.Size / 2));
    }
    private async Task Click(SlotAddress address, MouseButton button, bool shift = false)
    {
        Vector2 pos = Center(address);
        Input.ParseInputEvent(new InputEventMouseMotion { Position = pos, GlobalPosition = pos });
        await Frames(1);
        Input.ParseInputEvent(new InputEventMouseButton { ButtonIndex = button, Position = pos, GlobalPosition = pos, Pressed = true, ShiftPressed = shift });
        await Frames(1);
        Input.ParseInputEvent(new InputEventMouseButton { ButtonIndex = button, Position = pos, GlobalPosition = pos, Pressed = false, ShiftPressed = shift });
        await Frames(2);
    }
    private async Task Drag(SlotAddress address, Vector2 destination)
    {
        Vector2 start = Center(address);
        Input.ParseInputEvent(new InputEventMouseMotion { Position = start });
        Input.ParseInputEvent(new InputEventMouseButton { ButtonIndex = MouseButton.Left, Position = start, Pressed = true, ButtonMask = MouseButtonMask.Left });
        Input.ParseInputEvent(new InputEventMouseMotion { Position = destination, Relative = destination - start, ButtonMask = MouseButtonMask.Left });
        await Frames(2);
        Check(GetViewport().GuiIsDragging(), "Actual GUI drag preview is active without removing items");
        await Capture("drag-preview");
        Input.ParseInputEvent(new InputEventMouseButton { ButtonIndex = MouseButton.Left, Position = destination, Pressed = false });
        await Frames(2);
    }
    private async Task InteractAt(Vector2 position)
    {
        _actor.Position = position; await Frames(2); _input.Tap = true; await Frames(2);
    }
    private SlotAddress ScrapSlot()
    {
        var slots = _world.SuppliesFor(1).Storage.Slots;
        for (int i = 0; i < slots.Count; i++) if (slots[i]?.ResourceId == "scrap") return new(InventoryKind.Storage, i);
        throw new InvalidOperationException("No Scrap in storage");
    }
    private async Task Frames(int count)
    {
        for (int i = 0; i < count; i++)
        { await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame); await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame); }
    }
    private async Task Capture(string name)
    {
        if (!Array.Exists(OS.GetCmdlineUserArgs(), arg => arg == "--capture-ux")) return;
        await Frames(3); await ToSignal(RenderingServer.Singleton, RenderingServer.SignalName.FramePostDraw);
        string folder = ProjectSettings.GlobalizePath("user://inventory-ux"); DirAccess.MakeDirRecursiveAbsolute(folder);
        Check(GetViewport().GetTexture().GetImage().SavePng($"{folder}/{name}.png") == Error.Ok, $"Capture {name}");
    }
    private static void Check(bool value, string message)
    { if (!value) throw new InvalidOperationException(message); GD.Print($"PASS: {message}"); }
}
