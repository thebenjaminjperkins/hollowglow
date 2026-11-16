using System;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Hollowlight.Core;
using Hollowlight.Presentation;
using Hollowlight.World;

namespace Hollowlight.Tools;
public partial class RescueSmoke : Node
{
    private static bool _reset;
    private readonly SmokeInput _input=new();
    private ThresholdWorld _world=null!;
    private PlayerCharacter _actor=null!;
    private PowerSnapshot Power=>_world.Electrical!;
    private RescueSnapshot Signal=>_world.Signal!;
    private SuppliesSnapshot Stock=>_world.SuppliesFor(1);
    public override async void _Ready()
    {
        ProcessMode=ProcessModeEnum.Always;
        try
        {
            var room=GD.Load<PackedScene>(ProjectSettings.GetSetting("application/run/main_scene").AsString()).Instantiate<SanctuaryRoom>();
            room.ProcessMode=ProcessModeEnum.Pausable;room.InputOverride=_input;AddChild(room);_world=room.World;_actor=_world.GetNode<PlayerCharacter>("Player");await Frames(3);
            Check(!Signal.Completed&&Signal.SignalDays==0&&!_world.Clock.Frozen&&!GetTree().Paused,"Fresh run has no signal, outage or victory freeze");
            if(_reset)
            {
                Check(Stock.Personal.Count("food")==2&&Stock.Storage.Count("fuel")==2&&Power.Consumed==0&&_world.GrowingPlots.Count==4,"New Run restores unchanged starting Food, Fuel, four plots and clock");
                GD.Print("PASS: contextual beacon controls, outage/recovery, terminal rescue freeze, exactly-once completion and New Run reset");GetTree().Quit();return;
            }
            _world.Clock.Advance(1800);await Frames(2);
            Check(Signal.Status==SignalStatus.Offline&&Signal.TotalLossDays==0&&Signal.CurrentOutageTicks==0,"Startup delays do not penalize a never-started signal");
            await Beacon();await Capture("offline");
            _world.BeaconControls.ActivateRow(0);await Frames(2);Check(!Power.Running,"Beacon toggle cannot manufacture electrical supply");
            await KeyPress(Key.Down);await KeyPress(Key.Enter);Check(_world.PowerVisible&&!_world.BeaconVisible,"Keyboard opens existing power allocation from beacon context");
            _world.ClosePower();
            foreach(var source in _world.FuelWorld!.Sources)
            {
                _actor.Position=new(source.ApproachX,source.ApproachY);await Frames(3);_input.Tap=true;await Frames(3);
                Check(Stock.Remaining[source.Site.Id]==0,"Gather actual finite Fuel for the rescue run");
            }
            await Generator();_world.PowerControls.ActivateRow(2);await Frames(2);Check(Power.Fuel==5,"Manual loading caps tank at five Fuel");
            _world.PowerControls.ActivateRow(0);await Frames(2);_world.ClosePower();await Beacon();
            await Joy(JoyButton.A);Check(Signal.Status==SignalStatus.Transmitting,"Assigned controller starts beacon transmission immediately");
            var position=_actor.Position;_input.Movement=Vector2.Right;long ticks=_world.Clock.ElapsedTicks;await Frames(10);_input.Movement=Vector2.Zero;
            Check(position==_actor.Position&&_world.Clock.ElapsedTicks>ticks,"Beacon context holds only the actor while simulation continues");
            _world.Clock.Advance(1200);await Frames(2);await Capture("transmitting");
            await MouseToggle();Check(Signal.Status==SignalStatus.Interrupted,"Mouse switches beacon off without changing other consumers");
            _world.Clock.Advance(300);await Frames(2);Check(Signal.TotalLossDays>.062m&&Signal.TotalLossDays<.064m,"Extended outage degrades only beyond grace");
            await Capture("degrading");
            await Joy(JoyButton.A);_world.Clock.Advance(30);await Frames(2);await Joy(JoyButton.A);
            decimal loss=Signal.TotalLossDays;_world.Clock.Advance(30);await Frames(2);
            Check(Signal.TotalLossDays>loss&&Signal.GraceRemainingTicks==0,"Brief power restoration cannot renew grace");
            await Joy(JoyButton.A);_world.Clock.Advance(150);await Frames(2);
            Check(Signal.EpisodeOutageTicks==0&&Signal.EpisodeLossDays==0,"Stable quarter-day transmission rearms an outage episode");
            _actor.Position=new Vector2(400,208);await Frames(3);
            Check(!_world.BeaconVisible&&!_actor.InventoryOpen,"Leaving beacon range closes its contextual interface");
            await Beacon();await Joy(JoyButton.B);Check(!_world.BeaconVisible,"Controller cancellation restores ordinary movement");
            // The remaining five-unit tank covers seven signal days and this small setback.
            _world.Clock.Advance(99999);await Frames(3);
            Check(Signal.Completed&&Signal.CompletionCount==1&&_world.VictoryScreen.Visible&&GetTree().Paused&&_world.Clock.Frozen,"Rescue opens one shared terminal screen and freezes the completed run");
            var saved=_world.Clock.SaveState();var fuel=Power.Fuel;var food=_world.HungerFor(1);position=_actor.Position;
            _input.Movement=Vector2.Right;_world.Clock.Advance(6000);await Frames(10);_input.Movement=Vector2.Zero;
            Check(_world.Clock.SaveState()==saved&&Power.Fuel==fuel&&_world.HungerFor(1)==food&&position==_actor.Position&&Signal.CompletionCount==1,"Terminal screen freezes time, Fuel, meals, movement and repeated victory");
            await Capture("rescued");
            if(OS.GetCmdlineUserArgs().Contains("--quit-victory")){_world.VictoryScreen.Activate(1);return;}
            _reset=true;Input.ParseInputEvent(new InputEventKey {PhysicalKeycode=Key.Enter,Pressed=true});
        }
        catch(Exception error){GD.PushError(error.ToString());GetTree().Paused=false;GetTree().Quit(1);}
    }
    private async Task Beacon()
    {
        var station=_world.PowerStations.Single(s=>s.Id=="beacon");_actor.Position=station.Position;await Frames(3);_input.Tap=true;await Frames(3);
        Check(_world.BeaconVisible&&!_world.PowerVisible,"Nearby radio interaction opens only the contextual beacon interface");
    }
    private async Task Generator()
    {
        var station=_world.PowerStations.Single(s=>s.Id=="generator");_actor.Position=station.Position;await Frames(3);_input.Tap=true;await Frames(3);
        Check(_world.PowerVisible,"Generator retains existing allocation interface");
    }
    private async Task KeyPress(Key key)
    {Input.ParseInputEvent(new InputEventKey {PhysicalKeycode=key,Pressed=true});await Frames(2);Input.ParseInputEvent(new InputEventKey {PhysicalKeycode=key,Pressed=false});await Frames(2);}
    private async Task Joy(JoyButton button)
    {Input.ParseInputEvent(new InputEventJoypadButton {Device=0,ButtonIndex=button,Pressed=true});await Frames(2);Input.ParseInputEvent(new InputEventJoypadButton {Device=0,ButtonIndex=button,Pressed=false});await Frames(2);}
    private async Task MouseToggle()
    {
        var button=_world.BeaconControls.GetChildren().OfType<Button>().First();
        var point=GetViewport().GetFinalTransform()*(button.GetGlobalTransformWithCanvas()*(button.Size/2));
        Input.ParseInputEvent(new InputEventMouseMotion {Position=point,GlobalPosition=point});await Frames(1);
        foreach(bool down in new[]{true,false}){Input.ParseInputEvent(new InputEventMouseButton {Position=point,GlobalPosition=point,ButtonIndex=MouseButton.Left,Pressed=down,ButtonMask=down?MouseButtonMask.Left:0});await Frames(2);}
    }
    private async Task Frames(int count)
    {for(int i=0;i<count;i++){await ToSignal(GetTree(),SceneTree.SignalName.PhysicsFrame);await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}}
    private async Task Capture(string name)
    {
        if(!OS.GetCmdlineUserArgs().Contains("--capture-rescue"))return;
        await Frames(4);RenderingServer.ForceDraw(false);
        string folder=ProjectSettings.GlobalizePath("user://rescue");DirAccess.MakeDirRecursiveAbsolute(folder);
        Check(GetViewport().GetTexture().GetImage().SavePng($"{folder}/{name}.png")==Error.Ok,"Capture "+name);
    }
    private static void Check(bool value,string message)
    {if(!value)throw new InvalidOperationException(message);GD.Print("PASS: "+message);}
}
