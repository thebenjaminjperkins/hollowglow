# Hollowlight — Vision Update

Draft for owner review · 2026-11-17 · Document A

> Repository integration note — 2026-11-17: Imported from the owner's revised
> `HOLLOWLIGHT_VISION_UPDATE_REVISED.md`. This review draft is the intended-design
> authority for subsequent planning, as expressly directed by the owner. Existing
> approved Vision/Roadmap files are preserved; implementation still needs milestone
> approval. See the [consistency review and VS0 scope](PLANNING_REVIEW_VS0.md).

This draft consolidates the owner's latest “Vision Consolidation and Development
Replanning” direction and compatible prior decisions. It does **not** replace the
[approved Vision](../Hollowlight_Vision.md) or [Roadmap](../Hollowlight_Roadmap.md).
The latest owner direction governs this draft where older documents conflict.
Approval of design principles is distinct from approval to implement a milestone.
The companion [Development Alignment Brief](DEVELOPMENT_ALIGNMENT_BRIEF_DRAFT.md)
audits the prototype and proposes the route forward.

**Status language:** “Approved principle” records owner direction, not a delivered
feature. “Preferred direction” needs playtesting. “Open” needs a consequential
decision; this draft supplies no hidden default. Existing prototype values remain
unchanged, even where the intended design differs.

## 1. Game identity and player experience

**Approved principle.** Hollowlight is a 2–6-player cooperative survival-adventure
game in detailed top-down 2D pixel art. Players make a home beneath a mountain in
a beautiful, mysterious world being reclaimed by strange botanical life: the Bloom.
They explore, cultivate food, develop infrastructure, manage electricity, fight or
avoid organisms, recover teammates and prepare an eventual evacuation.

The emotional contrast is central: **the surface is beautiful and dangerous; the
sanctuary is warm and secure. Returning home should feel relieving.** Curiosity,
wonder, melancholy, teamwork and occasional chaotic humor can coexist. The world
must communicate more than rust and loss. Neither relentless combat nor constant
base maintenance should crowd out its quieter pleasures.

Players specialize flexibly through equipment, preparation, knowledge and the work
at hand. There are no mandatory caretaker roles, fixed classes or character-level
progression commitments. Groups should improvise, divide work and change roles.
The original direct eight-direction movement and following orthogonal camera are
useful foundations; deliberate weapons must remain compatible with responsive movement.

### Experience acceptance principles

**Approved intent.** Hollowlight is not a walking simulator, an idle farming game,
or a continuous combat arena. Ordinary play should offer worthwhile competing
activities, but should leave room for quiet travel, planning, and conversation.
Meaningful actions improve survival, open future options, reveal something worth
knowing, or introduce a comprehensible risk. Do not pad expeditions with distance,
search timers, or arbitrary chores to manufacture activity.

The game's memorable moments should emerge from shared decisions: returning
through closing iron doors, rescuing an explorer, losing valuable gear, spending
scarce materials on the wrong project, or seeing a teammate emerge from the
vegetable resurrection chamber with a temporary botanical oddity. Humor should
be contextual and systemic rather than constant scripted jokes. A protected
sanctuary is not a guaranteed victory: food, beacon power, equipment and eventual
extraction still demand planning and dangerous exploration.

## 2. Three connected gameplay pillars

| Pillar | Player decisions | Contribution to the run |
|---|---|---|
| Sanctuary development | What to grow, build, power, store and prepare; how to recover | Produces capability, reserves and a relieving home |
| Exploration | Where to go, what to investigate, carry, preserve or dismantle | Provides materials, equipment, knowledge and strategic opportunities |
| Combat and survival | When to engage, retreat, clear territory or rescue a teammate | Makes routes, preparation and cooperation consequential |

**Approved principle.** These pillars should have relatively even importance over
a run; this is not a strict division of every minute. Early play favors discovery
and establishment, midgame combines development with increasingly dangerous
exploration and combat, and late play emphasizes survival, preparation and extraction.
Investments should connect pillars: a cleared route helps hauling, a better food
reserve permits an expedition, and equipment recovered outside enables a rescue.

**Preparation enables freedom.** Stocked supplies, sound power allocation and
appropriate infrastructure let home operate unattended temporarily. Normal growth
and operation should not require repeated clicks. Quiet contribution remains useful
without assigning someone to stay behind permanently. Prepared groups should be
able to spend meaningful periods at home or away; exact autonomy durations need testing.

## 3. Emotional and artistic identity

**Approved principle.** Beautiful botanical reclamation defines the surface:
varied natural greens, flowers, moss, ferns and vines; surviving paint, weathered
wood and recognizable infrastructure; unusual botanical forms with restrained teal
and violet accents. Ordinary vegetation should outnumber supernatural accents.
Colonization varies between objects and places. Daylight invites curiosity; night
changes the atmosphere and increases danger. Regions need recognizable identities.

The sanctuary is warm, compact and lived in, with natural cave boundaries, functional
work areas, gardens, salvaged equipment and signs of shared life. Avoid uniformly
brown scenery, equal rust on everything, random surface noise and cheerful caricature.
Subtle environmental movement, reactive vegetation, ambient sound and occasional
bioluminescent activity support life without obscuring important states.

Visual coherence is a **playable-scene acceptance criterion**, not an asset-sheet
criterion. Test walking behind walls and furniture, cave/garage corners, contact
shadows, occlusion, fog edges, doorways and HUD cropping in motion at actual zoom.
Fog of war, illumination, light-blocking geometry and contact shadows are separate
systems. The existing square black masking and incoherent wall/object presentation
must not be declared acceptable merely because automated checks pass.

The current complete environments do not meet this standard. A substantial visual
foundation overhaul is expected. Existing shaders, fixed palette restrictions,
terrain assembly and rendering architecture may be refactored or replaced. Preserve
useful detailed artwork and functioning gameplay where practical; previous technical
validation does not settle artistic acceptance.

The durable requirements are modular reusable assets, coherent pixel density,
material shading, readable silhouettes and states, consistent sprite grounding and
sorting, adjustable illumination, believable architecture and contextual composition.
The current 64-color/two-native-pixels-per-world-unit pipeline is a useful baseline,
**not an irrevocable design constraint**. Major changes to density, palette or camera
still need comparison and approval. A static painted whole-world background cannot
replace reusable terrain and location modules.

Rendering must distinguish physical collision, visual occlusion, illumination,
current sight and discovered geography. Remembered geography must not expose unseen
dynamic changes. Evaluate moving scenes at gameplay scale, in daylight, interiors
and night, rather than judging isolated enlarged sprites alone.

## 4. Sanctuary: security, circulation and development

**Approved principle.** Home is a compact, roughly square main chamber with irregular
natural boundaries and one shared central circulation area. Agriculture, workshop,
power generation/storage, communal supplies, radio, damaged resurrection chamber
and living space sit around it. Avoid maze-like routes, exact artificial symmetry
and excessive unused floor. The current approximate 24×24-tile review footprint is
provisional; convenience and credible composition matter more than exact dimensions.
Preserve short daily routes and clearance for 2–6 players and eventual wheelbarrows.

Two substantial iron bunker doors meet at the center of the entrance. Closed doors
and intact walls **guarantee protection against ordinary Bloom organisms**. Enemies
may enter through open access or player mistakes. Doors remain mechanically operable
without power. Mandatory nightly sieges, arbitrary breaches and routine door failure
are outside this security contract. This supersedes older uncertainty about ordinary
enemies breaking into an otherwise secure sanctuary. Exceptional threats are not
implicitly authorized; any exception requires an explicit design decision.

Construction takes place in designated zones with flexible placement inside them:
blueprint placement → local construction stockpile → physical material delivery →
short assembly with visible progress → commissioning and operation. Modest assembly
times scale with project size; logistics and investment matter more than waiting.
Material ownership, cancellation and recovery must be legible and safe. Construction
stockpiles do not authorize automatic withdrawal from every communal container.

Reserve future garden and utility space without granting infrastructure for free.
The four starting plots apply to every party size. The previous five-chamber plan
is a prototype, not a permanent geography requirement. The hall review profile is
also a testable foundation, not automatic approval of final geometry or artwork.

## 5. Agriculture, electricity and autonomy

**Approved principle.** Food and Biomass production are major investments. Protected
indoor agriculture has limited space; exposed outdoor production offers potential
productivity advantages with danger and possible protection costs. Players prepare
land, install beds, plant, harvest and improve production through infrastructure.
Outdoor crops are not automatically destroyed every night. Protection may involve
location choice, clearing nearby colonies, barriers or active defense, depending on
circumstances. Farming setup should involve site preparation and appropriate
lighting for enhanced indoor production, while routine care remains light enough
for the entire party to leave after preparing.

Retain the useful existing Cave Potato/Glowcap growth, seed renewal, readable stages
and inventory-safe harvesting foundation. Basic crops do not acquire an electrical
lighting requirement. Crop values and any future Marrowvine, grow-light or botanical
catalyst effects need their own approved configurations. The study's eight-plot
maximum and candidate recipes are not silently installed in the game.

A developed sanctuary can eventually sustain approximately its basic Food and
renewable beacon-power needs without further finite surface inputs. This is a
late-game reward requiring exploration and investment. Ownership of a generator
or theoretical average yields alone do not establish self-sufficiency: a repeatable
operating schedule needs seed renewal, labor, storage and reserves between harvests.
It does not provide unlimited electricity for every optional machine.

Valid machinery placements connect automatically to one shared electrical network.
Players manually allocate limited capacity among enabled consumers. Fuel generation
provides stronger initial output; Biomass generation starts lower and can improve.
Batteries, reserves and eventually limited automation extend capability and autonomy.
Avoid cable-drawing chores unless later testing establishes a specific need.

Preserve understandable demand/capacity feedback and non-destructive activation
rejection as the working foundation. Exact multi-generator behavior, battery rules,
upgrade effects and operating costs remain implementation decisions. The isolated
study's proportional sharing/unsupplied-bus model is evidence, not an implicit live
power-system migration.

Food, electricity, materials, daylight and labor remain constraints. Personal meals,
recoverable hunger penalties and gradual recovery are useful existing foundations.
Adding combat does not silently make hunger lethal or revise crop/food/fuel balance.

## 6. Resources, equipment and progression

**Approved category direction:** Metal, Basic Parts, Advanced Parts, Biomass, Fuel,
Food and Seeds. Crop-specific seed identities and selected specialized items may
sit within or outside these general categories. Unique components can support
specialized discoveries without creating a catalog of interchangeable small parts.

Major investments compete for shared fundamental materials. Basic survival recipes
are known from the beginning; exploration unlocks specialized knowledge shared by
the group. Most projects require knowledge and materials; a few advanced projects
also require specialized facilities. Avoid elaborate tiers, XP gates and research timers.

Equipment is transferable and has no routine durability. The six-slot hotbar
should be integrated with the backpack rather than treated as six unrelated
storage inventories; its exact controls and equipped-item semantics require a
hands-on UX test. A functional, dependable wheelbarrow is a cargo tool, not a
randomly temperamental comedy object. Do not introduce improvised cooking as an
assumed system. Botanical crop mutations and occasional resurrection mutations
remain thematic opportunities, with effects and frequency unapproved. Candidate investments
include beds, grow lights, botanical catalysts, Biomass generation, batteries,
weapons, armor, backpacks, salvage tools, wheelbarrows, vehicle restoration and
resurrection-chamber repair. Their inclusion here does not authorize a large catalog.
Essential technologies/capabilities have guaranteed minimum acquisition opportunities
and alternative paths where practical; optional projects and bosses may vary.

Keep explicit personal carrying limits and communal storage. The existing six base
slots, ten-unit stacks, transfer safety, personal crafting ingredients and first-six
future-hotbar convention are working prototype foundations. Additional installed
slots are storage-only. Future transferable equipment, dropped backpacks and local
construction stockpiles require explicit ownership rules rather than automatic pooling.
Existing `scrap`/`organic` IDs remain until a reviewed Metal/Biomass migration.
Previous candidate project costs and world profiles are not final balance.

## 7. Exploration and connected regions

**Approved principle.** A substantial local region surrounds the mountain sanctuary;
additional regions connect naturally through roads, trails, tunnels and passes.
Transitions should feel geographically continuous even if loading or streaming is
necessary. Different players can occupy different regions simultaneously. The
sanctuary remains the recognizable anchor while geography and regional arrangements
vary meaningfully across runs.

Significant locations support investigation, finite location-based salvage,
equipment recovery, knowledge, specialized objectives and return visits. Persistent
depletion gives choices lasting consequences. A garage belongs on a suitable road;
a greenhouse belongs in a coherent agricultural setting. Machinery should have
recognizable purpose, tool requirements and dismantled states. Carrying constraints
and irreversible salvage decisions matter without inflated interaction timers.

A successful group typically explores a meaningful portion of the generated world,
with some optional places undiscovered or unvisited. Avoid interchangeable nodes
and empty walking. Shared fog-of-war discoveries support coordination; mapped
geography and current sight remain independent. The treatment of last-known object
information and teammate sight sharing must be decided explicitly.

## 8. Rule-driven world generation

**Approved principle.** Use constraint-based rules with controlled seeded randomness.
Build coherent geography and infrastructure before detailed locations/resources.
Use authored modular structures for important places rather than arbitrary building
geometry. Rules express required/optional locations, count ranges, adjacency,
separation, terrain suitability, infrastructure dependencies, access/transport needs,
distance and travel-time budgets and essential progression opportunities.

Validate connectivity, interaction access, resource/progression feasibility and
extraction viability, including the recovery opportunity after a missed evacuation.
Reproducible seeds help diagnose failures. A connected map is insufficient if a
required tool lies behind its own tool gate or a necessary recovery objective is
unreachable. Major landmarks remain stable during a run while local ecology changes.
The number, scale and streaming boundaries of regions remain open.

## 9. Bloom ecology and combat

**Approved principle.** The Bloom is an evolving ecosystem, not random enemy spawning.
Danger relates to elapsed time, distance, darkness, local ecology and player
disturbance. Combat, noise, harvesting and dismantling can create temporary local
alerts. Significant actions, colony destruction and boss defeats may have lasting
effects. Colonies, migration and ecological growth recover populations; clearing
colonies provides lasting local safety benefits without eliminating all isolated
threats or migration. Do not erase major landmarks to simulate change.

Combat must be engaging on its own merits, with responsive input, clear hit
feedback, distinct enemy behaviors, understandable risk, and credible death.
It should not exist merely as a delay or material tax. Encounters become more
common and dangerous with time and distance, while individual locations and
territories provide their own ecological danger. Quiet and avoidance remain
valuable, but weapons and armor must materially reward investment.

Combat is unavoidable across the overall survival/extraction experience, while
individual encounters and bosses can often be avoided. Preparation, positioning,
equipment, teamwork and player knowledge should matter. Distinct behaviors take
priority over enemies differentiated only by health totals. Retreat and readable
counterplay belong in the first combat test.

**Preferred direction.** Responsive movement with deliberate weapons; exact aiming,
attack cadence, damage, ammunition, enemy roster and difficulty require playtesting.
Optional bosses inhabit believable places, reward investment and can influence
territory. The Glasshouse Heart and its unique agricultural growth catalyst are an
example, not a finalized encounter, mandatory progression gate or approved stat block.

## 10. Death, equipment recovery and resurrection

**Approved principle.** Death is a credible exploration outcome. A dead player leaves
a persistent recoverable backpack containing carried equipment at the death
location; teammates can recover its contents. Resurrection never restores those
items automatically. “Persistent” requires stable session state; disk-save policy
is still open and must not be implied by current in-memory depletion.

Home starts with a damaged chamber requiring one substantial repair investment.
Before repair, emergency recoveries are limited. After repair, resurrection uses
renewable Biomass. Exact counts, costs, power demands, location/activation rules and
spectating behavior remain open. Older DNA/food recipe suggestions are not requirements.

**A full-party wipe immediately ends the run regardless of recovery resources.**
Remaining charges or a repaired chamber cannot resurrect a group after everyone
has died. **Preferred direction:** a downed state with teammate revival; the boundary
between downed, dead and full-party wipe requires explicit testing and approval.
Do not silently equate all-downed with all-dead or permit indefinite recovery loops.

## 11. Multiplayer and shared authority

**Approved principle.** Player-hosted, server-authoritative sessions target 2–6 players.
One player hosts the simulation; friends join. The host validates consequential
inventory, crafting, construction, combat, death, resurrection and consumption
actions. Shared state includes time, exploration, world resources, construction,
agriculture, power, enemies/ecology, recovery and extraction. Authority must remain
compatible with later dedicated hosting without building a service platform now.

The current single-player scene and multi-actor model tests are foundations, not
networking. Two connected players are required for the first cooperative vertical
slice. Dedicated servers, public matchmaking, mixed local/online play, migration,
reconnect and long-session saves need separate scope decisions. Player-on-player violence remains an intended future option, including the
possibility of consequential mistakes or betrayal; its permissions, damage and
anti-griefing design are open. Do not silently enable friendly fire in the first
slice.

## 12. Beacon, extraction and outcomes

**Approved principle.** Players know the final extraction objective from the start.
Accumulated successful beacon transmission establishes an extraction opportunity;
it does not immediately win or freeze the game. The generated world determines
the eventual extraction location. Players leave home, cross dangerous territory and
survive evacuation. Equipment, supplies, transportation, route knowledge and cleared
territory improve their chances. No single optional vehicle, boss reward or upgrade
is mandatory.

Recoverable outages and clear progress/remaining-time feedback remain useful.
The existing startup grace, bounded loss and anti-flapping model are a baseline for
testing, not a new full reset. The playable seven-day terminal prototype and the
study-only fourteen-day Standard candidate are distinct. Final difficulty lengths,
short-slice timing and extraction windows remain open; neither is changed here.

| Outcome | Intended consequence |
|---|---|
| Everyone boards and escapes | Full rescue |
| At least one boards, others do not | Partial rescue; others are left behind |
| Nobody boards and survivors remain | Missed extraction; costly second chance may be pursued |
| Everyone dies | Immediate run failure regardless of resurrection resources |

**The run ends when the first rescue transport departs with at least one player
aboard.** A transport leaving empty does not count as successful termination. A
second chance requires reestablished communication **and** an additional
world-dependent recovery objective. Generation must make that objective feasible;
it must be difficult rather than a trivial repeat. Exact windows, costs, consequences
of further misses and other terminal rules remain provisional.

## 13. Scope discipline and open decisions

Preserve valuable working systems, source artwork/provenance, compact regression
fixtures and reproducible studies. Replace flawed behavior or presentation where
necessary to meet the experience. Use small milestones with actual human playtests
and automated evidence; numerical feasibility is not multiplayer balance. The next
deliverable is a complete 15–20-minute cooperative experience, not the entire game.
This document does not create the final Gameplay & Economy Brief.

The **15–20-minute vertical slice is a development validation scenario**, not
the intended duration of the complete game. The longer survival run and candidate
fourteen-day economy remain separate design work. The short slice must demonstrate
at least one meaningful sanctuary investment, expedition, threat, return and
extraction, not claim to validate full-run balance.

The key unresolved decisions are short-slice clock/mission pacing versus Standard
duration; rendering/palette/camera comparison; combat controls and lethality;
downed/wipe and emergency recovery details; door obstruction rules; construction
cancellation/material ownership; resource/equipment migration; multi-generator and
battery behavior; region scope and persistence; extraction windows/second chances.
Each must be resolved before its dependent implementation, without freezing earlier
independent progress. See the companion brief for recommendations and review gates.
