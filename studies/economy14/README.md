# Hollowlight: isolated 14-day economy validation study

This is a reproducible numerical study of a **candidate future economy**, not the
Gameplay & Economy Brief and not an implementation of that economy in Hollowlight.
Playable source, recipes, quantities, graphics, world generation and existing tests
are not modified. `.gdignore` keeps study artifacts out of Godot import.

## Default-run findings — validated 2026-11-16

- **81/81 scenarios rescued at Day 15, 00:23**: all 27 baselines and all 54
  individually applied setbacks. Every setback triggered. No missed meals,
  transmission interruptions or signal degradation occurred in these policies.
- **24/27 baselines confirmed underground operation.** Most confirmations fall
  on Days 8–11; the latest is Day 14. The initial feasible-schedule date is reported
  separately from the date the 6.5-day observed continuation passes.
- Two six-player Highway strategies (infrastructure/exploration) obtain a feasible
  witness on **Day 12**, but rescue ends the run before their operational confirmation
  window finishes. These are **confirmation censored by victory**, not proven
  unsustainable economies or failed rescues.
- Six-player Valley exploration-first cannot buy the biomass generator: equipment
  and three new beds leave **9 Metal against a 10-Metal cost**, with no Metal left
  at sites. This strategy still rescues using Fuel; it has Food production and
  Biomass reserves but lacks renewable generation machinery.
- The missed-harvest suite adds no missed meals or rescue delay. The empty far
  expedition also leaves rescue timing unchanged; confirmed independence is
  delayed by up to **one in-game day** in matching scenarios.
- Occupied actor time is approximately **5.5–12.6%** of available actor-seconds.
  Current travel/labor, ample Fuel and regrowing wild Biomass leave substantial
  logistical slack. This evidence supports reviewing the intended pressure before
  finalizing the Brief, not automatically increasing costs or threats.

All 23 study tests and the pre-existing C# core/regression suite passed. An independent
verifier reconciled every daily resource balance and actor/location schedule. A
second full 81-scenario run produced **byte-identical report and event-log artifacts**.
A pre-study snapshot confirmed all **450 pre-existing files unchanged**; the existing
uncommitted M6a work remains intact. See the detailed reports for policy limitations.

## Reproduce

Python 3.10+ standard library only, from the repository root:

```sh
python3 -m unittest discover -s studies/economy14 -v
python3 studies/economy14/run.py
```

The second command runs **27 baselines + 54 paired setback scenarios**. To compare
another deterministic run without overwriting the checked-in evidence:

```sh
python3 studies/economy14/run.py --output /tmp/hollowlight-economy14-replay
```

`--baseline-only` limits the run to 27 configurations; `--config PATH` selects an
alternative study configuration. Alternatives are sensitivity experiments, not
authorization to rebalance the game. Do not merge their outputs into the default
report directory. The runner exits nonzero on a model exception/invariant failure.
A 28-day observation cutoff is an ordinary outcome, not a simulation exception or
an invented permanent-death condition.

## Evidence

- [Results and nine representative daily ledgers](reports/RESULTS.md).
- [All 81 scenario summaries](reports/summary.csv).
- [All daily ledgers](reports/daily.csv), with cumulative production/recovery,
  resource spending, generator burn, remaining site stocks, cargo, upgrades,
  necessary capacity/reserve dates and activity totals.
- [Exact initial location inventories](reports/location-inventories.csv), including
  the seeded Marrowvine opportunities and per-patch replenishment periods.
- [54 paired setback comparisons](reports/paired-setbacks.csv).
- [Candidate investment material comparisons](reports/candidate-investments.csv).
- [Full structured results](reports/results.json), including proof witnesses,
  confirmation failures and completed investment dates.
- [Configuration and source hashes](reports/inputs.json) and
  [artifact checksums](reports/checksums.json).
- `reports/events/*.jsonl.gz`: deterministic compressed chronological action logs.
  Each completed action records actor, actual location, start/end, whether applied,
  quantities and electrical command results. For example:

```sh
python3 -m gzip -dc studies/economy14/reports/events/old_highway__balanced__6p__baseline.jsonl.gz
```

No wall-clock timestamps, Python object IDs or randomized dictionary iteration
enter reports. Compressed logs use an empty filename and gzip mtime zero.

## Rule provenance

The owner approved the candidate values in conversation. `config.json` is the
complete executable input, including all 18 location inventories, core recipes,
ten inactive candidate projects, crop yields, starts and equipment parameters.
Model-source hashes preserve policy and timing assumptions not expressed as data.

| Implemented baseline, read only | Isolated candidate replacement/addition |
|---|---|
| Ten-minute days, 6/1/3 phases | Retained |
| Seven accumulated signal days | Fourteen signal days; observe up to 28 elapsed days |
| Six-slot personal, stack ten, chest 24 | Retained; transferable backpack gives eight slots |
| Four plots, implemented maximum five | Four initial / eight possible prepared plots |
| Existing Potato / Glowcap values | Same growth/yields; study-only Organic → Biomass |
| Scrap / Organic resource IDs | Study-only Metal / Biomass; no game rename |
| Personal hunger, two starting Food | Same dawn settlement and gradual recovery |
| 4-PU Fuel generator, five-Fuel tank | Same formula, now applied to its proportional share |
| Single generator | Add 2-PU biomass generator, combined bus, manual controls |
| Implemented finite salvage budgets | Owner-supplied profiles replace them; never add both |
| Immediate playable crafting | Proposed study assembly labor; no playable recipe change |

Starting supplies are two personal Food each, two shared Fuel, two Potato seeds
and two Glowcap seeds. No free parts, Metal, Biomass, gear or Marrowvine seeds.
Existing generator/workbench/beacon/chest/lights and four prepared plots are
already functional infrastructure; there is no invented beacon repair prerequisite.

## World acquisition and regrowth

Metal/Basic/Advanced/Fuel/Food totals are exactly the supplied 65/21/3/16/12,
50/18/2/9/18 and 70/27/5/11/12. Wild patch capacities total **12/20/10 Biomass**:

- Highway: three patches, capacities 4/5/3, at verge/greenhouse/depot vegetation.
- Valley: four patches, capacities 6/4/6/4, at nursery/farm/glasshouse/quarry growth.
- Industrial: two patches, capacities 6/4, at seep nursery/machinery-yard verge.

A deterministic hash chooses each patch's **3, 3.5 or 4 days to full replenishment**.
Regrowth is gradual and linear below its capacity; it starts after harvesting and
never accumulates overflow above capacity. Only whole units are harvestable.
The first regenerated whole unit can therefore appear earlier than three days;
this is not a three-day lockout followed by a full respawn. Source biomass work is
two seconds per unit plus pile handling, limiting collection rate as well as stock.
Patch counts/capacities never multiply with player count. All other salvage and
seed opportunities remain finite, with persistent depletion.

Seed opportunities contain one Marrowvine seed each. Counts are seeded within
Highway 1–2, Valley 2–3 and Industrial 1–2; their locations are among profile
locations 2–4 and do not require a tool. Seed propagation uses one seed + one
Biomass → two seeds for each of the three crop types. First harvests may bank a
returned seed for propagation instead of immediately consuming it in replanting.
No immature crop is destroyed to change allocation. All strategies can cultivate
all four initial plots; one surplus Glowcap can become Marrowvine after harvest.

Hand-accessible Metal/Basic Parts are at locations 1–4. Mechanical piles at 5–6,
and all Advanced Parts, require a physically carried Salvage Tool. Food, Fuel,
vegetation and seeds need no tool. Each resource is a separate pile; carrying
limits permit partial collection and retain the remainder.

## Travel, exploration and task scheduling

The model uses M6a seed-1701 measured movement legs, conservatively rounded up to
whole seconds, after subtracting its four seconds of handling. Source measurements
and hashes are retained. Outbound/return seconds for location classes 1–5 are
12/12, 9/9, 18/18, 18/18 and 34/34. Location 6 is a **provisional 60/60-second
extension**, not a measured M6a journey.

Strategies initially know nearby trailheads 1/2. Visiting 1 reveals 3 then 5;
visiting 2 reveals 4 then 6. A location's contents are unavailable to decision
scoring until visited; revealed landmark names can suggest vegetation. Knowledge
and observed depletion are shared. Known patch regrowth is predictably modeled.
Unknown-site admission uses a fixed 120-second handling/exploration allowance,
not hidden quantities. Baseline trips are single-site returns, not optimized
multi-stop tours. Policies admit trips expected to fit the 360-second daylight.

Measured sanctuary workbench/Fuel-generator/garden approaches are rounded up to
3/5/3 seconds. Other station-to-station travel conservatively routes via storage;
plot-to-plot walking uses the study's construction grid. The biomass utility
installation uses a provisional five-second storage approach. These are spatial
approximations, not new Godot movement measurements.

Actors have one action at a time and a persistent location. Main task progress is
scheduled in one-second quanta, rounding action completion conservatively. Travel
progress changes with the actual actor's hunger speed. Power/exhaustion/rescue and
wild regrowth use exact rational fractional accounting inside each quantum,
including partial exhaustion and exact terminal victory. Meals settle before task
commits at the same dawn. Scene/menu use is not simulated as a global pause.

Collection, transfers, seed planting, whole harvests, assembly and installation all
validate location, quantities, limits and prerequisites. Per-site, per-plot,
workbench and shared-equipment reservations prevent conflicting commitments.
Some optimistic resource-preparation requests can be rejected after another actor
uses stock; rejected requests commit no unavailable operation and are logged.
Personal ingredients are removed only on successful assembly. A paused powered
assembly retains its actor/materials; another home actor can service power.
Completed kits occupy a slot while transported to installation.

Timing assumptions beyond measured walking:

| Action | Seconds |
|---|---:|
| Pile handling / chest resource-type transfer | 2 |
| Loose Metal/Basic, per unit | 1 |
| Mechanical Metal/Basic, per unit | 2 |
| Advanced Parts, per unit | 5 |
| Food/Fuel/seed collection | Pile handling only |
| Plant / harvest, per plot | 3 each |
| Generator load / control command | 2 / 1 |
| Bed assembly / clearing / installation | 10 / 30 / 15 |
| Biomass generator assembly / installation | 30 / 30 |
| Tool assembly | 20 |
| Wheelbarrow assembly / deployment | 30 / 5 |
| Backpack assembly / equip | 20 / 2 |
| Seed propagation | 5 |

Agricultural footprints are eight non-overlapping 2×2 placements in a 10×6-cell
zone with protected cross aisles. The first four are prepared; additions choose
nearby legal footprints. One biomass utility installation is targeted by these
policies. This study does not evaluate free-placement collision physics or
alternative utility-room arrangements.

## Equipment, electricity and policies

The five core costs remain exactly approved: bed 6 Metal/5 Biomass; generator
10 Metal/4 Basic; tool 5 Metal/3 Basic; wheelbarrow 10 Metal/2 Basic; backpack
8 Metal/2 Basic. No repair/durability consumption is charged.

Equipment completion dates mean the item is physically ready; the separate equip
actions record when a backpack actually increases a particular actor’s capacity.
The tool occupies one slot and unlocks mechanical piles. The shared backpack
adds two slots to one actor; it occupies a slot when stowed. The shared wheelbarrow
adds eight cargo slots, has one owner/pusher, and returns physically to storage.
The policies keep the backpack on its selected hauler; mid-run backpack reassignment
is not exercised. Tools and the wheelbarrow return to communal access between
expeditions. The wheelbarrow travels at normal walking speed. Road access: Highway all except 4; Valley
1/3/5; Industrial all except 3. It cannot teleport to a second carrier or traverse
foot-only routes.

Fuel/biomass generator capacities are 4/2 PU. Proportional load sharing uses running
capacity. Fuel burns `0.2 + 0.2 × its supplied PU` units/day, including idle cost.
Biomass burns one unit per supplied PU-day (two at full output), no idle cost.
Tanks are five Fuel / six Biomass, loaded manually in fitting whole units; neither
unit automatically restarts after exhaustion. Demand is beacon 2, workbench 2,
optional lights 1. Over-capacity activation is rejected. If capacity falls below
enabled demand, the whole bus is unsupplied, selections remain, and an actor must
restore capacity or reduce demand. Running Fuel units still pay idle cost then.

Night lighting uses available already-running capacity, is disabled for workshop
service when needed, and does not justify starting another generator on its own.
The policy manually stops Fuel once two Marrowvine/one Glowcap plots are growing,
a biomass unit is running with reserve, and the workbench is not needed. This can
precede the first Marrowvine harvest, using collected Biomass as a bridge. The
separate confirmation test determines whether that becomes an underground loop.

All strategies initially plant two Potatoes and two Glowcaps before propagating
seeds. Every strategy prioritizes electrical service and personal food access;
eating uses the existing automatic personal-inventory dawn settlement, rather
than manual mid-day recovery. Actor 0
is the resident farmer/builder; others explore/haul and can service electricity.
Production goals by party size are Potato 2/3/4, Glowcap 1, Marrowvine 2: five/six/
seven plots. Goals are investment policies, not assertions that those assets suffice.

- **Infrastructure-first:** production expansion/generation first; buy the tool when known
  mechanical piles must be unlocked to fund the next production project, otherwise afterward. Accessible material piles and hand-collected Fuel make this
  possible without pretending locked mechanical piles are already recovered.
- **Exploration-first:** tool, wheelbarrow, backpack before expansion/generation.
- **Balanced:** tool, production expansion/generation, then wheelbarrow/backpack
  when affordable. Next production costs take precedence over optional transport.

Unknown exploration uses revealed distance/landmark hints. Known sites supply
shortages: Food reserves, remaining signal Fuel budget, bed/propagation Biomass,
core materials and seeds. All purchases require actual personal withdrawals.
After a repeatable underground schedule is found and affordable policy projects
are complete, a confirmation trial stops external Food/Fuel/Biomass/seed gathering
and further construction. Other salvage may continue. This maintenance-mode change
is explicit, not an automatic resource subsidy or hidden optimization.

## Sustainability: capacity, witness, confirmation, reserves

Necessary conservative capacity uses 1300 seconds per Potato harvest and 1900 per
Glowcap/Marrowvine harvest (100 seconds of service slack beyond existing growth).
Dates meeting capacity + one-day reserve are **necessary indicators**, not proofs.

`sustainability.py` constructs a no-surface/no-Fuel continuation with two workers,
100-second service windows, exact personal/chest capacity, whole harvests,
replacement seeds, daily meals, manual hopper loads and conservative batched farm
labor. It curtails harvesting when stocks are high; it never discards overflow.
It must find an **identical bounded inventory/crop/meal-phase/hopper state**, proving
a repeating continuation with no further external inputs. Search limit is 20,000
windows; failure to find a cycle means "not demonstrated", not impossibility.
At least one day's Food and two Biomass reserve must be maintained. A six-Fuel
stockpile or the ownership of a generator alone cannot pass this verifier.

The actual strategy then operates without new external Food/Fuel/Biomass. Confirmation
requires at least **6.5 elapsed days**, no new misses or outages, no sub-day reserves,
and ending Food/Biomass stocks at least as large as at the trial start. Failed
trials resume normal acquisition and are recorded. Reserves are monitored throughout the main run, including before harvest replenishment.
The confirmed date is the date
these tests pass, not the earlier machine purchase or feasible-witness date.

Wild and crop production have separate counters. Stored quantities are reported
separately from rates. Conservative crop-origin reserve lower bounds subtract all
consumption from crop output first; a zero lower bound does not prove no crop units
remain. There is no arbitrary claim that fungible Fuel/Biomass in a hopper has a
physically tracked origin. Personal/communal reserves and cargo are separate fields.

## Setbacks and interpretation boundaries

Each baseline is paired separately with: (1) first mature harvest on/after Day 4
postponed 300 seconds, with no crop destruction; (2) first far expedition returns
empty after arrival, leaving the source intact. Trigger time is logged. No damage,
combat, permanent death, item-loss mechanic or new threat is invented.

Ten additional candidate projects retain the supplied costs but **cannot be
constructed in strategy runs** because effects/prerequisites/timing are undefined.
Material-only feasibility is reported per project, not as an invented operational
benefit. Gross recovery and current uncommitted material affordability are distinct;
shared ingredients cannot actually be spent on multiple projects simultaneously.

A rescue can succeed without a renewable underground loop. Missed meals are not a
new defeat rule. Finite shortages can block a strategy's shopping list without
making every possible strategy or rescue impossible. Observation cutoffs and
model exceptions have different statuses. Numerical results do not prove human
coordination, threat balance, discovery time, physical controller behavior or
networked multiplayer. No combat, sleep/fatigue, vehicles, batteries, fertilizer
benefits or resurrection behavior is simulated.

## Verify the evidence

After reproduction, validate checksums, source inputs, every daily resource balance,
chronological actor locations, paired initial generation and sustainability witnesses:

```sh
python3 studies/economy14/verify.py
python3 studies/economy14/verify.py --compare-dir /tmp/hollowlight-economy14-replay
```

The second command requires a completed reproduction first. Generated inputs hash
the model, configuration, study tests, reporting/verification code, M6a measurements
and relevant runtime JSON files. Source edits intentionally invalidate old evidence;
rerun the study after any change. On this development run, all pre-existing repository
files were also compared against a pre-study SHA-256 snapshot; that local snapshot
is external to the repository and is not required for ordinary reproduction.

## Arithmetic to keep in view

Beacon-only Fuel operation for fourteen transmission days costs **8.4 Fuel**, before
workshop/lighting overhead. Starting plus surface Fuel is **18 / 11 / 13**, so the
baseline generation budget alone does not make underground biomass power mandatory.
The study tests its usefulness and independence rather than assuming it is required.

At full long-run patch utilization, the approved wild capacities and 3–4-day refill
periods permit **3–4 / 5–6.67 / 2.5–3.33 Biomass per day**, before travel and labor.
All exceed one generator’s two-Biomass/day beacon load. This creates a second
renewable-power route through repeated surface gathering. The underground metric
therefore excludes *all* future surface inputs, including regrown wild Biomass.
These are capacity bounds, not claims that every strategy recovers that output.

The active catalog leaves Advanced Parts unspent. Their economic value cannot be
inferred from extra inventory alone. Implementing effects for the ten inactive
candidate projects is a separate design decision; no price, supply or mechanic is
automatically changed by this study.
