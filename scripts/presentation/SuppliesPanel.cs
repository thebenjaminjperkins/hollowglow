using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Hollowlight.Core;

namespace Hollowlight.Presentation;

// UI selection is a preview, never an inventory/cursor container. Only requests
// cross into World; immutable snapshots remain authoritative after every gesture.
public partial class SuppliesPanel : Control
{
    public event Action<SlotRequest>? OperationRequested;
    public event Action? CloseRequested;
    public event Action<EatRequest>? EatRequested;
    public event Action<InstallRequest>? InstallRequested;
    private Button? _install;
    private InstallRequest? _pointerInstall;
    private bool HasKitResource=>_resources.Any(r=>r.Id is "backpack_kit" or "bed_kit");
    public void InstallFocused()
    {var request=CurrentInstallRequest();if(request!=null)InstallRequested?.Invoke(request);}
    private InstallRequest? CurrentInstallRequest()
    {
        if(!Visible||_splitting||_storeFocused)return null;
        var address=_source??Focused;
        return address.Container==InventoryKind.Personal&&(Stack(address)?.ResourceId is "backpack_kit" or "bed_kit")
            ? new(address.Index,_source!=null?_selectionVersion:_state.Version):null;
    }
    public void ShowInstallResult(CraftResult result)
    {CancelSelection();Notice(result.Status==CraftStatus.Applied?"Installed":result.Status==CraftStatus.AccessDenied?"Bed needs garden marker; kit kept":CraftingPanel.Message(result.Status));}

    private HungerSnapshot? _meal;
    private Button? _eat;
    private EatRequest? _pointerMeal;
    public InventoryAppearance Appearance { get; private set; } = null!;
    public int ControllerDevice { get; set; }
    public bool ChestOpen { get; private set; }
    public SlotAddress Focused { get; private set; } = new(InventoryKind.Personal, 0);
    public SlotAddress? Selected => _source;
    public int SelectionQuantity => _quantity;
    public SupplyVersion SelectionVersion => _selectionVersion;
    public SupplyVersion DisplayedVersion => _state.Version;
    public bool Splitting => _splitting;
    public int SplitQuantity => _quantity;
    public int Page { get; private set; }
    public bool UsingController { get; private set; }
    private SuppliesSnapshot _state = null!;
    private IReadOnlyList<ResourceDefinition> _resources = null!;
    private readonly List<SlotCell> _cells = new();
    private Control _content = null!, _splitBox = null!;
    private Label _detail = null!, _hint = null!, _splitLabel = null!;
    private Button? _store;
    private SlotAddress? _source;
    private SupplyVersion _selectionVersion;
    private int _quantity;
    private bool _splitting, _storeFocused;
    private Vector2 _stick;
    private double _repeat;
    private string _notice = "";
    private double _noticeTime;
    private int PageSize => Appearance.ChestColumns * Appearance.ChestRows;
    public int PageCount => Math.Max(1, (_state.Storage.Capacity + PageSize - 1) / PageSize);
    private InventorySnapshot Inventory(InventoryKind kind) => kind == InventoryKind.Personal ? _state.Personal : _state.Storage;
    public ItemStack? Stack(SlotAddress address) => address.Index >= 0 && address.Index < Inventory(address.Container).Capacity
        ? Inventory(address.Container).Slots[address.Index] : null;
    public override void _Ready()
    {
        Appearance = InventoryAppearance.Load();
        MouseFilter = MouseFilterEnum.Ignore;
        TextureFilter = TextureFilterEnum.Nearest;
        Visible = false;
    }
    public void Open(SuppliesSnapshot state, IReadOnlyList<ResourceDefinition> resources, bool chest)
    {
        _state = state; _resources = resources; ChestOpen = chest;
        Page = 0; Focused = new(InventoryKind.Personal, 0); _storeFocused = false;
        CancelSelection(); _noticeTime = 0; _stick = Vector2.Zero;
        Visible = true; Rebuild();
    }
    public void Close()
    {
        Visible = false; ChestOpen = false; _stick = Vector2.Zero;
        CancelSelection();
    }
    public void Refresh(SuppliesSnapshot state)
    {
        if (!Visible) return;
        if (_source != null && state.Version != _selectionVersion)
        { CancelSelection(); Notice("Changed; reselect"); }
        bool resized=_state.Personal.Capacity!=state.Personal.Capacity;
        _state = state;
        if(resized)Rebuild();
        foreach (var cell in _cells) cell.Refresh();
        UpdateDetail();
    }
    public void RefreshMeal(HungerSnapshot? meal) => _meal = meal;
    public void EatFocused()
    {
        var request = CurrentEatRequest();
        if (request != null) EatRequested?.Invoke(request);
    }
    private EatRequest? CurrentEatRequest()
    {
        if (!Visible || _splitting || _storeFocused || _meal == null) return null;
        var address = _source ?? Focused;
        return address.Container == InventoryKind.Personal
            ? new(address.Index, _source != null ? _selectionVersion : _state.Version, _meal.Day) : null;
    }
    public void ShowEatResult(EatResult result)
    {
        CancelSelection();
        Notice(result.Status switch {
            EatStatus.Eaten => "Meal credited",
            EatStatus.Satisfied => "Already eaten today",
            EatStatus.Stale => "Changed; reselect",
            EatStatus.NotEdible => "Not edible",
            _ => "Cannot eat that item"
        });
    }
    public void ShowResult(SupplyResult result)
    {
        CancelSelection();
        Notice(result.Status switch {
            SupplyStatus.Applied => $"Moved {result.Amount}",
            SupplyStatus.Stale => "Changed; reselect",
            SupplyStatus.NoCapacity => "No room",
            SupplyStatus.Incompatible => "Use matching slot",
            SupplyStatus.Empty => "Empty slot",
            _ => "No items moved."
        });
    }
    private void Notice(string text) { _notice = text; _noticeTime = 2.5; UpdateDetail(); }
    public void CancelSelection()
    {
        _pointerInstall=null;
        _source = null; _splitting = false; _quantity = 0;
        if (_splitBox != null) _splitBox.Visible = false;
        foreach (var cell in _cells) cell.QueueRedraw();
    }
    public SlotCell? Cell(SlotAddress address) => _cells.FirstOrDefault(c => c.Address == address);
    public string ResourceName(string id) => _resources.First(r => r.Id == id).Name;
    public void PointerFocus(SlotAddress address) { UsingController = false; FocusSlot(address); }
    public void FocusSlot(SlotAddress address)
    {
        Focused = address; _storeFocused = false;
        foreach (var cell in _cells) cell.QueueRedraw();
        UpdateDetail();
    }
    public void Activate(SlotAddress destination)
    {
        if (!Visible || _splitting) return;
        FocusSlot(destination);
        if (_source == null)
        {
            var stack = Stack(destination);
            if (stack == null) return;
            _noticeTime = 0;
            _source = destination; _quantity = stack.Quantity; _selectionVersion = _state.Version;
            UpdateDetail(); foreach (var cell in _cells) cell.QueueRedraw();
            return;
        }
        if (_source == destination) { CancelSelection(); UpdateDetail(); return; }
        OperationRequested?.Invoke(new(SlotAction.Move, _source.Value, destination, _quantity, _selectionVersion));
    }
    public bool BeginDrag(SlotAddress address, int quantity, SupplyVersion version)
    {
        if (!Visible || _splitting || Stack(address) == null || quantity < 1) return false;
        if (_state.Version != version) { CancelSelection(); Notice("Changed; reselect"); return false; }
        CancelSelection(); FocusSlot(address); _source = address;
        _quantity = Math.Min(quantity, Stack(address)!.Quantity); _selectionVersion = version;
        _noticeTime = 0; UpdateDetail(); return true;
    }
    public void QuickTransfer(SlotAddress address)
    {
        if (!Visible || !ChestOpen || _splitting) return;
        CancelSelection();
        OperationRequested?.Invoke(new(SlotAction.QuickTransfer, address, default, int.MaxValue, _state.Version));
    }
    public void StoreAll()
    {
        if (!Visible || !ChestOpen || _splitting) return;
        CancelSelection();
        OperationRequested?.Invoke(new(SlotAction.DepositAll, default, default, int.MaxValue, _state.Version));
    }
    public void BeginSplit(SlotAddress address)
    {
        if (!Visible) return;
        var stack = Stack(address);
        if (stack == null || stack.Quantity < 2) { Notice("Need two to split"); return; }
        CancelSelection(); _noticeTime = 0; FocusSlot(address); _source = address; _selectionVersion = _state.Version;
        _quantity = (stack.Quantity + 1) / 2; _splitting = true; _splitBox.Show(); UpdateSplit();
    }
    public void ChangeSplit(int amount)
    {
        if (!_splitting || _source == null) return;
        _quantity = Math.Clamp(_quantity + amount, 1, Stack(_source.Value)!.Quantity - 1); UpdateSplit();
    }
    public void ConfirmSplit()
    {
        if (!_splitting) return;
        _splitting = false; _splitBox.Hide(); UpdateDetail();
    }
    private void UpdateSplit() => _splitLabel.Text = $"Split  {_quantity} / {Stack(_source!.Value)!.Quantity}";
    public void ChangePage(int direction)
    {
        if (!ChestOpen || _splitting) return;
        Page = (Page + direction + PageCount) % PageCount;
        if (Focused.Container == InventoryKind.Storage) Focused = new(InventoryKind.Storage, Page * PageSize);
        Rebuild();
    }
    private void Rebuild()
    {
        if (_content != null) { RemoveChild(_content); _content.QueueFree(); }
        _cells.Clear(); _store = null; _eat = null; _install=null;
        _content = new Control { MouseFilter = MouseFilterEnum.Ignore }; AddChild(_content);
        int cell = Appearance.CellSize, leftWidth = Appearance.BackpackColumns * cell + 16;
        int backRows = (_state.Personal.Capacity + Appearance.BackpackColumns - 1) / Appearance.BackpackColumns;
        int width = leftWidth + (ChestOpen ? Appearance.ChestColumns * cell + 24 : 0);
        int gridRows = ChestOpen ? Math.Max(backRows, Appearance.ChestRows) : backRows + (_meal != null || HasKitResource ? 1 : 0);
        int height = gridRows * cell + 42;
        Position = new Vector2(Mathf.Floor((384 - width) / 2f), Mathf.Floor((216 - height - 32) / 2f));
        _content.AddChild(new ColorRect { Size = new Vector2(width, height + 32), Color = new Color("202832f5"), MouseFilter = MouseFilterEnum.Ignore });
        Text("BACKPACK", 8, 5, 9);
        AddGrid(InventoryKind.Personal, 0, _state.Personal.Capacity, Appearance.BackpackColumns, new Vector2(8, 20));
        int actionY=24+backRows*cell, controlHeight=backRows>2&&ChestOpen?12:18;
        int useY=actionY+(ChestOpen?(backRows>2?14:22):0);
        if (ChestOpen)
        {
            Text($"COMMUNAL CHEST  {Page + 1}/{PageCount}", leftWidth + 8, 5, 8);
            AddGrid(InventoryKind.Storage, Page * PageSize, Math.Min(PageSize, _state.Storage.Capacity - Page * PageSize),
                Appearance.ChestColumns, new Vector2(leftWidth + 8, 20));
            _store = Button("Store all", new Vector2(8, actionY), new Vector2(leftWidth - 16, controlHeight), StoreAll);
            _store.MouseEntered += () => { _storeFocused = true; UpdateDetail(); };
            if (PageCount > 1)
            {
                Button("<", new Vector2(leftWidth + 8, 22 + Appearance.ChestRows * cell), new Vector2(18, 16), () => ChangePage(-1));
                Button(">", new Vector2(width - 26, 22 + Appearance.ChestRows * cell), new Vector2(18, 16), () => ChangePage(1));
            }
        }
        if (_meal != null)
        {
            _eat = Button("Eat  F / RS", new Vector2(8, useY),
                new Vector2(leftWidth - 16, controlHeight), () => {
                    if (_pointerMeal != null) EatRequested?.Invoke(_pointerMeal);
                    _pointerMeal = null;
                });
            _eat.ButtonDown += () => _pointerMeal = CurrentEatRequest();
        }
        if(HasKitResource)
        {
            _install=Button("Install U / LS",new Vector2(8,useY),new Vector2(leftWidth-16,controlHeight),()=>{
                if(_pointerInstall!=null)InstallRequested?.Invoke(_pointerInstall);_pointerInstall=null;});
            _install.ButtonDown+=()=>_pointerInstall=CurrentInstallRequest();
        }
        _detail = Text("", 8, height - 16, 8);
        _hint = Text("", 8, height + 1, 8);
        _hint.Size = new Vector2(width - 16, 32);
        _hint.AutowrapMode = TextServer.AutowrapMode.WordSmart;
        _splitBox = new Control { Position = new Vector2(width / 2f - 76, 28), Visible = _splitting, MouseFilter = MouseFilterEnum.Stop };
        _content.AddChild(_splitBox);
        _splitBox.AddChild(new ColorRect { Size = new Vector2(152, 72), Color = new Color("303842ff"), MouseFilter = MouseFilterEnum.Ignore });
        _splitLabel = new Label { Position = new Vector2(12, 8), MouseFilter = MouseFilterEnum.Ignore };
        _splitLabel.AddThemeFontSizeOverride("font_size", 10); _splitBox.AddChild(_splitLabel);
        Button("−", new Vector2(10, 29), new Vector2(24, 17), () => ChangeSplit(-1), _splitBox);
        Button("+", new Vector2(38, 29), new Vector2(24, 17), () => ChangeSplit(1), _splitBox);
        Button("OK", new Vector2(67, 29), new Vector2(34, 17), ConfirmSplit, _splitBox);
        Button("Cancel", new Vector2(105, 29), new Vector2(40, 17), () => { CancelSelection(); UpdateDetail(); }, _splitBox);
        UpdateDetail();
    }
    private void AddGrid(InventoryKind kind, int start, int count, int columns, Vector2 origin)
    {
        for (int i = 0; i < count; i++)
        {
            var address = new SlotAddress(kind, start + i);
            var cell = new SlotCell { OwnerPanel = this, Address = address, Position = origin + new Vector2(i % columns, i / columns) * Appearance.CellSize,
                Size = Vector2.One * (Appearance.CellSize - 2) };
            _content.AddChild(cell); _cells.Add(cell); cell.Refresh();
        }
    }
    private Label Text(string text, float x, float y, int size)
    {
        var label = new Label { Text = text, Position = new Vector2(x, y), MouseFilter = MouseFilterEnum.Ignore };
        label.AddThemeFontSizeOverride("font_size", size); label.AddThemeColorOverride("font_color", new Color("ead4a9"));
        _content.AddChild(label); return label;
    }
    private Button Button(string text, Vector2 pos, Vector2 size, Action action, Node? parent = null)
    {
        var button = new Button { Text = text, Position = pos, FocusMode = FocusModeEnum.None };
        button.AddThemeFontSizeOverride("font_size", 8);
        button.AddThemeColorOverride("font_color", new Color("ead4a9"));
        foreach (string state in new[] { "normal", "hover", "pressed" })
            button.AddThemeStyleboxOverride(state, new StyleBoxFlat { BgColor = new Color(state == "normal" ? "303b3e" : "47555e"),
                BorderColor = new Color(state == "normal" ? "69797e" : "d4ad73"), BorderWidthLeft = 1, BorderWidthRight = 1,
                BorderWidthTop = 1, BorderWidthBottom = 1 });
        button.Pressed += () => { if (!_splitting || parent == _splitBox) action(); };
        (parent ?? _content).AddChild(button); button.Size = size; return button;
    }
    private void UpdateDetail()
    {
        if (_detail == null || _state == null) return;
        var stack = Stack(Focused);
        var edibleAddress = _source ?? Focused;
        var edible = Stack(edibleAddress);
        if (_eat != null)
        {
            _eat.Visible = edibleAddress.Container == InventoryKind.Personal && edible != null && _resources.Any(r => r.Id == edible.ResourceId && r.Edible);
            _eat.Disabled = _splitting || _storeFocused || _meal == null || _meal.Eaten >= _meal.Required;
        }
        if(_install!=null){_install.Visible=CurrentInstallRequest()!=null;_install.Disabled=_splitting||_storeFocused;}
        _detail.Text = _noticeTime > 0 ? _notice : _storeFocused ? "Store all that fits; keep any remainder." : stack == null ? "Empty slot" :
            _resources.First(r => r.Id == stack.ResourceId).Name + (_resources.First(r => r.Id == stack.ResourceId).Edible ? " (edible)" : "");
        bool swapping = _source != null && Stack(_source.Value) is { } source && stack != null && source.ResourceId != stack.ResourceId;
        if (_splitting)
        {
            _hint.Text = UsingController ? (ChestOpen ? "Left/right Quantity   A Confirm   B Cancel" : "Left/right Adjust\nA OK   B Cancel") : (ChestOpen ? "Choose quantity, confirm, then select a destination." : "Choose quantity\nThen destination");
            return;
        }
        _hint.Text = UsingController ? (_source == null ? (ChestOpen ? "A Select   X Quick   Y Split   B Close" : "A Select  Y Split\nB Close") : (swapping ? "A Swap stacks   B Cancel" : (ChestOpen ? "A Place / merge   B Cancel" : "A Place / merge\nB Cancel"))) :
            (_source == null ? (ChestOpen ? "Click Select   Shift-click Quick   Right-click Split" : "Click Select\nRight-click Split") : (swapping ? "Swap stacks   Esc Cancel" : (ChestOpen ? $"Place {_quantity}   Esc Cancel" : $"Place {_quantity}\nEsc Cancel")));
    }
    public void Navigate(int x, int y)
    {
        if (_splitting) { ChangeSplit(x == 0 ? -y : x); return; }
        if (_storeFocused)
        {
            _storeFocused = false; FocusSlot(new(InventoryKind.Personal, Math.Min(_state.Personal.Capacity-1, Appearance.BackpackColumns))); return;
        }
        int columns = Focused.Container == InventoryKind.Personal ? Appearance.BackpackColumns : Appearance.ChestColumns;
        int local = Focused.Container == InventoryKind.Personal ? Focused.Index : Focused.Index - Page * PageSize;
        int col = local % columns, row = local / columns;
        if (ChestOpen && x > 0 && Focused.Container == InventoryKind.Personal && col == columns - 1)
        { FocusSlot(new(InventoryKind.Storage, Page * PageSize + Math.Min(row, Appearance.ChestRows - 1) * Appearance.ChestColumns)); return; }
        if (x < 0 && Focused.Container == InventoryKind.Storage && col == 0)
        { FocusSlot(new(InventoryKind.Personal, Math.Min(_state.Personal.Capacity-1, row * Appearance.BackpackColumns + Appearance.BackpackColumns - 1))); return; }
        int count = Focused.Container == InventoryKind.Personal ? _state.Personal.Capacity : Math.Min(PageSize, _state.Storage.Capacity - Page * PageSize);
        if (ChestOpen && Focused.Container == InventoryKind.Personal && y > 0 && local + columns >= _state.Personal.Capacity)
        { _storeFocused = true; UpdateDetail(); return; }
        int next = Math.Clamp(row + y, 0, (count - 1) / columns) * columns + Math.Clamp(col + x, 0, columns - 1);
        FocusSlot(new(Focused.Container, (Focused.Container == InventoryKind.Storage ? Page * PageSize : 0) + Math.Min(next, count - 1)));
    }
    private void Confirm()
    {
        if (_splitting) ConfirmSplit(); else if (_storeFocused) StoreAll(); else Activate(Focused);
    }
    private void Back()
    {
        if (_source != null || _splitting) { CancelSelection(); UpdateDetail(); }
        else CloseRequested?.Invoke();
    }
    public override void _UnhandledInput(InputEvent input)
    {
        if (!Visible) return;
        if (input is InputEventJoypadButton or InputEventJoypadMotion && input.Device != ControllerDevice) return;
        bool handled = false;
        if (input is InputEventKey key && key.Pressed)
        {
            UsingController = false;
            handled = true;
            switch (key.PhysicalKeycode)
            {
                case Key.Left: Navigate(-1, 0); break; case Key.Right: Navigate(1, 0); break;
                case Key.Up: Navigate(0, -1); break; case Key.Down: Navigate(0, 1); break;
                default:
                    if (key.Echo) break;
                    switch (key.PhysicalKeycode)
                    {
                        case Key.U: InstallFocused(); break;
                        case Key.F: EatFocused(); break;
                        case Key.Enter: Confirm(); break;
                        case Key.Space: if (!_storeFocused) BeginSplit(_source ?? Focused); break;
                        case Key.Q: if (!_storeFocused) QuickTransfer(_source ?? Focused); break;
                        case Key.Escape: Back(); break;
                        case Key.Tab: CloseRequested?.Invoke(); break;
                        case Key.Pageup: ChangePage(-1); break; case Key.Pagedown: ChangePage(1); break;
                        default: handled = false; break;
                    }
                    break;
            }
        }
        else if (input is InputEventJoypadButton joy && joy.Pressed)
        {
            UsingController = true; handled = true;
            switch (joy.ButtonIndex)
            {
                case JoyButton.DpadLeft: Navigate(-1, 0); break; case JoyButton.DpadRight: Navigate(1, 0); break;
                case JoyButton.DpadUp: Navigate(0, -1); break; case JoyButton.DpadDown: Navigate(0, 1); break;
                case JoyButton.LeftStick: InstallFocused(); break;
                case JoyButton.RightStick: EatFocused(); break;
                case JoyButton.A: Confirm(); break; case JoyButton.B: Back(); break;
                case JoyButton.X: if (!_storeFocused) QuickTransfer(_source ?? Focused); break; case JoyButton.Y: if (!_storeFocused) BeginSplit(_source ?? Focused); break;
                case JoyButton.Back: CloseRequested?.Invoke(); break;
                case JoyButton.LeftShoulder: ChangePage(-1); break; case JoyButton.RightShoulder: ChangePage(1); break;
                default: handled = false; break;
            }
        }
        else if (input is InputEventJoypadMotion motion && motion.Axis is JoyAxis.LeftX or JoyAxis.LeftY)
        {
            UsingController = true;
            Vector2 previous = StickDirection();
            float value = Math.Abs(motion.AxisValue) > 0.55f ? motion.AxisValue : 0;
            if (motion.Axis == JoyAxis.LeftX) _stick.X = value; else _stick.Y = value;
            if (previous != StickDirection()) _repeat = 0;
            handled = true;
        }
        if (handled) { UpdateDetail(); GetViewport().SetInputAsHandled(); }
    }
    private Vector2 StickDirection() => Math.Abs(_stick.X) >= Math.Abs(_stick.Y)
        ? new Vector2(Math.Sign(_stick.X), 0) : new Vector2(0, Math.Sign(_stick.Y));
    public override void _Process(double delta)
    {
        if (!Visible) return;
        _noticeTime = Math.Max(0, _noticeTime - delta);
        if (_store != null && IsInstanceValid(_store)) _store.SelfModulate = _storeFocused ? new Color("d4ad73") : Colors.White;
        if (_stick.LengthSquared() > 0)
        {
            _repeat -= delta;
            if (_repeat <= 0) { var direction = StickDirection(); Navigate((int)direction.X, (int)direction.Y); _repeat = 0.18; }
        }
        UpdateDetail();
    }
}
