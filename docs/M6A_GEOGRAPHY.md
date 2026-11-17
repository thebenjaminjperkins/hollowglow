# M6a: World layout and geography — review profile

Implemented 2026-11-15, awaiting owner visual/gameplay approval. Accepted M5a is
committed as **29c0b79**; M6a remains uncommitted for review. Vision and existing
resource, farming, hunger, inventory, crafting, power and rescue values are unchanged.

## Open the review world

From the repository, with Godot **4.4.1 .NET** and .NET 8 available:

```sh
godot --path . -- --world-layout=mountain-region --world-seed=1701
```

If `godot` is not installed on PATH, the locally validated binary is:

```sh
export DOTNET_ROOT="$HOME/.dotnet"
export PATH="$DOTNET_ROOT:$PATH"
/tmp/hollowlight-godot-m5/Godot_v4.4.1-stable_mono_linux_x86_64/Godot_v4.4.1-stable_mono_linux.x86_64 --path . -- --world-layout=mountain-region --world-seed=1701
```

The `/tmp` download is temporary; substitute your installed .NET Godot binary if
it has been removed. `godot --path .` still selects the original compact refuge.
`--sanctuary-variant` still selects its alternate compact layout. Try seeds 499
and 8 to compare geography and finite supplies. Do not combine a compact-layout
comparison flag with the region profile and expect it to change the sanctuary.

Movement and diagonal speed, foot collider, camera coverage and interaction reach
are unchanged. Walk through the cave arch to cross between sanctuary and surface;
a brief fade identifies the transition. E / controller A still interacts; I opens
the backpack, H shows help, L compares neutral lighting. F2/F3 are developer home/
surface teleports, F4 advances a minute, F5 a day, Shift+F5 three days, and R resets
the run. Use ordinary walking when judging expedition timing.

## Spatial structure

- **Sanctuary:** 64×48 cells / 1024×768 world units. Five organically outlined,
  connected chambers: common/storage, workshop/power, garden, radio, entrance.
  A short central loop connects daily work. Four original growing plots and the
  existing fifth-plot kit socket remain available; no free expansion is granted.
- **Surface:** one continuous 192×144-cell / 3072×2304-unit local region. Mountain
  forecourt, Roadkeeper's shelter, Fernwater hollow, Ashwell ruins, two creek
  crossings and Cinder freight yard are linked by roads/trails and open terrain.
  Creek and mountain are physical barriers; trees/ruins use small foot collisions.
- **One active threshold:** interior and surface are separate spatial zones under
  the same session owner, both loaded in one scene. Crossing moves the actor and
  camera; it never reloads simulation, restocks resources or pauses the clock.
  This is an explicit transition, not a physically continuous cave/surface seam.

The scene uses the approved PNG/atlas production pipeline and Godot TileMapLayer,
with merged solid terrain collision runs from the same ground mask. New props are
Y-sorted sprites with explicit feet/anchors. Lighting stays adjustable; day/dusk/
night surface tints are presentation only. No new environmental danger is applied.
See [art provenance and export workflow](../assets/region/README.md).

## Generation and extensibility

[data/regions/mountain.json](../data/regions/mountain.json) owns landmark bounds,
route graph, widths, optional edges, chamber links, anchors, props and source-to-
landmark assignments. Named seeded streams separate geography, individual routes,
decoration, source positions and original quantity rolls. Generation places graph
nodes against geographical constraints, builds traversable ground/crossings,
connects required edges, then adds art and reachable source sockets. Navigation
avoids prop footprints and diagonal corner-cutting; interaction approaches are
checked after material-source collisions are placed.

Current runs preserve recognizable mountain/river/settlement relationships while
varying landmark locations, creek course, route bends, an optional connection,
vegetation and resource accessibility. This is deliberately constrained variation,
not a claim of arbitrary novel geography. Tests also move the freight yard, remove
a route and mirror the surface using profile data. Identity and east/west mirror
orientations are supported; vertically flipped entrances would need a deliberate
threshold/art facing implementation. No source rule assumes x>320 or a fixed road
coordinate. More distant regions can later use stable route/landmark IDs and the
same session boundary, without increasing this local map now.

The entry vestibule reserves a future controlled-door approach. Broad service
roads/crossing reserve room for later transport, while narrow trails provide a
contrasting route class. Open soil areas can later host reviewed outdoor farming.
These are spatial provisions, not doors, wheelbarrows, vehicles, outdoor crops or
night threats. Their dimensions and collision needs still require their own tests.

## Conserved economy

All original source IDs and quantity streams are retained. Three material nodes
still total **26 Scrap / 8 Organic**; six Food sources retain their existing **8–17**
possible Food budget; two Fuel caches retain **5–8 Fuel**, plus the same two shared
starter Fuel. Each actor still starts with two Food. Nothing scales with players.
The previously noted 9–17 Food planning estimate does not change the existing
8-Food minimum implementation. No stock or yield was rebalanced here.

Plants are assigned to the forecourt/shelter/hollow; containers to shelter/hamlet/
yard; original salvage to shelter/hollow/yard; Fuel to shelter/yard. Positions are
seeded, spaced and reachable. Existing partial collection, capacity checks,
finite depletion, explicit storage transfers and stale-command guards are reused.

[Actual physics travel measurements](M6A_TRAVEL.md) cover all eleven sources in
three seeds, including threshold traversal, gathering, return and deposit.
Round trips take **13.4–78.2 seconds**, at most **21.7%** of six-minute daylight.
Storage-to-workbench/garden/generator routes take about **2–5 seconds one way**.
Far trips are shorter than the earlier one-way planning estimate; do not infer
long-distance expedition pressure from this still-compact region.

[Daily 2/4/6-player economy ledgers](M6A_ECONOMY.md) replay those measured times,
both M5a investment choices, regular and delayed harvest visits, and unchanged
Food/Fuel/rescue operations. All 36 coordinated scenarios reach rescue. Smaller
Food margins and missed meals remain possible without expansion or with delayed
care; wasteful Fuel use still has the known unreachable-rescue outcomes. M6a adds
no compensating supplies. These are simulations, not playable network multiplayer.

## Review and validation

Build and model tests:

```sh
dotnet build Hollowlight.csproj --no-restore
dotnet run --project tests/Hollowlight.Core.Tests.csproj -- --region-balance docs/M6A_ECONOMY.md docs/measurements/region
python3 tools/art/build_region.py --check
python3 tools/art/check_region.py
python3 tools/art/check_sanctuary.py
```

Actual movement/gathering measurements (writes `user://region/travel-SEED.json`):

```sh
godot --headless --fixed-fps 60 --path . res://tools/RegionSmoke.tscn -- --world-seed=1701
```

Repeat for 499 and 8. Capture the profile in a graphical session with
`res://tools/RegionSmoke.tscn -- --capture-region`; the overview camera is a review
tool only. [Screenshots](screenshots/region/) include gameplay views and explicitly
labeled zoomed-out overviews. Visual inspection used software rendering on the
available desktop. No controller hardware or multi-machine networking was tested.

Manual review should follow the road out, recognize the two crossings and freight
yard, gather and deposit at least one source, return to the garden, use the powered
workbench, inspect a crop after travel and reset. Judge repeated chore convenience,
route recognition, floor texture repetition, landmark distinction and sanctuary
atmosphere at normal zoom. The compact fixtures remain the focused regression maps.

Automated coverage includes 32 generated seeds, exact regeneration, meaningful
layout/route variation, graph/interaction connectivity, unchanged budgets and
five-chamber access. Actual physics trips cover three representative
seeds; live crafting and power UI also pass in the region. All eighteen previous
runtime checks pass across both compact layouts. No survival system is added.

Remaining presentation compromises: mirrored texture periods and reused ruin
assemblies are visible; landmark-specific detail is intentionally limited. One
controlled actor is playable, and the current region is loaded as a whole rather
than streamed. Future geographic variety and multiplayer need further milestones.
Stop here for owner review before making this the default or expanding systems.
