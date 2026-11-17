# VS0.1 — Minimum authority, observer and region contracts

2026-11-17. Interface specifications only; no classes, transport, profile or runtime
refactor implemented. Names describe responsibilities, not a new framework/API.
Use existing C# transactions and Godot spatial queries behind narrow adapters.

## 1. Identity and lifetime

| Identity | Owner and minimum meaning | Required rejection |
|---|---|---|
| Session/epoch | Host-issued run identity; new run replaces it | Requests/snapshots from a previous run |
| Actor ID | Session actor registry; host binds peer to actor | Peer cannot choose another player's actor ID |
| Region ID | Stable generated/authored instance key within session | Target/actor in different regions cannot interact locally |
| Object/plot/container ID | Stable within region; network key is region+object | Unknown, depleted, replaced or unavailable targets |
| Command ID | Unique per peer/epoch sequence with bounded retained results | Retry cannot consume/award twice; stale revision still checked for new commands |
| State version | Session plus relevant inventory/plot/power/object revision | Stale snapshot gestures cannot act on replacement contents |

Existing Guid session/revisions and object names are reusable. Do not require all
systems to share a single incrementing revision or prematurely replace current
versions. Measure contention in the two-player proof. Region ID cannot depend on
whatever scene is currently attached to the local player's camera.

## 2. Host command and result boundary

Conceptual envelope: session/epoch, command ID, requested operation, target region/
object or slots, quantity and observed state versions. Actor identity is resolved
from the authenticated connection binding, not trusted because it appears in a
payload. Local host input goes through the same action checks as client input.

Before commit, the host validates connected actor ownership and eligibility,
session/version, region, authoritative position, range/LOS/access rules, target
state, required tool/power, materials and carrying capacity. Existing operations
preflight and commit under their current state-owner lock. Rejected actions return
a status plus enough fresh state to recover UI without secretly retrying a different
intent. Movement intent must be bounded; no client-supplied teleport is authoritative.

Result contract: command correlation, applied/rejected status, affected revisions
and authoritative deltas or a replacement snapshot. A repeated ID returns its
original result or an explicit expired-retry response without mutation; retention
limits are implementation details to bound in VS2. Snapshots/events never award
items merely because the client receives them twice.

Examples to demonstrate in VS2: one client wins a final chest stack; two actors
cannot harvest one plot twice; an expired dismantle ticket cannot strip another
stage; a remote request cannot refuel; a replay from the previous run cannot mutate
the new one. Seed/content agreement is needed at bootstrap; seed alone does not
synchronize depletion or discoveries. No combat payload/stat schema is fixed here.

## 3. Simulation and local observer

One host advances session time exactly once. Agriculture, meals, fuel and signal
continue when either player opens a menu or leaves the sanctuary. Client display
interpolation does not advance authoritative production or resource burn. Updates
crossing fuel exhaustion/day boundaries preserve current exact accounting.

The local observer context supplies local actor ID, actor region, camera and input
device, plus read-only versions of explored geography, current sight and dynamic
state authorized for presentation. Remove lookups of a globally named `Player`
from rendering/authority code as scoped by VS1/VS2; keep that node in legacy scenes
if fixtures need it. One player's UI blocks only that actor's movement/interaction,
not world time or the other player's input, and grants no invulnerability.

Keep four concepts distinct:

1. **Shared discovered geography:** session memory, retained when observers depart.
2. **Current sight:** per-observer host-derived occlusion; current model is the
   proposed initial behavior. Shared *live* team sight remains an approval choice.
3. **Illumination:** read-only local rendering from time/exposure/lights/occluders;
   light cannot grant discovery or current knowledge through walls.
4. **Contact shadows:** visual grounding; they neither block rays nor reveal objects.

Mapped static terrain may remain readable; unseen dynamic contents, depletion,
creatures and state changes must not be revealed automatically. VS2 must explicitly
choose whether client replicas omit hidden dynamic state or merely suppress it;
do not claim rendering masks provide network secrecy. Teammate markers/last-known
objects are not silently invented to fill that policy gap.

## 4. Region and spatial authority, without a generation project

Minimum region record: ID, bounds/origin, stable objects, navigation/collision view,
opaque sight geometry and revision. For the first proof the current hall/surface
can remain in their existing coordinate atlas, with an adapter identifying the
logical region. No streaming, general solver or new region content is required.

Host region transition validates the actor and authorized exit, moves it to the
existing destination landing, updates region/camera view and cancels invalid local
interaction sessions. Actor A crossing cannot unload actor B's room or reset world
contents/time. Quantities/depletion remain session-owned, not reconstructed by a
fresh scene visit. A two-process hall/surface occupancy test suffices initially.
Independent streamed region instances and meaningful procedural variety follow
the first connected combat/recovery test; do not enforce the old full VS3 gate now.

Navigation, body collision, attack/interaction rays, sight and light occlusion have
different purposes but refer to consistent authored/generated boundaries. Future
door/construction state increments a spatial revision and updates affected views;
closed physical doors cannot remain optically transparent to ordinary enemy attacks.
VS0.1 defines that responsibility, not an implementation or new collider. VS2
only needs current static geometry/access; VS4 owns dynamic doors/placement.

## 5. Signal opportunity and terminal outcomes

Current `SharedRescue` freezes the clock and guards mutations at signal completion.
Future signal-ready must emit exactly one opportunity notification while simulation
continues. Keep signal accounting separate from extraction status and terminal run
result; do not rename the existing terminal enum and assume the freeze is gone.

Known outcomes: departure with one or more aboard terminates in full/partial rescue;
empty departure with living survivors does not; true all-dead party fails immediately
regardless of chamber stock. Recovery never duplicates lost equipment. A dedicated
recovery scenario tests this; death is not required for a successful ordinary run.

No arbitrary event precedence is defined here. Same-tick wipe/departure, all-downed
and disconnected-player eligibility stay unresolved until VS6/VS7. VS1/VS2 must not
implement ending changes or fake reconnect/death policies to hide this gap. Host-loss
session policy, JIP and reconnection proposals are in the scenario contract.

## 6. Minimum handoff tests

VS1 receives explicit observer/region inputs, preserves independently tested logical
masks, and presents correctly moving actors/assets without changing collision.
VS2 proves two independent clients/views against one host authority; explicit
inventory/farm/power/garage conflicts; one clock owner; split hall/surface residency;
new-epoch rejection and content/bootstrap consistency. Those are implementation
tickets, not networking delivered by this document. No general ECS, plugin service,
transport framework, save format or full region lifecycle is required at this gate.
