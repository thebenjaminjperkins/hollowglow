using System;
using System.Collections.Generic;
using System.Linq;

namespace Hollowlight.Core;

public sealed record DiscoverySnapshot(Guid Session, IReadOnlyList<GridPoint> Explored, IReadOnlyList<GridPoint> Visible);

// Reusable session geography memory. Exploration is shared; current sight is observer-local.
public sealed class WorldDiscovery
{
    private readonly object _gate;
    private readonly Guid _session;
    private readonly bool[,] _opaque;
    private readonly HashSet<GridPoint> _explored = new();
    private readonly Dictionary<int, HashSet<GridPoint>> _visible = new();
    private readonly Dictionary<int, (GridPoint Cell, int Radius)> _positions = new();
    private long _revision;
    public long Revision {get{lock(_gate)return _revision;}}
    public int Width => _opaque.GetLength(0);
    public int Height => _opaque.GetLength(1);
    public WorldDiscovery(bool[,] opaque) : this(opaque, Guid.NewGuid(), new object()) { }
    internal WorldDiscovery(bool[,] opaque, Guid session, object gate)
    { _opaque = (bool[,])opaque.Clone(); _session = session; _gate = gate; }
    public bool Explored(GridPoint p) { lock (_gate) return _explored.Contains(p); }
    public bool Visible(int actor, GridPoint p) { lock (_gate) return _visible.TryGetValue(actor, out var cells) && cells.Contains(p); }
    public void RemoveObserver(int actor) { lock (_gate) { if(_visible.Remove(actor))_revision++; _positions.Remove(actor); } }
    public DiscoverySnapshot Snapshot(int actor)
    {
        lock (_gate) return new(_session, _explored.OrderBy(p=>p.Y).ThenBy(p=>p.X).ToArray(),
            _visible.TryGetValue(actor,out var cells)?cells.OrderBy(p=>p.Y).ThenBy(p=>p.X).ToArray():Array.Empty<GridPoint>());
    }
    public void Observe(int actor, GridPoint origin, int radius)
    {
        if(actor<=0 || radius<1 || radius>64)throw new ArgumentException("Invalid discovery observer.");
        lock (_gate)
        {
            if(_positions.TryGetValue(actor,out var previous)&&previous==(origin,radius))return;
            var visible=new HashSet<GridPoint>();
            for(int y=Math.Max(0,origin.Y-radius);y<Math.Min(Height,origin.Y+radius+1);y++)
            for(int x=Math.Max(0,origin.X-radius);x<Math.Min(Width,origin.X+radius+1);x++)
            {
                var target=new GridPoint(x,y);
                if((x-origin.X)*(x-origin.X)+(y-origin.Y)*(y-origin.Y)<=radius*radius && Sight(origin,target))visible.Add(target);
            }
            _visible[actor]=visible;_positions[actor]=(origin,radius);_explored.UnionWith(visible);_revision++;
        }
    }
    private bool Opaque(int x,int y)=>x<0||y<0||x>=Width||y>=Height||_opaque[x,y];
    private bool Sight(GridPoint a,GridPoint b)
    {
        int x=a.X,y=a.Y,dx=Math.Abs(b.X-x),dy=-Math.Abs(b.Y-y),sx=x<b.X?1:-1,sy=y<b.Y?1:-1,error=dx+dy;
        while(x!=b.X||y!=b.Y)
        {
            int twice=2*error,nx=x,ny=y;
            if(twice>=dy){error+=dy;nx+=sx;}
            if(twice<=dx){error+=dx;ny+=sy;}
            if(nx!=x&&ny!=y&&(Opaque(nx,y)||Opaque(x,ny)))return false;
            x=nx;y=ny;
            if(x==b.X&&y==b.Y)return true; // The blocking wall itself is visible.
            if(Opaque(x,y))return false;
        }
        return true;
    }
}

public sealed partial class SharedSupplies
{
    public WorldDiscovery? Discovery {get;private set;}
    public void InitializeDiscovery(bool[,] opaque)
    { lock(_gate) { if(Discovery!=null)throw new InvalidOperationException("Discovery initializes once.");Discovery=new(opaque,_session,_gate); } }
}
