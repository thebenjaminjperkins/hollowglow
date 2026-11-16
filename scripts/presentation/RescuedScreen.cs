using System;
using Godot;

namespace Hollowlight.Presentation;

public partial class RescuedScreen : Control
{
    public int ControllerDevice {get;set;}
    public bool TransitionStarted {get;private set;}
    public int FocusedRow {get;private set;}
    private readonly Button[] _buttons=new Button[2];
    public override void _Ready()
    {
        Visible=false;ProcessMode=ProcessModeEnum.Always;
        AddChild(new ColorRect {Size=new Vector2(384,216),Color=new Color("202832ef"),MouseFilter=MouseFilterEnum.Stop});
        AddText("RESCUED",new Vector2(126,48),24);
        AddText("Your signal led rescuers to the sanctuary.",new Vector2(64,86),11);
        AddText("Transmission complete. The run is finished.",new Vector2(66,104),9);
        for(int i=0;i<2;i++)
        {
            int row=i;var button=new Button {Text=i==0?"New Run":"Quit",Position=new Vector2(112,132+i*25),FocusMode=FocusModeEnum.None};
            button.AddThemeFontSizeOverride("font_size",12);
            button.AddThemeColorOverride("font_color",new Color("ead4a9"));
            foreach(string style in new[]{"normal","hover","pressed"})button.AddThemeStyleboxOverride(style,new StyleBoxFlat {BgColor=new Color(style=="normal"?"303b3e":"47555e"),BorderColor=new Color("87785b"),BorderWidthBottom=1});
            AddChild(button);button.Size=new Vector2(160,21);_buttons[i]=button;button.Pressed+=()=>Activate(row);button.MouseEntered+=()=>FocusedRow=row;
        }
    }
    private void AddText(string text,Vector2 position,int size)
    {var label=new Label {Text=text,Position=position,MouseFilter=MouseFilterEnum.Ignore};label.AddThemeFontSizeOverride("font_size",size);label.AddThemeColorOverride("font_color",new Color("ead4a9"));AddChild(label);}
    public void Present(){FocusedRow=0;TransitionStarted=false;Show();}
    public void Activate(int row)
    {
        if(!Visible||TransitionStarted)return;TransitionStarted=true;
        var tree=GetTree();tree.Paused=false;
        if(row==0)tree.ReloadCurrentScene();else tree.Quit();
    }
    public override void _UnhandledInput(InputEvent input)
    {
        if(!Visible)return;
        if(input is InputEventJoypadButton&&input.Device!=ControllerDevice)return;
        if(input is InputEventKey key&&key.Pressed&&!key.Echo)
        {
            if(key.PhysicalKeycode is Key.Up or Key.Down)FocusedRow=1-FocusedRow;
            else if(key.PhysicalKeycode==Key.Enter){GetViewport().SetInputAsHandled();Activate(FocusedRow);return;}else return;
        }
        else if(input is InputEventJoypadButton joy&&joy.Pressed)
        {
            if(joy.ButtonIndex is JoyButton.DpadUp or JoyButton.DpadDown)FocusedRow=1-FocusedRow;
            else if(joy.ButtonIndex==JoyButton.A){GetViewport().SetInputAsHandled();Activate(FocusedRow);return;}else return;
        }
        else return;
        GetViewport().SetInputAsHandled();
    }
    public override void _Process(double delta)
    {if(Visible)for(int i=0;i<2;i++)_buttons[i].SelfModulate=i==FocusedRow?new Color("d4ad73"):Colors.White;}
}
