using System;
using System.Text.Json;

namespace Hollowlight.Core;

public enum DayPhase { Day, Dusk, Night }
public sealed record ClockDefinition(double DaySeconds = 360, double DuskSeconds = 60, double NightSeconds = 180)
{
    public static ClockDefinition Parse(string json) => JsonSerializer.Deserialize<ClockDefinition>(json,
        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? throw new JsonException("Missing clock definition.");
}
public sealed record ClockState(long ElapsedTicks, double FractionalTick = 0);
public sealed record ClockSnapshot(long ElapsedTicks, long CycleTicks, long DayNumber, DayPhase Phase,
    long PhaseElapsedTicks, long PhaseDurationTicks)
{
    public double TotalSeconds => (double)ElapsedTicks / TimeSpan.TicksPerSecond;
    public double PhaseElapsedSeconds => (double)PhaseElapsedTicks / TimeSpan.TicksPerSecond;
    public double PhaseRemainingSeconds => (double)(PhaseDurationTicks - PhaseElapsedTicks) / TimeSpan.TicksPerSecond;
}

// Authoritative elapsed simulation time, independent of wall clock and presentation.
// Serializable state includes the fractional tick so subdivided updates retain time.
public sealed class GameClock
{
    private readonly long _dayTicks, _duskTicks, _nightTicks;
    private long _elapsedTicks;
    private object _advanceGate = new();
    private Action<long>? _advanced;
    private Func<long,long>? _limit;
    public bool Frozen { get; private set; }
    internal void Freeze() => Frozen=true;
    internal void BindAdvanceLimit(object gate,Func<long,long> limit)
    {
        if(!ReferenceEquals(_advanceGate,gate) || _limit!=null)throw new InvalidOperationException("Invalid clock limit owner.");
        _limit=limit;
    }
    // Bind at session initialization. Time advancement and inventory/meal commands
    // subsequently share the same authority lock, including direct developer skips.
    internal void BindAuthority(object gate, Action<long> advanced)
    {
        if (_advanced != null && !ReferenceEquals(_advanceGate, gate)) throw new InvalidOperationException("Clock already has another authority.");
        _advanceGate = gate; _advanced += advanced;
    }
    private double _fractionalTick;
    public long CycleTicks { get; }
    public long ElapsedTicks { get { lock (_advanceGate) return _elapsedTicks; } }
    public GameClock(ClockDefinition definition, ClockState? initial = null)
    {
        _dayTicks = Duration(definition.DaySeconds);
        _duskTicks = Duration(definition.DuskSeconds);
        _nightTicks = Duration(definition.NightSeconds);
        CycleTicks = checked(_dayTicks + _duskTicks + _nightTicks);
        if (initial != null)
        {
            if (initial.ElapsedTicks < 0 || !double.IsFinite(initial.FractionalTick) || Math.Abs(initial.FractionalTick) > 0.5)
                throw new ArgumentOutOfRangeException(nameof(initial));
            _elapsedTicks = initial.ElapsedTicks; _fractionalTick = initial.FractionalTick;
        }
    }
    private static long Duration(double seconds)
    {
        if (!double.IsFinite(seconds) || seconds <= 0) throw new ArgumentOutOfRangeException(nameof(seconds));
        long ticks = checked((long)Math.Round(seconds * TimeSpan.TicksPerSecond));
        if (ticks < 1) throw new ArgumentOutOfRangeException(nameof(seconds));
        return ticks;
    }
    public void Advance(double seconds)
    {
        lock (_advanceGate)
        {
            if (!double.IsFinite(seconds) || seconds < 0) throw new ArgumentOutOfRangeException(nameof(seconds));
            if(Frozen)return;
            double ticks = seconds * TimeSpan.TicksPerSecond + _fractionalTick;
            long whole = checked((long)Math.Round(ticks));
            long next = checked(_elapsedTicks + whole);
            long limited=_limit?.Invoke(next)??next;
            _elapsedTicks = limited;
            _fractionalTick = limited==next?ticks-whole:0;
            _advanced?.Invoke(_elapsedTicks);
            if(Frozen)_fractionalTick=0;
        }
    }
    public ClockState SaveState() { lock (_advanceGate) return new(_elapsedTicks, _fractionalTick); }
    public ClockSnapshot Snapshot()
    {
        lock (_advanceGate)
        {
            long inCycle = _elapsedTicks % CycleTicks;
            if (inCycle < _dayTicks) return new(_elapsedTicks, CycleTicks, _elapsedTicks / CycleTicks + 1,
                DayPhase.Day, inCycle, _dayTicks);
            if (inCycle < _dayTicks + _duskTicks) return new(_elapsedTicks, CycleTicks, _elapsedTicks / CycleTicks + 1,
                DayPhase.Dusk, inCycle - _dayTicks, _duskTicks);
            return new(_elapsedTicks, CycleTicks, _elapsedTicks / CycleTicks + 1,
                DayPhase.Night, inCycle - _dayTicks - _duskTicks, _nightTicks);
        }
    }
}
