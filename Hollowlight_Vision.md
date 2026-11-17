# HOLLOWLIGHT
## Game Vision | Living document | v0.3 — approved design revision

> **A cozy, melancholic 2D pixel-art cooperative survival roguelite for 2–6 players.** Build a life beneath a mountain, venture into a finite and hostile surface world, and maintain a rescue signal long enough to be found. Optional player conflict can complicate a shared struggle without becoming its dominant strategy.

This document describes the intended experience, atmosphere and design principles,
not an implementation specification or permission to build future systems. The
Roadmap records completed work and proposes incremental tests; Bootstrap governs
initialization and the first slice. This revision records the owner's approved direction. Individual future slices and
unresolved rules still require review; approval of the Vision is not gameplay authorization.

## 1. The experience we are making

Hollowlight alternates between two feelings:

- **Below:** warmth, relative safety, shared work and a home that becomes more capable
  and personal. Irregular cavern walls, hanging vegetation, strange luminous growth,
  textured salvaged machines, gardens, soft lamps and improvised technology coexist.
- **Above:** quiet beauty and unease among abandoned structures, finite resources and
  hostile environments. Daylight permits exploration, scavenging and confrontation
  or evasion. At night, exposure becomes especially dangerous.

The world is melancholic and mysterious, not relentlessly frantic. Early days are
forgiving. Players have time to learn, tend plants and enjoy each other's company.
Later decisions involve risk, food, electricity, supplies, time and loyalty. Humor
can emerge from friends and slightly absurd technology without dissolving the sorrow.

**Core promise:** A quiet day improving the sanctuary and a risky expedition that
makes tomorrow easier are both useful ways to contribute. Circumstances encourage
players to divide work naturally, without mandatory roles or initially fixed classes.

## 2. Acquire, sustain and protect

Three activities depend on one another:

- **Acquire:** Explore, scavenge and transport resources; confront or evade creatures.
  Finite nearby supplies eventually push expeditions farther from safety.
- **Sustain:** Farm, craft, generate power, repair equipment and allocate resources.
  Supplies become meals, functioning communications and better expedition capability.
- **Protect:** Control sanctuary entrances, secure exposed infrastructure, respond to
  shortages and emergencies, and rescue teammates. Protection enables future work.

The daily rhythm is to assess needs and daylight, choose work, explore or improve
home, return and allocate, then prepare for night. Prepared groups should be able
to spend roughly one or two days underground without an expedition. Quiet work
must remain worthwhile; growth and normal machine operation should not demand
constant clicking or repetitive upkeep.

**Night is primarily a logistical and exploration deadline, not a compulsory
siege round.** Risk comes from location, exposure, depletion and decisions as well
as elapsed time. More players add both capability and personal consumption; shared
infrastructure costs should not simply multiply with every player.

## 3. The mountain sanctuary and its entrances

The sanctuary is a relatively secure shared home, not a menu between expeditions.
It eventually supports underground plots, personal inventories and communal storage,
food preparation, generation and distribution of power, a radio beacon, a workshop,
repairs, recovery facilities and modest personalization.

Substantial **manually controllable doors** protect entrances. Players can choose
to open or close them at any time. Closed doors provide meaningful safety; open
entrances create opportunities for creatures to enter. Neither compulsory nightly
attacks nor automatically scheduled door openings define the experience.

Relative safety does not promise invincible doors or solve every environmental
hazard. Door durability, breach behavior, creature pursuit and access rules remain
to be tested. The sanctuary should stay comfortable enough to reward returning.
Infrastructure should solve recognizable problems without making the base a chore.

## 4. Safe and exposed agriculture

Preserve the existing underground farming system as a dependable foundation:
limited growing space, readable growth stages, time to maturity and seed renewal.
The current Cave Potato and Glowcap crops, four plots and inventory-safe harvests
remain valid completed work. This vision update does not replace or rebalance them.

Future outdoor agriculture offers higher productivity **or** faster growth in
exchange for exposure to the hostile environment, especially at night. Players may
invest in protection, lighting or other infrastructure to reduce risk. Outdoor
crops are **not automatically destroyed every night**. Exact threats, protection
benefits, yields and growth rates require playtesting and approval.

This expresses a broader principle: **exposing infrastructure to environmental
danger can increase productivity**. Safe production and exposed production should
both retain value; neither should universally dominate the other.

## 5. A small interconnected resource economy

Food, electricity, materials, daylight and human labor are the central constraints.
Use a small, understandable set of provisional resource categories:

- Metal
- Biomass
- Basic machine parts
- Advanced machine parts
- Food
- Fuel
- Seeds

These are categories to explore, not an approved final item catalog. Existing Scrap
and Organic Material remain functioning resources. How they map to Metal and
Biomass, and whether their names change, requires an explicit migration decision.
Do not silently add all categories, rename IDs or invalidate inventory/crop data.

Resources need competing uses. Materials invested in an expedition cannot also
repair the sanctuary; fuel spent on optional production may be needed to sustain
communications. Avoid an inventory of arbitrary screws and bolts or many nearly
identical intermediate ingredients.

Food production, surface food scavenging and later outdoor agriculture provide
complementary routes to sustaining the group. Hunger should first create legible,
recoverable penalties rather than sudden death. M3a's personal meals and gradual
recovery are the current foundation; later health consequences are not established.
The known six-player underground-food deficit is accepted pending future production
and acquisition options, not permission to silently increase existing yields.

## 6. Power and competing investments

The sanctuary needs electricity for communications and other useful infrastructure.
Explore generation strategies such as fuel-based and biomass generators, with
different costs and benefits. Multiple generator types are candidates for later
experiments, not a requirement to build them together in the first power slice.
Power storage and selectively shutting down consumers should help groups handle
shortages. The radio beacon is a critical consumer, but allocating all power to it
should have understandable consequences elsewhere.

Progression asks players to choose among:

- Weapons and armor
- Backpacks and tools
- Agricultural improvements
- Power generation and storage
- Sanctuary repairs and protection
- Transportation
- Resurrection infrastructure

Investments should change what players can accomplish: reach a distant site,
transport a useful load, protect a crop or recover a friend. Numerical efficiency
can support those changes, but statistics alone are not the purpose of progression.
Exact recipes, costs, power priorities and whether any crop equipment needs power
remain decisions for their own slices; existing crops gain no new upkeep here.

## 7. The surface, exploration and combat

Each run should generate a different, navigable world with coherent routes,
recognizable landmarks, abandoned structures, finite salvage and risk gradients.
Procedural generation should create fresh decisions rather than random clutter,
and preserve useful authored-feeling places and safe traversal. Reproducible seeds
support testing. Whether sanctuary layouts themselves are generated remains open;
current authored layouts and modular art are reusable development fixtures.

Nearby supplies are accessible and relatively forgiving. As they are exhausted,
travel becomes longer and more dangerous. Distant sites and hostile creatures
provide access to valuable resources. Some places remain dangerous at noon; night
makes exposure especially risky. Warnings should let players judge escalating danger.
Quiet discoveries and traces of ordinary lives matter alongside loot and enemies.

Combat should be situationally necessary over a run, while an individual encounter
may be avoided through preparation, movement, stealth or clever decisions. Avoidance
must have useful counterplay; neither mandatory fighting at every encounter nor
universal consequence-free evasion is the target. Enemy designs and any stealth
mechanics are subject to testing, not a commitment to a large separate system.

Weapons, armor, improved tools, backpacks and eventually transportation make more
dangerous expeditions feasible. Equipment improves options without removing the
need to plan a return trip or protect what the group brings home.

## 8. Players, conflict and recovery

The target is 2–6 players, eventually locally and online, working toward a shared
rescue. No mandatory classes are established. Specialties may emerge through gear,
experience and the needs of the moment.

Optional player conflict is part of the intended direction: players may eventually
damage or kill each other. Scarcity can invite selfish allocation, withheld supplies
or betrayal, but player killing must not become the dominant resource strategy.
Which modes permit it, defaults, friendly-fire rules, consent/access controls and
lethality remain consequential decisions. This does not authorize PvP implementation.

Teammate rescue should matter. A downed-state test precedes final death rules.
Recovered DNA plus scarce food, biomass and power may eventually support strange
bioreconstitution. A revived friend with cosmetic botanical traits is a welcome
tonal accent, provided the resource tradeoff is meaningful. Costs, timing, failure
conditions and permanent death remain open; no resurrection system is implied by
current hunger penalties.

## 9. Victory, loss and the rescue beacon

The group must survive a difficulty-dependent number of days while maintaining a
functioning radio beacon. Power and signal status should make rescue progress and
outage consequences understandable.

Short signal interruptions should be recoverable. Prolonged loss may reset progress
or impose a substantial delay; exact thresholds, qualifying signal time and setback
rules remain open to playtesting. The final extraction sequence is also unresolved.
Avoid sudden opaque game-overs. Rescue should culminate the group's preparation
rather than become a timer that can be ignored.

Seven days remains an initial complete-run playtest target, not the final length
for every difficulty. Full-run duration, defeat conditions and save/resume policy
require later approval. The immediate priority is connecting the first complete
survival loop before adding broad progression or content libraries.

## 10. The Bloom and the Long Dark (working fiction)

Strange hostile growth reclaims the surface. By day it can look dormant and beautiful;
at night vines spread, flowers open and some organisms hunt living things. Darkness
and the Bloom are related, but the mechanism remains deliberately mysterious.
The underground may be protected by geology, biology, technology or a combination.
It should feel mystical without excluding useful machinery.

Do not lock the game into zombies, a specific catastrophe or a large monster roster.
Threats should be readable, support counterplay and reinforce the atmosphere.
Environmental danger can matter as much as direct combat.

## 11. Art, sound and interface

The approved sanctuary establishes the provisional production standard: coherent
pixel density and palette, organic silhouettes, material texture and wear, modular
PNG sprites/terrain atlases, consistent directional animation and independently
adjustable environmental light. New assets should meet the existing quality target
at actual gameplay zoom, not only in enlarged reference art. Preserve reusable
terrain and props suitable for future procedural assembly, rather than a painted
whole-room background.

The owner's 2026-11-16 overgrown-garage reference updates the provisional visual
target: a beautiful world reclaimed by nature, with slightly less uniform rust
and more surviving paint, weathered wood and recognizable machinery. Moss, vines,
ferns, small flowers and occasional mushrooms vary from light colonization to
heavy overgrowth. Restrained Bloom accents use unusual plant shapes, muted violet
foliage and subtle teal bioluminescence. Keep detailed material shading and readable
silhouettes; avoid covering every object equally or making the world relentlessly
bleak or cartoonishly cheerful. Daylight Bloom should be beautiful; its danger
should emerge primarily through future nighttime behavior and environmental change.
This is an art target, not authorization to implement those mechanics. Preserve
M6b's working assets until a [small visual comparison](docs/ART_DIRECTION_UPDATE.md)
is reviewed; no gameplay, resource, collision or interaction changes follow from it.

Warm cave gardens and humming electronics contrast with muted ruins, luminous
growth, fog and nocturnal movement. Ambient sound, restrained music and environmental
storytelling should support that contrast. Keep crop stages, machines, hazards,
interactions and approaching night readable without excessive noise. Interfaces
should expose decisions through compact status and contextual prompts, not meters
and complicated menus. Humor stays occasional; the world retains its quiet sorrow.

## 12. Established direction, provisional ideas and open decisions

**Established direction:** Cooperative 2–6-player survival with optional conflict;
Acquire/Sustain/Protect; relatively secure mountain sanctuary and manual protective
doors; finite surface resources and outward pressure; safe underground versus
productive exposed agriculture; competing investments; critical powered radio;
recoverable short outages; coherent modular pixel art and review between slices.

**Provisional ideas:** The seven resource categories and final item names; generator
variants and operating costs; outdoor productivity/protection values; specific
craftables and equipment; seven-day test duration; DNA/vegetable reconstruction
recipe; particular enemies and the Bloom's explanation. These are not all approved
for implementation merely because they appear here.

**Unresolved choices:** Conflict modes/defaults and lethality; beacon outage and
extraction rules; difficulty and persistent saves; refuge generation and world scale;
transportation form; death/resurrection timing; resource migration and recipes;
metaprogression and final title. Ask before finalizing consequential choices.

Success means players choose to explore, negotiate resource use, divide work
naturally, enjoy downtime and want another run. Stories such as “we shut down the
workshop to keep the beacon alive” or “we spent our last vegetables bringing a
friend back” express the intended relationships rather than authorize new systems.
