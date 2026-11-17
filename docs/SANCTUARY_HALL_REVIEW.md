# M6c follow-up — sanctuary hall review

The four approved steps are implemented: compact HUD, softer conservative fog and
grounding, coherent garage architecture, and a separate sanctuary hall profile.
The default sanctuary is unchanged. No broader environmental artwork pass,
functional bunker doors, vehicles, resurrection, batteries or biomass generation.

## Run

From the repository root with Godot 4.4.1 .NET:

```sh
godot --path . -- --world-layout=sanctuary-hall-review --world-seed=1701
```

This workspace currently has a temporary editor if `godot` is not on PATH:

```sh
export PATH="$HOME/.dotnet:$PATH"
export DOTNET_ROOT="$HOME/.dotnet"
export DOTNET_CLI_HOME=/tmp/hollowlight-m5-cli
export NUGET_PACKAGES="$HOME/.nuget/packages"
export XDG_DATA_HOME=/tmp/hollowlight-data
export XDG_CONFIG_HOME=/tmp/hollowlight-config
export XDG_CACHE_HOME=/tmp/hollowlight-cache
/tmp/hollowlight-godot-m6b/Godot_v4.4.1-stable_mono_linux_x86_64/Godot_v4.4.1-stable_mono_linux.x86_64 --path . -- --world-layout=sanctuary-hall-review --world-seed=1701
```

The `/tmp` editor is not a permanent installation. Existing `compact`,
`mountain-region` and `garage-expedition` profiles remain available. Omitting
`--world-layout` still runs the compact refuge. Existing movement, interaction,
inventory, crop selection, gathering and machine controls remain unchanged.

## Hall and reservations

[Implemented footprint diagram](screenshots/sanctuary-hall/footprint.svg) and
[gameplay screenshot gallery](screenshots/sanctuary-hall/README.md).

One irregular, roughly square cave replaces the five linked chambers **only in
the new profile**. Total walkable sanctuary area is **561 tiles**, including the
shallow alcove and entrance, versus **1036** in the old region sanctuary. Native
pixel density, world scale and 384×216-unit camera coverage remain unchanged.
The central area is open, with workshop, storage, garden, power and radio around
the perimeter. Existing bedrolls remain near the southern side.

- Four starting plots remain in two trays. The original kit installs exactly one
  fifth plot at its reserved socket; recipes, seeds, growth and harvests are intact.
- Garden reservation is seven by eight tiles, allowing approximately eight future
  plot positions. This adds no usable plots, upgrades, seeds or crop definitions.
- Utility reservation is **96×64 world units**, beside existing power, for future
  biomass generation/battery infrastructure. It installs no machinery or consumers.
- Turning space is **64×64 units**. Main circulation stays open and short.
- The shallow northern alcove contains an inert damaged shell, composed from
  existing machinery/bedroll artwork. It is not a new generator, crafting station,
  salvage source or functional resurrection system. Dedicated artwork is deferred.
- The entrance has **six clear tiles / 96 units** for two future 48-unit iron leaves.
  Stone/steel abutments are grounded in matching solid cells. The lintel uses a
  shared cutaway-overhead render rule so it does not clip passing characters.
  There is no leaf state, operation, power demand or closed-door collider. The
  four-tile-deep inner apron and threshold stay clear. Travel still uses the same
  sanctuary/surface transition system and unchanged surface landing.

The floor-mask plan, anchors, fixtures and reservations live in
`data/regions/sanctuary_hall.json`; `SanctuaryPlan` adapts them to the region input.
Surface generation uses the original definition and seeded streams. Sixteen model
seeds verify exact surface ground/source equivalence with the old garage profile.

## Presentation changes

**HUD:** Cohesive-room chrome now uses viewport-space layout, bounded labels and
measured text widths. Status, transient location, contextual prompt and feedback
respect safe margins. Single-line messages stay compact; longer ones wrap into
bounded two-line panels. Existing hunger warning colors remain. Inventory/modal
grids keep their original coordinate system, capacities and input hit targets.
The historical raw-scene HUD is retained separately for regression compatibility.

Supported minimum window is **960×540**. The tested window requests are 960×540,
1280×720, 1536×864, 1920×1080, 1024×768 and 2560×1080. Integer scaling and aspect
preservation may add letterboxing. Native viewport-content captures are 768×432
or 1536×864, depending on the available integer scale; OS borders/letterboxing are
not included in viewport PNGs. Every case retains 384×216 world-unit coverage.

**Fog:** The existing explored/current mask stays nearest-sampled and authoritative.
The surface shader computes a rounded, inward feather near unrevealed neighbours,
with gentle world-anchored variation. It darkens/conceals extra edge pixels rather
than revealing outside the logical mask. Dynamic fragments still discard when
current visibility is absent; mapped terrain does not show unseen object updates.
`WorldDiscovery` itself is byte-identical to the previous checkpoint.

**Grounding:** `data/contact_shadows.json` supplies material-independent contact
patches: four tire contacts and a restrained chassis patch for the van; feet/base
contacts for the compressor, cabinets, crate and sanctuary furniture. Contact
shadows do not receive direct point lighting. Sprite scale and original painted
material shading are unchanged. No PNG/source-art replacement or baked-shadow
repainting was needed. Moving registered render bodies update their own ground
projection; the rule is reusable for future actors.

**Architecture:** Garage walls now occupy their complete original 16-unit width
in either orientation, with consistent face/cap/base placement and exposed-edge
joins. Narrow repeated posts and arbitrary corner overlays are removed. Floor-level
metal threshold strips connect the existing concrete/apron and side entrances;
they add no collision. The old garage ground mask, openings, object footprints and
interaction positions are unchanged. Fog, ambient lighting, contact shadows and
actual light occlusion remain separate inputs. No new light consumer is added.

## Actual walking measurements

Measured with normal actor input, physics and clock at the existing 80-unit/s speed.
Thirty directed station pairs are recorded; repositioning to the starting point
is excluded, and no teleports/time skips are used during measured segments.
These are efficient known-route walks, not human menu or social-coordination times.

| From storage to | One way |
|---|---:|
| Workshop | 1.30s |
| Garden | 1.75s |
| Power | 2.22s |
| Radio | 2.88s |
| Entrance landing | 3.73s |

The longest measured directed pair is **4.08s**. Raw measurements, all six window
cases and physical-proxy evidence are in [hall.json](measurements/sanctuary-hall/hall.json).
The entrance measurement ends at the inner landing, before crossing to the surface.

## Validation and reproduction

```sh
dotnet build Hollowlight.csproj --no-restore
dotnet run --project tests/Hollowlight.Core.Tests.csproj
godot --headless --fixed-fps 60 --path . res://tools/HallSmoke.tscn
godot --disable-render-loop --fixed-fps 60 --path . res://tools/HallSmoke.tscn -- --capture-hall
python tools/package_hall_review.py /path/to/Hollowlight/user-data
python tools/check_hall_review.py
```

`--disable-render-loop` renders only explicitly requested screenshots while normal
simulation/input/physics keep running. This avoids unnecessary software-renderer
work during long automated walks; these runs are not frame-rate benchmarks.

- Build has zero warnings/errors. Core tests include 16 new hall seeds plus the
  existing inventory, farming, hunger, crafting, power, rescue and geography suites.
- All 23 existing engine regression cases pass: nine tools in both compact layouts,
  garage seeds 1701/499/8, preserved van and the original M6a region.
- Hall runtime validates access to storage/workshop/power/radio, withdrawn-seed
  planting, passive growth, real finite material gathering, powered bed-kit crafting,
  fifth-plot installation, exact three-Food/one-seed harvest and New Run reset.
- Six test bodies with 8×6 feet pass the doorway simultaneously in opposing lanes,
  with actor-to-actor collision enabled and **16-unit minimum center separation**.
  The observer is temporarily excluded from the test and restored afterward.
  Live player collision settings remain unchanged; this is not networked multiplayer.
- A **48×24-unit** test body physically travels from the entrance to station drop
  areas and the turning pad. Both a rotated rectangle and its circumscribed circle
  fit, checking the full rotation envelope. This is a provisional clearance proxy,
  not a wheelbarrow feature or final vehicle-size specification.
- Matched old-layout captures retain **655 identical collision shapes**, generated
  ground/source data, actor positions, camera coverage and capture clock times.
  Rendered lamp probes still show zero change behind solid walls in both directions
  and positive illumination through the doorway.
- Existing art, economy data, regression layouts, study artifacts and discovery
  implementation remain byte-identical; hashes are archived in
  [preserved-inputs.json](measurements/sanctuary-hall/preserved-inputs.json).

See [summary](measurements/sanctuary-hall/summary.json),
[runtime results](measurements/sanctuary-hall/runtime-validation.json),
[build](measurements/sanctuary-hall/build.txt), and
[core tests](measurements/sanctuary-hall/core-tests.txt).

## Review limits

The hall is intentionally opt-in. Existing textures, furniture and machinery remain
the art baseline; the inert chamber and frame reuse those materials. Terrain
repetition and the broader beautiful-decay vegetation/material comparison remain
deferred. Fog is softer but still derives from the existing coarse visibility grid.
Lighting retains the existing 2D point-light/cutaway limitations. Captures use Linux
Compatibility rendering on Mesa llvmpipe; hardware performance, physical controller
feel and human multiplayer circulation have not been established by these tests.

Stop for visual/gameplay review before replacing any default layout or beginning
the environmental artwork phase. No survival expansion or economy rebalance.
