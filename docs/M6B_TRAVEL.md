# M6b — measured garage expeditions

Measured in Godot 4.4.1 .NET at fixed 60 Hz using actual input, physics collision
and clock elapsed time. Normal movement, camera coverage and interaction distances
are unchanged. Measurements begin beside the communal chest and end after explicit
deposit back there. No teleport, debug time skip or artificial handling delay is
used. The path is an efficient known route; inspection and UI inputs run at test
speed and do not represent human decision time.

| Activity | Seed 1701 dismantle | Seed 499 dismantle | Seed 8 dismantle | Seed 1701 preserve |
|---|---:|---:|---:|---:|
| Walk chest → threshold → garage | 36.10s | 36.53s | 35.07s | 36.10s |
| Circulate within site | 16.92s | 16.92s | 16.92s | 16.92s |
| Search, split, collect, inspect | 0.77s | 0.77s | 0.77s | 0.57s |
| Completed dismantling, including input frames | 3.35s | 3.35s | 3.35s | 0.85s |
| Cancelled initial van removal | 0.28s | 0.28s | 0.28s | — |
| Return to communal chest | 35.07s | 35.53s | 34.02s | 35.07s |
| Deposit | 0.08s | 0.08s | 0.08s | 0.08s |
| **Total** | **92.57s** | **93.47s** | **90.48s** | **89.58s** |
| Of six-minute daylight | 25.7% | 26.0% | 25.1% | 24.9% |
| Scrap recovered | 14 | 14 | 14 | 10 |
| Food recovered | 4 | 2 | 3 | 4 |
| Fuel recovered | 4 | 4 | 3 | 4 |

Each route searches all four containers, tests partial Food collection, acquires
the tool, dismantles the compressor, visits/inspects the van and deposits all loot
plus the tool. Dismantling also tests releasing the first hold without damaging
the van, reinspecting and completing both stages. Preservation leaves all four
van Scrap untouched. The seed-1701 maximum loot fits the original six slots:
Food6, Fuel4, Scrap10+4 and tool1 occupy five slots; carrying other supplies can
force partial collection or a second trip. Inventory capacity is not bypassed.

**Finding:** the target of an engaging 2–4-minute expedition is not demonstrated by
these automated timings. They establish a roughly 90-second efficient lower-bound
route. Reading, discovery, choosing what to carry and discussing the van may extend
a first human visit, but that has not been measured. The slice deliberately avoids
lengthening interactions or roads to force a target. The van currently has no
functional restoration payoff, so its long-term tradeoff also cannot be balanced.

Reproduce with `GarageSmoke.tscn`, as described in the [guide](M6B_GARAGE.md).
Raw clock-based samples: [1701 dismantle](measurements/garage/travel-1701-dismantle.json),
[499](measurements/garage/travel-499-dismantle.json),
[8](measurements/garage/travel-8-dismantle.json),
[1701 preserve](measurements/garage/travel-1701-preserve.json).
The separate screenshot mode uses review positioning and is excluded from these
measurements. No claim of multiplayer balance follows from these tests.
