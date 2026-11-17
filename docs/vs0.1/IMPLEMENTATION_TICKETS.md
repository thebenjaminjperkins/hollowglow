# VS0.1 — Bounded VS1/VS2 implementation tickets

2026-11-17. **Not started. Approval required for each ticket.** Shared design input:
[scenario](SCENARIO_CONTRACT.md), [minimal contracts](ARCHITECTURE_CONTRACTS.md).
The approved C2 resolution supersedes the broader VS3 prerequisites in the imported
Alignment Brief: minimal identity/access/navigation before combat, substantial
generation variation after the first connected combat/recovery demonstration.

## VS1 — One whole-scene presentation comparison

**Objective:** establish reusable rendering/art rules in the existing hall,
threshold/road and garage. Whole scenes in motion must feel colorful, coherent,
grounded and readable. Technical pass counts are supporting evidence.

**Bounded scope:** one existing baseline and one candidate scene assembly; one
local-observer/region adapter; terrain/wall/prop sorting/grounding, conservative
natural fog presentation, interior/day/night illumination and occlusion; compact
HUD at supported resolutions. Reuse useful detailed PNGs, character, spritesheets,
footprints and manifests. Compare retained shader versus separated compositing
through a small technical experiment only as necessary to choose the single
candidate. No third pipeline, full-world art replacement or new location library.

**Asset budget:** existing fuel cabinet, compressor, wall corner and their immediate
composition are the bounded art subjects, with restrained greenery/Bloom accents.
Review the technical candidate in motion before producing replacement art. Existing
material/palette constraints may be challenged through documented comparison;
native density/camera changes require explicit approval, not accidental scaling.

**Interfaces/dependencies:** use the VS0.1 observer/region contract; no requirement
for connected networking to view the benchmark. Align the adapter with VS2 before
either hardens. Do not make VS2 wait for a broad artwork rollout. Hall geometry,
body footprints, movement, ranges, resources and default profile remain unchanged
unless a specific correction is separately justified and approved.

**Implementation units:** (1) observer-driven render registration and capture case
list; (2) one candidate fixing fog/joins/grounding/light composition with current
art; stop for technical/whole-scene review; (3) only if authorized, the bounded
beautiful-decay art comparison. No automatic move from (2) to asset replacement.

**Automated acceptance:** matched seed/camera/time gameplay-scale captures; character
passes in front/behind props/walls; conservative hidden-object state masking; light
does not reveal geography; bidirectional wall/opening lamp probes; six existing
window/aspect cases; legacy core/runtime/collision/configuration checks relevant
to touched systems. Measure draw/material/light cost before promising performance.

**Human acceptance:** moving views of hall, entrance, cabinet/van/compressor and
garage corners in day/interior/night. No clipping, floating effect, hard square
black mask accepted merely by a numeric probe. Warm inhabited home, surviving paint,
varied greenery and readable interactable states, quiet areas and subtle asynchronous
movement. Record remaining defects and get owner acceptance of the whole-scene
comparison; no claims from enlarged assets alone.

**Risks:** raised art/ground-mask disagreement, palette or baked shading limits,
hidden-object leaks, excessive per-sprite costs and large presentation refactor.
Stop if the bounded technical experiment cannot meet the visual contract; present
evidence and next options rather than broadening automatically.

**Exclusions/exit:** no gameplay, functional doors, combat, new scenery destinations,
resource changes or default-layout replacement. Exit after reviewed technical
comparison; bounded artwork production/replacement stays behind its explicit gate.

## VS2 — Two Linux instances, one authoritative survival session

**Objective:** one host and one direct-connect client with independent keyboard/
mouse or controller input and cameras, using the existing hall/road/garage rules.
Prove actual connected cooperation before adding combat/construction complexity.

**Bounded scope:** session/epoch and peer→actor registry; one host command dispatcher;
bootstrap snapshot/content agreement; authoritative movement/spatial checks and
state results; bounded replay handling; remote actor presentation; independent
local UI and observer. Preserve the pure C# transactions and their conservation,
slot/plot/power versions. Adapt single-controlled-actor scene assumptions only as
needed; no new networking framework or inventory rewrite.

**Interaction budget:** movement/threshold travel, chest slot operations, gathering/
garage search and held dismantling, farming, manual Fuel/power controls, hybrid
meals and existing powered crafting. All current rules need to remain functional;
new combat, construction, hotbar activation and extraction are outside this proof.
Use the original terminal rescue fixture only for regression, not a new ending.

**Dependencies/approvals before implementation:** minimal contracts are available
now. Approve initial JIP/reconnect/host-loss/client-disconnect policies, per-observer
current sight/team markers, hidden-state replication policy and test hardware/
controller availability. Direct Linux host/client and preserving both input modes
are approved requirements; particular transport/ports are routine engineering
choices unless they introduce services/dependencies. No full VS3 generation gate.

**Implementation units:** (1) host/client bootstrap and movement on existing geometry;
(2) dispatch existing mutations with peer/spatial/version validation and local UI;
(3) split-party/retry/conflict/new-run proof. Stop at the first two-human connected
checkpoint; report latency/feel and correctness before adding more features.

**Automated acceptance:** two separate processes and explicit per-actor logging;
one clock advances, crop/meal/Fuel continuity while UI open; simultaneous last-stack
transfer and same-plot/garage-stage races; different plots operate independently;
manual power rejection and exact resource conservation; invalid region/range/peer
ownership rejected; duplicate/stale/previous-epoch commands cannot award twice;
client bootstrap matches content/state, depleted objects persist across travel;
hall/surface split occupancy does not reset the other actor's region/time. Run
relevant existing regressions. Exercise delay/reorder/retry paths and report limits.

**Human acceptance:** two Linux instances, preferably separate desktops, two humans
can join, walk, search, transfer, plant/harvest and allocate power without sharing
input, camera, menu pause or invulnerability. Test keyboard/mouse and a real
controller for local focus/selection; report unavailable hardware instead of
claiming synthetic events prove feel. Measure movement/interaction latency on the
tested link; agree acceptable feel from evidence, not an invented network budget.

**Risks:** authority spoofing, local-player assumptions, global revision contention,
snapshot races, hidden-information exposure, client responsiveness and native engine
exit/resource lifetime errors. Report bootstrap/conflict logs and clean shutdown.

**Exclusions/exit:** no public matchmaking/NAT service, host migration, deployment,
general streaming system, optional PvP or gameplay rebalance. Join/reconnect only
if explicitly selected. Completed two-process and two-human evidence plus owner
review required; model races and six simulated actor bodies are insufficient.

## After these tickets

Proceed to minimal spatial updates, paired doors/one investment and first connected
Bloom combat/recovery only under subsequent approved scopes. Carry the material-
access blocker and unresolved C4 decisions forward. Substantial procedural variety
and independent region content cannot delay that first encounter by reinstating
the superseded full VS3 exit gate. No ticket here begins automatically after VS0.1.
