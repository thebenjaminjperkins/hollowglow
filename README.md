# Hollowlight — The Threshold

Godot .NET/C# prototype of the first approved slice. The creative authority is
[the Vision](Hollowlight_Vision.md); scope follows [the Roadmap](Hollowlight_Roadmap.md)
and [Bootstrap](Hollowlight_Bootstrap.md). The Vision remains unchanged; the Roadmap records approved milestones.

Normal play now uses the approved [cohesive sanctuary art standard](assets/sanctuary/README.md):

```sh
godot --path .
```

The refuge uses matched PNG sprites, modular TileMapLayer terrain and adjustable
lighting. L compares neutral and lamplit views. Earlier scenes remain available
for historical comparisons; M2a adds finite gathering and shared-chest transfers without survival consumption.

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

No weight, encumbrance, crafting, hunger, fuel or power is implemented. Existing
refuge vegetation/workstations remain decorative; the authored surface growth is
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

To launch with these temporary tools in this workspace:

```sh
export PATH="/tmp/hollowlight-dotnet:$PATH"
export DOTNET_ROOT=/tmp/hollowlight-dotnet
export DOTNET_CLI_HOME=/tmp/hollowlight-cli
export NUGET_PACKAGES=/tmp/hollowlight-nuget
export XDG_DATA_HOME=/tmp/hollowlight-data
export XDG_CONFIG_HOME=/tmp/hollowlight-config
export XDG_CACHE_HOME=/tmp/hollowlight-cache
/tmp/hollowlight-godot/Godot_v4.4.1-stable_mono_linux_x86_64/Godot_v4.4.1-stable_mono_linux.x86_64 --path .
```

The isolated NuGet cache was populated from the official editor's bundled packages.
The source project itself uses ordinary package references and does not depend on
these temporary absolute paths.

See [decisions](docs/DECISIONS.md) and [validation notes](docs/PLAYTEST_LOG.md).
