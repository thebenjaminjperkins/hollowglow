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

// Same harness is run before and after M6c. Diagnostic positioning/lamp never enter normal play.
public partial class PresentationReview : Node
{
    private readonly SmokeInput _input=new();
    private ThresholdWorld _world=null!;
    private PlayerCharacter _actor=null!;
    private string _folder="";
    private readonly List<object> _views=new();
    private readonly List<double> _frameTimes=new();
    private readonly List<double> _adapterCpu=new();
    private bool Recording=>OS.GetCmdlineUserArgs().Contains("--record");
    public override async void _Ready()
    {
        try
        {
            string tag=OS.GetCmdlineUserArgs().FirstOrDefault(a=>a.StartsWith("--review-tag="))?.Substring(13)??"after";
            _folder=ProjectSettings.GlobalizePath("user://presentation/"+tag);DirAccess.MakeDirRecursiveAbsolute(_folder);
            var room=GD.Load<PackedScene>("res://scenes/SanctuaryRoom.tscn").Instantiate<SanctuaryRoom>();
            room.LayoutProfile="garage-expedition";room.WorldSeed=1701;room.InputOverride=_input;AddChild(room);
            _world=room.World;_actor=_world.GetNode<PlayerCharacter>("Player");await Frames(5);
            if(tag=="after")
            {
                Check(room.Presentation.Visuals.SurfaceCount>200&&room.Presentation.Visuals.OccluderCount>100,"Reusable surface metadata and world wall occluders are assembled");
                Check(Descendants(_world).OfType<PointLight2D>().All(l=>l.ShadowEnabled&&l.GetParent() is WorldVisuals),"Local lights are shadow-enabled and independent of hidden fixture art");
            }
            await Place(Point(_world.Region!.Anchor("chest"))+new Vector2(0,18));await Capture("sanctuary");
            var fern=_world.GetNode<ThresholdMap>("Map").GetChildren().OfType<SanctuaryProp>().First(p=>p.Kind==PixelSprite.Motif.Fern);
            await Place(fern.Position+new Vector2(20,12));await Capture("cave-vegetation");
            if(Recording)await RecordRoute("sanctuary-motion",new[]{fern.Position+new Vector2(-24,14),fern.Position+new Vector2(24,14),fern.Position+new Vector2(24,-14),fern.Position+new Vector2(-24,-14)});
            _world._UnhandledKeyInput(new InputEventKey{PhysicalKeycode=Key.F3,Pressed=true});await Frames(4);
            var plan=_world.Garage!;
            await Place(Point(plan.Entry));await Capture("garage-entry");
            await Place(Point(new(plan.Origin.X+18,plan.Origin.Y+6)));await Capture("garage-bay");
            if(Recording)await RecordRoute("garage-motion",new[]{Point(plan.Entry),Point(new(plan.Origin.X+21,plan.Origin.Y+9)),Point(new(plan.Origin.X+21,plan.Origin.Y+5)),Point(new(plan.Origin.X+18,plan.Origin.Y+5))});
            _world.Clock.Advance(390-_world.Clock.Snapshot().TotalSeconds);await Place(Point(plan.Entry));await Capture("garage-dusk");
            _world.Clock.Advance(480-_world.Clock.Snapshot().TotalSeconds);await Capture("garage-night");
            // Fog disabled only for static-ground photometry: compare wall/doorway light blocking independently.
            _world.GetNode<ExplorationFog>("WorldFog").Hide();
            await Place(Point(new(plan.Origin.X+7,plan.Origin.Y+6)));
            var lamp=new PointLight2D{Name="DiagnosticLamp",Texture=new GradientTexture2D{Width=256,Height=256,
                Fill=GradientTexture2D.FillEnum.Radial,FillFrom=Vector2.One*.5f,FillTo=new(1,.5f),
                Gradient=new Gradient{Colors=new[]{Colors.White,new Color(1,1,1,0)}}},Energy=1.4f,
                Color=new Color("efbb79"),ShadowEnabled=true,Position=Point(new(plan.Origin.X+5,plan.Origin.Y+3)),RangeZMin=-20,RangeZMax=20};
            _world.AddChild(lamp);await Capture("occlusion-left");
            var discovered=_world.Discovery!.Snapshot(1);
            lamp.Position=Point(new(plan.Origin.X+9,plan.Origin.Y+3));await Capture("occlusion-right");
            lamp.Hide();await Capture("occlusion-off");
            Check(discovered.Explored.SequenceEqual(_world.Discovery.Snapshot(1).Explored)&&discovered.Visible.SequenceEqual(_world.Discovery.Snapshot(1).Visible),"Changing illumination does not alter discovered or current geography");
            var collisions=Descendants(_world).OfType<CollisionShape2D>().Where(s=>s.GetParent() is PhysicsBody2D).Select(s=>new{
                Position=s.GlobalPosition.ToString(),Type=s.Shape.GetType().Name,
                Size=s.Shape is RectangleShape2D rect?rect.Size.ToString():s.Shape is ConvexPolygonShape2D poly?string.Join(";",poly.Points.Select(p=>p.ToString())):"other",
                Layer=((PhysicsBody2D)s.GetParent()).CollisionLayer,Mask=((PhysicsBody2D)s.GetParent()).CollisionMask}).OrderBy(s=>s.Position).ThenBy(s=>s.Size).ToArray();
            System.IO.File.WriteAllText(_folder+"/evidence.json",JsonSerializer.Serialize(new{Tag=tag,Views=_views,Collisions=collisions,
                Ground=string.Join("",_world.Region!.Ground.Cast<GroundKind>().Select(k=>(int)k)),
                Sources=_world.Region.Sources.Select(s=>s.Site).ToArray(),MoveSpeed=_world.Config.MoveSpeed,InteractionRange=_world.Config.InteractionRange,
                FixedSimulationFrameMilliseconds=_frameTimes,AdapterCpuMilliseconds=_adapterCpu},new JsonSerializerOptions{WriteIndented=true}));
            GD.Print("PASS: matched presentation captures, collision/generation evidence and actual-input movement frames: "+_folder);GetTree().Quit();
        }
        catch(Exception e){GD.PushError(e.ToString());GetTree().Quit(1);}
    }
    private static IEnumerable<Node> Descendants(Node n){foreach(var child in n.GetChildren()){yield return child;foreach(var descendant in Descendants(child))yield return descendant;}}
    private static Vector2 Point(GridPoint p)=>new(p.WorldX,p.WorldY);
    private async Task Place(Vector2 p){_input.Movement=Vector2.Zero;_actor.Position=p;await Frames(4);}
    private async Task RecordRoute(string name,Vector2[] targets)
    {
        string dir=_folder+"/"+name;DirAccess.MakeDirRecursiveAbsolute(dir);int index=0;
        foreach(var target in targets)
        {
            var path=_world.Region!.Path(MountainRegion.CellAt(_actor.Position.X,_actor.Position.Y),MountainRegion.CellAt(target.X,target.Y));
            foreach(var cell in path)
            {
                int guard=0;
                while(_actor.Position.DistanceTo(Point(cell))>2&&guard++<120)
                {
                    _input.Movement=(Point(cell)-_actor.Position).Normalized();await Frames(1);
                    if(guard%4==0){RenderingServer.ForceDraw(false);GetViewport().GetTexture().GetImage().SavePng(dir+$"/{index++:D4}.png");}
                }
                if(guard>=120)throw new InvalidOperationException("Movement recording stuck at "+_actor.Position);
            }
        }
        _input.Movement=Vector2.Zero;
    }
    private async Task Capture(string name)
    {
        await Frames(4);RenderingServer.ForceDraw(false);
        if(GetViewport().GetTexture().GetImage().SavePng(_folder+"/"+name+".png")!=Error.Ok)throw new InvalidOperationException("Capture failed");
        _views.Add(new{Name=name,Actor=_actor.Position.ToString(),Seconds=_world.Clock.Snapshot().TotalSeconds});GD.Print("CAPTURE "+name);
    }
    private async Task Frames(int n){for(int i=0;i<n;i++){await ToSignal(GetTree(),SceneTree.SignalName.PhysicsFrame);await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);_frameTimes.Add(GetProcessDeltaTime()*1000);var v=_world.GetNodeOrNull<WorldVisuals>("WorldVisuals");if(v!=null)_adapterCpu.Add(v.LastCpuMilliseconds);}}
    private static void Check(bool condition,string message){if(!condition)throw new InvalidOperationException(message);GD.Print("PASS: "+message);}
}
