using System;
using System.Text.Json;

namespace Hollowlight.Core;

public sealed record RescueDefinition(decimal TargetDays = 7, decimal OutageGraceDays = .25m,
    decimal LossPerOutageDay = .25m, decimal MaxEpisodeLossDays = 1, decimal StableRecoveryDays = .25m)
{
    public static RescueDefinition Parse(string json) => JsonSerializer.Deserialize<RescueDefinition>(json,
        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? throw new JsonException("Missing rescue definition.");
    public void Validate()
    {
        if(TargetDays<=0 || TargetDays>100 || OutageGraceDays<0 || MaxEpisodeLossDays<=0 || StableRecoveryDays<=0 ||
            LossPerOutageDay<=0 || LossPerOutageDay>10 || new[]{TargetDays,OutageGraceDays,LossPerOutageDay,MaxEpisodeLossDays,StableRecoveryDays}
                .AnyInvalidPrecision()) throw new ArgumentException("Invalid rescue definition.");
    }
}
internal static class RescuePrecision
{
    internal static bool AnyInvalidPrecision(this decimal[] values)
    {
        foreach(var value in values)if(value*1000000!=decimal.Truncate(value*1000000))return true;
        return false;
    }
}
public enum SignalStatus { Offline, Transmitting, Interrupted, Rescued }
public sealed record RescueSnapshot(SignalStatus Status, decimal SignalDays, decimal RequiredDays, long RemainingTicks,
    long CurrentOutageTicks, long EpisodeOutageTicks, long GraceRemainingTicks, long StableRecoveryRemainingTicks,
    decimal EpisodeLossDays, decimal TotalLossDays, bool Degrading, bool LossCapped, bool Completed, int CompletionCount, decimal MaxEpisodeLossDays, decimal LossPerOutageDay);

// Exact signal credits; one simulation tick = 1,000,000 credits. Short
// restorations earn progress but do not rearm an outage episode's grace/loss cap.
internal sealed class RescueState
{
    internal const decimal Scale = 1000000;
    internal readonly long CycleTicks, GraceTicks, RecoveryTicks;
    internal readonly decimal TargetCredits, MaxLossCredits, LossPerTick;
    internal decimal Signal, EpisodeLoss, TotalLoss;
    internal long Outage, EpisodeOutage, Stable;
    internal bool Started, Transmitting, Completed;
    internal int CompletionCount;
    internal RescueState(RescueDefinition definition,long cycleTicks)
    {
        definition.Validate();CycleTicks=cycleTicks;
        TargetCredits=definition.TargetDays*cycleTicks*Scale;
        MaxLossCredits=definition.MaxEpisodeLossDays*cycleTicks*Scale;
        GraceTicks=checked((long)decimal.Round(definition.OutageGraceDays*cycleTicks));
        RecoveryTicks=checked((long)decimal.Round(definition.StableRecoveryDays*cycleTicks));
        if(RecoveryTicks<1 || TargetCredits<Scale)throw new ArgumentException("Rescue intervals must span simulation ticks.");
        LossPerTick=definition.LossPerOutageDay*Scale;
    }
    internal void SetPowered(bool powered)
    {
        if(powered!=Transmitting)
        {
            if(powered)Outage=0;else Stable=0;
            Transmitting=powered;
        }
    }
    internal void Advance(long ticks,bool powered)
    {
        if(Completed || ticks==0)return;
        SetPowered(powered);
        if(powered)
        {
            Started=true;Outage=0;
            Signal=Math.Min(TargetCredits,Signal+ticks*Scale);
            Stable=(long)Math.Min((decimal)RecoveryTicks,(decimal)Stable+ticks);
            if(Stable>=RecoveryTicks){EpisodeOutage=0;EpisodeLoss=0;}
            if(Signal>=TargetCredits){Completed=true;CompletionCount=1;}
        }
        else if(Started)
        {
            Stable=0;Outage=checked(Outage+ticks);
            long previous=EpisodeOutage;EpisodeOutage=checked(EpisodeOutage+ticks);
            long exposed=Math.Max(0,EpisodeOutage-GraceTicks)-Math.Max(0,previous-GraceTicks);
            decimal lost=Math.Min(Signal,Math.Min(MaxLossCredits-EpisodeLoss,exposed*LossPerTick));
            Signal-=lost;EpisodeLoss+=lost;TotalLoss+=lost;
        }
    }
    internal long NeededTicks => (long)decimal.Ceiling((TargetCredits-Signal)/Scale);
    internal RescueSnapshot Snapshot()
    {
        decimal perDay=CycleTicks*Scale;
        bool capped=EpisodeLoss>=MaxLossCredits;
        return new(Completed?SignalStatus.Rescued:Transmitting?SignalStatus.Transmitting:Started?SignalStatus.Interrupted:SignalStatus.Offline,
            Signal/perDay,TargetCredits/perDay,NeededTicks,Transmitting?0:Outage,EpisodeOutage,
            Math.Max(0,GraceTicks-EpisodeOutage),Math.Max(0,RecoveryTicks-Stable),EpisodeLoss/perDay,TotalLoss/perDay,
            Started&&!Transmitting&&EpisodeOutage>=GraceTicks&&!capped&&Signal>0,capped,Completed,CompletionCount,MaxLossCredits/perDay,LossPerTick/Scale);
    }
}
