using Godot;
using Hollowlight.Core;

namespace Hollowlight.Presentation;

public partial class SlotCell : Control
{
    public SuppliesPanel OwnerPanel { get; set; } = null!;
    public SlotAddress Address { get; set; }
    private int _dragQuantity;
    private SupplyVersion _dragVersion;
    private Label _count = null!;
    public override void _Ready()
    {
        MouseFilter = MouseFilterEnum.Stop;
        TextureFilter = TextureFilterEnum.Nearest;
        _count = new Label { Position = new Vector2(Size.X - 15, Size.Y - 11), Size = new Vector2(13, 10),
            HorizontalAlignment = HorizontalAlignment.Right, MouseFilter = MouseFilterEnum.Ignore };
        _count.AddThemeFontSizeOverride("font_size", 8);
        _count.AddThemeColorOverride("font_color", new Color("f0dfb4"));
        _count.AddThemeColorOverride("font_shadow_color", new Color("202832"));
        _count.AddThemeConstantOverride("shadow_offset_x", 1); _count.AddThemeConstantOverride("shadow_offset_y", 1);
        AddChild(_count);
        MouseEntered += () => OwnerPanel.PointerFocus(Address);
    }
    public void Refresh()
    {
        var stack = OwnerPanel.Stack(Address);
        _count.Text = stack == null ? "" : stack.Quantity.ToString();
        TooltipText = stack == null ? "" : OwnerPanel.ResourceName(stack.ResourceId);
        QueueRedraw();
    }
    public override void _Draw()
    {
        DrawRect(new Rect2(Vector2.Zero, Size), new Color("303b3e"));
        DrawLine(new Vector2(1, Size.Y - 1), Size - Vector2.One, new Color("19262d"));
        var stack = OwnerPanel.Stack(Address);
        Texture2D? icon = stack == null ? null : OwnerPanel.Appearance.Icon(stack.ResourceId);
        if (icon != null)
        {
            Vector2 size = icon.GetSize() * 0.5f;
            DrawTextureRect(icon, new Rect2(((Size - size) / 2).Floor(), size), false);
        }
        if (OwnerPanel.Focused == Address)
            DrawRect(new Rect2(Vector2.One, Size - Vector2.One * 2), new Color("d4ad73"), false, 1);
        if (OwnerPanel.Selected == Address)
            DrawRect(new Rect2(Vector2.One * 2, Size - Vector2.One * 4), new Color("93b9a0"), false, 1);
    }
    public override void _GuiInput(InputEvent input)
    {
        if (input is not InputEventMouseButton mouse || !mouse.Pressed) return;
        OwnerPanel.PointerFocus(Address);
        if (mouse.ButtonIndex == MouseButton.Left)
        {
            _dragQuantity = OwnerPanel.Selected == Address ? OwnerPanel.SelectionQuantity : OwnerPanel.Stack(Address)?.Quantity ?? 0;
            _dragVersion = OwnerPanel.Selected == Address ? OwnerPanel.SelectionVersion : OwnerPanel.DisplayedVersion;
            if (mouse.ShiftPressed) OwnerPanel.QuickTransfer(Address); else OwnerPanel.Activate(Address);
        }
        else if (mouse.ButtonIndex == MouseButton.Right) OwnerPanel.BeginSplit(Address);
        else return;
        AcceptEvent();
    }
    public override Variant _GetDragData(Vector2 atPosition)
    {
        if (OwnerPanel.Stack(Address) is not { } stack || OwnerPanel.Splitting) return default;
        if (!OwnerPanel.BeginDrag(Address, _dragQuantity, _dragVersion)) return default;
        Texture2D? texture = OwnerPanel.Appearance.Icon(stack.ResourceId);
        if (texture != null)
            SetDragPreview(new TextureRect { Texture = texture, Size = texture.GetSize(), MouseFilter = MouseFilterEnum.Ignore });
        return "hollowlight-slot";
    }
    public override bool _CanDropData(Vector2 atPosition, Variant data) => OwnerPanel.Visible && !OwnerPanel.Splitting &&
        data.VariantType == Variant.Type.String && data.AsString() == "hollowlight-slot" && OwnerPanel.Selected != null;
    public override void _DropData(Vector2 atPosition, Variant data) => OwnerPanel.Activate(Address);
    public override void _Notification(int what)
    {
        if (what == NotificationDragEnd && IsInsideTree() && !GetViewport().GuiIsDragSuccessful()) OwnerPanel.CancelSelection();
    }
}
