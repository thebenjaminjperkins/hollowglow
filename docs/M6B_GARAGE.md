# M6b — Cinder Maintenance Garage

Implemented as a separate review profile. The compact default, alternate compact
fixture and `mountain-region` profile remain available. This milestone adds one
location, not new survival mechanics or revised economy values.

## Run

From the repository root, using Godot **4.4.1 .NET**:

```sh
godot --path . -- --world-layout=garage-expedition --world-seed=1701
```

If `godot` is not on PATH, this workspace currently has a temporary editor:

```sh
export PATH="$HOME/.dotnet:$PATH"
export DOTNET_ROOT="$HOME/.dotnet"
export DOTNET_CLI_HOME=/tmp/hollowlight-m5-cli
export NUGET_PACKAGES="$HOME/.nuget/packages"
export XDG_DATA_HOME=/tmp/hollowlight-data
export XDG_CONFIG_HOME=/tmp/hollowlight-config
export XDG_CACHE_HOME=/tmp/hollowlight-cache
GODOT_BIN=/tmp/hollowlight-godot-m6b/Godot_v4.4.1-stable_mono_linux_x86_64/Godot_v4.4.1-stable_mono_linux.x86_64
"$GODOT_BIN" --path . -- --world-layout=garage-expedition --world-seed=1701
```

That editor is not a permanent installation and may disappear after `/tmp` cleanup.
The project needs the .NET build of Godot, not the standard editor.

## Playtest route and controls

Leave through the sanctuary threshold and follow the surface road toward the
**Cinder freight yard**. The connected apron enters the roofless maintenance
garage: an office and side circulation on the left, parts/tool area in the middle,
service bay with van, compressor and fuel cabinet on the right. No teleport or
special travel menu is needed. F3 remains a developer surface shortcut, outside
normal expedition measurements.

- Move with the existing keyboard/controller controls; diagonal movement remains.
- Approach containers and press **E / A** to search. The two-panel slot interface
  shows personal inventory and take-only location contents. Click/select and place,
  split with right-click/Space/Y, quick-take with Q/Shift-click/X, or choose
  **Take all**. Leftovers remain in the container. Escape/B cancels selection first,
  then closes; Tab/Back closes. Existing backpack/chest behavior is unchanged.
- Take the **salvage tool** from the workshop rack with E/A. It occupies a normal
  slot, is reusable, has no durability, and must be in personal inventory to work.
  Return it to the empty rack with E/A or explicitly deposit it in communal storage.
  Other actors can withdraw/use it. Only this prototype location guarantees a tool;
  the resource/transfer model permits future crafting or other acquisition.
- Hold **E / A** at the compressor to remove its one 2-Scrap part (0.8 seconds).
  Release, request movement, leave range or cancel to interrupt. Whole-part capacity
  is required both before work and at completion; no half-part loss.
- Inspect the van. **Preserve / Back** is the default and changes nothing.
  **Choose dismantling…** arms the choice; then hold E/A at the vehicle. The first
  completed 1.2-second removal irreversibly ends its restoration opportunity.
  A second hold removes the remaining part. Each yields 2 Scrap. Cancelling the
  first unfinished removal leaves preservation possible. Already completed parts
  remain removed. Restoration is unimplemented and the choice is not claimed balanced.
- Return to the chest to deposit. Six slots, stack limit ten, the existing backpack
  extension, food, farming, hunger, power, fuel and rescue rules remain intact.
- **R** starts a fresh run, restoring sources, tool, van opportunity and exploration.
  Travel/time do not restore objects; there is no new disk save/load system.

## Resource conservation

| Existing allocation | Garage representation | Quantity |
|---|---|---:|
| Machine housing, 14 Scrap | Parts cabinet | 4 Scrap |
| Same allocation | Bench crate | 4 Scrap |
| Same allocation | Compressor | 2 Scrap |
| Same allocation | Van, two removals | 2 + 2 Scrap |
| Far Food container #1 | Office cupboard | Original seeded roll, 2–4 Food |
| Far Fuel cache #0 | Fuel cabinet | Original seeded roll, 3–5 Fuel |

The three original sources are replaced, not retained alongside these objects.
Other sources are unchanged. World materials remain **26 Scrap and 8 Organic**;
preserving the van makes only 22 Scrap currently recoverable. No extra Food or Fuel
is generated. One approved fixture tool is the only added item. The accepted
14-day Python study remains separate and has not been imported into gameplay.

## Discovery and authority

`WorldDiscovery` is a reusable session model over world opacity, not a garage mask.
Explored geography is shared; current line of sight belongs to each observer.
Walls occlude sight and prevent diagonal corner peeking. Leaving sight preserves
dim terrain memory, but dynamic objects are hidden rather than showing their
current container, crop, machine or vehicle state remotely. Current implementation
uses object foot anchors for visibility, so large sprites can appear/disappear as
their anchor enters/leaves sight. There is no separate minimap in this slice.

`SharedSupplies` owns finite quantities, inventory transactions, per-object revisions,
exclusive work tickets, stages and discovery session identity under its existing
authority lock. Work commits once, rejects stale/replayed operations, rechecks tool
and capacity, and does not progress from developer clock skips. Different actors
can work different objects concurrently in model tests. Scene code adds proximity,
line-of-sight and held-input validation. No networking or live multiplayer added.

The [modular art workflow](../assets/garage/README.md) uses shared-palette PNG state
sheets, atlas floors and Y-sorted wall modules. Colliders stay in world units and
remain after depletion, including the stripped vehicle chassis. Camera coverage,
movement speed, interaction range and all other locations' footprints are unchanged.

## Validation and review

```sh
dotnet build Hollowlight.csproj --no-restore
dotnet run --project tests/Hollowlight.Core.Tests.csproj
python tools/art/check_garage.py
python tools/art/check_region.py
python tools/art/check_sanctuary.py
godot --headless --fixed-fps 60 --path . res://tools/GarageSmoke.tscn -- --world-seed=1701
godot --headless --fixed-fps 60 --path . res://tools/GarageSmoke.tscn -- --world-seed=499
godot --headless --fixed-fps 60 --path . res://tools/GarageSmoke.tscn -- --world-seed=8
godot --headless --fixed-fps 60 --path . res://tools/GarageSmoke.tscn -- --preserve-van
```

Checks cover 32 seeded layouts, original budgets, reachable approaches and road
connection; full/partial collection, stale requests, capacity at work commit, tool
transfer/loss, van consent/cancellation/stages, conflicting actors, shared discovery,
reset and legacy regressions. [Runtime evidence](measurements/garage/runtime-validation.json)
records 23 successful engine runs: nine legacy tools in both compact layouts,
three garage seeds, preservation branch and preserved M6a region navigation.
Keyboard/controller events are simulated; physical controller and multiplayer
human play remain unverified.

See [measured expedition breakdown](M6B_TRAVEL.md) and
[gameplay-scale screenshots](screenshots/garage/README.md). Efficient scripted
trips are shorter than the 2–4-minute first-visit target; no delays were added.
Please judge navigation, visual cohesion, discovery clarity, carrying decisions,
tool-sharing friction and whether exploration is interesting before further scope.
