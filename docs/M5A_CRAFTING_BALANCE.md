# M5a revised-cost seven-day investment balance

Reproduce: `dotnet run --project tests/Hollowlight.Core.Tests.csproj -- --crafting-balance docs/M5A_CRAFTING_BALANCE.md`

Before implementation, exact budget enumeration confirmed the revised costs allow one backpack plus a working bed (20 Scrap, 8 Organic), but two backpacks plus bed/propagation require 28 Scrap, above the world's finite 26. No new design conflict: the choice is group distribution versus production, rather than exclusive recipe categories. Two actors can outfit both instead of expanding; four/six can outfit at most three, with the third waiting for renewable Organic.

This post-implementation replay uses actual authoritative Craft/Install/Plant/Harvest/Gather/Transfer/Power/Hunger/Rescue operations, unchanged seeded Food/Fuel worlds, four starting plots, two seeds per crop, two starting Food per actor and two shared Fuel. No injected resources, altered source quantities or changed growth/yields. All inputs are explicitly withdrawn into the operator's backpack. Kits for other actors pass through the chest. Conservation includes materials intentionally consumed and the exact net seed created by propagation.

Timing: original farm plants D1 01:00/01:10/01:20/01:30; visits at 03:00/05:30/08:30 (delayed omits 05:30), ten seconds per plot, replant five seconds later. Near Food returns D1 02:00, far D3 05:00; original material nodes gathered D1 02:30/02:40/02:50 and deposited at 03:30. Backpack workshop visit 03:30–04:00; second kit visit 04:10–04:40. Expansion policy propagates the first returned potato seed D3 03:10, replants the original at 03:35 and plants the fifth at 04:00. Backpack-only groups of 4/6 craft the third after Glowcaps, D4 03:50–04:20. Each powered visit lasts thirty seconds; crafting itself is immediate. These are reproducible compact-map cooperation assumptions, not measured networked expeditions.

Power retains M4b's scheduled policy (beacon continuously, workbench seconds 30–60/day, lighting during Night), plus the explicit extra visits above. Near Fuel returns D2 03:00, far D4 03:00, manual refills daily 03:00 and on deliveries. Rows settle at dawn; simulation continues beyond Day 7 to the exact seven-signal-day victory. No automatic resource pooling/refueling or priorities.

### Seed 499, 2 players, backpacks only, regular care

| Day | Farm Food | Field Food | Food supply − need | Missed meals | Food reserve | Fuel burned | Fuel left incl. unpicked | Signal days | Signal lost | Available Scrap / Organic | Installed packs / plots | Rescue feasibility |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---|:---|:---|
| 1 | 0 | 3 | +5 | 0 | 5 | 0.690 | 7.310 | 0.950 | 0.000 | 10 / 2 | 2 / 4 | Feasible lower bound |
| 2 | 0 | 0 | -2 | 0 | 3 | 0.680 | 6.630 | 1.950 | 0.000 | 10 / 2 | 2 / 4 | Feasible lower bound |
| 3 | 6 | 5 | +9 | 0 | 12 | 0.680 | 5.950 | 2.950 | 0.000 | 10 / 2 | 2 / 4 | Feasible lower bound |
| 4 | 4 | 0 | +2 | 0 | 14 | 0.680 | 5.270 | 3.950 | 0.000 | 10 / 4 | 2 / 4 | Feasible lower bound |
| 5 | 6 | 0 | +4 | 0 | 18 | 0.680 | 4.590 | 4.950 | 0.000 | 10 / 4 | 2 / 4 | Feasible lower bound |
| 6 | 0 | 0 | -2 | 0 | 16 | 0.680 | 3.910 | 5.950 | 0.000 | 10 / 4 | 2 / 4 | Feasible lower bound |
| 7 | 10 | 0 | +8 | 0 | 24 | 0.680 | 3.230 | 6.950 | 0.000 | 10 / 6 | 2 / 4 | Feasible lower bound |

Outcome: Rescued at D8 00:30; signal 7.000, lost 0.000, Fuel left 3.200; total missed meals 0. Fuel sufficient with these collection/refill timings.

Harvest ledger through termination: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 05:30 Cave Potato +3; D5 05:40 Cave Potato +3; D7 05:50 Glowcap +2; D7 06:00 Glowcap +2; D7 08:30 Cave Potato +3; D7 08:40 Cave Potato +3.

### Seed 499, 2 players, one backpack + fifth plot + propagation, regular care

| Day | Farm Food | Field Food | Food supply − need | Missed meals | Food reserve | Fuel burned | Fuel left incl. unpicked | Signal days | Signal lost | Available Scrap / Organic | Installed packs / plots | Rescue feasibility |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---|:---|:---|
| 1 | 0 | 3 | +5 | 0 | 5 | 0.690 | 7.310 | 0.950 | 0.000 | 6 / 1 | 1 / 5 | Feasible lower bound |
| 2 | 0 | 0 | -2 | 0 | 3 | 0.680 | 6.630 | 1.950 | 0.000 | 6 / 1 | 1 / 5 | Feasible lower bound |
| 3 | 6 | 5 | +9 | 0 | 12 | 0.700 | 5.930 | 2.950 | 0.000 | 6 / 0 | 1 / 5 | Feasible lower bound |
| 4 | 4 | 0 | +2 | 0 | 14 | 0.680 | 5.250 | 3.950 | 0.000 | 6 / 2 | 1 / 5 | Feasible lower bound |
| 5 | 9 | 0 | +7 | 0 | 21 | 0.680 | 4.570 | 4.950 | 0.000 | 6 / 2 | 1 / 5 | Feasible lower bound |
| 6 | 0 | 0 | -2 | 0 | 19 | 0.680 | 3.890 | 5.950 | 0.000 | 6 / 2 | 1 / 5 | Feasible lower bound |
| 7 | 13 | 0 | +11 | 0 | 30 | 0.680 | 3.210 | 6.950 | 0.000 | 6 / 4 | 1 / 5 | Feasible lower bound |

Outcome: Rescued at D8 00:30; signal 7.000, lost 0.000, Fuel left 3.180; total missed meals 0. Fuel sufficient with these collection/refill timings.

Harvest ledger through termination: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 05:30 Cave Potato +3; D5 05:40 Cave Potato +3; D5 06:10 Cave Potato +3; D7 05:50 Glowcap +2; D7 06:00 Glowcap +2; D7 08:30 Cave Potato +3; D7 08:40 Cave Potato +3; D7 09:10 Cave Potato +3.

### Seed 499, 4 players, backpacks only, regular care

| Day | Farm Food | Field Food | Food supply − need | Missed meals | Food reserve | Fuel burned | Fuel left incl. unpicked | Signal days | Signal lost | Available Scrap / Organic | Installed packs / plots | Rescue feasibility |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---|:---|:---|
| 1 | 0 | 3 | +7 | 0 | 7 | 0.690 | 7.310 | 0.950 | 0.000 | 10 / 2 | 2 / 4 | Feasible lower bound |
| 2 | 0 | 0 | -4 | 0 | 3 | 0.680 | 6.630 | 1.950 | 0.000 | 10 / 2 | 2 / 4 | Feasible lower bound |
| 3 | 6 | 5 | +7 | 0 | 10 | 0.680 | 5.950 | 2.950 | 0.000 | 10 / 2 | 2 / 4 | Feasible lower bound |
| 4 | 4 | 0 | 0 | 0 | 10 | 0.700 | 5.250 | 3.950 | 0.000 | 2 / 1 | 3 / 4 | Feasible lower bound |
| 5 | 6 | 0 | +2 | 0 | 12 | 0.680 | 4.570 | 4.950 | 0.000 | 2 / 1 | 3 / 4 | Feasible lower bound |
| 6 | 0 | 0 | -4 | 0 | 8 | 0.680 | 3.890 | 5.950 | 0.000 | 2 / 1 | 3 / 4 | Feasible lower bound |
| 7 | 10 | 0 | +6 | 0 | 14 | 0.680 | 3.210 | 6.950 | 0.000 | 2 / 3 | 3 / 4 | Feasible lower bound |

Outcome: Rescued at D8 00:30; signal 7.000, lost 0.000, Fuel left 3.180; total missed meals 0. Fuel sufficient with these collection/refill timings.

Harvest ledger through termination: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 05:30 Cave Potato +3; D5 05:40 Cave Potato +3; D7 05:50 Glowcap +2; D7 06:00 Glowcap +2; D7 08:30 Cave Potato +3; D7 08:40 Cave Potato +3.

### Seed 499, 4 players, one backpack + fifth plot + propagation, regular care

| Day | Farm Food | Field Food | Food supply − need | Missed meals | Food reserve | Fuel burned | Fuel left incl. unpicked | Signal days | Signal lost | Available Scrap / Organic | Installed packs / plots | Rescue feasibility |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---|:---|:---|
| 1 | 0 | 3 | +7 | 0 | 7 | 0.690 | 7.310 | 0.950 | 0.000 | 6 / 1 | 1 / 5 | Feasible lower bound |
| 2 | 0 | 0 | -4 | 0 | 3 | 0.680 | 6.630 | 1.950 | 0.000 | 6 / 1 | 1 / 5 | Feasible lower bound |
| 3 | 6 | 5 | +7 | 0 | 10 | 0.700 | 5.930 | 2.950 | 0.000 | 6 / 0 | 1 / 5 | Feasible lower bound |
| 4 | 4 | 0 | 0 | 0 | 10 | 0.680 | 5.250 | 3.950 | 0.000 | 6 / 2 | 1 / 5 | Feasible lower bound |
| 5 | 9 | 0 | +5 | 0 | 15 | 0.680 | 4.570 | 4.950 | 0.000 | 6 / 2 | 1 / 5 | Feasible lower bound |
| 6 | 0 | 0 | -4 | 0 | 11 | 0.680 | 3.890 | 5.950 | 0.000 | 6 / 2 | 1 / 5 | Feasible lower bound |
| 7 | 13 | 0 | +9 | 0 | 20 | 0.680 | 3.210 | 6.950 | 0.000 | 6 / 4 | 1 / 5 | Feasible lower bound |

Outcome: Rescued at D8 00:30; signal 7.000, lost 0.000, Fuel left 3.180; total missed meals 0. Fuel sufficient with these collection/refill timings.

Harvest ledger through termination: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 05:30 Cave Potato +3; D5 05:40 Cave Potato +3; D5 06:10 Cave Potato +3; D7 05:50 Glowcap +2; D7 06:00 Glowcap +2; D7 08:30 Cave Potato +3; D7 08:40 Cave Potato +3; D7 09:10 Cave Potato +3.

### Seed 499, 6 players, backpacks only, regular care

| Day | Farm Food | Field Food | Food supply − need | Missed meals | Food reserve | Fuel burned | Fuel left incl. unpicked | Signal days | Signal lost | Available Scrap / Organic | Installed packs / plots | Rescue feasibility |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---|:---|:---|
| 1 | 0 | 3 | +9 | 0 | 9 | 0.690 | 7.310 | 0.950 | 0.000 | 10 / 2 | 2 / 4 | Feasible lower bound |
| 2 | 0 | 0 | -6 | 0 | 3 | 0.680 | 6.630 | 1.950 | 0.000 | 10 / 2 | 2 / 4 | Feasible lower bound |
| 3 | 6 | 5 | +5 | 0 | 8 | 0.680 | 5.950 | 2.950 | 0.000 | 10 / 2 | 2 / 4 | Feasible lower bound |
| 4 | 4 | 0 | -2 | 0 | 6 | 0.700 | 5.250 | 3.950 | 0.000 | 2 / 1 | 3 / 4 | Feasible lower bound |
| 5 | 6 | 0 | 0 | 0 | 6 | 0.680 | 4.570 | 4.950 | 0.000 | 2 / 1 | 3 / 4 | Feasible lower bound |
| 6 | 0 | 0 | -6 | 0 | 0 | 0.680 | 3.890 | 5.950 | 0.000 | 2 / 1 | 3 / 4 | Feasible lower bound |
| 7 | 10 | 0 | +4 | 0 | 4 | 0.680 | 3.210 | 6.950 | 0.000 | 2 / 3 | 3 / 4 | Feasible lower bound |

Outcome: Rescued at D8 00:30; signal 7.000, lost 0.000, Fuel left 3.180; total missed meals 0. Fuel sufficient with these collection/refill timings.

Harvest ledger through termination: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 05:30 Cave Potato +3; D5 05:40 Cave Potato +3; D7 05:50 Glowcap +2; D7 06:00 Glowcap +2; D7 08:30 Cave Potato +3; D7 08:40 Cave Potato +3.

### Seed 499, 6 players, one backpack + fifth plot + propagation, regular care

| Day | Farm Food | Field Food | Food supply − need | Missed meals | Food reserve | Fuel burned | Fuel left incl. unpicked | Signal days | Signal lost | Available Scrap / Organic | Installed packs / plots | Rescue feasibility |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---|:---|:---|
| 1 | 0 | 3 | +9 | 0 | 9 | 0.690 | 7.310 | 0.950 | 0.000 | 6 / 1 | 1 / 5 | Feasible lower bound |
| 2 | 0 | 0 | -6 | 0 | 3 | 0.680 | 6.630 | 1.950 | 0.000 | 6 / 1 | 1 / 5 | Feasible lower bound |
| 3 | 6 | 5 | +5 | 0 | 8 | 0.700 | 5.930 | 2.950 | 0.000 | 6 / 0 | 1 / 5 | Feasible lower bound |
| 4 | 4 | 0 | -2 | 0 | 6 | 0.680 | 5.250 | 3.950 | 0.000 | 6 / 2 | 1 / 5 | Feasible lower bound |
| 5 | 9 | 0 | +3 | 0 | 9 | 0.680 | 4.570 | 4.950 | 0.000 | 6 / 2 | 1 / 5 | Feasible lower bound |
| 6 | 0 | 0 | -6 | 0 | 3 | 0.680 | 3.890 | 5.950 | 0.000 | 6 / 2 | 1 / 5 | Feasible lower bound |
| 7 | 13 | 0 | +7 | 0 | 10 | 0.680 | 3.210 | 6.950 | 0.000 | 6 / 4 | 1 / 5 | Feasible lower bound |

Outcome: Rescued at D8 00:30; signal 7.000, lost 0.000, Fuel left 3.180; total missed meals 0. Fuel sufficient with these collection/refill timings.

Harvest ledger through termination: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 05:30 Cave Potato +3; D5 05:40 Cave Potato +3; D5 06:10 Cave Potato +3; D7 05:50 Glowcap +2; D7 06:00 Glowcap +2; D7 08:30 Cave Potato +3; D7 08:40 Cave Potato +3; D7 09:10 Cave Potato +3.

### Seed 1701, 2 players, backpacks only, regular care

| Day | Farm Food | Field Food | Food supply − need | Missed meals | Food reserve | Fuel burned | Fuel left incl. unpicked | Signal days | Signal lost | Available Scrap / Organic | Installed packs / plots | Rescue feasibility |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---|:---|:---|
| 1 | 0 | 4 | +6 | 0 | 6 | 0.690 | 7.310 | 0.950 | 0.000 | 10 / 2 | 2 / 4 | Feasible lower bound |
| 2 | 0 | 0 | -2 | 0 | 4 | 0.680 | 6.630 | 1.950 | 0.000 | 10 / 2 | 2 / 4 | Feasible lower bound |
| 3 | 6 | 10 | +14 | 0 | 18 | 0.680 | 5.950 | 2.950 | 0.000 | 10 / 2 | 2 / 4 | Feasible lower bound |
| 4 | 4 | 0 | +2 | 0 | 20 | 0.680 | 5.270 | 3.950 | 0.000 | 10 / 4 | 2 / 4 | Feasible lower bound |
| 5 | 6 | 0 | +4 | 0 | 24 | 0.680 | 4.590 | 4.950 | 0.000 | 10 / 4 | 2 / 4 | Feasible lower bound |
| 6 | 0 | 0 | -2 | 0 | 22 | 0.680 | 3.910 | 5.950 | 0.000 | 10 / 4 | 2 / 4 | Feasible lower bound |
| 7 | 10 | 0 | +8 | 0 | 30 | 0.680 | 3.230 | 6.950 | 0.000 | 10 / 6 | 2 / 4 | Feasible lower bound |

Outcome: Rescued at D8 00:30; signal 7.000, lost 0.000, Fuel left 3.200; total missed meals 0. Fuel sufficient with these collection/refill timings.

Harvest ledger through termination: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 05:30 Cave Potato +3; D5 05:40 Cave Potato +3; D7 05:50 Glowcap +2; D7 06:00 Glowcap +2; D7 08:30 Cave Potato +3; D7 08:40 Cave Potato +3.

### Seed 1701, 2 players, one backpack + fifth plot + propagation, regular care

| Day | Farm Food | Field Food | Food supply − need | Missed meals | Food reserve | Fuel burned | Fuel left incl. unpicked | Signal days | Signal lost | Available Scrap / Organic | Installed packs / plots | Rescue feasibility |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---|:---|:---|
| 1 | 0 | 4 | +6 | 0 | 6 | 0.690 | 7.310 | 0.950 | 0.000 | 6 / 1 | 1 / 5 | Feasible lower bound |
| 2 | 0 | 0 | -2 | 0 | 4 | 0.680 | 6.630 | 1.950 | 0.000 | 6 / 1 | 1 / 5 | Feasible lower bound |
| 3 | 6 | 10 | +14 | 0 | 18 | 0.700 | 5.930 | 2.950 | 0.000 | 6 / 0 | 1 / 5 | Feasible lower bound |
| 4 | 4 | 0 | +2 | 0 | 20 | 0.680 | 5.250 | 3.950 | 0.000 | 6 / 2 | 1 / 5 | Feasible lower bound |
| 5 | 9 | 0 | +7 | 0 | 27 | 0.680 | 4.570 | 4.950 | 0.000 | 6 / 2 | 1 / 5 | Feasible lower bound |
| 6 | 0 | 0 | -2 | 0 | 25 | 0.680 | 3.890 | 5.950 | 0.000 | 6 / 2 | 1 / 5 | Feasible lower bound |
| 7 | 13 | 0 | +11 | 0 | 36 | 0.680 | 3.210 | 6.950 | 0.000 | 6 / 4 | 1 / 5 | Feasible lower bound |

Outcome: Rescued at D8 00:30; signal 7.000, lost 0.000, Fuel left 3.180; total missed meals 0. Fuel sufficient with these collection/refill timings.

Harvest ledger through termination: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 05:30 Cave Potato +3; D5 05:40 Cave Potato +3; D5 06:10 Cave Potato +3; D7 05:50 Glowcap +2; D7 06:00 Glowcap +2; D7 08:30 Cave Potato +3; D7 08:40 Cave Potato +3; D7 09:10 Cave Potato +3.

### Seed 1701, 4 players, backpacks only, regular care

| Day | Farm Food | Field Food | Food supply − need | Missed meals | Food reserve | Fuel burned | Fuel left incl. unpicked | Signal days | Signal lost | Available Scrap / Organic | Installed packs / plots | Rescue feasibility |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---|:---|:---|
| 1 | 0 | 4 | +8 | 0 | 8 | 0.690 | 7.310 | 0.950 | 0.000 | 10 / 2 | 2 / 4 | Feasible lower bound |
| 2 | 0 | 0 | -4 | 0 | 4 | 0.680 | 6.630 | 1.950 | 0.000 | 10 / 2 | 2 / 4 | Feasible lower bound |
| 3 | 6 | 10 | +12 | 0 | 16 | 0.680 | 5.950 | 2.950 | 0.000 | 10 / 2 | 2 / 4 | Feasible lower bound |
| 4 | 4 | 0 | 0 | 0 | 16 | 0.700 | 5.250 | 3.950 | 0.000 | 2 / 1 | 3 / 4 | Feasible lower bound |
| 5 | 6 | 0 | +2 | 0 | 18 | 0.680 | 4.570 | 4.950 | 0.000 | 2 / 1 | 3 / 4 | Feasible lower bound |
| 6 | 0 | 0 | -4 | 0 | 14 | 0.680 | 3.890 | 5.950 | 0.000 | 2 / 1 | 3 / 4 | Feasible lower bound |
| 7 | 10 | 0 | +6 | 0 | 20 | 0.680 | 3.210 | 6.950 | 0.000 | 2 / 3 | 3 / 4 | Feasible lower bound |

Outcome: Rescued at D8 00:30; signal 7.000, lost 0.000, Fuel left 3.180; total missed meals 0. Fuel sufficient with these collection/refill timings.

Harvest ledger through termination: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 05:30 Cave Potato +3; D5 05:40 Cave Potato +3; D7 05:50 Glowcap +2; D7 06:00 Glowcap +2; D7 08:30 Cave Potato +3; D7 08:40 Cave Potato +3.

### Seed 1701, 4 players, one backpack + fifth plot + propagation, regular care

| Day | Farm Food | Field Food | Food supply − need | Missed meals | Food reserve | Fuel burned | Fuel left incl. unpicked | Signal days | Signal lost | Available Scrap / Organic | Installed packs / plots | Rescue feasibility |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---|:---|:---|
| 1 | 0 | 4 | +8 | 0 | 8 | 0.690 | 7.310 | 0.950 | 0.000 | 6 / 1 | 1 / 5 | Feasible lower bound |
| 2 | 0 | 0 | -4 | 0 | 4 | 0.680 | 6.630 | 1.950 | 0.000 | 6 / 1 | 1 / 5 | Feasible lower bound |
| 3 | 6 | 10 | +12 | 0 | 16 | 0.700 | 5.930 | 2.950 | 0.000 | 6 / 0 | 1 / 5 | Feasible lower bound |
| 4 | 4 | 0 | 0 | 0 | 16 | 0.680 | 5.250 | 3.950 | 0.000 | 6 / 2 | 1 / 5 | Feasible lower bound |
| 5 | 9 | 0 | +5 | 0 | 21 | 0.680 | 4.570 | 4.950 | 0.000 | 6 / 2 | 1 / 5 | Feasible lower bound |
| 6 | 0 | 0 | -4 | 0 | 17 | 0.680 | 3.890 | 5.950 | 0.000 | 6 / 2 | 1 / 5 | Feasible lower bound |
| 7 | 13 | 0 | +9 | 0 | 26 | 0.680 | 3.210 | 6.950 | 0.000 | 6 / 4 | 1 / 5 | Feasible lower bound |

Outcome: Rescued at D8 00:30; signal 7.000, lost 0.000, Fuel left 3.180; total missed meals 0. Fuel sufficient with these collection/refill timings.

Harvest ledger through termination: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 05:30 Cave Potato +3; D5 05:40 Cave Potato +3; D5 06:10 Cave Potato +3; D7 05:50 Glowcap +2; D7 06:00 Glowcap +2; D7 08:30 Cave Potato +3; D7 08:40 Cave Potato +3; D7 09:10 Cave Potato +3.

### Seed 1701, 6 players, backpacks only, regular care

| Day | Farm Food | Field Food | Food supply − need | Missed meals | Food reserve | Fuel burned | Fuel left incl. unpicked | Signal days | Signal lost | Available Scrap / Organic | Installed packs / plots | Rescue feasibility |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---|:---|:---|
| 1 | 0 | 4 | +10 | 0 | 10 | 0.690 | 7.310 | 0.950 | 0.000 | 10 / 2 | 2 / 4 | Feasible lower bound |
| 2 | 0 | 0 | -6 | 0 | 4 | 0.680 | 6.630 | 1.950 | 0.000 | 10 / 2 | 2 / 4 | Feasible lower bound |
| 3 | 6 | 10 | +10 | 0 | 14 | 0.680 | 5.950 | 2.950 | 0.000 | 10 / 2 | 2 / 4 | Feasible lower bound |
| 4 | 4 | 0 | -2 | 0 | 12 | 0.700 | 5.250 | 3.950 | 0.000 | 2 / 1 | 3 / 4 | Feasible lower bound |
| 5 | 6 | 0 | 0 | 0 | 12 | 0.680 | 4.570 | 4.950 | 0.000 | 2 / 1 | 3 / 4 | Feasible lower bound |
| 6 | 0 | 0 | -6 | 0 | 6 | 0.680 | 3.890 | 5.950 | 0.000 | 2 / 1 | 3 / 4 | Feasible lower bound |
| 7 | 10 | 0 | +4 | 0 | 10 | 0.680 | 3.210 | 6.950 | 0.000 | 2 / 3 | 3 / 4 | Feasible lower bound |

Outcome: Rescued at D8 00:30; signal 7.000, lost 0.000, Fuel left 3.180; total missed meals 0. Fuel sufficient with these collection/refill timings.

Harvest ledger through termination: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 05:30 Cave Potato +3; D5 05:40 Cave Potato +3; D7 05:50 Glowcap +2; D7 06:00 Glowcap +2; D7 08:30 Cave Potato +3; D7 08:40 Cave Potato +3.

### Seed 1701, 6 players, one backpack + fifth plot + propagation, regular care

| Day | Farm Food | Field Food | Food supply − need | Missed meals | Food reserve | Fuel burned | Fuel left incl. unpicked | Signal days | Signal lost | Available Scrap / Organic | Installed packs / plots | Rescue feasibility |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---|:---|:---|
| 1 | 0 | 4 | +10 | 0 | 10 | 0.690 | 7.310 | 0.950 | 0.000 | 6 / 1 | 1 / 5 | Feasible lower bound |
| 2 | 0 | 0 | -6 | 0 | 4 | 0.680 | 6.630 | 1.950 | 0.000 | 6 / 1 | 1 / 5 | Feasible lower bound |
| 3 | 6 | 10 | +10 | 0 | 14 | 0.700 | 5.930 | 2.950 | 0.000 | 6 / 0 | 1 / 5 | Feasible lower bound |
| 4 | 4 | 0 | -2 | 0 | 12 | 0.680 | 5.250 | 3.950 | 0.000 | 6 / 2 | 1 / 5 | Feasible lower bound |
| 5 | 9 | 0 | +3 | 0 | 15 | 0.680 | 4.570 | 4.950 | 0.000 | 6 / 2 | 1 / 5 | Feasible lower bound |
| 6 | 0 | 0 | -6 | 0 | 9 | 0.680 | 3.890 | 5.950 | 0.000 | 6 / 2 | 1 / 5 | Feasible lower bound |
| 7 | 13 | 0 | +7 | 0 | 16 | 0.680 | 3.210 | 6.950 | 0.000 | 6 / 4 | 1 / 5 | Feasible lower bound |

Outcome: Rescued at D8 00:30; signal 7.000, lost 0.000, Fuel left 3.180; total missed meals 0. Fuel sufficient with these collection/refill timings.

Harvest ledger through termination: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 05:30 Cave Potato +3; D5 05:40 Cave Potato +3; D5 06:10 Cave Potato +3; D7 05:50 Glowcap +2; D7 06:00 Glowcap +2; D7 08:30 Cave Potato +3; D7 08:40 Cave Potato +3; D7 09:10 Cave Potato +3.

### Seed 8, 2 players, backpacks only, regular care

| Day | Farm Food | Field Food | Food supply − need | Missed meals | Food reserve | Fuel burned | Fuel left incl. unpicked | Signal days | Signal lost | Available Scrap / Organic | Installed packs / plots | Rescue feasibility |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---|:---|:---|
| 1 | 0 | 4 | +6 | 0 | 6 | 0.690 | 6.310 | 0.950 | 0.000 | 10 / 2 | 2 / 4 | Feasible lower bound |
| 2 | 0 | 0 | -2 | 0 | 4 | 0.680 | 5.630 | 1.950 | 0.000 | 10 / 2 | 2 / 4 | Feasible lower bound |
| 3 | 6 | 8 | +12 | 0 | 16 | 0.680 | 4.950 | 2.950 | 0.000 | 10 / 2 | 2 / 4 | Feasible lower bound |
| 4 | 4 | 0 | +2 | 0 | 18 | 0.680 | 4.270 | 3.950 | 0.000 | 10 / 4 | 2 / 4 | Feasible lower bound |
| 5 | 6 | 0 | +4 | 0 | 22 | 0.680 | 3.590 | 4.950 | 0.000 | 10 / 4 | 2 / 4 | Feasible lower bound |
| 6 | 0 | 0 | -2 | 0 | 20 | 0.680 | 2.910 | 5.950 | 0.000 | 10 / 4 | 2 / 4 | Feasible lower bound |
| 7 | 10 | 0 | +8 | 0 | 28 | 0.680 | 2.230 | 6.950 | 0.000 | 10 / 6 | 2 / 4 | Feasible lower bound |

Outcome: Rescued at D8 00:30; signal 7.000, lost 0.000, Fuel left 2.200; total missed meals 0. Fuel sufficient with these collection/refill timings.

Harvest ledger through termination: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 05:30 Cave Potato +3; D5 05:40 Cave Potato +3; D7 05:50 Glowcap +2; D7 06:00 Glowcap +2; D7 08:30 Cave Potato +3; D7 08:40 Cave Potato +3.

### Seed 8, 2 players, one backpack + fifth plot + propagation, regular care

| Day | Farm Food | Field Food | Food supply − need | Missed meals | Food reserve | Fuel burned | Fuel left incl. unpicked | Signal days | Signal lost | Available Scrap / Organic | Installed packs / plots | Rescue feasibility |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---|:---|:---|
| 1 | 0 | 4 | +6 | 0 | 6 | 0.690 | 6.310 | 0.950 | 0.000 | 6 / 1 | 1 / 5 | Feasible lower bound |
| 2 | 0 | 0 | -2 | 0 | 4 | 0.680 | 5.630 | 1.950 | 0.000 | 6 / 1 | 1 / 5 | Feasible lower bound |
| 3 | 6 | 8 | +12 | 0 | 16 | 0.700 | 4.930 | 2.950 | 0.000 | 6 / 0 | 1 / 5 | Feasible lower bound |
| 4 | 4 | 0 | +2 | 0 | 18 | 0.680 | 4.250 | 3.950 | 0.000 | 6 / 2 | 1 / 5 | Feasible lower bound |
| 5 | 9 | 0 | +7 | 0 | 25 | 0.680 | 3.570 | 4.950 | 0.000 | 6 / 2 | 1 / 5 | Feasible lower bound |
| 6 | 0 | 0 | -2 | 0 | 23 | 0.680 | 2.890 | 5.950 | 0.000 | 6 / 2 | 1 / 5 | Feasible lower bound |
| 7 | 13 | 0 | +11 | 0 | 34 | 0.680 | 2.210 | 6.950 | 0.000 | 6 / 4 | 1 / 5 | Feasible lower bound |

Outcome: Rescued at D8 00:30; signal 7.000, lost 0.000, Fuel left 2.180; total missed meals 0. Fuel sufficient with these collection/refill timings.

Harvest ledger through termination: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 05:30 Cave Potato +3; D5 05:40 Cave Potato +3; D5 06:10 Cave Potato +3; D7 05:50 Glowcap +2; D7 06:00 Glowcap +2; D7 08:30 Cave Potato +3; D7 08:40 Cave Potato +3; D7 09:10 Cave Potato +3.

### Seed 8, 4 players, backpacks only, regular care

| Day | Farm Food | Field Food | Food supply − need | Missed meals | Food reserve | Fuel burned | Fuel left incl. unpicked | Signal days | Signal lost | Available Scrap / Organic | Installed packs / plots | Rescue feasibility |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---|:---|:---|
| 1 | 0 | 4 | +8 | 0 | 8 | 0.690 | 6.310 | 0.950 | 0.000 | 10 / 2 | 2 / 4 | Feasible lower bound |
| 2 | 0 | 0 | -4 | 0 | 4 | 0.680 | 5.630 | 1.950 | 0.000 | 10 / 2 | 2 / 4 | Feasible lower bound |
| 3 | 6 | 8 | +10 | 0 | 14 | 0.680 | 4.950 | 2.950 | 0.000 | 10 / 2 | 2 / 4 | Feasible lower bound |
| 4 | 4 | 0 | 0 | 0 | 14 | 0.700 | 4.250 | 3.950 | 0.000 | 2 / 1 | 3 / 4 | Feasible lower bound |
| 5 | 6 | 0 | +2 | 0 | 16 | 0.680 | 3.570 | 4.950 | 0.000 | 2 / 1 | 3 / 4 | Feasible lower bound |
| 6 | 0 | 0 | -4 | 0 | 12 | 0.680 | 2.890 | 5.950 | 0.000 | 2 / 1 | 3 / 4 | Feasible lower bound |
| 7 | 10 | 0 | +6 | 0 | 18 | 0.680 | 2.210 | 6.950 | 0.000 | 2 / 3 | 3 / 4 | Feasible lower bound |

Outcome: Rescued at D8 00:30; signal 7.000, lost 0.000, Fuel left 2.180; total missed meals 0. Fuel sufficient with these collection/refill timings.

Harvest ledger through termination: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 05:30 Cave Potato +3; D5 05:40 Cave Potato +3; D7 05:50 Glowcap +2; D7 06:00 Glowcap +2; D7 08:30 Cave Potato +3; D7 08:40 Cave Potato +3.

### Seed 8, 4 players, one backpack + fifth plot + propagation, regular care

| Day | Farm Food | Field Food | Food supply − need | Missed meals | Food reserve | Fuel burned | Fuel left incl. unpicked | Signal days | Signal lost | Available Scrap / Organic | Installed packs / plots | Rescue feasibility |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---|:---|:---|
| 1 | 0 | 4 | +8 | 0 | 8 | 0.690 | 6.310 | 0.950 | 0.000 | 6 / 1 | 1 / 5 | Feasible lower bound |
| 2 | 0 | 0 | -4 | 0 | 4 | 0.680 | 5.630 | 1.950 | 0.000 | 6 / 1 | 1 / 5 | Feasible lower bound |
| 3 | 6 | 8 | +10 | 0 | 14 | 0.700 | 4.930 | 2.950 | 0.000 | 6 / 0 | 1 / 5 | Feasible lower bound |
| 4 | 4 | 0 | 0 | 0 | 14 | 0.680 | 4.250 | 3.950 | 0.000 | 6 / 2 | 1 / 5 | Feasible lower bound |
| 5 | 9 | 0 | +5 | 0 | 19 | 0.680 | 3.570 | 4.950 | 0.000 | 6 / 2 | 1 / 5 | Feasible lower bound |
| 6 | 0 | 0 | -4 | 0 | 15 | 0.680 | 2.890 | 5.950 | 0.000 | 6 / 2 | 1 / 5 | Feasible lower bound |
| 7 | 13 | 0 | +9 | 0 | 24 | 0.680 | 2.210 | 6.950 | 0.000 | 6 / 4 | 1 / 5 | Feasible lower bound |

Outcome: Rescued at D8 00:30; signal 7.000, lost 0.000, Fuel left 2.180; total missed meals 0. Fuel sufficient with these collection/refill timings.

Harvest ledger through termination: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 05:30 Cave Potato +3; D5 05:40 Cave Potato +3; D5 06:10 Cave Potato +3; D7 05:50 Glowcap +2; D7 06:00 Glowcap +2; D7 08:30 Cave Potato +3; D7 08:40 Cave Potato +3; D7 09:10 Cave Potato +3.

### Seed 8, 6 players, backpacks only, regular care

| Day | Farm Food | Field Food | Food supply − need | Missed meals | Food reserve | Fuel burned | Fuel left incl. unpicked | Signal days | Signal lost | Available Scrap / Organic | Installed packs / plots | Rescue feasibility |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---|:---|:---|
| 1 | 0 | 4 | +10 | 0 | 10 | 0.690 | 6.310 | 0.950 | 0.000 | 10 / 2 | 2 / 4 | Feasible lower bound |
| 2 | 0 | 0 | -6 | 0 | 4 | 0.680 | 5.630 | 1.950 | 0.000 | 10 / 2 | 2 / 4 | Feasible lower bound |
| 3 | 6 | 8 | +8 | 0 | 12 | 0.680 | 4.950 | 2.950 | 0.000 | 10 / 2 | 2 / 4 | Feasible lower bound |
| 4 | 4 | 0 | -2 | 0 | 10 | 0.700 | 4.250 | 3.950 | 0.000 | 2 / 1 | 3 / 4 | Feasible lower bound |
| 5 | 6 | 0 | 0 | 0 | 10 | 0.680 | 3.570 | 4.950 | 0.000 | 2 / 1 | 3 / 4 | Feasible lower bound |
| 6 | 0 | 0 | -6 | 0 | 4 | 0.680 | 2.890 | 5.950 | 0.000 | 2 / 1 | 3 / 4 | Feasible lower bound |
| 7 | 10 | 0 | +4 | 0 | 8 | 0.680 | 2.210 | 6.950 | 0.000 | 2 / 3 | 3 / 4 | Feasible lower bound |

Outcome: Rescued at D8 00:30; signal 7.000, lost 0.000, Fuel left 2.180; total missed meals 0. Fuel sufficient with these collection/refill timings.

Harvest ledger through termination: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 05:30 Cave Potato +3; D5 05:40 Cave Potato +3; D7 05:50 Glowcap +2; D7 06:00 Glowcap +2; D7 08:30 Cave Potato +3; D7 08:40 Cave Potato +3.

### Seed 8, 6 players, one backpack + fifth plot + propagation, regular care

| Day | Farm Food | Field Food | Food supply − need | Missed meals | Food reserve | Fuel burned | Fuel left incl. unpicked | Signal days | Signal lost | Available Scrap / Organic | Installed packs / plots | Rescue feasibility |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---|:---|:---|
| 1 | 0 | 4 | +10 | 0 | 10 | 0.690 | 6.310 | 0.950 | 0.000 | 6 / 1 | 1 / 5 | Feasible lower bound |
| 2 | 0 | 0 | -6 | 0 | 4 | 0.680 | 5.630 | 1.950 | 0.000 | 6 / 1 | 1 / 5 | Feasible lower bound |
| 3 | 6 | 8 | +8 | 0 | 12 | 0.700 | 4.930 | 2.950 | 0.000 | 6 / 0 | 1 / 5 | Feasible lower bound |
| 4 | 4 | 0 | -2 | 0 | 10 | 0.680 | 4.250 | 3.950 | 0.000 | 6 / 2 | 1 / 5 | Feasible lower bound |
| 5 | 9 | 0 | +3 | 0 | 13 | 0.680 | 3.570 | 4.950 | 0.000 | 6 / 2 | 1 / 5 | Feasible lower bound |
| 6 | 0 | 0 | -6 | 0 | 7 | 0.680 | 2.890 | 5.950 | 0.000 | 6 / 2 | 1 / 5 | Feasible lower bound |
| 7 | 13 | 0 | +7 | 0 | 14 | 0.680 | 2.210 | 6.950 | 0.000 | 6 / 4 | 1 / 5 | Feasible lower bound |

Outcome: Rescued at D8 00:30; signal 7.000, lost 0.000, Fuel left 2.180; total missed meals 0. Fuel sufficient with these collection/refill timings.

Harvest ledger through termination: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 05:30 Cave Potato +3; D5 05:40 Cave Potato +3; D5 06:10 Cave Potato +3; D7 05:50 Glowcap +2; D7 06:00 Glowcap +2; D7 08:30 Cave Potato +3; D7 08:40 Cave Potato +3; D7 09:10 Cave Potato +3.

### Seed 499, 2 players, backpacks only, delayed care

| Day | Farm Food | Field Food | Food supply − need | Missed meals | Food reserve | Fuel burned | Fuel left incl. unpicked | Signal days | Signal lost | Available Scrap / Organic | Installed packs / plots | Rescue feasibility |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---|:---|:---|
| 1 | 0 | 3 | +5 | 0 | 5 | 0.690 | 7.310 | 0.950 | 0.000 | 10 / 2 | 2 / 4 | Feasible lower bound |
| 2 | 0 | 0 | -2 | 0 | 3 | 0.680 | 6.630 | 1.950 | 0.000 | 10 / 2 | 2 / 4 | Feasible lower bound |
| 3 | 6 | 5 | +9 | 0 | 12 | 0.680 | 5.950 | 2.950 | 0.000 | 10 / 2 | 2 / 4 | Feasible lower bound |
| 4 | 4 | 0 | +2 | 0 | 14 | 0.680 | 5.270 | 3.950 | 0.000 | 10 / 4 | 2 / 4 | Feasible lower bound |
| 5 | 6 | 0 | +4 | 0 | 18 | 0.680 | 4.590 | 4.950 | 0.000 | 10 / 4 | 2 / 4 | Feasible lower bound |
| 6 | 0 | 0 | -2 | 0 | 16 | 0.680 | 3.910 | 5.950 | 0.000 | 10 / 4 | 2 / 4 | Feasible lower bound |
| 7 | 4 | 0 | +2 | 0 | 18 | 0.680 | 3.230 | 6.950 | 0.000 | 10 / 6 | 2 / 4 | Feasible lower bound |

Outcome: Rescued at D8 00:30; signal 7.000, lost 0.000, Fuel left 3.200; total missed meals 0. Fuel sufficient with these collection/refill timings.

Harvest ledger through termination: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 08:30 Cave Potato +3; D5 08:40 Cave Potato +3; D7 08:50 Glowcap +2; D7 09:00 Glowcap +2.

### Seed 499, 2 players, one backpack + fifth plot + propagation, delayed care

| Day | Farm Food | Field Food | Food supply − need | Missed meals | Food reserve | Fuel burned | Fuel left incl. unpicked | Signal days | Signal lost | Available Scrap / Organic | Installed packs / plots | Rescue feasibility |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---|:---|:---|
| 1 | 0 | 3 | +5 | 0 | 5 | 0.690 | 7.310 | 0.950 | 0.000 | 6 / 1 | 1 / 5 | Feasible lower bound |
| 2 | 0 | 0 | -2 | 0 | 3 | 0.680 | 6.630 | 1.950 | 0.000 | 6 / 1 | 1 / 5 | Feasible lower bound |
| 3 | 6 | 5 | +9 | 0 | 12 | 0.700 | 5.930 | 2.950 | 0.000 | 6 / 0 | 1 / 5 | Feasible lower bound |
| 4 | 4 | 0 | +2 | 0 | 14 | 0.680 | 5.250 | 3.950 | 0.000 | 6 / 2 | 1 / 5 | Feasible lower bound |
| 5 | 9 | 0 | +7 | 0 | 21 | 0.680 | 4.570 | 4.950 | 0.000 | 6 / 2 | 1 / 5 | Feasible lower bound |
| 6 | 0 | 0 | -2 | 0 | 19 | 0.680 | 3.890 | 5.950 | 0.000 | 6 / 2 | 1 / 5 | Feasible lower bound |
| 7 | 4 | 0 | +2 | 0 | 21 | 0.680 | 3.210 | 6.950 | 0.000 | 6 / 4 | 1 / 5 | Feasible lower bound |

Outcome: Rescued at D8 00:30; signal 7.000, lost 0.000, Fuel left 3.180; total missed meals 0. Fuel sufficient with these collection/refill timings.

Harvest ledger through termination: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 08:30 Cave Potato +3; D5 08:40 Cave Potato +3; D5 09:10 Cave Potato +3; D7 08:50 Glowcap +2; D7 09:00 Glowcap +2.

### Seed 499, 4 players, backpacks only, delayed care

| Day | Farm Food | Field Food | Food supply − need | Missed meals | Food reserve | Fuel burned | Fuel left incl. unpicked | Signal days | Signal lost | Available Scrap / Organic | Installed packs / plots | Rescue feasibility |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---|:---|:---|
| 1 | 0 | 3 | +7 | 0 | 7 | 0.690 | 7.310 | 0.950 | 0.000 | 10 / 2 | 2 / 4 | Feasible lower bound |
| 2 | 0 | 0 | -4 | 0 | 3 | 0.680 | 6.630 | 1.950 | 0.000 | 10 / 2 | 2 / 4 | Feasible lower bound |
| 3 | 6 | 5 | +7 | 0 | 10 | 0.680 | 5.950 | 2.950 | 0.000 | 10 / 2 | 2 / 4 | Feasible lower bound |
| 4 | 4 | 0 | 0 | 0 | 10 | 0.700 | 5.250 | 3.950 | 0.000 | 2 / 1 | 3 / 4 | Feasible lower bound |
| 5 | 6 | 0 | +2 | 0 | 12 | 0.680 | 4.570 | 4.950 | 0.000 | 2 / 1 | 3 / 4 | Feasible lower bound |
| 6 | 0 | 0 | -4 | 0 | 8 | 0.680 | 3.890 | 5.950 | 0.000 | 2 / 1 | 3 / 4 | Feasible lower bound |
| 7 | 4 | 0 | 0 | 0 | 8 | 0.680 | 3.210 | 6.950 | 0.000 | 2 / 3 | 3 / 4 | Feasible lower bound |

Outcome: Rescued at D8 00:30; signal 7.000, lost 0.000, Fuel left 3.180; total missed meals 0. Fuel sufficient with these collection/refill timings.

Harvest ledger through termination: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 08:30 Cave Potato +3; D5 08:40 Cave Potato +3; D7 08:50 Glowcap +2; D7 09:00 Glowcap +2.

### Seed 499, 4 players, one backpack + fifth plot + propagation, delayed care

| Day | Farm Food | Field Food | Food supply − need | Missed meals | Food reserve | Fuel burned | Fuel left incl. unpicked | Signal days | Signal lost | Available Scrap / Organic | Installed packs / plots | Rescue feasibility |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---|:---|:---|
| 1 | 0 | 3 | +7 | 0 | 7 | 0.690 | 7.310 | 0.950 | 0.000 | 6 / 1 | 1 / 5 | Feasible lower bound |
| 2 | 0 | 0 | -4 | 0 | 3 | 0.680 | 6.630 | 1.950 | 0.000 | 6 / 1 | 1 / 5 | Feasible lower bound |
| 3 | 6 | 5 | +7 | 0 | 10 | 0.700 | 5.930 | 2.950 | 0.000 | 6 / 0 | 1 / 5 | Feasible lower bound |
| 4 | 4 | 0 | 0 | 0 | 10 | 0.680 | 5.250 | 3.950 | 0.000 | 6 / 2 | 1 / 5 | Feasible lower bound |
| 5 | 9 | 0 | +5 | 0 | 15 | 0.680 | 4.570 | 4.950 | 0.000 | 6 / 2 | 1 / 5 | Feasible lower bound |
| 6 | 0 | 0 | -4 | 0 | 11 | 0.680 | 3.890 | 5.950 | 0.000 | 6 / 2 | 1 / 5 | Feasible lower bound |
| 7 | 4 | 0 | 0 | 0 | 11 | 0.680 | 3.210 | 6.950 | 0.000 | 6 / 4 | 1 / 5 | Feasible lower bound |

Outcome: Rescued at D8 00:30; signal 7.000, lost 0.000, Fuel left 3.180; total missed meals 0. Fuel sufficient with these collection/refill timings.

Harvest ledger through termination: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 08:30 Cave Potato +3; D5 08:40 Cave Potato +3; D5 09:10 Cave Potato +3; D7 08:50 Glowcap +2; D7 09:00 Glowcap +2.

### Seed 499, 6 players, backpacks only, delayed care

| Day | Farm Food | Field Food | Food supply − need | Missed meals | Food reserve | Fuel burned | Fuel left incl. unpicked | Signal days | Signal lost | Available Scrap / Organic | Installed packs / plots | Rescue feasibility |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---|:---|:---|
| 1 | 0 | 3 | +9 | 0 | 9 | 0.690 | 7.310 | 0.950 | 0.000 | 10 / 2 | 2 / 4 | Feasible lower bound |
| 2 | 0 | 0 | -6 | 0 | 3 | 0.680 | 6.630 | 1.950 | 0.000 | 10 / 2 | 2 / 4 | Feasible lower bound |
| 3 | 6 | 5 | +5 | 0 | 8 | 0.680 | 5.950 | 2.950 | 0.000 | 10 / 2 | 2 / 4 | Feasible lower bound |
| 4 | 4 | 0 | -2 | 0 | 6 | 0.700 | 5.250 | 3.950 | 0.000 | 2 / 1 | 3 / 4 | Feasible lower bound |
| 5 | 6 | 0 | 0 | 0 | 6 | 0.680 | 4.570 | 4.950 | 0.000 | 2 / 1 | 3 / 4 | Feasible lower bound |
| 6 | 0 | 0 | -6 | 0 | 0 | 0.680 | 3.890 | 5.950 | 0.000 | 2 / 1 | 3 / 4 | Feasible lower bound |
| 7 | 4 | 0 | -2 | 2 | 0 | 0.680 | 3.210 | 6.950 | 0.000 | 2 / 3 | 3 / 4 | Feasible lower bound |

Outcome: Rescued at D8 00:30; signal 7.000, lost 0.000, Fuel left 3.180; total missed meals 2. Fuel sufficient with these collection/refill timings.

Harvest ledger through termination: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 08:30 Cave Potato +3; D5 08:40 Cave Potato +3; D7 08:50 Glowcap +2; D7 09:00 Glowcap +2.

### Seed 499, 6 players, one backpack + fifth plot + propagation, delayed care

| Day | Farm Food | Field Food | Food supply − need | Missed meals | Food reserve | Fuel burned | Fuel left incl. unpicked | Signal days | Signal lost | Available Scrap / Organic | Installed packs / plots | Rescue feasibility |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---|:---|:---|
| 1 | 0 | 3 | +9 | 0 | 9 | 0.690 | 7.310 | 0.950 | 0.000 | 6 / 1 | 1 / 5 | Feasible lower bound |
| 2 | 0 | 0 | -6 | 0 | 3 | 0.680 | 6.630 | 1.950 | 0.000 | 6 / 1 | 1 / 5 | Feasible lower bound |
| 3 | 6 | 5 | +5 | 0 | 8 | 0.700 | 5.930 | 2.950 | 0.000 | 6 / 0 | 1 / 5 | Feasible lower bound |
| 4 | 4 | 0 | -2 | 0 | 6 | 0.680 | 5.250 | 3.950 | 0.000 | 6 / 2 | 1 / 5 | Feasible lower bound |
| 5 | 9 | 0 | +3 | 0 | 9 | 0.680 | 4.570 | 4.950 | 0.000 | 6 / 2 | 1 / 5 | Feasible lower bound |
| 6 | 0 | 0 | -6 | 0 | 3 | 0.680 | 3.890 | 5.950 | 0.000 | 6 / 2 | 1 / 5 | Feasible lower bound |
| 7 | 4 | 0 | -2 | 0 | 1 | 0.680 | 3.210 | 6.950 | 0.000 | 6 / 4 | 1 / 5 | Feasible lower bound |

Outcome: Rescued at D8 00:30; signal 7.000, lost 0.000, Fuel left 3.180; total missed meals 0. Fuel sufficient with these collection/refill timings.

Harvest ledger through termination: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 08:30 Cave Potato +3; D5 08:40 Cave Potato +3; D5 09:10 Cave Potato +3; D7 08:50 Glowcap +2; D7 09:00 Glowcap +2.

## Findings

The revised costs permit one backpack and farming together, but not two backpacks and a farm expansion. Day-1 Organic pays exactly for the mixed choice. Maximum backpack-only counts are 2/3/3 for groups of 2/4/6; the third uses Organic from a Glowcap harvest. No stock scales with players. A fifth potato planted D3 04:00 yields six extra Food by Day 7 with regular care, or three with delayed care. In the minimum Food world, six actors with delayed care change from two missed meals to none and one Food left. Original crop yields and growth are unchanged. Food surpluses for smaller groups make carrying capacity a more plausible alternative, although the compact map limits the strength of the backpack benefit.

All tested policies reach rescue. Fuel remains finite: unnecessary full-load operation and outages retain the M4b unreachable-rescue cases. Crafting does not introduce extra Fuel or an instant Fuel fee; thirty powered seconds cost 0.02 extra Fuel at the existing load formula. These simulations do not establish human group balance or physical/network multiplayer.
