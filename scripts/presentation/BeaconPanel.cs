using System;
using System.Collections.Generic;
using Godot;
using Hollowlight.Core;

namespace Hollowlight.Presentation;

public partial class BeaconPanel : Control
{
    public event Action? CloseRequested, AllocationRequested;
    public event Action<PowerRequest>? OperationRequested;
    public int ControllerDevice { get; set; }
    public int FocusedRow { get; private set; }
    private Label _status=null!,_progress=null!,_remaining=null!,_outage=null!,_history=null!,_warning=null!;
    private readonly List<Button> _buttons=new();
    private PowerSnapshot _power=null!;
    private SupplyVersion _inventory;
    private string _notice="";
    private double _noticeTime,_repeat;
    private int _stick;
    private PowerRequest? _pressed;
    public override void _Ready()
    {
        Visible=false;Position=new Vector2(56,29);MouseFilter=MouseFilterEnum.Ignore;
        AddChild(new ColorRect {Size=new Vector2(272,174),Color=new Color("202832f5"),MouseFilter=MouseFilterEnum.Ignore});
        Text("RADIO BEACON / RESCUE",8,4,10);
        _status=Text("",8,22);_progress=Text("",8,36);_remaining=Text("",8,50);
        _outage=Text("",8,64);_history=Text("",8,78);_warning=Text("",8,94);
        _warning.AddThemeColorOverride("font_color",new Color("e8ab85"));
        for(int i=0;i<2;i++)
        {
            int row=i;var button=new Button {Position=new Vector2(8,123+i*16),FocusMode=FocusModeEnum.None};
            button.AddThemeFontSizeOverride("font_size",8);button.AddThemeColorOverride("font_color",new Color("ead4a9"));
            foreach(string style in new[]{"normal","hover","pressed"})button.AddThemeStyleboxOverride(style,new StyleBoxFlat {BgColor=new Color(style=="normal"?"303b3e":"47555e"),BorderColor=new Color("87785b"),BorderWidthBottom=1});
            AddChild(button);button.Size=new Vector2(256,14);_buttons.Add(button);
            button.MouseEntered+=()=>FocusedRow=row;
            button.ButtonDown+=()=>_pressed=Request();
            button.Pressed+=()=>{if(row==0&&_pressed!=null)OperationRequested?.Invoke(_pressed);else if(row==1)AllocationRequested?.Invoke();_pressed=null;};
        }
        Text("Up/down Select   Enter / A Apply   Esc / B Close",8,158);
    }
    private Label Text(string value,float x,float y,int size=8)
    {
        var label=new Label {Text=value,Position=new Vector2(x,y),MouseFilter=MouseFilterEnum.Ignore};
        label.AddThemeFontSizeOverride("font_size",size);label.AddThemeColorOverride("font_color",new Color("ead4a9"));AddChild(label);return label;
    }
    public void Open(RescueSnapshot rescue,PowerSnapshot power,SupplyVersion inventory)
    {Visible=true;FocusedRow=0;_notice="";_noticeTime=0;_stick=0;Refresh(rescue,power,inventory);}
    public void Close(){Hide();_pressed=null;_stick=0;}
    public void Refresh(RescueSnapshot rescue,PowerSnapshot power,SupplyVersion inventory)
    {
        _power=power;_inventory=inventory;if(!Visible)return;
        _status.Text=$"{rescue.Status.ToString().ToUpperInvariant()} | Beacon requires {System.Linq.Enumerable.Single(power.Consumers,c=>c.Id=="beacon").Demand} PU";
        _status.Modulate=new Color(rescue.Status==SignalStatus.Transmitting?"93b9a0":rescue.Status==SignalStatus.Interrupted?"d4ad73":"93a39b");
        _progress.Text=$"Signal {rescue.SignalDays:0.000} / {rescue.RequiredDays:0.##} days";
        _remaining.Text=$"{(rescue.Status==SignalStatus.Transmitting?"ETA if uninterrupted":"Paused; transmission needed")}: {Duration(rescue.RemainingTicks)}";
        _outage.Text=$"Current outage {Duration(rescue.CurrentOutageTicks)} | Grace left {Duration(rescue.GraceRemainingTicks)}";
        _history.Text=$"Episode loss {rescue.EpisodeLossDays:0.000} / {rescue.MaxEpisodeLossDays:0.##} day | Rearm in {Duration(rescue.StableRecoveryRemainingTicks)}";
        string warning=rescue.LossCapped?"Setback capped. Restore transmission to recover.":rescue.Degrading?$"Signal degrading: {rescue.LossPerOutageDay:0.##} day per outage day.":
            rescue.EpisodeOutageTicks>0?"Brief restorations keep outage history.":"";
        _warning.Text=rescue.Degrading||rescue.LossCapped?warning:_noticeTime>0?_notice:warning;
        bool enabled=System.Linq.Enumerable.Any(power.Consumers,c=>c.Id=="beacon"&&c.Enabled);
        _buttons[0].Text=enabled?"Switch beacon OFF":"Switch beacon ON";_buttons[1].Text="Power allocation / generator controls";
    }
    public static string Duration(long ticks)
    {long seconds=(long)Math.Ceiling((double)ticks/TimeSpan.TicksPerSecond);return $"{seconds/60:00}:{seconds%60:00}";}
    private PowerRequest Request()=>new(PowerAction.SetConsumer,_power.Version,_inventory,
        !System.Linq.Enumerable.Any(_power.Consumers,c=>c.Id=="beacon"&&c.Enabled),"beacon");
    public void ActivateRow(int row)
    {if(!Visible)return;FocusedRow=Math.Clamp(row,0,1);if(row==0)OperationRequested?.Invoke(Request());else AllocationRequested?.Invoke();}
    public void ShowResult(PowerResult result)
    {
        _notice=result.Status switch {PowerStatus.NoSupply=>"Start the generator in Power allocation.",PowerStatus.OverCapacity=>$"Overload {result.RequestedDemand}/{result.Capacity} PU. Free capacity first.",
            PowerStatus.Stale=>"Power changed; review and try again.",PowerStatus.Applied=>"Beacon switch updated.",_=>"Beacon control unavailable."};_noticeTime=4;
    }
    public override void _UnhandledInput(InputEvent input)
    {
        if(!Visible)return;
        if(input is InputEventJoypadButton or InputEventJoypadMotion&&input.Device!=ControllerDevice)return;
        bool handled=true;
        if(input is InputEventKey key&&key.Pressed)
        {
            switch(key.PhysicalKeycode)
            {case Key.Up:case Key.Down:FocusedRow=1-FocusedRow;break;case Key.Enter:if(!key.Echo)ActivateRow(FocusedRow);break;
             case Key.Escape:case Key.Tab:if(!key.Echo)CloseRequested?.Invoke();break;default:handled=false;break;}
        }
        else if(input is InputEventJoypadButton joy&&joy.Pressed)
        {
            switch(joy.ButtonIndex){case JoyButton.DpadDown:case JoyButton.DpadUp:FocusedRow=1-FocusedRow;break;case JoyButton.A:ActivateRow(FocusedRow);break;
                case JoyButton.B:case JoyButton.Back:CloseRequested?.Invoke();break;default:handled=false;break;}
        }
        else if(input is InputEventJoypadMotion motion&&motion.Axis==JoyAxis.LeftY)
        {int next=Math.Abs(motion.AxisValue)>.55?Math.Sign(motion.AxisValue):0;if(next!=_stick)_repeat=0;_stick=next;}
        else handled=false;
        if(handled)GetViewport().SetInputAsHandled();
    }
    public override void _Process(double delta)
    {
        if(!Visible)return;_noticeTime=Math.Max(0,_noticeTime-delta);
        if(_stick!=0){_repeat-=delta;if(_repeat<=0){FocusedRow=1-FocusedRow;_repeat=.18;}}
        for(int i=0;i<_buttons.Count;i++)_buttons[i].SelfModulate=i==FocusedRow?new Color("d4ad73"):Colors.White;
    }
}
