using System.Collections.Generic;
using Godot;
using Hollowlight.Core;
using Hollowlight.Presentation;

namespace Hollowlight.World;

public partial class ThresholdWorld : Node2D
{
    [Export] public WorldConfig Config { get; set; } = null!;
    private readonly List<Interactable> _interactables = new();
    private SharedSupplies _state = null!;
    private SupplySetup _setup = null!;
    private bool _inventoryVisible;
    private Interactable? _openedChest;
    public bool InventoryVisible => _inventoryVisible;
    public bool ChestAccessible => _inventoryVisible && HasChestAccess();
    public SuppliesPanel InventoryPanel => _hud.Supplies;
    // Scene-owned controlled actor, not a global singleton. Interactions carry actor identity.
    private PlayerCharacter _controlledActor = null!;
    private ThresholdHud _hud = null!;
    private Interactable? _focused;
    private string _feedback = "Passage east. Bring salvage home. H: help";
    private double _feedbackTime = 6;
    private bool _debug;
    private static readonly Vector2 Home = new(136, 208);
    private static readonly Vector2 Outside = new(368, 176);
    public SuppliesSnapshot SuppliesFor(int playerId) => _state.Snapshot(playerId);
    public int StoredTotal => SuppliesFor(_controlledActor.PlayerId).Storage.Total;
    public int CarriedBy(int playerId) => SuppliesFor(playerId).Personal.Total;

    public override void _Ready()
    {
        _setup = SupplySetup.Parse(FileAccess.GetFileAsString("res://data/supplies.json"));
        _controlledActor = GetNode<PlayerCharacter>("Player");
        _state = _setup.CreateState(new[] { _controlledActor.PlayerId }, Config.StorageSlots);
        _hud = GetNode<ThresholdHud>("HUD");
        _controlledActor.InteractionRequested += Interact;
        _hud.Supplies.CloseRequested += CloseInventory;
        _hud.Supplies.OperationRequested += HandleSlotRequest;
        AddInteraction("shared-storage", "shared chest", new Vector2(88, 224), true);
        foreach (var site in _setup.Sites)
            AddInteraction(site.Id, site.Name, new Vector2(site.X, site.Y), resource: site.ResourceId, asset: site.Asset);
    }

    private void AddInteraction(string id, string name, Vector2 position, bool storage = false, string resource = "", string asset = "salvage")
    {
        var target = new Interactable {
            ObjectId = id, DisplayName = name, Position = position, IsStorage = storage,
            ResourceId = resource, PresentationAsset = asset
        };
        target.Initialize(storage ? PixelSprite.Motif.Storage : PixelSprite.Motif.Salvage);
        AddChild(target);
        _interactables.Add(target);
    }

    public override void _PhysicsProcess(double delta)
    {
        _feedbackTime = System.Math.Max(0, _feedbackTime - delta);
        Vector2 position = _controlledActor.GlobalPosition;
        if (position.X < 0 || position.Y < 0 || position.X > 640 || position.Y > 352)
        {
            Teleport(Home);
            Notify("Recovered at home. Carried salvage preserved.");
        }
        if (_openedChest != null && !HasChestAccess()) CloseInventory();
        _focused = FindTarget(_controlledActor);
        foreach (var target in _interactables) target.Art.Highlight = target == _focused;
    }

    private Interactable? FindTarget(PlayerCharacter actor)
    {
        Interactable? nearest = null;
        float best = Config.InteractionRange * Config.InteractionRange;
        foreach (var target in _interactables)
        {
            if (!target.Visible) continue;
            float distance = actor.GlobalPosition.DistanceSquaredTo(target.GlobalPosition);
            if (distance > best) continue;
            var query = PhysicsRayQueryParameters2D.Create(actor.GlobalPosition, target.GlobalPosition, 1);
            var hit = GetWorld2D().DirectSpaceState.IntersectRay(query);
            // Solid scenery blocks interaction, but the target's own collision is expected.
            if (hit.Count > 0 && hit["collider"].AsGodotObject() != target) continue;
            best = distance;
            nearest = target;
        }
        return nearest;
    }

    private void Interact(PlayerCharacter actor)
    {
        var target = FindTarget(actor);
        if (target == null) { Notify("Move closer to a salvage object or the shared chest."); return; }
        var snapshot = _state.Snapshot(actor.PlayerId);
        if (target.IsStorage)
        {
            OpenInventory(target);
        }
        else
        {
            var result = _state.Gather(actor.PlayerId, target.ObjectId, int.MaxValue, snapshot.Version);
            if (result.Status == SupplyStatus.Applied && _state.Snapshot(actor.PlayerId).Remaining[target.ObjectId] == 0)
                target.SetCollected();
            Report(result, $"Gathered {target.DisplayName}:");
        }
    }

    // Spatial/actor validation belongs to the world. The inventory owner performs
    // atomic quantity/capacity/version validation; UI never mutates a container.
    public SupplyResult? TransferSupplies(PlayerCharacter actor, string resource, int amount, bool withdraw, SupplyVersion expected)
    {
        if (actor != _controlledActor || !ChestAccessible)
        { Notify("Move closer to the shared chest."); return null; }
        var result = _state.Transfer(actor.PlayerId, resource, amount, withdraw, expected);
        Report(result, withdraw ? "Withdrew" : "Stored");
        return result;
    }

    private bool HasChestAccess() => _openedChest != null && GodotObject.IsInstanceValid(_openedChest) &&
        !_openedChest.IsQueuedForDeletion() && _openedChest.IsInsideTree() && _openedChest.IsVisibleInTree() &&
        _openedChest.CollisionLayer != 0 && _controlledActor.GlobalPosition.DistanceSquaredTo(_openedChest.GlobalPosition) <=
        Config.InteractionRange * Config.InteractionRange;

    private void OpenInventory(Interactable? chest = null)
    {
        _hud.HideHelp();
        _openedChest = chest;
        _inventoryVisible = true;
        _controlledActor.InventoryOpen = true;
        _hud.Supplies.ControllerDevice = _controlledActor.ControllerDevice;
        _hud.Supplies.Open(_state.Snapshot(_controlledActor.PlayerId), _state.Resources, chest != null);
    }
    public void CloseInventory()
    {
        _openedChest = null; _inventoryVisible = false;
        _controlledActor.InventoryOpen = false;
        _hud.Supplies.Close();
    }
    private void HandleSlotRequest(SlotRequest request) => _hud.Supplies.ShowResult(ApplyInventoryCommand(_controlledActor, request));
    public SupplyResult ApplyInventoryCommand(PlayerCharacter actor, SlotRequest request)
    {
        bool needsChest = request.Action != SlotAction.Move || request.Source.Container != InventoryKind.Personal ||
            request.Destination.Container != InventoryKind.Personal;
        if (actor != _controlledActor || !_inventoryVisible || (needsChest && !HasChestAccess()))
            return new SupplyResult(SupplyStatus.AccessDenied, 0, _state.Snapshot(_controlledActor.PlayerId).Version);
        return _state.ApplySlots(actor.PlayerId, request);
    }
    public override void _UnhandledInput(InputEvent input)
    {
        if (input is InputEventJoypadButton joy && joy.Pressed && joy.ButtonIndex == JoyButton.Back && joy.Device == _controlledActor.ControllerDevice)
        {
            if (_inventoryVisible) CloseInventory(); else OpenInventory();
            GetViewport().SetInputAsHandled();
        }
    }

    private void Report(SupplyResult result, string action)
    {
        string message = result.Status switch {
            SupplyStatus.Applied => $"{action} {result.Amount} units.",
            SupplyStatus.Stale => "Supplies changed. Review counts and try again.",
            SupplyStatus.NoCapacity => "No room. Remaining supplies stay at their source.",
            SupplyStatus.Empty => "Nothing available to transfer.",
            _ => "That supply request could not be completed."
        };
        Notify(message);
    }

    public override void _Process(double delta)
    {
        bool outside = _controlledActor.Position.X >= ThresholdMap.SurfaceStart;
        var snapshot = _state.Snapshot(_controlledActor.PlayerId);
        string prompt = _focused == null ? ""
            : (_focused.IsStorage ? "E / A  Open communal chest" : $"E  Gather {_focused.DisplayName} ({snapshot.Remaining[_focused.ObjectId]} left)");
        string debug = _debug
            ? $"{Engine.GetFramesPerSecond()} fps | pos {_controlledActor.Position.Round()} | vel {_controlledActor.Velocity.Round()}\n" +
              $"target: {_focused?.ObjectId ?? "none"} | seed {Config.DecorationSeed}\nF2: home   F3: surface   R: restore all salvage"
            : "";
        _hud.Present(outside ? "SURFACE CLEARING" : "REFUGE / HOME", snapshot,
            prompt, _feedbackTime > 0 ? _feedback : "", debug, _debug);
    }

    public override void _UnhandledKeyInput(InputEvent @event)
    {
        if (@event is not InputEventKey key || !key.Pressed || key.Echo) return;
        switch (key.PhysicalKeycode)
        {
            case Key.R:
                // Reload detaches this node immediately. Handle the event before the tree changes.
                GetViewport().SetInputAsHandled();
                GetTree().ReloadCurrentScene();
                return;
            case Key.Tab: if (_inventoryVisible) CloseInventory(); else OpenInventory(); break;
            case Key.F1: _debug = !_debug; break;
            case Key.H: _hud.ToggleHelp(); break;
            case Key.F2: Teleport(Home); Notify("Debug: returned home; quantities preserved."); break;
            case Key.F3: Teleport(Outside); Notify("Debug: moved outside; quantities preserved."); break;
            default: return;
        }
        GetViewport().SetInputAsHandled();
    }

    private void Teleport(Vector2 position)
    {
        CloseInventory();
        _controlledActor.Position = position;
        _controlledActor.Velocity = Vector2.Zero;
        _controlledActor.GetNode<Camera2D>("Camera2D").ResetSmoothing();
    }

    private void Notify(string message) { _feedback = message; _feedbackTime = 4; }

    public override void _ExitTree()
    {
        if (_controlledActor != null)
        {
            _controlledActor.InventoryOpen = false;
            _controlledActor.InteractionRequested -= Interact;
        }
        if (_hud != null)
        {
            _hud.Supplies.CloseRequested -= CloseInventory;
            _hud.Supplies.OperationRequested -= HandleSlotRequest;
        }
    }
}
