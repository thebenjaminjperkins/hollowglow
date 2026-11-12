# M3a: meals and gradual hunger recovery

M2b was approved, revalidated and committed as `453114a` before this milestone.
M3a implements only personal food consumption and hunger. Farming values, four
plots, seeds, inventory sizes, stack limits, gathering and communal capacity remain
unchanged. Food retains its existing icon and resource ID; its definition now lives
in the common resource catalog so hunger does not depend on farming being enabled.

## Playing

Every registered player starts Fed with **two personal Food**, and **0/1 eaten
today**. There is no free credited meal. The first deadline is dawn at the start
of Day 2 (600 seconds by default). Keep Food in your own backpack: storage is never
automatically withdrawn from, even while its interface is open.

Open the backpack with Tab / controller View, focus or select a personal Food slot,
and click **Eat**, press **F**, or click the **right stick (RS)** on the assigned
controller. Each action eats one unit. Edible items are identified in the detail
line. Seeds, Scrap and Organic Material cannot be eaten. Storage items must first
be withdrawn. Existing select, split, merge, swap and quick-transfer controls stay
unchanged. Eating is unavailable during quantity selection.

Manual eating credits the current day. At dawn, automatic eating consumes only
that day's unmet requirement from personal edible stacks, then clears the daily
credit for the next day. Manual eating therefore prevents duplicate automatic
consumption. Once today's requirement is satisfied, additional eating consumes
nothing: no stockpiling tomorrow's credit or eating extra meals to erase hunger.
For configurable multi-unit daily requirements, partial units count toward today's
meal, but an incomplete requirement at dawn still adds one missed meal.

The permanent HUD shows the hunger state. While inventory is open, the footer shows
units eaten/required today, time to the next dawn, accumulated missed meals and the
current movement multiplier. The world, crops and clock continue while inventory
is open; only that actor's movement is held. There is no invulnerability.

## Gradual recovery

The score records **accumulated missed meals**, not a streak that eating erases.
Every incomplete dawn requirement adds one. Completing a daily requirement removes
`recoveryPerMeal` from the score, clamped at zero. Recovery happens immediately when
the final required unit is eaten, whether manually or automatically. A credited
manual meal is not recovered a second time when dawn settles it.

With the default recovery of one, a score of five requires five satisfied daily
requirements to return to zero, assuming no further misses. Eating at score three
leaves two (Starving); the following day's meal leaves one (Hungry); the next leaves
zero (Fed). A meal just before dawn and the following day's meal just after dawn
are two different daily requirements and can each recover once. There is no
additional recovery cooldown and no obligation to repay old Food quantities.

| Accumulated missed meals | State | Movement |
|---:|---|---:|
| 0 | Fed | 100% |
| 1 | Hungry | 100%, warning only |
| 2 | Starving | 90% |
| 3 or more | Critical | 80%, capped |

Penalties depend on the remaining score, not simply on whether today's meal is
credited. Normalized eight-direction movement and collision shapes are unchanged.
There is no stamina, health drain, death, incapacitation, combat or resurrection.

## Configuration and authority

`data/hunger.json` configures `dailyFood`, `startingFood`, `startingResource`,
`recoveryPerMeal`, `starvingAt`, `criticalAt`, `starvingSpeed` and `criticalSpeed`.
Resource definitions in `data/supplies.json` have an optional `edible` flag (false by
default). Every edible unit currently has equal meal value; there is no nutrition
system. Only the existing Food is edible in the live catalog. A second edible type
exists only in test fixtures. Clock phase durations remain in `data/clock.json`.
Configuration is loaded at session start, not dynamically during play.

The engine-independent SharedSupplies owner keeps per-player meal credit, score,
counters and the authoritative meal day. Consumption, meal credit and recovery
share the inventory lock. Manual requests include session/revision, slot and day;
replays, changed slots and old-day requests are rejected. Mouse Eat captures its
request at pointer-down so a deadline during a held click cannot eat a replacement
stack on release. Partial automatic supplies are counted and conserved.

Clock advancement binds to the same owner lock at initialization. Normal frames,
F4, F5, Shift+F5 and direct clock advances settle every crossed dawn in order.
Time passage alone does not invalidate inventory previews; actual Food consumption
does. Initialization validates all personal starter allocations before granting any,
and cannot be repeated within a session. R creates a fresh session, restoring two
Food per actor, empty meal credits, zero missed meals, seeds, plots and salvage.

Immutable hunger snapshots and clock state serialize without rendering references.
This supplies a boundary for future synchronization/save work; persistent saves,
late joining and multiplayer transport are not implemented. Live play still controls
one actor. The core authority and balance fixtures exercise independent groups of
2, 4 and 6 players.

## Testing and balance

```sh
dotnet build Hollowlight.csproj
dotnet run --project tests/Hollowlight.Core.Tests.csproj -- --food-balance docs/M3A_FOOD_BALANCE.md
godot --headless --fixed-fps 60 --path . res://tools/HungerSmoke.tscn
godot --headless --fixed-fps 60 --path . res://tools/HungerSmoke.tscn -- --sanctuary-variant
godot --fixed-fps 60 --path . res://tools/HungerSmoke.tscn -- --capture-hunger
```

For a short manual check, eat once from your backpack, press F5 and confirm that
no second Food was removed. Exhaust your supplies, use Shift+F5 to miss three meals,
and inspect the movement penalty. Grow/harvest a potato with debug skips, then eat:
its first meal should reduce the score by only one. Advance to another day before
eating again. Deposit Food and verify that dawn never draws from the chest. R resets
all state. F4 advances a minute without pausing the world.

[The seven-day ledger](M3A_FOOD_BALANCE.md) uses actual core operations and finite
planting/harvest/resupply schedules. Regular care produces 26 farm Food; delayed
care produces 20. Two and four players meet every deadline in both tested schedules;
six players miss eight and ten individual meals respectively. Six-player scarcity
is an accepted limitation. No starting-stock or crop rebalance was made; future
surface scavenging, outdoor agriculture and infrastructure are separate milestones.

Baseline engine fixtures explicitly disable hunger to retain their original stock
and timing assertions. HungerSmoke separately runs with the normal enabled defaults,
covering UI inputs, dawn/recovery, movement, farming, stale mouse gestures and reset.
Actual gameplay-scale captures are in [screenshots/hunger](screenshots/hunger/).
Physical controller hardware and networked play have not been tested.
