# M6a measured expedition and chore timings

Measured in Godot 4.4.1 .NET at fixed 60 Hz using the real player input/movement and collision implementation, ordinary 80-unit/s speed, 384×216-world-unit gameplay coverage, and authoritative clock. No teleporting or debug time skips during measurements. Pathfinding supplies test input only; normal play remains direct movement without an autopilot.

Each expedition starts at the communal chest, walks through the threshold, reaches a finite source, gathers with E, returns through the threshold, opens storage and explicitly deposits the collected stock. The script waits two seconds at both the destination and storage (four seconds of handling per trip). Gathering itself remains instantaneous. These are efficient known-route timings, not discovery/search time, human menu timing, or network latency. Hunger is serviced with carried supplies; no movement penalty occurs during the recorded trips.

Daylight lasts **360 seconds**. Every measured individual trip fits comfortably; a sequence of several trips still consumes a substantial fraction of daylight. Surface source quantities are not multiplied by party size.

## Seed 1701

| Source / destination | Outbound from chest | Round trip + gathering/deposit | Daylight |
|---|---:|---:|---:|
| radio-shell / hut | 11.7s | 27.2s | 7.5% |
| tool-case / hollow | 17.6s | 38.6s | 10.7% |
| machine-housing / yard | 33.6s | 71.1s | 19.7% |
| food-near-wild_plant-0 / entrance | 5.7s | 14.8s | 4.1% |
| food-near-wild_plant-1 / hut | 8.8s | 21.7s | 6.0% |
| food-near-container-0 / hut | 10.4s | 24.6s | 6.8% |
| food-far-wild_plant-0 / hollow | 15.9s | 35.2s | 9.8% |
| food-far-container-0 / hamlet | 17.7s | 39.5s | 11.0% |
| food-far-container-1 / yard | 35.8s | 75.5s | 21.0% |
| fuel-near-cache-0 / hut | 8.9s | 21.7s | 6.0% |
| fuel-far-cache-0 / yard | 33.5s | 70.8s | 19.7% |

| Sanctuary duty | One-way walk from chest | Round trip + two interactions |
|---|---:|---:|
| workbench | 2.6s | 9.3s |
| generator | 4.5s | 13.1s |
| garden | 2.4s | 8.9s |

Three near-Food trips together: **61.0s**; three far-Food trips: **150.2s**; all three material-node trips: **136.8s**. Each trip returns to storage, so combining nearby stops can save walking.

## Seed 499

| Source / destination | Outbound from chest | Round trip + gathering/deposit | Daylight |
|---|---:|---:|---:|
| radio-shell / hut | 12.0s | 27.9s | 7.8% |
| tool-case / hollow | 15.2s | 33.8s | 9.4% |
| machine-housing / yard | 36.5s | 76.9s | 21.4% |
| food-near-wild_plant-0 / entrance | 4.7s | 13.4s | 3.7% |
| food-near-wild_plant-1 / hut | 13.0s | 29.9s | 8.3% |
| food-near-container-0 / hut | 12.9s | 29.7s | 8.2% |
| food-far-wild_plant-0 / hollow | 12.7s | 28.9s | 8.0% |
| food-far-container-0 / hamlet | 19.1s | 42.3s | 11.7% |
| food-far-container-1 / yard | 37.2s | 78.2s | 21.7% |
| fuel-near-cache-0 / hut | 13.8s | 31.5s | 8.7% |
| fuel-far-cache-0 / yard | 33.8s | 71.4s | 19.8% |

| Sanctuary duty | One-way walk from chest | Round trip + two interactions |
|---|---:|---:|
| workbench | 2.6s | 9.3s |
| generator | 4.5s | 13.1s |
| garden | 2.4s | 8.9s |

Three near-Food trips together: **73.0s**; three far-Food trips: **149.4s**; all three material-node trips: **138.6s**. Each trip returns to storage, so combining nearby stops can save walking.

## Seed 8

| Source / destination | Outbound from chest | Round trip + gathering/deposit | Daylight |
|---|---:|---:|---:|
| radio-shell / hut | 11.8s | 27.4s | 7.6% |
| tool-case / hollow | 10.9s | 25.3s | 7.0% |
| machine-housing / yard | 32.5s | 68.7s | 19.1% |
| food-near-wild_plant-0 / entrance | 4.7s | 13.4s | 3.7% |
| food-near-wild_plant-1 / hut | 9.2s | 22.3s | 6.2% |
| food-near-container-0 / hut | 9.8s | 23.5s | 6.5% |
| food-far-wild_plant-0 / hollow | 11.3s | 26.1s | 7.3% |
| food-far-container-0 / hamlet | 19.9s | 43.9s | 12.2% |
| food-far-container-1 / yard | 34.7s | 73.1s | 20.3% |
| fuel-near-cache-0 / hut | 9.2s | 22.3s | 6.2% |
| fuel-far-cache-0 / yard | 35.4s | 74.5s | 20.7% |

| Sanctuary duty | One-way walk from chest | Round trip + two interactions |
|---|---:|---:|
| workbench | 2.6s | 9.3s |
| generator | 4.5s | 13.1s |
| garden | 2.4s | 8.9s |

Three near-Food trips together: **59.1s**; three far-Food trips: **143.1s**; all three material-node trips: **121.4s**. Each trip returns to storage, so combining nearby stops can save walking.

## Interpretation

Across the three seeds, measured round trips range from **13.4 to 78.2 seconds**, using at most **21.7%** of one daylight period. The provisional dimensions therefore produce a compact local region. Far journeys are shorter than the earlier 45–75-second *one-way* planning estimate; M6a does not add detours, movement penalties or larger dimensions to force that estimate. This should be judged during review. Future exposure and carrying constraints are separate approvals.

Raw reproducible measurements: [seed 1701](measurements/region/travel-1701.json), [seed 499](measurements/region/travel-499.json), [seed 8](measurements/region/travel-8.json). The [daily economy replay](M6A_ECONOMY.md) uses these actual times, rounded upward.
