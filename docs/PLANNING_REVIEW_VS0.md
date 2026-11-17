# Revised planning drafts — consistency review and proposed VS0 scope

Review date: 2026-11-17. Documentation only; VS0 has not begun.

## 1. Integration and authority

The owner's revised files are incorporated at the existing review-draft paths:

- [Vision Update](HOLLOWLIGHT_VISION_UPDATE_DRAFT.md), imported from
  `HOLLOWLIGHT_VISION_UPDATE_REVISED.md`.
- [Development Alignment Brief](DEVELOPMENT_ALIGNMENT_BRIEF_DRAFT.md), imported
  from `DEVELOPMENT_ALIGNMENT_BRIEF_REVISED.md`.

Each import retains the supplied body, including its links, audit, qualifications
and milestone structure. Only an integration/provenance note was added. Existing
approved [Vision](../Hollowlight_Vision.md), [Roadmap](../Hollowlight_Roadmap.md),
[decision history](DECISIONS.md), gameplay and evidence remain untouched.

For **subsequent planning**, the revised Vision Update is the intended-design
authority, as directed by the user's current request. The approved historical
documents still record prior approvals and the live baseline. The Alignment Brief
is an audit/proposal and cannot override that intended vision or grant permission
to implement it. Its embedded “handoff,” “in parallel” and milestone instructions
are reviewed planning content, not authorization to execute VS0–VS8, delegate work,
commit the dirty tree or replace graphics. The present authorization covers draft
integration, consistency review and a recommendation only.

## 2. Remaining consequential tensions — not silently resolved

| ID | Passages | Finding and recommendation for approval |
|---|---|---|
| C1 | Alignment §7 handoff item 2 versus VS0 objective and VS7 | “Short playable home–garage–return–extraction scenario” could require extraction during VS0, although VS0 is baseline/planning and VS7 implements extraction. Recommend VS0 specify the future scenario and replay the existing baseline only; no new gameplay or playable extraction in VS0. Approve this boundary before starting. |
| C2 | VS3 scope safeguard versus VS3 completion and VS4→VS5 dependencies | Variety may be deferred until after combat/recovery, but mandatory VS3 varied geography/region-instance acceptance can still block that combat. Recommend gate only stable identities, the existing garage's access/conservation and the minimum region-boundary contract before VS4/VS5; defer nonessential variety. Decide whether the independent region-instance proof must also precede combat or may follow VS6. Do not delete the full-game generation requirement. |
| C3 | Alignment §4 “each step … playable demonstration” versus VS0 | VS0's contract is not new playable functionality. Recommend its demonstration use the unchanged baseline, with a written scenario walkthrough. Keep actual connected and whole-scene demonstrations mandatory at VS2 and VS1. |
| C4 | Vision §§10/12; Alignment VS6/VS7 | Immediate full-party death and transport departure can coincide; downed and disconnected players also affect survivor/boarding eligibility. No agreed arbitration or all-downed rule exists. Preserve immediate true-wipe failure and departure-based ending; decide boundary cases before VS6/VS7 rather than inferring them now. |

C1–C3 are sequencing/scope decisions, not evidence that the intended design is
incoherent. C4 is an explicitly unresolved rule that prevents a complete terminal
state specification. None authorizes a new default or blocks this documentation work.

## 3. Prior approvals versus intended design

These are deliberate, documented differences. Preserve current functionality now;
future implementation needs its own approved migration/profile scope.

| Topic | Historical/current baseline | Revised direction and review result |
|---|---|---|
| Sanctuary security | Older approved Vision/Roadmap leave durability/breach questions open | Closed paired doors/intact walls protect against ordinary Bloom, remain usable without electricity, no mandatory sieges. Explicitly superseded for planning; not an unresolved invitation to add breaches. |
| Sanctuary layout | Five-chamber prototype; newer hall opt-in | Compact irregular square hall with perimeter functions. Already compatible with approved follow-up; approximate 24×24 and reservations remain provisional, not immutable dimensions. |
| Art/rendering | Approved modular PNG/64-color/2px-per-world-unit baseline; technical corrections preserved artwork | Whole-scene coherence overrides loyalty to palette/shader architecture. Comparison/approval still required for replacement; no permission to replace art in this assignment. |
| Beacon ending | Seven accumulated days, terminal Rescued screen freezes the prototype | Beacon establishes extraction opportunity; run continues. Intentional future replacement of terminal policy, not a current regression fix. Keep accounting/outage model as a baseline pending review. |
| Construction | Immediate powered personal-ingredient recipes, transferable kits, one fixed fifth socket | Delivered local stockpiles, brief assembly, flexible designated zones. Distinct future ownership/workflow; no remote communal auto-spending implied. Exact kit/raw-material routing remains open. |
| Equipment/hotbar | Six backpack indices reserved for a future hotbar, two upgrade slots storage-only; installed upgrade bound to actor | Integrated six-slot hotbar is compatible. Transferable equipment and death packs still require installed-upgrade/equipped-item rules; no functional hotbar or removable backpack exists yet. |
| Resources and production | Scrap/Organic IDs, four plots/max fifth, current crops/recipes | Seven category direction and future Food/Biomass autonomy. No automatic rename, eight-plot upgrade, Marrowvine or generator migration. Preserve live values. |
| Death/resurrection | No implemented health/death/recovery system; earlier DNA/food ideas provisional | Dropped gear, limited emergency recovery, repaired Biomass resurrection, immediate true-party wipe. This supersedes speculative older recipes without assigning new counts/costs. |
| PvP | Optional future conflict with permissions unresolved | Retained as future direction. No implicit friendly fire or betrayal implementation. |

Enhanced indoor lighting in Vision §5 is compatible with **no power requirement
for basic crops**: enhancement effects remain unapproved. Botanical oddities in
the experience examples are thematic opportunities, not mandatory mutation stats,
frequency or a resurrection side effect. Cooking is explicitly not assumed; a
dependable wheelbarrow does not introduce durability or random failure.

## 4. Provisional-value and link checks

No candidate numerical value was found promoted into a final full-game requirement
by the revisions. Preserve these distinctions:

- 15–20 minutes is a development validation target, not full-game duration or a
  hard timeout. A second-chance branch may exceed it; its limits remain undecided.
- Four-minute cycles, three signal days and the minute-by-minute pacing envelope
  are illustrative **unapproved** short-profile options. Ten-minute cycles and
  seven-day terminal rescue remain current gameplay. Fourteen days is a Standard
  candidate studied separately, not a final runtime change.
- Four starting plots is intended policy. Eight possible prepared plots, new
  recipes/world supplies and Biomass operating values remain study assumptions;
  the game still supports only the existing fifth upgrade.
- Approximately 24×24 hall, 2–4-minute expedition target, current density/palette,
  single-weapon/enemy slice and three fresh playtest pairs have distinct roles:
  provisional geometry/pacing/production baseline or **proposed validation scope**,
  not final game-balance mandates.
- Recorded 23 engine cases, six windows/actor proxies, 16 seeds and walking timings
  are historical evidence. They do not establish current human or network acceptance.

All Markdown relative links in both integrated drafts resolve from `docs/`; no
link rewriting was needed because the existing filenames were retained. Links to
historical reports establish the report exists, not that every nested historical
artifact is present. In particular, the four previously deleted rescue PNGs remain
missing; this review neither restores nor deletes evidence.

The Alignment Brief's checkout audit still matches observed HEAD `6f7e0e0` and
the existing dirty-tree/deleted-image state. Conflicting historical commit hashes
in prose logs remain a provenance issue; do not rewrite history or claim a clean
checkpoint. Document reconciliation and approved checkpoint scope belong in VS0.

## 5. Smallest recommended next step — VS0.1 baseline and scenario contract

**Proposal only.** Bound VS0.1 to a reproducible existing baseline and written
implementation plan. Do not create/register a playable review profile, change
timing, implement extraction or begin the renderer/network prototypes yet.
Those actions require the later approved work packages.

### Deliverables

1. **Baseline manifest:** HEAD, dirty/untracked/deleted paths, selected content
   hashes, tool versions and existing fixture/test commands. Record current
   omissions as omissions. Propose a precise commit/include/exclude list, but do
   not commit, restore or discard existing work without authorization.
2. **Scenario contract:** one proposed separate profile using hall + existing
   road/garage, two eventual connected players and the intended home–expedition–
   return–extraction arc. Identify the minimum investment, encounter, recovery and
   ending demonstrations, assigning each to VS1–VS8 rather than implementing them.
3. **Chronological timing worksheet:** compare unchanged ten-minute baseline with
   a proposed short-profile clock/target. Include planting, two-/three-cycle crop
   maturity, actual travel lower bounds, meal deadlines, fuel burn, workshop power
   conflicts and extraction/recovery allowance. Identify where two starting Food
   and finite stocks mask pressure. Keep yields, quantities and recipes unchanged;
   expose infeasibility instead of fixing it through rebalance. This is a narrow
   scenario preflight, not a rerun/replacement of the 14-day economy study.
4. **Minimal contracts and test plan:** document session/actor/region/object IDs,
   host-validated command/results, local observer context, independent explored/
   current/illumination inputs and nonterminal signal-ready versus terminal run
   outcomes. Specify consumers and example messages/states only; no transport,
   general framework, renderer rewrite, live multiplayer or extraction code.
5. **First implementation handoff:** bounded VS1 visual comparison and VS2 network
   proof tickets, with explicit dependencies, budgets, acceptance evidence and
   a stop gate. Record the C2 sequencing decision and unresolved later mechanics.

### Acceptance criteria

- Existing approved documents, gameplay, JSON balance/configuration, artwork,
  shaders, generators, studies and fixtures retain their pre-VS0 hashes. The only
  additions/changes are the explicitly approved planning/baseline artifacts.
- Another developer can identify the exact baseline and reproduce its documented
  build/core/smoke commands. Run selected existing checks in VS0.1 and report actual
  outcomes/tool blockers; do not describe historical green runs as new passes.
- The scenario identifies at least one meaningful investment, expedition, threat,
  return, recovery test and evacuation, with two-player responsibilities and room
  for quiet play. New functionality is clearly assigned to later milestones.
- Timing differences are enumerated, crop/meal/fuel chronology reconciles and any
  impossible or uncertain requirement is visible. No 20-minute deadline, shortened
  full-game promise, free materials or hidden inflation of interaction timers.
- Written authority/observer/region boundaries are sufficient to scope VS1/VS2;
  unresolved control/death/extraction rules have decision owners and deadlines.
- Owner approves the scenario contract, timing proposal and first implementation
  tickets. VS0.1 completion itself does not authorize VS1/VS2 execution.

### Decisions requested before VS0.1 starts

| Decision | Recommendation | Alternative/tradeoff |
|---|---|---|
| VS0 boundary (C1/C3) | Baseline capture + scenario/contracts/worksheet only; unchanged-baseline walkthrough | Build an opt-in profile shell now; introduces runtime changes without yet delivering the arc. A full playable extraction in VS0 would require a material replan. |
| Baseline handling | Inventory/hash existing dirty state; propose a separate checkpoint include/exclude list for approval | Authorize a defined checkpoint now, naming which existing changes and deleted evidence belong; avoid a blanket commit of unrelated work. |
| Timing experiment | Evaluate one coherent accelerated clock/target candidate against unchanged baseline; leave actual values pending worksheet approval | Independent mission/crop overrides provide control but represent the full economy less faithfully. Four minutes/three days is only an illustrative candidate. |
| Scenario envelope | Existing hall/garage, one investment, one weapon/enemy, death-pack/emergency-recovery test, short extraction as eventual deliverables | Chamber repair, extra equipment or a new location widen the slice and delay the first human encounter test. None is selected automatically. |
| Initial connected-test target | Propose two desktop machines, direct-connect host/client, start together; confirm OS/input devices and host-loss/reconnect expectations in the contract | Wider platforms, join-in-progress or reconnect add early acceptance work. No transport/platform selection is implemented in VS0.1. |
| Pre-combat dependency (C2) | Defer nonessential geography variety; define the minimum region-identity/access proof before combat | Require full VS3 region-instance proof first; reduces integration uncertainty but delays the first encounter. Explicitly choose the boundary. |

Detailed aiming/damage, downed/wipe arbitration (C4), emergency counts, construction
cancel/overflow, exact enhanced farming, transport window and second-chance costs
can stay open during VS0.1. Give them deadlines at their dependent milestones;
do not make every later rule a blocker for baseline capture.

## 6. Review stop

Only draft integration and this consistency/scope report were produced. No VS0
implementation, profile registration, gameplay/artwork work, commit or new gameplay
validation is claimed. Review the proposed VS0.1 boundary and consequential tensions
before authorizing the next work package.
