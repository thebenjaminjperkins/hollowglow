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

// Review harness only: unchanged hall/world, real input/physics and authority operations.
public partial class MaterialRouteReview : Node
{
    private readonly SmokeInput _input=new();
    private ThresholdWorld _world=null!;
    private PlayerCharacter _actor=null!;
    private MountainRegion Region=>_world.Region!;
    private SuppliesSnapshot Stock=>_world.SuppliesFor(1);
    private readonly Dictionary<string,double> _seconds=new();
    private readonly List<object> _legs=new();
    private string? _phase;
    private int _peakSlots;
    private Vector2 Chest=>_world.StoragePosition+new Vector2(0,18);
    public override async void _Ready()
    {
        try
        {
            string route=OS.GetCmdlineUserArgs().FirstOrDefault(a=>a.StartsWith("--material-route="))?.Split('=')[1]??"garage-detours";
            Check(route is "organic" or "scrap" or "garage-detours","Known itinerary");
            var room=GD.Load<PackedScene>("res://scenes/SanctuaryRoom.tscn").Instantiate<SanctuaryRoom>();
            room.LayoutProfile="sanctuary-hall-review";room.InputOverride=_input;AddChild(room);
            _world=room.World;_actor=_world.GetNode<PlayerCharacter>("Player");await Frames(4);await Walk(Chest);
            Check(Stock.Personal.Capacity==6&&_world.GrowingPlots.Count==4&&_world.Signal!.RequiredDays==7,"Unchanged inventory, plots and normal signal target");
            double start=_world.Clock.Snapshot().TotalSeconds;
            await Leg("hall_to_surface",()=>Walk(Point(Region.Anchor("insidePortal")),true));
            if(route=="garage-detours")
            {
                await Leg("surface_to_garage",()=>Walk(Point(_world.Garage!.Entry)));
                _phase="garage_search_and_work";
                await Visit("garage-van");await Interact();_world.VehicleControls.ActivateRow(0);await Frames(2);
                foreach(string id in new[]{"garage-office","garage-parts","garage-crate","garage-fuel"})
                {
                    await Visit(id);await Interact();Check(_world.LootControls.Visible,"Container access "+id);
                    _world.LootControls.StoreAll();await Frames(2);_world.CloseGarage();
                    Check(_world.ObjectState(id).Remaining==0,"Finite container collected "+id);
                }
                await Visit("garage-rack");await Interact();Check(Stock.Personal.Count(GaragePlan.Tool)==1,"Tool occupies slot");
                await Visit("garage-compressor");_input.Hold=true;_input.Tap=true;await Frames(3);
                Check(_world.Dismantling,"Real held dismantling begins");
                for(int i=0;i<120&&_world.Dismantling;i++)await Frames(1);
                _input.Hold=false;await Frames(2);Check(_world.ObjectState("garage-compressor").Remaining==0,"Compressor allocation recovered");
                Check(Stock.Personal.Count("scrap")==10&&_world.ObjectState("garage-van").RestorationAvailable,"Van preserved; exactly ten garage Scrap");
                await Gather("tool-case","garage_to_organic");await Gather("radio-shell","organic_to_scrap");
            }
            else await Gather(route=="organic"?"tool-case":"radio-shell","surface_to_source");
            int scrap=Stock.Personal.Count("scrap"),organic=Stock.Personal.Count("organic");
            int hutRemaining=Stock.Remaining["radio-shell"];
            await Leg("return_to_surface_threshold",()=>Walk(Point(Region.Entrance.Cell),true));
            await Leg("return_to_hall_storage",()=>Walk(Chest));
            _phase="deposit";await Interact();Check(_world.ChestAccessible,"Explicit chest access");
            var deposit=_world.ApplyInventoryCommand(_actor,new(SlotAction.DepositAll,new(InventoryKind.Personal,0),new(InventoryKind.Storage,0),0,Stock.Version));
            Check(deposit.Status==SupplyStatus.Applied,"Explicit deposit commits");await Frames(2);_world.CloseInventory();
            double expeditionSeconds=_world.Clock.Snapshot().TotalSeconds-start;
            double? installationSeconds=null;
            if(route=="garage-detours")
            {
                Check(organic==8&&scrap==20&&hutRemaining==2,"Six slots force partial Scrap collection; remainder retained");
                _phase="withdraw_and_power";await Interact();
                foreach(var pair in new[]{("scrap",12),("organic",4),("fuel",1)})
                    Check(_world.TransferSupplies(_actor,pair.Item1,pair.Item2,true,Stock.Version)?.Status==SupplyStatus.Applied,"Personal ingredient withdrawal");
                _world.CloseInventory();await Walk(_world.PowerStations.Single(s=>s.Id=="generator").Position);await Interact();
                _world.PowerControls.ActivateRow(1);await Frames(2);_world.PowerControls.ActivateRow(0);await Frames(2);
                _world.PowerControls.ActivateRow(3);await Frames(2);_world.PowerControls.ActivateRow(4);await Frames(2);_world.ClosePower();
                _phase="craft_and_install";await Walk(_world.PowerStations.Single(s=>s.Id=="workbench").Position);await Interact();
                Check(_world.CraftAtWorkbench(_actor,new("growing_bed",Stock.Version,_world.Electrical!.Version)).Status==CraftStatus.Applied,"Current powered bed recipe commits");
                _world.ClosePower();await Walk(_world.GardenSocket+new Vector2(0,12));await Interact();
                Check(_world.GrowingPlots.Count==5&&Stock.Storage.Count("scrap")==8&&Stock.Storage.Count("organic")==4,"Fifth plot installed; exact material leftovers");
                Check(hutRemaining+4+8+12==26,"Scrap conserved: sources, stored and consumed");
                installationSeconds=_world.Clock.Snapshot().TotalSeconds-start;
            }
            string folder=ProjectSettings.GlobalizePath("user://material-routes");DirAccess.MakeDirRecursiveAbsolute(folder);
            System.IO.File.WriteAllText(folder+$"/{Region.Seed}-{route}.json",JsonSerializer.Serialize(new{
                Seed=Region.Seed,Route=route,ExpeditionSeconds=expeditionSeconds,ThroughInstalledFifthPlotSeconds=installationSeconds,
                PhaseSeconds=_seconds,Legs=_legs,RecoveredScrap=scrap,RecoveredOrganic=organic,HutScrapRemaining=hutRemaining,PeakUsedSlots=_peakSlots,
                VanPreserved=_world.ObjectState("garage-van").RestorationAvailable,
                Note="Real input/physics/clock, current normal timing, no teleports, time skips, free supplies or altered generation. Efficient known route and test-speed UI; not two-human/combat/blueprint assembly timing."},new JsonSerializerOptions{WriteIndented=true})+"\n");
            GD.Print($"PASS: measured {Region.Seed} {route}: expedition {expeditionSeconds:0.00}s; installed {installationSeconds:0.00}s; peak slots {_peakSlots}");
            GetTree().Quit();
        }
        catch(Exception e){GD.PushError(e.ToString());GetTree().Quit(1);}
    }
    private async Task Gather(string id,string leg)
    {
        var source=Region.Sources.Single(s=>s.Site.Id==id);
        await Leg(leg,()=>Walk(new((float)source.ApproachX,(float)source.ApproachY)));
        _phase="source_gathering";await Frames(2);
        Check(_world.GatherSupplies(_actor,id,Stock.Version).Status==SupplyStatus.Applied,"Current finite source gather "+id);await Frames(2);
    }
    private async Task Leg(string name,Func<Task> action)
    {
        _phase=name;double before=_world.Clock.Snapshot().TotalSeconds;await action();
        _legs.Add(new{Name=name,Seconds=_world.Clock.Snapshot().TotalSeconds-before});
    }
    private async Task Visit(string id)=>await Walk(Point(_world.Garage!.Objects.Single(o=>o.Id==id).Approach));
    private async Task Interact(){_input.Tap=true;await Frames(3);}
    private static Vector2 Point(GridPoint p)=>new(p.WorldX,p.WorldY);
    private async Task Walk(Vector2 target,bool portal=false)
    {
        bool zone=_world.OnSurface;
        var path=Region.Path(MountainRegion.CellAt(_actor.Position.X,_actor.Position.Y),MountainRegion.CellAt(target.X,target.Y));
        Check(path.Count>0,"Navigable route");
        foreach(var p in path)if(!await Move(Point(p),zone,portal))return;
        await Move(target,zone,portal);
        if(portal){for(int i=0;i<60&&_world.OnSurface==zone;i++)await Frames(1);Check(_world.OnSurface!=zone,"Physical threshold crossed");}
    }
    private async Task<bool> Move(Vector2 target,bool zone,bool portal)
    {
        for(int i=0;i<180;i++)
        {
            if(portal&&zone!=_world.OnSurface){_input.Movement=Vector2.Zero;return false;}
            var d=target-_actor.Position;if(d.Length()<1.5f){_input.Movement=Vector2.Zero;return true;}
            _input.Movement=d.Normalized();await Frames(1);
        }
        throw new InvalidOperationException("Physics blocked en route to "+target);
    }
    private async Task Frames(int n)
    {
        double before=_world.Clock.Snapshot().TotalSeconds;
        for(int i=0;i<n;i++){await ToSignal(GetTree(),SceneTree.SignalName.PhysicsFrame);await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);_peakSlots=Math.Max(_peakSlots,Stock.Personal.UsedSlots);}
        if(_phase!=null)_seconds[_phase]=_seconds.GetValueOrDefault(_phase)+_world.Clock.Snapshot().TotalSeconds-before;
    }
    private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
}
