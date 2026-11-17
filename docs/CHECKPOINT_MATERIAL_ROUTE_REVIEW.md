# Checkpoint and material-route review

2026-11-17. VS0.1 is accepted as completed planning. This report closes the authorized
checkpoint/material-route review; it does not start VS1 or VS2 or extend planning.

## Checkpoint status

The existing three separate commits already implement C1/C2/C3:

| Group | Commit | Status |
|---|---|---|
| C1 functional M6 review baseline | `8244aad` | 281 explicit paths; **not** environmental graphics acceptance |
| C2 isolated economy feasibility study | `5925fde` | 99 explicit paths; candidate economy stays isolated |
| C3 revised drafts and VS0.1 package | `d777be0` | 33 explicit paths; no new playable configuration |

The tree was clean at the start of this review. Commit file sets were checked
against the [checkpoint lists](vs0.1/checkpoint-paths.json) and captured package.
The four restored `docs/screenshots/rescue/` images match original `6f7e0e0` Git
objects byte for byte. They were not fabricated; no restoration overwrite was
needed now because their correct originals were already present. Relocated copies
remain. Existing history has not been rewritten. See the earlier
[resolution record](COMMIT_CHECKPOINT_2026-11-17.md) for alternate historical hashes.

The previous push remains blocked by HTTPS credentials/SSH-account write access;
local commits are preserved. No remote success is implied by this checkpoint.

## Actual hall routes

New review-only harness: [MaterialRouteReview.cs](../tools/MaterialRouteReview.cs),
run on the existing `sanctuary-hall-review` world. Each case starts at the communal
chest and uses normal input, physics, current clock, real threshold crossings,
finite sources, authority checks and explicit deposit. No debug teleport/time skip,
free stock, moved resource, recipe edit or accelerated playable profile.

**Seed1701 single-source round trips**, including gathering and deposit:

| Existing destination | Resource | Actual round trip |
|---|---|---:|
| Hollow (`tool-case`) | Organic8 | **35.10s** |
| Hut (`radio-shell`) | Scrap12 | **23.65s** |

**Combined itinerary:** hall → road/garage → hollow Organic → hut Scrap → hall →
withdraw ingredients → manually load/start/allocate power → craft bed → install.
Garage search includes Food/Fuel/parts/crate, personal tool pickup and compressor
dismantling. The van is inspected and **preserved throughout**.

| Seed | Expedition through deposit | Through fifth-plot installation | Peak used slots |
|---:|---:|---:|---:|
| 1701 | **99.37s** | **106.85s** | 6 / 6 |
| 499 | **107.03s** | **114.53s** | 6 / 6 |
| 8 | **94.22s** | **101.70s** | 6 / 6 |

These are efficient known routes with test-speed menus and current immediate
crafting/installation. They are not human discovery, networked coordination,
future blueprint delivery/assembly, combat or evacuation measurements. Timing uses
elapsed simulation seconds at fixed60 Hz; it is not rendering performance evidence.

Seed1701 measured phases: hall-to-surface4.28s, surface-to-garage32.12s, garage
circulation/search/work15.82s, garage-to-hollow25.48s, hollow-to-hut9.95s,
hut-to-surface-threshold7.77s and inner-threshold-to-chest3.73s. Gathering/deposit
inputs take the remaining fraction; withdrawal/power/crafting/installation add7.48s.
Full legs and raw results: [measurement directory](measurements/material-route-review/).

## Feasibility and carrying consequences

All three combined cases recover **Scrap20 and Organic8** without dismantling the
van. Six slots fill with Food, Fuel, tool, Organic and two Scrap stacks. The hut
therefore yields only10 of its12 Scrap; **two remain at the source**. Finite
partial collection and carrying limits are enforced, not bypassed for the test.

At home the actor deposits, explicitly withdraws **Scrap12 + Organic4** and one
existing Fuel, starts the generator, enables beacon2 PU + workbench2 PU and crafts
at the powered bench. The existing fifth plot installs successfully. Shared
leftovers are **Scrap8 + Organic4**. Scrap conservation:12 consumed +8 stored +2
uncollected hut +4 preserved van = original26. Organic4 consumed +4 stored =8
recovered; no crop yield was required to finance construction.

The four starter seeds remain unchanged. The harness validates installation, not
a full five-plot planting cycle: normal play would propagate a Potato seed using
the existing seed1 + Organic1 → seeds2 recipe to provide an extra seed after
planting the initial four. The remaining Organic can fund that recipe; no free
fifth seed is assumed. Crop growth/yields and all existing recipes stay unchanged.

**Finding:** the approved existing-source detours resolve the material blocker.
The combined route plus existing powered upgrade fits under two efficient minutes
in three seeds; no evidence here justifies more supplies, relocation or lower costs.
The detours do not create excessive travel in this measured case. Meaningful pack
pressure and preserving-versus-dismantling remain available decisions; a human pair
still needs to judge discovery, coordination and enjoyment with the first Bloom
encounter. This review establishes feasibility, **not proven engagement**.

## Isolated timing and sequence decisions

The owner approved the **240s cycle (144 Day /24 Dusk /72 Night)** and **three
accumulated signal days** as provisional isolated slice-testing values. They remain
in the worksheet/specification only; no playable profile is registered and the
normal600s cycle/seven-day beacon, resource stocks, production and recipes remain
unchanged. Fifteen–twenty minutes is a human-test goal, not an automatic deadline.

For comparison only, leaving at01:00 places the measured installed upgrade around
02:42–02:55. The candidate first night begins02:48, so human delays/combat can extend
the return into darkness. Do not slow travel or compress interactions to force the
schedule. A three-minute expedition allowance is plausible from these lower bounds
but still needs human testing. Future assembly time remains a separate decision.

The preferred sequence is now planning complete → checkpoint complete → **VS1
visual foundation** → VS2 connected Linux host/client → first connected Bloom
encounter/recovery. Only minimum region identity/navigation/spatial authority must
precede combat; substantial generation variety is deferred.

VS1's final acceptance is a cohesive, attractive sanctuary **and connected outdoor
environment** in motion. The fuel-cabinet/compressor/wall-corner trial is an
intermediate checkpoint, not final acceptance. VS2 starts with both players present,
direct Linux host/client, keyboard/mouse and controller requirements, no migration
or join-in-progress. A client disconnect is **not an in-game death**. Reconnection
and host-loss handling still need their bounded implementation decision.

No successful normal run must include death. Dedicated recovery tests cover lost
equipment and limited emergency resurrection. True all-dead failure and departure-
based resolution remain; simultaneous arbitration, all-downed and disconnected
survivor eligibility remain explicitly unresolved until dependent milestones.

## Validation and reproduction

Fresh main-project build: zero warnings/errors. Five engine cases passed: the two
seed1701 single-source trips and combined routes for1701/499/8. Each combined case
checks original capacities/target/plots, real source access/depletion, partial
collection, tool ownership, intact van, power-at-craft, exact ingredient use and
fifth-plot installation. Headless editor import also passed for the new test tool.
Existing runtime/data/artwork remain unchanged; no new graphical acceptance claim.

With the installed Godot .NET executable/environment (see
[VS0.1 commands](vs0.1/VALIDATION.md)):

```sh
dotnet build Hollowlight.csproj --no-restore
"$HOLLOWLIGHT_ENGINE" --headless --fixed-fps 60 --path . res://tools/MaterialRouteReview.tscn -- --world-seed=1701 --material-route=organic
"$HOLLOWLIGHT_ENGINE" --headless --fixed-fps 60 --path . res://tools/MaterialRouteReview.tscn -- --world-seed=1701 --material-route=scrap
"$HOLLOWLIGHT_ENGINE" --headless --fixed-fps 60 --path . res://tools/MaterialRouteReview.tscn -- --world-seed=1701 --material-route=garage-detours
```

Repeat the final command with499 or8 for the other seeded routes. Raw JSON writes
to `user://material-routes`; archived logs/JSON, exact commands and preservation/
provenance checks are in the measurement directory. Original failure/out-of-scope
conditions cause nonzero exit instead of adding material or bypassing a blocker.

Stop here. The next authorization is the **first bounded VS1 implementation
ticket**, with its whole-environment acceptance and intermediate review gates.
Do not start VS1/VS2, repeat broad planning or rebalance the economy automatically.
