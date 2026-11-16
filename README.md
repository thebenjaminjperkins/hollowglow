# Hollowlight — The Threshold

Godot .NET/C# prototype of the first approved slice. The creative authority is
[the Vision](Hollowlight_Vision.md); scope follows [the Roadmap](Hollowlight_Roadmap.md)
and [Bootstrap](Hollowlight_Bootstrap.md). The Vision remains unchanged; the Roadmap records approved milestones.

Normal play uses the approved [cohesive sanctuary art standard](assets/sanctuary/README.md):

```sh
godot --path .
```

The refuge uses matched PNG sprites, modular TileMapLayer terrain and adjustable
lighting. L compares neutral and lamplit views. Earlier scenes remain available
for historical comparisons; M2a adds finite gathering and shared-chest transfers without survival consumption.

## M5a — Basic crafting and resource investment (approved)

Enable the existing **2-PU workbench**, approach it and press **E / A**. Three
immediate recipes use personal ingredients: Backpack Extension (**8 Scrap + 3
Organic**), Growing-Bed Kit (**12 Scrap + 4 Organic**), and Cave Potato Seed
Propagation (**1 seed + 1 Organic → 2 seeds**). Withdraw communal materials first.

Kits remain transferable. In inventory, select a backpack kit and use **Install /
U / left-stick click** for eight slots; the original six remain reserved for the
future hotbar, with two added storage-only slots. Take a bed kit to the garden
marker and press **E / A** for one shared fifth plot. Existing crop values remain.

See the [crafting/testing guide](docs/M5A_CRAFTING.md) and
[revised-cost daily investment balances](docs/M5A_CRAFTING_BALANCE.md). M4b is
committed as `2690c74` with report correction `ad4684b`; M5a is owner-approved.

## M4b — Radio beacon and rescue (approved)

Power the existing **2-PU beacon** to accumulate seven signal days. Approach the
radio and press **E / A** for contextual progress, ETA and outage information.
Outages have a 0.25-day grace, then lose 0.25 signal-day per outage day, capped at
one day per episode. Short restorations keep episode history; 0.25 uninterrupted
powered day renews grace and cap. Startup waiting has no penalty.

**Rescued** freezes the completed run and offers **New Run** and **Quit**. Existing
Food, Fuel and farming values are unchanged. Finite Fuel can make rescue unreachable
if wasted or exhausted after setbacks; no forced game-over or hidden refill is added.

See the [rules and testing guide](docs/M4B_RESCUE.md),
[combined daily Food/Fuel/rescue ledger](docs/M4B_COMBINED_BALANCE.md), and
[reviewed screenshots](docs/screenshots/rescue/). M4a baseline is committed as
`4cd4ce8`; the owner approved M4b on 2026-11-15.

## M4a — Basic power generation and manual allocation

Withdraw the **two shared Fuel** from the chest, approach the existing generator,
and press **E / A**. Load from your backpack, start it, and select consumers:
**beacon 2 PU, workbench 2 PU, sanctuary lighting 1 PU** against **4 PU** generation.
An over-capacity activation is rejected; powered consumers keep running. Disable
another manually. Local machine interactions open their focused switch.

The five-Fuel tank burns continuously at **0.2 + 0.2 × supplied PU per full day**.
Stopping preserves fractional Fuel; emptying the tank stops supply. Refueling and
restarting are manual. Two seeded finite Fuel caches provide 2–3 near / 3–5 far Fuel,
with no player-count scaling. The panel shows state, load, spare capacity, Fuel,
burn and estimated runtime. Crop growth is unchanged; M4b adds beacon rescue progress. M5a now adds the three approved crafting recipes.

See the [power operation/testing guide](docs/M4A_POWER.md), [daily Fuel balances](docs/M4A_FUEL_BALANCE.md)
and [Fuel art sources/prompts](assets/power/README.md). M4a is preserved in commit `4cd4ce8`.

## M3b — Finite surface Food scavenging

Six sources now appear on the surface: two wild plants and one food container near
the entrance, one plant and two containers farther away. Their approved quantity
ranges produce **8–17 Food per world**, with no automatic player-count scaling.
**E / controller A** instantly gathers what fits; remainders stay at the source.
Picked plants and empty containers remain visible and never respawn during a run.

```sh
godot --path . -- --world-seed=1701
```

Seed 1701 supplies 14 Food; seeds 499 and 143 demonstrate the 8/17 extremes. Seeded
placement is generated on the existing authored terrain; a full procedural terrain
generator is still future work. R resets the entire run to the same initial seeded
supply. Existing crops, hunger, capacities, stacks and starting Food/seeds are unchanged.

See the [scavenging/testing guide](docs/M3B_SCAVENGING.md), [daily seven-day balance
ledger](docs/M3B_FOOD_BALANCE.md), and [sprite sources/prompts](assets/scavenging/README.md).
The ledger covers 2/4/6 players, poor/default/rich worlds, regular/delayed farm care
and a late-expedition stress case. This milestone stops for review before power.

## M3a — Food consumption and hunger

M2b was approved and committed as **`453114a`** before M3a. Players now start with
**two personal Food** and need **one per day**. Open your backpack, focus Food and
click **Eat**, press **F**, or click the controller **right stick (RS)**. At dawn,
only an unmet requirement is automatically eaten from your own pack. Communal
storage is never drawn from automatically.

Missed meals accumulate: one warns, two reduce movement to 90%, and three or more
to 80%. Each satisfied daily requirement recovers **one** accumulated miss by
default; eating cannot instantly clear prolonged hunger. Recovery and penalties
are configurable in `data/hunger.json`. The compact HUD shows the state; inventory
shows today's credit, next dawn, missed-meal score and movement percentage.

See the [meal/recovery rules and testing guide](docs/M3A_HUNGER.md) and the
[timed seven-day 2/4/6-player balance report](docs/M3A_FOOD_BALANCE.md). Six-player
scarcity is accepted for this milestone. The four plots and all crop yields/times
remain unchanged; there is no economy rebalance or additional survival system.

```sh
dotnet build Hollowlight.csproj
dotnet run --project tests/Hollowlight.Core.Tests.csproj -- --food-balance docs/M3A_FOOD_BALANCE.md
godot --headless --fixed-fps 60 --path . res://tools/HungerSmoke.tscn
```

## M2b — Basic agriculture and time

The inventory milestone was committed as **`24b7f70`** before this work. M2b adds
four independent planting plots in the two existing sanctuary garden beds and a
continuous, configurable clock. The default cycle is **6 minutes Day / 1 minute
Dusk / 3 minutes Night**. The compact clock shows day number, phase and elapsed
minutes/seconds within that phase. Time and crops continue during inventory use
and exploration. M3a adds daily hunger; night still adds no enemies.

| Crop | Maturity after planting | Complete harvest |
| --- | --- | --- |
| Cave Potato | 2 full cycles (20 real minutes by default) | 3 Food + 1 Cave Potato Seed |
| Glowcap | 3 full cycles (30 real minutes by default) | 2 Food + 1 Organic Material + 1 Glowcap Seed |

The communal chest starts with **two seeds of each crop**, shared once per run.
Withdraw seeds into your own pack before planting. Approach the front of a bed:
**C / controller LB or RB** selects the crop, and **E / A** plants, inspects growing
crops or harvests mature ones. Corner marks identify the selected planting position.
Plots show seedling, growing and mature sprites. Harvesting requires room for the
**entire** yield, including the returned seed. If it will not fit, the crop remains
mature and untouched. Make room, return, harvest and replant using the returned seed.

**F4** advances a minute; **F5** a full cycle; **Shift+F5** three cycles. **R** resets
time to Day 1, clears crops and hunger, and restores two personal Food, shared
starter seeds and finite salvage. These are developer controls; no crop upkeep is required.

Configuration: `data/clock.json` contains phase durations; `data/farming.json`
contains crop durations, growth-stage threshold, seed costs, yields, starter stock
and per-bed plot offsets. Six personal slots, stack limit ten and communal capacity
are unchanged. Farming's default starter stock needs at least two communal slots;
`WorldConfig.FarmingEnabled` can disable farming for isolated inventory fixtures.
The one-slot inventory test explicitly does so rather than dropping seeds or enlarging storage.

Planting/harvesting share the authoritative inventory lock. Per-plot session/revision
checks prevent duplicate commands without making unrelated plots conflict. Growth
is derived from elapsed integer simulation ticks and planted timestamps. Clock
state and plot snapshots serialize for future save/synchronization work; a persistent
save/load system and network sessions are not implemented in this milestone.

```sh
godot --headless --fixed-fps 60 --path . res://tools/FarmingSmoke.tscn
godot --headless --fixed-fps 60 --path . res://tools/FarmingSmoke.tscn -- --sanctuary-variant
godot --fixed-fps 60 --path . res://tools/FarmingSmoke.tscn -- --capture-farm
```

The scripted playthrough verifies growth during inventory/exploration, yields,
full-pack safety, replanting and reset. Reviewed gameplay captures are in
[docs/screenshots/farming](docs/screenshots/farming/). See the
[farming asset guide](assets/farming/README.md) for preserved source art and processing.
Both authored layouts and modular terrain assembly remain intact; this slice adds
no world generator, cooking, power, watering, diseases or additional crops.

## Prerequisites

- Godot **4.4.1 .NET** editor (the standard editor cannot run C#).
- **.NET 8 SDK**, Git, and optionally VS Code with the recommended extensions.
- Linux or Windows desktop. No third-party art or gameplay packages.

This is a pinned compatibility baseline, not a claim that these are the latest releases.
Godot 4.4 uses .NET 8: https://godotengine.org/releases/4.4/.
Get the matching editor from https://godotengine.org/download/archive/4.4.1-stable/
and the SDK from https://dotnet.microsoft.com/download/dotnet/8.0.
Keep the editor's GodotSharp folder beside its executable.

## Build and test

Open `project.godot` in the matching .NET editor, build the C# solution with the
editor's Build button, and press F5. The project launches directly into The
Threshold. Use `scenes/SanctuaryRoom.tscn` when running the playable scene; the player
scene alone is a reusable component, not a standalone level.

From this directory:

```sh
dotnet build Hollowlight.csproj
dotnet run --project tests/Hollowlight.Core.Tests.csproj
```

The tests compile the actual engine-independent salvage model, use no test packages,
and exit nonzero on failure. VS Code's default build task builds the game; the
“Test resource logic” task runs those tests. Use Godot's debugger for engine errors;
attach VS Code's C# debugger to the running game for managed breakpoints.

With the .NET editor executable available as `godot` (substitute its actual name):

```sh
godot --headless --editor --path . --import
dotnet build Hollowlight.csproj
godot --headless --path . --quit-after 120
godot --headless --fixed-fps 60 --path . res://tools/ThresholdSmoke.tscn -- --playable-refuge
godot --headless --fixed-fps 60 --path . res://tools/ThresholdSmoke.tscn -- --playable-refuge --sanctuary-variant
godot --path .
```

The import command should complete before building. A timed headless launch detects
startup errors; it does not verify visuals or movement. For editor import automation,
add `--quit` if your editor remains running after import.
The separate smoke scene drives the real player through fixed physics ticks and
tests movement, collision, passage traversal, pickup range, storage, debug recovery,
and scene resets. It exits nonzero on assertion failure and prints a final `PASS`.
It uses scripted actor input, so it does not replace a keyboard-feel playtest.

## Controls and smoke test

WASD/arrows or the assigned controller's left stick move with normalized diagonals.
**E / controller A** gathers a nearby resource or opens the communal chest. Chest
interaction opens storage without moving any items. **Tab / controller View** opens
or closes the backpack anywhere. While inventory is open, this actor stands still;
the world and other actors continue normally. Collision remains active.

The backpack shows a **3×2 icon grid**; an opened chest adds a separate **6×4 grid**.
Stack counts appear on icons; hovering/focusing shows the item name. The permanent
HUD contains only a backpack icon and occupied-slot count. Hints appear in context.

| Action | Mouse / keyboard | Controller |
| --- | --- | --- |
| Navigate | Mouse / arrows | D-pad / left stick |
| Select, then place or merge | Click / Enter; drag-and-drop also supported | A |
| Quick-transfer a stack at the chest | Shift-click / Q | X |
| Split and choose quantity | Right-click / Space; arrows or −/+ | Y; D-pad / stick |
| Confirm quantity, then choose destination | Enter / OK | A |
| Cancel selection, then close | Escape; Tab closes directly | B; View closes directly |
| Chest page | Page Up / Page Down / page buttons | LB / RB |

Use **Store all** explicitly to deposit everything that fits. It is clickable or
reachable below the backpack grid with arrow/D-pad navigation and Enter/A. Matching
stacks merge up to ten; whole stacks of different resources swap. A partial stack
needs an empty or matching destination. Overflow remains in its source slot.
Selections and drag previews never remove inventory items before a confirmed command.

Opening the chest requires proximity and interaction. Once open, minor line-of-sight
changes do not revoke access. Leaving interaction range, hiding/removing/disabling
the chest, or exiting the interface closes access and discards selection previews.
The backpack alone never grants access to communal contents.

**R** resets inventories, storage and finite nodes, including an open split preview.
**H** toggles help while inventory is closed; **L** compares lighting. **F1** shows
debug information; **F2/F3** teleport home/outside, closing inventory but preserving supplies.

1. Gather the three finite sites: 12 Scrap, 8 Organic Material and 14 Scrap. Open
   the backpack, inspect icons/counts, move stacks, split a stack and cancel a selection.
2. Return to the chest and interact: no automatic deposit occurs. Try quick transfer,
   manual merging/swapping and Store all. Combined totals remain 26 Scrap / 8 Organic.
3. Test mouse, keyboard and controller navigation. Select a split quantity and either
   click a destination or drag that quantity; release a drag outside to cancel safely.
4. Close with a pending selection. Reopen and reset during a split. Sources/counts
   reset correctly and movement resumes. Walk the passage and repeat on both layouts.
5. Full-storage fixture: run `tools/SuppliesReview.tscn -- --small-storage`. It clones
   configuration in memory, verifies partial deposits/full storage and captures results.
   Core tests also cover full packs, partial merges, conflicts and conservation.

Presentation is configured in `data/inventory_ui.json`: backpack columns, chest
columns/rows, cell size and native PNG icon regions. Layout changes do not alter
six-slot personal capacity or communal slot capacity. Larger chests use pages.
Icons are nearest-filtered AtlasTexture crops from the approved salvage/mushroom
PNGs at native density; the HUD backpack is a crop of the approved character sheet.
No world art was replaced or generated for this UX slice.

Resource definitions, finite quantities, locations and existing sprite assignments
are in `data/supplies.json`. Add definitions/sites there without changing inventory
code. `data/threshold.tres` sets communal slot capacity (default **24**, configurable
1–256), movement/range and decoration seed. Player capacity is **six slots** and
stack size is **ten**. These yields/capacity are review fixtures, not economy balance.
All quantities that do not fit remain at their source; there is no overflow discard.

`SharedSupplies` is the active runtime model (`SalvageState` remains only for historical M1 tests).
It owns immutable definitions and private containers, returns immutable
snapshots and applies each complete mutation under one lock. Commands require a
session/revision token; competing requests or retries using old state are rejected.
The world separately validates actor identity and access. Chest line of sight is
checked when opening; ongoing sessions validate availability and interaction range.
There is no client-side optimistic inventory mutation. Actual networking, transport
retries/authentication and multi-machine behavior remain unimplemented; concurrency
validation uses two actors against the same authoritative model. Slot operations
use the same lock and session/revision token. UI gesture tokens are captured at
selection/pointer-down and stale previews cannot move a replacement stack.

No weight or encumbrance is implemented; M5a adds only three basic crafting recipes. Existing
refuge workstations remain decorative; garden beds now contain functional plots; the authored surface growth is
the finite Organic Material source. Approved art and modular terrain are unchanged.

The normal refuge preserves 384×216 world coverage with a 768×432 logical grid,
2× camera zoom and default 2× display (1536×864). PNG density is two native pixels
per world unit; all assets share the provisional 64-color palette. See the
[production guide](assets/sanctuary/README.md) for source provenance, terrain rules,
lighting, processing checks and review requirements for new assets.

The unchanged layout data drives both original collision and modular terrain
assembly. `--sanctuary-variant` selects the second authored layout:

```sh
godot --path . -- --sanctuary-variant
```

Graphical review tools:

```sh
godot --fixed-fps 60 --path . -- --capture-room
godot --fixed-fps 60 --path . res://tools/RefugeWalkReview.tscn
godot --fixed-fps 60 --path . res://tools/InventoryUxReview.tscn -- --capture-ux
godot --fixed-fps 60 --path . res://tools/SuppliesReview.tscn
godot --fixed-fps 60 --path . res://tools/SuppliesReview.tscn -- --small-storage
```

The first captures gameplay/native/neutral room views and exits. The second drives
real movement in eight directions and captures walk phases at actual gameplay
scale into `user://refuge-walk`. These use scripted input rather than keyboard input.
The supplies review checks real gathering/transfers and captures pack/chest/full-storage
views in `user://supplies-review`; the small-storage fixture does not modify project data.
The UX review injects actual mouse/keyboard/controller events, checks modal ownership,
chest sessions and stale-state behavior, and saves views in `user://inventory-ux`.
Run without `--capture-ux` in headless mode for automatic assertions. Physical
controller hardware has not been tested. Reviewed captures are in
[docs/screenshots/inventory-ux](docs/screenshots/inventory-ux/); M2a captures remain historical.
Historical scenes remain at `scenes/Threshold.tscn`, `tools/ArtBenchmark.tscn`
and `tools/MachineryTrial.tscn`; their lower-resolution art is not the current standard.

## Code layout

- `scenes/`: level entry and reusable player scene.
- `scripts/core/`: authoritative inventory/node state, immutable snapshots and resource definitions.
- `scripts/input/`: actor-owned input source, separated from world rules.
- `scripts/world/`: movement, authored geometry, interaction validation and world owner.
- `scripts/presentation/`: approved sprites/terrain, read-only inventory panel and HUD.
- `data/`: exported tunables; `tests/`: deterministic console assertions.
- `tools/`: scripted input, engine smoke-test runner, and its separate test scene.

Future networking would route actor commands through the world owner; no networking
or multiple-player session behavior is implemented or validated in this slice.

## Validation and local environment

Validated on Linux with Godot 4.4.1 .NET and .NET SDK 8.0.428: game builds without
warnings, deterministic tests pass, editor import and headless launch complete,
and the engine smoke test passes. The running game's refuge and surface views were
captured and inspected. See [validation notes](docs/PLAYTEST_LOG.md) for scope and
remaining owner playtests; Windows and physical-controller testing were not performed.

The tools were initially absent, so official toolchains were installed temporarily
under `/tmp/hollowlight-dotnet` and `/tmp/hollowlight-godot`. They are not committed
or installed system-wide. Git initialization needed execution outside the sandbox's
read-only metadata mount and then succeeded. No existing history or remotes were changed.
Normal prerequisites above are still required on another machine or after `/tmp` cleanup.

For M5a, .NET 8.0.424 is available in `$HOME/.dotnet`; the temporary engine was
restored under `/tmp/hollowlight-godot-m5`. To launch in this workspace:

```sh
export PATH="$HOME/.dotnet:$PATH"
export DOTNET_ROOT="$HOME/.dotnet"
export DOTNET_CLI_HOME=/tmp/hollowlight-m5-cli
export NUGET_PACKAGES="$HOME/.nuget/packages"
export XDG_DATA_HOME=/tmp/hollowlight-data
export XDG_CONFIG_HOME=/tmp/hollowlight-config
export XDG_CACHE_HOME=/tmp/hollowlight-cache
/tmp/hollowlight-godot-m5/Godot_v4.4.1-stable_mono_linux_x86_64/Godot_v4.4.1-stable_mono_linux.x86_64 --path .
```

The isolated NuGet cache was populated from the official editor's bundled packages.
The source project itself uses ordinary package references and does not depend on
these temporary absolute paths.

See [decisions](docs/DECISIONS.md) and [validation notes](docs/PLAYTEST_LOG.md).
