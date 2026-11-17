using System;
using System.Collections.Generic;
using System.Linq;

namespace Hollowlight.Core;

public sealed record ExplorationDefinition(string Id,string Name,string Kind,string NodeId,string ResourceId,
    int[] Stages,string ToolResource="",double Seconds=.8);
public sealed record ExplorationSnapshot(ExplorationDefinition Definition,SupplyVersion Version,bool Opened,int Stage,int Remaining,int? Worker)
{
    public bool RestorationAvailable=>Definition.Kind=="vehicle"&&Stage==0;
}
public enum ExplorationStatus { Applied, Stale, Empty, NoCapacity, NoTool, Busy, ConsentRequired, Invalid, AccessDenied, Working }
public sealed record ExplorationResult(ExplorationStatus Status,int Amount=0,Guid Ticket=default);

public sealed partial class SharedSupplies
{
    private sealed class ObjectState { public ExplorationDefinition Definition=null!;public long Revision;public bool Opened;public int Stage; }
    private sealed class Work {public Guid Id=Guid.NewGuid();public string Object="";public int Actor;public long Elapsed;}
    private readonly Dictionary<string,ObjectState> _objects=new();
    private readonly Dictionary<string,Work> _work=new();
    public void InitializeExploration(IEnumerable<ExplorationDefinition> definitions)
    {
        lock(_gate)
        {
            if(_objects.Count>0)throw new InvalidOperationException("Exploration initializes once.");
            foreach(var d in definitions)
            {
                if(!_nodes.TryGetValue(d.NodeId,out var node)||node.ResourceId!=d.ResourceId||d.Seconds<=0||
                    d.Kind is not ("container" or "tool-rack" or "machinery" or "vehicle")||
                    (d.Stages.Length>0&&(d.Stages.Any(n=>n<1)||d.Stages.Sum()!=node.Quantity))||
                    (d.ToolResource!=""&&!_resources.ContainsKey(d.ToolResource)))throw new ArgumentException("Invalid exploration definition.");
                _objects.Add(d.Id,new(){Definition=d});
            }
        }
    }
    public ExplorationSnapshot InspectObject(string id)
    {
        lock(_gate)
        {
            var o=_objects[id];return new(o.Definition,new(_session,o.Revision),o.Opened,o.Stage,_remaining[o.Definition.NodeId],_work.TryGetValue(id,out var w)?w.Actor:null);
        }
    }
    public ExplorationResult SearchObject(int actor,string id,SupplyVersion expected)
    {
        lock(_gate)
        {
            if(RunCompleted||!_players.ContainsKey(actor))return new(ExplorationStatus.AccessDenied);
            if(!_objects.TryGetValue(id,out var o)||o.Definition.Kind is not ("container" or "tool-rack"))return new(ExplorationStatus.Invalid);
            if(expected!=new SupplyVersion(_session,o.Revision))return new(ExplorationStatus.Stale);
            if(!o.Opened){o.Opened=true;o.Revision++;}return new(ExplorationStatus.Applied);
        }
    }
    public ExplorationResult TakeObject(int actor,string id,int requested,SupplyVersion inventory,SupplyVersion source,int destination=-1)
    {
        lock(_gate)
        {
            if(RunCompleted||!_players.TryGetValue(actor,out var bag))return new(ExplorationStatus.AccessDenied);
            if(!_objects.TryGetValue(id,out var o)||!o.Opened||o.Definition.Kind is not ("container" or "tool-rack")||requested<1)return new(ExplorationStatus.Invalid);
            if(inventory!=Version||source!=new SupplyVersion(_session,o.Revision))return new(ExplorationStatus.Stale);
            string resource=o.Definition.ResourceId;
            int room=bag.RoomFor(resource);
            if(destination>=0)
            {
                if(!bag.Valid(destination))return new(ExplorationStatus.Invalid);
                var stack=bag.At(destination);
                if(stack!=null&&stack.ResourceId!=resource)return new(ExplorationStatus.Invalid);
                room=StackLimit-(stack?.Quantity??0);
            }
            int amount=Math.Min(requested,Math.Min(room,_remaining[o.Definition.NodeId]));
            if(amount==0)return new(_remaining[o.Definition.NodeId]==0?ExplorationStatus.Empty:ExplorationStatus.NoCapacity);
            if(destination<0)bag.Add(resource,amount);else bag.Set(destination,new(resource,(bag.At(destination)?.Quantity??0)+amount));
            _remaining[o.Definition.NodeId]-=amount;_revision++;o.Revision++;
            return new(ExplorationStatus.Applied,amount);
        }
    }
    public ExplorationResult ReturnTool(int actor,string id,SupplyVersion inventory,SupplyVersion source)
    {
        lock(_gate)
        {
            if(RunCompleted||!_players.TryGetValue(actor,out var bag))return new(ExplorationStatus.AccessDenied);
            if(!_objects.TryGetValue(id,out var o)||o.Definition.Kind!="tool-rack")return new(ExplorationStatus.Invalid);
            if(inventory!=Version||source!=new SupplyVersion(_session,o.Revision))return new(ExplorationStatus.Stale);
            if(_remaining[o.Definition.NodeId]!=0||bag.Count(o.Definition.ResourceId)<1)return new(ExplorationStatus.Invalid);
            bag.Remove(o.Definition.ResourceId,1);_remaining[o.Definition.NodeId]=1;_revision++;o.Revision++;
            return new(ExplorationStatus.Applied,1);
        }
    }
    public ExplorationResult BeginDismantle(int actor,string id,SupplyVersion inventory,SupplyVersion source,bool consent)
    {
        lock(_gate)
        {
            if(RunCompleted||!_players.TryGetValue(actor,out var bag))return new(ExplorationStatus.AccessDenied);
            if(!_objects.TryGetValue(id,out var o)||o.Definition.Stages.Length==0)return new(ExplorationStatus.Invalid);
            if(inventory!=Version||source!=new SupplyVersion(_session,o.Revision))return new(ExplorationStatus.Stale);
            if(_work.ContainsKey(id)||_work.Values.Any(w=>w.Actor==actor))return new(ExplorationStatus.Busy);
            if(o.Stage>=o.Definition.Stages.Length)return new(ExplorationStatus.Empty);
            if(o.Definition.ToolResource!=""&&bag.Count(o.Definition.ToolResource)==0)return new(ExplorationStatus.NoTool);
            if(o.Definition.Kind=="vehicle"&&o.Stage==0&&!consent)return new(ExplorationStatus.ConsentRequired);
            if(bag.RoomFor(o.Definition.ResourceId)<o.Definition.Stages[o.Stage])return new(ExplorationStatus.NoCapacity);
            var work=new Work{Actor=actor,Object=id};_work[id]=work;o.Revision++;
            return new(ExplorationStatus.Working,Ticket:work.Id);
        }
    }
    public ExplorationResult AdvanceDismantle(int actor,Guid ticket,long activeTicks)
    {
        lock(_gate)
        {
            var work=_work.Values.FirstOrDefault(w=>w.Id==ticket);
            if(work==null)return new(ExplorationStatus.Stale);
            if(work.Actor!=actor)return new(ExplorationStatus.AccessDenied);
            var o=_objects[work.Object];var bag=_players[actor];
            if(activeTicks<0)return new(ExplorationStatus.Invalid);
            if(RunCompleted){CancelDismantle(actor,ticket);return new(ExplorationStatus.AccessDenied);}
            if(o.Definition.ToolResource!=""&&bag.Count(o.Definition.ToolResource)==0){CancelDismantle(actor,ticket);return new(ExplorationStatus.NoTool);}
            work.Elapsed=checked(work.Elapsed+activeTicks);
            if(work.Elapsed<(long)(o.Definition.Seconds*TimeSpan.TicksPerSecond))return new(ExplorationStatus.Working,Ticket:ticket);
            int amount=o.Definition.Stages[o.Stage];
            if(bag.RoomFor(o.Definition.ResourceId)<amount){CancelDismantle(actor,ticket);return new(ExplorationStatus.NoCapacity);}
            bag.Add(o.Definition.ResourceId,amount);_remaining[o.Definition.NodeId]-=amount;o.Stage++;o.Revision++;_revision++;_work.Remove(work.Object);
            return new(ExplorationStatus.Applied,amount);
        }
    }
    public void CancelDismantle(int actor,Guid ticket)
    {
        lock(_gate)
        {
            var w=_work.Values.FirstOrDefault(w=>w.Id==ticket&&w.Actor==actor);
            if(w!=null){_work.Remove(w.Object);_objects[w.Object].Revision++;}
        }
    }
}
