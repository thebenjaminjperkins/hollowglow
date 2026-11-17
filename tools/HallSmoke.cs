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

public partial class HallSmoke : Node
{
    private readonly SmokeInput _input=new();
    private ThresholdWorld _world=null!;
    private PlayerCharacter _actor=null!;
    private SanctuaryPlan Plan=>_world.Region!.Definition.Hall!;
    private MountainRegion Region=>_world.Region!;
    private SuppliesSnapshot Stock=>_world.SuppliesFor(1);
    private readonly List<object> _timings=new();
    private readonly List<object> _hudCases=new();
    private float _closestActors=float.MaxValue;
    private bool Capture=>OS.GetCmdlineUserArgs().Contains("--capture-hall");
    private string Folder=>ProjectSettings.GlobalizePath("user://hall-review");
    private static bool _reset;
    private Vector2 Chest=>_world.StoragePosition+new Vector2(0,18);
    public override async void _Ready()
    {
        try
        {
            var room=GD.Load<PackedScene>("res://scenes/SanctuaryRoom.tscn").Instantiate<SanctuaryRoom>();room.LayoutProfile="sanctuary-hall-review";room.InputOverride=_input;AddChild(room);
            _world=room.World;_actor=_world.GetNode<PlayerCharacter>("Player");await Frames(4);
            if(_reset){Check(_world.GrowingPlots.Count==4&&Stock.Personal.Capacity==6&&Stock.Storage.Count("potato_seed")==2&&_world.Clock.Snapshot().DayNumber==1,"Hall reset restores original inventory, four empty plots, seeds and clock");GetTree().Quit();return;}
            Check(_world.GrowingPlots.Count==4&&_world.Electrical!.Consumers.Count==3,"Four starting plots and original power consumers; no future infrastructure installed");
            foreach(var sprite in WorldVisuals.Descendants(_world).OfType<Sprite2D>())
                if(sprite.Texture is AtlasTexture slice&&slice.Atlas.ResourcePath.EndsWith("fuel-cabinet.png"))
                {var ink=slice.GetImage().GetUsedRect();Check(ink.Size.X>0&&ink.Size.Y>0,"Structural iron-frame sample contains visible pixels");}
            await Walk(Chest);await Interact();Check(_world.ChestAccessible,"Storage approach and interaction work");
            _world.TransferSupplies(_actor,"potato_seed",1,true,Stock.Version);_world.CloseInventory();
            var plot=_world.GrowingPlots[0];await Walk(plot.Position+new Vector2(0,12));await Interact();
            Check(_world.Plot(plot.PlotId).Stage==CropStage.Seedling,"Existing planting consumes a withdrawn seed in the hall");
            var points=new Dictionary<string,Vector2>{{"storage",Chest},{"garden",plot.Position+new Vector2(0,12)},
                {"workshop",_world.PowerStations.Single(s=>s.Id=="workbench").Position},{"power",_world.PowerStations.Single(s=>s.Id=="generator").Position},
                {"radio",_world.PowerStations.Single(s=>s.Id=="beacon").Position},{"entrance",Point(Region.Anchor("insideLanding"))}};
            foreach(var from in points)foreach(var to in points)
            {
                if(from.Key==to.Key)continue;await Walk(from.Value);double before=_world.Clock.Snapshot().TotalSeconds;await Walk(to.Value);
                double seconds=_world.Clock.Snapshot().TotalSeconds-before;_timings.Add(new{From=from.Key,To=to.Key,Seconds=seconds});
            }
            Check(_world.Plot(plot.PlotId).Progress>0,"Crop grows during actual walking and exploration");
            foreach(var station in _world.PowerStations.Where(s=>s.Id is "generator" or "workbench" or "beacon"))
            {await Walk(station.Position);await Interact();Check(_world.PowerVisible||_world.BeaconVisible||_world.CraftingVisible,"Contextual access: "+station.Id);_world.ClosePower();}
            await HudChecks();await SixActors();await BarrowClearance();
            if(Capture)
            {
                foreach(var p in points){await Walk(p.Value);await Shot("hall-"+p.Key);}
                await Walk(Point(Plan.Cell(17,5)));await Shot("resurrection-alcove");
                await Walk(points["entrance"]);_input.Movement=Vector2.Down;await Frames(48);_input.Movement=Vector2.Zero;
                await Shot("entrance-crossing");
            }
            // Exercise relocation through existing authority and UI without inventing supplies.
            await VerifyFifthPlot();
            DirAccess.MakeDirRecursiveAbsolute(Folder);
            System.IO.File.WriteAllText(Folder+"/validation.json",JsonSerializer.Serialize(new{Seed=Region.Seed,Walking=_timings,Hud=_hudCases,
                SimulatedActors=6,MinimumActorSeparation=_closestActors,LiveActorCollisionMask=_actor.CollisionMask,ProxyFootprint=new[]{48,24},StartingPlots=4,InstalledPlots=_world.GrowingPlots.Count,
                Note="Walking measured through normal input and physics; clearance proxies are test-only, not multiplayer or wheelbarrow mechanics."},new JsonSerializerOptions{WriteIndented=true}));
            GD.Print("PASS: hall measurements, six-actor circulation, wide proxy, responsive HUD, existing farming/power/crafting and reset");
            _reset=true;_world._UnhandledKeyInput(new InputEventKey{PhysicalKeycode=Key.R,Pressed=true});
        }
        catch(Exception e){GD.PushError(e.ToString());GetTree().Quit(1);}
    }
    private async Task HudChecks()
    {
        foreach(var size in new[]{new Vector2I(960,540),new(1280,720),new(1536,864),new(1920,1080),new(1024,768),new(2560,1080)})
        {
            GetWindow().Size=size;await Frames(4);var hud=_world.GetNode<ThresholdHud>("HUD");
            hud.Present("An intentionally long sanctuary location name for safe-area and wrapping validation",Stock,
                "E / A Inspect the damaged machinery and choose whether to preserve it or dismantle it", "Long feedback must remain within the viewport safe margins", "",false,_world.Clock.Snapshot(),_world.HungerFor(1));
            foreach(var p in hud.Chrome!.Panels.Where(p=>p.Visible))
            {
                var transform=p.GetGlobalTransformWithCanvas();var a=transform*Vector2.Zero;var b=transform*p.Size;var view=_actor.GetViewportRect().Size;
                Check(a.X>=11&&a.Y>=11&&b.X<=view.X-11&&b.Y<=view.Y-11,"HUD panel stays in safe viewport bounds: "+size);
            }
            Check((_actor.GetViewportRect().Size/_actor.GetNode<Camera2D>("Camera2D").Zoom).DistanceTo(new(384,216))<.01,"World camera coverage preserved at "+size);
            _hudCases.Add(new{Window=size.ToString(),Viewport=_actor.GetViewportRect().Size.ToString(),Passed=true});
            if(Capture)await Shot("hud-"+size.X+"x"+size.Y,false);
        }
        GetWindow().Size=new(1536,864);await Frames(4);
    }
    private async Task SixActors()
    {
        var probes=new List<HallProbeBody>();int x=(Plan.Offset[0]+Plan.Entrance[0])*16,y=(Plan.Offset[1]+Plan.Entrance[1])*16;
        _actor.Position=Point(Plan.Cell(16,25));_actor.CollisionLayer=0;_actor.GetNode<RoomCharacter>("RoomCharacter").Hide();await Frames(2);
        for(int i=0;i<6;i++)
        {
            var start=new Vector2(x+8+i*16,i<3?y+40:y-64);var probe=new HallProbeBody{Position=start,Target=new(start.X,i<3?y-64:y+40),CollisionMask=3};
            _world.AddChild(probe);_world.GetNode<WorldVisuals>("WorldVisuals").Register(probe,"character");probes.Add(probe);
        }
        for(int frame=0;frame<180&&probes.Any(p=>p.Position.DistanceTo(p.Target)>1.5f);frame++)
        {
            await Frames(1);
            for(int i=0;i<6;i++)for(int j=i+1;j<6;j++)_closestActors=Mathf.Min(_closestActors,probes[i].Position.DistanceTo(probes[j].Position));
        }
        Check(probes.All(p=>p.Position.DistanceTo(p.Target)<=1.5f),"Six collision-enabled actors pass the door apron in opposing lanes");
        Check(_closestActors>=8,"Simulated actors remain separated, with no overlapping feet");
        if(Capture)await Shot("six-actors");
        foreach(var p in probes)p.QueueFree();await Frames(2);
        _actor.CollisionLayer=2;_actor.GetNode<RoomCharacter>("RoomCharacter").Show();
        Check(_actor.CollisionLayer==2&&_actor.CollisionMask==1,"Spectator-only exclusion ends; live player collision settings preserved");
    }
    private bool Fits(GridPoint cell,Vector2 size)
    {
        foreach(float dx in new[]{-size.X/2+1,0,size.X/2-1})foreach(float dy in new[]{-size.Y/2+1,0,size.Y/2-1})
            if(!Region.Walkable(MountainRegion.CellAt(cell.WorldX+dx,cell.WorldY+dy)))return false;
        return true;
    }
    private IReadOnlyList<GridPoint> WidePath(GridPoint start,GridPoint goal,Vector2 size)
    {
        var q=new Queue<GridPoint>();q.Enqueue(start);var seen=new Dictionary<GridPoint,GridPoint>{{start,start}};
        while(q.TryDequeue(out var p))
        {
            if(p==goal){var path=new List<GridPoint>{p};while(p!=start){p=seen[p];path.Add(p);}path.Reverse();return path;}
            foreach(var d in new[]{new GridPoint(1,0),new(0,1),new(-1,0),new(0,-1)})
            {var next=new GridPoint(p.X+d.X,p.Y+d.Y);if(!seen.ContainsKey(next)&&Fits(next,size)){seen[next]=p;q.Enqueue(next);}}
        }
        throw new InvalidOperationException("No wide clearance path "+start+" -> "+goal);
    }
    private async Task BarrowClearance()
    {
        var size=new Vector2(48,24);var start=Plan.Cell(16,29);var proxy=new HallProbeBody{Footprint=size,ShowActor=false,Position=Point(start),Target=Point(start),CollisionMask=1};_world.AddChild(proxy);await Frames(2);
        foreach(var target in new[]{Plan.Cell(16,26),Plan.Cell(14,11),Plan.Cell(20,12),Plan.Cell(10,12),Plan.Cell(10,16),Plan.Cell(22,17),Plan.Cell(24,24)})
        {
            foreach(var cell in WidePath(MountainRegion.CellAt(proxy.Position.X,proxy.Position.Y),target,size))
            {proxy.Target=Point(cell);int guard=0;while(proxy.Position.DistanceTo(proxy.Target)>1.5f&&guard++<120)await Frames(1);Check(guard<120,"Wide proxy physical clearance");}
        }
        var query=new PhysicsShapeQueryParameters2D{Shape=new RectangleShape2D{Size=size},Transform=new Transform2D(Mathf.Pi/2,proxy.Position),CollisionMask=1};
        Check(_world.GetWorld2D().DirectSpaceState.IntersectShape(query).Count==0,"48x24 proxy also fits rotated in the turning pad");
        query.Shape=new CircleShape2D{Radius=size.Length()/2};query.Transform=new Transform2D(0,proxy.Position);
        Check(_world.GetWorld2D().DirectSpaceState.IntersectShape(query).Count==0,"Circumscribed rotation sweep is clear for every proxy orientation");
        proxy.QueueFree();await Frames(2);
    }
    private async Task VerifyFifthPlot()
    {
        if(!_world.OnSurface){await Walk(Point(Region.Anchor("insidePortal")));for(int i=0;i<60&&!_world.OnSurface;i++)await Frames(1);}Check(_world.OnSurface,"Hall entrance reaches same surface");
        foreach(string id in new[]{"radio-shell","tool-case"})
        {var source=Region.Sources.Single(s=>s.Site.Id==id);await Walk(new((float)source.ApproachX,(float)source.ApproachY));await Interact();Check(Stock.Remaining[id]==0,"Finite unchanged material gathering: "+id);}
        await Walk(Point(Region.Entrance.Cell));for(int i=0;i<60&&_world.OnSurface;i++)await Frames(1);Check(!_world.OnSurface,"Surface return reaches new hall");
        await Walk(Chest);await Interact();_world.TransferSupplies(_actor,"fuel",1,true,Stock.Version);_world.CloseInventory();
        var generator=_world.PowerStations.Single(s=>s.Id=="generator");await Walk(generator.Position);await Interact();
        _world.PowerControls.ActivateRow(1);await Frames(2);_world.PowerControls.ActivateRow(0);await Frames(2);_world.PowerControls.ActivateRow(4);await Frames(2);_world.PowerControls.ActivateRow(5);await Frames(2);_world.ClosePower();
        var bench=_world.PowerStations.Single(s=>s.Id=="workbench");await Walk(bench.Position);await Interact();
        Check(_world.CraftAtWorkbench(_actor,new("growing_bed",Stock.Version,_world.Electrical!.Version)).Status==CraftStatus.Applied,"Existing powered recipe crafts a bed kit with finite personal materials");
        _world.ClosePower();await Walk(_world.GardenSocket+new Vector2(0,12));await Interact();
        Check(_world.GrowingPlots.Count==5,"Existing fifth plot installs; future reservation creates no extra plots");
        Check(_world.Electrical!.Consumers.Count==3,"Resurrection and utility reservations add no consumers");
        if(Capture){await Walk(Point(Plan.Cell(16,16)));await Shot("hall-powered");}
        _world.Clock.Advance(1200);await Frames(2);var plot=_world.GrowingPlots[0];
        Check(_world.Plot(plot.PlotId).Stage==CropStage.Mature,"Hall farming matures through ordinary clock skips and Fuel exhaustion");
        await Walk(plot.Position+new Vector2(0,12));int food=Stock.Personal.Count("food"),seed=Stock.Personal.Count("potato_seed");await Interact();
        Check(Stock.Personal.Count("food")==food+3&&Stock.Personal.Count("potato_seed")==seed+1,"Existing three-Food harvest and seed return work in the relocated garden");
    }
    private async Task Walk(Vector2 target)
    {
        bool zone=_world.OnSurface;var path=Region.Path(MountainRegion.CellAt(_actor.Position.X,_actor.Position.Y),MountainRegion.CellAt(target.X,target.Y));
        if(path.Count==0)throw new InvalidOperationException("No route "+_actor.Position+" -> "+target);
        foreach(var cell in path)
        {
            int guard=0;while(_actor.Position.DistanceTo(Point(cell))>1.5f&&guard++<180)
            {if(_world.OnSurface!=zone){_input.Movement=Vector2.Zero;return;}_input.Movement=(Point(cell)-_actor.Position).Normalized();await Frames(1);}
            if(guard>=180)throw new InvalidOperationException("Movement stuck: "+_actor.Position+" -> "+Point(cell));
        }
        int last=0;while(_actor.Position.DistanceTo(target)>1.5f&&last++<180)
        {if(_world.OnSurface!=zone)break;_input.Movement=(target-_actor.Position).Normalized();await Frames(1);}
        _input.Movement=Vector2.Zero;Check(last<180,"Final approach clear");
    }
    private async Task Interact(){_input.Tap=true;await Frames(3);}
    private static Vector2 Point(GridPoint p)=>new(p.WorldX,p.WorldY);
    private async Task Frames(int n){for(int i=0;i<n;i++){await ToSignal(GetTree(),SceneTree.SignalName.PhysicsFrame);await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}}
    private async Task Shot(string name,bool settle=true){if(!Capture)return;if(settle)await Frames(3);RenderingServer.ForceDraw(false);DirAccess.MakeDirRecursiveAbsolute(Folder);GetViewport().GetTexture().GetImage().SavePng(Folder+"/"+name+".png");}
    private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);GD.Print("PASS: "+message);}
}
