# M6c — technical presentation checkpoint

Technical corrections only. Existing artwork is retained. The fuel-cabinet,
compressor, wall-corner and environmental-art comparison has **not** begun and
requires approval of this checkpoint first.

## Review evidence

- [Matched before/after screenshots](screenshots/m6c/README.md), original 1536×864
  window pixels and unchanged gameplay camera coverage.
- [Sanctuary movement comparison](recordings/m6c/sanctuary-motion-comparison.mp4)
  and [garage/doorway movement comparison](recordings/m6c/garage-motion-comparison.mp4).
  Before is left; after is right. Neither panel is resized.
- [Geometry and lighting checks](measurements/m6c/validation.json),
  [before capture state](measurements/m6c/before.json),
  [after capture state](measurements/m6c/after.json), and
  [recording details](measurements/m6c/recordings.json).

The camera, seed, positions and capture clock times match exactly. All **655
collision shapes**, generated ground cells, source definitions/quantities, movement
speed and interaction range match the M6b baseline. Scene geometry and economy data
were not edited. Presentation changes apply to existing cohesive-room rendering;
the garage remains the optional `garage-expedition` profile and compact layouts
remain the default/regression fixtures.

## Defects and corrections

| Defect | Root cause | Technical correction |
|---|---|---|
| Actors drawn over cave faces regardless of position | Faces were on fixed negative Z layers | Ground remains on terrain layers; upright faces use ground-contact Y-sort roots shared with actors/props |
| Garage wall gaps and repeated column caps | Transparent source margins at joins; complete post repeated every cell | Sample opaque interior strips at unchanged native density; use continuous shaft sections, end caps and junction details from existing PNGs |
| Fog cuts across elevated artwork | High-Z rectangular fog painted over everything | Surface materials project visibility to the owning ground footprint; terrain memory and current dynamic-object masking are separate |
| Large objects pop with one anchor; short mask lag after movement | Single-point visibility plus independently timed presentation cache | Coverage spans the object footprint; per-fragment current masking remains conservative; a read-only discovery revision updates masks on authoritative sight changes |
| Viewport-clear triangles at cut-corner terrain boundaries | Transparent atlas corners could expose unpainted background | Stable unknown-geography background and opaque unknown-terrain mask, without changing floor/collision cells |
| Interior/underground tint depends on where the player stands | One global ambient color switched by controlled actor position | Spatial exposure/shelter texture and reusable lighting-zone nodes; daylight/dusk/night color comes from the existing clock |
| Lamps illuminate through walls | No light occluders | Boundary-edge wall occluders and selected large-prop casters, independent of collision geometry |
| Lighting can disappear with hidden fixture art | Light nodes inherited the prop's visibility | Physical light nodes are independent of discovery-hidden art; existing power bindings remain authoritative |
| Self-shadowing risk from elevated bulb positions inside supports | Sprite-space lamp position used as a ground-space ray origin | Project light origins in front of solid supports; keep glow at bulb height with texture offset |
| Status diode could render over unrelated foreground objects | Diode used a higher global Z layer | Keep it in the owning object's sort group and current-visibility material |
| Held tool had inconsistent pixel density | Sprite used scale 0.35 | Use the shared 0.5 scale; no tool behavior or footprint change |

## Reusable architecture

`data/presentation.json` defines semantic ground footprints, remembered/static
classification and optional light casting. Native sprite dimensions and anchors
continue to come from the existing asset manifests. `WorldVisuals` registers
surfaces, records their independent local visual bounds, creates material bindings,
and releases its owned shader/texture resources on scene exit. Shared artwork and
the loaded shader are not disposed as renderer-owned assets.

`WallAssembly` places upright faces at ground-contact origins and assembles the
existing masonry texture parts by adjacency. It creates no physics bodies. Existing
colliders, corner cuts, navigation and generated ground remain authoritative.

`LightingZone` is an axis-aligned, presentation-only region with sky exposure,
shelter and priority. Generated environments can emit these alongside modular
terrain. The garage renderer supplies one roofless-interior zone from its existing
wall bounds; the lighting renderer does not special-case garage geometry. Cave
ambience stays independent of exterior time. The roofless garage retains diffuse
sky illumination with modest shelter shading. Dusk blends toward readable cool
night; the end of night blends back toward daylight. No clock durations change.

`world_surface.gdshader` consumes three independent things:

1. **Explored geography** — shared session memory, read from `WorldDiscovery`.
2. **Current visibility** — observer-local sight; dynamic fragments outside it are
   discarded, and mapped but unseen static terrain receives no live local lighting.
3. **Illumination** — spatial ambience plus shadow-enabled Godot point lights.
   Light never grants discovery or current sight.

The renderer reads these values without writing simulation state. `WorldDiscovery`
only adds a revision counter; its sight/occlusion/exploration rules are unchanged.
Interaction checks still require the original authoritative point visibility,
range and ray checks, even when part of a large object's artwork is visible.
Indicators and garden markers use the same visibility policy. There is no new
multiplayer/networking implementation.

## Lighting evidence

The diagnostic lamp is a test-harness fixture, not a free gameplay light or new
consumer. Three captures use the same night clock/camera: lamp left, lamp right,
and lamp off. Discovery rendering is disabled **only for these static-floor probes**
so fog cannot masquerade as a shadow. Ordinary screenshots retain discovery.

Mean absolute RGB changes relative to lamp-off, sampled on open floor:

| Probe | Before | After |
|---|---:|---:|
| Left lamp → floor behind solid divider | 62.59 | **0.00** |
| Left lamp → floor on its own side | 87.82 | **48.76** |
| Left lamp → floor through open doorway | 49.53 | **27.60** |
| Right lamp → floor behind divider in reverse | 48.57 | **0.00** |

This verifies actual rendered wall occlusion in both directions and light through
an opening. Moving/toggling the lamp leaves explored/current discovery snapshots
unchanged. Baseline/post-change lamp brightness is not claimed identical.

## Reproduce

Use the same Godot 4.4.1 .NET editor and environment setup as
[the garage guide](M6B_GARAGE.md). Normal review launch:

```sh
godot --path . -- --world-layout=garage-expedition --world-seed=1701
```

Capture the current renderer and actual-input movement:

```sh
dotnet build Hollowlight.csproj --no-restore
godot --fixed-fps 60 --path . res://tools/PresentationReview.tscn -- --review-tag=after --record
```

The harness uses explicit review positioning for stills and real player input/
physics for movement clips. Captures go to `user://presentation/after/`.
The archived `before` images and state were captured from M6b before changes;
running `--review-tag=before` with current code does **not** restore the old renderer.
Retain the archived baseline when packaging subsequent comparisons.

```sh
python tools/package_presentation_review.py /path/to/user-data/presentation
python tools/check_presentation_evidence.py
dotnet run --project tests/Hollowlight.Core.Tests.csproj
python tools/art/check_garage.py
python tools/art/check_region.py
python tools/art/check_sanctuary.py
```

Videos are sampled real-input routes, played at 15 fps; per-cell sampling makes
them review clips rather than exact elapsed-time or performance measurements.
Individual clips preserve native window resolution. MP4 compression is lossy;
use PNGs to inspect individual pixels. Fixed 60-Hz simulation deltas are not measured
frame rates. Adapter CPU measurements exclude GPU, rendering and screenshot I/O.

## Validation results

- [Build](measurements/m6c/build.txt): Godot 4.4.1 .NET / SDK 8.0.424, zero warnings/errors.
  [Core tests](measurements/m6c/core-tests.txt) pass, including shared discovery and
  revision invariants, seeded geography, inventory, agriculture, hunger, power,
  crafting and rescue conservation/concurrency cases.
- [23 final runtime runs](measurements/m6c/runtime-validation.json) pass: nine
  legacy tools in both compact layouts, garage seeds 1701/499/8, van preservation
  branch and original M6a region navigation. The power test now locates tagged
  physical lights independently of their old prop parent; it still checks actual
  light visibility during on/off operations. Existing survival assertions remain.
- [Preserved input hashes](measurements/m6c/preserved-inputs.json) cover **271**
  pre-existing art, data and study files, all byte-identical. The new presentation
  metadata file contains no economy quantities. Archive checker verifies 655
  unchanged collision shapes, original ground/source data and exact matched views.
- Pixel probes verify actual wall blocking in both directions and light through
  the open doorway. Native-resolution clips show actual movement near props and
  across the garage entrance, plus the existing vegetation reaction.
- During an earlier concurrent matrix, two headless processes passed their gameplay
  assertions but aborted on Godot C# native shutdown bindings. Unmodified isolated
  reruns passed. Renderer-owned resources now explicitly release bindings on scene
  exit; the final 23-run matrix and [six targeted repeats](measurements/m6c/shutdown-reruns.json)
  exited cleanly. [Investigation](measurements/m6c/shutdown-investigation.json)
  records the initial failures; a single upstream cause is not established. No
  engine upgrade or forced-GC workaround was introduced.
- Adapter CPU samples: median **0.054ms**, p95 **1.50ms**, maximum **30.98ms** in
  the recorded run. These measure the adapter only, with occasional allocation/
  mask-update costs; they do not establish GPU performance or a steady frame rate.

## Remaining limits and review gate

- Sight still uses the existing conservative 16-world-unit grid. Boundaries can be
  visibly stepped and large objects can be partially masked at the edge of sight;
  no hidden-state reveal is used to make them look smoother.
- These are 2D point-light shadows, not height-aware 3D illumination. Outdoor
  daylight/moonlight currently has ambient color changes, not a moving sun or
  directional cast-shadow simulation. Existing painted material shading remains.
- Environments retain the existing cutaway/roofless policy; no full roof-fade system.
- Texture repetition, sparse composition, uniformly rusty materials and small
  source-frame registration differences remain art-phase work. Existing fern brush
  reactions and mushroom pulses remain; new idle/Bloom animation is deferred.
- Captures use Linux Compatibility rendering on Mesa llvmpipe. No hardware GPU,
  physical-controller, live multiplayer or cross-platform performance claim.

Stop here for technical review. Only after approval should the bounded fuel-cabinet,
compressor, wall-corner and immediate-environment comparison begin. No new locations,
resources, survival rules or environmental artwork have been produced in this phase.
