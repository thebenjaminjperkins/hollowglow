using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Godot;
using Hollowlight.Core;
using Hollowlight.World;

namespace Hollowlight.Presentation;

public sealed record SurfaceRule(Rect2 Ground,bool StaticGeometry=false,bool OccludesLight=false);

// Read-only presentation adapter. Never writes inventory, discovery, time, navigation or collision.
public partial class WorldVisuals : Node2D
{
    public ThresholdWorld World{get;set;}=null!;
    public Color CaveAmbient{get;set;}=new(.78f,.80f,.84f);
    public bool Neutral{get;set;}
    public bool DiscoveryEnabled{get;set;}=true;
    public int OccluderCount{get;private set;}
    public int SurfaceCount=>_materials.Count;
    private readonly List<ShaderMaterial> _materials=new();
    private readonly Dictionary<Node2D,Rect2> _bounds=new();
    private readonly Dictionary<Node2D,Rect2> _movingGround=new();
    private readonly Dictionary<string,SurfaceRule> _rules=new();
    private Image _geography=null!;
    private ImageTexture _map=null!,_exposure=null!;
    private Shader _shader=null!;
    private int _width,_height;
    private long _lastRevision=-1;
    private Color _lastSky=new(-1,-1,-1),_lastCave=new(-1,-1,-1);
    private bool _lastNeutral,_lastDiscovery;
    public double LastCpuMilliseconds{get;private set;}
    public override void _Ready()
    {
        _width=World.Region?.Width??40;_height=World.Region?.Height??22;
        // Cut-corner atlas alpha must reveal cave mass/unknown geography, never the viewport clear color.
        AddChild(new ColorRect{Size=new(_width*16,_height*16),Color=new("10191e"),ZIndex=-100,MouseFilter=Control.MouseFilterEnum.Ignore,LightMask=0});
        _shader=GD.Load<Shader>("res://scripts/presentation/world_surface.gdshader");
        using(var doc=JsonDocument.Parse(FileAccess.GetFileAsString("res://data/presentation.json")))
            foreach(var entry in doc.RootElement.GetProperty("families").EnumerateObject())
            {
                var v=entry.Value;var a=v.GetProperty("ground");
                _rules[entry.Name]=new(new(a[0].GetSingle(),a[1].GetSingle(),a[2].GetSingle(),a[3].GetSingle()),
                    v.TryGetProperty("staticGeometry",out var s)&&s.GetBoolean(),v.TryGetProperty("occludesLight",out var o)&&o.GetBoolean());
            }
        _geography=Image.CreateEmpty(_width,_height,false,Image.Format.Rgb8);_geography.Fill(Colors.White);_map=ImageTexture.CreateFromImage(_geography);
        var zones=Image.CreateEmpty(_width,_height,false,Image.Format.Rgb8);
        for(int y=0;y<_height;y++)for(int x=0;x<_width;x++)
        {
            bool outside=World.Region?.IsSurface(x*16+8,y*16+8)??x>=20;
            zones.SetPixel(x,y,new(outside?1:0,0,0));
        }
        foreach(var zone in Descendants(World).OfType<LightingZone>().OrderBy(z=>z.Priority))
        {
            var bounds=new Rect2(zone.ToGlobal(zone.Bounds.Position),zone.Bounds.Size*zone.GlobalScale);
            for(int y=Math.Max(0,Mathf.FloorToInt(bounds.Position.Y/16));y<Math.Min(_height,Mathf.CeilToInt(bounds.End.Y/16));y++)
            for(int x=Math.Max(0,Mathf.FloorToInt(bounds.Position.X/16));x<Math.Min(_width,Mathf.CeilToInt(bounds.End.X/16));x++)
                zones.SetPixel(x,y,new(zone.SkyExposure,zone.Shelter,0));
        }
        _exposure=ImageTexture.CreateFromImage(zones);
        RegisterScene();BuildWallOccluders();UpdateMap();UpdateMaterials();
    }
    public static IEnumerable<Node> Descendants(Node root)
    {foreach(var n in root.GetChildren()){yield return n;foreach(var child in Descendants(n))yield return child;}}
    private SurfaceRule Rule(string name)=>_rules.GetValueOrDefault(name,_rules["item"]);
    public void RegisterScene()
    {
        // Register semantic roots first; children inherit the same ground projection and visibility policy.
        foreach(var n in Descendants(World).OfType<Node2D>().ToArray())
        {
            if(n is PlayerCharacter)Register(n,Rule("character"));
            else if(n is GarageObjectArt garage)
            {
                var o=garage.Object.Rule;Register(n,new SurfaceRule(new(-o.Width/2f,o.OffsetY-o.Height/2f,o.Width,o.Height),false,o.Kind is "machinery" or "vehicle" or "container"));
            }
            else if(n is SanctuaryProp prop)
            {
                string family=prop.Kind.ToString().ToLowerInvariant();family=family=="generator"?"machine":family=="bench"?"workbench":family=="plant"?"fern":family;
                Register(n,Rule(family));
            }
            else if(n is Interactable target)Register(n,Rule(target.IsStorage?"storage":"item"));
            else if(n is GrowingPlotArt)Register(n,Rule("crop"));
            else if(n is GardenSocketArt)Register(n,Rule("crop"));
            else if(n is RegionProp region)
                Register(n,new SurfaceRule(new(-Math.Max(8,region.Placement.Width/2f),-Math.Max(4,region.Placement.Height/2f),Math.Max(16,region.Placement.Width),Math.Max(8,region.Placement.Height)),region.Placement.Asset is not ("fern" or "mushrooms"),region.Placement.Width>0));
            else if(n.HasMeta("surface_family"))Register(n,Rule(n.GetMeta("surface_family").AsString()));
            else if(n.HasMeta("wall_surface"))Register(n,n.HasMeta("surface_footprint")?Rule("wall") with{Ground=n.GetMeta("surface_footprint").AsRect2()}:Rule("wall"));
        }
        foreach(var tile in Descendants(World).OfType<TileMapLayer>())Apply(tile,0,new());
        // Discovery can hide a fixture's art; the physical light keeps illuminating visible surfaces.
        foreach(var light in Descendants(World).OfType<PointLight2D>().ToArray())
        {
            light.ShadowEnabled=true;
            if(light.GetParent() is Node2D fixture&&_bounds.TryGetValue(fixture,out var ground))
            {
                // Project the emitter in front of its solid support; preserve the visible glow at bulb height.
                // A source inside its own closed caster would shadow the entire room.
                Vector2 visibleEmitter=light.GlobalPosition,source=new(fixture.GlobalPosition.X,ground.End.Y+2);
                light.Reparent(this,true);light.GlobalPosition=source;light.Offset=visibleEmitter-source;
            }
            else if(light.GetParent()!=this)light.Reparent(this,true);
        }
    }
    public void Register(Node2D owner,SurfaceRule rule)
    {
        var ground=new Rect2(owner.GlobalPosition+rule.Ground.Position,rule.Ground.Size);_bounds[owner]=ground;
        if(owner is CharacterBody2D)_movingGround[owner]=rule.Ground;
        Rect2 visual=rule.Ground;
        foreach(var sprite in Descendants(owner).OfType<Sprite2D>())
        {
            var rect=sprite.GetRect();
            foreach(var point in new[]{rect.Position,new Vector2(rect.End.X,rect.Position.Y),rect.End,new Vector2(rect.Position.X,rect.End.Y)})
                visual=visual.Expand(owner.ToLocal(sprite.ToGlobal(point)));
        }
        owner.SetMeta("presentation_ground",rule.Ground);owner.SetMeta("presentation_visual_bounds",visual);
        owner.SetMeta("presentation_static",rule.StaticGeometry);
        if(owner is GrowingPlotArt or GardenSocketArt)Apply(owner,2,ground);
        foreach(var item in Descendants(owner).OfType<CanvasItem>())
            if(item is Sprite2D or PowerIndicator)Apply(item,rule.StaticGeometry?1:2,ground);
        if(rule.OccludesLight)AddOccluder(ground);
    }
    public void Register(Node2D owner,string family)=>Register(owner,Rule(family));
    private void Apply(CanvasItem item,int kind,Rect2 ground)
    {
        if(item.HasMeta("world_material"))return;
        var material=new ShaderMaterial{Shader=_shader};item.Material=material;item.SetMeta("world_material",true);
        material.SetShaderParameter("geography",_map);material.SetShaderParameter("exposure",_exposure);
        material.SetShaderParameter("world_size",new Vector2(_width*16,_height*16));
        material.SetShaderParameter("surface_kind",kind);material.SetShaderParameter("ground_rect",new Vector4(ground.Position.X,ground.Position.Y,ground.Size.X,ground.Size.Y));
        material.SetShaderParameter("contact_shadow",item.HasMeta("contact_shadow"));
        SetLighting(material,SkyAmbient());
        _materials.Add(material);
    }
    public bool IsVisible(Node2D owner)
    {
        if(World.Discovery==null||!DiscoveryEnabled)return true;
        Rect2 bounds=_bounds.GetValueOrDefault(owner,new Rect2(owner.GlobalPosition-new Vector2(4,4),new(8,8)));
        // Moving owners retain local footprint metadata without changing their simulation origin.
        if(owner is PlayerCharacter)return true;
        for(int y=Mathf.FloorToInt(bounds.Position.Y/16);y<=Mathf.FloorToInt((bounds.End.Y-.1f)/16);y++)
        for(int x=Mathf.FloorToInt(bounds.Position.X/16);x<=Mathf.FloorToInt((bounds.End.X-.1f)/16);x++)
            if(World.Discovery.Visible(World.GetNode<PlayerCharacter>("Player").PlayerId,new(x,y))&&!Opaque(x,y))return true;
        return false;
    }
    private bool Opaque(int x,int y)=>!World.GetNode<ThresholdMap>("Map").Layout.IsFloor(x,y);
    private void AddOccluder(Rect2 r)
    {
        var polygon=new OccluderPolygon2D{Polygon=new[]{r.Position,new Vector2(r.End.X,r.Position.Y),r.End,new Vector2(r.Position.X,r.End.Y)}};
        AddChild(new LightOccluder2D{Occluder=polygon,ShowBehindParent=true});OccluderCount++;
    }
    private void BuildWallOccluders()
    {
        // Boundary edges only: internal solid-cell edges do not create duplicate/shimmering shadows.
        for(int y=0;y<_height;y++)for(int x=0;x<_width;x++)
        {
            if(!Opaque(x,y))continue;
            var p=new Vector2(x*16,y*16);
            void Edge(Vector2 a,Vector2 b)=>AddChild(new LightOccluder2D{Occluder=new OccluderPolygon2D{Closed=false,Polygon=new[]{a,b}},ShowBehindParent=true});
            if(!Opaque(x,y-1)){Edge(p,p+new Vector2(16,0));OccluderCount++;}
            if(!Opaque(x+1,y)){Edge(p+new Vector2(16,0),p+new Vector2(16,16));OccluderCount++;}
            if(!Opaque(x,y+1)){Edge(p+new Vector2(16,16),p+new Vector2(0,16));OccluderCount++;}
            if(!Opaque(x-1,y)){Edge(p+new Vector2(0,16),p);OccluderCount++;}
        }
    }
    private void UpdateMap()
    {
        if(World.Discovery==null)return;
        if(_lastRevision==World.Discovery.Revision)return;
        _lastRevision=World.Discovery.Revision;
        _geography.Fill(Colors.Black);
        var snapshot=World.Discovery.Snapshot(World.GetNode<PlayerCharacter>("Player").PlayerId);
        foreach(var p in snapshot.Explored)_geography.SetPixel(p.X,p.Y,new(1,0,Opaque(p.X,p.Y)?1:0));
        foreach(var p in snapshot.Visible)_geography.SetPixel(p.X,p.Y,new(1,1,Opaque(p.X,p.Y)?1:0));
        _map.Update(_geography);
    }
    public Color SkyAmbient()
    {
        var clock=World.Clock.Snapshot();
        var daylight=new Color(1,.98f,.94f);var night=new Color(.56f,.65f,.76f);
        // Smooth around configured phase boundaries, without changing authoritative clock durations.
        if(clock.Phase==DayPhase.Day)return daylight;
        if(clock.Phase==DayPhase.Dusk)return daylight.Lerp(night,(float)(clock.PhaseElapsedSeconds/(clock.PhaseDurationTicks/(double)TimeSpan.TicksPerSecond)));
        return night.Lerp(daylight,(float)Math.Clamp(1-clock.PhaseRemainingSeconds/15,0,1));
    }
    private void UpdateMaterials()
    {
        var sky=SkyAmbient();
        if(sky==_lastSky&&CaveAmbient==_lastCave&&Neutral==_lastNeutral&&DiscoveryEnabled==_lastDiscovery)return;
        foreach(var material in _materials)SetLighting(material,sky);
        _lastSky=sky;_lastCave=CaveAmbient;_lastNeutral=Neutral;_lastDiscovery=DiscoveryEnabled;
    }
    private void SetLighting(ShaderMaterial material,Color sky)
    {material.SetShaderParameter("neutral",Neutral);material.SetShaderParameter("discovery_enabled",DiscoveryEnabled&&World.Discovery!=null);material.SetShaderParameter("cave_ambient",CaveAmbient);material.SetShaderParameter("sky_ambient",sky);}
    public override void _Process(double delta)
    {
        long started=System.Diagnostics.Stopwatch.GetTimestamp();
        var actor=World.GetNode<PlayerCharacter>("Player");UpdateMap();
        foreach(var pair in _movingGround.ToArray())
        {
            var moving=pair.Key;
            if(!GodotObject.IsInstanceValid(moving)||!moving.IsInsideTree()){_movingGround.Remove(moving);_bounds.Remove(moving);continue;}
            var ground=new Rect2(moving.GlobalPosition+pair.Value.Position,pair.Value.Size);_bounds[moving]=ground;
            foreach(var sprite in Descendants(moving).OfType<Sprite2D>())if(sprite.Material is ShaderMaterial material)material.SetShaderParameter("ground_rect",new Vector4(ground.Position.X,ground.Position.Y,ground.Size.X,ground.Size.Y));
        }
        UpdateMaterials();
        LastCpuMilliseconds=System.Diagnostics.Stopwatch.GetElapsedTime(started).TotalMilliseconds;
    }
    public override void _ExitTree()
    {
        // These resources are unique to this renderer instance. Release managed bindings before
        // reset/shutdown rather than leaving hundreds of shader/texture wrappers to finalizers.
        // The loaded shader and shared asset textures are NOT owned here and remain cached.
        foreach(var material in _materials)material.Dispose();
        _materials.Clear();_bounds.Clear();_movingGround.Clear();
        _map?.Dispose();_exposure?.Dispose();_geography?.Dispose();
    }
}
