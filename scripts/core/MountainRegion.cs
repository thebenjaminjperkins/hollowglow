using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Stream = Hollowlight.Core.FiniteScavenging.SeedStream;

namespace Hollowlight.Core;

public readonly record struct GridPoint(int X,int Y)
{
    public int WorldX=>X*16+8;
    public int WorldY=>Y*16+8;
}
public sealed record MountainShape(double X,double Y,double Rx,double Ry);
public sealed record LandmarkRule(string Id,string Name,string Kind,double MinX,double MaxX,double MinY,double MaxY,int Radius,string Attachment="");
public sealed record RouteRule(string Id,string From,string To,string Kind,int HalfWidth,double Chance,int Bend=10);
public sealed record ChamberRule(string Id,string Name,int X,int Y,int Rx,int Ry);
public sealed record RegionDefinition(int SurfaceWidth,int SurfaceHeight,int SurfaceOffsetX,int SanctuaryWidth,int SanctuaryHeight,
    string[] Orientations,MountainShape Mountain,double[][] River,LandmarkRule[] Landmarks,RouteRule[] Routes,
    ChamberRule[] Chambers,string[][] ChamberLinks,Dictionary<string,int[]> Anchors,PropPlacement[] Props,Dictionary<string,string> ResourceLocations,Dictionary<string,string>? ResourceVisuals=null,SanctuaryPlan? Hall=null)
{
    public static RegionDefinition Parse(string json)=>JsonSerializer.Deserialize<RegionDefinition>(json,
        new JsonSerializerOptions {PropertyNameCaseInsensitive=true})??throw new ArgumentException("Missing region definition.");
    public void Validate()
    {
        if(SurfaceWidth<64||SurfaceWidth>256||SurfaceHeight<64||SurfaceHeight>256||SanctuaryWidth<32||SanctuaryHeight<32||
            SurfaceOffsetX<SanctuaryWidth+8||Orientations.Length==0||Orientations.Any(x=>x is not ("identity" or "mirror-x"))||
            Landmarks.Length<2||Landmarks.Select(n=>n.Id).Distinct().Count()!=Landmarks.Length||Chambers.Select(n=>n.Id).Distinct().Count()!=Chambers.Length||
            Landmarks.Any(n=>n.MinX<.03||n.MaxX>.97||n.MinY<.03||n.MaxY>.97||n.MinX>n.MaxX||n.MinY>n.MaxY||n.Radius<2)||
            Routes.Any(e=>!Landmarks.Any(n=>n.Id==e.From)||!Landmarks.Any(n=>n.Id==e.To)||e.HalfWidth<1||e.Chance<0||e.Chance>1)||
            River.Length<2||River.Any(p=>p.Length!=2)||Mountain.Rx<=0||Mountain.Ry<=0||
            new[]{"home","chest","insidePortal","insideLanding","expansion"}.Any(id=>!Anchors.ContainsKey(id)||Anchors[id].Length!=2))
            throw new ArgumentException("Invalid region dimensions, graph, geometry or anchors.");
    }
}
public enum GroundKind { Rock, Cave, Grass, Soil, Road, Water, Bridge, Gravel }
public sealed record RegionLandmark(string Id,string Name,string Kind,GridPoint Cell,int Radius);
public sealed record RegionRoute(string Id,string From,string To,string Kind,IReadOnlyList<GridPoint> Cells);
public sealed record RegionDecoration(string Asset,int X,int Y,int Width=0,int Height=0,int Variant=0);

// One generated spatial profile. Simulation remains in the existing session owner.
// Graph definitions, bounds, orientation and source sockets are inputs, not world-coordinate rules.
public sealed class MountainRegion
{
    public RegionDefinition Definition {get;}
    public int Seed {get;}
    public int Width=>Definition.SurfaceOffsetX+Definition.SurfaceWidth;
    public int Height=>Math.Max(Definition.SurfaceHeight,Definition.SanctuaryHeight);
    public GroundKind[,] Ground {get;}
    public CaveLayout Layout {get;private set;}=null!;
    public IReadOnlyList<RegionLandmark> Landmarks=>_landmarks;
    public IReadOnlyList<RegionRoute> Routes=>_routes;
    public IReadOnlyList<RegionDecoration> Decorations=>_decor;
    public IReadOnlyList<GeneratedSourceSite> Sources=>_sources;
    public GaragePlan? Garage {get;private set;}
    private readonly List<RegionLandmark> _landmarks=new();
    private readonly List<RegionRoute> _routes=new();
    private readonly List<RegionDecoration> _decor=new();
    private readonly List<GeneratedSourceSite> _sources=new();
    private readonly bool[,] _nav;
    private readonly List<PropPlacement> _props=new();
    public GridPoint Anchor(string id)=>new(Definition.Anchors[id][0],Definition.Anchors[id][1]);
    public RegionLandmark Entrance=>_landmarks.Single(n=>n.Kind=="entrance");
    public GridPoint OutsideLanding=>new(Entrance.Cell.X,Entrance.Cell.Y+2);
    public bool IsSurface(double x,double y)=>x>=Definition.SurfaceOffsetX*16&&x<Width*16&&y>=0&&y<Definition.SurfaceHeight*16;
    public bool InBounds(double x,double y)=>x>=0&&y>=0&&x<Width*16&&y<Height*16;
    public bool Walkable(GridPoint p)=>p.X>=0&&p.Y>=0&&p.X<Width&&p.Y<Height&&_nav[p.X,p.Y];
    private MountainRegion(RegionDefinition definition,int seed)
    {Definition=definition;Seed=seed;Ground=new GroundKind[Width,Height];_nav=new bool[Width,Height];}
    public static MountainRegion Generate(RegionDefinition definition,SupplySetup supplies,ScavengingSetup food,ScavengingSetup fuel,int seed)
    {
        definition.Validate();var region=new MountainRegion(definition,seed);
        region.BuildSanctuary();region.BuildSurface();region.BakeNavigation();region.PlaceSources(supplies,food,fuel);region.DressGroves();region.MakeLayout();return region;
    }
    private bool Floor(int x,int y)=>x>=0&&y>=0&&x<Width&&y<Height&&Ground[x,y] is not (GroundKind.Rock or GroundKind.Water);
    private void Disc(int cx,int cy,int radius,GroundKind kind,bool preserveWater=false)
    {
        for(int y=cy-radius;y<=cy+radius;y++)for(int x=cx-radius;x<=cx+radius;x++)
            if(x>0&&y>0&&x<Width-1&&y<Height-1&&(x-cx)*(x-cx)+(y-cy)*(y-cy)<=radius*radius&&
                (!preserveWater||Ground[x,y] is not (GroundKind.Water or GroundKind.Rock or GroundKind.Bridge)))Ground[x,y]=kind;
    }
    private static IEnumerable<GridPoint> Line(GridPoint a,GridPoint b)
    {
        int count=Math.Max(Math.Abs(a.X-b.X),Math.Abs(a.Y-b.Y));
        if(count==0){yield return a;yield break;}
        for(int i=0;i<=count;i++)yield return new((int)Math.Round(a.X+(b.X-a.X)*(double)i/count),(int)Math.Round(a.Y+(b.Y-a.Y)*(double)i/count));
    }
    private void BuildSanctuary()
    {
        if(Definition.Hall is {} hall)
        {
            for(int y=0;y<hall.Rows.Length;y++)for(int x=0;x<hall.Rows[y].Length;x++)
                if(hall.Rows[y][x]=='.'){var p=hall.Cell(x,y);Ground[p.X,p.Y]=GroundKind.Cave;}
            _props.AddRange(Definition.Props);return;
        }
        foreach(var room in Definition.Chambers)
            for(int y=room.Y-room.Ry-1;y<=room.Y+room.Ry+1;y++)for(int x=room.X-room.Rx-1;x<=room.X+room.Rx+1;x++)
            {
                double edge=1+ .045*Math.Sin(x*1.71+y*.93)+.025*Math.Cos(y*2.3-x*.51);
                if(x>0&&y>0&&x<Definition.SanctuaryWidth-1&&y<Definition.SanctuaryHeight-1&&
                    Math.Pow((x-room.X)/(double)room.Rx,2)+Math.Pow((y-room.Y)/(double)room.Ry,2)<edge)Ground[x,y]=GroundKind.Cave;
            }
        foreach(var link in Definition.ChamberLinks)
        {
            var a=Definition.Chambers.Single(c=>c.Id==link[0]);var b=Definition.Chambers.Single(c=>c.Id==link[1]);
            foreach(var p in Line(new(a.X,a.Y),new(b.X,b.Y)))Disc(p.X,p.Y,2,GroundKind.Cave);
        }
        _props.AddRange(Definition.Props);
        _decor.Add(new("mouth",Anchor("insidePortal").WorldX,Anchor("insidePortal").WorldY+8));
    }
    private void BuildSurface()
    {
        var random=new Stream(Seed,"mountain-plan-v1");int offset=Definition.SurfaceOffsetX,w=Definition.SurfaceWidth,h=Definition.SurfaceHeight;
        var river=Definition.River.Select(p=>new GridPoint((int)(p[0]*w)+random.Next(9)-4,(int)(p[1]*(h-1)))).ToArray();
        int RiverX(int y)
        {
            for(int i=1;i<river.Length;i++)if(y<=river[i].Y)
            {var a=river[i-1];var b=river[i];return (int)Math.Round(a.X+(b.X-a.X)*(y-a.Y)/(double)Math.Max(1,b.Y-a.Y));}
            return river[^1].X;
        }
        var m=Definition.Mountain;
        bool MountainAt(int x,int y)=>Math.Pow((x-m.X*w)/(m.Rx*w),2)+Math.Pow((y-m.Y*h)/(m.Ry*h),2)<1+.055*Math.Sin(y*.7)+.025*Math.Cos(x*.9);
        foreach(var rule in Definition.Landmarks)
        {
            int x=(int)(rule.MinX*w)+random.Next(Math.Max(1,(int)((rule.MaxX-rule.MinX)*w)+1));
            int y=(int)(rule.MinY*h)+random.Next(Math.Max(1,(int)((rule.MaxY-rule.MinY)*h)+1));
            if(rule.Attachment=="river")x=RiverX(y);
            if(rule.Attachment=="mountain-foot")
            {
                int last=Enumerable.Range(1,h-2).Where(yy=>MountainAt(x,yy)).DefaultIfEmpty(y-2).Max();y=last+2;
            }
            _landmarks.Add(new(rule.Id,rule.Name,rule.Kind,new(x+offset,y),rule.Radius));
        }
        for(int y=1;y<h-1;y++)for(int x=1;x<w-1;x++)
        {
            Ground[x+offset,y]=MountainAt(x,y)?GroundKind.Rock:
                Math.Abs(x-RiverX(y))<=2+(int)(1+Math.Sin(y*.19))?GroundKind.Water:GroundKind.Grass;
        }
        foreach(var node in _landmarks)
        {
            if(node.Kind is "bridge" or "footbridge")
            {
                int half=node.Kind=="bridge"?3:1;
                for(int y=node.Cell.Y-half;y<=node.Cell.Y+half;y++)for(int x=node.Cell.X-8;x<=node.Cell.X+8;x++)Ground[x,y]=GroundKind.Bridge;
            }
            else Disc(node.Cell.X,node.Cell.Y,node.Radius,node.Kind is "yard" or "hamlet"?GroundKind.Gravel:GroundKind.Soil,true);
            if(node.Kind=="entrance")Disc(node.Cell.X,node.Cell.Y,2,GroundKind.Gravel);
        }
        BakeNavigation();
        foreach(var edge in Definition.Routes)
        {
            var stream=new Stream(Seed,"route-v1:"+edge.Id);if(stream.Next(10000)>=edge.Chance*10000)continue;
            var from=_landmarks.Single(n=>n.Id==edge.From);var to=_landmarks.Single(n=>n.Id==edge.To);
            var middle=new GridPoint((from.Cell.X+to.Cell.X)/2+stream.Next(edge.Bend*2+1)-edge.Bend,
                (from.Cell.Y+to.Cell.Y)/2+stream.Next(edge.Bend*2+1)-edge.Bend);
            var first=Walkable(middle)?Path(from.Cell,middle):Array.Empty<GridPoint>();
            var second=Walkable(middle)?Path(middle,to.Cell):Array.Empty<GridPoint>();
            IReadOnlyList<GridPoint> path=first.Count>0&&second.Count>0?first.Concat(second.Skip(1)).ToArray():Path(from.Cell,to.Cell);
            if(path.Count==0)throw new ArgumentException("Region graph edge cannot connect: "+edge.Id);
            foreach(var p in path)Disc(p.X,p.Y,edge.HalfWidth,edge.Kind=="road"?GroundKind.Road:GroundKind.Soil,true);
            _routes.Add(new(edge.Id,edge.From,edge.To,edge.Kind,path));
        }
        // Orientation is a profile input. All surface fields, graph points and routes transform together.
        string orientation=Definition.Orientations[random.Next(Definition.Orientations.Length)];
        GridPoint Transform(GridPoint p)=>new((orientation == "mirror-x"?w-1-(p.X-offset):p.X-offset)+offset,
            p.Y);
        if(orientation!="identity")
        {
            var copy=(GroundKind[,])Ground.Clone();
            for(int y=0;y<h;y++)for(int x=offset;x<Width;x++){var p=Transform(new(x,y));Ground[p.X,p.Y]=copy[x,y];}
            for(int i=0;i<_landmarks.Count;i++)_landmarks[i]=_landmarks[i] with {Cell=Transform(_landmarks[i].Cell)};
            for(int i=0;i<_routes.Count;i++)_routes[i]=_routes[i] with {Cells=_routes[i].Cells.Select(Transform).ToArray()};
        }
        BuildDecoration();
    }
    private void BuildDecoration()
    {
        var random=new Stream(Seed,"region-decoration-v1");int offset=Definition.SurfaceOffsetX;
        foreach(var node in _landmarks.Where(n=>n.Kind is "hamlet" or "yard" or "shelter"))
        {
            foreach(int dx in new[]{-48,0,48})_decor.Add(new("ruin",node.Cell.WorldX+dx,node.Cell.WorldY-64,28,8,0));
            foreach(int dx in new[]{-64,64})_decor.Add(new("ruin",node.Cell.WorldX+dx,node.Cell.WorldY+32,28,8,1));
            _decor.Add(new("sign",node.Cell.WorldX-56,node.Cell.WorldY+20,0,0));
        }
        _decor.Add(new("mouth",Entrance.Cell.WorldX,Entrance.Cell.WorldY+8,0,0));
        for(int i=0;i<1200;i++)
        {
            int x=offset+2+random.Next(Definition.SurfaceWidth-4),y=2+random.Next(Definition.SurfaceHeight-4);
            if(Ground[x,y]!=GroundKind.Grass||_landmarks.Any(n=>Math.Abs(n.Cell.X-x)+Math.Abs(n.Cell.Y-y)<n.Radius+4))continue;
            bool wooded=Math.Sin(x*.047)+Math.Cos(y*.071)>.4;
            if(!wooded&&i%4!=0)continue;
            if(_decor.Any(d=>Math.Abs(d.X-(x*16+8))<28&&Math.Abs(d.Y-(y*16+8))<24))continue;
            _decor.Add(new(wooded&&i%3!=0?"tree":i%2==0?"fern":"stones",x*16+8,y*16+8,wooded&&i%3!=0?12:0,wooded&&i%3!=0?8:0,random.Next(3)));
        }
    }
    private void BakeNavigation()
    {
        for(int y=0;y<Height;y++)for(int x=0;x<Width;x++)_nav[x,y]=Floor(x,y);
        void Block(double px,double py,double width,double height)
        {
            if(width<=0||height<=0)return;
            for(int y=(int)Math.Floor((py-height/2-3)/16);y<=(int)Math.Floor((py+height/2+3)/16);y++)
            for(int x=(int)Math.Floor((px-width/2-4)/16);x<=(int)Math.Floor((px+width/2+4)/16);x++)
                if(x>=0&&y>=0&&x<Width&&y<Height&&Math.Abs(x*16+8-px)<width/2+4&&Math.Abs(y*16+8-py)<height/2+3)_nav[x,y]=false;
        }
        foreach(var p in _props)Block(p.X,p.Y,p.Width,p.Height);
        if(Definition.Hall is {} hall)foreach(var f in hall.Fixtures){var p=hall.FixtureFoot(f);Block(p.WorldX,p.WorldY,f.Width,f.Height);}
        foreach(var p in _decor)Block(p.X,p.Y,p.Width,p.Height);
        if(Garage!=null)foreach(var p in Garage.Objects)Block(p.Foot.WorldX,p.Foot.WorldY+p.Rule.OffsetY,p.Rule.Width,p.Rule.Height);
        Block(Anchor("chest").WorldX,Anchor("chest").WorldY,14,8);
        foreach(var source in _sources.Where(s=>s.Band=="material"))Block(source.Site.X,source.Site.Y+1,14,8);
    }
    private void PlaceSources(SupplySetup supply,ScavengingSetup food,ScavengingSetup fuel)
    {
        var budgets=supply.Sites.Select(s=>(Site:s,Type:"material",Band:"material")).Concat(
            FiniteScavenging.RollBudget(food,Seed).Concat(FiniteScavenging.RollBudget(fuel,Seed))
                .Select(s=>(s.Site,s.Type,s.Band))).ToArray();
        var distances=Distances(Entrance.Cell);
        foreach(var budget in budgets)
        {
            if(!Definition.ResourceLocations.TryGetValue(budget.Site.Id,out string? location))throw new ArgumentException("Missing resource landmark: "+budget.Site.Id);
            var node=_landmarks.Single(n=>n.Id==location);var random=new Stream(Seed,"region-source-v1:"+budget.Site.Id);
            var candidates=new List<GridPoint>();
            for(int y=node.Cell.Y-node.Radius;y<=node.Cell.Y+node.Radius;y++)for(int x=node.Cell.X-node.Radius;x<=node.Cell.X+node.Radius;x++)
            {
                var p=new GridPoint(x,y);
                if(!Walkable(p)||!Walkable(new(x,y+1))||!Walkable(new(x-1,y))||!Walkable(new(x+1,y))||!Walkable(new(x,y-1))||!distances.ContainsKey(p))continue;
                if(_sources.Any(s=>Math.Abs(s.Site.X-p.WorldX)<48&&Math.Abs(s.Site.Y-p.WorldY)<48)||
                    _decor.Any(d=>Math.Abs(d.X-p.WorldX)<36&&Math.Abs(d.Y-p.WorldY)<36)||
                    p==Entrance.Cell||Math.Abs(p.X-Entrance.Cell.X)+Math.Abs(p.Y-Entrance.Cell.Y)<4)continue;
                candidates.Add(p);
            }
            if(candidates.Count==0)throw new ArgumentException("No resource socket at "+location);
            var chosen=candidates[random.Next(candidates.Count)];
            var site=budget.Site with {X=chosen.WorldX,Y=chosen.WorldY,Asset=Definition.ResourceVisuals?.GetValueOrDefault(budget.Site.Id)??budget.Site.Asset};
            _sources.Add(new(site,budget.Type,budget.Band,(int)Math.Round(distances[chosen]),chosen.WorldX,chosen.WorldY+12));
            BakeNavigation();distances=Distances(Entrance.Cell);
        }
        foreach(var source in _sources)
            if(Path(Entrance.Cell,CellAt(source.ApproachX,source.ApproachY)).Count==0)throw new ArgumentException("Unreachable resource: "+source.Site.Id);
    }
    private void DressGroves()
    {
        // Cosmetic ground plants come after source placement: never move a budget,
        // interaction socket or collision footprint merely to fill out a clearing.
        foreach(var grove in _landmarks.Where(n=>n.Kind=="grove"))
        {
            var random=new Stream(Seed,"grove-dressing-v1:"+grove.Id);
            for(int i=0;i<32;i++)
            {
                int x=grove.Cell.WorldX+(random.Next(grove.Radius*24)-grove.Radius*12);
                int y=grove.Cell.WorldY+(random.Next(grove.Radius*24)-grove.Radius*12);
                var cell=CellAt(x,y);
                if(!Walkable(cell)||Math.Abs(x-grove.Cell.WorldX)+Math.Abs(y-grove.Cell.WorldY)<40||
                    _sources.Any(n=>Math.Abs(n.Site.X-x)<40&&Math.Abs(n.Site.Y-y)<40)||
                    _decor.Any(n=>Math.Abs(n.X-x)<24&&Math.Abs(n.Y-y)<24))continue;
                _decor.Add(new(i%4==0?"mushrooms":"fern",x,y));
            }
        }
    }
    private void MakeLayout()
    {
        var rows=new string[Height];
        for(int y=0;y<Height;y++){var row=new char[Width];for(int x=0;x<Width;x++)row[x]=Floor(x,y)?'.':'#';rows[y]=new string(row);}
        Layout=new CaveLayout {Name="Mountain region",Rows=rows,Props=_props.ToArray()};
    }
    public void InstallGarage(GarageDefinition definition)
    {
        if(Garage!=null)throw new InvalidOperationException("One garage per review profile.");
        var yard=_landmarks.Single(n=>n.Kind=="yard");
        Garage=new(definition,yard.Cell,_sources);
        var connection=_routes.Single(r=>r.To==yard.Id&&r.Kind=="road").Cells.Reverse().First(p=>!Garage.Contains(p.X,p.Y,2));
        _decor.RemoveAll(p=>Garage.Contains(CellAt(p.X,p.Y).X,CellAt(p.X,p.Y).Y,2));
        for(int y=Garage.Origin.Y;y<Garage.Origin.Y+definition.Height;y++)
        for(int x=Garage.Origin.X;x<Garage.Origin.X+definition.Width;x++)
        {
            if(x<=0||y<=0||x>=Width-1||y>=Height-1)throw new ArgumentException("Garage leaves generated region.");
            Ground[x,y]=Garage.Wall(x,y)?GroundKind.Rock:GroundKind.Gravel;
        }
        var budgets=definition.Objects.Select(o=>o.FromBudget).Where(s=>s!="").ToHashSet();
        _sources.RemoveAll(s=>budgets.Contains(s.Site.Id));
        _sources.AddRange(Garage.Objects.Select(o=>o.Source));
        BakeNavigation();
        var road=Path(connection,Garage.Entry);
        if(road.Count==0)throw new ArgumentException("Garage apron cannot connect to road.");
        foreach(var p in road)if(!Garage.Building(p.X,p.Y))Disc(p.X,p.Y,2,GroundKind.Road,true);
        _routes.Add(new("garage-apron","bridge","yard","road",road));
        foreach(var p in new[]{new GridPoint(Garage.Origin.X,Garage.Origin.Y+13),new GridPoint(Garage.Origin.X+22,Garage.Origin.Y+15)})
            _decor.Add(new("fern",p.WorldX,p.WorldY));
        BakeNavigation();MakeLayout();
        foreach(var o in Garage.Objects)if(Path(Entrance.Cell,o.Approach).Count==0)throw new ArgumentException("Garage object unreachable: "+o.Id);
    }
    public static GridPoint CellAt(double x,double y)=>new((int)Math.Floor(x/16),(int)Math.Floor(y/16));
    private static readonly GridPoint[] Steps={new(1,0),new(0,1),new(-1,0),new(0,-1),new(1,1),new(1,-1),new(-1,1),new(-1,-1)};
    public IReadOnlyList<GridPoint> Path(GridPoint start,GridPoint goal,bool diagonals=true)
    {
        if(!Walkable(start)||!Walkable(goal))return Array.Empty<GridPoint>();
        var frontier=new PriorityQueue<GridPoint,double>();var cost=new Dictionary<GridPoint,double>{{start,0}};var previous=new Dictionary<GridPoint,GridPoint>();frontier.Enqueue(start,0);
        while(frontier.TryDequeue(out var p,out _))
        {
            if(p==goal)
            {
                var result=new List<GridPoint>{p};while(previous.TryGetValue(p,out var before)){p=before;result.Add(p);}result.Reverse();return result;
            }
            foreach(var step in Steps.Take(diagonals?8:4))
            {
                var n=new GridPoint(p.X+step.X,p.Y+step.Y);bool diagonal=step.X!=0&&step.Y!=0;
                if(!Walkable(n)||diagonal&&(!Walkable(new(p.X+step.X,p.Y))||!Walkable(new(p.X,p.Y+step.Y))))continue;
                double next=cost[p]+(diagonal?Math.Sqrt(2):1)*16;
                if(cost.TryGetValue(n,out double known)&&known<=next)continue;
                cost[n]=next;previous[n]=p;frontier.Enqueue(n,next+(Math.Abs(n.X-goal.X)+Math.Abs(n.Y-goal.Y))*(diagonals?11:16));
            }
        }
        return Array.Empty<GridPoint>();
    }
    public Dictionary<GridPoint,double> Distances(GridPoint start)
    {
        var result=new Dictionary<GridPoint,double>();if(!Walkable(start))return result;
        var queue=new Queue<GridPoint>();queue.Enqueue(start);result[start]=0;
        while(queue.TryDequeue(out var p))foreach(var step in Steps.Take(4))
        {var n=new GridPoint(p.X+step.X,p.Y+step.Y);if(Walkable(n)&&!result.ContainsKey(n)){result[n]=result[p]+16;queue.Enqueue(n);}}
        return result;
    }
    public string AreaName(double x,double y)
    {
        var p=CellAt(x,y);
        if(IsSurface(x,y))return _landmarks.OrderBy(n=>Math.Pow(n.Cell.X-p.X,2)+Math.Pow(n.Cell.Y-p.Y,2)).First().Name;
        return Definition.Chambers.OrderBy(n=>Math.Pow(n.X-p.X,2)+Math.Pow(n.Y-p.Y,2)).First().Name;
    }
}
