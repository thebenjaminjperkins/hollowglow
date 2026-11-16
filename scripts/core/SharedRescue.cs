using System;
using System.Linq;

namespace Hollowlight.Core;

public sealed partial class SharedSupplies
{
    private RescueState? _rescue;
    private bool RunCompleted => _rescue?.Completed==true;
    public event Action? RescueCompleted;
    public void InitializeRescue(RescueDefinition definition)
    {
        lock(_gate)
        {
            if(_rescue!=null)throw new InvalidOperationException("Rescue initializes once per session.");
            if(_power==null || _powerClock==null || !_consumers.Any(c=>c.Id=="beacon"))throw new InvalidOperationException("Rescue requires the powered beacon.");
            var candidate=new RescueState(definition,_powerClock.CycleTicks);
            _powerClock.BindAdvanceLimit(_gate,LimitRescueTime);
            _rescue=candidate;SynchronizeRescueSignal();
        }
    }
    public RescueSnapshot Rescue() {lock(_gate)return (_rescue??throw new InvalidOperationException("Rescue not initialized.")).Snapshot();}
    private void SynchronizeRescueSignal() => _rescue?.SetPowered(_running&&_enabledConsumers.Contains("beacon"));
    private long PoweredTicks(long maximum)
    {
        if(!_running || !_enabledConsumers.Contains("beacon"))return 0;
        return (long)Math.Min((decimal)maximum,decimal.Floor(_tankCredits/(BurnRate*1000000)));
    }
    private long LimitRescueTime(long proposed)
    {
        if(RunCompleted)return _powerClock!.ElapsedTicks;
        long start=_powerClock!.ElapsedTicks,delta=proposed-start;
        if(_rescue!.NeededTicks<=PoweredTicks(delta))return checked(start+_rescue.NeededTicks);
        return proposed;
    }
    private void AdvanceRescue(long ticks,bool powered)
    {
        if(_rescue==null || RunCompleted)return;
        _rescue.Advance(ticks,powered);
        if(RunCompleted)
        {
            _powerClock!.Freeze();
            RescueCompleted?.Invoke();
        }
    }
}
