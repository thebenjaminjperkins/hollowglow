# HOLLOWLIGHT
## Development & Playtesting Roadmap | v0.2

**Companion documents:** `Hollowlight_Vision.md` (experience and creative authority) and `Hollowlight_BOOTSTRAP.md` (repository initialization and first slice).

This is an **incremental plan, not authorization to implement every milestone at once**. Keep each milestone playable, demonstrate it, gather feedback, and stop for approval before the next. Revise the plan when playtesting reveals better design choices.

## 1. Guiding rules for Codex

1. Read the Vision and current decision log before proposing work. If documents conflict, ask rather than guess.
2. Implement only the **currently approved milestone or smaller slice**. Do not silently expand scope.
3. Ask the owner about decisions that materially change the player experience. Offer 2–3 options, tradeoffs, and a recommendation. Make ordinary engineering choices independently.
4. Favor complete, testable vertical slices over disconnected systems.
5. Keep a runnable build, automated tests for deterministic logic, and brief manual test instructions. Report what was actually tested and what remains unverified.
6. Keep the game aesthetically coherent. Introduce recognizable pixel-art assets and readable effects progressively, without polishing unproven mechanics excessively.
7. Track decisions and playtest results in `docs/DECISIONS.md` and `docs/PLAYTEST_LOG.md`.
8. Do not claim physical-controller, multi-machine, network, or visual testing unless actually performed.

## 2. Milestones

### M0. Initialize the project
**Goal:** A reproducible, clean foundation.

- Set up the Godot 4 .NET C# project, Git, VS Code configuration, build instructions, and basic test harness.
- Establish scene organization, configuration data, seed handling, and a minimal main menu or debug launch path.
- Add placeholder assets only as needed to validate the setup.

**Exit:** Fresh clone can be opened and built with documented prerequisites; project launches; repository is clean; known limitations documented.

**Ask owner:** Any blocking environment or platform decisions not already documented.

### M1. The Threshold: feel and place
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

### M2a. Shared supplies — approved scope, implemented for review
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

### M2a UX. Visual inventory refinement — approved, implemented for review
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

### M2. Shared resources and basic economy
**Goal:** Resources matter, but do not become chores.

- Inventory, carrying limits, shared storage, finite salvage, simple gathering and repair interactions.
- Food consumption, hunger states, fuel, scrap, and basic UI for needs and stocks.
- Separate per-player consumption from shared infrastructure overhead; support configurable player count in balance simulations.
- Introduce one or two meaningful competing uses for a resource.

**Exit:** A small group can make decisions about gathering versus spending, and shortages produce recoverable consequences.

**Playtest:** Are shortages legible? Is inventory friction acceptable? Can two players keep up? Does six-player labor trivialize costs?

**Ask owner:** Inventory model, sharing/ownership, initial resource taxonomy, and hunger severity.

### M3. Farming, power, and the beacon
**Goal:** A safe day at home is a valid, useful choice.

- Limited growing plots, planting, visible growth stages, harvest timing, and seed renewal.
- Generator fuel, finite power capacity, selectable consumers, and clear warnings.
- Rescue beacon signal-day progress and outage handling, with rules configurable for tests.
- Workbench and a small set of craftable repairs/tools; avoid a sprawling crafting tree.

**Exit:** The group can spend 1–2 days inside with good preparation, and can deliberately choose between powering the beacon and other equipment.

**Playtest:** Do players understand crop timing and power tradeoffs? Does labor allocation emerge naturally? Can a beacon outage be recovered from?

**Ask owner:** Beacon penalty, farm power dependence, production ratios, and initial workstation roster.

### M4. Procedural surface and escalating exploration
**Goal:** Every run offers different routes and reasons to venture farther.

- Seeded generation using connected, traversable regions and recognizable landmarks.
- Near, middle, and distant risk/reward areas; finite salvage placement.
- Travel time, carrying limits, and improved tools that enable new salvage opportunities.
- World-generation validation for reachability, essential-resource availability, and safe starts.

**Exit:** Multiple seeds produce meaningfully different, completable exploration choices, and distant resources are attractive without making nearby areas useless immediately.

**Playtest:** Does procedural variation create decisions rather than confusion? Is backtracking tolerable? Is scarcity fair?

**Ask owner:** World scale, degree of authored landmarks, and whether transportation belongs in this prototype.

### M5. Day/night danger and combat
**Goal:** Daylight creates a deadline; night outside feels dangerous.

- Clear time-of-day cues and warnings, with a forgiving early period.
- Small enemy roster with readable behavior and environmental threats, more intense at night and in riskier regions.
- Basic weapons, damage, retreat, downed-player rescue, and safe refuge rules.
- Difficulty escalation driven by location, resource depletion, and time rather than only enemy health.

**Exit:** Players can judge when to return, and staying outside at night is a meaningful risk rather than an arbitrary instant death.

**Playtest:** Is combat readable? Is retreat possible? Are quiet nights actually quiet? Is the early game forgiving?

**Ask owner:** Enemy aesthetic, friendly fire, direct sanctuary attacks, and combat lethality.

### M6. Complete seven-day rescue run
**Goal:** The whole game loop has a beginning, middle, and ending.

- Tune a seven-signal-day test run with a working beacon, rescue event, and clear victory/defeat screens.
- Include a small number of equipment and infrastructure upgrades with real tradeoffs.
- Introduce resource-cost recovery/resurrection if the downed-state test supports it.
- Save the run or support short playtest sessions as needed; decide persistence scope with the owner.

**Exit:** A 2–6 player group can complete or fail a run without developer intervention; rescue progress and resource pressure are understandable.

**Playtest:** Are there natural job divisions, memorable risks, recovery from setbacks, and a satisfying ending? Is the run too long or too short?

**Ask owner:** Final extraction format, resurrection cost/timing, difficulty presets, and run save/resume requirements.

### M7. Multiplayer completion and reliability
**Goal:** Friends can play together locally and online.

- Build on the networking plan established early; implement and validate shared state, inventory, world seed, farming, power, time, enemies, and rescue progress.
- Support 2–6 players as feasible; local, online, and mixed local/remote play are target experiences, not assumed completed features.
- Test disconnect/reconnect, host loss policy, join timing, and latency behavior.

**Exit:** Real-device and multi-machine tests pass on supported configurations, with known limitations documented.

**Ask owner:** Hosting/lobby UX, reconnect policy, supported platforms, and whether mixed local/remote play is mandatory for first release.

### M8. Visual identity, audio, and balance pass
**Goal:** The prototype communicates the intended game, not just its mechanics.

- Consistent pixel-art tiles, characters, machines, crops, salvage, and enemy silhouettes.
- Atmospheric contrast, restrained music, sound cues, readable lighting and warnings, approachable UI.
- Balance food/power/fuel/loot for 2, 4, and 6 players; run repeated seeded simulations and real group playtests.
- Optimize only after measuring bottlenecks.

**Exit:** Players understand the game visually, can complete a run, and want to play again. No major readability or economy blockers remain.

**Ask owner:** Approve a cohesive art/audio direction before commissioning or generating a large asset set.

## 3. Provisional seven-day economy for simulation

**These are test fixtures, not locked recipes or final tuning.** Keep them data-driven and verify solvability across seeds and player counts.

- Test day length: **10 minutes** (6 day, 1 dusk, 3 night); seven days approximately 70 minutes plus pauses.
- Food need: **1 unit per active player per day**. Missing food causes staged penalties before death.
- Four-player example starting stocks: **10 food, 6 seeds, 6 fuel, 8 scrap, 1 component, 2 biomass**. Scale starting stocks carefully for 2–6 players; do not assume this example is universal.
- Crops under consideration: Cave Potato (2 days, 3 food), Glowcap (3 days, 2 food + biomass), Bitterleaf (2 days, biomass). Harvest returns a seed. Start with **two plots** for balance testing, with expansion possible.
- Power example: generator capacity **4**, fuel use **1/day while active**; beacon demand **2**, grow lights **1**, workshop **1**, recovery chamber **2**. Verify when loads consume power and fuel, especially during partial days.
- Beacon example: earn one signal day after a qualifying powered day/night; short outage pauses; **three consecutive missed days reset** is a candidate to test, not an approved final penalty.
- Candidate craftables: bandage, basic weapon, salvage tool, backpack expansion, additional farm plot, generator repair, fuel-efficiency upgrade.
- Death recovery: teammates can rescue downed players; DNA-and-vegetable bioreconstitution is a later test with a meaningful resource cost.

**Balance gate:** Simulate baseline, cautious, aggressive, and unlucky strategies for 2, 4, and 6 players. Check food sustainability, fuel availability, recovery from a bad day, and whether beacon progress is achievable. If the sample numbers make survival trivial or impossible, propose changes and request approval.

## 4. Milestone review protocol

At the end of each milestone, Codex must provide:

1. A concise summary of what changed and why.
2. How to launch and manually test it.
3. Build/test results, including tests that could not be run.
4. Screenshots or captures **only if actually produced and inspected**.
5. Open design decisions and a recommended next slice.
6. Git commit reference if committed, and a clean/dirty working-tree report.

Then **stop and wait for owner approval**. Do not continue automatically.

## 5. First group playtest rubric

Ask players to rate (1–5) and describe:

- Enjoyment of the refuge and its atmosphere.
- Ease of understanding food, power, crops, and the beacon.
- Freedom to choose tasks without being assigned roles.
- Reward and risk of exploration.
- Tension of returning before night.
- Fairness of setbacks, rescue, and resource scarcity.
- Desire to play another run and what they would do differently.

Record observations separately from proposed fixes. Prioritize fixing the core loop over adding content.
