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

public partial class RegionSmoke : Node
{
    private readonly SmokeInput _input=new();
    private ThresholdWorld _world=null!;
    private PlayerCharacter _actor=null!;
    private MountainRegion Region=>_world.Region??throw new InvalidOperationException("Region review requires mountain-region profile");
    private SuppliesSnapshot Stock=>_world.SuppliesFor(_actor.PlayerId);
    private static bool _reset;
    private readonly List<object> _trips=new(),_chores=new();
    public override async void _Ready()
    {
        try
        {
            var room=GD.Load<PackedScene>(ProjectSettings.GetSetting("application/run/main_scene").AsString()).Instantiate<SanctuaryRoom>();
            room.LayoutProfile="mountain-region";room.InputOverride=_input;AddChild(room);_world=room.World;_actor=_world.GetNode<PlayerCharacter>("Player");await Frames(3);
            Check(Region.Definition.SurfaceWidth==192&&Region.Definition.SurfaceHeight==144&&_world.GrowingPlots.Count==4,"Selectable region has approved dimensions and four original plots");
            if(_reset)
            {
                Check(!_world.OnSurface&&Stock.Personal.Capacity==6&&_world.Clock.Snapshot().DayNumber==1&&Stock.Remaining.Values.Sum()==Region.Sources.Sum(s=>s.Site.Quantity),"Reset restores same-seed geography, original finite sources and shared time");
                GD.Print("PASS: region physics travel, portal continuity, chores, all finite sources, farming/economy conservation and reset");GetTree().Quit();return;
            }
            if(OS.GetCmdlineUserArgs().Contains("--capture-region")){await ReviewCaptures();GetTree().Quit();return;}
            await Walk(ChestApproach());await OpenChest();_world.TransferSupplies(_actor,"potato_seed",1,true,Stock.Version);_world.CloseInventory();
            var plot=_world.GrowingPlots[0];await Walk(plot.Position+new Vector2(0,12));_input.Tap=true;await Frames(3);
            var planted=_world.Plot(plot.PlotId);Check(planted.Stage==CropStage.Seedling,"Existing planting works at the relocated garden");
            foreach(string id in new[]{"workbench","generator"})await MeasureChore(id,_world.PowerStations.Single(s=>s.Id==id).Position);
            await MeasureChore("garden",plot.Position+new Vector2(0,12));
            foreach(var source in Region.Sources)await Expedition(source);
            Check(Region.Sources.All(s=>Stock.Remaining[s.Site.Id]==0),"Repeated threshold crossings never respawn collected resources");
            Check(_world.Plot(plot.PlotId).PlantedTicks==planted.PlantedTicks&&_world.Plot(plot.PlotId).Progress>planted.Progress,"Farm grows throughout physical exploration and transitions");
            long consumed=_world.HungerFor(_actor.PlayerId)!.Consumed;
            Check(Stock.Personal.Count("food")+Stock.Storage.Count("food")+consumed==2+Region.Sources.Where(s=>s.Site.ResourceId=="food").Sum(s=>s.Site.Quantity),"Gathered/consumed/stored Food conserves the original budget");
            Check(Stock.Storage.Count("scrap")==26&&Stock.Storage.Count("organic")==8&&Stock.Storage.Count("fuel")==2+Region.Sources.Where(s=>s.Site.ResourceId=="fuel").Sum(s=>s.Site.Quantity),"Material and Fuel totals unchanged after actual collection trips");
            string folder=ProjectSettings.GlobalizePath("user://region");DirAccess.MakeDirRecursiveAbsolute(folder);
            System.IO.File.WriteAllText(folder+"/travel-"+Region.Seed+".json",JsonSerializer.Serialize(new {Seed=Region.Seed,DaylightSeconds=360,
                Measurement="Godot physics and authoritative elapsed time; each destination and deposit includes two seconds inspection/menu time; immediate E gathering.",Chores=_chores,Trips=_trips},new JsonSerializerOptions {WriteIndented=true})+"\n");
            _reset=true;_world._UnhandledKeyInput(new InputEventKey {PhysicalKeycode=Key.R,Pressed=true});
        }
        catch(Exception error){GD.PushError(error.ToString());GetTree().Quit(1);}
    }
    private Vector2 ChestApproach()=>_world.StoragePosition+new Vector2(0,18);
    private double Seconds=>_world.Clock.Snapshot().TotalSeconds;
    private async Task MeasureChore(string id,Vector2 destination)
    {
        await Walk(ChestApproach());double start=Seconds;await Walk(destination);double walk=Seconds-start;
        _input.Tap=true;await Frames(123);_world.ClosePower();_world.CloseInventory();await Walk(ChestApproach());await OpenChest();await Frames(120);_world.CloseInventory();
        double total=Seconds-start;_chores.Add(new {Activity=id,OneWayWalkingSeconds=walk,RoundTripWithInteractionsSeconds=total});
        Check(walk<=6,"Convenient storage-to-"+id+" walking route: "+walk.ToString("0.00")+"s");
        GD.Print($"CHORE {id}: {walk:0.00}s one-way, {total:0.00}s round trip incl. interactions");
    }
    private async Task Expedition(GeneratedSourceSite source)
    {
        await Walk(ChestApproach());double start=Seconds;Guid session=Stock.Version.Session;
        await Walk(new Vector2(Region.Anchor("insidePortal").WorldX,Region.Anchor("insidePortal").WorldY),true);
        Check(_world.OnSurface&&Stock.Version.Session==session,"Exit preserves shared session: "+source.Site.Id);
        await Walk(new Vector2(source.ApproachX,source.ApproachY));double outbound=Seconds-start;
        int before=Stock.Remaining[source.Site.Id];_input.Tap=true;await Frames(123);
        int gathered=before-Stock.Remaining[source.Site.Id];Check(gathered==source.Site.Quantity,"Physical approach and E gathering: "+source.Site.Id);
        await Walk(new Vector2(Region.Entrance.Cell.WorldX,Region.Entrance.Cell.WorldY),true);
        Check(!_world.OnSurface&&Stock.Version.Session==session,"Return preserves depletion, inventory and time");
        await Walk(ChestApproach());await OpenChest();await Frames(120);
        Check(_world.TransferSupplies(_actor,source.Site.ResourceId,gathered,false,Stock.Version)?.Amount==gathered,"Explicit expedition deposit conserves gathered stock");
        int refill=Math.Max(0,2-Stock.Personal.Count("food"));if(refill>0)_world.TransferSupplies(_actor,"food",refill,true,Stock.Version);
        _world.CloseInventory();double elapsed=Seconds-start;
        _trips.Add(new {Source=source.Site.Id,Location=Region.Definition.ResourceLocations[source.Site.Id],Resource=source.Site.ResourceId,
            Quantity=gathered,OutboundFromChestSeconds=outbound,RoundTripWithInteractionsSeconds=elapsed,DaylightFraction=elapsed/360});
        Check(elapsed<360,"Expedition fits daylight: "+source.Site.Id+" "+elapsed.ToString("0.0")+"s");
        GD.Print($"EXPEDITION {source.Site.Id}: outbound {outbound:0.00}s, round trip + gather/deposit {elapsed:0.00}s / 360s daylight");
    }
    private async Task OpenChest(){_input.Tap=true;await Frames(3);Check(_world.ChestAccessible,"Relocated communal chest opens normally");}
    private async Task Walk(Vector2 target,bool portal=false)
    {
        bool initialZone=_world.OnSurface;var path=Region.Path(MountainRegion.CellAt(_actor.Position.X,_actor.Position.Y),MountainRegion.CellAt(target.X,target.Y));
        if(path.Count==0)throw new InvalidOperationException($"No path {_actor.Position} to {target}");
        foreach(var cell in path)
            if(!await Move(new Vector2(cell.WorldX,cell.WorldY),initialZone,portal))return;
        await Move(target,initialZone,portal);
        if(portal)
        {
            for(int i=0;i<60&&_world.OnSurface==initialZone;i++)await Frames(1);
            Check(_world.OnSurface!=initialZone,"Walk-through threshold transitions without pausing simulation");
        }
    }
    private async Task<bool> Move(Vector2 target,bool zone,bool portal)
    {
        for(int frame=0;frame<180;frame++)
        {
            if(portal&&_world.OnSurface!=zone){_input.Movement=Vector2.Zero;return false;}
            var distance=target-_actor.Position;
            if(distance.Length()<1.5f){_input.Movement=Vector2.Zero;return true;}
            _input.Movement=distance.Normalized();await Frames(1);
        }
        throw new InvalidOperationException($"Physics navigation stuck at {_actor.Position}, target {target}");
    }
    private async Task ReviewCaptures()
    {
        // Review-only camera positioning. Expedition measurement above uses actual movement.
        _actor.Position=ChestApproach();await Frames(3);await OpenChest();_world.TransferSupplies(_actor,"fuel",2,true,Stock.Version);_world.CloseInventory();
        var generator=_world.PowerStations.Single(s=>s.Id=="generator");_actor.Position=generator.Position;await Frames(3);_input.Tap=true;await Frames(3);
        _world.PowerControls.ActivateRow(2);await Frames(2);_world.PowerControls.ActivateRow(0);await Frames(2);_world.PowerControls.ActivateRow(5);await Frames(2);_world.ClosePower();
        foreach(string area in new[]{"common","garden","vestibule"})
        {
            var room=Region.Definition.Chambers.Single(c=>c.Id==area);_actor.Position=new Vector2(room.X*16+8,room.Y*16+24);await Capture(area);
        }
        await Overview(false);
        _world._UnhandledKeyInput(new InputEventKey {PhysicalKeycode=Key.F3,Pressed=true});await Frames(3);
        foreach(string id in new[]{"entrance","hut","bridge","hamlet","yard","hollow"})
        {var node=Region.Landmarks.Single(n=>n.Id==id);_actor.Position=new Vector2(node.Cell.WorldX,node.Cell.WorldY+48);await Capture(id);}
        await Overview(true);
    }
    private async Task Overview(bool surface)
    {
        var window=GetWindow();window.ContentScaleSize=new Vector2I(768,576);window.Size=new Vector2I(1536,1152);
        var camera=_actor.GetNode<Camera2D>("Camera2D");camera.TopLevel=true;camera.Zoom=Vector2.One*(surface?.25f:.75f);
        camera.LimitLeft=-10000;camera.LimitRight=10000;camera.LimitTop=-10000;camera.LimitBottom=10000;
        camera.GlobalPosition=surface?new Vector2((Region.Definition.SurfaceOffsetX+Region.Definition.SurfaceWidth/2f)*16,Region.Definition.SurfaceHeight*8):new Vector2(Region.Definition.SanctuaryWidth*8,Region.Definition.SanctuaryHeight*8);
        _world.GetNode<ThresholdHud>("HUD").Hide();await Capture(surface?"surface-overview":"sanctuary-overview");_world.GetNode<ThresholdHud>("HUD").Show();
        camera.TopLevel=false;camera.Position=Vector2.Zero;camera.Zoom=Vector2.One*2;SanctuaryRoom.ConfigureWindow(window);
    }
    private async Task Capture(string name)
    {
        await Frames(4);RenderingServer.ForceDraw(false);string folder=ProjectSettings.GlobalizePath("user://region");DirAccess.MakeDirRecursiveAbsolute(folder);
        Check(GetViewport().GetTexture().GetImage().SavePng(folder+"/"+name+".png")==Error.Ok,"Capture "+name);
    }
    private async Task Frames(int n){for(int i=0;i<n;i++){await ToSignal(GetTree(),SceneTree.SignalName.PhysicsFrame);await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}}
    private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);GD.Print("PASS: "+message);}
}
