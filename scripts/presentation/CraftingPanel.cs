using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Hollowlight.Core;

namespace Hollowlight.Presentation;

public partial class CraftingPanel : Control
{
    public event Action? CloseRequested,AllocationRequested;
    public event Action<CraftRequest>? CraftRequested;
    public int ControllerDevice {get;set;}
    public int FocusedRow {get;private set;}
    public int SelectedRecipe {get;private set;}
    public string Warning=>_notice;
    private CraftingSnapshot _state=null!;
    private InventoryAppearance _appearance=null!;
    private readonly List<Button> _buttons=new();
    private readonly List<Label> _names=new();
    private readonly List<Label> _requirements=new();
    private Label _title=null!,_description=null!,_feedback=null!;
    private CraftRequest? _pressed;
    private string _notice="";
    private bool _noticeOk;
    private double _noticeTime,_repeat;
    private int _stick;
    public override void _Ready()
    {
        Visible=false;Position=new Vector2(36,29);MouseFilter=MouseFilterEnum.Ignore;
        _appearance=InventoryAppearance.Load();
        AddChild(new ColorRect {Size=new Vector2(312,177),Color=new Color("202832f5"),MouseFilter=MouseFilterEnum.Ignore});
        _title=Text("WORKBENCH / CRAFTING",8,4,10);
        for(int i=0;i<3;i++)
        {
            int row=i;var button=MakeButton("",new Vector2(8,23+i*30),new Vector2(296,27));
            button.Pressed+=()=>Select(row);button.MouseEntered+=()=>FocusedRow=row;_buttons.Add(button);
            _names.Add(Text("",31,25+i*30,8));
            _requirements.Add(Text("",31,37+i*30,8));
        }
        _description=Text("",8,115,8);_description.Size=new Vector2(296,27);_description.AutowrapMode=TextServer.AutowrapMode.WordSmart;
        var craft=MakeButton("Craft selected  Enter / A",new Vector2(8,144),new Vector2(144,15));
        craft.ButtonDown+=()=>_pressed=Request();craft.Pressed+=()=>{if(_pressed!=null)CraftRequested?.Invoke(_pressed);_pressed=null;};craft.MouseEntered+=()=>FocusedRow=3;_buttons.Add(craft);
        var power=MakeButton("Power allocation",new Vector2(160,144),new Vector2(144,15));
        power.Pressed+=()=>AllocationRequested?.Invoke();power.MouseEntered+=()=>FocusedRow=4;_buttons.Add(power);
        _feedback=Text("",8,163,8);
    }
    private Button MakeButton(string text,Vector2 position,Vector2 size)
    {
        var button=new Button {Text=text,Position=position,FocusMode=FocusModeEnum.None};
        button.AddThemeFontSizeOverride("font_size",8);button.AddThemeColorOverride("font_color",new Color("ead4a9"));
        foreach(string style in new[]{"normal","hover","pressed"})button.AddThemeStyleboxOverride(style,new StyleBoxFlat {
            BgColor=new Color(style=="normal"?"303b3e":"47555e"),BorderColor=new Color("87785b"),BorderWidthBottom=1});
        AddChild(button);button.Size=size;return button;
    }
    private Label Text(string text,float x,float y,int size)
    {
        var label=new Label {Text=text,Position=new Vector2(x,y),MouseFilter=MouseFilterEnum.Ignore};
        label.AddThemeFontSizeOverride("font_size",size);label.AddThemeColorOverride("font_color",new Color("ead4a9"));AddChild(label);return label;
    }
    public void Open(CraftingSnapshot state,SuppliesSnapshot stock,IReadOnlyList<ResourceDefinition> resources)
    {
        _state=state;SelectedRecipe=FocusedRow=0;_noticeTime=0;_notice="";_stick=0;Visible=true;
        // Icons are approved PNG atlas regions, not new procedural asset drawings.
        foreach(var old in GetChildren().OfType<Sprite2D>().ToArray()){RemoveChild(old);old.QueueFree();}
        for(int i=0;i<state.Recipes.Count;i++)
        {
            var icon=_appearance.Icon(state.Recipes[i].Recipe.Icon);
            if(icon!=null)AddChild(new Sprite2D {Texture=icon,Position=new Vector2(19,36+i*30),Scale=Vector2.One*.5f,TextureFilter=TextureFilterEnum.Nearest});
        }
        Refresh(state,stock,resources);
    }
    public void Refresh(CraftingSnapshot state,SuppliesSnapshot stock,IReadOnlyList<ResourceDefinition> resources)
    {
        _state=state;if(!Visible)return;
        bool powered=!state.Recipes.Any(r=>r.Status==CraftStatus.NoPower);
        _title.Text=powered?"WORKBENCH / 2 PU | POWERED":"WORKBENCH / 2 PU | OFFLINE";
        for(int i=0;i<state.Recipes.Count;i++)
        {
            var recipe=state.Recipes[i].Recipe;
            _names[i].Text=recipe.Name;
            _buttons[i].AddThemeConstantOverride("outline_size",0);
            _requirements[i].Text=string.Join("   ",recipe.Inputs.Select(p=>$"{resources.First(r=>r.Id==p.Key).ShortName} {stock.Personal.Count(p.Key)}/{p.Value}"));
        }
        _description.Text=state.Recipes[SelectedRecipe].Recipe.Description;
        _feedback.Text=!powered?Message(CraftStatus.NoPower):_noticeTime>0?_notice:Message(state.Recipes[SelectedRecipe].Status);
        _feedback.Modulate=new Color(powered&&(_noticeTime>0?_noticeOk:state.Recipes[SelectedRecipe].Status==CraftStatus.Applied)?"93b9a0":"e8ab85");
    }
    public void Close(){Hide();_pressed=null;_stick=0;}
    public void Select(int row){SelectedRecipe=Math.Clamp(row,0,_state.Recipes.Count-1);FocusedRow=3;_noticeTime=0;_pressed=null;}
    private CraftRequest Request()=>new(_state.Recipes[SelectedRecipe].Recipe.Id,_state.Version,_state.PowerVersion);
    public void ActivateRow(int row)
    {
        if(!Visible)return;
        if(row<3)Select(row);else if(row==3)CraftRequested?.Invoke(Request());else AllocationRequested?.Invoke();
    }
    public void ShowResult(CraftResult result){_noticeOk=result.Status==CraftStatus.Applied;_notice=result.Status==CraftStatus.Applied?"Crafted. Output is in your backpack.":Message(result.Status);_noticeTime=4;}
    public static string Message(CraftStatus status)=>status switch {
        CraftStatus.Applied=>"Ready to craft.",CraftStatus.NoPower=>"Workbench needs power. Open Power allocation.",
        CraftStatus.Insufficient=>"Missing ingredients in backpack. Withdraw from chest.",CraftStatus.NoCapacity=>"No output space. Inputs kept; make room first.",
        CraftStatus.Stale=>"Supplies or power changed. Review and try again.",CraftStatus.AlreadyInstalled=>"Already installed. Kit kept.",
        CraftStatus.LimitReached=>"Upgrade limit / enough kits already exist.",_=>"Unavailable. No ingredients spent."};
    private void Navigate(int step)
    {FocusedRow=(FocusedRow+step+5)%5;if(FocusedRow<3){SelectedRecipe=FocusedRow;_noticeTime=0;_pressed=null;}}
    public override void _UnhandledInput(InputEvent input)
    {
        if(!Visible)return;
        if(input is InputEventJoypadButton or InputEventJoypadMotion && input.Device!=ControllerDevice)return;
        bool handled=true;
        if(input is InputEventKey key&&key.Pressed)
        {
            switch(key.PhysicalKeycode){case Key.Up:Navigate(-1);break;case Key.Down:Navigate(1);break;
                case Key.Enter:if(!key.Echo)ActivateRow(FocusedRow);break;
                case Key.Escape:case Key.Tab:if(!key.Echo)CloseRequested?.Invoke();break;default:handled=false;break;}
        }
        else if(input is InputEventJoypadButton joy&&joy.Pressed)
        {
            switch(joy.ButtonIndex){case JoyButton.DpadUp:Navigate(-1);break;case JoyButton.DpadDown:Navigate(1);break;
                case JoyButton.A:ActivateRow(FocusedRow);break;case JoyButton.B:case JoyButton.Back:CloseRequested?.Invoke();break;default:handled=false;break;}
        }
        else if(input is InputEventJoypadMotion motion&&motion.Axis==JoyAxis.LeftY)
        {int next=Math.Abs(motion.AxisValue)>.55?Math.Sign(motion.AxisValue):0;if(next!=_stick)_repeat=0;_stick=next;}
        else handled=false;
        if(handled)GetViewport().SetInputAsHandled();
    }
    public override void _Process(double delta)
    {
        if(!Visible)return;_noticeTime=Math.Max(0,_noticeTime-delta);
        if(_stick!=0){_repeat-=delta;if(_repeat<=0){Navigate(_stick);_repeat=.18;}}
        for(int i=0;i<_buttons.Count;i++)_buttons[i].SelfModulate=i==FocusedRow?new Color("d4ad73"):i==SelectedRecipe?new Color("b8cbb5"):Colors.White;
    }
}
