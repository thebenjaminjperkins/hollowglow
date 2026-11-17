# VS0.1 — Chronological timing preflight

2026-11-17. **One proposed accelerated configuration, not approved or installed.**
This narrow arithmetic worksheet reads existing definitions and compares a fixed
real-time schedule. It is not a new playable economy, actor scheduler or replacement
for the accepted [14-day feasibility study](../../studies/economy14/README.md).

## Inputs and provenance

| Parameter | Unchanged baseline | Single proposed short-scenario candidate |
|---|---|---|
| Cycle and phases | 600s: Day360 / Dusk60 / Night180 | 240s: Day144 / Dusk24 / Night72 (same proportions) |
| Accumulated transmission target | 7 signal days | 3 signal days |
| Existing production, meals, power formula, resources and recipes | Read from `data/*.json` | Identical; no quantity/yield/cost override |
| Signal completion policy | Current terminal freeze | Future VS7 nonterminal opportunity; modeled analytically only |

These are the only timing/target proposals. Candidate phases are explicit so night
is not omitted from the comparison. Four initial plots and two seeds of each crop;
two personal Food each, one Food/player/cycle. Potato takes two cycles, produces
three Food + seed; Glowcap three cycles, two Food + Organic + seed. Running generator
burns `0.2 + 0.2 × supplied PU` Fuel/cycle, capacity4/tank5. Beacon2, bench2, lights1.
Two shared starter Fuel. Original bed costs12 Scrap/4 Organic; backpack8/3;
Potato propagation seed1/Organic1 produces seeds2.

Historical seed1701 garage result: preserve van → Scrap10, Food4, Fuel4, tool1;
dismantle → Scrap14 with restoration opportunity irreversibly lost. Garage contains
no Organic. No automatic seed propagation or construction without inputs.
Data are from [garage measurements](../M6B_TRAVEL.md) and current definitions.

## Common real-time schedule and actor occupancy

| Time | Actor A | Actor B | Interpretation |
|---|---|---|---|
| 00:00–00:30 | Withdraw and plant four plots | Withdraw Fuel, approach generator | Planting epoch00:30 for both crop groups; exact human planting stagger omitted |
| 00:30–01:00 | Prepare loadout and approach entrance | Load one Fuel; start at01:00; enable beacon+lights | Three-PU running demand; no additional free fuel/gear |
| 01:00–04:00 | Garage investigation/salvage | Same expedition, complementary search/hauling | Provisional three-minute human allowance, not a measured two-actor schedule or imposed timer |
| 04:00–04:30 | Deposit loot; each withdraw two Food at04:15 | Return, deposit and refuel one at04:30 | Dawn settles before arrival at04:00 in candidate; starting Food covers it |
| 08:00–10:00 | Farm service when mature; after harvest withdraw three Food each at09:00 | Refuel at08:00, disable lights/enable bench; reverse at10:00 | Four-PU bench load reservation is conservative budget, **not a completed craft** |
| 12:30 (candidate) | Harvest/replant both Glowcaps, store full yield | Free for inspection/preparation | Food4 + Organic2; no bed/backpack credited |
| 13:00 | Prepare evacuation | Refuel one at13:00 | Candidate signal-ready; no freeze; destination/window unapproved |
| 16:30 (candidate) | Harvest/replant both Potatoes again | Available for travel/preparation | If both depart earlier, this harvest may be skipped; baseline result below is an optimistic serviced case |
| 13:00–20:00 | Intended evacuation actions | Intended evacuation actions | No transport/combat/death simulation; timing allowance only |

No actor is assigned simultaneous farming and refueling. Human service durations,
combat and extraction are unmeasured; rows are a review schedule, not proof of
physical feasibility. At16:30 the optional farm service can conflict with evacuation:
skip it if both players have left rather than invent a caretaker or remote harvest.
Growth itself continues unattended.

## Chronological comparison

| Event | Baseline real time | Candidate real time | Consequence |
|---|---:|---:|---|
| First dusk / night | 06:00 / 07:00 | 02:24 / 02:48 | Candidate expedition ends during first night; encounter timing must acknowledge this |
| First dawn and meal deadline | 10:00 | 04:00 | Candidate starting two Food each cover first two dawns, not a free meal credit |
| Potato growing stage (35%) | 07:30 | 03:18 | Planted00:30; graphics retain existing stage fractions |
| First Potato maturity | 20:30 | 08:30 | Baseline gives no planted-to-harvest cycle within20 minutes |
| Glowcap growing stage (35%) | 11:00 | 04:42 | Existing automatic growth; no powered-light requirement |
| First Glowcap maturity | 30:30 | 12:30 | Candidate yields total Organic2, insufficient for either kit |
| Second Potato maturity with immediate replant | 40:30 | 16:30 | Optional second Food6 harvest, not required for extraction stock in this schedule |
| Second Glowcap maturity with immediate replant | 60:30 | 24:30 | First time Organic4 can exist using only these two Glowcaps, too late for20-minute investment |
| Signal target from generator start01:00, no outage | 71:00 | 13:00 | Requires continuous supply, future nonterminal policy and actual evacuation afterwards |

Whole-harvest room/seed safety remains mandatory. At eight-/twelve-minute dawns,
existing personal Food plus the04:15 explicit distribution covers the deadline
before the corresponding delayed harvest. At09:00, after Potato harvest, distribute
three additional Food per player. No communal automatic meal or future credit.

## Resource and capacity reconciliation through minute20

| Quantity | Baseline | Candidate |
|---|---:|---:|
| Starting Food + garage Food + crop Food | 4 + 4 + 0 = 8 | 4 + 4 + 16 = 24 |
| Food eaten (two actors) | 4 at two dawns | 10 at five dawns |
| Ending Food (personal / chest) | 4 / 0 | 4 / 10 |
| Missed meals | 0 | 0 |
| Fuel available (starter + recovered) | 2 + 4 = 6 | 2 + 4 = 6 |
| Fuel burned | 1.56 | 3.90 |
| Remaining loose / tank Fuel | 2 / 2.44 | 2 / 0.10 |
| Signal credited by20:00 | 1.9 / 7 days | 3 / 3 days, capped ready |
| Recovered Scrap, van preserved | 10 | 10 |
| Produced Organic | 0 | 2 |
| Completed bed/backpack/propagation investments | None | None |

Fuel integrates exactly: seventeen minutes at3 PU and two at4 PU, all after01:00.
Baseline burn `(17×0.8 + 2×1.0)/10 = 1.56`; candidate `/4 = 3.90`.
Manual whole-unit loads at01:00,04:30,08:00,13:00 total4; neither tank exceeds5
nor exhausts. No auto-loading/restart. Loaded Fuel is not double-counted as inventory.
At4 PU, lights must be off; beacon+bench+lights5 is rejected. No interruptions in
this schedule. Candidate outage grace and stable recovery each become one real
minute (0.25 cycle), with unchanged loss0.25 signal-day/off-day and cap1 signal day.
Startup has no outage penalty. Unplanned outages can delay13:00; no second candidate
or fabricated recovery stock is assumed.

The preserve-van haul fits six original slots: Food up to6 including personal stock,
Fuel4, Scrap10, tool1 occupy four slots. Dismantling adds Scrap4 in a fifth slot.
Future weapon/equipment occupancy is unresolved; do not claim those loads will
still fit without testing. Existing chest24 has room for the worksheet's typed
stacks (including seeds). Crop harvest6 Food and seed returns can be staged with
empty pack slots and explicit deposit; the worksheet does not execute slot commands.
Personal Food never exceeds one stack. No full or partial harvest resource is lost.

If the optional16:30 farm service is skipped, subtract Food6: candidate ends with
Food8 instead of14, still no missed meals under the explicit09:00 resupply. This
slack, starter supplies and available Fuel mask pressure. It is not proof that
farming, investment or power allocation feels meaningful in two-person play.

## Blocking findings and recommendation

1. **No feasible infrastructure investment in the strict garage-only20-minute arc.**
   Organic2 is short of bed4/backpack3; preserving the van also leaves Scrap10<12.
   Dismantling alone cannot solve Organic. Propagation costs Organic and competes
   with the project; it is not a source of free construction material.
2. **Proposed timing needs approval.** Faster clock shortens grace/meal deadlines and
   makes first night occur on the expedition. The baseline cannot demonstrate harvest
   or extraction in20 minutes. Shortening only beacon target leaves the crop conflict.
3. **Existing source detours are possible, not assumed.** The old M6a seed1701
   Organic-source trip was38.6s round trip, Scrap hut27.2s, each including4s handling.
   Those use the five-chamber chest, not the new hall or a combined two-person route.
   Approve visits and remeasure; no invented garage allocation or implicit relocation.
4. **Power/food success alone is insufficient.** Current load budget has no meaningful
   successful craft, no combat consumption or death gear. Resolve acquisition and
   investment access before a runnable scenario is accepted. Do not rebalance now.

Recommend approval of the four-minute/three-signal-day candidate **for a later
isolated profile only**, contingent on material-access resolution and human pacing
tests. It is not implemented or a recommendation to change full-game values.

## Reproduce

From repository root, Python standard library:

```sh
python3 docs/vs0.1/timing_worksheet.py
```

Outputs: [baseline ledger](timing-unchanged.csv), [candidate ledger](timing-candidate-not-approved.csv),
[summary](timing-summary.json). The script reads current definitions, uses rational
fuel arithmetic, asserts funded intervals/personal meals and refuses to credit
unavailable investments. It intentionally does not simulate networking, attacks,
physical labor/capacity or evacuation. Read the scenario limitations before using it.
