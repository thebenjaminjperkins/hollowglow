# HOLLOWLIGHT
## Game Vision | Living document | v0.2

> **A cozy, melancholic 2D pixel-art survival roguelite for 2–6 friends.** Maintain a refuge beneath a broken world, venture above for finite resources, and keep a rescue signal alive long enough to be found.

This document describes the intended player experience, fiction, tone, and gameplay principles. It is **not** a technical implementation specification. The companion Roadmap defines what to test and when; Bootstrap defines how to initialize the project.

## 1. The experience we are making

Hollowlight alternates between two feelings:

- **Below:** warmth, safety, shared work, discovery, and a home that becomes more capable and personal. Rough cavern walls, hanging vines, strange luminous growth, reclaimed machines, gardens, soft lamps, and improvised technology coexist.
- **Above:** quiet beauty and unease in a ruined world. Daylight permits exploration, salvaging, and occasional combat. Risk varies by location. At night the surface becomes exceptionally dangerous; being caught far from home is a consequential choice.

The world should be **melancholic and mysterious, not relentlessly frantic**. Early days are forgiving. Players should have time to learn, tend plants, and enjoy each other's company. Later days ask harder questions about risk, food, power, time, and loyalty. Humor can emerge from players and slightly absurd technology without destroying the atmosphere.

**Core promise:** A group can spend a quiet day making the refuge better, then choose a risky expedition because what lies outside could make tomorrow easier. Both activities matter.

## 2. The core loop

1. **Assess:** Review the rescue beacon, food, fuel, crops, tools, injuries, and available daylight.
2. **Choose labor:** Players independently decide whether to farm, cook, repair, craft, scout, salvage, hunt, or accompany others. Specialization is useful, but nobody is forced into a job.
3. **Explore:** Venture into a different procedurally generated world each run. Nearby salvage is limited; distant sites offer greater rewards and greater risk.
4. **Return and allocate:** Store loot, share supplies, craft equipment, repair infrastructure, ration food, and choose what receives power.
5. **Endure night:** The refuge is normally safe from direct attacks, but survival systems still consume resources. The surface becomes far more hostile. Players may stay inside, work, rest, or risk a night expedition.
6. **Repeat:** Better tools, weapons, storage, and eventually transportation extend the group's reach. Survive until rescue can locate and extract the group.

The loop must not require a siege every night. **Night is primarily a logistical and exploration deadline**, not a compulsory tower-defense round.

## 3. Victory, loss, and the beacon

The rescue beacon is the run's central objective. It must remain operational and powered to accumulate rescue progress. Difficulty determines the required number of successful signal days and may change environmental pressure.

- A short outage should be recoverable, for example by pausing progress.
- A prolonged outage should impose a serious setback; a full reset after multiple missed days is a candidate, **not yet a final rule**.
- Players must understand the beacon's status, the power it requires, and the consequences of an outage.
- Rescue should feel like a culmination of the group's preparation, not a timer that can be ignored. A final extraction event is an **open design choice**.
- A run can fail when the group can no longer recover, or when other explicitly communicated defeat conditions are met. Avoid sudden opaque game-overs.

A seven-day rescue requirement is the **first playtest target**, not the definitive length of a full game. The intended full-game length and save/resume model remain open.

## 4. Survival as a small interconnected economy

Keep the initial economy legible. Its central constraints are **food, power, materials, daylight, and human labor**.

- **Food:** Players consume it. Crops occupy plots and take days to mature; immature plants cannot be harvested. Hunting and scavenging provide alternatives. Prepared groups should be able to stay underground for roughly **one or two days** without an expedition.
- **Power:** A generator or other source supplies limited capacity and consumes fuel or other inputs. The beacon is essential, but workshops, grow lights, medical equipment, and other consumers compete for power. Turning something off is a meaningful choice.
- **Materials:** Salvage from the surface becomes repairs, tools, weapons, storage, improved production, and later vehicles. Avoid a needlessly granular inventory of screws and bolts.
- **Labor:** Planting, harvesting, repairs, cooking, and scavenging require player attention. Growth and machine operation can continue without constant clicking. More players add capability but also consume more resources.
- **Risk:** Better equipment makes dangerous exploration more survivable, while infrastructure investments improve long-term sustainability. Neither route should be universally optimal.

**Balance philosophy:** Early game resembles forgiving survival; later game is balanced survival with meaningful shortages. Shortages should first create choices and recoverable penalties rather than instant death. Scarcity should create memorable negotiations, not compulsory busywork.

## 5. The refuge

The underground refuge is a **relatively secure, shared home**, not merely a menu between missions. It should contain or eventually support:

- Garden plots with visually distinct growth stages and a small number of crops with different purposes.
- Shared storage and a place to prepare food.
- Generator, power distribution/priorities, and the radio beacon.
- A workshop for tools, weapons, repairs, and infrastructure.
- A recovery area and eventually a strange bioreconstitution device.
- Room for modest decoration, personalization, and visual improvements without turning decoration into a mandatory system.

Infrastructure should solve recognizable problems. A better generator frees power for other devices. A better workshop makes useful equipment. More plots increase production capacity but require seeds, time, and labor. The base should not demand constant maintenance once basic systems are stable.

## 6. The surface and procedural discovery

Each run generates a different, navigable surface with recognizable environmental themes, routes, landmarks, salvage sites, and risk gradients. The goal is **fresh decisions**, not random clutter.

- Nearby sites are accessible and relatively forgiving but contain finite salvage.
- More distant sites demand time, supplies, and equipment; they offer rare components and discoveries.
- Daytime threats vary by location. Some areas are dangerous even at noon.
- Night changes the threat landscape dramatically. Players should see or hear warning signs before the danger escalates.
- The surface contains quiet discoveries, traces of ordinary lives, and mystery, not just loot containers and enemies.
- Better backpacks, tools, weapons, protective equipment, and perhaps repaired vehicles expand the practical exploration radius.

Procedural generation should preserve authored-feeling landmarks, coherent routes, and safe traversal. A seed should be replayable for debugging, but players should not know the best route on a fresh run.

## 7. The Bloom and the Long Dark (working fiction)

The surface is being reclaimed by a strange, hostile growth. By day it can appear dormant, even beautiful. At night it becomes active: vines spread, flowers open, and some organisms hunt living things. Darkness and the Bloom are related, but the precise mechanism is **intentionally mysterious and undecided**.

The underground has an unusual protective quality, perhaps geological, biological, technological, or some combination. It should feel mystical without excluding useful machinery.

Do not lock the game into zombies, a specific catastrophe explanation, or a large monster roster yet. Favor threats that are readable, have counterplay, and reinforce the day/night distinction. Environmental danger may matter as much as direct combat.

## 8. Players, cooperation, and setbacks

- Target **2–6 players**, locally or online eventually, with meaningful participation for small and large groups.
- Players share a common rescue goal but can pursue different activities. Cooperation should emerge from shortages and opportunities, not assigned chores.
- Avoid mandatory classes initially. Roles, specialties, and friendly-fire rules are open design choices.
- Downed players should be rescuable. If lost, a recovered DNA sample plus scarce food/biomass/power may allow reconstruction.
- **Vegetable-human resurrection** is a welcome tonal accent: a revived friend may emerge with cosmetic botanical traits. Keep the joke charming and the resource tradeoff meaningful.
- Do not reward griefing or killing teammates as a dominant resource strategy. Whether betrayal is allowed, and in which modes, requires an explicit design decision.
- Scale per-person needs with player count, but avoid multiplying all infrastructure costs and chores linearly.

## 9. Art, sound, and interface

**Visual target:** Readable top-down pixel art with warm, layered underground spaces and a muted, overgrown, broken surface. Cozy cave gardens and humming salvaged electronics contrast with dark vines, ruins, fog, and nocturnal movement. Stylization matters more than asset count.

- Use coherent silhouettes, lighting, palette, and scale; do not leave the prototype permanently as generic rectangles.
- Make growth stages, powered machines, beacon status, hazards, salvageable objects, and approaching night visually clear.
- Atmosphere should come from environmental storytelling, ambient sound, music restraint, and contrast between safe and unsafe spaces.
- Interfaces should make resource decisions legible without drowning players in meters and menus.
- Humor should be occasional and situational; the world should retain its quiet sorrow.

## 10. What success feels like

A successful session produces stories such as: "We stayed home to harvest one more crop," "We shut down the workshop to keep the beacon alive," "We almost lost someone collecting fuel," or "We spent our last vegetables bringing our friend back."

We will judge the game by whether players **choose** to explore, negotiate resource use, divide work naturally, enjoy downtime, and want to attempt another run.

## 11. Decisions intentionally open

- Final title and detailed explanation of the catastrophe.
- Exact enemies, crops, recipes, machines, and numerical balance.
- Classes or equipment-driven specialties.
- Full beacon outage penalty and final extraction rules.
- Permadeath, friendly fire, betrayal, and resurrection timing.
- Fixed versus generated refuge layout; vehicles and distant outposts.
- Full-run length, saving, difficulty modes, and meta-progression.

The Roadmap should present these as decisions to test and ask the project owner about, not silently resolve them.
