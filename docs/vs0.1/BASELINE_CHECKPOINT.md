# VS0.1 — Baseline and proposed checkpoint plan

Captured 2026-11-17, before validation or VS0.1 document/tool outputs. No staging,
commit, restoration, discard or edit of existing work has been performed.

## Captured identity and inventory

- Branch `main`, HEAD `6f7e0e060254d64b137fe7638cd7af6c61678491`, “Complete M5a
  basic crafting and resource investment.” HEAD alone does not reproduce this tree.
- [Machine-readable baseline manifest](baseline-manifest.json): every Git tracked,
  untracked and ignored working-tree file, excluding `.git` internals and the new
  `docs/vs0.1/` outputs to avoid self-hashing. Existing file bytes use SHA-256;
  missing tracked files use null working hash plus HEAD blob/object and SHA-256.
  Index object/mode/stage and working status distinguish staged/unstaged content.
- 743 existing nonignored files, four missing tracked PNGs, 173 ignored cache/build
  files. Hashes do not back up bytes; preserve the checkout and existing assets.
  A later checkpoint must include untracked dependencies, not just `git diff`.
- Git state at capture: 360 unchanged tracked, 33 modified tracked, four deleted
  tracked and 350 untracked files; nothing staged. VS0.1 outputs are a separate
  subsequent addition, not part of the pre-work counts.
- Raw [pre-work status](validation/git-status-before.txt) and
  [diff summary](validation/git-diff-before.txt) are preserved. Status expands
  untracked files rather than collapsing them to directories.

| Proposed treatment | Files | Working bytes | Meaning |
|---|---:|---:|---|
| Preserve already tracked, unchanged | 360 | 31,451,717 | Already in HEAD; no new change needed |
| C1: integrated M6 review baseline | 281 | 35,832,530 | Existing modified/untracked runtime, assets, tests, data and evidence |
| C2: isolated economy study | 99 | 6,663,443 | Existing study sources/config/reports/logs; accepted feasibility evidence |
| C3: planning drafts | 3 | 83,045 | Existing revised drafts and consistency report |
| Investigate deleted evidence | 4 | — | Do not stage deletions or restore without decision |
| Exclude ignored generated cache/build | 173 | 11,337,861 | Hash recorded at capture; normal validation may regenerate them |

[checkpoint-paths.json](checkpoint-paths.json) enumerates **each exact existing path**
under these treatments. Classifications are recommendations, not claims of owner
acceptance of every M6 visual. None has been committed.

## Proposed commit sequence, only if later authorized

1. **C1 — Preserve integrated M6a/M6b/M6c review baseline.** Include exactly
   `candidate-C1-integrated-M6-review-baseline` in the path list: modified tracked
   code/docs/tests plus new region/garage art (masters/manifests/prompts/exports),
   data, core/world/render scripts and `.uid`s, harnesses, art builders/checkers,
   matching hashes, reports/screenshots/recordings. These changes are integrated:
   splitting by historical milestone now risks nonbuilding intermediate states.
   Commit title/body should say “review baseline,” not “final visuals accepted.”
   Preserve existing approved-document edits as captured; no new revisions to them.
2. **C2 — Preserve isolated 14-day feasibility study.** Include exactly
   `candidate-C2-isolated-study`, including deterministic reports and compressed
   event evidence. Do not activate study values or rerun/rebalance it as part of
   this checkpoint. Keep separation from playable gameplay explicit.
3. **C3 — Record revised planning and VS0.1 deliverables.** Include exactly the
   three `candidate-C3-planning` paths plus this new VS0.1 package. Its exact new
   artifact list/hashes are in `artifact-checksums.json` (plus that file itself).
   Revised Vision is planning authority; original approved files remain historical.

Before any commit, compare the captured path/hash sets with the working tree;
stop on concurrent changes and refresh the proposal rather than overwriting them.
Use an explicit approved path list. Do not use blanket `git add -A`, stage caches,
or bundle unknown deletions. Commit identity/message and final include/exclude scope
need user approval. This plan does not authorize a write to `.git`.

## Items to investigate or explicitly exclude

- Four missing tracked captures: `docs/screenshots/rescue/degrading.png`,
  `offline.png`, `rescued.png`, `transmitting.png`. Their HEAD hashes are recorded.
  Determine whether deletion was intentional and which historical links need
  repair. Do not restore/recreate them or silently commit their removal. A C1–C3
  checkpoint can preserve the code but **will not make the tree clean** while these
  remain unresolved. Do not claim a complete historical screenshot archive.
- Historical prose cites alternate hashes such as `29c0b79`, `4cd4ce8` and
  `2690c74`; observed checkout lineage uses M5a `6f7e0e0`, M4a `899d9af`, M4b
  `602c1cc`/`c0ae6d2`. Keep the actual current objects; investigate provenance before
  changing historical prose. No history rewrite or inferred cherry-pick mapping.
- Exclude all `exclude-generated-cache` paths and subsequently generated `.godot`,
  `bin`, `obj`, `__pycache__` data. They are captured for completeness, not proposed
  source-control content. Game `.uid`, import recipe and source PNG files are **not**
  interchangeable with cache; exact path lists prevent accidental exclusion.
- No system SDK/editor, `/tmp` user data/logs, secrets, `.git` internals or machine
  configuration are proposed for commit. Reproduction notes identify tool versions;
  the temporary Godot location is not a portable installation promise.

## Current executable baseline

Default compact refuge remains default. Opt-in `mountain-region`,
`garage-expedition` and `sanctuary-hall-review` remain separate. Seven-day terminal
rescue, four initial/max fifth plot, current recipes/Food/Fuel/hunger values remain
unchanged. The new short-scenario candidate exists only on paper/in a worksheet.

Fresh selected checks, environment and limitations are in
[VALIDATION.md](VALIDATION.md). Full old 23-process/M6 captures are historical and
were not rerun/recreated wholesale. No new human visual acceptance is inferred.
