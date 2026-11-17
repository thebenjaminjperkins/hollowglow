using System;
using Godot;
using Hollowlight.Core;

namespace Hollowlight.Presentation;

public partial class VehicleInspection : Control
{
    public event Action? PreserveRequested;
    public event Action<SupplyVersion>? DismantleRequested;
    public int ControllerDevice{get;set;}
    public int FocusedRow{get;private set;}
    private SupplyVersion _version,_pressed;
    public override void _Ready()
    {
        Hide();Position=new(55,55);TextureFilter=TextureFilterEnum.Nearest;
        AddChild(new ColorRect{Size=new(274,115),Color=new("202832f5"),MouseFilter=MouseFilterEnum.Ignore});
        var label=new Label{Position=new(8,6),Text="DAMAGED SERVICE VAN\nPreserve: leave the restoration opportunity intact.\nRestoration is not implemented.\nDismantle: remove it irreversibly for 4 Scrap.\nThis tradeoff is not yet economically balanced."};
        label.AddThemeFontSizeOverride("font_size",8);label.AddThemeColorOverride("font_color",new Color("ead4a9"));AddChild(label);
        var keep=Button("Preserve / Back",new(8,91));keep.Pressed+=()=>PreserveRequested?.Invoke();
        var scrap=Button("Choose dismantling...",new(139,91));scrap.ButtonDown+=()=>_pressed=_version;scrap.Pressed+=()=>DismantleRequested?.Invoke(_pressed);
    }
    private Button Button(string text,Vector2 position)
    {
        var b=new Button{Text=text,Position=position,Size=new(127,17),FocusMode=FocusModeEnum.None};b.AddThemeFontSizeOverride("font_size",8);AddChild(b);return b;
    }
    public void Open(SupplyVersion version){_version=version;FocusedRow=0;Show();Highlight();}
    public void ActivateRow(int row){if(row==0)PreserveRequested?.Invoke();else DismantleRequested?.Invoke(_version);}
    public override void _UnhandledInput(InputEvent e)
    {
        if(!Visible)return;
        if(e is InputEventJoypadButton joy&&joy.Device!=ControllerDevice)return;
        if(e is InputEventJoypadMotion motion&&motion.Device==ControllerDevice&&motion.Axis is JoyAxis.LeftX or JoyAxis.LeftY&&Math.Abs(motion.AxisValue)>.6f){FocusedRow=motion.AxisValue>0?1:0;Highlight();GetViewport().SetInputAsHandled();return;}
        if(e is InputEventKey k&&k.Pressed&&!k.Echo)
        {
            if(k.PhysicalKeycode is Key.Escape){PreserveRequested?.Invoke();}
            else if(k.PhysicalKeycode is Key.Left or Key.Up)FocusedRow=0;
            else if(k.PhysicalKeycode is Key.Right or Key.Down)FocusedRow=1;
            else if(k.PhysicalKeycode is Key.Enter or Key.Space)ActivateRow(FocusedRow);else return;
            GetViewport().SetInputAsHandled();
        }
        else if(e is InputEventJoypadButton j&&j.Pressed)
        {
            if(j.ButtonIndex==JoyButton.B)PreserveRequested?.Invoke();
            else if(j.ButtonIndex is JoyButton.DpadLeft or JoyButton.DpadUp)FocusedRow=0;
            else if(j.ButtonIndex is JoyButton.DpadRight or JoyButton.DpadDown)FocusedRow=1;
            else if(j.ButtonIndex==JoyButton.A)ActivateRow(FocusedRow);else return;
            GetViewport().SetInputAsHandled();
        }
        Highlight();
    }
    private void Highlight(){foreach(var node in GetChildren())if(node is Button button)button.Modulate=(button.Position.X<130)==(FocusedRow==0)?new Color("d4ad73"):Colors.White;}
}
