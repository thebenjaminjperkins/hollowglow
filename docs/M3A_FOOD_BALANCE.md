# M3a seven-day food balance

Reproduce: `dotnet run --project tests/Hollowlight.Core.Tests.csproj -- --food-balance docs/M3A_FOOD_BALANCE.md`

Each scenario uses the actual four plots, two shared seeds per crop, unchanged 2/3-day growth and 3/2 Food yields, two starting personal Food per player, and one Food/day. Day 1 starts at elapsed 0; rows settle at the following dawn. Food is never auto-withdrawn from storage. No exploration food exists in this milestone.

A designated gardener withdraws seeds at 00:20, plants at 01:00, 01:10, 01:20 and 01:30. Scheduled visits occur at 03:00, 05:30 and 08:30 of each ten-minute cycle. Ready plots take ten seconds each to harvest/replant, with replanting five seconds after harvesting. After each visit, the gardener deposits harvested Food/Organic; a group resupply occurs one minute after the scheduled visit. Players explicitly withdraw toward two personal Food each, rotating first priority by day. This represents regular cooperation with exploration intervals, not instant harvesting or automatic communal feeding. A delayed-care scenario omits the midday visit, retaining the same planting and resupply rules. These are reproducible timing scenarios, not measured human playtests.

## Regular care: three visits per day

### 2 players

| Completed day | Farm Food to date | Eaten to date | Missed meals to date | Personal Food | Chest Food | Missed-meal scores by player |
|---:|---:|---:|---:|---:|---:|:---|
| 1 | 0 | 2 | 0 | 2 | 0 | 0,0 |
| 2 | 0 | 4 | 0 | 0 | 0 | 0,0 |
| 3 | 6 | 6 | 0 | 2 | 2 | 0,0 |
| 4 | 10 | 8 | 0 | 2 | 4 | 0,0 |
| 5 | 16 | 10 | 0 | 2 | 8 | 0,0 |
| 6 | 16 | 12 | 0 | 2 | 6 | 0,0 |
| 7 | 26 | 14 | 0 | 2 | 14 | 0,0 |

Harvest ledger: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 05:30 Cave Potato +3; D5 05:40 Cave Potato +3; D7 05:50 Glowcap +2; D7 06:00 Glowcap +2; D7 08:30 Cave Potato +3; D7 08:40 Cave Potato +3.

### 4 players

| Completed day | Farm Food to date | Eaten to date | Missed meals to date | Personal Food | Chest Food | Missed-meal scores by player |
|---:|---:|---:|---:|---:|---:|:---|
| 1 | 0 | 4 | 0 | 4 | 0 | 0,0,0,0 |
| 2 | 0 | 8 | 0 | 0 | 0 | 0,0,0,0 |
| 3 | 6 | 12 | 0 | 2 | 0 | 0,0,0,0 |
| 4 | 10 | 16 | 0 | 2 | 0 | 0,0,0,0 |
| 5 | 16 | 20 | 0 | 4 | 0 | 0,0,0,0 |
| 6 | 16 | 24 | 0 | 0 | 0 | 0,0,0,0 |
| 7 | 26 | 28 | 0 | 4 | 2 | 0,0,0,0 |

Harvest ledger: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 05:30 Cave Potato +3; D5 05:40 Cave Potato +3; D7 05:50 Glowcap +2; D7 06:00 Glowcap +2; D7 08:30 Cave Potato +3; D7 08:40 Cave Potato +3.

### 6 players

| Completed day | Farm Food to date | Eaten to date | Missed meals to date | Personal Food | Chest Food | Missed-meal scores by player |
|---:|---:|---:|---:|---:|---:|:---|
| 1 | 0 | 6 | 0 | 6 | 0 | 0,0,0,0,0,0 |
| 2 | 0 | 12 | 0 | 0 | 0 | 0,0,0,0,0,0 |
| 3 | 6 | 18 | 0 | 0 | 0 | 0,0,0,0,0,0 |
| 4 | 10 | 22 | 2 | 0 | 0 | 0,1,1,0,0,0 |
| 5 | 16 | 28 | 2 | 0 | 0 | 0,0,0,0,0,0 |
| 6 | 16 | 28 | 8 | 0 | 0 | 1,1,1,1,1,1 |
| 7 | 26 | 34 | 8 | 4 | 0 | 0,0,0,0,0,0 |

Harvest ledger: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 05:30 Cave Potato +3; D5 05:40 Cave Potato +3; D7 05:50 Glowcap +2; D7 06:00 Glowcap +2; D7 08:30 Cave Potato +3; D7 08:40 Cave Potato +3.

## Delayed care: two visits per day

### 2 players

| Completed day | Farm Food to date | Eaten to date | Missed meals to date | Personal Food | Chest Food | Missed-meal scores by player |
|---:|---:|---:|---:|---:|---:|:---|
| 1 | 0 | 2 | 0 | 2 | 0 | 0,0 |
| 2 | 0 | 4 | 0 | 0 | 0 | 0,0 |
| 3 | 6 | 6 | 0 | 2 | 2 | 0,0 |
| 4 | 10 | 8 | 0 | 2 | 4 | 0,0 |
| 5 | 16 | 10 | 0 | 2 | 8 | 0,0 |
| 6 | 16 | 12 | 0 | 2 | 6 | 0,0 |
| 7 | 20 | 14 | 0 | 2 | 8 | 0,0 |

Harvest ledger: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 08:30 Cave Potato +3; D5 08:40 Cave Potato +3; D7 08:50 Glowcap +2; D7 09:00 Glowcap +2.

### 4 players

| Completed day | Farm Food to date | Eaten to date | Missed meals to date | Personal Food | Chest Food | Missed-meal scores by player |
|---:|---:|---:|---:|---:|---:|:---|
| 1 | 0 | 4 | 0 | 4 | 0 | 0,0,0,0 |
| 2 | 0 | 8 | 0 | 0 | 0 | 0,0,0,0 |
| 3 | 6 | 12 | 0 | 2 | 0 | 0,0,0,0 |
| 4 | 10 | 16 | 0 | 2 | 0 | 0,0,0,0 |
| 5 | 16 | 20 | 0 | 4 | 0 | 0,0,0,0 |
| 6 | 16 | 24 | 0 | 0 | 0 | 0,0,0,0 |
| 7 | 20 | 28 | 0 | 0 | 0 | 0,0,0,0 |

Harvest ledger: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 08:30 Cave Potato +3; D5 08:40 Cave Potato +3; D7 08:50 Glowcap +2; D7 09:00 Glowcap +2.

### 6 players

| Completed day | Farm Food to date | Eaten to date | Missed meals to date | Personal Food | Chest Food | Missed-meal scores by player |
|---:|---:|---:|---:|---:|---:|:---|
| 1 | 0 | 6 | 0 | 6 | 0 | 0,0,0,0,0,0 |
| 2 | 0 | 12 | 0 | 0 | 0 | 0,0,0,0,0,0 |
| 3 | 6 | 18 | 0 | 0 | 0 | 0,0,0,0,0,0 |
| 4 | 10 | 22 | 2 | 0 | 0 | 0,1,1,0,0,0 |
| 5 | 16 | 28 | 2 | 0 | 0 | 0,0,0,0,0,0 |
| 6 | 16 | 28 | 8 | 0 | 0 | 1,1,1,1,1,1 |
| 7 | 20 | 32 | 10 | 0 | 0 | 0,0,0,0,2,2 |

Harvest ledger: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 08:30 Cave Potato +3; D5 08:40 Cave Potato +3; D7 08:50 Glowcap +2; D7 09:00 Glowcap +2.

## Interpretation

A positive final group balance does not guarantee every meal deadline is met: production arrives in discrete harvests, and Food left in the chest is not eaten. Missed deadlines below count individual player-meals, not lost Food. Supplied meals gradually recover accumulated missed-meal scores. Priority rotation distributes scarce supplies without creating food.

The mixed four-plot farm averages 4⅓ Food/day over the long run, below six players' 6/day. Surface exploration currently rewards Scrap and Organic only and cannot repair a Food shortage. Future scavenging, outdoor agriculture and infrastructure are explicitly deferred. No crop, starter-seed, plot-count or economy rebalance was made.

