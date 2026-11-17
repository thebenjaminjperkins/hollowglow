# Hollowlight — Development Alignment Brief

Draft for owner review · 2026-11-17 · Document B

> Repository integration note — 2026-11-17: Imported from the owner's revised
> `DEVELOPMENT_ALIGNMENT_BRIEF_REVISED.md`, preserving its repository audit and
> VS0–VS8 structure. The [revised Vision Update](HOLLOWLIGHT_VISION_UPDATE_DRAFT.md)
> is the intended-design authority for subsequent planning. Milestone recommendations
> remain proposals; document-internal handoff instructions do not authorize execution.
> See the [consistency review and VS0 scope](PLANNING_REVIEW_VS0.md) for unresolved
> scope tensions and the recommended next approval gate.

Companion: [Vision Update draft](HOLLOWLIGHT_VISION_UPDATE_DRAFT.md). This is a
repository audit and proposed replacement sequence, not a change to the approved
[Vision](../Hollowlight_Vision.md), [Roadmap](../Hollowlight_Roadmap.md) or gameplay.
The owner's latest consolidation request is the intended-design authority for
this draft; working prototype behavior is evidence rather than a design veto.
Every milestone below requires scope approval before implementation.

## 1. Recommendation and audit boundary

Build toward one **15–20-minute, two-player connected survival-adventure slice**:
warm sanctuary → purposeful garage expedition → an infrastructure decision →
Bloom encounter/recovery → short final extraction. Establish a representative
visual foundation and a connected authority proof early. Then extend that same
experience rather than building independent catalogs of systems.

Keep working simulation invariants and regression profiles. Allow substantial
presentation replacement and intentional changes to prototype completion/ownership
rules where the new vision requires them. Do not blend the candidate future economy
into the live prototype during architecture work.

This review inspected the current design documents, Bootstrap, decision/validation
history, runtime JSON, core models, scene orchestration, renderer/shader, geography,
garage, test harnesses, economy-study configuration/results, Git history and saved
gameplay-scale hall/garage captures. Findings below link to primary local evidence.
No new engine run, human playtest or network validation was performed for this
documentation assignment. Historical automated results remain historical results.

### Repository state and evidence quality

- Current `HEAD` is **`6f7e0e0`**, “Complete M5a basic crafting and resource
  investment.” Earlier commits visible in this checkout include M4a `899d9af`,
  M4b `602c1cc`/`c0ae6d2`, M3b `37fd833`, M2b `7b72761` and M2a `e0a3405`.
  Several prose logs cite different historical hashes. Treat checkout history and
  file content as evidence; reconcile references at a later authorized checkpoint,
  rather than silently editing history or assuming those other hashes exist here.
- M6a/M6b/M6c, their evidence and the isolated study are present in an already
  dirty tree, with substantial untracked content. This is not a clean release.
  Four tracked `docs/screenshots/rescue/*.png` files were already deleted when this
  audit began. Do not attribute those deletions to this assignment or regenerate them.
- M5a and earlier gameplay milestones have owner acceptance. M6a/M6b review profiles
  are implemented; the original M6c technical phase was provisionally accepted.
  Hall/presentation follow-up has reported validation, but no subsequent final
  visual/gameplay acceptance is assumed. The new request explicitly identifies
  complete-world visuals as unsatisfactory.
- [Hall evidence](SANCTUARY_HALL_REVIEW.md) reports 23 existing engine cases, six
  window sizes, 16 hall seeds, six colliding actor proxies and a 48×24 clearance
  proxy. These establish specific technical properties, not connected multiplayer,
  human navigation comfort or the desired art direction.

### Design priority clarification

The 15–20-minute target is a **test scenario**, not a shorter replacement for the
full survival game. The vertical slice must prove that exploration, sanctuary work
and combat are individually enjoyable and meaningfully interdependent. It should
not merely demonstrate a checklist of technically functioning systems. Human
observation of meaningful choices, idle travel, UI friction, teamwork and relief
on returning home is a release gate.

The owner also expects a visually colorful, living world rather than uniformly
rusty assets; a sanctuary that is compact and actually feels inhabited; a reliable
closed-door security contract; and a procedural generator that encodes geographic
relationships rather than distributing random pickups.

## 2. Current prototype versus intended game

Disposition terms apply to the named boundary, not a blanket rewrite: **preserve**
working behavior/invariants; **extend** missing capabilities; **refactor** ownership
or interfaces; **replace** incompatible behavior; **defer** content with no immediate
slice purpose. A system may need more than one treatment.

| System and observed evidence | Disposition | Alignment action |
|---|---|---|
| Godot 4.4.1 .NET / net8.0, C# project, console core tests | Preserve | Keep engine baseline initially; no engine migration is justified by this audit |
| [Movement](../scripts/world/PlayerCharacter.cs), normalized eight-direction input, feet collision, following camera, directional presentation | Preserve / extend | Keep current feel while adding actor ownership, attacks, health and eventual death states; do not add unapproved stamina/weight |
| [SharedSupplies](../scripts/core/SharedSupplies.cs), atomic transfers, immutable snapshots, session/revision checks | Preserve / refactor | Reuse conservation and conflict safety; introduce host command dispatch, stable IDs and explicit permissions; split ownership only where needed |
| [ThresholdWorld](../scripts/world/ThresholdWorld.cs) partials, one `_controlledActor`, UI access validation | Refactor | Separate session simulation, spatial validation, actor/view context and local UI; replace identity checks that reject every actor except the controlled one |
| Slot-grid backpack/chest, personal ingredients, partial collection, full-harvest safety | Preserve / extend | Keep rules and tests; adapt sessions to remote actors and later death-pack/construction containers; eventual UX refinement remains useful |
| [Clock](../scripts/core/GameClock.cs), [agriculture](../scripts/core/SharedAgriculture.cs), [hunger](../scripts/core/SharedHunger.cs) | Preserve / extend | Maintain elapsed-time growth/meal accounting and gradual recovery; add intentional snapshot/bootstrap and short-profile timing, not silent new upkeep |
| [Immediate crafting](../scripts/core/SharedCrafting.cs), three recipes, installed personal backpack, fixed fifth socket | Preserve fixture / extend / refactor | Keep powered transactional recipe logic; add separate blueprint/stockpile assembly; future freely placed beds and transferable worn equipment need reviewed ownership changes |
| [Fuel power](../scripts/core/SharedPower.cs), one generator, manual switches, exact consumption | Preserve / extend | Keep safe activation rejection and accounting; add shared bus, placements, Biomass/storage only when scoped; study implementation is not live infrastructure |
| [Beacon](../scripts/core/SharedRescue.cs), seven-day terminal completion freezes clock and blocks transactions | Preserve accounting / replace terminal policy | Signal-ready must become nonterminal extraction availability; current end-of-run guards cannot merely be relabeled |
| Compact fixtures, M6a five chambers, [hall mask/reservations](../scripts/core/SanctuaryPlan.cs) | Preserve fixtures / extend hall | Use hall for sanctuary composition/security benchmark; four starting plots; future flexible zones; no automatic default swap |
| [Garage model](../scripts/core/SharedExploration.cs), finite loot, transferable tool, interruptible part commits, irreversible van salvage | Preserve / extend | Reuse meaningful exploration foundation and state sheets; preserve vehicle choice honestly without implementing restoration |
| [MountainRegion](../scripts/core/MountainRegion.cs), seeded bounded landmarks, routes, river, mirroring, approach tests | Preserve generators as fixtures / refactor and extend | Evolve graph input into explicit constraints/progression validation; support region identities and concurrent residency instead of one atlas-coordinate assumption |
| [Discovery](../scripts/core/WorldDiscovery.cs): session explored set, actor-local sight, fixed opaque mask | Preserve concepts / extend | Make blockers revisioned for doors/construction; region-scoped snapshots; separate explored memory, current sight and light in every view |
| [WorldVisuals](../scripts/presentation/WorldVisuals.cs), [shader](../scripts/presentation/world_surface.gdshader), contact metadata, wall assembly | Refactor / replace failing presentation | Retain useful metadata/tests/PNGs; select renderer and visibility treatment through a bounded scene comparison, without preserving a failed shader by default |
| Shared 64-color processed PNG pipeline, terrain TileMapLayers, independent physics | Preserve useful assets / revise standard as needed | Existing density/provenance useful; fixed palette, material bindings and terrain assembly are reviewable; prioritize colorful coherent whole scenes |
| Doors, combat, health, colonies, death, recovery, connected multiplayer and extraction | Extend (absent) | Build smallest coherent versions in the sequence below; do not claim current state models implement them |
| [14-day study](../studies/economy14/README.md) | Preserve separately | Accepted feasibility baseline, not final prices, pressure, playable resource migration or the 20-minute scenario |
| Vehicle restoration, bosses, full ecology, large recipe/location catalogs, batteries/automation breadth, PvP | Defer | Keep extension points and reserve spaces; add only after a demonstrated gameplay need |

### Protected baseline facts

The running game uses ten-minute 6/1/3 phases, seven accumulated signal days, four
initial plots with a maximum fifth via kit, two Potato/two Glowcap starter seeds,
two personal starting Food each and one Food/player/day. Potato matures in two
cycles for three Food plus seed; Glowcap in three for two Food, Organic and seed.
Fuel capacity is 4 PU with a five-Fuel tank; running burn is `0.2 + 0.2 × supplied
PU` per cycle. Beacon/workbench/lights demand 2/2/1 PU. Personal storage begins at
six slots with stack ten; the installed backpack adds two storage-only slots.
Current recipes remain 8 Scrap/3 Organic for backpack, 12 Scrap/4 Organic for bed,
and Potato seed + Organic → two Potato seeds.

These are inspected facts from `data/*.json`, not recommended final balance.
Candidate Metal/Biomass/parts, eight plots, Marrowvine, Biomass generator, revised
recipes and 14-day Standard target are **study/future design values**. No live
resource rename, quantity, recipe, yield or consumption change accompanies this brief.

## 3. Architectural findings and proposed boundaries

### A. Authority exists within a process; connected authority does not

`SharedSupplies` partials serialize inventory, farming, hunger, power, rescue,
crafting and exploration through shared state ownership/locks. Session versions,
separate power/plot versions and preflight copies are valuable. They do not provide
peer authentication, packet retry handling, a wire schema or remote access checks.
Global inventory revisions can also cause unrelated actors to receive stale-command
rejections. Measure that contention before adopting the revision scope unchanged.

The scene constructs one player inventory, fetches the node named `Player`, advances
the clock locally and performs range/LOS validation through one controlled actor.
The UI and world orchestration must stop serving as the authority boundary.

**Proposed:** one host-owned session, actor registry and command dispatcher. Resolve
peer→actor identity on the host; validate session, action state, region, proximity,
inventory, power and object revision there. Commands carry IDs for bounded duplicate
suppression plus stale-state tokens; a retry returns an existing result without a
second mutation. Send immutable snapshots/events to local views. Keep plain C#
rules and Godot spatial checks at a narrow adapter, without introducing an ECS,
distributed simulation or a new framework merely for future possibilities.

Host movement/combat authority, local responsiveness and interpolation require an
early two-process proof. Prefer a small direct-connect transport spike using the
engine's networking facilities as a candidate, not a committed transport/API design.
No client may award resources, hits, resurrection or extraction outcomes. Clock and
offscreen growth advance once on the host. A seed alone cannot synchronize depletion,
work tickets, dynamic blockers or clients on different content versions.

### B. Rendering improvements are useful evidence, not a finished foundation

The current stack uses modular PNGs/atlases, TileMapLayer ground, Y-sorted roots,
per-surface shader materials, a geography texture, exposure/shelter texture and
shadow-enabled point lights. Ground/visual bounds and contact-shadow metadata are
reusable. Saved probes show wall occlusion works under tested conditions.

However, `WorldVisuals` still traverses scene types, binds materials per drawable,
gets observer identity from `Player`, scans full regions for masks/zones/occluders
and relies on ground-footprint projection. The 16-unit sight grid remains evident
under inward feathering. Raised wall/prop art, cutaways, sprite sorting and physical
occlusion need one coherent contract; adding more patches is not an acceptance test.
Palette restriction and existing diffuse shading may also limit the colorful target.
Performance risk follows from this structure but is not measured GPU evidence.

**Proposed:** region presentation consumes read-only semantic surfaces and an explicit
local observer context. Asset definitions carry anchor, visual bounds, footprint,
sort plane, occlusion/roof roles, contact profile and optional emission separately.
Terrain/architecture own consistent joins; structures can emit collision, navigation
and sight/light boundaries from a common definition without making those systems
identical. Compare a retained-shader revision with a separated fog/compositing approach
in one hall–threshold–garage benchmark. Either must hide unseen dynamic state and
support different observers. Choose by moving-gameplay results and profile data,
not preference for more shaders or fewer shaders. No renderer/palette choice is
approved by this recommendation.

### C. Dynamic spatial truth must precede meaningful doors and combat

Current sight copies a static opaque grid; wall/light/nav geometry is mostly built
once. Doors, construction, creatures and dropped backpacks add changing state.
Define region/object identities and spatial revisions, with explicit updates for
collision, traversal, sight and light occlusion. Rebuilding everything every frame
or leaving a closed door transparent to hostile perception are both failure modes.

Door state must preserve the new security contract, remain usable unpowered, and
handle obstructed closure predictably. Combat actions need host timing, unique hit
resolution, actor eligibility, cancellation and world collision. Reuse responsive
movement; do not couple attack animation to client-awarded damage or invent hunger
health damage. Downed/dead/boarded eligibility must be checked by the same authority.

### D. Geography is a good graph prototype, not a general constraint solver

Current regional rules express bounds, attachments, optional route chance and
source-to-landmark allocation. Generation uses a fixed mountain/river relationship,
small landmark ranges and identity/mirror orientation. Region dimensions have
explicit limits and sanctuary/surface live in one offset coordinate space.
It cannot yet express the full count/exclusion/dependency/progression specification
or simultaneous independently loaded regions.

**Proposed stages:** region graph → terrain/infrastructure → constrained landmark
selection/placement → authored modules → resource/knowledge/tool allocation →
access/progression/extraction validation → decoration. Start with explicit rules
and bounded deterministic retries, not a universal solver. Failed worlds produce
seed, failed constraints and diagnostics; never silently remove an essential gate
or add supplies. Keep stable generated IDs plus sparse per-session deltas for
depletion, discoveries, construction and ecology. Regions with players simulate
normally; farm/power/time rules must also progress correctly away from home.

### E. Rescue completion is entangled with the global stop condition

`RescueState` currently marks completion terminal; `SharedRescue` limits clock advance,
freezes it and raises completion. Other operations consult `RunCompleted`, and
`ThresholdRescue` pauses the tree. A cosmetic “Extraction ready” label would still
freeze the run and prevent the expedition.

**Proposed:** separate signal accumulation, extraction opportunity and terminal run
outcome. Exact-time completion produces one opportunity event, not a global freeze.
Boarding/departure resolves full/partial rescue once; an empty departure with
survivors remains nonterminal; true full-party death terminates independently of
chamber resources. Keep the seven-day terminal fixture until the new profile passes.
Whole-session snapshots must eventually cover these states; a serializable clock
alone is not a save game.

## 4. Proposed milestone sequence

Use **VS0–VS8** as draft labels to avoid colliding with historical M numbers. Work
proceeds on a separate cooperative review profile. Existing compact, M6a, garage,
hall and seven-day rescue fixtures stay available. Each step ends in a review,
with its declared tests and a playable demonstration; no automatic advancement.

### VS0 — Reproducible baseline and short-session contract

- **Objective/scope:** Select the exact two-player demonstration, reconcile checkout
  history references, record preserved fixtures and resolve blocking slice choices.
  Design the 15–20-minute scenario clock without changing normal play.
- **Reuse:** Existing manifests, seed profiles, tests, balance studies and hall/garage
  evidence. The current HEAD is an anchor, not a substitute for untracked M6 evidence.
- **Changes needed:** Approved scenario contract, profile/config separation, input
  and run scripts, explicit host/client scope and acceptance checklist. An authorized
  commit checkpoint is recommended before implementation, not performed here.
- **Dependencies:** Approval of these drafts; timing and minimum death/extraction scope.
- **Risks:** Compressing only beacon time leaves crops irrelevant; compressing the
  whole clock also changes meal deadlines and fuel burn per real minute.
- **Automated validation:** Existing build/core/runtime fixtures; assert legacy config
  hashes unchanged and every selected profile/content version reproducible.
- **Human criteria:** Explain the intended home–expedition–return–extraction loop and
  its success/failure conditions without assuming future catalogs are present.
- **Complete when:** Owner approves scenario timing, controls/platform scope, minimum
  investments, failure rules and preservation list; executable baseline is captured.

### VS1 — Representative visual foundation, with architecture decision

- **Objective/scope:** One coherent hall, entrance, road approach and existing garage
  at gameplay scale. Repair terrain/architecture/composition, colorful greenery,
  grounding, sorting, fog, interior/day/night light and restrained asynchronous life.
  Avoid world-wide art replacement or new locations.
- **Reuse:** Useful PNGs/state sheets, anchors/footprints, TileMapLayer where suitable,
  contact metadata, lighting probes, HUD tests and matched captures.
- **Changes needed:** Explicit observer/region presentation input; bounded alternative
  fog/render trial; revised modular art/palette specification if required; common
  architecture/occlusion definitions. Produce candidates separately before migration.
- **Dependencies:** VS0; architectural contracts reviewed alongside VS2 before either
  is frozen. No demand to finish a final renderer before proving networking.
- **Risks:** Cosmetic detail hiding sorting bugs, incompatible baked shading, dynamic
  information leaks, per-object material/light cost, UI obscuring interaction.
- **Automated validation:** Wall/roof/sprite overlap cases; no hidden object-state leak;
  illumination cannot grant discovery; lamp probes; window/aspect tests; unchanged
  gameplay geometry where intended; capture and hardware/software profile evidence.
- **Human criteria:** A moving character reads correctly around cabinet, van, wall
  and vegetation; home feels warm, outdoors colorful/mysterious, night readable;
  different surfaces feel intentional rather than noisy or uniformly rusty.
- **Complete when:** Owner accepts the whole-scene comparison in motion, selected
  presentation architecture and reusable authoring rules, with known limits recorded.

### VS2 — Two connected players and one authoritative session

- **Objective/scope:** Host plus one client direct-connect, independent input/cameras,
  shared time/resources/discovery, conflicting inventory/garage/farm/power commands.
  Prove simultaneous sanctuary/surface occupancy using existing region geometry.
- **Reuse:** Core transactions, snapshots, locks, actor IDs, slot UI and movement.
- **Changes needed:** Session/actor registry, host dispatcher, bootstrap snapshot,
  duplicate-safe command/results, remote actors and per-view rendering. Remove
  `_controlledActor` authorization shortcuts; retain local controlled-view ownership.
- **Dependencies:** VS0; VS1 observer contract. This proof can precede final VS1 art
  acceptance, but must pass before adding networked combat/construction complexity.
- **Risks:** Client spoofing, stale-global-version churn, duplicate work commits,
  reset epochs, client clocks, input latency, snapshot races and hidden-state leakage.
- **Automated validation:** Launch two processes; simultaneous chest/harvest/part races,
  retry/reordering/stale session requests, one clock owner, snapshot consistency and
  content mismatch rejection; scripted latency/loss where feasible. Model tests alone
  do not satisfy this gate.
- **Human criteria:** Two people on separate machines can move, gather, trade and split
  locations without one player's menu/camera pausing or redirecting the other.
- **Complete when:** Reproducible two-process tests and separate-machine play pass;
  host-loss/join policy is documented and a dedicated host remains architecturally possible.

### VS3 — Constraint-driven excursion and progression placement

**Scope safeguard:** This is a narrow *existing-garage expedition* and one-region-
boundary proof, not a full procedural-world overhaul. Do not allow general
constraint-solver architecture or multi-region content production to delay the
first networked encounter. If VS2's region identity contract permits it, defer
nonessential procedural variety until after the first combat/recovery playtest.


- **Objective/scope:** Upgrade the local region rules around the existing garage,
  sanctuary and reserved extraction/recovery sockets. Vary meaningful route/landmark
  relationships, not only decoration; retain authored modular building circulation.
- **Reuse:** M6a graph/streams/approach checks, M6b finite contents/tool/state changes,
  discovery and physical expedition measurements.
- **Changes needed:** Count/adjacency/suitability/separation/access constraints, stable
  region/object IDs, progression dependency validation and deterministic failure reports.
  Add one small connected test region/route boundary to prove two actors may remain
  in different regions without resetting time/depletion; no new themed location library.
- **Dependencies:** VS1 spatial contract and VS2 authoritative bootstrap/identity.
- **Risks:** Key behind own lock, disconnected extraction, mirrored road/building mismatch,
  essential loot consumed by optional allocation, empty travel and retry bias.
- **Automated validation:** Seed batch with connectivity, resource conservation, reachable
  sockets, essential-acquisition/alternative-path checks and valid initial/second-chance
  extraction paths; revisit/disconnect-region tests retain state. Bounded retries emit
  diagnostics. Test adverse dependency arrangements, not only many easy seeds.
- **Human criteria:** Garage is plausibly road-connected, routes/landmarks recognizable,
  one expedition offers meaningful carrying/salvage decisions and optional content
  can remain unvisited. Measure actual discovery/decision/return time; do not pad timers.
- **Complete when:** Approved seed set varies meaningful geography and passes required
  constraints; two actors can occupy distinct region instances concurrently; 2–4-minute
  expedition target is evaluated by humans with activity breakdowns.

### VS4 — Sanctuary security and one construction investment

- **Objective/scope:** Mechanically operated paired doors and one useful blueprint
  project in a construction zone. Deliver materials locally, assemble briefly and
  commission it; preserve four starting plots and demonstrate a farming/power choice.
- **Reuse:** Hall reservations, bed/crop PNGs and transactional kit/recipe model,
  personal/chest inventories, powered bench and power-allocation feedback.
- **Changes needed:** Dynamic spatial blockers, obstruction-safe door actions,
  blueprint footprint/stockpile/assembly states, host-validated deposits and placement.
  Recommend one additional bed as the first project; recipe routing/cost treatment
  requires explicit approval rather than silently replacing the existing fifth socket.
- **Dependencies:** VS2 commands, VS3 region/spatial validation, approved placement and
  cancellation rules. VS5 supplies the final ordinary-enemy security proof.
- **Risks:** Materials lost by cancellation, two builders completing twice, blocked
  circulation/doorway, remote assembly or abandoned stockpile duplication.
- **Automated validation:** Partial deposits/assembly, competing builders, rejected
  footprints, disconnect/reset, conservation, revisioned navigation/sight/light changes,
  manual no-power door operation and wide-body passage. Preserve legacy recipes.
- **Human criteria:** Two people can deliver/build without menu friction; project choice
  changes the next expedition or harvest; no routine caretaker; doors feel substantial
  and their current clearance/state is unambiguous.
- **Complete when:** One networked project is installed/operating with all inputs
  accounted for, safe cancel/recovery behavior, and functional paired doors. Full
  security acceptance waits for VS5; no generic catalog or nightly siege.

### VS5 — First Bloom encounter and credible retreat

**Experience priority:** This is the first decisive test of whether Hollowlight
is fun outside the sanctuary. Test aiming, attack cadence, threat telegraphs,
positioning, noise response and retreat with two humans; do not accept a passing
hit-resolution test as proof of satisfying combat. Ensure the garage objective
contains something worth doing beyond reaching a container.


- **Objective/scope:** One deliberate weapon and one readable ordinary Bloom archetype
  in the existing excursion; responsive movement, avoidance, engagement, retreat and
  a modest local disturbance response. Dangerous exposure must complement exploration.
- **Reuse:** Movement/collision, region sight, dynamic doors, tool work cancellation,
  actor authority and the accepted environmental presentation.
- **Changes needed:** Host-owned health/hits, attack timing/telegraphs, bounded AI states,
  obstacle-aware movement, damage feedback and transferable weapon ownership. Door/
  wall protection must apply to perception and all attack forms used by this enemy.
- **Dependencies:** VS2/VS4; approved controls, damage/death threshold, inventory behavior
  under attack and friendly-fire scope. VS6 follows immediately for recovery play.
- **Risks:** Animation/hit mismatch, attacks through walls, client hit claims, latency,
  unavoidable stun loops, safe-home promise broken by pathing/projectiles.
- **Automated validation:** Duplicate hit suppression, range/wall/eligibility checks,
  simultaneous attacks, moving targets, interruption, pursuit through open entrances
  and inability to harm sanctuary occupants through closed intact boundaries.
- **Human criteria:** New players can read and evade an attack, cooperate or retreat;
  weapon feels deliberate without sluggish movement; home is reliably relieving.
- **Complete when:** Real connected players understand engage/avoid/retreat counterplay,
  security tests pass and selected control/timing values are accepted. No boss roster,
  full ecology, XP, routine durability or automatic hunger damage.

### VS6 — Death, dropped equipment and recovery

- **Objective/scope:** Persistent death backpack, teammate retrieval, limited emergency
  recovery and immediate true full-party-wipe failure. Evaluate a downed/revive state
  if approved; do not infer its wipe semantics. Show the damaged chamber honestly.
- **Reuse:** Slot containers/conservation, host action checks, health/death events,
  region object IDs, damaged alcove and once-only completion safeguards.
- **Changes needed:** Explicit living/downed/dead/recovering states, one death/drop
  transaction, owner-independent backpack access, recovery eligibility/charges and
  terminal wipe arbitration. Define disconnected players before counting survivors.
- **Dependencies:** VS5 and approved emergency count, downed/wipe, respawn and loss rules.
- **Risks:** Equipment copied on resurrection, capacity overflow, simultaneous death/
  recovery races, disconnected actors keeping a wiped run alive indefinitely.
- **Automated validation:** One backpack per death, all contents conserved, partial
  retrieval/full capacity, empty resurrection, concurrency and reset. Last living
  death terminates despite recovery stock; retries cannot recover after terminal wipe.
- **Human criteria:** Dying hurts without opaque loss; teammate can locate/recover gear;
  dead player understands options; failure and remaining recovery charges are clear.
- **Complete when:** Two-player death/retrieval/recovery/wipe tests work end to end.
  Renewable chamber repair/Biomass costs remain deferred unless explicitly selected
  as the slice's single investment; emergency recovery is not claimed as full design.

### VS7 — Beacon opportunity and short final extraction

- **Objective/scope:** Signal readiness opens a world-dependent evacuation opportunity;
  dangerous travel, boarding and departure resolve full/partial rescue. An empty
  departure with survivors permits a costly second opportunity with an additional
  feasible world objective. Use one small authored objective pattern, not a quest system.
- **Reuse:** Exact powered transmission/outage accounting, contextual beacon UI,
  generated route/sockets, host state/events, combat and terminal idempotence tests.
- **Changes needed:** Separate opportunity and terminal state machines; remove rescue-
  ready clock/mutation freeze in the new profile; authority validates boarding,
  departure, left-behind outcomes and recovery prerequisites.
- **Dependencies:** VS3 extraction/recovery feasibility, VS5/VS6 survival and explicit
  extraction-window/second-chance rules. No optional vehicle/boss/tool can be the
  sole required path unless an essential alternative is guaranteed.
- **Risks:** Ready-state freezes farm/power, duplicate transports, last-frame board/death
  races, solo survivor softlock, remote boarding or impossible recovery expenditure.
- **Automated validation:** Powered-only progress, skips/outages, exactly-one opportunity,
  continuing simulation after readiness, full/partial/empty departure, costly recovery
  path, wipe precedence and late/repeated network commands; preserve old ending fixture.
- **Human criteria:** Objective known from start, location/window legible, departure tense
  but fair; preparation and route knowledge help; being left behind is clearly signaled.
- **Complete when:** All four outcome classes run correctly across two processes and
  human pairs complete the short evacuation without developer intervention. Exact
  handling of further missed attempts must be approved before claiming full-game coverage.

### VS8 — Integrated cooperative slice evaluation

**Core design question:** Would two friends voluntarily play another run? Record
where curiosity, pressure, collaboration, surprise and relief actually occurred,
not only whether they completed the objective. If the slice feels like a walking
simulator with occasional menus, revisit activity design before adding regions.


- **Objective/scope:** Evaluate the 15–20-minute experience, fix integration defects
  and publish evidence. This is a playtest gate, not a balancing license.
- **Reuse:** Every preceding slice plus legacy regression and isolated economic evidence.
- **Changes needed:** Minimal onboarding, contextual/audio feedback, scenario telemetry
  recorded locally for development only if approved, or manual observation sheets.
  No accounts, external analytics or automatic economy changes.
- **Dependencies:** VS1–VS7 accepted, with the shortened scenario explicitly configured.
- **Risks:** Feature presence mistaken for fun, one player carrying all useful work,
  scripted success masking lack of choices, tests missing human UI/network friction.
- **Automated validation:** Complete two-process seeded runs; farm/food/power/assembly/
  combat/death/extraction invariants, legacy fixtures and repeatability; compare all
  changed configuration values explicitly. Simulated 4/6-player stress is supporting
  evidence, not proof that real 4/6-player play is ready.
- **Human criteria:** At least three fresh pairs, one unfamiliar with the prototype;
  record first-session duration, pillar/activity time, choices, idle/chore time,
  deaths/recoveries, salvage returns and completion/failure comprehension. Repeat
  one seed and try a different arrangement; note what changes decisions.
- **Complete when:** Owner accepts a reproducibly playable 15–20-minute two-player
  run where all three pillars contribute, including at least one return home,
  harvest and investment decision, meaningful Bloom encounter, tested recovery and
  final evacuation. Report failure as well as success; no claim of final multiplayer
  balance or final Gameplay & Economy Brief follows from this gate.

## 5. Pacing, economics and evidence that constrain this plan

The live clock cannot simply be called a 20-minute slice: Potato takes roughly
20 real minutes from planting, Glowcap 30, and seven signal days require at least
70 powered minutes. The study's fourteen-day target requires at least 140 powered
minutes at the same clock. A two-day run would also barely exercise food pressure
with two personal starting Food. These are design conflicts to resolve explicitly,
not reasons to alter quantities while documenting the plan.

**Recommended for approval:** a separate short-scenario clock/target, keeping crop
durations in simulation days and preserving all legacy profiles. For discussion
only, a four-minute cycle would mature Potato/Glowcap after 8/12 real minutes and
three signal days would take 12 powered minutes, leaving a final travel window.
Those illustrative numbers are **unapproved**, change real-minute consumption and
must be replayed chronologically before use. They do not prove a good 20-minute
experience or replace the fourteen-day Standard candidate. An alternative is a
scenario-specific accelerated farming schedule; it is more independent of meals/
power but less representative. Do not choose either silently.

A proposed pacing envelope, also for review: minutes 0–3 orientation/planting and
allocation; 3–8 exploration/salvage/encounter; 8–12 return, harvest/build and recovery;
12–16 preparation/second objective; 16–20 extraction. Flexible parallel work and
failures will move these boundaries. The second-chance branch may exceed the normal
20-minute target; its maximum duration is a separate approval decision, not hidden
automatic defeat at minute 20.

Measured M6a excursions were 13.4–78.2s round trip; garage runs 90.5–93.5s with
test-speed UI. [Garage measurements](M6B_TRAVEL.md) explicitly say this is an efficient
lower bound, not a measured engaging 2–4-minute human visit. Hall storage walks are
1.30s workshop, 1.75s garden, 2.22s power, 2.88s radio, 3.73s inner entrance; longest
station pair 4.08s. Preserve useful home convenience. Increase meaning through
investigation, danger, carrying and decisions before extending roads or hold timers.

The accepted [14-day study](../studies/economy14/README.md) reports all 81 scenarios
rescuing at Day 15 00:23, no missed meals/outages under its policies, 24/27 baseline
underground confirmations and only about 5.5–12.6% occupied actor time. Two other
confirmations are censored by rescue; one strategy cannot afford its generator.
Fuel and regrowing wild Biomass provide substantial slack. None of those scenarios
models combat, death, connected play or final extraction. Preserve this feasibility
baseline; revisit balance only after the meaningful exploration/danger loop is tested.

## 6. Ranked risks and decision gates

| Risk | Early evidence required | Design/technical guardrail |
|---|---|---|
| Whole-world presentation remains unconvincing | VS1 moving hall/garage/day/night comparison, two observers, profile data | Replace failed architecture if warranted; no art-library expansion before scene acceptance |
| Networking arrives after assumptions harden | VS2 two-process and separate-machine conflicts/traversal | Host validates actions and owns clock; rendering/input depend on local view, not simulation ownership |
| Combat breaks movement, security or recovery | VS5 wall/door/latency cases followed promptly by VS6 | Ordinary enemies cannot bypass closed security; no client-authored damage or item grants |
| Seeded worlds are connected but unwinnable | VS3 acquisition/extraction dependency checks with bad seeds | Guaranteed essential opportunities, bounded retries and explicit infeasibility; no silent resource top-ups |
| 20-minute demonstration misrepresents full economy | VS0/VS8 chronological short-profile replay plus human sessions | Version/profile separation; record accelerated assumptions; do not infer 14-day balance |
| Construction/death broaden ownership unsafely | VS4/VS6 conservation across every container/state | One authoritative commit; visible local stockpiles and recoverable death contents |
| Offscreen regions/time lose state | VS2/VS3 split-party tests, VS7 nonterminal signal readiness | Session-owned simulation and region deltas; no scene-tree pause on beacon-ready |

### Consequential decisions remaining before dependent implementation

| Decision | Options/tradeoff and recommendation | Needed by |
|---|---|---|
| Slice pacing versus Standard duration | Separate accelerated simulation/target profile (recommended) versus independent mission/farming overrides; former coherent, latter more surgical but less representative. Approve actual timings after chronological preflight; Standard 14 days remains candidate | VS0 |
| Scope of visual replacement | Bounded renderer/palette comparison (recommended) versus retaining current stack by default; compare motion/readability/cost before selecting new rules | VS1 |
| Connected-session policy | Recommend direct-connect host/client, start together, explicit host-loss ending and no migration for first slice; confirm join-in-progress/reconnect expectations and initial desktop/controller targets | VS2 |
| Shared current sight | Shared explored memory + observer-local current sight is current model and recommendation; shared live team vision is easier coordination but changes information. Decide minimap/teammate markers and hidden-state replication separately | VS2 |
| Door obstruction and use | Recommend refuse closure while occupied with clear feedback; pushing/crushing creates new combat rules. Confirm control reach/permissions and ordinary-enemy attack boundary | VS4 |
| Construction ownership/cancel | Recommend explicit deliveries into accessible local stockpile and no destruction of deposited inputs on cancel; decide exact return destination/overflow, who cancels, kit-versus-raw-material commissioning and initial project cost | VS4 |
| Resource/equipment migration | Keep Scrap/Organic and existing recipes during proofs (recommended); later versioned mapping to Metal/Biomass/parts. Decide installed-backpack transfer/death treatment and equipment slot rules rather than assuming kit ownership suffices | Before migrated content / VS5–VS6 |
| Combat and downed/wipe | Recommend one deliberate weapon/enemy and no PvP in slice. Approve aiming, timing, damage, friendly-fire handling, downed expiration/revive and whether all-downed converts promptly to death; never silently redefine all-dead wipe | VS5–VS6 |
| Recovery | Approve limited emergency count/cost, respawn location, activation and dead-player wait. Recommend defer repair-to-renewable Biomass resurrection until recovery is enjoyable; do not claim chamber ownership already provides it | VS6 |
| Extraction/recovery attempt | Approve location disclosure, transport window/boarding/abandonment warnings, race precedence and feasible costly second-chance objective. Recommend one objective pattern plus generated sockets; decide further misses and branch duration explicitly | VS7 |
| Long-run infrastructure/persistence | Approve Biomass/battery/upgrades effects, flexible-zone caps, save/resume and server-region lifecycle in later slices; narrow profile bootstrap now, not a promise of disk persistence | After core slice unless directly required |

## 7. Recommended immediate handoff

1. Obtain owner approval of both edited drafts and the exact baseline/dirty-tree
   handling. Do not overwrite approved historical documents automatically.
2. Scope VS0 as a **separate review profile** with explicit timing assumptions and
   a short playable home–garage–return–extraction scenario.
3. In parallel, define the minimal observer/region/authority contracts needed by
   the bounded visual comparison and two-process network proof. Avoid speculative
   abstractions or a full content migration.
4. Stop at the first whole-scene visual checkpoint and at the first two-human
   connected-play checkpoint. Reprioritize quickly if either fails to demonstrate
   the intended experience.

## 8. Deferred scope and review stop

Do not use this sequence to implement the project catalog, vehicles, a boss roster,
full ecological simulation, routine durability, PvP, public matchmaking, dedicated
server deployment or final economy balance. Preserve future doors/transport clearance,
shared knowledge, region identities and commodity extensibility without speculative
frameworks. A small disturbance/ordinary-threat test can precede colonies/migration;
one blueprint project can precede Biomass/battery/automation breadth.

This assignment creates only these two drafts. Existing approved Vision/Roadmap,
gameplay, data, assets, shaders, generation and studies remain unchanged. Approve
or revise the design consolidation and proposed sequence first; then scope VS0 and
the bounded visual/network proofs. Stop here before implementation and before the
final Gameplay & Economy Brief.
