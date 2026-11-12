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
