# Hollowlight VS0.1 — Baseline and vertical-slice planning package

2026-11-17. Authorized VS0.1 deliverables produced; **awaiting review**. No VS1/VS2
implementation, new playable profile, gameplay/artwork change or commit.

Planning authority is the [owner-revised Vision Update](../HOLLOWLIGHT_VISION_UPDATE_DRAFT.md).
The [Alignment Brief](../DEVELOPMENT_ALIGNMENT_BRIEF_DRAFT.md) retains the audit and
milestone structure. The owner's subsequent VS0.1 decisions recorded here supersede
the earlier [consistency review](../PLANNING_REVIEW_VS0.md) proposals where resolved;
those historical draft files are preserved without overwriting existing work.

## Deliverables

| Deliverable | Artifact |
|---|---|
| Captured tracked/untracked/deleted/ignored baseline | [Manifest](baseline-manifest.json), [checkpoint plan](BASELINE_CHECKPOINT.md), [exact path treatments](checkpoint-paths.json) |
| Written two-player hall–road–garage–return–extraction contract | [Scenario](SCENARIO_CONTRACT.md), including separate death/recovery test |
| Baseline versus one accelerated candidate | [Timing worksheet](TIMING_WORKSHEET.md), [reproducible arithmetic](timing_worksheet.py), [summary](timing-summary.json), [baseline CSV](timing-unchanged.csv), [candidate CSV](timing-candidate-not-approved.csv) |
| Minimal authority/observer/region interfaces | [Contracts](ARCHITECTURE_CONTRACTS.md) |
| Bounded visual/network implementation handoffs | [VS1/VS2 tickets](IMPLEMENTATION_TICKETS.md) |
| Fresh selected validation and preservation | [Validation](VALIDATION.md), [preservation](preservation.json), [package hashes](artifact-checksums.json) |

## Approved boundaries and resolutions

- VS0 defines a future scenario; it does not build extraction, register a profile,
  change runtime timing or start a renderer/network rewrite. C1/C3 resolved.
- Minimum region identity, navigation and spatial-authority contracts precede
  combat. Full procedural variety/VS3 acceptance does not block the first connected
  Bloom combat/recovery demonstration. C2 resolved as directed.
- Two Linux desktop instances, direct host/client; keyboard/mouse and controller
  retained. No permanent caretaker. Death/recovery has a dedicated test, not a
  required death in every successful playthrough.
- True full-party death fails immediately; departure with at least one aboard
  resolves rescue. C4 same-time arbitration, all-downed behavior and disconnected
  eligibility remain open until dependent milestones.
- Inventory/hash existing work, propose precise checkpoint; do not commit, restore,
  discard or overwrite existing work. Normal check caches/user data are separate.

## Findings requiring review

1. **Checkpoint:** recommend the integrated M6 review baseline, isolated study and
   revised planning/VS0.1 as three explicit groups, not a blanket dirty-tree commit.
   Four pre-existing rescue screenshots are missing and historical commit references
   disagree. Keep deletions unstaged/preserved until intent is decided. See exact plan.
2. **Accelerated timing:** proposed240s cycle (144/24/72 phases) and3 accumulated
   signal days reach readiness at13:00 with a1:00 start. This is **not approved or
   installed**. Potato/Glowcap mature at08:30/12:30 when planted00:30. First night
   starts02:48 on the expedition; meal and outage windows compress too. Full-game
   timing, resource values, recipes and study assumptions are unchanged.
3. **Infrastructure blocker:** strict garage-only salvage supplies no Organic;
   two Glowcaps supply only2 before20:00, short of bed4/backpack3. Preserved van
   leaves10 Scrap against bed12. No project was falsely credited as completed.
   Recommend authorizing small visits to **existing** Organic/hut Scrap sources
   rather than adding materials/rebalancing; measure those hall routes first.
   This changes the visit envelope and requires approval.
4. **Later policy approvals:** host-loss/JIP/reconnection/client disconnect, observer-
   local versus shared live sight and hidden-state replication before VS2; project/
   stockpile cancellation before VS4; weapon acquisition, attack/gear/death rules
   before VS5/VS6; exact evacuation/recovery costs/windows and C4 before VS7.

Fresh build/core, hall smoke, garage preservation smoke and all three existing art
checkers passed. Numerical and scripted results do not accept the current art or
prove connected play; no new visual/human/network claim is made. The timing schedule
has no missed meals, ample Fuel and material blockers, not proven enjoyable pressure.

## Review gate

Approve or revise the checkpoint groups/deleted-file handling, short-profile timing,
investment/material access and first bounded VS1/VS2 ticket/policy scope. This package
does not authorize those implementations. Stop here; no automatic VS1 or VS2 start,
no broader redesign, no economy rebalance and no final Gameplay & Economy Brief.
