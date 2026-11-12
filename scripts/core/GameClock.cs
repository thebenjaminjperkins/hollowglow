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
    private double _fractionalTick;
    public long CycleTicks { get; }
    public long ElapsedTicks => _elapsedTicks;
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
        if (!double.IsFinite(seconds) || seconds < 0) throw new ArgumentOutOfRangeException(nameof(seconds));
        double ticks = seconds * TimeSpan.TicksPerSecond + _fractionalTick;
        long whole = checked((long)Math.Round(ticks));
        long next = checked(_elapsedTicks + whole);
        _elapsedTicks = next;
        _fractionalTick = ticks - whole;
    }
    public ClockState SaveState() => new(_elapsedTicks, _fractionalTick);
    public ClockSnapshot Snapshot()
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
