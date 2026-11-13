# HOLLOWLIGHT
## Development & Playtesting Roadmap | v0.3 — M4a validated baseline; M4b scoping

**Companions:** [Vision](Hollowlight_Vision.md), [Bootstrap](Hollowlight_Bootstrap.md),
[decision history](docs/DECISIONS.md) and [validation log](docs/PLAYTEST_LOG.md).

This is an incremental plan, not authorization to implement future milestones.
The owner has approved M3a's implementation scope, including gradual recovery;
M3a is implemented and validated. The revised Vision/Roadmap were approved, followed
by explicit approval of the leaner M3b Food-scavenging scope. M3b is implemented, validated and accepted by the owner. M4a was subsequently approved with non-destructive capacity rejection and is now
approved on reported validation pending final hands-on confirmation. Its validated
baseline is authorized for commit. M4b outage parameters still need owner approval
before implementation. Bootstrap has
no changed initialization or technical requirements and remains unchanged.

## 1. Current position and priority

The immediate priority is the **first connected survival loop**, not a complete
content library, combat economy or infrastructure simulator. Players should acquire
finite resources, sustain personal meals and critical infrastructure, protect their
home and recover from shortages while working toward a functioning rescue signal.
Keep each step independently playable, test it, report and stop for review.

| Work | Current status |
|---|---|
| M0/M1 initialization and The Threshold | Completed; first playable authored refuge/surface |
| M1.5 art benchmarks and sanctuary integration | Approved and integrated; modular PNG/atlas standard |
| M2a shared supplies and inventory UX | Approved; included in commit `24b7f70` |
| M2b agriculture and continuous time | Approved; committed as `453114a` |
| M3a food consumption and hunger | Implemented/validated; preserved in accepted M3b baseline `5e6abff` |
| M3b finite surface Food scavenging | Approved and completed; committed baseline includes M3a and approved design revisions |
| M4a basic power generation and manual allocation | Reported validation approved; commit authorized; final hands-on confirmation pending |
| Subsequent gameplay | Proposals only; separate approval required |

Live play controls one actor. Core models support independent actor inventories,
meal states and conflicting operations; tests and simulations do not constitute
playable multiplayer. There are two authored layouts with reusable TileMapLayer
terrain, not a procedural world generator. M4a provides basic power and a beacon consumer indicator. Rescue progress, crafting,
protective doors, enemies, damage, outdoor farming and transport remain future work.
M3b adds seeded resource placement, not terrain generation.

## 2. Rules for development

1. Read the Vision and current decisions before proposing work. Ask about conflicts.
2. Implement only an explicitly approved milestone or smaller slice. A design goal
   in this document is not implementation authorization.
3. Ask before consequential gameplay or visual choices; offer options, tradeoffs and
   a recommendation. Resolve ordinary engineering details independently.
4. Keep input, presentation and authoritative simulation separate. Preserve working
   systems, data, conservation rules and reusable art; migrate deliberately, not silently.
5. Favor complete vertical slices and the smallest useful resource catalog. Add no
   resource category until a concrete interaction needs it.
6. Use configurable rules, deterministic tests and brief manual checks. Distinguish
   model simulations, controller events, real hardware and network-session evidence.
7. Keep the approved art standard at gameplay scale. Do not reopen the visual pipeline
   or generate a broad asset library for each mechanic.
8. Record decisions and playtest results. Report a milestone and stop for approval.

## 3. Completed milestone history and approved M3a work

The following records preserve the original scope, acceptance checks and exclusions.
Exclusions apply to their named slice; later explicit approvals extend the game
without erasing the earlier implementation history. Their historical review gates
do not reopen already accepted work. Commits and current status are summarized above.

### M0. Initialize the project — completed
**Goal:** A reproducible, clean foundation.

- Set up the Godot 4 .NET C# project, Git, VS Code configuration, build instructions, and basic test harness.
- Establish scene organization, configuration data, seed handling, and a minimal main menu or debug launch path.
- Add placeholder assets only as needed to validate the setup.

**Exit:** Fresh clone can be opened and built with documented prerequisites; project launches; repository is clean; known limitations documented.

**Ask owner:** Any blocking environment or platform decisions not already documented.

### M1. The Threshold: feel and place — completed
**Goal:** Experience the contrast between underground home and surface.

- One small underground sanctuary, one surface clearing, and a clear passage between them.
- Responsive top-down movement, camera, collision, simple interactions, and a few collectible objects.
- Recognizable early pixel-art silhouettes, warm versus muted palettes, and an obvious distinction between safety and exposure.
- Debug tools for teleport/reset and inspecting interactions.

**Exit:** One player can leave the refuge, collect something, return, and store it; the experience is understandable without explanation.

**Playtest:** Does movement feel good? Does the refuge feel like somewhere to return to? Can players tell what is interactive?

**Ask owner:** Art-direction references, movement feel, camera framing, and sanctuary mood before substantial art work.

### M1.5. Sanctuary Identity — approved and integrated
**Goal:** Evaluate a cozy, mystical underground home before expanding survival mechanics.

- Approved current standard: reference-processed PNG sprites, shared 64-color palette,
  2 native pixels/world unit, 32×32 terrain cells (16×16 world), 40×56 character frames,
  warm adjustable lamps and restrained teal bioluminescence. The normal playable entry
  uses this cohesive kit with unchanged simulation and 384×216 world camera coverage.
- Earlier lower-resolution benchmarks remain historical comparisons. Future assets
  must pass the production guide's native and gameplay-scale consistency review.
- Replace the rectangular refuge with irregular cave contours, alcoves, natural vegetation,
  salvaged work areas, and small signs of habitation.
- Use reusable terrain rules and prop scenes with independent art, footprints, and lights.
  Demonstrate two authored arrangements using the same kit; do not implement world generation.
- Improve four-direction character presentation and depth ordering; make the HUD compact
  with contextual prompts and optional help/debug information.
- Preserve movement, collision, salvage collection/deposit, reset, and debug recovery.
- Approved refinement: establish the four-asset visual benchmark (character,
  salvaged machine, reactive vegetation, cave wall) using shared-palette spritesheets
  before expanding the library. Preserve diagonal movement and gameplay rules.

**Exit:** Both arrangements pass existing gameplay checks, captures are visually inspected,
and the owner evaluates warmth, mystery, navigation, character readability, and HUD space.

**Gate:** Stop after reporting. No additional survival systems until this visual milestone
has been evaluated and the owner approves further work. The broader M8 pass remains later.

### M2a. Shared supplies — approved and completed
**Goal:** Gather finite materials, bring them home and redistribute them through shared storage.

- Generalize the existing single salvage count to typed per-player stacks and a
  communal chest, with explicit deposit and withdrawal and a compact contents panel.
- Reuse the three existing finite surface salvage sites and existing approved art.
  Two approved types: Scrap and Organic Material; definitions remain extensible.
- Retain instantaneous E gathering, range/line-of-sight checks, depletion and reset.
- Verify conservation, depleted-source rejection, transfer limits and independent
  actor inventories using two simulated actors; playable networking remains later.
- Keep art density, modular atlases, footprints, camera and movement unchanged.

**Acceptance:** Gather, deposit, inspect the chest and withdraw; repeated commands
cannot duplicate resources. Source + carried + communal quantities remain conserved.
Reset restores the authored supply. Existing refuge smoke tests continue to pass.

**Approved:** Personal inventories plus communal storage; six slots per player,
ten units per slot. Communal capacity is configurable (24 slots initially).
Transfers/gathering partially fit, retaining all remainder at the source. One state
owner validates atomic mutations and rejects conflicting/stale session revisions.

**Review gate:** Validate limits, partial transfers, full storage, depleted nodes,
conservation and reset, then stop for owner review before the rest of M2.

**Excluded:** Hunger, food, fuel, power, crafting, repairs, regeneration, day/night,
combat, new regions, world generation and multiplayer. Evaluate this transfer loop
before authorizing any resource consumption or the rest of M2.

### M2a UX. Visual inventory refinement — approved and committed
**Goal:** Make the working inventory loop convenient and visually legible.

- Configurable 3×2 backpack and 6×4 chest grids with approved PNG item icons/counts.
- Backpack anywhere; communal contents only after a nearby chest interaction.
  Keep established sessions through minor LOS changes; close on range exit,
  unavailability or interface/scene exit. Opening does not deposit automatically.
- Mouse/keyboard/controller selection, drag/drop, quick transfer, quantity splitting,
  merging and full-stack swaps. Explicit Store all; minimal permanent HUD.
- Hold only the inventory owner stationary. World/other actors keep running; no
  invulnerability, collision change or global pause is introduced.
- Atomic slot commands share the existing authoritative state owner/version checks;
  selection and drag previews hold no inventory items.

**Acceptance:** Validate slot limits, partial operations, stale/conflicting commands,
cancellation, session access, modal input, resets and navigation. Inspect gameplay-scale
captures; distinguish simulated controller input from physical-device validation.

**Gate:** Stop for owner review. Resource definitions/yields, capacities, approved
world art, movement outside inventory and all other gameplay remain unchanged.
No new survival systems, resources, crafting or equipment are authorized.

### M2b. Basic agriculture and time — approved
**Goal:** Establish a small underground farming loop and continuous simulation time.

- Inventory milestone approved and committed as `24b7f70` before this work.
- Configurable 6-minute day, 1-minute dusk, 3-minute night, compact phase clock,
  developer skips, continued advancement during inventory use/exploration.
- Requested farming scope: four plots; Cave Potato (2 elapsed cycles, 3 Food + 1 seed)
  and Glowcap (3 elapsed cycles, 2 Food + 1 Organic Material + 1 seed).
- Data-driven crop definitions/yields and three readable growth stages; personal
  seed consumption and safe harvesting through the authoritative inventory owner.
- Preserve both authored layouts and modular terrain assembly. No procedural world
  generator currently exists; this slice must not silently add one.

**Approved decisions:** Shared starter supply of two seeds per crop in the communal
chest; one common Food resource; all-or-nothing harvests that leave the mature crop
intact when its complete yield does not fit. Two independent planting positions per
existing bed preserve collision footprints. Per-plot revisions and one inventory
owner serialize conflicting actions while allowing different actors on different plots.

**Implemented:** Clock, developer controls, contextual crop selection, planting,
automatic three-stage growth, complete harvesting, returned seeds and replant/reset.
Crop/clock/plot parameters and pixel assets are data-driven and modular.

**Validation/exit:** Seed consumption, stages, continuous growth, maturity checks,
correct yields, capacity safety, conflicting harvests, resets and time consistency;
existing gameplay tests and actual visual inspection. Stop for owner review.

**Excluded:** Consumption/hunger, cooking, electricity, watering, fertilizer, diseases,
advanced equipment, additional crops, nighttime attacks and beacon countdown.
This scope supersedes the earlier broad M2/M3 sequence only for the requested slice.

### M3a. Food consumption and hunger — implemented for owner review

**Approved scope:** Hybrid manual eating and dawn settlement from personal inventory
only; two starting Food per player; one Food/day; accumulated missed-meal penalties
with gradual recovery. M2b was committed as `453114a` before implementation.

- Current daily requirement is credited once; dawn consumes only the shortfall.
  No communal auto-withdrawal, future meal banking or wasted extra eating.
- Default recovery removes one accumulated miss per satisfied daily requirement,
  once only, whether manually or automatically. Configurable recovery, thresholds,
  movement multipliers, daily requirements and personal starting Food.
- Hungry at one miss (warning); Starving at two (90% movement); Critical at three+
  (80%). No health, stamina, combat, death or incapacitation introduced.
- Compact status, contextual Eat action in the existing inventory, independent
  player states and atomic inventory/meal operations. Every time skip settles dawns.
- Four plots and crop values stay unchanged. Timed seven-day simulations for 2/4/6
  players include finite planting, harvest, replant and personal resupply schedules.
  Six-player shortages are accepted; future scavenging/outdoor agriculture and
  infrastructure remain future work, with no economy rebalance in this slice.

**Validation:** Core and concurrency tests, integrated mouse/keyboard/controller-event
checks, hunger recovery and movement, farming/clock/inventory regression checks on
both layouts, reset, conservation and gameplay-scale visual inspection. Details:
[meal rules](docs/M3A_HUNGER.md), [daily balance ledger](docs/M3A_FOOD_BALANCE.md).
M3a implementation and validation are preserved in the accepted M3b baseline.
Broader power/beacon/crafting scope remains unapproved. No implementation is authorized
by the planning proposals below.

## 4. Proposed route to the first connected survival loop

**Sequencing approved; implementation is gated per slice.** M3b is accepted and
committed. M4a was separately approved and is implemented below for review. M4b
and later slices remain proposals. No step silently rebalances crops, renames
resources or introduces later danger systems.

### M3b. Finite surface Food scavenging — approved and completed

**Goal:** Give expeditions immediate finite relief while agriculture remains renewable.

- Six seeded sources: near two wild plants (1–2 Food each) and one container (1–3);
  far one wild plant (1–2) and two containers (2–4 each). The explicit approved
  ranges total **8–17 Food**; no top-up to the owner's approximate 9–17 is applied.
- Near/far path-distance bands remain 32–112 / 160–288 world units. Deterministic
  placement/quantity streams, reachable interaction approaches and spacing respect
  current terrain/props. No player-count scaling, rerolls or respawn timers.
- Instant gathering into existing personal inventory, partial collection, retained
  remainders, full-pack safety, visible depleted PNG states and same-seed reset.
- Same Food item and authoritative gathering/transfers; farming, hunger, capacities,
  starting Food/seeds and original salvage values remain unchanged.
- Two original imagegen source families processed into palette-matched PNG sheets;
  material shading and live environmental lighting remain independent.

**Validated:** Core generation across 256 seeds on both layouts, concurrent gathers,
conservation/capacity/depletion/reset; live approach collision, E collection, storage,
eating, both PNG states and time skips; previous gameplay regressions and graphical
inspection. [Guide](docs/M3B_SCAVENGING.md), [daily balances](docs/M3B_FOOD_BALANCE.md).

Regular farm care plus timely scavenging meets all tested 2/4/6-player deadlines.
The eight-Food world with delayed care leaves six players two meals short on Day 7;
a late far expedition causes five misses on Day 6. No other systems were rebalanced.
Actual terrain generation, hostile surface danger and power remain outside this slice.
**Gate:** Owner accepted M3b and requested its commit (`5e6abff`). Subsequent M4a
approval is recorded below; M4b is not authorized.

### M4a. Basic power generation and manual allocation — validated baseline accepted pending hands-on

Formerly M3c; named M4a by the owner. M3b baseline committed as `5e6abff` first.

- Approved generator: 4 PU, five-Fuel tank, two shared starting Fuel in the chest.
  Manual personal-inventory loading, start/stop and consumer switches.
- Consumers: beacon 2 PU, workbench 2 PU, supplemental lighting 1 PU. Beacon is
  powered-state feedback only; workbench has readiness/task lighting, no crafting.
- Owner's adjustment: reject an activation above capacity, warn with requested
  demand/capacity, preserve powered consumers. No breaker trips or automatic priorities.
- Continuous burn: 0.2 + 0.2 × supplied PU per cycle while running. Exact fractional
  credits, no off-state burn, no automatic refill/restart, safe whole-unit tank loading.
- Two seeded finite Fuel caches: near 2–3, far 3–5. Same authority/gathering rules,
  reachable approaches, partial collection and depleted PNGs; no player scaling.
- Generator panel and local machine controls expose state, active switches, spare
  capacity, Fuel, rate and runtime. World time continues during controls.
- Existing crops/four plots, Food/material sites, hunger, starting Food/seeds and
  inventory capacities are preserved. No grow-light dependence or growth bonus.

**Validation:** Exact conservation, fractional/time subdivision, depletion, tank limits,
finite seeded caches, concurrency/stale commands, reset, real input-event/UI checks,
previous gameplay regressions and inspected graphical captures. [Guide](docs/M4A_POWER.md),
[daily 2/4/6-player Fuel ledger](docs/M4A_FUEL_BALANCE.md). Scheduled operation costs
4.9 Fuel/week, full load 7; costs are shared rather than multiplied per player.

**Gate:** Owner approved the reported validation pending final hands-on confirmation
and requested committing M4a before M4b. M4b implementation waits for approval of
specific outage rules and consequential victory behavior. Grow-light effects and
all additional survival systems remain outside that slice.

### M4b. Rescue progression and recoverable signal interruption — later proposal

Formerly M3d. M4a represents the beacon as a consumer only; signal-day progression
and outage consequences belong here.

**Goal:** Make power allocation serve a visible group objective.

- Connect a critical beacon load to the tested power model; display signal and
  rescue progress without requiring a new management menu.
- Make short interruptions recoverable and prolonged loss consequential.
- Use developer controls to test qualifying time, recovery and reset; preserve
  continuous world time and independent player food needs.

**Ask:** What earns progress, difficulty-dependent duration, short-outage grace,
prolonged-loss threshold and reset versus substantial delay. Recommendation: test
pause for short interruptions and a bounded delay for long ones before testing a
full reset. This recommendation is not an approved rule. Final extraction stays open.

**Exit:** Group choices visibly affect the beacon; outages and recovery are legible
and deterministic. No final extraction sequence or automatic defeat rule is assumed.

### M3e. One bounded repair/crafting allocation choice

**Goal:** Give the workshop a useful decision rather than a recipe library.

- Test one small repair or craft loop with a meaningful alternative use of its
  material, such as restoring infrastructure versus improving expedition capability.
- Introduce machine-part distinctions only if the chosen interaction needs them.
- Reuse authoritative inventory operations and accessible contextual UI. Preserve
  existing item IDs until a reviewed migration is actually needed.

**Ask:** The exact repair/craftable, competing investment, ingredients, duration,
workstation behavior and any capacity or equipment-slot changes. A working workshop
prop does not imply approval for weapons, armor, backpacks and construction together.

**Exit:** Spending is atomic and understandable, cancellation/full inventories are
safe, and the result changes a practical capability. No sprawling crafting tree.

### M3f. Manually controlled sanctuary entrance

**Goal:** Establish the sanctuary's safety contract before introducing intruding enemies.

- Test one substantial entrance door with intentional opening/closing, readable
  state and coherent collision/navigation. Players control it at any time.
- Preserve usable exits and safe handling of actors/items in the doorway.
- Prepare the same collision/entry rules for later creatures; no enemy roster or
  mandatory attack schedule is required to validate the first door slice.

**Ask:** Placement/art, control reach, access permissions, obstruction behavior,
closing on actors, durability/breach assumptions and how to test meaningful safety.
Closed doors must matter; they are not declared invincible by this plan.

**Exit:** Manual access and blocked entry work predictably; the sanctuary is not
subject to compulsory nightly attacks. Actual enemy-entry tests follow in M5.

### M3g. First connected survival-loop review and early cooperation gate

**Goal:** Evaluate acquisition, meals, power, communications and home management
as one experience before adding broad world/progression systems.

- Connect the preceding approved slices on the compact authored map. Test finite
  depletion, returning to share supplies, competing spending, short interruptions
  and recovery. Use shortened developer sessions before a full-length run.
- Simulate timed strategies for 2/4/6 players, including harvest/travel delays and
  unsuccessful expeditions. State which economies remain unsustainable and why.
- Request a small real two-player local or host/client proof before relying on
  claims about cooperative labor. Preserve the authority boundary; defer broad
  lobby, reconnect and platform support to M7.

**Ask:** Which local/online configuration to prove first and its input/camera/hosting
scope, the minimum prototype ending, and any balance changes supported by evidence.
No networking architecture or winner/loser condition is chosen by this document.

**Exit:** One connected loop can be explained, played and recovered from without
manual inventory fixes. Separate actual cooperative evidence from simulations.
This is the first survival-loop prototype, not the full combat/extraction experience.
Review and approve the next scope before proceeding.

## 5. Later proposals after the connected-loop review

### Procedural surface and escalating exploration — historical M4 planning entry

Retain this future scope after the connected-loop review; its old M4 label does
not authorize or replace the newly requested M4a/M4b power and rescue slices.

- Seeded, connected regions with recognizable landmarks, abandoned structures and
  finite supplies. Validate safe starts, essential-resource availability and return routes.
- Nearby depletion pushes travel outward; distant and dangerous sites offer valuable
  rewards. Reuse modular terrain/props and existing interactions rather than replacing them.
- Test carrying/travel limits before adding vehicles. Refuge generation remains a
  separate choice; modular authored sanctuaries stay valid.

**Exit:** Multiple seeds produce understandable, fair exploration decisions and useful
outward pressure. **Ask:** World scale, landmark authorship, refuge layout policy and
how resource distributions remain fair across player counts and seeds.

### M5. Surface danger, situational combat and teammate rescue

- A small readable threat set, environmental hazards, retreat and stronger night
  exposure. Test creature entry through open doors and safety behind closed ones.
- Combat is sometimes necessary across a run, but encounters can be avoided through
  preparation, movement, stealth or clever choices. Valuable hostile sites/creatures
  provide reasons to take risk; not every creature must drop a crafting ingredient.
- Introduce the smallest health/weapon/protection and downed-rescue scope required
  by the approved threat test. Do not silently turn hunger into lethal health damage.
- Preserve forgiving early play and quiet nights at home. No compulsory nightly siege.

**Exit:** Danger, avoidance, retreat, rescue and entrance consequences are legible.
**Ask:** Enemy aesthetic/behavior, evasion rules, damage/lethality, friendly-fire
handling, door breaches and teammate-rescue timing. Optional betrayal is not
implemented as a side effect of a damage system.

### M5b. Exposed production and infrastructure protection

- Preserve safe, space-limited underground farming. Test a small outdoor extension
  with higher yields or faster growth, using existing crops where practical.
- Connect outdoor exposure to actual tested hazards, especially at night. Protection,
  lighting or other investment can reduce risk; crops are not destroyed by a nightly rule.
- Apply the same productivity-versus-exposure principle to other infrastructure only
  if the first agricultural test produces meaningful choices.
- Compare a second generation strategy or small power storage increment separately
  when their different costs/benefits would create a useful allocation decision.

**Exit:** Both indoor and outdoor production have reasons to exist; risk and mitigation
are understandable. **Ask:** Yield versus speed advantage, hazard/damage model,
protection effects, energy costs and any changes to crop definitions. No rebalance
is authorized by this planning entry.

### M6. Complete group rescue run

- Assemble the tested systems into a complete run with clear progress, recovery,
  an approved extraction and explicit victory/defeat conditions.
- Start from the provisional seven-day target, then test difficulty-dependent lengths.
- Include only a few capability-changing investments that create tradeoffs: weapons
  and armor, backpacks/tools, farming, power/storage, repairs/protection. Do not
  require every branch for the first complete run.
- Decide save/resume scope before long sessions. Revalidate the complete group loop,
  not just aggregate resource totals, for 2/4/6 players.

**Exit:** Groups can finish or fail a run without developer intervention and describe
useful role division and recoverable setbacks. **Ask:** Extraction, loss conditions,
difficulty presets, full-run length, persistence and the minimal upgrade roster.

### Later progression and optional conflict — separately approved slices

Transportation, expanded workstations, resurrection infrastructure and optional
player conflict remain intended directions after the first loop is evaluated.
Prioritize investments that unlock destinations, carrying ability, production,
protection or teammate recovery rather than only statistical increases.

- Prototype DNA/food/biomass/power reconstruction only after downed rescue is tested.
- Test optional player damage/killing in explicitly selected modes/settings. Scarcity
  may invite selfish allocation or betrayal; killing must not become the best way
  to obtain supplies. Do not enable lethal friendly fire implicitly.
- Treat transport as an expedition/logistics change, not a mandatory early vehicle system.

**Ask:** Mode defaults/consent and conflict access, lethality and safeguards, death/
resurrection timing and costs, vehicle form and upgrade priorities. No class system,
large item catalog or meta-progression is established.

### M7. Multiplayer completion and reliability

Build on the early cooperation proof rather than defer all multiplayer risk here.
Validate 2–6-player world/seed, time, inventories, farming, hunger, power, doors,
threats and rescue state. Test local/online configurations actually supported,
join timing, disconnect/reconnect, host loss and latency; mixed local/remote play
is a target to scope, not an assumed delivered feature.

**Exit:** Real-device and multi-machine tests pass, with limitations recorded.
**Ask:** Hosting/lobby experience, supported platforms, reconnect/host-loss policy
and whether mixed play is necessary for the first release.

### M8. Cohesion, audio and balance

Preserve the approved visual standard as new modules enter the game; do not defer
coherent art until this pass. Add restrained audio, atmosphere and readable warnings.
Run repeated seeded and real group playtests; compare strategies and shortages for
2/4/6 players. Profile before optimizing or enlarging the library.

**Exit:** Players understand decisions, enjoy downtime and want another run.
**Ask:** Any major visual/audio change and content expansion supported by playtests.

## 6. Balance baseline, provisional categories and historical fixtures

### Current implemented baseline — preserve until a separate change is approved

- Ten-minute cycles: 6 minutes Day, 1 Dusk, 3 Night. Time continues in inventory.
- One Food per player/day, hybrid personal eating, two personal starting Food.
  First deadline is Day 2 dawn. No communal auto-withdrawal or future credit banking.
- Gradual hunger recovery: one accumulated miss removed per satisfied daily
  requirement by default. Warning at one miss, 90% movement at two, 80% at three+.
  No health, death or incapacity from hunger is implemented.
- Four underground plots; two shared Cave Potato seeds and two shared Glowcap seeds.
  Potato: 2 elapsed cycles, 3 Food + seed. Glowcap: 3 cycles, 2 Food + Organic + seed.
  A whole harvest must fit; no yield or seed is silently destroyed.
- Six personal slots, stack size ten, 24 communal slots by default (configurable).
  Live resource IDs are `scrap`, `organic`, `food`, `potato_seed`, `glowcap_seed`,
  plus `fuel` introduced by M4a.
- Three finite surface sites hold 12 Scrap, 8 Organic Material and 14 Scrap. They
  retain their original materials; M3b adds six separate seeded Food sources.
  M4a introduces power and Fuel; machine parts are not implemented.

| Players | Personal starting Food, group total | Shared starter seeds | Plots |
|---:|---:|---:|---:|
| 2 | 4 | 4 | 4 |
| 4 | 8 | 4 | 4 |
| 6 | 12 | 4 | 4 |

The [M3a timed ledger](docs/M3A_FOOD_BALANCE.md) reports zero missed meals for 2/4
players in both tested schedules; six players miss eight individual meals with
regular care, ten with delayed care. This is an accepted limitation pending future
scavenging, outdoor production and infrastructure; it is not a hidden rebalance.
Simulation evidence is not a human multiplayer playtest.

### Provisional resource direction — not a migration or final catalog

Metal, Biomass, basic machine parts, advanced machine parts, Food, Fuel and Seeds
are the owner's category direction. Keep Scrap and Organic Material working until
item naming/mapping, recipes and any migration are reviewed. Do not presume that
Organic is already a generator fuel or implement every category immediately.

### Older economy examples — archived, not active defaults

Earlier Roadmap examples included four-player stocks of 10 Food, 6 seeds, 6 Fuel,
8 Scrap, 1 component and 2 biomass; generator capacity 4 at 1 Fuel/day; beacon 2,
grow lights/workshop 1 each and recovery chamber 2; a three-missed-day beacon reset;
Bitterleaf and several craftables. These were provisional simulation ideas, not
approved systems or current stocks. M2a/M2b/M3a supersede their inventory, crop,
starting-stock and hunger assumptions. No third crop, compulsory powered growing,
health consequence or beacon reset follows from these archived examples.

**Balance gate:** Before changing any stock, yield, cost or severity, simulate timed
baseline, cautious, aggressive and unlucky strategies for 2/4/6 players. Include
travel, harvest/replant timing, storage distribution, consumer shutdowns and recovery
from a bad day as systems become available. Present changes and get approval.

## 7. Review and sequencing changes

The older broad M2 economy and M3 farming/power/beacon headings are now split into
completed M2a, M2b, M3a and proposed M3b–M3g slices. M4–M8 retain their broad purposes;
an early cooperation gate precedes their final multiplayer completion, and M5b
makes exposed production an explicit test. Earlier implementation history is kept
above and in the decision/playtest logs. No working system is replaced by this plan.

At each milestone report what changed, how to run/test it, actual validation,
unverified configurations, inspected captures when available, consequential open
choices, the proposed next small slice, commit reference and worktree status.
Then stop for owner approval.

Ask group playtesters to rate atmosphere, clarity of needs and beacon, freedom to
choose work, exploration risk/reward, fairness of shortages and rescue, optional
conflict behavior when tested, and desire for another run. Separate observations
from fixes; prioritize the first survival loop over additional content.
