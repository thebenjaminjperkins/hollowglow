using System;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Hollowlight.Core;
using Hollowlight.Presentation;
using Hollowlight.World;

namespace Hollowlight.Tools;
public partial class PowerSmoke : Node
{
    private static bool _reset;
    private readonly SmokeInput _input=new();
    private ThresholdWorld _world=null!;
    private PlayerCharacter _actor=null!;
    private PowerSnapshot Power=>_world.Electrical!;
    private SuppliesSnapshot Stock=>_world.SuppliesFor(1);
    private PowerPanel Panel=>_world.PowerControls;
    public override async void _Ready()
    {
        try
        {
            var room=GD.Load<PackedScene>(ProjectSettings.GetSetting("application/run/main_scene").AsString()).Instantiate<SanctuaryRoom>();
            room.CraftingEnabled=false;room.InputOverride=_input;AddChild(room);_world=room.World;_actor=_world.GetNode<PlayerCharacter>("Player");await Frames(3);
            Check(!Power.Running&&Power.Fuel==0&&Power.Demand==0&&Stock.Storage.Count("fuel")==2,"Stopped empty 4-PU generator; two shared starter Fuel; all switches off");
            Check(_world.FuelWorld!.Sources.Count==2&&_world.ScavengingWorld!.Sources.Count==6&&_world.GrowingPlots.Count==4,
                "Two finite Fuel caches coexist with unchanged six Food sources and four plots");
            if(_reset)
            {
                Check(Power.Consumed==0&&!_world.PowerVisible&&!_actor.InventoryOpen,"Reset clears Fuel burn, power state and modal controls");
                GD.Print("PASS: live power controls, overload rejection, keyboard/controller/mouse, fractional burn/time, finite Fuel, manual refill/restart and reset");GetTree().Quit();return;
            }
            await AtGenerator();
            Input.ParseInputEvent(new InputEventJoypadButton {Device=1,ButtonIndex=JoyButton.DpadDown,Pressed=true});await Frames(2);
            Check(Panel.FocusedRow==0,"Other controller cannot change power navigation");
            Input.ParseInputEvent(new InputEventJoypadMotion {Device=0,Axis=JoyAxis.LeftY,AxisValue=1});await Frames(2);
            Input.ParseInputEvent(new InputEventJoypadMotion {Device=0,Axis=JoyAxis.LeftY,AxisValue=0});await Frames(2);
            Check(Panel.FocusedRow==1,"Assigned controller stick navigates power rows");await KeyPress(Key.Up);
            Panel.ActivateRow(0);await Frames(2);Check(!Power.Running&&Panel.Warning.Contains("Fuel"),"Starting without Fuel warns without changing state");
            _input.Movement=Vector2.Right;var position=_actor.Position;long ticks=_world.Clock.ElapsedTicks;await Frames(60);_input.Movement=Vector2.Zero;
            Check(_actor.Position==position&&_world.Clock.ElapsedTicks>ticks&&_actor.CollisionLayer==2,"Only menu owner is stationary; time and collision continue");
            _world.ClosePower();await Chest();
            Check(_world.TransferSupplies(_actor,"fuel",2,true,Stock.Version)?.Amount==2,"Withdraw starter Fuel through existing storage operations");
            _world.CloseInventory();await AtGenerator();
            Panel.ActivateRow(1);Panel.ActivateRow(1);await Frames(2);
            // The second same-frame request uses the old displayed revision and must reject.
            Check(Power.Fuel==1&&Stock.Personal.Count("fuel")==1,"Stale same-frame refuel cannot double-consume items");
            Panel.ActivateRow(2);await Frames(2);Check(Power.Fuel==2&&Stock.Personal.Count("fuel")==0,"Fill transfers remaining personal Fuel atomically");
            await KeyPress(Key.Up);await KeyPress(Key.Up);await KeyPress(Key.Enter);
            Check(Power.Running&&Power.RatePerDay==.2m,"Keyboard starts generator at idle burn rate");
            await KeyPress(Key.Down);await KeyPress(Key.Down);await KeyPress(Key.Down);await KeyPress(Key.Enter);
            Check(Power.Consumers.Single(c=>c.Id=="beacon").Powered&&Power.Demand==2,"Beacon receives its existing two-PU allocation");
            await Joy(JoyButton.DpadDown);await Joy(JoyButton.A);
            Check(Power.Demand==4&&Power.Consumers.Single(c=>c.Id=="workbench").Powered,"Controller enables workbench within 4-PU capacity");
            await ClickRow(5);await Frames(2);
            Check(Power.Running&&Power.Demand==4&&Power.Consumers.Single(c=>c.Id=="beacon").Powered&&Panel.Warning.Contains("5/4"),
                "Mouse over-capacity activation rejected; existing consumers and generator remain powered");
            Check(!LampsVisible(),"Lighting consumer stays off when capacity activation is rejected");
            await Capture("overload");
            Panel.ActivateRow(4);await Frames(2);Panel.ActivateRow(5);await Frames(2);
            Check(Power.Demand==3&&Power.Available==1&&Power.RatePerDay==.8m,"Manual switch-off makes room for lighting");
            Check(LampsVisible(),"Lighting consumer controls actual supplemental lamps");
            decimal before=Power.Fuel;_world.Clock.Advance(60);
            Check(Math.Abs(Power.Fuel-(before-.08m))<.000000000000000001m,"Direct time advance burns exact fractional Fuel at current load");
            await Capture("running");
            // A held mouse command is versioned at press, not release.
            await MouseRow(0,true);
            var result=_world.PowerCommand(_actor,new(PowerAction.SetConsumer,Power.Version,Stock.Version,false,"lighting"));
            Check(result.Status==PowerStatus.Applied,"Concurrent control command accepted");
            await Frames(2);await MouseRow(0,false);
            Check(Power.Running&&Panel.Warning.Contains("changed"),"Held stale mouse Stop rejected without changing the newer power state");
            var old=new PowerRequest(PowerAction.LoadFuel,Power.Version,Stock.Version);
            _actor.Position=new Vector2(400,208);await Frames(3);
            Check(!_world.PowerVisible&&!_actor.InventoryOpen&&_world.PowerCommand(_actor,old).Status==PowerStatus.AccessDenied,"Leaving station closes controls and rejects stale remote commands");
            foreach(var source in _world.FuelWorld.Sources)
            {
                _actor.Position=new Vector2(source.ApproachX,source.ApproachY);await Frames(3);
                var query=new PhysicsShapeQueryParameters2D {Shape=new RectangleShape2D {Size=new Vector2(8,6)},Transform=new Transform2D(0,_actor.Position),CollisionMask=1};
                Check(_world.GetWorld2D().DirectSpaceState.IntersectShape(query).Count==0,"Generated Fuel approach clears actual feet collision");
                await Capture("fuel-available",source);_input.Tap=true;await Frames(3);
                var target=_world.GetChildren().OfType<Interactable>().Single(t=>t.ObjectId==source.Site.Id);
                Check(Stock.Remaining[source.Site.Id]==0&&target.Visible&&target.GetNode<ScavengingArt>("FoodArt").StateFrame==1,"Finite Fuel gathers normally and leaves depleted PNG visible");
                await Capture("fuel-empty",source);
            }
            int carried=Stock.Personal.Count("fuel");_world.Clock.Advance(6000);await Frames(3);
            Check(!Power.Running&&Power.Fuel==0&&Power.Consumers.All(c=>!c.Powered)&&Stock.Personal.Count("fuel")==carried,
                "Exhaustion stops supply without consuming carried Fuel or auto-refueling");
            Check(_world.FuelWorld.Sources.All(s=>Stock.Remaining[s.Site.Id]==0),"Time skips do not respawn Fuel caches");
            await AtGenerator();Panel.ActivateRow(2);await Frames(2);
            Check(Power.Fuel==5&&!Power.Running&&Stock.Personal.Count("fuel")==carried-5,"Five-unit tank cap preserves refuel remainder and stays stopped");
            Panel.ActivateRow(0);await Frames(2);Check(Power.Running,"Manual restart restores selected consumers after refueling");
            _world.ClosePower();
            var bench=_world.PowerStations.Single(s=>s.Id=="workbench");_actor.Position=bench.Position+new Vector2(0,12);await Frames(3);_input.Tap=true;await Frames(3);
            Check(_world.PowerVisible&&Panel.FocusedRow==4,"Local machine interaction opens its own focused switch");
            await Joy(JoyButton.B);Check(!_world.PowerVisible&&!_actor.InventoryOpen,"Controller cancellation releases movement");
            _reset=true;_world._UnhandledKeyInput(new InputEventKey {PhysicalKeycode=Key.R,Pressed=true});
        }
        catch(Exception error) {GD.PushError(error.ToString());GetTree().Quit(1);}
    }
    private bool LampsVisible()=>WorldVisuals.Descendants(_world).OfType<PointLight2D>()
        .Where(p=>p.HasMeta("power_consumer")&&p.GetMeta("power_consumer").AsString()=="lighting").Any(p=>p.IsVisibleInTree());
    private async Task AtGenerator()
    {
        var station=_world.PowerStations.Single(s=>s.Id=="generator");_actor.Position=station.Position+new Vector2(0,12);
        await Frames(3);_input.Tap=true;await Frames(3);Check(_world.PowerVisible,"Nearby generator interaction opens power controls");
    }
    private async Task Chest() {_actor.Position=_world.StoragePosition+new Vector2(0,18);await Frames(3);_input.Tap=true;await Frames(3);}
    private async Task KeyPress(Key key)
    {Input.ParseInputEvent(new InputEventKey {PhysicalKeycode=key,Pressed=true});await Frames(2);Input.ParseInputEvent(new InputEventKey {PhysicalKeycode=key,Pressed=false});await Frames(2);}
    private async Task Joy(JoyButton button)
    {Input.ParseInputEvent(new InputEventJoypadButton {Device=0,ButtonIndex=button,Pressed=true});await Frames(2);Input.ParseInputEvent(new InputEventJoypadButton {Device=0,ButtonIndex=button,Pressed=false});await Frames(2);}
    private async Task ClickRow(int row) {await MouseRow(row,true);await MouseRow(row,false);}
    private async Task MouseRow(int row,bool pressed)
    {
        var button=Panel.GetChildren().OfType<Button>().ElementAt(row);
        var point=GetViewport().GetFinalTransform()*(button.GetGlobalTransformWithCanvas()*(button.Size/2));
        Input.ParseInputEvent(new InputEventMouseMotion {Position=point,GlobalPosition=point});await Frames(1);
        Input.ParseInputEvent(new InputEventMouseButton {Position=point,GlobalPosition=point,ButtonIndex=MouseButton.Left,Pressed=pressed,ButtonMask=pressed?MouseButtonMask.Left:0});await Frames(2);
    }
    private async Task Frames(int count)
    {for(int i=0;i<count;i++){await ToSignal(GetTree(),SceneTree.SignalName.PhysicsFrame);await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}}
    private async Task Capture(string name,GeneratedSourceSite? source=null)
    {
        if(!OS.GetCmdlineUserArgs().Contains("--capture-power"))return;
        var saved=_actor.Position;if(source!=null)_actor.Position=new(source.Site.X-12,source.Site.Y+4);
        await Frames(4);await ToSignal(RenderingServer.Singleton,RenderingServer.SignalName.FramePostDraw);
        string folder=ProjectSettings.GlobalizePath("user://power");DirAccess.MakeDirRecursiveAbsolute(folder);
        Check(GetViewport().GetTexture().GetImage().SavePng($"{folder}/{name}.png")==Error.Ok,"Capture "+name);
        _actor.Position=saved;await Frames(2);
    }
    private static void Check(bool value,string message)
    {if(!value)throw new InvalidOperationException(message);GD.Print("PASS: "+message);}
}
