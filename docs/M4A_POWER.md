# M4a: basic power generation and manual allocation

M3b's accepted baseline is committed as `5e6abff`. M4a adds only power operation,
manual allocation and finite Fuel logistics. Rescue progress remains M4b; farming,
hunger, Food/material sources, inventory capacity and starting Food/seeds are unchanged.

## Start and operate

The generator starts stopped, with an empty tank and all consumer switches off.
The communal chest contains **two shared Fuel**, allocated once per session, not
per player. Withdraw it into your personal backpack before loading the generator.

1. Approach the existing generator and press **E / controller A**.
2. Use **Load 1 Fuel** or **Fill tank**. Only personal Fuel is transferred.
3. Select **Start generator**.
4. Switch on the desired consumers. The panel shows load/capacity, available PU,
   generator state, Fuel, burn rate and estimated runtime at the current load.
5. Manually switch something off before enabling a consumer that would exceed capacity.

Mouse clicks, Up/Down + Enter, controller D-pad/stick + A select/apply controls.
Esc/B/Tab/View closes the panel. Opening it holds only its actor stationary; the
world, other actors, clock, crops, meals and generator continue. Collision remains
active. Leaving the station's interaction range closes access. Local interactions
at the workbench, communications shelf and lamp controls open the same panel focused
on that machine's switch. No global remote-control key or automatic priorities exist.

## Approved capacities, consumption and overload

| Device | Demand |
|---|---:|
| Generator | Supplies 4 PU |
| Radio beacon | 2 PU |
| Workbench | 2 PU |
| Supplemental sanctuary lighting | 1 PU |

All three consumers request 5 PU. Enabling the third is rejected with a **5/4 PU**
warning. Existing consumers continue operating; the generator does not trip and
no priority is selected automatically. Rejection changes no switch or Fuel state.
Circuit-breaker faults/damage are not implemented.

While running, burn per full clock cycle is:

`0.2 Fuel + 0.2 Fuel × supplied PU`

| Running load | Fuel per ten-minute cycle |
|---|---:|
| Idle, no consumers | 0.2 |
| Beacon alone, 2 PU | 0.6 |
| Beacon + lights, 3 PU | 0.8 |
| Beacon + workbench, 4 PU | 1.0 |

Off consumes nothing. Fuel is accounted continuously, not deducted or rounded at
dawn. The five-Fuel tank accepts whole inventory units only: a half-unit of free
space cannot consume a whole item. Fill moves only whole units that fit, leaving
all remainder in the backpack. Stopping retains fractional Fuel and selected switches.
Emptying the tank stops generation exactly at zero. It never draws from personal
or communal stock. Refilling does not restart it; restart is manual. Selected
switches are retained but receive no supply while stopped/empty. Start the generator
before enabling an additional consumer; disabling a selected switch remains possible
while stopped. Runtime is shown in gameplay minutes:seconds while running.

The beacon's live diode and panel indicate whether it is powered. There is no
signal-day progress, outage penalty, extraction or rescue countdown. The workbench
has electrical readiness/task lighting only, without crafting production. Supplemental
lamps respond to their consumer; ambient visibility and natural bioluminescence
remain. L is still a lighting-preview control, not a simulation switch.

**Crops remain completely independent of electricity.** Growth, yields, stages,
four plots and seeds are unchanged. No grow light or growth bonus/requirement is added.

## Fuel caches and resource conservation

Two seeded, finite surface caches add **2–3 near Fuel** and **3–5 far Fuel**. Combined
with the two shared starters, total available Fuel is **7–10**. Quantities do not
scale with players. The existing near/far path bands, source spacing, reachable
approaches, instantaneous gathering, partial collection, source remainders and
visible depleted states apply. Nothing respawns during a run; R creates a new run
with the same initial seed/layout/configuration and resets generator state.

The common placement module is now named `FiniteScavenging`; Food's original domain,
IDs, quantity stream and placement rules remain unchanged. Fuel uses its own domain,
respects existing Food as nonblocking ground props, and never consumes Food's random
stream. Full terrain generation remains future work.

Refueling is an authoritative inventory command: item removal and tank credit
addition share SharedSupplies' lock. Power and inventory session/revision tokens
reject stale/refired/competing commands. Mouse commands capture their tokens on
pointer-down; release cannot operate a newer state. Simulation updates and both
meal/power clock callbacks share that same authority lock. Only actual item loading
changes inventory revisions; ongoing burn does not invalidate inventory previews.

Exact Fuel counters store integer credits in `decimal`: one unit is
`cycleTicks × 1,000,000` credits. The configured rates have at most six decimal places;
per-tick burn is integral. Division occurs only for snapshots/UI, not accumulated
state. This preserves fractions across time subdivision, stop/start and depletion.
Immutable snapshots/credit counters serialize without Godot objects. Persistent
saves, multiplayer transport, authentication and late joining are not implemented.

Configuration lives in `data/power.json` and `data/fuel_scavenging.json`. Fuel uses
existing six-slot personal inventories and stack limit ten; it is non-edible.
`WorldConfig.PowerEnabled` disables this milestone for isolated legacy fixtures.
Existing matched machine PNGs are reused; only the small Fuel kit is new. See
[art sources and full prompt](../assets/power/README.md).

## Test and review

Using the temporary tools installed in this workspace, launch from repository root:

```sh
DOTNET_ROOT=/tmp/hollowlight-dotnet /tmp/hollowlight-godot/Godot_v4.4.1-stable_mono_linux_x86_64/Godot_v4.4.1-stable_mono_linux.x86_64 --path . -- --world-seed=1701
```

With the documented tools on PATH:

```sh
dotnet build Hollowlight.csproj
dotnet run --project tests/Hollowlight.Core.Tests.csproj -- --power-balance docs/M4A_FUEL_BALANCE.md
godot --headless --fixed-fps 60 --path . res://tools/PowerSmoke.tscn
godot --headless --fixed-fps 60 --path . res://tools/PowerSmoke.tscn -- --sanctuary-variant
godot --fixed-fps 60 --path . res://tools/PowerSmoke.tscn -- --capture-power
```

Manual review: withdraw/load starter Fuel, start, enable beacon + workbench, then
try lighting. Confirm the 5/4 warning and uninterrupted existing service. Disable
workbench and enable lighting; burn should fall to 0.8/day and runtime increase.
Use F4/F5 to test consumption, stop to preserve Fuel, gather both caches, refill,
check that restarting is manual, and reset with R. Farm with the generator stopped
to confirm unchanged crops. Try a nearly full tank and a full backpack: no Fuel
should be lost. Check local switches and panel cancellation/range exit.

[Seven-day Fuel ledgers](M4A_FUEL_BALANCE.md) use actual seeded minimum/maximum
worlds and authoritative transfers/commands for 2/4/6 actors. Scheduled operation
costs 4.9 Fuel, full load 7. The minimum-stock world finishes full service exactly
empty at the final boundary; delayed scavenging can cause outages. This isolates
power logistics rather than changing the existing Food balance. [Captures](screenshots/power/)
show the inspected panel, overload rejection and cache states at gameplay zoom.

Automated checks cover exact/fractional conservation, rates, depletion, partial
loading, continuous time, seeded finite caches, concurrency, stale commands and
reset. Input-event tests do not substitute for physical controller hardware or
networked/group playtests. M4a stops here for owner review before M4b.
