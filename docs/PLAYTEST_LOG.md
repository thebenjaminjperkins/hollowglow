# Playtest and validation log

## 2026-11-11 — initialization

### Observations

- Read all three Hollowlight documents completely; no conflict between their core scopes.
- Initial workspace contained only those documents and protected empty metadata directories.
- `command -v` found Git and VS Code, but no Godot, dotnet, or C# compiler.
- `git status` reports no repository; `git init` fails because `.git` is read-only.
- SDK setup endpoint could not resolve through the available shell network.
- Initial `dotnet build` and console-test commands both exited 127 (`dotnet` absent).
- Project/test XML and VS Code JSON parse successfully using Python. This is
  configuration validation, not C# compilation or gameplay validation.
- The owner then selected direct movement/following camera and amber/teal/gray-green
  16-pixel motifs at 3× scale; implementation followed those choices.

### Completed validation

- Installed official .NET SDK 8.0.428 and Godot 4.4.1 .NET into isolated `/tmp`
  directories after retrying a timed-out automatic network approval. Used bundled
  Godot NuGet packages, without adding dependencies or changing system toolchains.
- Initialized Git through authorized execution outside the read-only metadata mount.
- `dotnet build Hollowlight.csproj`: passed, zero warnings/errors.
- `dotnet run --project tests/Hollowlight.Core.Tests.csproj`: passed uniqueness,
  unknown pickup rejection, actor isolation, quantity conservation, repeated/empty
  deposits, fresh state, and invalid quantities.
- Godot editor import: passed with isolated XDG directories and SDK on PATH.
  Initial sandboxed editor attempts failed on local sockets and user-data access;
  these environment issues were resolved for the successful import.
- Headless main-scene launch: completed 180 frames without reported errors.
- Engine smoke scene: passed fixed-tick movement, normalized diagonals, cave wall
  collision, passage traversal, out-of-bounds recovery, pickup range and uniqueness,
  all three pickups, shared deposit, repeat deposit, debug teleports preserving
  quantities, and resets both while carrying and after storing.
- That test exposed a real reset-handler error: after reloading the scene, the
  handler tried to access its detached viewport. Fixed by handling the event before
  reload; rerun passed with no error output.
- Launched the actual game through X11/OpenGL on Intel Iris Xe and inspected captures
  of the surface and refuge. Debug home teleport was also sent to the game window.
  An earlier graphical session ended with signal 143 and engine shutdown warnings;
  headless tests were clean. A short final graphical run exited normally (code 0)
  without error output.
- Captures in `docs/screenshots/` show the actual running game, not concept art.
- Final scene/resource references resolve and all three original design-document
  SHA256 hashes match the initial inspection. Git diff whitespace checks passed;
  generated engine/build outputs are ignored.
- Git is initialized on its default `master` branch. All intended source files and
  documentation are staged for review; no commit was created. Working tree contains
  staged additions, rather than a clean committed milestone.

### Owner playtest still needed

- Keyboard feel, usefulness of camera framing, prompt clarity, and emotional contrast.
- Complete the README smoke checklist manually, especially rapid repeated key presses
  and reset from different locations. Scripted input tests do not establish keyboard feel.
- No physical-controller, Windows, multiplayer, or multi-machine tests were performed.

### Recommended next step

Review/iterate this slice based on owner playtesting. M2 remains unapproved; obtain
inventory, sharing/ownership, resource taxonomy, and hunger-severity decisions before it.

## 2026-11-11 — M1.5 Sanctuary Identity

### Authorization and scope

The owner confirmed working basic gameplay, requested environmental identity before
survival expansion, and approved organic pixel art with modular assets. The work
adds irregular cave geometry, vegetation, lighting, salvaged props, character animation,
and a compact HUD. The Vision is preserved. M2 and later survival systems remain gated.

### Observations and validation

- Built with Godot.NET.Sdk 4.4.1 and .NET 8.0.428: zero warnings and zero errors.
- Final Godot headless editor import completed without errors; scene/resource
  references, load counts, JSON/XML, and Git whitespace checks passed. Vision and
  Bootstrap hashes remain unchanged; the Roadmap records only the approved M1.5 insertion.
- Pure C# tests passed resource conservation/uniqueness and verified that both
  authored layout masks have connected floor, a closed boundary, and reachable
  home, storage, passage, and salvage anchors. Solid props are anchored on floor.
- The real-engine smoke test passed for Hearth Hollow and Rootfall Nook: movement
  speed, normalized diagonals, wall collision, continuous passage, out-of-bounds
  recovery, pickup range, unique pickup, all deposits, debug teleports, and two resets.
- Added a direct physics-query check that a visible chamfered rock corner is solid
  while the tile's floor center is clear. Passed in both arrangements.
- Actual graphical presentation tours ran through X11/OpenGL on Intel Iris Xe,
  captured the viewport only, and exited normally without error output.
- Inspected primary refuge, workshop, storage prompt, threshold, and surface views,
  plus alternate refuge/workshop captures. Curated captures are in `screenshots/m15/`.
- Refined rock continuity, subtle floor color variation, and text-sized HUD toasts
  after inspecting the first captures. Retained 3× scaling and the following camera.
- Screenshots show rendering and contextual prompts, not physical-controller testing
  or a claim that the owner has accepted the atmosphere. Tours use scripted input.

### Owner evaluation

Run the original salvage loop in both arrangements. Assess warmth/mystery, whether
the chamber reads as natural shelter, whether lighting and plants guide attention,
character readability in motion and behind/in front of props, and HUD obstruction.
H opens help; F1 retains debugging. Windows/controller/network testing is unperformed.

Stop at this milestone for feedback. Recommended next work is a bounded visual
iteration based on that evaluation; no survival-system expansion is authorized yet.

## 2026-11-11 — Approved four-asset quality benchmark

### Implementation

- Character, generator, fern and cave wall now load lossless indexed PNG sheets.
  Other art remains at its previous quality level pending benchmark review.
- One shared 32-color palette, explicit editable pixel layers, and an offline PNG
  exporter replace runtime geometry for these assets. No random texture/noise or
  concept-image downsampling is used by that exporter.
- The character has four facings, two idle poses and four walk poses per facing,
  fixed anchors, and a stable diagonal-axis tie rule. Movement/input/feet collider
  and mutable salvage rules were not changed.
- Fern Area2D brush feedback is nonblocking and presentation-only. Entering while
  moving selects authored bend frames and then settles to rest.
- A developer comparison scene shows the same assets under neutral/unshaded and
  actual amber lighting. SPACE brushes vegetation; 1 toggles walk/idle.
- `assets/benchmark/README.md` records the resolution, anchors, frame order,
  palette/material/lighting rules, authoring/export workflow, and provenance.

### Validation actually performed

- Game build: passed with zero warnings/errors using the existing isolated toolchain.
- Deterministic tests: passed resource rules, stable facing on diagonal ties, idle
  facing, directional vegetation brush/settling, and both layout connectivity checks.
- Live-engine smoke tests passed on both arrangements: original movement/diagonal
  speed, cave collision/corners, passage, recovery, pickups, storage, teleports and
  resets. Added real Area2D entry test verifies brushing the fern without blocking
  player travel, followed by settling.
- Export checks passed all four sheets: exactly 32 shared palette entries, valid
  palette symbols, exact dimensions, no clipped pixels, and byte-for-byte agreement
  between authored source and exported PNG. PNG imports are lossless without mipmaps.
- Godot editor import completed without errors. An automatic approval attempt for
  the final import timed out; its permitted retry succeeded.
- Ran the benchmark viewer and integrated sanctuary tour graphically through
  X11/OpenGL. Both exited normally. Inspected the source/light comparison and live
  workshop view, refined the first wall/fern treatments, then inspected final captures.
- Actual captures: `screenshots/benchmark/runtime.png` and `workshop.png`.
  The separate `reference.png` is a built-in imagegen concept reference, not a
  gameplay capture; its exact prompt is in `docs/benchmark_reference_prompt.txt`.
- Vision hash remains unchanged. No physical controller, Windows or multiplayer
  validation was performed. Visual quality approval remains with the owner.
- Source, PNG exports/import descriptors, guides and curated captures are staged
  for review. No commit was created; the repository has staged additions.

### Review gate

Evaluate character motion in four directions and diagonals, machine depth/wear,
foliage overlap and reaction, and the wall section at native and 3× scale. Run the
normal salvage loop. Do not broaden art production or survival systems before review.

## 2026-11-11 — Separate machinery-first extraction trial

- Approved scope: process the reference's machinery directly in a separate viewer;
  keep existing benchmark/gameplay unchanged and defer all other asset work.
- Saved original crop, editable background mask, extracted RGBA source, exact recipe,
  source hash, palette and native 64×64 candidate. The image uses 47 of 64 provisional
  swatches, preserving source silhouette/aspect in 60×47 visible pixels.
- Built the game with the new independent tool: zero warnings/errors. Core logic
  tests and the original live smoke tests passed on both sanctuary arrangements.
- Ran the trial through X11/OpenGL and checked its 768×432 logical grid/2× camera:
  actual coverage remains 384×216 world units. Horizontal travel and normalized
  diagonal travel passed. Existing 8×6 player feet, 26×9 machine collider, speed 80,
  interaction range 22 and resource counts were preserved.
- Captured original/candidate at equal player position and camera center, both at
  the intended 1536×864 display and a native 768×432 window. Both display scales
  retain the same camera coverage. Capture and audit run exited normally.
- Visually inspected the extracted source, native candidate, full gameplay output,
  and assembled native/gameplay comparison boards. Fine wear is more faithful;
  the candidate is wider/lower, denser and darker than the old simple upright sprite.
- Reproducible processing, 64-color palette, binary alpha, transparent exhaust opening,
  exact image dimensions and unchanged hashes for 61 protected production files passed.
- Evidence and compromises: `assets/trials/machinery/README.md` and
  `docs/screenshots/machinery-trial/`. Capture crops are unscaled screenshots.
- No generation call, production asset migration, survival expansion, physical
  controller or Windows testing occurred. Owner quality acceptance remains pending.
- Trial sources, processing evidence, comparison captures and documentation are
  staged for review. No commit was created; the original benchmark remains intact.

Stop after reporting the comparison. No wider pipeline/library change is authorized.

## 2026-11-11 — Cohesive sanctuary room

The owner subsequently authorized a focused whole-room benchmark using the accepted
machine quality target. The workflow was explained before implementation. Built a
matched compact source kit and an opt-in playable room; no survival expansion.

### Implemented and observed

- Reused the approved machine byte-for-byte. Extracted/registered four character
  views from the original reference and produced PNG cutout walk/idle poses.
- Built-in imagegen produced a matched furniture sheet, vegetation sheet and terrain
  material sheet. Exact prompts/masters and processing outputs are in
  `assets/sanctuary/source`; the normalized kit shares one palette and density.
- Real TileMapLayer ceiling/floor/wall layers assemble atlas tiles from the original
  floor mask. All original collision bodies and gameplay rules are retained.
- Visible legacy geometry art is hidden in this scene. Furniture, chest, vegetation,
  character, floors/walls and salvage now use PNG textures. Non-solid old ground
  line/rug clutter is omitted. The functional surface loop reuses the bounded kit.
- Live lamp/ambient lighting is adjustable with L. First capture exposed a repeated
  floor border lattice; replaced individual border blending with periodic material
  patches. Inspected the revised gameplay views and quieter ceiling treatment.
- Ambient tint, lamp energy and lamp radius scale are exposed on the room scene
  root for editor tuning; no painted whole-room illumination is required.

### Validation actually performed

- .NET build passed: zero warnings/errors. Godot editor import completed cleanly.
- Pure logic tests passed. Real-engine smoke tests with the room presentation passed
  on both existing layouts: movement, diagonal speed, corners/walls, passage,
  recovery, pickup range/uniqueness, deposits, teleports and two resets.
- New room checks passed: TileMapLayer covers every original floor tile, tile
  collision is disabled, world camera coverage remains 384×216, old character
  rendering is hidden, and the new textured fern reacts without blocking travel.
- Processing checks passed source/export reproducibility, common 64-color palette,
  binary alpha, anchors, periodic floor joins, chamfer masks and approved-machine identity.
- Hash checks confirm 69 pre-existing gameplay/scene/data/benchmark files unchanged.
  Only the test harness gained an optional presentation flag; old launch behavior remains.
- Ran graphical capture tours through X11/OpenGL and inspected gameplay, neutral-light,
  workshop/home views. Captures are actual 1536×864 viewport images; the native view
  is 768×432. Measured lighting differences confirm that L changes actual rendering.
- Curated captures: `docs/screenshots/sanctuary-room`. No controller, Windows,
  multiplayer or subjective owner acceptance is claimed.

### Remaining review

Assess cohesion at normal gameplay zoom, object scale/depth overlap, lighting mood,
floor readability and reference-cutout character motion. The character walk uses
registered pixel offsets rather than newly drawn anatomy, and form lighting remains
in source textures. These are bounded benchmark compromises, not a completed asset
production pass for the full game. Stop for owner review before wider production.

Final source/assets, preserved masters/prompts, checks, captures and documentation
are staged for review. No commit was created. The working tree contains staged
additions; generated engine/build/Python caches are excluded.


## 2026-11-11 — Approved sanctuary integrated into normal play

### Changes and observations

- Normal launch/F5 mounts the approved cohesive presentation over the original
  simulation. Restored the original home spawn rather than the benchmark's workshop
  capture position. Earlier comparison scenes remain separately runnable.
- Reviewed native walk frames and real eight-direction movement captures at gameplay
  zoom. Direction rows are correct, exact diagonals keep a stable facing axis and
  the foot baseline stays registered. Added a ground-line export guard; it makes
  no changes to the current PNG pixels. Walk remains reference-cutout articulation.
- Animation uses actual post-collision displacement rather than requested velocity;
  its cadence follows speed, and blocked walking returns to idle. No movement,
  collision, range, layout, resource logic or camera coverage was changed.

### Validation actually performed

- C# build and deterministic resource/layout/presentation tests passed.
- Godot editor import completed cleanly with local editor sockets permitted. The
  first sandboxed import had socket-permission errors; the permitted rerun passed.
- Normal-entry smoke tests passed on both authored layouts, through movement,
  normalized diagonals, chamfer/wall collision, passage traversal, recovery, fern
  reactions, range, finite pickup uniqueness, deposits, teleports and two resets.
- Added actual runtime facing checks, idle after stopping, and idle while holding
  against a wall. Existing 384x216 world camera coverage and tile collision checks pass.
- Graphical normal-entry capture passed native/gameplay views and adjustable light
  output. Inspected the home view and native/gameplay walk frames. First graphical
  walk run stalled; fixed-fps 60 rerun captured every phase and exited successfully.
- All eight captured directions report speed approximately 80 world units/second;
  diagonal components are approximately 56.57. Four successive walk phases cycle
  consistently at the approved full-speed cadence. Actual viewport crops, without
  resizing, are in `docs/screenshots/sanctuary-room/walk-gameplay.png`.
- Export reproducibility, shared palette/binary alpha, all 24 character ground
  anchors, terrain joins/masks and approved-machine identity passed. The 68 older
  gameplay/scene/data/benchmark files remain byte-identical; the project snapshot
  matches after reversing only the approved default-entry change.
- Keyboard feel and subjective animation appeal remain owner playtest items;
  graphical scripted movement is not a physical-input or multiplayer test.

### Proposed next review

Roadmap M2a would add typed per-player stacks and explicit shared-chest transfers,
starting with existing finite gathering sites and approved art. Ownership, resource
count and carrying limits await owner approval. No part of the broader survival
economy was implemented. Changes are staged for review; no commit was created.


## 2026-11-11 — M2a Shared supplies

### Implemented

- Typed finite Scrap/Organic gathering, six-slot personal inventories with stacks
  of ten, configurable communal storage (24 slots), and explicit partial transfers.
- Read-only pack/chest panel and compact carried counts. E retains gather/store-all;
  Z/X and Shift give one/ten-unit transfers. Out-of-range access is refused even
  when the panel remains open. Source quantities remain visible in gather prompts.
- Model/state ownership is separate from rendering and input. Mutations validate
  actor/resource/quantity, capacity and session revision under a single lock.
- Reused existing PNG sprites and site footprints; organic growth uses the approved
  mushrooms. Layouts, movement, character animation and terrain pipeline are unchanged.

### Validation actually performed

- .NET build passed with zero warnings/errors and Godot editor import passed.
- Core suite passed six-slot and ten-stack limits, typed stack separation, partial
  gathering, source depletion, source/destination-limited transfers, full storage,
  unknown/invalid requests, immutable snapshots, actor isolation and reset.
- Two-actor Parallel.Invoke tests passed 32 gather and withdrawal races, plus
  competing deposits into the last storage slot. Replayed/stale commands are
  rejected; a fresh session rejects tokens from a different session at equal revision.
- 2,000 seeded mixed commands checked stack bounds and resource conservation after
  each operation, including transfers across both actors and shared storage.
- Real-engine normal-entry tests passed on both authored layouts: original movement,
  normalized diagonals, wall/corner collision, passage, recovery, foliage, range,
  finite pickup uniqueness, typed deposits, keyboard one/ten withdrawals, selected
  Organic transfers, stale replay rejection, open-panel movement speed, distance
  checks and two resets.
- Graphical review exercised actual gathering, chest UI transfers, partial withdrawal
  (request ten Organic, receive eight), and open-panel rejection after leaving range.
  A separate one-slot fixture passed partial deposit (12 -> 10 stored + 2 carried),
  full storage preserving carried supplies, emptied-slot reuse and type separation.
- Inspected real gameplay-scale personal/chest/withdrawal/full-storage captures,
  preserved in `docs/screenshots/m2a`.
  The one-slot fixture duplicates configuration in memory and does not alter game data.
- Art export/palette/anchor/terrain checks and runtime-diff review preserve the
  approved visual standard; source masters and all runtime PNGs remain unchanged.

### Review limits and next gate

No real network session, multi-machine contention, physical-controller or Windows
validation was performed. Parallel tests cover the authoritative inventory model,
not transport/authentication. Keyboard feel and panel ergonomics remain owner review
items. No survival consumption, weight, crafting or power was added. Stop at M2a;
no later slice is authorized. Changes are staged for review; no commit was created.


## 2026-11-11 — Approved visual inventory UX

### Implemented and observed

- Configurable icon grids: 3×2 personal pack, 6×4 communal chest, paged when required.
  Names appear on focus/hover and stack counts stay on icons. Permanent HUD is a
  small backpack icon plus occupied slots; instructions are contextual.
- Mouse click/place, actual drag/drop, split quantity selection, matching merges,
  whole-stack swaps, quick transfers and explicit Store all. Keyboard/controller
  navigate both containers and quantity controls; controls are in README.
- Chest interaction opens a session without auto-deposit. Established sessions
  survive LOS obstruction and close on range exit, unavailability or explicit exit.
- Opening inventory blocks only its owner's movement/interaction input. World
  simulation and another actor continue; collision layers/masks are untouched.
  No invulnerability flag, collision bypass or global pause was introduced.
- Selection and pointer-down tokens stay with UI previews until an authoritative
  slot command commits. Split-drag preserves selected quantity; cancelled drags,
  stale gestures and closing the menu do not remove source items.
- Reused native regions of approved PNGs. Source masters, runtime world PNGs,
  resource definitions/yields, carrying capacities, scene colliders and layout data
  remain unchanged. No resources or survival mechanics were added.

### Validation actually performed

- C# build passed with zero warnings/errors. Godot editor import passed; its first
  approval review timed out and the permitted retry completed successfully.
- Existing M2a core tests passed; new slot tests passed moves, partial merges,
  splits, whole/cross-container swaps, full targets, partial quick transfer, invalid
  requests, stale/replayed commands and reset tokens. 32 conflicting split races
  had exactly one winner. 3,000 mixed slot commands preserved resource totals and
  stack bounds (in addition to the existing 2,000-command M2a suite).
- Real-engine smoke tests passed on both authored layouts: original movement,
  normalized diagonals, walls/corners, passage, recovery, foliage, typed collection,
  chest opening, explicit deposit, slot transfers, cancellation and range closure.
  Reset during an unfinished split restored sources/empty storage and released
  modal input, with two complete reloads in each run.
- The UX review injected actual Godot mouse/keyboard/joypad events. It passed
  keyboard selection/quantity/destination, controller D-pad/stick navigation,
  controller cancellation/quick transfer, other-device rejection, mouse split,
  quick transfer, actual partial-stack drag/drop and outside-drop cancellation.
- A second actor continued moving 40 units while the menu owner remained still;
  SceneTree was not paused and owner collision remained active. A temporary physics
  blocker changed LOS while chest transfers remained available. Range exit and
  hiding the chest closed access; closed/stale requests were rejected.
- Changed chest page rows in memory to verify navigation over two pages without
  changing the 24-slot capacity. Standard 3×2 / 6×4 configuration loaded correctly.
- Graphical X11/OpenGL capture runs passed and were visually inspected. Initial
  captures exposed default Control minimum-size expansion and narrow-panel text
  overflow; fixed HUD icon rendering, button sizes and footer wrapping/padding.
  Final actual 1536×864 views are in `docs/screenshots/inventory-ux`.
- The adapted one-slot graphical chest fixture passed partial deposit, full storage,
  source-limited withdrawal and type separation through the new explicit interface.
  Compared 36 resource/layout/collision-scene/PNG files byte-for-byte against the
  approved staged baseline; all remain unchanged.
- Art export/palette/anchor/terrain checks passed. Preservation checks retain old
  snapshots and explicitly layer only approved inventory-UX runtime hashes over
  the prior M2a records. No world art was replaced or repainted.

### Review boundaries

Controller navigation was tested through injected events; no physical controller,
Windows build or network/multi-machine session was tested. The two-actor checks are
in-process simulation, not a finished multiplayer mode. Assess icon readability,
selection feedback, split controls and panel ergonomics during owner playtest.
Changes are staged for review; no commit was created. Stop before any further
survival systems or content expansion.


## 2026-11-12 — Inventory commit and M2b clock foundation (in progress)

- Revalidated inventory build/core tests and engine smoke tests on both layouts.
  Created initial project commit `24b7f70` before starting M2b changes.
- Implemented the independently authorized clock portion while awaiting required
  farming choices: 6-minute day / 1-minute dusk / 3-minute night, compact day/phase
  elapsed display, phase progress strip and developer time advancement.
- Integer simulation ticks with fractional carry passed exact phase boundaries,
  60Hz partition consistency, custom durations, multi-day skips and serializable
  state restoration tests. Existing inventory/resource/layout tests passed.
- Build passed with zero warnings/errors. Live ClockSmoke passed time while exploring
  and while inventory is open, minute/day/three-day controls, passive time retaining
  inventory previews, dusk/night boundaries and reset to Day 1.
- Existing gameplay smoke tests passed again on both layouts with the clock present.
- Graphical clock capture run passed; inspected the actual gameplay night view and
  readable clock. Captures are preserved in `docs/screenshots/clock`.
- Farming is not yet implemented. Pending owner choices: starting seed source,
  shared Food versus crop-specific foods, and full-inventory harvest behavior.
  Do not present this increment as completed M2b. No crop artwork or related
  survival mechanics were added while those decisions remain unanswered.


## 2026-11-12 — Completed M2b: basic agriculture and time

### Implemented and observed

- Confirmed owner choices: one shared starter allocation of two seeds per crop,
  one common Food resource, and all-or-nothing harvesting. Added four independent
  plots as two positions per existing garden bed on both existing layouts.
- Personal seed consumption, contextual C / controller shoulder crop selection,
  E/A planting/inspection/harvesting, passive three-stage growth, exact configured
  yields, seed returns and replanting. Full harvests remain on mature plots until
  the entire bundle fits, including returned seeds.
- Continuous configurable 6/1/3-minute cycle, compact day/phase display and developer
  minute/day/three-day skips. Existing clock integer-tick/save-state groundwork is retained.
- Farming shares the inventory state owner's lock; plot-specific session revisions
  protect planting and harvesting without making independent plots conflict.
  Growth changes no inventory revision. Starter allocation also updates inventory
  revision so an earlier snapshot cannot operate on newly inserted seed stacks.
- New crop/empty-bed/seed/Food PNGs came from the built-in imagegen matched kit;
  exact prompt, source, recipe and hash are preserved. Native palette/density and
  pixel anchors match the sanctuary standard. Gameplay inspection prompted a small
  increase in seedling footprint so sprouts read against the textured soil.

### Validation actually performed

- .NET game build and core suite passed with zero warnings/errors. Godot editor
  import completed cleanly for scripts, PNGs and the farming art manifest.
- Unit tests passed seed ownership/costs, stage thresholds, elapsed-cycle maturity,
  premature-harvest rejection, exact potato/Glowcap yields and returned seeds,
  replanting, invalid actors/crops, stale/replayed requests and fresh-session reset.
- Capacity tests covered no free slots, enough space for Food but not the seed,
  and full grids where the complete bundle fits existing partial stacks. Failed
  harvests leave plot and inventory versions/contents unchanged. Small starter
  storage rejects the complete allocation without inserting partial stock.
- Competing same-plot planting consumed only one seed; 32 two-actor race fixtures
  accepted simultaneous planting on different plots and exactly one same-plot
  harvest, with no duplicated yields. Existing inventory race/conservation suites
  (2,000 transfer commands and 3,000 slot commands) also passed.
- Full real-engine farming smoke passed on both layouts: withdraw starter seeds,
  plant all four plots with contextual inputs, grow during inventory management
  and surface exploration, inspect stage changes, harvest configured yields,
  reject replay, replant, reject a full-pack harvest, store supplies, collect the
  preserved crop and reset crops/seeds/inventory/time.
- Existing movement/collision/passage/foliage/gathering/storage/reset smoke tests
  passed on both layouts. Inventory UX event tests and clock smoke also passed.
  Old resource-only assertions now account for starter seeds and locate resource
  slots by ID, keeping those tests independent of starter stock ordering.
- Actual graphical farming capture runs passed and were inspected at intended
  1536×864 gameplay scale. Seedling, growing and both mature-crop views are in
  `docs/screenshots/farming`. Original sanctuary PNGs, terrain layouts, player
  collision scene and Scrap/Organic source data are unchanged.
- Farming export reproducibility, common 64-color palette, binary alpha, native
  resolutions, root anchors, increasing growth silhouettes and preserved source
  SHA256 passed. Previous sanctuary exports and preservation checks also passed.

### Scope and review limits

No real network or multi-machine session, physical-controller hardware or Windows
build was tested. Controller selection was tested with injected engine events;
multiplayer safety was tested in the authoritative two-actor model. Clock state and
plot snapshots serialize, but full persistent save/load is not implemented.
No hunger, consumption, cooking, power, watering, fertilizer, diseases, equipment,
extra crops, nighttime attacks, beacon countdown or world generator was added.
The two authored layouts and modular terrain path remain reusable. Stop for owner
review of this complete slice. M2b changes are staged; inventory baseline commit
remains `24b7f70`, with no new M2b commit made before review.
