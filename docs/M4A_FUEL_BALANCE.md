# M4a seven-day Fuel balance

Reproduce: `dotnet run --project tests/Hollowlight.Core.Tests.csproj -- --power-balance docs/M4A_FUEL_BALANCE.md`

Uses actual seeded near/far Fuel caches, authoritative gathering/storage/refueling, the shared clock and exact generator credits. Two shared starter Fuel; empty five-Fuel tank; four-PU capacity. No player-count scaling. This isolates power logistics rather than resimulating Food or farming; existing food ledgers remain applicable without changing their values.

A scavenger returns the near cache on Day 2 at 03:00 and the far cache on Day 4 at 03:00. Fuel is gathered into personal inventory, deposited in communal storage, explicitly withdrawn by the operator and manually loaded. Whole units that cannot fit stay in inventory and are deposited. No automatic refueling is simulated. Travel schedules are conservative fixture assumptions on the compact map, not measured group playtests.

Two policies: **scheduled** keeps the beacon on, runs the workbench for the first minute of each day, and lighting during the three-minute Night (no overlap with workbench). **full** runs beacon + workbench continuously; lighting stays off. Generator burn is 0.2 + 0.2 × supplied PU per complete ten-minute cycle. Scheduled costs 0.7/day = 4.9/week; full costs 1/day = 7/week.

## Seed 8, surface Fuel 5, scheduled load

### 2 players

| Completed day | Burn today | Burn to date | Tank | Stored/carried Fuel | Unpicked Fuel | Service |
|---:|---:|---:|---:|---:|---:|:---|
| 1 | 0.7 | 0.7 | 1.3 | 0 | 5 | Running |
| 2 | 0.7 | 1.4 | 2.6 | 0 | 3 | Running |
| 3 | 0.7 | 2.1 | 1.9 | 0 | 3 | Running |
| 4 | 0.7 | 2.8 | 4.2 | 0 | 0 | Running |
| 5 | 0.7 | 3.5 | 3.5 | 0 | 0 | Running |
| 6 | 0.7 | 4.2 | 2.8 | 0 | 0 | Running |
| 7 | 0.7 | 4.9 | 2.1 | 0 | 0 | Running |

### 4 players

| Completed day | Burn today | Burn to date | Tank | Stored/carried Fuel | Unpicked Fuel | Service |
|---:|---:|---:|---:|---:|---:|:---|
| 1 | 0.7 | 0.7 | 1.3 | 0 | 5 | Running |
| 2 | 0.7 | 1.4 | 2.6 | 0 | 3 | Running |
| 3 | 0.7 | 2.1 | 1.9 | 0 | 3 | Running |
| 4 | 0.7 | 2.8 | 4.2 | 0 | 0 | Running |
| 5 | 0.7 | 3.5 | 3.5 | 0 | 0 | Running |
| 6 | 0.7 | 4.2 | 2.8 | 0 | 0 | Running |
| 7 | 0.7 | 4.9 | 2.1 | 0 | 0 | Running |

### 6 players

| Completed day | Burn today | Burn to date | Tank | Stored/carried Fuel | Unpicked Fuel | Service |
|---:|---:|---:|---:|---:|---:|:---|
| 1 | 0.7 | 0.7 | 1.3 | 0 | 5 | Running |
| 2 | 0.7 | 1.4 | 2.6 | 0 | 3 | Running |
| 3 | 0.7 | 2.1 | 1.9 | 0 | 3 | Running |
| 4 | 0.7 | 2.8 | 4.2 | 0 | 0 | Running |
| 5 | 0.7 | 3.5 | 3.5 | 0 | 0 | Running |
| 6 | 0.7 | 4.2 | 2.8 | 0 | 0 | Running |
| 7 | 0.7 | 4.9 | 2.1 | 0 | 0 | Running |

## Seed 8, surface Fuel 5, continuous full load

### 2 players

| Completed day | Burn today | Burn to date | Tank | Stored/carried Fuel | Unpicked Fuel | Service |
|---:|---:|---:|---:|---:|---:|:---|
| 1 | 1.0 | 1.0 | 1.0 | 0 | 5 | Running |
| 2 | 1.0 | 2.0 | 2.0 | 0 | 3 | Running |
| 3 | 1.0 | 3.0 | 1.0 | 0 | 3 | Running |
| 4 | 1.0 | 4.0 | 3.0 | 0 | 0 | Running |
| 5 | 1.0 | 5.0 | 2.0 | 0 | 0 | Running |
| 6 | 1.0 | 6.0 | 1.0 | 0 | 0 | Running |
| 7 | 1.0 | 7.0 | 0.0 | 0 | 0 | Empty at final boundary |

### 4 players

| Completed day | Burn today | Burn to date | Tank | Stored/carried Fuel | Unpicked Fuel | Service |
|---:|---:|---:|---:|---:|---:|:---|
| 1 | 1.0 | 1.0 | 1.0 | 0 | 5 | Running |
| 2 | 1.0 | 2.0 | 2.0 | 0 | 3 | Running |
| 3 | 1.0 | 3.0 | 1.0 | 0 | 3 | Running |
| 4 | 1.0 | 4.0 | 3.0 | 0 | 0 | Running |
| 5 | 1.0 | 5.0 | 2.0 | 0 | 0 | Running |
| 6 | 1.0 | 6.0 | 1.0 | 0 | 0 | Running |
| 7 | 1.0 | 7.0 | 0.0 | 0 | 0 | Empty at final boundary |

### 6 players

| Completed day | Burn today | Burn to date | Tank | Stored/carried Fuel | Unpicked Fuel | Service |
|---:|---:|---:|---:|---:|---:|:---|
| 1 | 1.0 | 1.0 | 1.0 | 0 | 5 | Running |
| 2 | 1.0 | 2.0 | 2.0 | 0 | 3 | Running |
| 3 | 1.0 | 3.0 | 1.0 | 0 | 3 | Running |
| 4 | 1.0 | 4.0 | 3.0 | 0 | 0 | Running |
| 5 | 1.0 | 5.0 | 2.0 | 0 | 0 | Running |
| 6 | 1.0 | 6.0 | 1.0 | 0 | 0 | Running |
| 7 | 1.0 | 7.0 | 0.0 | 0 | 0 | Empty at final boundary |

## Seed 0, surface Fuel 8, scheduled load

### 2 players

| Completed day | Burn today | Burn to date | Tank | Stored/carried Fuel | Unpicked Fuel | Service |
|---:|---:|---:|---:|---:|---:|:---|
| 1 | 0.7 | 0.7 | 1.3 | 0 | 8 | Running |
| 2 | 0.7 | 1.4 | 3.6 | 0 | 5 | Running |
| 3 | 0.7 | 2.1 | 2.9 | 0 | 5 | Running |
| 4 | 0.7 | 2.8 | 4.2 | 3 | 0 | Running |
| 5 | 0.7 | 3.5 | 3.5 | 3 | 0 | Running |
| 6 | 0.7 | 4.2 | 2.8 | 3 | 0 | Running |
| 7 | 0.7 | 4.9 | 2.1 | 3 | 0 | Running |

### 4 players

| Completed day | Burn today | Burn to date | Tank | Stored/carried Fuel | Unpicked Fuel | Service |
|---:|---:|---:|---:|---:|---:|:---|
| 1 | 0.7 | 0.7 | 1.3 | 0 | 8 | Running |
| 2 | 0.7 | 1.4 | 3.6 | 0 | 5 | Running |
| 3 | 0.7 | 2.1 | 2.9 | 0 | 5 | Running |
| 4 | 0.7 | 2.8 | 4.2 | 3 | 0 | Running |
| 5 | 0.7 | 3.5 | 3.5 | 3 | 0 | Running |
| 6 | 0.7 | 4.2 | 2.8 | 3 | 0 | Running |
| 7 | 0.7 | 4.9 | 2.1 | 3 | 0 | Running |

### 6 players

| Completed day | Burn today | Burn to date | Tank | Stored/carried Fuel | Unpicked Fuel | Service |
|---:|---:|---:|---:|---:|---:|:---|
| 1 | 0.7 | 0.7 | 1.3 | 0 | 8 | Running |
| 2 | 0.7 | 1.4 | 3.6 | 0 | 5 | Running |
| 3 | 0.7 | 2.1 | 2.9 | 0 | 5 | Running |
| 4 | 0.7 | 2.8 | 4.2 | 3 | 0 | Running |
| 5 | 0.7 | 3.5 | 3.5 | 3 | 0 | Running |
| 6 | 0.7 | 4.2 | 2.8 | 3 | 0 | Running |
| 7 | 0.7 | 4.9 | 2.1 | 3 | 0 | Running |

## Seed 0, surface Fuel 8, continuous full load

### 2 players

| Completed day | Burn today | Burn to date | Tank | Stored/carried Fuel | Unpicked Fuel | Service |
|---:|---:|---:|---:|---:|---:|:---|
| 1 | 1.0 | 1.0 | 1.0 | 0 | 8 | Running |
| 2 | 1.0 | 2.0 | 3.0 | 0 | 5 | Running |
| 3 | 1.0 | 3.0 | 2.0 | 0 | 5 | Running |
| 4 | 1.0 | 4.0 | 4.0 | 2 | 0 | Running |
| 5 | 1.0 | 5.0 | 3.0 | 2 | 0 | Running |
| 6 | 1.0 | 6.0 | 2.0 | 2 | 0 | Running |
| 7 | 1.0 | 7.0 | 1.0 | 2 | 0 | Running |

### 4 players

| Completed day | Burn today | Burn to date | Tank | Stored/carried Fuel | Unpicked Fuel | Service |
|---:|---:|---:|---:|---:|---:|:---|
| 1 | 1.0 | 1.0 | 1.0 | 0 | 8 | Running |
| 2 | 1.0 | 2.0 | 3.0 | 0 | 5 | Running |
| 3 | 1.0 | 3.0 | 2.0 | 0 | 5 | Running |
| 4 | 1.0 | 4.0 | 4.0 | 2 | 0 | Running |
| 5 | 1.0 | 5.0 | 3.0 | 2 | 0 | Running |
| 6 | 1.0 | 6.0 | 2.0 | 2 | 0 | Running |
| 7 | 1.0 | 7.0 | 1.0 | 2 | 0 | Running |

### 6 players

| Completed day | Burn today | Burn to date | Tank | Stored/carried Fuel | Unpicked Fuel | Service |
|---:|---:|---:|---:|---:|---:|:---|
| 1 | 1.0 | 1.0 | 1.0 | 0 | 8 | Running |
| 2 | 1.0 | 2.0 | 3.0 | 0 | 5 | Running |
| 3 | 1.0 | 3.0 | 2.0 | 0 | 5 | Running |
| 4 | 1.0 | 4.0 | 4.0 | 2 | 0 | Running |
| 5 | 1.0 | 5.0 | 3.0 | 2 | 0 | Running |
| 6 | 1.0 | 6.0 | 2.0 | 2 | 0 | Running |
| 7 | 1.0 | 7.0 | 1.0 | 2 | 0 | Running |

## Interpretation

Shared generation is independent of player count. Both minimum and maximum cache worlds meet these seven-day schedules with the planned return/refill times. The poorest world ends full-load service exactly empty at Day 8 dawn, with no buffer for delayed scavenging. Scheduled service leaves 2.1–5.1 Fuel across world extremes; full service leaves 0–3. Food still costs 14/28/42 units for 2/4/6 players independently.

Starting Fuel alone supports only two full-load days (or about 2.86 scheduled days). Surface collection is necessary for a powered week. Near Fuel extends the window, while full service requires the far cache. Disabling consumers saves Fuel; stopping the generator saves idle burn as well. There is no battery, second generator, crop-power requirement, crafting production or rescue-progress simulation in M4a.
