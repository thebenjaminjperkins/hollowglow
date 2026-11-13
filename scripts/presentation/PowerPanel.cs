using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Hollowlight.Core;

namespace Hollowlight.Presentation;

public partial class PowerPanel : Control
{
    public event Action? CloseRequested;
    public event Action<PowerRequest>? OperationRequested;
    public int ControllerDevice { get; set; }
    public int FocusedRow { get; private set; }
    public string Warning => _notice;
    private PowerSnapshot _power = null!;
    private SupplyVersion _inventory;
    private readonly List<Button> _buttons = new();
    private Label _summary = null!, _warning = null!;
    private string _notice = "";
    private double _noticeTime, _repeat;
    private Vector2 _stick;
    private PowerRequest? _pointerRequest;
    public override void _Ready()
    {
        Position = new Vector2(58,28); MouseFilter = MouseFilterEnum.Ignore; Visible = false;
        AddChild(new ColorRect { Size = new Vector2(268,176), Color = new Color("202832f5"), MouseFilter = MouseFilterEnum.Ignore });
        AddText("POWER / MANUAL ALLOCATION",8,4,10);
        _summary = AddText("",8,18,8); _summary.Size = new Vector2(252,42);
        for(int row=0;row<6;row++)
        {
            int index=row;
            var button=new Button { Position=new Vector2(8,63+row*14), FocusMode=FocusModeEnum.None };
            button.AddThemeFontSizeOverride("font_size",8);
            button.AddThemeColorOverride("font_color",new Color("ead4a9"));
            foreach(string state in new[]{"normal","hover","pressed"}) button.AddThemeStyleboxOverride(state,new StyleBoxFlat {
                BgColor=new Color(state=="normal"?"303b3e":"47555e"),BorderColor=new Color("87785b"),BorderWidthBottom=1 });
            AddChild(button); button.Size=new Vector2(252,12); _buttons.Add(button);
            button.MouseEntered += ()=> { FocusedRow=index; };
            button.ButtonDown += ()=>_pointerRequest=Request(index);
            button.Pressed += ()=> { if(_pointerRequest!=null) OperationRequested?.Invoke(_pointerRequest); _pointerRequest=null; };
        }
        _warning=AddText("",8,148,8); _warning.Size=new Vector2(252,12); _warning.AddThemeColorOverride("font_color",new Color("e8ab85"));
        AddText("Up/down Select   Enter / A Apply   Esc / B Close",8,162,8);
    }
    private Label AddText(string text,float x,float y,int size)
    {
        var label=new Label { Text=text,Position=new Vector2(x,y),MouseFilter=MouseFilterEnum.Ignore };
        label.AddThemeFontSizeOverride("font_size",size);label.AddThemeColorOverride("font_color",new Color("ead4a9"));AddChild(label);return label;
    }
    public void Open(PowerSnapshot power, SupplyVersion inventory, string focus = "generator")
    {
        _notice="";_noticeTime=0;_stick=Vector2.Zero;
        FocusedRow=focus=="generator"?0:3+power.Consumers.ToList().FindIndex(c=>c.Id==focus);
        FocusedRow=Math.Clamp(FocusedRow,0,5);Visible=true;Refresh(power,inventory);
    }
    public void Close() { Hide();_pointerRequest=null;_stick=Vector2.Zero; }
    public void Refresh(PowerSnapshot power, SupplyVersion inventory)
    {
        _power=power;_inventory=inventory;
        if(!Visible)return;
        long seconds=(power.RuntimeTicks??0)/TimeSpan.TicksPerSecond;
        string runtime=power.Running?$"{seconds/60:00}:{seconds%60:00}":"--";
        _summary.Text=$"{(power.Running?"RUNNING":"STOPPED")} | Load {power.Demand}/{power.Capacity} PU | Free {power.Available} PU\n"+
            $"Fuel {power.Fuel:0.000}/{power.TankCapacity} | Burn {power.RatePerDay:0.0}/day\nRuntime at current load: {runtime}";
        _buttons[0].Text=power.Running?"Stop generator":"Start generator";
        _buttons[1].Text="Load 1 Fuel from backpack";_buttons[2].Text="Fill tank from backpack";
        for(int i=0;i<power.Consumers.Count;i++)
        {
            var c=power.Consumers[i];
            _buttons[i+3].Text=$"{(c.Enabled?"ON ":"OFF")}  {c.Name}  {c.Demand} PU  | {(c.Powered?"Powered":c.Enabled?"No supply":"Off")}";
        }
        _warning.Text=_noticeTime>0?_notice:"";
    }
    private PowerRequest Request(int row) => row switch {
        0=>new(PowerAction.SetRunning,_power.Version,_inventory,!_power.Running),
        1=>new(PowerAction.LoadFuel,_power.Version,_inventory,Quantity:1),
        2=>new(PowerAction.LoadFuel,_power.Version,_inventory,Quantity:int.MaxValue),
        _=>new(PowerAction.SetConsumer,_power.Version,_inventory,!_power.Consumers[row-3].Enabled,_power.Consumers[row-3].Id)
    };
    public void ActivateRow(int row) { if(Visible && row>=0 && row<_buttons.Count) { FocusedRow=row;OperationRequested?.Invoke(Request(row)); } }
    public void ShowResult(PowerResult result)
    {
        _notice=result.Status switch {
            PowerStatus.OverCapacity=>$"Overload {result.RequestedDemand}/{result.Capacity} PU. Switch another consumer off.",
            PowerStatus.NoFuel=>"No Fuel in backpack. Collect or withdraw some first.",
            PowerStatus.TankFull=>"Tank cannot fit a whole unit. Fuel stays in backpack.",
            PowerStatus.NoSupply=>"Start the generator before enabling another consumer.",
            PowerStatus.Stale=>"State changed. Review and try again.",
            PowerStatus.Applied=>result.Amount>0?$"Loaded {result.Amount} Fuel.":"Power controls updated.",
            PowerStatus.NoChange=>"Already set.", _=>"Power controls unavailable."
        };
        _noticeTime=5;_warning.Text=_notice;
    }
    public override void _UnhandledInput(InputEvent input)
    {
        if(!Visible)return;
        if(input is InputEventJoypadButton or InputEventJoypadMotion && input.Device!=ControllerDevice)return;
        bool handled=true;
        if(input is InputEventKey key && key.Pressed)
        {
            switch(key.PhysicalKeycode)
            {
                case Key.Up:FocusedRow=(FocusedRow+5)%6;break;
                case Key.Down:FocusedRow=(FocusedRow+1)%6;break;
                case Key.Enter:if(!key.Echo)ActivateRow(FocusedRow);break;
                case Key.Escape:case Key.Tab:if(!key.Echo)CloseRequested?.Invoke();break;
                default:handled=false;break;
            }
        }
        else if(input is InputEventJoypadButton joy && joy.Pressed)
        {
            switch(joy.ButtonIndex)
            {
                case JoyButton.DpadUp:FocusedRow=(FocusedRow+5)%6;break;
                case JoyButton.DpadDown:FocusedRow=(FocusedRow+1)%6;break;
                case JoyButton.A:ActivateRow(FocusedRow);break;
                case JoyButton.B:case JoyButton.Back:CloseRequested?.Invoke();break;
                default:handled=false;break;
            }
        }
        else if(input is InputEventJoypadMotion motion && motion.Axis==JoyAxis.LeftY)
        { float next=Math.Abs(motion.AxisValue)>.55f?Math.Sign(motion.AxisValue):0; if(next!=_stick.Y)_repeat=0;_stick.Y=next; }
        else handled=false;
        if(handled)GetViewport().SetInputAsHandled();
    }
    public override void _Process(double delta)
    {
        if(!Visible)return;_noticeTime=Math.Max(0,_noticeTime-delta);
        if(_stick.Y!=0) { _repeat-=delta;if(_repeat<=0) { FocusedRow=(FocusedRow+(int)_stick.Y+6)%6;_repeat=.18; } }
        for(int i=0;i<_buttons.Count;i++)_buttons[i].SelfModulate=i==FocusedRow?new Color("d4ad73"):Colors.White;
        if(_noticeTime==0)_warning.Text="";
    }
}
