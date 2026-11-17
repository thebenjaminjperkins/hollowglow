using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Godot;
using Hollowlight.Core;
using Hollowlight.Presentation;
using Hollowlight.World;

namespace Hollowlight.Tools;

public partial class GarageSmoke : Node
{
    private readonly SmokeInput _input=new();
    private ThresholdWorld _world=null!;
    private PlayerCharacter _actor=null!;
    private GaragePlan Garage=>_world.Garage!;
    private MountainRegion Region=>_world.Region!;
    private SuppliesSnapshot Stock=>_world.SuppliesFor(1);
    private readonly Dictionary<string,double> _times=new();
    private string _phase="setup";
    private static bool _reset;
    private bool CaptureMode=>OS.GetCmdlineUserArgs().Contains("--capture-garage");
    private bool Preserve=>OS.GetCmdlineUserArgs().Contains("--preserve-van");
    private Vector2 Chest=>_world.StoragePosition+new Vector2(0,18);
    public override async void _Ready()
    {
        try
        {
            var room=GD.Load<PackedScene>(ProjectSettings.GetSetting("application/run/main_scene").AsString()).Instantiate<SanctuaryRoom>();
            room.LayoutProfile="garage-expedition";room.InputOverride=_input;AddChild(room);_world=room.World;_actor=_world.GetNode<PlayerCharacter>("Player");await Frames(3);
            if(_reset)
            {
                Check(_world.ObjectState("garage-van").Stage==0&&_world.ObjectState("garage-van").RestorationAvailable&&
                    _world.ObjectState("garage-rack").Remaining==1&&Garage.Objects.All(o=>!_world.ObjectState(o.Id).Opened),"New Run restores objects, fixture tool and restoration opportunity");
                Check(!_world.Discovery!.Explored(Garage.Objects.Single(o=>o.Id=="garage-office").Foot),"Reset clears shared discovery");
                GD.Print("PASS: garage live navigation, searching/partial transfers, tool/van controls, cancellation, fog/memory, quantities, survival continuity, measured trip and reset");GetTree().Quit();return;
            }
            Check(_world.Signal!.RequiredDays==7&&_world.GrowingPlots.Count==4&&Stock.Personal.Capacity==6,"Playable economy stays seven-day/four-plot/six-slot");
            Check(_world.BeginGarageWork("garage-compressor").Status==ExplorationStatus.AccessDenied,"Remote dismantling rejected");
            if(CaptureMode){await Captures();GetTree().Quit();return;}
            await Walk(Chest);_times.Clear();_phase="outbound_walking";
            await Walk(Point(Region.Anchor("insidePortal")),true);Check(_world.OnSurface,"Actual walk through threshold");
            await Walk(Point(Garage.Entry));
            var office=Garage.Objects.Single(o=>o.Id=="garage-office");
            Check(!_world.Discovery!.Visible(1,office.Foot),"Building walls conceal office on approach");
            _phase="site_walking";await Visit("garage-van");_phase="inspection_inventory";await Interact();
            Check(_world.VehicleControls.Visible&&_world.VehicleControls.FocusedRow==0,"Vehicle inspection defaults to preservation");
            _world.VehicleControls.ActivateRow(0);await Frames(2);
            Check(_world.ObjectState("garage-van").RestorationAvailable,"Preserve leaves vehicle intact");
            foreach(string id in new[]{"garage-office","garage-parts","garage-crate","garage-fuel"})
            {
                _phase="site_walking";await Visit(id);_phase="inspection_inventory";await Interact();
                Check(_world.LootControls.Visible,"Search opens visual slot interface: "+id);
                if(id=="garage-office")
                {
                    int before=_world.ObjectState(id).Remaining;
                    _world.LootControls.BeginSplit(new(InventoryKind.Storage,0));_world.LootControls.ConfirmSplit();
                    int slot=Enumerable.Range(0,Stock.Personal.Capacity).First(i=>Stock.Personal.Slots[i]==null);
                    _world.LootControls.Activate(new(InventoryKind.Personal,slot));await Frames(2);
                    Check(_world.ObjectState(id).Remaining>0&&_world.ObjectState(id).Remaining<before,"Partial loot collection keeps source remainder");
                }
                if(id=="garage-parts")
                {
                    _world.LootControls.FocusSlot(new(InventoryKind.Storage,0));
                    _world.LootControls._UnhandledInput(new InputEventJoypadButton{ButtonIndex=JoyButton.X,Pressed=true,Device=_actor.ControllerDevice});
                }
                else if(id=="garage-crate")
                {
                    _world.LootControls.FocusSlot(new(InventoryKind.Storage,0));
                    _world.LootControls._UnhandledInput(new InputEventKey{PhysicalKeycode=Key.Q,Pressed=true});
                }
                else _world.LootControls.StoreAll();
                await Frames(2);Check(_world.ObjectState(id).Remaining==0,"Take all / keyboard / controller quick collection keeps finite count exact");
                _world.CloseGarage();await Frames(1);
            }
            _phase="site_walking";await Visit("garage-compressor");_phase="inspection_inventory";
            Check(_world.BeginGarageWork("garage-compressor").Status==ExplorationStatus.NoTool,"Personal tool gate enforced live");
            _phase="site_walking";await Visit("garage-rack");_phase="inspection_inventory";await Interact();
            Check(Stock.Personal.Count(GaragePlan.Tool)==1&&_world.ObjectState("garage-rack").Remaining==0,"Tool pickup occupies actual inventory slot");
            _phase="site_walking";await Visit("garage-compressor");await Work(false);
            Check(_world.ObjectState("garage-compressor").Stage==1,"Machine changes to stripped state");
            _phase="site_walking";await Visit("garage-van");
            if(!Preserve)
            {
                _phase="inspection_inventory";await Interact();_input.Hold=true;_world.VehicleControls.ActivateRow(1);_input.Tap=true;await Frames(2);
                Check(!_world.Dismantling,"Controller confirmation cannot also start destructive work; release is required");
                _input.Hold=false;await Frames(2);
                _phase="cancelled_active_work";_input.Hold=true;_input.Tap=true;await Frames(15);_input.Hold=false;await Frames(2);
                Check(!_world.Dismantling&&_world.ObjectState("garage-van").RestorationAvailable,"Interrupting first removal preserves van");
                await ArmVan();await Work(false);
                Check(_world.ObjectState("garage-van").Stage==1&&!_world.ObjectState("garage-van").RestorationAvailable,"First removal irreversibly ends restoration opportunity");
                await Work(false);Check(_world.ObjectState("garage-van").Stage==2,"Final stripped van state visible and persistent");
            }
            Check(_world.Discovery!.Explored(office.Foot)&&!_world.Discovery.Visible(1,office.Foot),"Explored office remains mapped outside current visibility");
            Check(!_world.GetNode<ThresholdMap>("Map").GetChildren().OfType<GarageObjectArt>().Single(a=>a.Object.Id==office.Id).Visible,"Unseen dynamic object state is not rendered through mapped geography");
            _phase="return_walking";await Walk(Point(Region.Entrance.Cell),true);await Walk(Chest);
            _phase="deposit_inventory";await Interact();Check(_world.ChestAccessible,"Original communal chest still works");
            int scrap=Stock.Personal.Count("scrap"),food=Stock.Personal.Count("food")-2,fuel=Stock.Personal.Count("fuel");
            Check(scrap==(Preserve?10:14),"Exactly allocated Scrap; preservation defers four");
            _world.TransferSupplies(_actor,"scrap",scrap,false,Stock.Version);
            _world.TransferSupplies(_actor,"food",food,false,Stock.Version);
            _world.TransferSupplies(_actor,"fuel",fuel,false,Stock.Version);
            _world.TransferSupplies(_actor,GaragePlan.Tool,1,false,Stock.Version);await Frames(2);_world.CloseInventory();
            Check(Stock.Storage.Count("scrap")==scrap&&Stock.Storage.Count("fuel")==fuel+2,"Deposited counts preserve all starting and acquired resources");
            Check(_world.ObjectState("garage-van").Stage==(Preserve?0:2)&&_world.Discovery.Explored(office.Foot)&&!_world.Discovery.Visible(1,office.Foot),"Threshold return preserves depletion, vehicle state and exploration");
            string folder=ProjectSettings.GlobalizePath("user://garage");DirAccess.MakeDirRecursiveAbsolute(folder);
            System.IO.File.WriteAllText(folder+$"/travel-{Region.Seed}-{(Preserve?"preserve":"dismantle")}.json",JsonSerializer.Serialize(new{Seed=Region.Seed,Branch=Preserve?"preserve":"dismantle",Seconds=_times,
                TotalSeconds=_times.Values.Sum(),Scrap=scrap,Food=food,Fuel=fuel,Measurement="Actual Godot physics/input; efficient known route; no deliberate inspection delays or teleports. Includes one cancelled first removal in dismantle branch."},new JsonSerializerOptions{WriteIndented=true})+"\n");
            GD.Print("MEASURED garage: "+JsonSerializer.Serialize(_times)+$"; total {_times.Values.Sum():0.00}s");
            _reset=true;_world._UnhandledKeyInput(new InputEventKey{PhysicalKeycode=Key.R,Pressed=true});
        }
        catch(Exception e){GD.PushError(e.ToString());GetTree().Quit(1);}
    }
    private static Vector2 Point(GridPoint p)=>new(p.WorldX,p.WorldY);
    private async Task Visit(string id)=>await Walk(Point(Garage.Objects.Single(o=>o.Id==id).Approach));
    private async Task Interact(){_input.Tap=true;await Frames(3);}
    private async Task ArmVan(){_phase="inspection_inventory";await Interact();_world.VehicleControls.ActivateRow(1);await Frames(2);}
    private async Task Work(bool capture)
    {
        _phase="dismantling";_input.Hold=true;_input.Tap=true;await Frames(3);
        Check(_world.Dismantling,"Held action begins");
        for(int i=0;i<120&&_world.Dismantling;i++)await Frames(1);
        Check(!_world.Dismantling,"Responsive dismantling completes");_input.Hold=false;await Frames(2);
    }
    private async Task Walk(Vector2 target,bool portal=false)
    {
        bool zone=_world.OnSurface;var path=Region.Path(MountainRegion.CellAt(_actor.Position.X,_actor.Position.Y),MountainRegion.CellAt(target.X,target.Y));
        if(path.Count==0)throw new InvalidOperationException($"No path {_actor.Position} -> {target}");
        foreach(var cell in path)if(!await Move(Point(cell),zone,portal))return;
        await Move(target,zone,portal);
        if(portal){for(int i=0;i<60&&_world.OnSurface==zone;i++)await Frames(1);Check(_world.OnSurface!=zone,"Threshold crosses without reload");}
    }
    private async Task<bool> Move(Vector2 target,bool zone,bool portal)
    {
        for(int i=0;i<180;i++)
        {
            if(portal&&_world.OnSurface!=zone){_input.Movement=Vector2.Zero;return false;}
            var difference=target-_actor.Position;if(difference.Length()<1.5f){_input.Movement=Vector2.Zero;return true;}
            _input.Movement=difference.Normalized();await Frames(1);
        }
        throw new InvalidOperationException($"Physics stuck {_actor.Position} -> {target}");
    }
    private async Task Frames(int n)
    {
        double before=_world.Clock.Snapshot().TotalSeconds;
        for(int i=0;i<n;i++){await ToSignal(GetTree(),SceneTree.SignalName.PhysicsFrame);await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
        _times[_phase]=_times.GetValueOrDefault(_phase)+_world.Clock.Snapshot().TotalSeconds-before;
    }
    private async Task Captures()
    {
        // Explicit review positioning only; measured headless trips above never teleport.
        _world._UnhandledKeyInput(new InputEventKey{PhysicalKeycode=Key.F3,Pressed=true});await Frames(3);
        _actor.Position=Point(Garage.Entry);await Capture("forecourt");
        foreach(string id in new[]{"garage-parts","garage-crate","garage-fuel","garage-rack","garage-office"})
        { _actor.Position=Point(Garage.Objects.Single(o=>o.Id==id).Approach);await Frames(3); }
        _actor.Position=Point(Garage.Objects.Single(o=>o.Id=="garage-office").Approach);await Frames(3);await Interact();await Capture("office-search");_world.CloseGarage();
        _actor.Position=Point(Garage.Objects.Single(o=>o.Id=="garage-rack").Approach);await Frames(3);await Interact();await Capture("workshop");
        _actor.Position=Point(Garage.Objects.Single(o=>o.Id=="garage-van").Approach);await Frames(3);await VanCapture("van-intact");
        await Interact();await Capture("van-inspection");_world.VehicleControls.ActivateRow(0);await Frames(2);
        await ArmVan();await Work(true);await VanCapture("van-partial");await Work(true);await VanCapture("van-stripped");
        _actor.Position=Point(Garage.Entry);await Capture("explored-memory");
        var camera=_actor.GetNode<Camera2D>("Camera2D");camera.Zoom=Vector2.One*1.25f;_actor.Position=Point(new(Garage.Origin.X+12,Garage.Origin.Y+9));
        await Capture("overview-review-zoom");camera.Zoom=Vector2.One*2;
    }
    private async Task Capture(string name)
    {
        await Frames(4);RenderingServer.ForceDraw(false);var folder=ProjectSettings.GlobalizePath("user://garage");DirAccess.MakeDirRecursiveAbsolute(folder);
        Check(GetViewport().GetTexture().GetImage().SavePng(folder+"/"+name+".png")==Error.Ok,"Capture "+name);
    }
    private async Task VanCapture(string name)
    {
        var saved=_actor.Position;
        _actor.Position=Point(Garage.Objects.Single(o=>o.Id=="garage-van").Foot)+new Vector2(55,-30);
        await Capture(name);_actor.Position=saved;await Frames(3);
    }
    private static void Check(bool v,string message){if(!v)throw new InvalidOperationException(message);GD.Print("PASS: "+message);}
}
