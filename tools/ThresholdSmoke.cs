using System;
using System.Threading.Tasks;
using Godot;
using Hollowlight.World;
using Hollowlight.Presentation;
using Hollowlight.Core;

namespace Hollowlight.Tools;

public partial class ThresholdSmoke : Node
{
    private static int _stage;
    private static bool _failed;
    private SceneTree _tree = null!;
    private ThresholdWorld _world = null!;
    private PlayerCharacter _actor = null!;
    private SmokeInput _input = null!;

    public override async void _Ready()
    {
        _tree = GetTree();
        try
        {
            _input = new SmokeInput { Name = "SmokeInput" };
            bool playable = Array.Exists(OS.GetCmdlineUserArgs(), arg => arg == "--playable-refuge");
            bool room = playable || Array.Exists(OS.GetCmdlineUserArgs(), arg => arg == "--sanctuary-room");
            RoomPresentation? presentation = null;
            if (playable)
            {
                string entry = ProjectSettings.GetSetting("application/run/main_scene").AsString();
                var refuge = GD.Load<PackedScene>(entry).Instantiate<SanctuaryRoom>();
                refuge.InputOverride = _input;
                AddChild(refuge);
                _world = refuge.World;
                presentation = refuge.Presentation;
            }
            else
            {
                _world = GD.Load<PackedScene>("res://scenes/Threshold.tscn").Instantiate<ThresholdWorld>();
                var actor = _world.GetNode<PlayerCharacter>("Player");
                actor.AddChild(_input);
                actor.InputPath = new NodePath("SmokeInput");
                AddChild(_world);
                if (room)
                {
                    SanctuaryRoom.ConfigureWindow(GetWindow());
                    presentation = new RoomPresentation { World = _world };
                    AddChild(presentation);
                }
            }
            _actor = _world.GetNode<PlayerCharacter>("Player");
            await Frames(3);
            Check(_world.CarriedBy(1) == 0 && _world.StoredTotal == 0, "Fresh scene clears both quantities");
            if (presentation != null)
            {
                var terrain = presentation.Terrain;
                int expected = 0;
                foreach (string row in _world.GetNode<ThresholdMap>("Map").Layout.Rows)
                    foreach (char cell in row) if (cell == '.') expected++;
                Check(terrain.Floor.GetUsedCells().Count == expected, "TileMapLayer covers every original floor cell");
                Check(!terrain.Floor.CollisionEnabled && !terrain.Walls.CollisionEnabled, "TileMapLayer does not duplicate simulation collision");
                Check((_actor.GetViewportRect().Size / _actor.GetNode<Camera2D>("Camera2D").Zoom).DistanceTo(new Vector2(384, 216)) < 0.01f,
                    "High-resolution room preserves original world camera coverage");
                Check(!_actor.GetNode<CanvasItem>("Art").IsVisibleInTree(), "Legacy character drawing hidden in cohesive room");
                presentation.ToggleLighting(); presentation.ToggleLighting();
            }

            if (_stage == 0)
            {
                var map = _world.GetNode<ThresholdMap>("Map");
                bool cornerChecked = false;
                for (int y = 0; y < 22 && !cornerChecked; y++)
                for (int x = 0; x < 20 && !cornerChecked; x++)
                    if (map.Layout.CutsCorner(x, y, -1, -1))
                    {
                        var blocked = _world.GetWorld2D().DirectSpaceState.IntersectPoint(new PhysicsPointQueryParameters2D {
                            Position = new Vector2(x * 16 + 1, y * 16 + 1), CollisionMask = 1
                        });
                        var open = _world.GetWorld2D().DirectSpaceState.IntersectPoint(new PhysicsPointQueryParameters2D {
                            Position = new Vector2(x * 16 + 8, y * 16 + 8), CollisionMask = 1
                        });
                        Check(blocked.Count > 0 && open.Count == 0, "Chamfered cave corner matches solid collision");
                        cornerChecked = true;
                    }
                Check(cornerChecked, "Irregular cave has a testable corner");
                Vector2 start = _actor.Position;
                _input.Movement = Vector2.Right;
                await Frames(60);
                _input.Movement = Vector2.Zero;
                Check(Mathf.Abs(_actor.Position.X - start.X - 80) < 2, "Fixed-tick horizontal speed");
                _actor.Position = start;
                _input.Movement = Vector2.One;
                await Frames(30);
                _input.Movement = Vector2.Zero;
                Check(Mathf.Abs(_actor.Position.DistanceTo(start) - 40) < 2, "Normalized diagonal speed");
                _actor.Position = start;
                _input.Movement = Vector2.Left;
                await Frames(140);
                _input.Movement = Vector2.Zero;
                Check(_actor.Position.X >= 19.8f && _actor.Position.X < 22, "Cave wall blocks movement");
                if (room)
                {
                    _input.Movement = Vector2.Left;
                    await Frames(10);
                    var art = _actor.GetNode<RoomCharacter>("RoomCharacter");
                    Check(art.FrameCoords.X < 2, "Walking stops while held against a wall");
                    Check(art.FrameCoords.Y == 2, "Blocked walk retains west facing");
                    foreach (Vector2 direction in new[] { Vector2.Down, Vector2.Up, Vector2.Left, Vector2.Right, Vector2.One })
                    {
                        _actor.Position = start;
                        _input.Movement = direction;
                        await Frames(10);
                        int expectedRow = direction.X < 0 ? 2 : direction.X > 0 ? 3 : direction.Y < 0 ? 1 : 0;
                        Check(art.FrameCoords.Y == expectedRow && art.FrameCoords.X >= 2,
                            $"Runtime directional walk: {direction}");
                    }
                    _input.Movement = Vector2.Zero;
                    await Frames(2);
                    Check(art.FrameCoords.X < 2 && art.FrameCoords.Y == 3, "Stopping preserves diagonal facing in idle");
                }
                _actor.Position = new Vector2(136, 176);
                _input.Movement = Vector2.Right;
                await Frames(180);
                _input.Movement = Vector2.Zero;
                Check(_actor.Position.X > 336, "Continuous passage is traversable");
                _actor.Position = new Vector2(-50, -50);
                await Frames(3);
                Check(_actor.Position.DistanceTo(start) < 1, "Out-of-bounds recovery returns home");
                ReactiveFern? fern = null;
                foreach (var child in map.GetChildren())
                    if (child is SanctuaryProp prop && prop.Kind == PixelSprite.Motif.Fern && prop.Position == new Vector2(96, 285))
                        fern = prop.GetNode<ReactiveFern>("ReactiveFern");
                Check(fern != null, "Benchmark fern instantiated in live map");
                _actor.Position = new Vector2(72, 285);
                _input.Movement = Vector2.Right;
                await Frames(30);
                _input.Movement = Vector2.Zero;
                Check(fern!.BrushCount > 0 && _actor.Position.X > 110, "Walking through fern triggers reaction without blocking movement");
                if (room) Check(fern.GetParent().GetNode<RoomSprite>("RoomArt").BrushCount > 0, "New textured fern reacts in cohesive room");
                await Frames(60);
                Check(fern.Response.Column == 0, "Live fern settles after player leaves");
                await Pickup(new Vector2(432, 200));
                Check(_world.CarriedBy(1) == 0, "Out-of-range pickup rejected");
                await Pickup(new Vector2(432, 178));
                await Tap();
                Check(_world.CarriedBy(1) == 12, "Pickup unique under repeated commands");
                _stage = 1;
                SendKey(Key.R);
                return;
            }

            if (_stage == 1)
            {
                await Pickup(new Vector2(432, 178));
                await Pickup(new Vector2(528, 130));
                await Pickup(new Vector2(512, 274));
                Check(_world.CarriedBy(1) == 34, "Reset restored all finite salvage");
                SendKey(Key.F2);
                Check(_world.CarriedBy(1) == 34, "Home teleport preserves carried quantity");
                SendKey(Key.F3);
                Check(_world.CarriedBy(1) == 34, "Surface teleport preserves carried quantity");
                await Pickup(new Vector2(88, 242));
                Check(_world.CarriedBy(1) == 34 && _world.StoredTotal == 0 && _world.ChestAccessible,
                    "Chest interaction opens without automatic transfer");
                _world.InventoryPanel.StoreAll();
                await Frames(2);
                Check(_world.CarriedBy(1) == 0 && _world.StoredTotal == 34, "Explicit store-all transfers every unit");
                await Tap();
                Check(_world.StoredTotal == 34, "Empty repeated deposit changes nothing");
                var deposited = _world.SuppliesFor(1);
                Check(deposited.Storage.Count("scrap") == 26 && deposited.Storage.Count("organic") == 8,
                    "Both resource types reach communal storage");
                Check(deposited.Personal.Capacity == 6 && deposited.Storage.Capacity == _world.Config.StorageSlots,
                    "Configured inventory capacities reach the playable world");
                var panel = _world.InventoryPanel;
                panel.QuickTransfer(new(InventoryKind.Storage, 0));
                await Frames(2);
                Check(_world.SuppliesFor(1).Personal.Count("scrap") == 10, "Quick-transfer chosen chest stack");
                panel.BeginSplit(new(InventoryKind.Personal, 0));
                Check(panel.SplitQuantity == 5, "Split starts at half the selected stack");
                panel.ConfirmSplit(); panel.Activate(new(InventoryKind.Personal, 1));
                await Frames(2);
                Check(_world.SuppliesFor(1).Personal.Slots[0]?.Quantity == 5 && _world.SuppliesFor(1).Personal.Slots[1]?.Quantity == 5,
                    "UI split commits to chosen destination");
                panel.Activate(new(InventoryKind.Personal, 1)); panel.Activate(new(InventoryKind.Personal, 0));
                await Frames(2);
                Check(_world.SuppliesFor(1).Personal.Slots[0]?.Quantity == 10 && _world.SuppliesFor(1).Personal.Slots[1] == null,
                    "UI merge restores stack without loss");
                panel.Activate(new(InventoryKind.Personal, 0)); panel.CancelSelection();
                Check(_world.CarriedBy(1) == 10 && panel.Selected == null, "Cancel selection leaves all items in source");
                var version = _world.SuppliesFor(1).Version;
                var move = new SlotRequest(SlotAction.Move, new(InventoryKind.Personal, 0), new(InventoryKind.Personal, 1), 1, version);
                Check(_world.ApplyInventoryCommand(_actor, move).Amount == 1 && _world.ApplyInventoryCommand(_actor, move).Status == SupplyStatus.Stale,
                    "World rejects replayed slot command");
                Vector2 panelStart = _actor.Position;
                _input.Movement = Vector2.Right;
                await Frames(30); _input.Movement = Vector2.Zero;
                Check(_actor.Position.DistanceTo(panelStart) < 0.1f && !_tree.Paused, "Inventory holds only this actor stationary without pausing world");
                panel.StoreAll(); await Frames(2);
                Check(_world.CarriedBy(1) == 0 && _world.StoredTotal == 34, "Store all restores complete stock after slot operations");
                _actor.Position = new Vector2(136, 208); await Frames(2);
                Check(!_world.InventoryVisible && !_world.ChestAccessible, "Leaving chest range closes session");
                Check(_world.ApplyInventoryCommand(_actor, move with { Version = _world.SuppliesFor(1).Version }).Status == SupplyStatus.AccessDenied,
                    "Closed session cannot transfer from stale UI");
                await Pickup(new Vector2(88, 242));
                _world.InventoryPanel.BeginSplit(new(InventoryKind.Storage, 0));
                Check(_world.InventoryPanel.Splitting, "Reset begins with an uncommitted split preview");
                _stage = 2;
                SendKey(Key.R);
                return;
            }

            Check(!_world.InventoryVisible && !_actor.InventoryOpen, "Reset clears modal input and all inventory previews");
            await Pickup(new Vector2(432, 178));
            Check(_world.CarriedBy(1) == 12, "Reset after deposit restores pickups");
            Check(_world.SuppliesFor(1).Remaining["tool-case"] == 8 && _world.SuppliesFor(1).Remaining["machine-housing"] == 14, "Reset restores both typed source quantities");
            GD.Print("PASS: typed gathering, chest UI transfers, stale-command rejection, and live physics movement, normalized diagonals, walls, passage, fall recovery, range, pickups, storage, debug teleports, and two scene resets");
            _tree.Quit(_failed ? 1 : 0);
        }
        catch (Exception error)
        {
            GD.PushError($"Threshold smoke failed: {error}");
            _failed = true;
            _tree.Quit(1);
        }
    }

    private async Task Frames(int count)
    {
        for (int i = 0; i < count; i++)
        {
            await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);
            await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        }
    }

    private async Task Pickup(Vector2 position)
    {
        _actor.Position = position;
        await Frames(2);
        await Tap();
    }

    private async Task Tap() { _input.Tap = true; await Frames(2); }
    private void SendKey(Key key) => _world._UnhandledKeyInput(new InputEventKey { PhysicalKeycode = key, Pressed = true });
    private static void Check(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
        GD.Print($"PASS: {message}");
    }
}
