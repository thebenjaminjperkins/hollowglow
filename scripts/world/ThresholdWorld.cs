using System.Collections.Generic;
using System.Linq;
using Godot;
using Hollowlight.Core;
using Hollowlight.Presentation;

namespace Hollowlight.World;

public partial class ThresholdWorld : Node2D
{
    [Export] public WorldConfig Config { get; set; } = null!;
    private readonly List<Interactable> _interactables = new();
    public HungerSnapshot? HungerFor(int playerId) => Config.HungerEnabled ? _state.Hunger(playerId) : null;
    public GameClock Clock { get; private set; } = null!;
    private SharedSupplies _state = null!;
    private AgricultureSetup? _farming;
    private readonly List<GrowingPlotArt> _plots = new();
    private GrowingPlotArt? _focusedPlot;
    private int _selectedCrop;
    public int StartingStoredTotal { get; private set; }
    public bool FarmingEnabled => _farming != null;
    public IReadOnlyList<GrowingPlotArt> GrowingPlots => _plots;
    public IReadOnlyList<CropDefinition> CropDefinitions => _state.Crops;
    public PlotSnapshot Plot(string id) => _state.Plot(id);
    public string? FocusedPlotId => _focusedPlot?.PlotId;
    public FoodWorld? ScavengingWorld { get; private set; }
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
        Clock = new GameClock(ClockDefinition.Parse(FileAccess.GetFileAsString("res://data/clock.json")));
        _setup = SupplySetup.Parse(FileAccess.GetFileAsString("res://data/supplies.json"));
        foreach (string argument in OS.GetCmdlineUserArgs().Where(a => a.StartsWith("--world-seed=", System.StringComparison.Ordinal)))
        {
            if (!int.TryParse(argument.Substring(13), out int seed)) throw new System.ArgumentException("World seed must be a 32-bit integer.");
            Config.WorldSeed = seed;
        }
        if (Config.ScavengingEnabled)
        {
            ScavengingWorld = FoodScavenging.Generate(FoodScavengingSetup.Parse(FileAccess.GetFileAsString("res://data/food_scavenging.json")),
                GetNode<ThresholdMap>("Map").Layout, _setup.Sites, Config.WorldSeed);
            _setup = _setup with { Sites = _setup.Sites.Concat(ScavengingWorld.Sources.Select(s => s.Site)).ToArray() };
        }
        _controlledActor = GetNode<PlayerCharacter>("Player");
        _farming = Config.FarmingEnabled ? AgricultureSetup.Parse(FileAccess.GetFileAsString("res://data/farming.json")) : null;
        _state = _setup.CreateState(new[] { _controlledActor.PlayerId }, Config.StorageSlots, _farming?.Resources);
        if (_farming != null)
        {
            var beds = GetNode<ThresholdMap>("Map").GetChildren().OfType<SanctuaryProp>().Where(p => p.Kind == PixelSprite.Motif.Garden).ToArray();
            var placements = beds.SelectMany((bed, b) => _farming.PlotOffsets.Select((offset, p) =>
                (Id: $"garden-{b}-{p}", Position: bed.Position + new Vector2(offset.X, offset.Y)))).ToArray();
            _state.InitializeAgriculture(_farming, Clock, placements.Select(p => p.Id));
            foreach (var place in placements)
            {
                var plot = new GrowingPlotArt { Name = place.Id, PlotId = place.Id, World = this, Position = place.Position };
                _plots.Add(plot); AddChild(plot);
            }
        }
        if (Config.HungerEnabled)
            _state.InitializeHunger(HungerDefinition.Parse(FileAccess.GetFileAsString("res://data/hunger.json")), Clock);
        StartingStoredTotal = _state.Snapshot(_controlledActor.PlayerId).Storage.Total;
        if (FarmingEnabled) _feedback = "Starter seeds are in the shared chest. C chooses a crop at a plot.";
        _hud = GetNode<ThresholdHud>("HUD");
        _controlledActor.InteractionRequested += Interact;
        _hud.Supplies.CloseRequested += CloseInventory;
        _hud.Supplies.EatRequested += HandleEat;
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
        target.IsFoodSource = ScavengingWorld?.Sources.Any(s => s.Site.Id == id) == true;
        target.Initialize(storage ? PixelSprite.Motif.Storage : PixelSprite.Motif.Salvage);
        if (target.IsFoodSource)
        {
            target.CollisionLayer = 0; // Ground-level salvage; preserve all existing navigation/collision.
            target.Art.Hide();
            target.AddChild(new ScavengingArt { Name = "FoodArt", Source = target });
        }
        AddChild(target);
        _interactables.Add(target);
    }

    public override void _PhysicsProcess(double delta)
    {
        Clock.Advance(delta);
        _controlledActor.HungerSpeedMultiplier = (float)(HungerFor(_controlledActor.PlayerId)?.SpeedMultiplier ?? 1);
        _feedbackTime = System.Math.Max(0, _feedbackTime - delta);
        Vector2 position = _controlledActor.GlobalPosition;
        if (position.X < 0 || position.Y < 0 || position.X > 640 || position.Y > 352)
        {
            Teleport(Home);
            Notify("Recovered at home. Carried salvage preserved.");
        }
        if (_openedChest != null && !HasChestAccess()) CloseInventory();
        _focused = FindTarget(_controlledActor);
        _focusedPlot = FindPlot(_controlledActor);
        if (_focusedPlot != null && _focused != null && _controlledActor.GlobalPosition.DistanceSquaredTo(_focusedPlot.GlobalPosition) <
            _controlledActor.GlobalPosition.DistanceSquaredTo(_focused.GlobalPosition)) _focused = null;
        else if (_focused != null) _focusedPlot = null;
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
        var plot = FindPlot(actor);
        if (plot != null && (target == null || actor.GlobalPosition.DistanceSquaredTo(plot.GlobalPosition) < actor.GlobalPosition.DistanceSquaredTo(target.GlobalPosition)))
        {
            var state = _state.Plot(plot.PlotId);
            UsePlot(actor, plot.PlotId, _state.Crops[_selectedCrop].Id, state.Version);
            return;
        }
        if (target == null) { Notify("Move closer to a salvage object or the shared chest."); return; }
        var snapshot = _state.Snapshot(actor.PlayerId);
        if (target.IsStorage)
        {
            OpenInventory(target);
        }
        else
        {
            GatherSupplies(actor, target.ObjectId, snapshot.Version);
        }
    }

    public SupplyResult GatherSupplies(PlayerCharacter actor, string nodeId, SupplyVersion version)
    {
        if (actor != _controlledActor || _inventoryVisible || FindTarget(actor)?.ObjectId != nodeId)
            return new(SupplyStatus.AccessDenied, 0, _state.Snapshot(_controlledActor.PlayerId).Version);
        var target = _interactables.First(t => t.ObjectId == nodeId);
        var result = _state.Gather(actor.PlayerId, nodeId, int.MaxValue, version);
        if (result.Status == SupplyStatus.Applied && _state.Snapshot(actor.PlayerId).Remaining[nodeId] == 0) target.SetCollected();
        if (target.IsFoodSource && result.Status == SupplyStatus.Empty) Notify("This Food source has been picked clean.");
        else Report(result, $"Gathered {target.DisplayName}:");
        return result;
    }

    private GrowingPlotArt? FindPlot(PlayerCharacter actor)
    {
        GrowingPlotArt? nearest = null;
        float best = Config.InteractionRange * Config.InteractionRange;
        foreach (var plot in _plots)
        {
            float distance = actor.GlobalPosition.DistanceSquaredTo(plot.GlobalPosition);
            if (distance > best) continue;
            var hit = GetWorld2D().DirectSpaceState.IntersectRay(PhysicsRayQueryParameters2D.Create(actor.GlobalPosition, plot.GlobalPosition, 1));
            if (hit.Count != 0) continue;
            nearest = plot; best = distance;
        }
        return nearest;
    }
    public FarmResult UsePlot(PlayerCharacter actor, string plotId, string cropId, SupplyVersion version)
    {
        if (actor != _controlledActor || _inventoryVisible || FindPlot(actor)?.PlotId != plotId)
            return new(FarmStatus.AccessDenied, version);
        var plot = _state.Plot(plotId);
        FarmResult result = plot.Stage == CropStage.Empty ? _state.Plant(actor.PlayerId, plotId, cropId, version)
            : _state.Harvest(actor.PlayerId, plotId, version);
        Notify(result.Status switch {
            FarmStatus.Planted => $"Planted {_state.Crops.First(c => c.Id == cropId).Name}.",
            FarmStatus.Harvested => "Harvest collected, including the returned seed.",
            FarmStatus.NoSeed => "Withdraw the required seeds from the shared chest.",
            FarmStatus.NoCapacity => "Make room for the complete harvest and returned seed.",
            FarmStatus.NotMature => $"{_state.Crops.First(c => c.Id == plot.CropId).Name} is still growing.",
            FarmStatus.Stale => "Plot changed. Inspect it and try again.",
            _ => "That plot cannot be used right now."
        });
        return result;
    }
    private void CycleCrop(int direction)
    {
        if (_inventoryVisible || _focusedPlot == null || _state.Plot(_focusedPlot.PlotId).Stage != CropStage.Empty) return;
        _selectedCrop = (_selectedCrop + direction + _state.Crops.Count) % _state.Crops.Count;
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
        _hud.Supplies.RefreshMeal(HungerFor(_controlledActor.PlayerId));
        _hud.Supplies.Open(_state.Snapshot(_controlledActor.PlayerId), _state.Resources, chest != null);
    }
    public void CloseInventory()
    {
        _openedChest = null; _inventoryVisible = false;
        _controlledActor.InventoryOpen = false;
        _hud.Supplies.Close();
    }
    private void HandleEat(EatRequest request)
    {
        var result = EatFood(_controlledActor, request);
        _hud.Supplies.ShowEatResult(result);
    }
    public EatResult EatFood(PlayerCharacter actor, EatRequest request)
    {
        if (actor != _controlledActor || !_inventoryVisible || !Config.HungerEnabled)
            return new(EatStatus.AccessDenied, 0);
        var result = _state.Eat(actor.PlayerId, request);
        _controlledActor.HungerSpeedMultiplier = (float)(HungerFor(actor.PlayerId)?.SpeedMultiplier ?? 1);
        return result;
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
        if (input is InputEventJoypadButton shoulder && shoulder.Pressed && shoulder.Device == _controlledActor.ControllerDevice &&
            shoulder.ButtonIndex is JoyButton.LeftShoulder or JoyButton.RightShoulder && !_inventoryVisible && _focusedPlot != null)
        { CycleCrop(shoulder.ButtonIndex == JoyButton.LeftShoulder ? -1 : 1); GetViewport().SetInputAsHandled(); return; }
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
            : (_focused.IsStorage ? "E / A  Open communal chest" : _focused.Depleted ? $"{_focused.DisplayName} | Empty" : $"E / A Gather {_focused.DisplayName} ({snapshot.Remaining[_focused.ObjectId]} left)");
        if (_focusedPlot != null && !_inventoryVisible)
        {
            var plot = _state.Plot(_focusedPlot.PlotId);
            if (plot.Stage == CropStage.Empty)
            {
                var crop = _state.Crops[_selectedCrop];
                prompt = $"E / A Plant {crop.Name}  ({snapshot.Personal.Count(crop.SeedResource)} seeds)   C / LB/RB Choose";
            }
            else
            {
                var crop = _state.Crops.First(c => c.Id == plot.CropId);
                double days = (double)plot.RemainingTicks / Clock.CycleTicks;
                prompt = plot.Stage == CropStage.Mature ? $"E / A Harvest {crop.Name}" : $"{crop.Name} | {plot.Stage} | {days:0.0} days left | E / A Inspect";
            }
        }
        string debug = _debug
            ? $"{Engine.GetFramesPerSecond()} fps | pos {_controlledActor.Position.Round()} | vel {_controlledActor.Velocity.Round()}\n" +
              $"target: {_focused?.ObjectId ?? "none"} | world seed {Config.WorldSeed}\nF2: home   F3: surface   R: restore all salvage\nF4: +1 min   F5: +1 day   Shift+F5: +3 days"
            : "";
        _hud.Present(outside ? "SURFACE CLEARING" : "REFUGE / HOME", snapshot,
            prompt, _feedbackTime > 0 ? _feedback : "", debug, _debug, Clock.Snapshot(), HungerFor(_controlledActor.PlayerId));
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
            case Key.C: CycleCrop(1); break;
            case Key.Tab: if (_inventoryVisible) CloseInventory(); else OpenInventory(); break;
            case Key.F4: Clock.Advance(60); Notify("Debug: advanced one minute."); break;
            case Key.F5:
                Clock.Advance((double)Clock.CycleTicks / System.TimeSpan.TicksPerSecond * (key.ShiftPressed ? 3 : 1));
                Notify(key.ShiftPressed ? "Debug: advanced three days." : "Debug: advanced one day.");
                break;
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
            _hud.Supplies.EatRequested -= HandleEat;
            _hud.Supplies.OperationRequested -= HandleSlotRequest;
        }
    }
}
