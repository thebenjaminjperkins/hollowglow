# HOLLOWLIGHT
## Codex Bootstrap | Repository initialization and first slice | v0.2

**Read this document in full before making changes.** Also read `Hollowlight_Vision.md` and `Hollowlight_Roadmap.md` in full. The Vision is the creative authority; the Roadmap controls milestone sequencing; this Bootstrap governs setup and the **first approved slice only**.

## 1. Your assignment

Initialize a new, maintainable **Godot 4 .NET / C#** project for Hollowlight, using **Git** and **VS Code**, then implement the smallest playable slice of Roadmap M1 ("The Threshold") after checking prerequisites and asking about any blocking creative choices.

Do **not** attempt to build the full survival economy, procedural world, enemies, farming, rescue, or multiplayer in this first pass. Do not assume an empty repository contains working project files.

**Working title:** Hollowlight. **Target experience:** top-down 2D pixel-art cooperative survival for 2–6 players, eventually supporting local and online play. The first slice may be single-player, but architecture must not hard-code a single global player or conflate player input with world simulation.

## 2. First inspect and plan

1. Inspect the directory, Git state, available Godot executable/version, .NET SDK, and VS Code workspace files. Never overwrite existing user work.
2. Read the Vision and Roadmap; summarize the first slice in a few sentences.
3. Check compatibility of the installed Godot .NET version and SDK. If unavailable, report the exact missing prerequisite and provide setup instructions; do not pretend a build succeeded.
4. Ask the owner only about **blocking creative choices**, such as top-down perspective interpretation, movement feel, and provisional visual palette. Offer concrete options and a recommendation. If no reply is needed for a reversible placeholder, state the assumption and proceed.
5. Propose the smallest set of changes, then implement the approved first slice.

## 3. Project and repository conventions

- Engine: **Godot 4 .NET**, with gameplay in **C#**. Prefer built-in engine systems and simple maintainable architecture.
- Editor: **VS Code**. Include appropriate workspace recommendations and documented launch/debug instructions where practical; avoid machine-specific absolute paths.
- Git: initialize if needed, use a Godot/.NET appropriate `.gitignore`, and commit logical working milestones if authorized by the user's normal workflow. Never rewrite history or force-push.
- Documentation: `README.md` (requirements, launch, controls, test), `docs/DECISIONS.md` (approved decisions and provisional assumptions), `docs/PLAYTEST_LOG.md` (observations and unresolved questions).
- Organize scenes, scripts, data, tests, and assets so content can grow without rewriting the project. Favor small, cohesive classes and data-driven parameters.
- Use a consistent naming convention and avoid speculative frameworks, plugin dependencies, or architecture designed for features that do not yet exist.
- Ensure resources/assets have clear provenance and compatible licenses. Do not download or copy copyrighted game assets without permission.
- Do not introduce telemetry, accounts, external services, or paid dependencies without asking.

## 4. First slice: The Threshold

Create one small, atmospheric, explorable test area with:

1. **Underground refuge:** rough cave walls, simple vines or luminous plants, a few recognizable salvaged machines/garden beds, and a warm palette. These can be simple original pixel-art placeholders, but should communicate a place, not a collection of unlabeled rectangles.
2. **Surface clearing:** a visually contrasting ruined or overgrown area with muted colors, some environmental detail, and a few salvageable objects.
3. **Transition:** an obvious passage between underground and surface, with correct collision and camera behavior.
4. **Player:** responsive top-down movement, collision, and basic interaction with objects.
5. **Salvage loop:** collect one simple resource aboveground, carry it back, and deposit it in shared storage (single-player access is sufficient for this slice).
6. **Readability:** visible interaction prompts, basic resource feedback, and a reliable reset/debug path.
7. **Minimal tests:** deterministic tests for inventory/storage or equivalent non-rendering logic, plus a reproducible manual smoke-test checklist.

Keep the map compact. No day/night system is necessary yet. No final-quality art is necessary, but the refuge and surface must already communicate the intended mood.

## 5. Acceptance criteria

- Project opens and runs with the documented Godot .NET version.
- C# solution builds successfully in the available environment, or exact blockers are reported honestly.
- Player can walk around both spaces, cannot walk through solid walls, and can transition between them.
- Player can interact with salvage, carry it, and deposit it in storage; quantities are correctly tracked.
- Camera, prompts, and visual contrast are readable at the intended play scale.
- Restarting/reloading does not leave the game in a broken state.
- Source files, project metadata, and intended assets are committed or staged appropriately; generated build caches are ignored.
- README contains clear launch steps, controls, and a concise manual test procedure.
- No unrelated roadmap milestone has been implemented without approval.

## 6. Architecture boundaries for future work

Plan for these constraints without implementing them now:

- **2–6 players** with future local and online play; input and camera ownership must not be globally assumed.
- World simulation should be separable from presentation, with a clear owner of mutable game state for eventual authoritative networking.
- Resources, recipes, crops, machines, and balance values should eventually be data-driven.
- Seeded procedural generation is required later; design scene content to be reusable, but do not build a full generator in M1.
- Future systems include farming, food consumption, power allocation, radio signal progress, riskier nights, combat, downed players, and botanical resurrection.
- Avoid premature optimization. Profile performance before introducing complexity.

## 7. Design-decision protocol

You **must ask** before deciding matters that substantially affect the game's identity, including:

- Art direction, visual scale, animation style, or major camera behavior.
- Inventory ownership/sharing, survival severity, resource economy, and crafting scope.
- Combat lethality, friendly fire, classes, death and resurrection rules.
- Beacon reset behavior, rescue conditions, run duration, and difficulty progression.
- World-generation style, procedural refuge layout, vehicles, or major progression features.
- Any major change to the Vision or Roadmap.

For each question, provide **2–3 options**, concise tradeoffs, and a recommendation. Do not block on minor reversible engineering details. Record confirmed decisions in `docs/DECISIONS.md` with dates and distinguish them from assumptions.

## 8. Verification and reporting

Before reporting completion:

1. Run the available C# build and automated tests.
2. Run a headless/editor validation if supported by the environment.
3. Launch and visually inspect the game **if graphical tools are available**; otherwise explicitly request a user playtest and do not claim visual verification.
4. Review Git diff and status, confirm no unintended files or caches are tracked.
5. Update README and the decision/playtest logs.

Return a concise report containing: implemented features, how to run/test, build/test outcomes, any unavailable validation, open design questions, and the next recommended slice. **Stop and wait for approval before M2.**

## 9. First prompt to Codex

> Read `Hollowlight_Vision.md`, `Hollowlight_Roadmap.md`, and `Hollowlight_BOOTSTRAP.md` completely. Inspect the empty or existing directory and development environment. Initialize the Godot 4 .NET C# Git/VS Code project and implement only the approved M1 "The Threshold" slice. Ask me about blocking creative decisions, document assumptions, validate what you can, and stop after reporting results. Do not proceed into farming, combat, procedural generation, or multiplayer without my approval.
