# Outstanding-work checkpoint — 2026-11-17

The owner authorized resolving all outstanding commits and pushing. This extends
the earlier VS0.1 capture-only authorization for Git operations; it does not approve
VS1/VS2 implementation, visual acceptance, new gameplay or economy changes.

The [VS0.1 baseline](vs0.1/BASELINE_CHECKPOINT.md), manifest and validation remain
historical records of the pre-commit working tree. Their recorded HEAD, untracked
status and missing-image observations are intentionally not rewritten after committing.

## Resolution

- The four missing `docs/screenshots/rescue/` PNGs have byte-identical copies in
  `docs/screenshots/region/rescue/`, verified against their original HEAD blobs.
  Restore the original paths from those copies so existing README/M4b links work.
  Retain the relocated copies as well; no source evidence is discarded.
- Preserve the existing integrated M6a/M6b/M6c review work as one checkpoint because
  code, assets, tests and evidence depend on each other. This commit does not claim
  final artistic acceptance. Default layouts and existing survival values remain.
- Preserve the accepted isolated study separately, including its reproducible
  inputs/reports. Candidate economy values remain outside playable systems.
- Preserve the owner-revised planning drafts and VS0.1 package separately. Revised
  Vision governs subsequent planning; historical approved Vision/Roadmap files keep
  their recorded state. No runtime review profile, combat or networking is added.
- Exclude generated `.godot`, `bin`, `obj` and Python caches. Explicit approved path
  groups from `docs/vs0.1/checkpoint-paths.json` determine staging; VS0.1 package and
  this resolution record join the planning group.

## Historical commit references

Current checkout history records M5a as `6f7e0e0`, M4a as `899d9af`, M4b as
`602c1cc` plus label fix `c0ae6d2`, M3b as `37fd833`, M2b as `7b72761` and M2a
as `e0a3405`. Older prose logs cite other hashes. Retain those logs as recorded
history and use the actual Git graph for checkout/ancestry; no rewrite or unverified
mapping between alternate hashes is attempted. The mismatch no longer blocks a
checkpoint of the actual file content.

## Validation basis

Fresh VS0.1 build, core tests, hall smoke, garage preservation smoke and three art
checkers passed. Before staging, all captured existing nonignored files and the
VS0.1 artifact checksums matched the validated package. The only subsequent content
changes are restoring the four exact historical PNGs, removing one extra trailing
blank line from the generated M6a economy report to pass staged whitespace checks,
and this resolution record.
Generated CSV evidence retains its checksum-verified CRLF record endings; staged
whitespace validation allows carriage returns at end of line for those records
without changing report bytes or repository-wide Git configuration.
No gameplay test rerun is required for those documentation/evidence-only changes.
Check staged whitespace, exact group membership and cache exclusions before each
commit; verify clean working tree and remote branch after a normal push.
