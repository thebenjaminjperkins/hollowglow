# M3b: finite surface Food scavenging

This milestone adds only finite Food sources. Existing farming, hunger, personal
inventories, stack limits, storage rules, starting supplies and clock values remain
unchanged. No power, enemies, new crop, resource type or terrain generator is added.

## Sources and world generation

| Band | Wild plants | Food containers | Band stock |
|---|---|---|---|
| Near | Two, 1–2 Food each | One, 1–3 Food | 3–7 Food |
| Far | One, 1–2 Food | Two, 2–4 Food each | 5–10 Food |

The six sources provide **8–17 Food**, expected mean 12.5 with uniform independent
rolls. The owner's approximate 9–17 total is superseded by the arithmetic of the
explicit approved ranges; no floor increase or other rebalance is applied.
Stock and source counts do not scale with players. Both types yield the existing
Food item, immediately edible through the existing personal inventory action.

`data/food_scavenging.json` configures source counts, quantity ranges, assets,
entrance, spacing and bands. `WorldConfig.WorldSeed` defaults to 1701; the normal
entry also accepts `--world-seed=<integer>`. F1 shows the active world seed.

Generation currently places resources on the two existing authored terrain layouts.
It does not create new terrain or expand the clearing. The pure placement model
accepts a layout and existing sites for future procedural-world integration.
Near/far are shortest cardinal walkable path distances from the surface entrance
cell center (344,184): near **32–112 world units**, far **160–288**. Distances use
16-unit terrain cells and actor-foot clearance around existing solid scenery and
salvage. These are compact-room distances, not large-world expedition claims.

Seeded candidate selection validates an accessible approach within the 22-unit
interaction radius. Sources keep 32 units apart, 48 from existing salvage, and a
quiet envelope around decorative props. Backtracking preserves all six sources;
unsatisfiable layouts/configurations fail explicitly instead of dropping sources,
changing quantity ranges or placing them out of reach. New sources are nonblocking
ground props, preserving current movement paths and all existing collision bodies.

The generation stream uses specified SplitMix64/FNV-1a domain labels. Placement
and per-source quantities have separate streams, independent of decoration,
interaction order and player count. A given seed/layout/configuration reproduces
source IDs, positions and initial quantities. The same surface in the two current
sanctuary variants therefore generates the same surface sources.

## Interacting, depletion and reset

Approach a source and press **E / controller A**. Gathering is instantaneous and
collects everything that fits in personal inventory, merging partial Food stacks
first. Any remainder stays at the source. A full backpack consumes nothing; deposit
or rearrange supplies and return. Interaction range and line-of-sight checks remain.

Source → personal → communal → personal transfers use the existing authoritative
inventory owner. Session/revision checks and its lock prevent conflicting gathers,
replays, duplication and loss. World access checks reject distant/wrong-actor/modal
commands. A source changes appearance only once its authoritative remainder is zero.
Containers remain visibly empty; picked plants retain small stems/roots. The nearby
prompt identifies an empty source, and repeat interactions produce no Food.

There are no respawn timers or quantity rerolls. Exploring away from a source,
using inventory, and advancing days preserve its depleted state. R is the existing
**new-run/debug reset**: it restores all supplies, crops, hunger and initial seeded
sources. Same seed recreates the same initial supply; a different seed changes it.
No cross-scene world persistence or disk save system is claimed in this slice.

## Run and validate

```sh
godot --path . -- --world-seed=1701
godot --path . -- --world-seed=499 --sanctuary-variant

dotnet build Hollowlight.csproj
dotnet run --project tests/Hollowlight.Core.Tests.csproj -- --scavenging-balance docs/M3B_FOOD_BALANCE.md
godot --headless --fixed-fps 60 --path . res://tools/ScavengingSmoke.tscn
godot --headless --fixed-fps 60 --path . res://tools/ScavengingSmoke.tscn -- --world-seed=499
godot --headless --fixed-fps 60 --path . res://tools/ScavengingSmoke.tscn -- --sanctuary-variant
godot --fixed-fps 60 --path . res://tools/ScavengingSmoke.tscn -- --capture-scavenging
```

Seeds 499, 1701 and 143 produce 8, 14 and 17 Food respectively. To check manually,
find a source, collect part of it with a nearly full Food stack, deposit and return
for the remainder. Deplete both types, leave and use F5; both should remain empty.
Store/withdraw the Food and eat it. R should restore the exact same seeded layout
and quantities. Existing farming and all original salvage sites remain usable.

Tests cover 256 seeds on both layouts, deterministic streams, ranges/counts/bands,
spacing, capacity, finite conservation, competing actors and reset. Live engine
checks cover actual approach collision, E input, partial/full collection, visible
states, storage/eating and no respawn across time skips. Legacy fixtures explicitly
disable scavenging to retain their original stock assertions; integrated hunger and
new scavenging checks run enabled defaults. Controller inventory events remain
covered by existing checks; physical controller A and actual multiplayer transport
are not validated here. Live play still controls one actor.

## Balance and review

[The seven-day report](M3B_FOOD_BALANCE.md) includes daily production, scavenging,
net flow, consumed/unmet meals, personal/chest reserves and unpicked supplies.
It uses actual seeded worlds, finite inventory gathering, unchanged crops and
personal dawn eating. Regular care produces 26 farm Food; delayed care produces 20.

With near supplies returned Day 1 and far supplies Day 3, regular care meets every
meal in all tested worlds for 2/4/6 players. Under delayed care, the eight-Food
world has two six-player meal shortages on Day 7; the 14- and 17-Food worlds have
none. In the low-stock/delayed-care stress case, deferring the far expedition until
Day 7 creates five missed meals on Day 6 for six players; Day 7 meals are then met. Two and
four players meet all deadlines in these scenarios. These are timing simulations,
not measured human expedition schedules or group playtests.

Scavenging is finite immediate relief. Agriculture is renewable but the unchanged
mixed four plots still average 4⅓ Food/day, below six players' 6/day. No system was
rebalanced to erase that accepted limitation. Review this milestone before power.

[Sprite sources, full prompts and processing](../assets/scavenging/README.md)
follow the approved pixel-art pipeline. [Gameplay captures](screenshots/scavenging/)
show both available and depleted families at normal zoom, without replacing old art.
