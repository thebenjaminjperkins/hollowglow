# Decisions

Dates use the owner's America/New_York timezone.

## Decided — 2026-11-11

- The owner authorized project initialization and only M1, The Threshold.
- Preserve the Hollowlight Vision; no economy, farming, combat, procedural world,
  rescue progression, or multiplayer implementation in this slice.
- Godot .NET, C#, Git, and a VS Code-friendly layout follow Bootstrap.
- Personal carried salvage and communal storage are the minimal approved loop.
  This does not establish later ownership, capacity, or resource-economy rules.
- Owner selected direct eight-direction movement with immediate starts/stops,
  orthogonal top-down view, and a camera following the controlled actor.
- Owner selected warm amber caves, teal luminous plants, gray-green ruins, and
  original 16-pixel motifs displayed at 3× scale as the provisional visual baseline.

## Provisional engineering choices — 2026-11-11

- Pin Godot.NET.Sdk 4.4.1 / net8.0 for a documented compatible baseline.
- Pure C# salvage state is owned by the world and addressed by actor/object IDs.
  Input and rendering do not own resource quantities. No networking framework.
- Standalone console assertions test real model source without external test packages.
- One configurable decoration seed is reserved for repeatable details, not world generation.
- Build outputs and caches are excluded; user source documents remain unchanged.
- Compact authored rooms share a continuous two-tile-wide passage; collision and
  camera limits apply across both spaces without scene-transition loading.
- Three salvage objects each contribute one unit. No capacity, weight, hunger,
  resource taxonomy, or crafting system is implied by these slice fixtures.
- Garden beds, generator, and plants are decorative silhouettes with no simulation.
- Native Godot drawing commands render original pixel motifs and textured tiles;
  all art in `scripts/presentation/PixelSprite.cs` and `ThresholdMap.cs` was created
  for this project. No external assets, AI image assets, or fonts were imported.
- A 384×216 logical canvas scales by integer multiples, starting at 1152×648.
  Movement uses Godot CharacterBody2D in floating mode, normalized diagonal input,
  fixed physics ticks, and an 8×6 feet collider; exported speed defaults to 80 px/s.

## Open — later review

- In-game movement feel, camera framing, atmosphere, and interaction readability
  still require a real playtest; approval of the baseline is not visual validation.
- All consequential later-milestone rules remain open as listed in the Vision.

## Approved M1.5 — 2026-11-11

- After reporting working movement/resource interaction, the owner requested a visual
  milestone before further survival mechanics and approved the proposed recommendation.
- Direction: organic, softly shaded pixel art; irregular caves, natural vegetation,
  amber artificial light, restrained teal bioluminescence, salvaged technology,
  approximately 16×24 characters, 16×16 terrain and the existing 3× scale.
- Preserve all existing gameplay functionality and the Vision. Evaluate this milestone
  before M2 or any new survival system. The Roadmap now records this intervening M1.5.

## M1.5 implementation choices — provisional

- Terrain is a reusable neighbour-aware kit of floor variants, rock faces, exposed
  edges and chamfered corners. The floor/collision edge derives from the same layout.
- Two authored data files (`data/sanctuary*.json`) hold cave masks and independent
  prop placements. `--sanctuary-variant` selects Rootfall Nook instead of Hearth Hollow.
  These demonstrate reuse, not a procedural generator or a decision to generate refuges.
- `scenes/SanctuaryProp.tscn` combines native pixel motifs, optional rectangular feet
  collision and a local light. Materials, roots, decking, textiles and vegetation are
  individual modules, not painted into a whole-room image. Art remains original project code.
- Dynamic PointLight2D pools provide warmth; mushroom light varies by only 6% over
  a slow cycle. Lights are unshadowed, decorative, and do not imply power mechanics.
- Nested Y sorting orders props and characters by their feet. Character animation
  reads velocity; the original movement rules, speed and feet collider are unchanged.
- HUD counters remain visible; location labels time out, prompts appear only nearby,
  feedback uses text-sized toasts, H toggles help, and F1 retains debug information.
- Actual graphical captures and owner evaluation are separate: captures establish
  visual inspection, while subjective atmosphere/feel acceptance remains with the owner.

## Approved four-asset benchmark — 2026-11-11

- The owner approved a bounded quality benchmark after reviewing the native drawing
  pipeline. Adopt palette-indexed PNG sheets for the player, generator, fern and wall.
- Keep 16px terrain, roughly 16×24 characters, 384×216 viewport and default 3× integer
  scaling. Use one 32-swatch source palette, deliberate connected clusters, upper-left
  form shading, restrained local light, and material-specific wear/texture.
- Editable source pixels and layer metadata live beside runtime PNG exports.
  Other motifs remain unchanged until this benchmark has been evaluated.
- Eight-direction normalized movement, speed, collider, inputs and resource rules
  remain intact. Four-direction animation uses a stable diagonal-axis tie rule.
- Fern reaction is visual only: a moving body entering a nonblocking trigger selects
  authored bend poses, then settles. No plant harvesting, damage or movement penalty.
- The built-in imagegen tool supplied a preserved concept reference. Production
  sheets are separately authored indexed pixels; no procedural rectangle renderer
  or cropped/generated reference image is used to draw those four assets at runtime.
- Stop for owner quality review before broadening the asset library or survival scope.

## Approved separate machinery extraction trial — 2026-11-11

- The owner approved a machinery-only trial that uses processed reference pixels as
  the provisional quality target, instead of a separately simplified approximation.
- Preserve the existing benchmark and gameplay unchanged. Do not migrate assets or
  expand the library. All trial runtime code/assets live in separate files.
- Test a 64×64 native frame and provisional 64-swatch palette. The separate window
  uses a 768×432 logical render grid, 2× camera zoom and 2× display, preserving the
  existing 384×216 world coverage, movement, collision and interaction measurements.
- Preserve source aspect and foot anchor. The old and candidate frames draw into
  the same world rectangle; the wider reference silhouette consequently reads lower.
- Use a recorded crop/mask/resampling/quantization recipe on the original reference.
  No new generated artwork or redraw is used in this trial.
- Evaluate one-pixel-per-source native assets, 1× native-window output, and intended
  2× gameplay output. Stop for review before any pipeline adoption or library work.

## Approved cohesive sanctuary-room benchmark — 2026-11-11

- The owner approved the machinery candidate as a quality reference and requested
  one cohesive, reusable room before wider game expansion. The workflow was
  explained before source production and implementation.
- Use a matched PNG furniture/vegetation kit, the approved machine, reference-derived
  character and modular terrain atlases. Native density is 2 pixels/world unit;
  every source asset uses the same 64-swatch palette and nearest integer scaling.
- Adopt real Godot TileMapLayer/TileSetAtlasSource for terrain rendering/assembly.
  Preserve existing simulation collision, and disable duplicate tile physics.
- Keep the benchmark opt-in through `scenes/SanctuaryRoom.tscn`, preserving earlier
  scenes and project defaults. This benchmarks one refuge; the existing surface
  remains usable for the unchanged salvage loop.
- Lamp/ambient lighting is live and adjustable with L. Sources retain material-form
  shading but do not contain a painted whole-room lighting/background composition.
- Generated source sheets, exact prompts, processing recipes, PNG exports, placement
  anchors and actual capture evidence are retained. Do not replace art with simpler
  geometric approximations or add random noise to simulate material quality.
- Reuse the same assets on the second existing floor mask for validation; do not
  implement procedural world generation or a large asset library.
- Stop for owner evaluation after validation. No survival mechanics are approved.


## Approved sanctuary integration — 2026-11-11

- Owner approved the cohesive sanctuary as Hollowlight's provisional art direction
  and asset-production standard. New assets must match its native pixel density,
  palette, material texture, shading and actual gameplay-scale readability.
- Normal project entry is now `scenes/SanctuaryRoom.tscn`, mounting the original
  Threshold simulation and approved RoomPresentation. Preserve the original home
  spawn (136,208), camera world coverage, movement, collisions and interaction range.
- Retain PNG sprites, independent adjustable lighting and modular TileMapLayer
  assembly from occupancy data. Historical scenes and source masters remain intact.
- Animation alone now reads actual post-collision travel at physics ticks. Keep
  the approved full-speed 8fps cadence, reduce cadence during slower wall sliding,
  and idle when blocked. Four-direction facing and diagonal-axis ties remain.
- Validate fixed boot registration at every exported pose. Current exported PNG
  bytes remain unchanged; this integration does not redraw or broaden the art kit.
- Proposed M2a Shared supplies is recorded in the Roadmap, awaiting explicit owner
  approval of ownership, resource types and carrying limits. No new resource,
  inventory interface, survival consumption, crafting or network system was added.


## Approved M2a — Shared supplies — 2026-11-11

- Owner explicitly approved personal inventories and communal storage, two initial
  resources (Scrap and Organic Material), six personal slots and stack size ten,
  larger configurable storage, finite gathering and safe deposit/withdrawal.
- Definitions and sites live in `data/supplies.json`; no resource-type switch is
  needed in inventory logic. Reuse the existing three sites/footprints and approved
  PNGs. The organic site uses the existing mushroom sprite; no art library expansion.
- Provisional review configuration: 24 chest slots (editor-configurable 1–256),
  12 Scrap / 8 Organic / 14 Scrap across the three finite sites. These quantities
  exercise stacking and partial transfer behavior, not survival economy balance.
- E gathers everything that fits or deposits all at the chest. Tab opens the pack;
  1/2 or comma/period selects a resource; Z deposits one and X withdraws one;
  Shift transfers up to ten. The panel does not pause movement. Every transfer
  rechecks the actor and spatial access, including line of sight, in the world.
- Private inventory state is owned by one engine-independent SharedSupplies model.
  One lock covers validation and both ends of every mutation. Immutable snapshots
  carry session/revision tokens; stale/conflicting/retried accepted commands cannot
  apply again. A new session invalidates commands issued before reset.
- Partial operations retain all remainder at source. Stacks never mix types, partial
  matching stacks fill before empty slots, and bulk deposit follows resource-definition
  order. Players can instead select a resource explicitly when storage space is scarce.
- This establishes an authoritative command boundary and tests in-process two-actor
  contention. Actual networking, identity authentication, packet delivery/retries and
  multiplayer sessions remain later Roadmap work; no network reliability claim is made.
- Preserve approved art, procedural-ready terrain assembly, camera coverage, movement
  and collision behavior. No weight, encumbrance, crafting, hunger, fuel or power.
  Stop after M2a validation and owner review before any further gameplay milestone.


## Approved inventory UX refinement — 2026-11-11

- Owner approved the proposed two-panel inventory and authoritative slot operations.
  Preserve M2a resources/data, six slots, stack limit ten and configurable communal
  capacity. Presentation defaults are 3×2 backpack / 6×4 chest, configurable separately.
- Reuse approved PNG source regions as item icons at native density; do not introduce
  a new palette, world asset family or geometric substitute for existing artwork.
- Tab/View opens backpack anywhere. E/A near the chest opens both inventories,
  replacing automatic deposit with an explicit Store all action. Mouse selection,
  drag/drop, keyboard/controller focus, quick transfer, splitting, merges and whole
  swaps share authoritative command methods, not presentation-owned item containers.
- Inventory holds only the controlled character stationary. The tree/world is not
  paused and collision is unchanged; no invulnerability state or damage exemption
  was added. Controller input is assigned to the actor's device, not every device.
- Opening chest access retains the original proximity/interaction validation.
  Ongoing access deliberately ignores LOS changes, closing only on range exit,
  unavailable chest or interface/scene exit. No automatic shared access from backpack.
- Quantity selection starts at half, commits only after choosing a destination,
  and cancels without moving items. Partial mismatched swaps are rejected. Remainders
  stay at source. Pointer-down/selection version tokens prevent stale gestures from
  moving replacement stacks. Replayed commands are rejected by the existing owner.
- Scope ends after UX validation and owner review; no new resources, crafting,
  hunger, power, equipment or other survival mechanics are approved.


## M2a accepted; M2b first increment — 2026-11-12

- Owner accepted the inventory milestone and explicitly requested a commit before
  M2b. Revalidated build/core logic and both-layout engine smoke tests, then created
  initial project commit `24b7f70` (the repository had no earlier commits).
- M2b authorizes a continuous 6/1/3-minute phase cycle, compact clock and debug skips,
  four sanctuary plots and the two specified crops only. No broader survival scope.
- The clock uses authoritative simulation ticks plus fractional carry, not wall time.
  Its serializable state can be resumed without accumulating update-partition drift.
  Display shows day, phase and elapsed phase time; developer skips do not mutate
  inventory revisions or invalidate selections. Reset restarts at Day 1.
- Crop duration will mean complete elapsed cycles after planting, not calendar
  day boundaries. Proposed placement is two positions per existing garden bed,
  preserving current collisions and authored-layout/terrain assembly.
- Farming-dependent choices are awaiting the owner: source of starter seeds,
  shared Food versus crop-specific food items, and full-inventory harvest behavior.
  No seed distribution, farming interactions or crop assets have been implemented yet.


## Confirmed M2b farming decisions — 2026-11-12

- Owner selected the recommended defaults: two Cave Potato seeds and two Glowcap
  seeds in communal storage, one common Food resource and whole-harvest capacity
  checks. Full inventories leave the mature crop completely intact, including its seed.
- Crop definitions, stage threshold, seed costs, yields, initial stock and per-bed
  offsets live in `data/farming.json`. Original Scrap/Organic definitions and finite
  salvage are unchanged. Farming adds only the two seed types and common Food.
- Two positions in each existing bed provide four plots on both layouts. Existing
  garden collision bodies remain unchanged; plot interaction points lie on the
  accessible front edge. Rendering uses separate empty-bed and growth-stage PNGs.
- E/A plants, inspects or harvests contextually; C or LB/RB selects the crop while
  near an empty plot. No farming menu or inventory ownership rule is introduced.
- Farm mutations share SharedSupplies' lock. Plot-specific session revisions reject
  same-plot conflicts/replays while independent plots accept different actors.
  Seed removal and plot planting are atomic; complete inventory-fit simulation occurs
  before harvest/seed return, so failure changes neither inventory nor mature crop.
- Clock advancement alone changes no inventory revision. Growth derives from planted
  integer ticks and crop duration; no per-plot timers or attention/upkeep system.
- Preserve the configured chest capacity. Startup stock must fit in full; the default
  farming setup needs two or more slots. The historical one-slot fixture disables
  farming explicitly and retains its original capacity/transfer checks.
- Built-in imagegen supplied a small matched source kit. Prompt, original source and
  recipe are preserved under `assets/farming/source`; runtime exports use the same
  64-swatch palette, 2px/world density, binary alpha and nearest display convention.
  Seedlings were enlarged within native frame budgets after gameplay-scale review.
- M2b ends here for owner review. No hunger, consumption, cooking, power, watering,
  diseases, advanced equipment, extra crops, attacks, beacon countdown or world
  generator was implemented. Actual multiplayer transport and persistent saves remain later.


## M3a approved food consumption and gradual recovery — 2026-11-12

- Revalidated and committed approved M2b as `453114a` before changing gameplay.
- Owner approved hybrid personal eating, 1 Food/day, 2 starting personal Food per
  actor, dawn settlement and warning/10%/20% movement penalties. No automatic
  communal withdrawals. No crop, seed-stock or four-plot rebalance.
- Owner adjusted recovery: satisfying a day reduces accumulated misses gradually,
  rather than erasing a prolonged starvation streak. Default implementation removes
  one per completed daily requirement, configurable as `recoveryPerMeal`. Manual
  completion recovers immediately; dawn does not recover that credit again. A
  partial meal does not recover, and an incomplete daily deadline adds one miss.
- Fed starts without free meal credit; the first obligation closes at Day 2 dawn.
  Food cannot be banked as future credit or eaten beyond the current requirement.
  Existing Food moved to the common catalog with edible metadata; no live resources
  were added. Synthetic extra edible types exist only in tests.
- Inventory authority owns consumption, meal credit and recovery under one lock.
  The clock binds to that lock before session exposure; every advance settles all
  crossed dawns. Session/revision/day requests reject retries and stale input.
  UI remains presentation and commands, with contextual mouse/F/RS Eat controls.
- Owner explicitly accepts the six-player deficit pending separate future surface
  food scavenging, outdoor agriculture and infrastructure. M3a changes none of those.
- Seven-day simulations use authoritative crop/inventory/meal operations and finite
  care schedules, report daily stock and missed deadlines, and conserve every Food
  unit. They are timing scenarios rather than measured human multiplayer sessions.
- Preserve all approved PNG/atlas art, modular terrain, camera, collisions, ranges,
  inventory capacities, transfers, crop data and clock phases. Actual procedural
  generation, transport, persistent saves and late joining remain future work.
- Stop after validation for owner review; no additional systems are approved here.


## Design documentation revision — proposed sequence — 2026-11-12

- Owner requested a documentation/planning update after M3a, without authorizing
  new gameplay or technical initialization changes. Bootstrap remains unchanged.
- Vision v0.3 records the owner's current direction: Acquire/Sustain/Protect,
  protective manual sanctuary doors, safe underground versus productive exposed
  agriculture, finite surface supplies, optional player conflict, competing
  capability investments and recoverable radio interruptions.
- Resource categories (Metal, Biomass, basic/advanced machine parts, Food, Fuel,
  Seeds) are provisional. Existing Scrap/Organic IDs, farming values and functioning
  systems remain intact; this update performs no resource or balance migration.
- Roadmap v0.3 preserves completed milestone history and distinguishes M3a's
  completed implementation/validation from pending owner acceptance. Old economy
  examples are archived rather than treated as active defaults.
- Proposed next sequence: finite Food scavenging, minimal generation/consumers,
  powered beacon, a bounded repair/crafting choice, manual entrance doors, then a
  connected survival-loop review with an early real-cooperation proof gate.
  This sequence awaits approval; it is not implementation authorization.
- Procedural surface, threats/rescue, exposed agriculture, broader generation/
  storage, complete rescue runs, multiplayer reliability and later progression
  remain separately reviewed increments. Optional conflict does not implicitly
  enable lethal friendly fire. No mandatory nightly attacks or nightly crop wipes.
- Beacon outage thresholds/penalties/extraction, conflict modes/lethality,
  door obstruction/breach rules, resource naming/migration and future production
  costs remain unresolved. Recommendations in the revised plan are proposals.
- Approved M3a work is preserved independently. No gameplay, tests, runtime data,
  artwork or Bootstrap changes were made as part of this documentation revision.


## Approved revised direction and M3b lean Food scavenging — 2026-11-12

- Owner approved the revised Vision/Roadmap, then explicitly approved M3b with six
  finite sources, unchanged near/far bands and leaner per-type quantity ranges.
- Near: two wild plants at 1–2 Food and one container at 1–3. Far: one plant at 1–2
  and two containers at 2–4. These sum to 8–17, rather than the approximate 9–17;
  retain the explicit ranges without a hidden top-up or player-count scaling.
- Instant gathering, partial collection, safe full/depleted behavior, reproducible
  seeded placement/quantities and distinct available/depleted visuals are approved.
  Both types yield the existing Food resource; wild plants do not regrow.
- Resource placement runs on existing authored terrain. Near/far shortest walkable
  path bands are 32–112 / 160–288 world units. Preserve all existing blockers and
  actor feet. Sources are nonblocking ground salvage with a clear validated approach,
  32-unit source spacing, and 48-unit clearance from original salvage sites.
- WorldSeed defaults to 1701 and can be set with --world-seed=. Specified seed streams
  separate placement, quantities and decoration. Sources initialize once per run;
  gathering and time do not reroll. R starts a new debug run with identical initial
  seed/layout/configuration; this is not automatic respawning or a disk-save system.
- Built-in imagegen (imagegen skill) supplied two original matched transparent source
  families. Preserve prompts/masters, shared-palette PNG exports and processing under
  assets/scavenging. Existing sanctuary/farm/benchmark assets are unchanged.
- Timed 2/4/6-player simulations use real seeded worlds at 8, 14 and 17 Food, actual
  gathering/transfers, unchanged farm visits and personal meal rules. Report each
  day's unmet meals and reserves; do not rebalance the accepted six-player deficit.
- Farming values, four plots, hunger/recovery, personal/chest limits, original salvage,
  starting Food/seeds and clock values are preserved. No power or additional survival
  system is approved here. Stop after validation for review before power generation.


## M3b accepted; M4a scoping requested — 2026-11-12

- Owner accepted M3b and explicitly requested a commit of the validated milestone.
  Revalidation passed build/core tests, both-layout scavenging, hunger and pixel-art
  checks. The complete baseline includes previously uncommitted M3a and approved
  Vision/Roadmap updates required to reproduce the accepted game.
- Next requested slice is named M4a: fuel-consuming generation, limited capacity,
  manually selected consumers and machine on/off controls. No automatic priorities.
  Beacon power representation belongs in M4a; rescue progress belongs to M4b.
- This renames the old M3c/M3d power/beacon proposals; the historical M4 procedural
  surface proposal stays future work. No world-generation expansion is authorized.
- Only proposing capacities, fuel rules, overload, consumer loads, UI and seven-day
  estimates is authorized now. M4a implementation requires owner approval. Existing
  crops must retain their behavior; future grow-light dependence is not established.


## Approved M4a with non-destructive allocation — 2026-11-12

- Owner approved 4-PU generation, 2-PU beacon/workbench and 1-PU supplemental lights,
  five-Fuel tank, two shared starting Fuel, manual loading and two finite caches
  (near 2–3 / far 3–5). No player-count scaling or automatic communal withdrawal.
- Preserve continuous burn formula 0.2 + 0.2 × supplied PU per complete clock cycle.
  Integer credit counters preserve fractions; stopping burns nothing, exhaustion
  stops at zero, and refueling never restarts automatically. Whole items transfer
  only if they fit; partial tank space cannot silently destroy the rest of a unit.
- Owner explicitly rejected the proposed breaker trip for overload: reject only
  the activation, show requested demand/capacity and preserve running consumers.
  No automatic priority, destructive trip, fault, damage or rescue progression.
- Start before enabling additional consumers. Stop/empty retains switch selections
  without supply; restart is explicit. Users can disable selected switches while off.
- Power and refueling commands share SharedSupplies' authority lock with inventory
  and clock callbacks. Separate power revisions avoid invalidating menus on every
  burn tick; inventory tokens protect manual loading. Captured pointer-down tokens
  reject stale clicks. No playable multiplayer transport or disk-save claim.
- Reuse existing generator, workbench, communications shelf and lamps. Status diodes
  and task/supplemental lighting reflect power; ambient/bioluminescent visibility stays.
  The only new original imagegen family is Fuel canisters, with preserved source,
  full prompt, icon and palette-normalized two-state PNGs under assets/power.
- Generalize the Food placement module to FiniteScavenging, retaining Food's exact
  default domains/IDs/ranges and independent quantity/placement streams. Fuel gets
  its own domain and respects Food as nonblocking ground props. Terrain stays authored.
- Existing farm growth, stages, yields, four plots, Food/material nodes, hunger,
  starting Food/seeds and inventory limits are unchanged. No grow lights or cooking,
  crafting output, batteries, alternate generators, faults or combat added.
- Stop after M4a validation for owner review before M4b rescue progression.


## M4a commit authorized; M4b rules awaiting approval — 2026-11-13

- Owner approved M4a on reported validation, pending final hands-on confirmation,
  and explicitly requested its commit before M4b. Preserve that distinction; no
  hands-on acceptance or hardware/network validation is implied by the commit.
- Revalidated build, core/concurrency tests, seven-day Fuel balances, power on both
  layouts, hunger regression and artwork checks before committing the baseline.
- Requested M4b: seven full elapsed signal days at the existing 2-PU beacon load,
  continuous powered-only progress, pause on interruptions, configurable grace/
  degradation and a proposed three-day uninterrupted-outage reset. Precise outage
  parameters require owner approval before any M4b code is implemented.
- Prototype victory may use a notification/screen rather than surface extraction.
  Post-victory simulation behavior is another consequential approval choice.
  Preserve food, power, recoverability and all other systems; no new failure rule.
- Inspect partial-interval accounting when a time skip crosses Fuel exhaustion:
  rescue must receive exactly the powered portion before the outage begins.
