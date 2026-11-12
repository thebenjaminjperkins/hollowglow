using System;
using System.Text.Json;
using Hollowlight.Core;

public static class ClockTests
{
    private static void Check(bool value, string message) { if (!value) throw new Exception(message); }
    public static void Run()
    {
        var config = new ClockDefinition(); var clock = new GameClock(config);
        Check(clock.CycleTicks == 600 * TimeSpan.TicksPerSecond && clock.Snapshot().DayNumber == 1, "Default ten-minute cycle starts at Day 1");
        clock.Advance(359.9); Check(clock.Snapshot().Phase == DayPhase.Day, "Day lasts six minutes");
        clock.Advance(0.1); Check(clock.Snapshot().Phase == DayPhase.Dusk && clock.Snapshot().PhaseElapsedTicks == 0, "Dusk boundary exact");
        clock.Advance(60); Check(clock.Snapshot().Phase == DayPhase.Night, "One-minute dusk");
        clock.Advance(180); Check(clock.Snapshot().Phase == DayPhase.Day && clock.Snapshot().DayNumber == 2, "Three-minute night wraps day");
        var subdivided = new GameClock(config);
        for (int i = 0; i < 600 * 60; i++) subdivided.Advance(1d / 60);
        Check(subdivided.ElapsedTicks == 600 * TimeSpan.TicksPerSecond && subdivided.Snapshot().DayNumber == 2,
            "60Hz updates equal one complete cycle without boundary drift");
        var custom = new GameClock(new(6, 1, 3)); custom.Advance(21.5);
        Check(custom.Snapshot().DayNumber == 3 && custom.Snapshot().Phase == DayPhase.Day, "Custom durations and multi-day skips");
        var restored = new GameClock(config, JsonSerializer.Deserialize<ClockState>(JsonSerializer.Serialize(subdivided.SaveState())));
        restored.Advance(60); subdivided.Advance(60);
        Check(restored.SaveState() == subdivided.SaveState(), "Serializable time state resumes consistently");
        foreach (double bad in new[] { -1d, double.NaN, double.PositiveInfinity })
        {
            long before = clock.ElapsedTicks;
            try { clock.Advance(bad); throw new Exception("Invalid clock advance accepted"); }
            catch (ArgumentOutOfRangeException) { }
            Check(clock.ElapsedTicks == before, "Invalid updates do not mutate time");
        }
        try { _ = new GameClock(new(0, 1, 3)); throw new Exception("Zero-length phase accepted"); }
        catch (ArgumentOutOfRangeException) { }
        Console.WriteLine("PASS: configurable clock phases, exact boundaries, 60Hz consistency, multi-day skips and serializable resume");
    }
}
