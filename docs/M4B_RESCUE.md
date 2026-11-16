# M4b: Radio beacon and rescue

Implemented and validated 2026-11-13; approved by the owner 2026-11-15. M4a was committed first as `4cd4ce8`;
the working tree was clean before M4b. Its final owner hands-on confirmation remains
separate from reported validation. The accepted M4b milestone is preserved in Git history.

## Rules

Configuration: [data/rescue.json](../data/rescue.json). One day always means one
complete configured clock cycle, currently 600 seconds, including Day/Dusk/Night.

| Setting | Default | Meaning |
|---|---:|---|
| targetDays | 7 | Net accumulated powered transmission required |
| outageGraceDays | 0.25 | 150 seconds of accumulated off-time per outage episode |
| lossPerOutageDay | 0.25 | Continuous signal-day loss per elapsed off-day after grace |
| maxEpisodeLossDays | 1 | Maximum actual loss per episode |
| stableRecoveryDays | 0.25 | Uninterrupted powered time needed to renew episode grace and cap |

Before the first actual transmission, waiting causes no outage penalty. There is
no start deadline. The beacon consumes the existing **2 PU**. Only supplied time
earns progress, at one signal second per powered second. Switching on or restoring
Fuel/start resumes accumulation immediately; recovery does not delay earning signal.

After grace, loss is `min(progress, episode cap remaining, exposed off-time × rate)`.
Progress cannot be negative. A full-day outage loses 0.1875 signal-day after grace;
an outage of 4.25 days reaches the one-day cap if enough signal exists. Further
uninterrupted downtime cannot cause more loss or a full reset. Actual loss at zero
is zero; no hidden penalty debt accrues.

**Anti-exploit rule:** A restoration shorter than 0.25 day pauses the penalty and
earns signal normally, but does not erase accumulated off-time, replenish grace,
or renew the loss cap. Those interruptions belong to one outage episode. Only
150 uninterrupted powered seconds renew both. The current outage timer resets on
restoration; episode off-time/loss remain until this healthy window completes.
Eight 30-second outages separated by one-second restorations therefore use 240
seconds of off-time, not eight fresh grace periods. Stable recovery is configurable.
There is no punitive three-day reset.

## Playing and testing

1. Launch normally (`godot --path .` with a Godot .NET executable on PATH; see the
   README's temporary-toolchain command if `godot` is unavailable).
2. Withdraw Fuel from the chest; approach the generator and press **E / A**. Load
   personal Fuel and start it. Enable the beacon, leaving optional loads off to
   conserve Fuel. Refills and restarts remain manual; tank capacity remains five.
3. Approach the existing communications shelf/radio and press **E / A**. Its
   contextual panel shows status, signal/target, conditional ETA or remaining
   required transmission, current outage, unused grace, episode loss and time to
   renew grace. It links to existing power controls. No permanent signal HUD meter.
4. Use up/down or assigned controller stick/D-pad to select, Enter/A to apply,
   mouse buttons to activate, Esc/B to close. Leaving range closes the panel.
   The menu holds its owner still while time, meals, growth and Fuel continue.
5. **F4** adds 60 seconds, **F5** one cycle, **Shift+F5** three cycles. Watch Fuel:
   a large skip stops earning transmission at exhaustion, even if it spans days.
   Refuel/restart to recover. Never-started delay remains penalty-free.
6. Test a short outage (under 150 seconds), a longer outage, rapid off/on blips,
   then 150 seconds of stable transmission. The world diode is green transmitting,
   amber interrupted, brown before initial service. Active degradation warnings
   take precedence over transient switch confirmations.
7. Reach seven net signal days. **Rescued** freezes simulation, movement, resource
   commands and progress exactly at completion. New Run reloads fresh initial
   stocks/plots/time; Quit exits. Repeated commands/skips cannot complete twice.
   R is the existing developer reset during ordinary play, not a post-victory action.

The earliest no-delay completion takes 70 real minutes at normal speed. Starting
at D1 00:30 reaches D8 00:30 without interruptions. ETA assumes uninterrupted supply,
not sufficient Fuel or a guaranteed collection schedule.

## Automated checks

```sh
dotnet build Hollowlight.csproj
dotnet run --project tests/Hollowlight.Core.Tests.csproj -- --rescue-balance docs/M4B_COMBINED_BALANCE.md
godot --headless --fixed-fps 60 --path . res://tools/RescueSmoke.tscn
godot --headless --fixed-fps 60 --path . res://tools/RescueSmoke.tscn -- --sanctuary-variant
godot --headless --fixed-fps 60 --path . res://tools/RescueSmoke.tscn -- --quit-victory
godot --fixed-fps 60 --path . res://tools/RescueSmoke.tscn -- --capture-rescue
```

The graphical test saves actual 1536×864 gameplay captures into `user://rescue`.
Reviewed copies: [offline](screenshots/rescue/offline.png),
[transmitting](screenshots/rescue/transmitting.png),
[degrading](screenshots/rescue/degrading.png), [victory](screenshots/rescue/rescued.png).
Captures deliberately skip startup days without feeding; Critical hunger in those
images is a test fixture consequence, not a new starting condition.

Core coverage includes configured/default rules, startup, long and repeated
outages, grace/cap recovery, zero floor, exhaustion within time skips, once-only
concurrent completion, exact victory/Fuel boundary, terminal mutations and reset.
Live tests cover keyboard/mouse/controller events, proximity/modal behavior,
continuous menu time, world freeze, New Run and Quit. Existing power, hunger,
Food gathering, farm and movement/collision tests pass on both layouts.

## Economy and limits

[Combined daily ledgers](M4B_COMBINED_BALANCE.md) use actual seeded sources and timed
planting, harvests, food distribution, expeditions and manual Fuel loading for
2/4/6 actors. Scheduled service completes at D8 00:30; poor-world Fuel reserve is
2.24, default-world reserve 3.24. Full load in the poorest world finishes exactly
empty. Six players in the poorest Food world with delayed care miss two meals on
Day 7, while rescue remains attainable. No crop or consumption changes compensate.

**Rescue can become unreachable.** Five days of workbench-only Fuel use in the
seven-Fuel world leaves less than the beacon-only 4.2 Fuel needed. Continuing full
load after a 2.5-day outage likewise exhausts Fuel before the additional lost signal
is recovered. Disabling optional loads earlier can avert that outcome. After all
finite Fuel is spent, no rescue recovery is possible in this slice; there is no
hidden replenishment or arbitrary game-over. Existing developer reset remains.
This is reported for balance review, not silently rebalanced.

## Implementation boundary

`SharedSupplies` owns power, inventory, hunger and rescue under one authority lock.
Exact signal credits use elapsed clock ticks; power splits skips at tank exhaustion.
A clock limit clips every simulation callback to the victory instant, preventing
post-victory Fuel burn, extra meals or growth. Completion latches before notification;
authoritative resource/farm/meal/power commands reject changes afterward. The scene
then pauses and presents an Always-processing terminal UI with guarded transitions.
Snapshots keep presentation separate from rules. Future network/save transport
must still serialize and restore the complete session; it is not implemented here.

All existing PNG artwork, camera coverage, collisions, movement, data stocks, farm
values and hunger rules remain unchanged. No new survival systems, art library,
network session, disk saves, extraction sequence or final defeat rules are included.
Physical controllers and human multiplayer have not been tested. Stop for review.
