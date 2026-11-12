# M3b seven-day Food scavenging balance

Reproduce: `dotnet run --project tests/Hollowlight.Core.Tests.csproj -- --scavenging-balance docs/M3B_FOOD_BALANCE.md`

Uses the authoritative seeded source generator, finite gathering, personal inventories, explicit storage transfers, unchanged crop simulation and dawn meals. All worlds have six sources independent of player count. The approved explicit ranges sum to **8–17 Food**, not 9–17; no minimum-stock top-up is applied. The expected uniform total is 12.5 Food.

Farm schedules match the M3a ledger: withdraw seeds at D1 00:20; plant at 01:00/01:10/01:20/01:30; check plots each day at 03:00, 05:30, 08:30 (regular) or 03:00 and 08:30 (delayed). Harvests take ten-second plot visits, replant five seconds after collection. Farm Food is deposited; every visit is followed by group resupply one minute later. Players explicitly withdraw toward two personal Food, giving everyone the next meal before a reserve and rotating priority by day. Starting Food remains two per actor; daily requirement remains one.

A second actor scavenges the three near nodes at D1 01:00/01:10/01:20 and deposits only those gathered quantities at D1 02:00. Far nodes are collected at D3 04:00/04:10/04:20 and deposited at D3 05:00. These conservative collection/return schedules allow search and travel time on the compact map; they are reproducible scenarios, not measured multiplayer trips or simulations of future large worlds. Sources and gathered quantities come from actual generated worlds, never fabricated deposits.

Each row settles the listed day at the next dawn. **Net supply − need** includes initial Food on Day 1, plus that day's farm/scavenging deliveries minus the full group requirement. A negative flow can be covered by an existing reserve; **Unmet meals today** reports actual personal shortages. Personal/chest reserve is the positive stock left after eating. Unpicked Food is not counted as available meals. Every daily row checks exact Food conservation and accounts for every obligation.

## World seed 499: 8 Food

food-near-wild_plant-0: 1; food-near-wild_plant-1: 1; food-near-container-0: 1; food-far-wild_plant-0: 1; food-far-container-0: 2; food-far-container-1: 2.

### Regular farm care

#### 2 players

| Day | Farm today | Scavenged today | Net supply − need | Eaten today | Unmet meals today | Personal reserve | Chest reserve | Unpicked | Missed-meal scores |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---|
| 1 | 0 | 3 | +5 | 2 | 0 | 2 | 3 | 5 | 0,0 |
| 2 | 0 | 0 | -2 | 2 | 0 | 2 | 1 | 5 | 0,0 |
| 3 | 6 | 5 | +9 | 2 | 0 | 2 | 10 | 0 | 0,0 |
| 4 | 4 | 0 | +2 | 2 | 0 | 2 | 12 | 0 | 0,0 |
| 5 | 6 | 0 | +4 | 2 | 0 | 2 | 16 | 0 | 0,0 |
| 6 | 0 | 0 | -2 | 2 | 0 | 2 | 14 | 0 | 0,0 |
| 7 | 10 | 0 | +8 | 2 | 0 | 2 | 22 | 0 | 0,0 |

Harvest ledger: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 05:30 Cave Potato +3; D5 05:40 Cave Potato +3; D7 05:50 Glowcap +2; D7 06:00 Glowcap +2; D7 08:30 Cave Potato +3; D7 08:40 Cave Potato +3.

Total scavenged 8; missed meals 0; ending reserve 24.

#### 4 players

| Day | Farm today | Scavenged today | Net supply − need | Eaten today | Unmet meals today | Personal reserve | Chest reserve | Unpicked | Missed-meal scores |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---|
| 1 | 0 | 3 | +7 | 4 | 0 | 4 | 3 | 5 | 0,0,0,0 |
| 2 | 0 | 0 | -4 | 4 | 0 | 3 | 0 | 5 | 0,0,0,0 |
| 3 | 6 | 5 | +7 | 4 | 0 | 4 | 6 | 0 | 0,0,0,0 |
| 4 | 4 | 0 | 0 | 4 | 0 | 4 | 6 | 0 | 0,0,0,0 |
| 5 | 6 | 0 | +2 | 4 | 0 | 4 | 8 | 0 | 0,0,0,0 |
| 6 | 0 | 0 | -4 | 4 | 0 | 4 | 4 | 0 | 0,0,0,0 |
| 7 | 10 | 0 | +6 | 4 | 0 | 4 | 10 | 0 | 0,0,0,0 |

Harvest ledger: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 05:30 Cave Potato +3; D5 05:40 Cave Potato +3; D7 05:50 Glowcap +2; D7 06:00 Glowcap +2; D7 08:30 Cave Potato +3; D7 08:40 Cave Potato +3.

Total scavenged 8; missed meals 0; ending reserve 14.

#### 6 players

| Day | Farm today | Scavenged today | Net supply − need | Eaten today | Unmet meals today | Personal reserve | Chest reserve | Unpicked | Missed-meal scores |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---|
| 1 | 0 | 3 | +9 | 6 | 0 | 6 | 3 | 5 | 0,0,0,0,0,0 |
| 2 | 0 | 0 | -6 | 6 | 0 | 3 | 0 | 5 | 0,0,0,0,0,0 |
| 3 | 6 | 5 | +5 | 6 | 0 | 6 | 2 | 0 | 0,0,0,0,0,0 |
| 4 | 4 | 0 | -2 | 6 | 0 | 6 | 0 | 0 | 0,0,0,0,0,0 |
| 5 | 6 | 0 | 0 | 6 | 0 | 6 | 0 | 0 | 0,0,0,0,0,0 |
| 6 | 0 | 0 | -6 | 6 | 0 | 0 | 0 | 0 | 0,0,0,0,0,0 |
| 7 | 10 | 0 | +4 | 6 | 0 | 4 | 0 | 0 | 0,0,0,0,0,0 |

Harvest ledger: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 05:30 Cave Potato +3; D5 05:40 Cave Potato +3; D7 05:50 Glowcap +2; D7 06:00 Glowcap +2; D7 08:30 Cave Potato +3; D7 08:40 Cave Potato +3.

Total scavenged 8; missed meals 0; ending reserve 4.

### Delayed farm care

#### 2 players

| Day | Farm today | Scavenged today | Net supply − need | Eaten today | Unmet meals today | Personal reserve | Chest reserve | Unpicked | Missed-meal scores |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---|
| 1 | 0 | 3 | +5 | 2 | 0 | 2 | 3 | 5 | 0,0 |
| 2 | 0 | 0 | -2 | 2 | 0 | 2 | 1 | 5 | 0,0 |
| 3 | 6 | 5 | +9 | 2 | 0 | 2 | 10 | 0 | 0,0 |
| 4 | 4 | 0 | +2 | 2 | 0 | 2 | 12 | 0 | 0,0 |
| 5 | 6 | 0 | +4 | 2 | 0 | 2 | 16 | 0 | 0,0 |
| 6 | 0 | 0 | -2 | 2 | 0 | 2 | 14 | 0 | 0,0 |
| 7 | 4 | 0 | +2 | 2 | 0 | 2 | 16 | 0 | 0,0 |

Harvest ledger: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 08:30 Cave Potato +3; D5 08:40 Cave Potato +3; D7 08:50 Glowcap +2; D7 09:00 Glowcap +2.

Total scavenged 8; missed meals 0; ending reserve 18.

#### 4 players

| Day | Farm today | Scavenged today | Net supply − need | Eaten today | Unmet meals today | Personal reserve | Chest reserve | Unpicked | Missed-meal scores |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---|
| 1 | 0 | 3 | +7 | 4 | 0 | 4 | 3 | 5 | 0,0,0,0 |
| 2 | 0 | 0 | -4 | 4 | 0 | 3 | 0 | 5 | 0,0,0,0 |
| 3 | 6 | 5 | +7 | 4 | 0 | 4 | 6 | 0 | 0,0,0,0 |
| 4 | 4 | 0 | 0 | 4 | 0 | 4 | 6 | 0 | 0,0,0,0 |
| 5 | 6 | 0 | +2 | 4 | 0 | 4 | 8 | 0 | 0,0,0,0 |
| 6 | 0 | 0 | -4 | 4 | 0 | 4 | 4 | 0 | 0,0,0,0 |
| 7 | 4 | 0 | 0 | 4 | 0 | 4 | 4 | 0 | 0,0,0,0 |

Harvest ledger: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 08:30 Cave Potato +3; D5 08:40 Cave Potato +3; D7 08:50 Glowcap +2; D7 09:00 Glowcap +2.

Total scavenged 8; missed meals 0; ending reserve 8.

#### 6 players

| Day | Farm today | Scavenged today | Net supply − need | Eaten today | Unmet meals today | Personal reserve | Chest reserve | Unpicked | Missed-meal scores |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---|
| 1 | 0 | 3 | +9 | 6 | 0 | 6 | 3 | 5 | 0,0,0,0,0,0 |
| 2 | 0 | 0 | -6 | 6 | 0 | 3 | 0 | 5 | 0,0,0,0,0,0 |
| 3 | 6 | 5 | +5 | 6 | 0 | 6 | 2 | 0 | 0,0,0,0,0,0 |
| 4 | 4 | 0 | -2 | 6 | 0 | 6 | 0 | 0 | 0,0,0,0,0,0 |
| 5 | 6 | 0 | 0 | 6 | 0 | 6 | 0 | 0 | 0,0,0,0,0,0 |
| 6 | 0 | 0 | -6 | 6 | 0 | 0 | 0 | 0 | 0,0,0,0,0,0 |
| 7 | 4 | 0 | -2 | 4 | 2 | 0 | 0 | 0 | 0,0,0,0,1,1 |

Harvest ledger: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 08:30 Cave Potato +3; D5 08:40 Cave Potato +3; D7 08:50 Glowcap +2; D7 09:00 Glowcap +2.

Total scavenged 8; missed meals 2; ending reserve 0.

## World seed 1701: 14 Food

food-near-wild_plant-0: 1; food-near-wild_plant-1: 1; food-near-container-0: 2; food-far-wild_plant-0: 2; food-far-container-0: 4; food-far-container-1: 4.

### Regular farm care

#### 2 players

| Day | Farm today | Scavenged today | Net supply − need | Eaten today | Unmet meals today | Personal reserve | Chest reserve | Unpicked | Missed-meal scores |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---|
| 1 | 0 | 4 | +6 | 2 | 0 | 2 | 4 | 10 | 0,0 |
| 2 | 0 | 0 | -2 | 2 | 0 | 2 | 2 | 10 | 0,0 |
| 3 | 6 | 10 | +14 | 2 | 0 | 2 | 16 | 0 | 0,0 |
| 4 | 4 | 0 | +2 | 2 | 0 | 2 | 18 | 0 | 0,0 |
| 5 | 6 | 0 | +4 | 2 | 0 | 2 | 22 | 0 | 0,0 |
| 6 | 0 | 0 | -2 | 2 | 0 | 2 | 20 | 0 | 0,0 |
| 7 | 10 | 0 | +8 | 2 | 0 | 2 | 28 | 0 | 0,0 |

Harvest ledger: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 05:30 Cave Potato +3; D5 05:40 Cave Potato +3; D7 05:50 Glowcap +2; D7 06:00 Glowcap +2; D7 08:30 Cave Potato +3; D7 08:40 Cave Potato +3.

Total scavenged 14; missed meals 0; ending reserve 30.

#### 4 players

| Day | Farm today | Scavenged today | Net supply − need | Eaten today | Unmet meals today | Personal reserve | Chest reserve | Unpicked | Missed-meal scores |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---|
| 1 | 0 | 4 | +8 | 4 | 0 | 4 | 4 | 10 | 0,0,0,0 |
| 2 | 0 | 0 | -4 | 4 | 0 | 4 | 0 | 10 | 0,0,0,0 |
| 3 | 6 | 10 | +12 | 4 | 0 | 4 | 12 | 0 | 0,0,0,0 |
| 4 | 4 | 0 | 0 | 4 | 0 | 4 | 12 | 0 | 0,0,0,0 |
| 5 | 6 | 0 | +2 | 4 | 0 | 4 | 14 | 0 | 0,0,0,0 |
| 6 | 0 | 0 | -4 | 4 | 0 | 4 | 10 | 0 | 0,0,0,0 |
| 7 | 10 | 0 | +6 | 4 | 0 | 4 | 16 | 0 | 0,0,0,0 |

Harvest ledger: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 05:30 Cave Potato +3; D5 05:40 Cave Potato +3; D7 05:50 Glowcap +2; D7 06:00 Glowcap +2; D7 08:30 Cave Potato +3; D7 08:40 Cave Potato +3.

Total scavenged 14; missed meals 0; ending reserve 20.

#### 6 players

| Day | Farm today | Scavenged today | Net supply − need | Eaten today | Unmet meals today | Personal reserve | Chest reserve | Unpicked | Missed-meal scores |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---|
| 1 | 0 | 4 | +10 | 6 | 0 | 6 | 4 | 10 | 0,0,0,0,0,0 |
| 2 | 0 | 0 | -6 | 6 | 0 | 4 | 0 | 10 | 0,0,0,0,0,0 |
| 3 | 6 | 10 | +10 | 6 | 0 | 6 | 8 | 0 | 0,0,0,0,0,0 |
| 4 | 4 | 0 | -2 | 6 | 0 | 6 | 6 | 0 | 0,0,0,0,0,0 |
| 5 | 6 | 0 | 0 | 6 | 0 | 6 | 6 | 0 | 0,0,0,0,0,0 |
| 6 | 0 | 0 | -6 | 6 | 0 | 6 | 0 | 0 | 0,0,0,0,0,0 |
| 7 | 10 | 0 | +4 | 6 | 0 | 6 | 4 | 0 | 0,0,0,0,0,0 |

Harvest ledger: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 05:30 Cave Potato +3; D5 05:40 Cave Potato +3; D7 05:50 Glowcap +2; D7 06:00 Glowcap +2; D7 08:30 Cave Potato +3; D7 08:40 Cave Potato +3.

Total scavenged 14; missed meals 0; ending reserve 10.

### Delayed farm care

#### 2 players

| Day | Farm today | Scavenged today | Net supply − need | Eaten today | Unmet meals today | Personal reserve | Chest reserve | Unpicked | Missed-meal scores |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---|
| 1 | 0 | 4 | +6 | 2 | 0 | 2 | 4 | 10 | 0,0 |
| 2 | 0 | 0 | -2 | 2 | 0 | 2 | 2 | 10 | 0,0 |
| 3 | 6 | 10 | +14 | 2 | 0 | 2 | 16 | 0 | 0,0 |
| 4 | 4 | 0 | +2 | 2 | 0 | 2 | 18 | 0 | 0,0 |
| 5 | 6 | 0 | +4 | 2 | 0 | 2 | 22 | 0 | 0,0 |
| 6 | 0 | 0 | -2 | 2 | 0 | 2 | 20 | 0 | 0,0 |
| 7 | 4 | 0 | +2 | 2 | 0 | 2 | 22 | 0 | 0,0 |

Harvest ledger: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 08:30 Cave Potato +3; D5 08:40 Cave Potato +3; D7 08:50 Glowcap +2; D7 09:00 Glowcap +2.

Total scavenged 14; missed meals 0; ending reserve 24.

#### 4 players

| Day | Farm today | Scavenged today | Net supply − need | Eaten today | Unmet meals today | Personal reserve | Chest reserve | Unpicked | Missed-meal scores |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---|
| 1 | 0 | 4 | +8 | 4 | 0 | 4 | 4 | 10 | 0,0,0,0 |
| 2 | 0 | 0 | -4 | 4 | 0 | 4 | 0 | 10 | 0,0,0,0 |
| 3 | 6 | 10 | +12 | 4 | 0 | 4 | 12 | 0 | 0,0,0,0 |
| 4 | 4 | 0 | 0 | 4 | 0 | 4 | 12 | 0 | 0,0,0,0 |
| 5 | 6 | 0 | +2 | 4 | 0 | 4 | 14 | 0 | 0,0,0,0 |
| 6 | 0 | 0 | -4 | 4 | 0 | 4 | 10 | 0 | 0,0,0,0 |
| 7 | 4 | 0 | 0 | 4 | 0 | 4 | 10 | 0 | 0,0,0,0 |

Harvest ledger: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 08:30 Cave Potato +3; D5 08:40 Cave Potato +3; D7 08:50 Glowcap +2; D7 09:00 Glowcap +2.

Total scavenged 14; missed meals 0; ending reserve 14.

#### 6 players

| Day | Farm today | Scavenged today | Net supply − need | Eaten today | Unmet meals today | Personal reserve | Chest reserve | Unpicked | Missed-meal scores |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---|
| 1 | 0 | 4 | +10 | 6 | 0 | 6 | 4 | 10 | 0,0,0,0,0,0 |
| 2 | 0 | 0 | -6 | 6 | 0 | 4 | 0 | 10 | 0,0,0,0,0,0 |
| 3 | 6 | 10 | +10 | 6 | 0 | 6 | 8 | 0 | 0,0,0,0,0,0 |
| 4 | 4 | 0 | -2 | 6 | 0 | 6 | 6 | 0 | 0,0,0,0,0,0 |
| 5 | 6 | 0 | 0 | 6 | 0 | 6 | 6 | 0 | 0,0,0,0,0,0 |
| 6 | 0 | 0 | -6 | 6 | 0 | 6 | 0 | 0 | 0,0,0,0,0,0 |
| 7 | 4 | 0 | -2 | 6 | 0 | 4 | 0 | 0 | 0,0,0,0,0,0 |

Harvest ledger: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 08:30 Cave Potato +3; D5 08:40 Cave Potato +3; D7 08:50 Glowcap +2; D7 09:00 Glowcap +2.

Total scavenged 14; missed meals 0; ending reserve 4.

## World seed 143: 17 Food

food-near-wild_plant-0: 2; food-near-wild_plant-1: 2; food-near-container-0: 3; food-far-wild_plant-0: 2; food-far-container-0: 4; food-far-container-1: 4.

### Regular farm care

#### 2 players

| Day | Farm today | Scavenged today | Net supply − need | Eaten today | Unmet meals today | Personal reserve | Chest reserve | Unpicked | Missed-meal scores |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---|
| 1 | 0 | 7 | +9 | 2 | 0 | 2 | 7 | 10 | 0,0 |
| 2 | 0 | 0 | -2 | 2 | 0 | 2 | 5 | 10 | 0,0 |
| 3 | 6 | 10 | +14 | 2 | 0 | 2 | 19 | 0 | 0,0 |
| 4 | 4 | 0 | +2 | 2 | 0 | 2 | 21 | 0 | 0,0 |
| 5 | 6 | 0 | +4 | 2 | 0 | 2 | 25 | 0 | 0,0 |
| 6 | 0 | 0 | -2 | 2 | 0 | 2 | 23 | 0 | 0,0 |
| 7 | 10 | 0 | +8 | 2 | 0 | 2 | 31 | 0 | 0,0 |

Harvest ledger: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 05:30 Cave Potato +3; D5 05:40 Cave Potato +3; D7 05:50 Glowcap +2; D7 06:00 Glowcap +2; D7 08:30 Cave Potato +3; D7 08:40 Cave Potato +3.

Total scavenged 17; missed meals 0; ending reserve 33.

#### 4 players

| Day | Farm today | Scavenged today | Net supply − need | Eaten today | Unmet meals today | Personal reserve | Chest reserve | Unpicked | Missed-meal scores |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---|
| 1 | 0 | 7 | +11 | 4 | 0 | 4 | 7 | 10 | 0,0,0,0 |
| 2 | 0 | 0 | -4 | 4 | 0 | 4 | 3 | 10 | 0,0,0,0 |
| 3 | 6 | 10 | +12 | 4 | 0 | 4 | 15 | 0 | 0,0,0,0 |
| 4 | 4 | 0 | 0 | 4 | 0 | 4 | 15 | 0 | 0,0,0,0 |
| 5 | 6 | 0 | +2 | 4 | 0 | 4 | 17 | 0 | 0,0,0,0 |
| 6 | 0 | 0 | -4 | 4 | 0 | 4 | 13 | 0 | 0,0,0,0 |
| 7 | 10 | 0 | +6 | 4 | 0 | 4 | 19 | 0 | 0,0,0,0 |

Harvest ledger: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 05:30 Cave Potato +3; D5 05:40 Cave Potato +3; D7 05:50 Glowcap +2; D7 06:00 Glowcap +2; D7 08:30 Cave Potato +3; D7 08:40 Cave Potato +3.

Total scavenged 17; missed meals 0; ending reserve 23.

#### 6 players

| Day | Farm today | Scavenged today | Net supply − need | Eaten today | Unmet meals today | Personal reserve | Chest reserve | Unpicked | Missed-meal scores |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---|
| 1 | 0 | 7 | +13 | 6 | 0 | 6 | 7 | 10 | 0,0,0,0,0,0 |
| 2 | 0 | 0 | -6 | 6 | 0 | 6 | 1 | 10 | 0,0,0,0,0,0 |
| 3 | 6 | 10 | +10 | 6 | 0 | 6 | 11 | 0 | 0,0,0,0,0,0 |
| 4 | 4 | 0 | -2 | 6 | 0 | 6 | 9 | 0 | 0,0,0,0,0,0 |
| 5 | 6 | 0 | 0 | 6 | 0 | 6 | 9 | 0 | 0,0,0,0,0,0 |
| 6 | 0 | 0 | -6 | 6 | 0 | 6 | 3 | 0 | 0,0,0,0,0,0 |
| 7 | 10 | 0 | +4 | 6 | 0 | 6 | 7 | 0 | 0,0,0,0,0,0 |

Harvest ledger: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 05:30 Cave Potato +3; D5 05:40 Cave Potato +3; D7 05:50 Glowcap +2; D7 06:00 Glowcap +2; D7 08:30 Cave Potato +3; D7 08:40 Cave Potato +3.

Total scavenged 17; missed meals 0; ending reserve 13.

### Delayed farm care

#### 2 players

| Day | Farm today | Scavenged today | Net supply − need | Eaten today | Unmet meals today | Personal reserve | Chest reserve | Unpicked | Missed-meal scores |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---|
| 1 | 0 | 7 | +9 | 2 | 0 | 2 | 7 | 10 | 0,0 |
| 2 | 0 | 0 | -2 | 2 | 0 | 2 | 5 | 10 | 0,0 |
| 3 | 6 | 10 | +14 | 2 | 0 | 2 | 19 | 0 | 0,0 |
| 4 | 4 | 0 | +2 | 2 | 0 | 2 | 21 | 0 | 0,0 |
| 5 | 6 | 0 | +4 | 2 | 0 | 2 | 25 | 0 | 0,0 |
| 6 | 0 | 0 | -2 | 2 | 0 | 2 | 23 | 0 | 0,0 |
| 7 | 4 | 0 | +2 | 2 | 0 | 2 | 25 | 0 | 0,0 |

Harvest ledger: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 08:30 Cave Potato +3; D5 08:40 Cave Potato +3; D7 08:50 Glowcap +2; D7 09:00 Glowcap +2.

Total scavenged 17; missed meals 0; ending reserve 27.

#### 4 players

| Day | Farm today | Scavenged today | Net supply − need | Eaten today | Unmet meals today | Personal reserve | Chest reserve | Unpicked | Missed-meal scores |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---|
| 1 | 0 | 7 | +11 | 4 | 0 | 4 | 7 | 10 | 0,0,0,0 |
| 2 | 0 | 0 | -4 | 4 | 0 | 4 | 3 | 10 | 0,0,0,0 |
| 3 | 6 | 10 | +12 | 4 | 0 | 4 | 15 | 0 | 0,0,0,0 |
| 4 | 4 | 0 | 0 | 4 | 0 | 4 | 15 | 0 | 0,0,0,0 |
| 5 | 6 | 0 | +2 | 4 | 0 | 4 | 17 | 0 | 0,0,0,0 |
| 6 | 0 | 0 | -4 | 4 | 0 | 4 | 13 | 0 | 0,0,0,0 |
| 7 | 4 | 0 | 0 | 4 | 0 | 4 | 13 | 0 | 0,0,0,0 |

Harvest ledger: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 08:30 Cave Potato +3; D5 08:40 Cave Potato +3; D7 08:50 Glowcap +2; D7 09:00 Glowcap +2.

Total scavenged 17; missed meals 0; ending reserve 17.

#### 6 players

| Day | Farm today | Scavenged today | Net supply − need | Eaten today | Unmet meals today | Personal reserve | Chest reserve | Unpicked | Missed-meal scores |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---|
| 1 | 0 | 7 | +13 | 6 | 0 | 6 | 7 | 10 | 0,0,0,0,0,0 |
| 2 | 0 | 0 | -6 | 6 | 0 | 6 | 1 | 10 | 0,0,0,0,0,0 |
| 3 | 6 | 10 | +10 | 6 | 0 | 6 | 11 | 0 | 0,0,0,0,0,0 |
| 4 | 4 | 0 | -2 | 6 | 0 | 6 | 9 | 0 | 0,0,0,0,0,0 |
| 5 | 6 | 0 | 0 | 6 | 0 | 6 | 9 | 0 | 0,0,0,0,0,0 |
| 6 | 0 | 0 | -6 | 6 | 0 | 6 | 3 | 0 | 0,0,0,0,0,0 |
| 7 | 4 | 0 | -2 | 6 | 0 | 6 | 1 | 0 | 0,0,0,0,0,0 |

Harvest ledger: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 08:30 Cave Potato +3; D5 08:40 Cave Potato +3; D7 08:50 Glowcap +2; D7 09:00 Glowcap +2.

Total scavenged 17; missed meals 0; ending reserve 7.

## Late expedition stress case: minimum-stock world, delayed farm care

Near trip unchanged; postpone the far trip until Day 7, returning at 05:00. This demonstrates that a finite cache discovered late cannot retroactively cure missed daily requirements.

#### 2 players

| Day | Farm today | Scavenged today | Net supply − need | Eaten today | Unmet meals today | Personal reserve | Chest reserve | Unpicked | Missed-meal scores |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---|
| 1 | 0 | 3 | +5 | 2 | 0 | 2 | 3 | 5 | 0,0 |
| 2 | 0 | 0 | -2 | 2 | 0 | 2 | 1 | 5 | 0,0 |
| 3 | 6 | 0 | +4 | 2 | 0 | 2 | 5 | 5 | 0,0 |
| 4 | 4 | 0 | +2 | 2 | 0 | 2 | 7 | 5 | 0,0 |
| 5 | 6 | 0 | +4 | 2 | 0 | 2 | 11 | 5 | 0,0 |
| 6 | 0 | 0 | -2 | 2 | 0 | 2 | 9 | 5 | 0,0 |
| 7 | 4 | 5 | +7 | 2 | 0 | 2 | 16 | 0 | 0,0 |

Harvest ledger: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 08:30 Cave Potato +3; D5 08:40 Cave Potato +3; D7 08:50 Glowcap +2; D7 09:00 Glowcap +2.

Total scavenged 8; missed meals 0; ending reserve 18.

#### 4 players

| Day | Farm today | Scavenged today | Net supply − need | Eaten today | Unmet meals today | Personal reserve | Chest reserve | Unpicked | Missed-meal scores |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---|
| 1 | 0 | 3 | +7 | 4 | 0 | 4 | 3 | 5 | 0,0,0,0 |
| 2 | 0 | 0 | -4 | 4 | 0 | 3 | 0 | 5 | 0,0,0,0 |
| 3 | 6 | 0 | +2 | 4 | 0 | 4 | 1 | 5 | 0,0,0,0 |
| 4 | 4 | 0 | 0 | 4 | 0 | 4 | 1 | 5 | 0,0,0,0 |
| 5 | 6 | 0 | +2 | 4 | 0 | 4 | 3 | 5 | 0,0,0,0 |
| 6 | 0 | 0 | -4 | 4 | 0 | 3 | 0 | 5 | 0,0,0,0 |
| 7 | 4 | 5 | +5 | 4 | 0 | 4 | 4 | 0 | 0,0,0,0 |

Harvest ledger: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 08:30 Cave Potato +3; D5 08:40 Cave Potato +3; D7 08:50 Glowcap +2; D7 09:00 Glowcap +2.

Total scavenged 8; missed meals 0; ending reserve 8.

#### 6 players

| Day | Farm today | Scavenged today | Net supply − need | Eaten today | Unmet meals today | Personal reserve | Chest reserve | Unpicked | Missed-meal scores |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---|
| 1 | 0 | 3 | +9 | 6 | 0 | 6 | 3 | 5 | 0,0,0,0,0,0 |
| 2 | 0 | 0 | -6 | 6 | 0 | 3 | 0 | 5 | 0,0,0,0,0,0 |
| 3 | 6 | 0 | 0 | 6 | 0 | 3 | 0 | 5 | 0,0,0,0,0,0 |
| 4 | 4 | 0 | -2 | 6 | 0 | 1 | 0 | 5 | 0,0,0,0,0,0 |
| 5 | 6 | 0 | 0 | 6 | 0 | 1 | 0 | 5 | 0,0,0,0,0,0 |
| 6 | 0 | 0 | -6 | 1 | 5 | 0 | 0 | 5 | 1,1,1,1,0,1 |
| 7 | 4 | 5 | +3 | 6 | 0 | 3 | 0 | 0 | 0,0,0,0,0,0 |

Harvest ledger: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 08:30 Cave Potato +3; D5 08:40 Cave Potato +3; D7 08:50 Glowcap +2; D7 09:00 Glowcap +2.

Total scavenged 8; missed meals 5; ending reserve 3.

## Interpretation

Scavenging supplies immediate relief but is permanently exhausted for that world. The farm remains renewable at unchanged rates; the mixed four plots average 4⅓ Food/day, below six players' need. With no farming at all, starting Food plus the finite cache supplies only 12–21 Food for two players, 16–25 for four, and 20–29 for six, against seven-day needs of 14/28/42. Even a rich cache does not establish long-term sustainability.

No quantities scale with players, no crops or hunger rules were rebalanced, and no enemies, respawning, power or other survival systems were added. Six-player shortages in poor/delayed scenarios are reported rather than patched. Core group simulations are not a networked or human multiplayer playtest.

