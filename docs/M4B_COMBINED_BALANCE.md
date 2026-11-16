# M4b combined Food / Fuel / rescue ledger

Reproduce: `dotnet run --project tests/Hollowlight.Core.Tests.csproj -- --rescue-balance docs/M4B_COMBINED_BALANCE.md`

Actual seeded Food and Fuel worlds, unchanged four plots, personal meals, manual transfers, exact generator accounting and rescue authority share one clock. Two starting Food per actor; shared seeds 2+2 and Fuel 2. Rows settle each completed day at dawn. Food flow includes starters on Day 1; reserves cover negative flows until exhausted. Fuel left includes unpicked caches, so feasibility is optimistic, not a promise that carried Fuel reaches the generator in time.

Farm: seeds at 00:20, planting 01:00/01:10/01:20/01:30; daily visits 03:00/05:30/08:30 (delayed omits 05:30), ten seconds per plot and replant five seconds later. Deposit yields immediately, resupply one minute after visits in rotating fair rounds toward two personal Food. Near Food returns D1 02:00, far D3 05:00; near Fuel D2 03:00, far D4 03:00. Gather thirty to sixty seconds before returns. Operator visits/refills daily 03:00 and on cache deliveries; initial transmission starts D1 00:30. These scripted cooperation schedules are not measured travel or networked playtests.

Scheduled load: beacon continuously, workbench seconds 30–60 daily, lighting 420–600. Full load: beacon + workbench continuously. Startup delay means seven transmission days finish after seven calendar days. Simulations continue to victory or Day 14; terminal freeze stops consumption exactly at victory. Outage stress stops the generator from elapsed 15:00–40:00 while retaining consumer selections; manual refills during the outage do not restart it. Wasteful case runs workbench alone for five days before attempting transmission; it illustrates finite-Fuel failure without changing supplies. Feasibility compares remaining Fuel with the optimistic 0.6 Fuel per remaining signal day (no future losses/idle/extras).

### Seed 8, 2 players, scheduled, regular care

Finite Food 12, surface Fuel 5 + 2 starter.

| Day | Farm | Field Food | Food supply − need | Missed meals | Food reserve | Fuel burned | Fuel left incl. unpicked | Signal days | Signal lost | Rescue feasibility |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---|
| 1 | 0 | 4 | +6 | 0 | 6 | 0.650 | 6.350 | 0.950 | 0.000 | Feasible lower bound |
| 2 | 0 | 0 | -2 | 0 | 4 | 0.680 | 5.670 | 1.950 | 0.000 | Feasible lower bound |
| 3 | 6 | 8 | +12 | 0 | 16 | 0.680 | 4.990 | 2.950 | 0.000 | Feasible lower bound |
| 4 | 4 | 0 | +2 | 0 | 18 | 0.680 | 4.310 | 3.950 | 0.000 | Feasible lower bound |
| 5 | 6 | 0 | +4 | 0 | 22 | 0.680 | 3.630 | 4.950 | 0.000 | Feasible lower bound |
| 6 | 0 | 0 | -2 | 0 | 20 | 0.680 | 2.950 | 5.950 | 0.000 | Feasible lower bound |
| 7 | 10 | 0 | +8 | 0 | 28 | 0.680 | 2.270 | 6.950 | 0.000 | Feasible lower bound |

Outcome: Rescued at D8 01:30; signal 7.000, lost 0.000, Fuel left 2.240; total missed meals 0. Fuel sufficient with these collection/refill timings.

Harvest ledger through termination: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 05:30 Cave Potato +3; D5 05:40 Cave Potato +3; D7 05:50 Glowcap +2; D7 06:00 Glowcap +2; D7 08:30 Cave Potato +3; D7 08:40 Cave Potato +3.

### Seed 8, 2 players, continuous full load

Finite Food 12, surface Fuel 5 + 2 starter.

| Day | Farm | Field Food | Food supply − need | Missed meals | Food reserve | Fuel burned | Fuel left incl. unpicked | Signal days | Signal lost | Rescue feasibility |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---|
| 1 | 0 | 4 | +6 | 0 | 6 | 0.950 | 6.050 | 0.950 | 0.000 | Feasible lower bound |
| 2 | 0 | 0 | -2 | 0 | 4 | 1.000 | 5.050 | 1.950 | 0.000 | Feasible lower bound |
| 3 | 6 | 8 | +12 | 0 | 16 | 1.000 | 4.050 | 2.950 | 0.000 | Feasible lower bound |
| 4 | 4 | 0 | +2 | 0 | 18 | 1.000 | 3.050 | 3.950 | 0.000 | Feasible lower bound |
| 5 | 6 | 0 | +4 | 0 | 22 | 1.000 | 2.050 | 4.950 | 0.000 | Feasible lower bound |
| 6 | 0 | 0 | -2 | 0 | 20 | 1.000 | 1.050 | 5.950 | 0.000 | Feasible lower bound |
| 7 | 10 | 0 | +8 | 0 | 28 | 1.000 | 0.050 | 6.950 | 0.000 | Feasible lower bound |

Outcome: Rescued at D8 01:30; signal 7.000, lost 0.000, Fuel left 0.000; total missed meals 0. Fuel sufficient with these collection/refill timings.

Harvest ledger through termination: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 05:30 Cave Potato +3; D5 05:40 Cave Potato +3; D7 05:50 Glowcap +2; D7 06:00 Glowcap +2; D7 08:30 Cave Potato +3; D7 08:40 Cave Potato +3.

### Seed 8, 2 players, full load, 2.5-day outage

Finite Food 12, surface Fuel 5 + 2 starter.

| Day | Farm | Field Food | Food supply − need | Missed meals | Food reserve | Fuel burned | Fuel left incl. unpicked | Signal days | Signal lost | Rescue feasibility |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---|
| 1 | 0 | 4 | +6 | 0 | 6 | 0.950 | 6.050 | 0.950 | 0.000 | Feasible lower bound |
| 2 | 0 | 0 | -2 | 0 | 4 | 0.500 | 5.550 | 1.388 | 0.063 | Feasible lower bound |
| 3 | 6 | 8 | +12 | 0 | 16 | 0.000 | 5.550 | 1.138 | 0.313 | Feasible lower bound |
| 4 | 4 | 0 | +2 | 0 | 18 | 0.000 | 5.550 | 0.888 | 0.563 | Feasible lower bound |
| 5 | 6 | 0 | +4 | 0 | 22 | 1.000 | 4.550 | 1.888 | 0.563 | Feasible lower bound |
| 6 | 0 | 0 | -2 | 0 | 20 | 1.000 | 3.550 | 2.888 | 0.563 | Feasible lower bound |
| 7 | 10 | 0 | +8 | 0 | 28 | 1.000 | 2.550 | 3.888 | 0.563 | Feasible lower bound |

Outcome: No rescue by end of Day 14; signal 5.438, lost 1.563, Fuel left 0.000; total missed meals 0. Finite Fuel falls below even beacon-only completion cost: unreachable without a new run or future supplies.

Harvest ledger through termination: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 05:30 Cave Potato +3; D5 05:40 Cave Potato +3; D7 05:50 Glowcap +2; D7 06:00 Glowcap +2; D7 08:30 Cave Potato +3; D7 08:40 Cave Potato +3; D10 03:00 Cave Potato +3; D10 03:10 Cave Potato +3; D10 08:50 Glowcap +2; D10 09:00 Glowcap +2; D12 05:30 Cave Potato +3; D12 05:40 Cave Potato +3; D14 03:20 Glowcap +2; D14 03:30 Glowcap +2; D14 08:30 Cave Potato +3; D14 08:40 Cave Potato +3.

### Seed 8, 4 players, scheduled, regular care

Finite Food 12, surface Fuel 5 + 2 starter.

| Day | Farm | Field Food | Food supply − need | Missed meals | Food reserve | Fuel burned | Fuel left incl. unpicked | Signal days | Signal lost | Rescue feasibility |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---|
| 1 | 0 | 4 | +8 | 0 | 8 | 0.650 | 6.350 | 0.950 | 0.000 | Feasible lower bound |
| 2 | 0 | 0 | -4 | 0 | 4 | 0.680 | 5.670 | 1.950 | 0.000 | Feasible lower bound |
| 3 | 6 | 8 | +10 | 0 | 14 | 0.680 | 4.990 | 2.950 | 0.000 | Feasible lower bound |
| 4 | 4 | 0 | 0 | 0 | 14 | 0.680 | 4.310 | 3.950 | 0.000 | Feasible lower bound |
| 5 | 6 | 0 | +2 | 0 | 16 | 0.680 | 3.630 | 4.950 | 0.000 | Feasible lower bound |
| 6 | 0 | 0 | -4 | 0 | 12 | 0.680 | 2.950 | 5.950 | 0.000 | Feasible lower bound |
| 7 | 10 | 0 | +6 | 0 | 18 | 0.680 | 2.270 | 6.950 | 0.000 | Feasible lower bound |

Outcome: Rescued at D8 01:30; signal 7.000, lost 0.000, Fuel left 2.240; total missed meals 0. Fuel sufficient with these collection/refill timings.

Harvest ledger through termination: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 05:30 Cave Potato +3; D5 05:40 Cave Potato +3; D7 05:50 Glowcap +2; D7 06:00 Glowcap +2; D7 08:30 Cave Potato +3; D7 08:40 Cave Potato +3.

### Seed 8, 4 players, continuous full load

Finite Food 12, surface Fuel 5 + 2 starter.

| Day | Farm | Field Food | Food supply − need | Missed meals | Food reserve | Fuel burned | Fuel left incl. unpicked | Signal days | Signal lost | Rescue feasibility |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---|
| 1 | 0 | 4 | +8 | 0 | 8 | 0.950 | 6.050 | 0.950 | 0.000 | Feasible lower bound |
| 2 | 0 | 0 | -4 | 0 | 4 | 1.000 | 5.050 | 1.950 | 0.000 | Feasible lower bound |
| 3 | 6 | 8 | +10 | 0 | 14 | 1.000 | 4.050 | 2.950 | 0.000 | Feasible lower bound |
| 4 | 4 | 0 | 0 | 0 | 14 | 1.000 | 3.050 | 3.950 | 0.000 | Feasible lower bound |
| 5 | 6 | 0 | +2 | 0 | 16 | 1.000 | 2.050 | 4.950 | 0.000 | Feasible lower bound |
| 6 | 0 | 0 | -4 | 0 | 12 | 1.000 | 1.050 | 5.950 | 0.000 | Feasible lower bound |
| 7 | 10 | 0 | +6 | 0 | 18 | 1.000 | 0.050 | 6.950 | 0.000 | Feasible lower bound |

Outcome: Rescued at D8 01:30; signal 7.000, lost 0.000, Fuel left 0.000; total missed meals 0. Fuel sufficient with these collection/refill timings.

Harvest ledger through termination: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 05:30 Cave Potato +3; D5 05:40 Cave Potato +3; D7 05:50 Glowcap +2; D7 06:00 Glowcap +2; D7 08:30 Cave Potato +3; D7 08:40 Cave Potato +3.

### Seed 8, 4 players, full load, 2.5-day outage

Finite Food 12, surface Fuel 5 + 2 starter.

| Day | Farm | Field Food | Food supply − need | Missed meals | Food reserve | Fuel burned | Fuel left incl. unpicked | Signal days | Signal lost | Rescue feasibility |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---|
| 1 | 0 | 4 | +8 | 0 | 8 | 0.950 | 6.050 | 0.950 | 0.000 | Feasible lower bound |
| 2 | 0 | 0 | -4 | 0 | 4 | 0.500 | 5.550 | 1.388 | 0.063 | Feasible lower bound |
| 3 | 6 | 8 | +10 | 0 | 14 | 0.000 | 5.550 | 1.138 | 0.313 | Feasible lower bound |
| 4 | 4 | 0 | 0 | 0 | 14 | 0.000 | 5.550 | 0.888 | 0.563 | Feasible lower bound |
| 5 | 6 | 0 | +2 | 0 | 16 | 1.000 | 4.550 | 1.888 | 0.563 | Feasible lower bound |
| 6 | 0 | 0 | -4 | 0 | 12 | 1.000 | 3.550 | 2.888 | 0.563 | Feasible lower bound |
| 7 | 10 | 0 | +6 | 0 | 18 | 1.000 | 2.550 | 3.888 | 0.563 | Feasible lower bound |

Outcome: No rescue by end of Day 14; signal 5.438, lost 1.563, Fuel left 0.000; total missed meals 0. Finite Fuel falls below even beacon-only completion cost: unreachable without a new run or future supplies.

Harvest ledger through termination: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 05:30 Cave Potato +3; D5 05:40 Cave Potato +3; D7 05:50 Glowcap +2; D7 06:00 Glowcap +2; D7 08:30 Cave Potato +3; D7 08:40 Cave Potato +3; D10 03:00 Cave Potato +3; D10 03:10 Cave Potato +3; D10 08:50 Glowcap +2; D10 09:00 Glowcap +2; D12 05:30 Cave Potato +3; D12 05:40 Cave Potato +3; D14 03:20 Glowcap +2; D14 03:30 Glowcap +2; D14 08:30 Cave Potato +3; D14 08:40 Cave Potato +3.

### Seed 8, 6 players, scheduled, regular care

Finite Food 12, surface Fuel 5 + 2 starter.

| Day | Farm | Field Food | Food supply − need | Missed meals | Food reserve | Fuel burned | Fuel left incl. unpicked | Signal days | Signal lost | Rescue feasibility |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---|
| 1 | 0 | 4 | +10 | 0 | 10 | 0.650 | 6.350 | 0.950 | 0.000 | Feasible lower bound |
| 2 | 0 | 0 | -6 | 0 | 4 | 0.680 | 5.670 | 1.950 | 0.000 | Feasible lower bound |
| 3 | 6 | 8 | +8 | 0 | 12 | 0.680 | 4.990 | 2.950 | 0.000 | Feasible lower bound |
| 4 | 4 | 0 | -2 | 0 | 10 | 0.680 | 4.310 | 3.950 | 0.000 | Feasible lower bound |
| 5 | 6 | 0 | 0 | 0 | 10 | 0.680 | 3.630 | 4.950 | 0.000 | Feasible lower bound |
| 6 | 0 | 0 | -6 | 0 | 4 | 0.680 | 2.950 | 5.950 | 0.000 | Feasible lower bound |
| 7 | 10 | 0 | +4 | 0 | 8 | 0.680 | 2.270 | 6.950 | 0.000 | Feasible lower bound |

Outcome: Rescued at D8 01:30; signal 7.000, lost 0.000, Fuel left 2.240; total missed meals 0. Fuel sufficient with these collection/refill timings.

Harvest ledger through termination: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 05:30 Cave Potato +3; D5 05:40 Cave Potato +3; D7 05:50 Glowcap +2; D7 06:00 Glowcap +2; D7 08:30 Cave Potato +3; D7 08:40 Cave Potato +3.

### Seed 8, 6 players, continuous full load

Finite Food 12, surface Fuel 5 + 2 starter.

| Day | Farm | Field Food | Food supply − need | Missed meals | Food reserve | Fuel burned | Fuel left incl. unpicked | Signal days | Signal lost | Rescue feasibility |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---|
| 1 | 0 | 4 | +10 | 0 | 10 | 0.950 | 6.050 | 0.950 | 0.000 | Feasible lower bound |
| 2 | 0 | 0 | -6 | 0 | 4 | 1.000 | 5.050 | 1.950 | 0.000 | Feasible lower bound |
| 3 | 6 | 8 | +8 | 0 | 12 | 1.000 | 4.050 | 2.950 | 0.000 | Feasible lower bound |
| 4 | 4 | 0 | -2 | 0 | 10 | 1.000 | 3.050 | 3.950 | 0.000 | Feasible lower bound |
| 5 | 6 | 0 | 0 | 0 | 10 | 1.000 | 2.050 | 4.950 | 0.000 | Feasible lower bound |
| 6 | 0 | 0 | -6 | 0 | 4 | 1.000 | 1.050 | 5.950 | 0.000 | Feasible lower bound |
| 7 | 10 | 0 | +4 | 0 | 8 | 1.000 | 0.050 | 6.950 | 0.000 | Feasible lower bound |

Outcome: Rescued at D8 01:30; signal 7.000, lost 0.000, Fuel left 0.000; total missed meals 0. Fuel sufficient with these collection/refill timings.

Harvest ledger through termination: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 05:30 Cave Potato +3; D5 05:40 Cave Potato +3; D7 05:50 Glowcap +2; D7 06:00 Glowcap +2; D7 08:30 Cave Potato +3; D7 08:40 Cave Potato +3.

### Seed 8, 6 players, full load, 2.5-day outage

Finite Food 12, surface Fuel 5 + 2 starter.

| Day | Farm | Field Food | Food supply − need | Missed meals | Food reserve | Fuel burned | Fuel left incl. unpicked | Signal days | Signal lost | Rescue feasibility |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---|
| 1 | 0 | 4 | +10 | 0 | 10 | 0.950 | 6.050 | 0.950 | 0.000 | Feasible lower bound |
| 2 | 0 | 0 | -6 | 0 | 4 | 0.500 | 5.550 | 1.388 | 0.063 | Feasible lower bound |
| 3 | 6 | 8 | +8 | 0 | 12 | 0.000 | 5.550 | 1.138 | 0.313 | Feasible lower bound |
| 4 | 4 | 0 | -2 | 0 | 10 | 0.000 | 5.550 | 0.888 | 0.563 | Feasible lower bound |
| 5 | 6 | 0 | 0 | 0 | 10 | 1.000 | 4.550 | 1.888 | 0.563 | Feasible lower bound |
| 6 | 0 | 0 | -6 | 0 | 4 | 1.000 | 3.550 | 2.888 | 0.563 | Feasible lower bound |
| 7 | 10 | 0 | +4 | 0 | 8 | 1.000 | 2.550 | 3.888 | 0.563 | Feasible lower bound |

Outcome: No rescue by end of Day 14; signal 5.438, lost 1.563, Fuel left 0.000; total missed meals 12. Finite Fuel falls below even beacon-only completion cost: unreachable without a new run or future supplies.

Harvest ledger through termination: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 05:30 Cave Potato +3; D5 05:40 Cave Potato +3; D7 05:50 Glowcap +2; D7 06:00 Glowcap +2; D7 08:30 Cave Potato +3; D7 08:40 Cave Potato +3; D10 03:00 Cave Potato +3; D10 03:10 Cave Potato +3; D10 08:50 Glowcap +2; D10 09:00 Glowcap +2; D12 05:30 Cave Potato +3; D12 05:40 Cave Potato +3; D14 03:20 Glowcap +2; D14 03:30 Glowcap +2; D14 08:30 Cave Potato +3; D14 08:40 Cave Potato +3.

### Seed 1701, 2 players, scheduled, regular care

Finite Food 14, surface Fuel 6 + 2 starter.

| Day | Farm | Field Food | Food supply − need | Missed meals | Food reserve | Fuel burned | Fuel left incl. unpicked | Signal days | Signal lost | Rescue feasibility |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---|
| 1 | 0 | 4 | +6 | 0 | 6 | 0.650 | 7.350 | 0.950 | 0.000 | Feasible lower bound |
| 2 | 0 | 0 | -2 | 0 | 4 | 0.680 | 6.670 | 1.950 | 0.000 | Feasible lower bound |
| 3 | 6 | 10 | +14 | 0 | 18 | 0.680 | 5.990 | 2.950 | 0.000 | Feasible lower bound |
| 4 | 4 | 0 | +2 | 0 | 20 | 0.680 | 5.310 | 3.950 | 0.000 | Feasible lower bound |
| 5 | 6 | 0 | +4 | 0 | 24 | 0.680 | 4.630 | 4.950 | 0.000 | Feasible lower bound |
| 6 | 0 | 0 | -2 | 0 | 22 | 0.680 | 3.950 | 5.950 | 0.000 | Feasible lower bound |
| 7 | 10 | 0 | +8 | 0 | 30 | 0.680 | 3.270 | 6.950 | 0.000 | Feasible lower bound |

Outcome: Rescued at D8 01:30; signal 7.000, lost 0.000, Fuel left 3.240; total missed meals 0. Fuel sufficient with these collection/refill timings.

Harvest ledger through termination: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 05:30 Cave Potato +3; D5 05:40 Cave Potato +3; D7 05:50 Glowcap +2; D7 06:00 Glowcap +2; D7 08:30 Cave Potato +3; D7 08:40 Cave Potato +3.

### Seed 1701, 4 players, scheduled, regular care

Finite Food 14, surface Fuel 6 + 2 starter.

| Day | Farm | Field Food | Food supply − need | Missed meals | Food reserve | Fuel burned | Fuel left incl. unpicked | Signal days | Signal lost | Rescue feasibility |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---|
| 1 | 0 | 4 | +8 | 0 | 8 | 0.650 | 7.350 | 0.950 | 0.000 | Feasible lower bound |
| 2 | 0 | 0 | -4 | 0 | 4 | 0.680 | 6.670 | 1.950 | 0.000 | Feasible lower bound |
| 3 | 6 | 10 | +12 | 0 | 16 | 0.680 | 5.990 | 2.950 | 0.000 | Feasible lower bound |
| 4 | 4 | 0 | 0 | 0 | 16 | 0.680 | 5.310 | 3.950 | 0.000 | Feasible lower bound |
| 5 | 6 | 0 | +2 | 0 | 18 | 0.680 | 4.630 | 4.950 | 0.000 | Feasible lower bound |
| 6 | 0 | 0 | -4 | 0 | 14 | 0.680 | 3.950 | 5.950 | 0.000 | Feasible lower bound |
| 7 | 10 | 0 | +6 | 0 | 20 | 0.680 | 3.270 | 6.950 | 0.000 | Feasible lower bound |

Outcome: Rescued at D8 01:30; signal 7.000, lost 0.000, Fuel left 3.240; total missed meals 0. Fuel sufficient with these collection/refill timings.

Harvest ledger through termination: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 05:30 Cave Potato +3; D5 05:40 Cave Potato +3; D7 05:50 Glowcap +2; D7 06:00 Glowcap +2; D7 08:30 Cave Potato +3; D7 08:40 Cave Potato +3.

### Seed 1701, 6 players, scheduled, regular care

Finite Food 14, surface Fuel 6 + 2 starter.

| Day | Farm | Field Food | Food supply − need | Missed meals | Food reserve | Fuel burned | Fuel left incl. unpicked | Signal days | Signal lost | Rescue feasibility |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---|
| 1 | 0 | 4 | +10 | 0 | 10 | 0.650 | 7.350 | 0.950 | 0.000 | Feasible lower bound |
| 2 | 0 | 0 | -6 | 0 | 4 | 0.680 | 6.670 | 1.950 | 0.000 | Feasible lower bound |
| 3 | 6 | 10 | +10 | 0 | 14 | 0.680 | 5.990 | 2.950 | 0.000 | Feasible lower bound |
| 4 | 4 | 0 | -2 | 0 | 12 | 0.680 | 5.310 | 3.950 | 0.000 | Feasible lower bound |
| 5 | 6 | 0 | 0 | 0 | 12 | 0.680 | 4.630 | 4.950 | 0.000 | Feasible lower bound |
| 6 | 0 | 0 | -6 | 0 | 6 | 0.680 | 3.950 | 5.950 | 0.000 | Feasible lower bound |
| 7 | 10 | 0 | +4 | 0 | 10 | 0.680 | 3.270 | 6.950 | 0.000 | Feasible lower bound |

Outcome: Rescued at D8 01:30; signal 7.000, lost 0.000, Fuel left 3.240; total missed meals 0. Fuel sufficient with these collection/refill timings.

Harvest ledger through termination: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 05:30 Cave Potato +3; D5 05:40 Cave Potato +3; D7 05:50 Glowcap +2; D7 06:00 Glowcap +2; D7 08:30 Cave Potato +3; D7 08:40 Cave Potato +3.

### Seed 499, 2 players, scheduled, regular care

Finite Food 8, surface Fuel 6 + 2 starter.

| Day | Farm | Field Food | Food supply − need | Missed meals | Food reserve | Fuel burned | Fuel left incl. unpicked | Signal days | Signal lost | Rescue feasibility |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---|
| 1 | 0 | 3 | +5 | 0 | 5 | 0.650 | 7.350 | 0.950 | 0.000 | Feasible lower bound |
| 2 | 0 | 0 | -2 | 0 | 3 | 0.680 | 6.670 | 1.950 | 0.000 | Feasible lower bound |
| 3 | 6 | 5 | +9 | 0 | 12 | 0.680 | 5.990 | 2.950 | 0.000 | Feasible lower bound |
| 4 | 4 | 0 | +2 | 0 | 14 | 0.680 | 5.310 | 3.950 | 0.000 | Feasible lower bound |
| 5 | 6 | 0 | +4 | 0 | 18 | 0.680 | 4.630 | 4.950 | 0.000 | Feasible lower bound |
| 6 | 0 | 0 | -2 | 0 | 16 | 0.680 | 3.950 | 5.950 | 0.000 | Feasible lower bound |
| 7 | 10 | 0 | +8 | 0 | 24 | 0.680 | 3.270 | 6.950 | 0.000 | Feasible lower bound |

Outcome: Rescued at D8 01:30; signal 7.000, lost 0.000, Fuel left 3.240; total missed meals 0. Fuel sufficient with these collection/refill timings.

Harvest ledger through termination: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 05:30 Cave Potato +3; D5 05:40 Cave Potato +3; D7 05:50 Glowcap +2; D7 06:00 Glowcap +2; D7 08:30 Cave Potato +3; D7 08:40 Cave Potato +3.

### Seed 499, 2 players, scheduled, delayed care

Finite Food 8, surface Fuel 6 + 2 starter.

| Day | Farm | Field Food | Food supply − need | Missed meals | Food reserve | Fuel burned | Fuel left incl. unpicked | Signal days | Signal lost | Rescue feasibility |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---|
| 1 | 0 | 3 | +5 | 0 | 5 | 0.650 | 7.350 | 0.950 | 0.000 | Feasible lower bound |
| 2 | 0 | 0 | -2 | 0 | 3 | 0.680 | 6.670 | 1.950 | 0.000 | Feasible lower bound |
| 3 | 6 | 5 | +9 | 0 | 12 | 0.680 | 5.990 | 2.950 | 0.000 | Feasible lower bound |
| 4 | 4 | 0 | +2 | 0 | 14 | 0.680 | 5.310 | 3.950 | 0.000 | Feasible lower bound |
| 5 | 6 | 0 | +4 | 0 | 18 | 0.680 | 4.630 | 4.950 | 0.000 | Feasible lower bound |
| 6 | 0 | 0 | -2 | 0 | 16 | 0.680 | 3.950 | 5.950 | 0.000 | Feasible lower bound |
| 7 | 4 | 0 | +2 | 0 | 18 | 0.680 | 3.270 | 6.950 | 0.000 | Feasible lower bound |

Outcome: Rescued at D8 01:30; signal 7.000, lost 0.000, Fuel left 3.240; total missed meals 0. Fuel sufficient with these collection/refill timings.

Harvest ledger through termination: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 08:30 Cave Potato +3; D5 08:40 Cave Potato +3; D7 08:50 Glowcap +2; D7 09:00 Glowcap +2.

### Seed 499, 4 players, scheduled, regular care

Finite Food 8, surface Fuel 6 + 2 starter.

| Day | Farm | Field Food | Food supply − need | Missed meals | Food reserve | Fuel burned | Fuel left incl. unpicked | Signal days | Signal lost | Rescue feasibility |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---|
| 1 | 0 | 3 | +7 | 0 | 7 | 0.650 | 7.350 | 0.950 | 0.000 | Feasible lower bound |
| 2 | 0 | 0 | -4 | 0 | 3 | 0.680 | 6.670 | 1.950 | 0.000 | Feasible lower bound |
| 3 | 6 | 5 | +7 | 0 | 10 | 0.680 | 5.990 | 2.950 | 0.000 | Feasible lower bound |
| 4 | 4 | 0 | 0 | 0 | 10 | 0.680 | 5.310 | 3.950 | 0.000 | Feasible lower bound |
| 5 | 6 | 0 | +2 | 0 | 12 | 0.680 | 4.630 | 4.950 | 0.000 | Feasible lower bound |
| 6 | 0 | 0 | -4 | 0 | 8 | 0.680 | 3.950 | 5.950 | 0.000 | Feasible lower bound |
| 7 | 10 | 0 | +6 | 0 | 14 | 0.680 | 3.270 | 6.950 | 0.000 | Feasible lower bound |

Outcome: Rescued at D8 01:30; signal 7.000, lost 0.000, Fuel left 3.240; total missed meals 0. Fuel sufficient with these collection/refill timings.

Harvest ledger through termination: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 05:30 Cave Potato +3; D5 05:40 Cave Potato +3; D7 05:50 Glowcap +2; D7 06:00 Glowcap +2; D7 08:30 Cave Potato +3; D7 08:40 Cave Potato +3.

### Seed 499, 4 players, scheduled, delayed care

Finite Food 8, surface Fuel 6 + 2 starter.

| Day | Farm | Field Food | Food supply − need | Missed meals | Food reserve | Fuel burned | Fuel left incl. unpicked | Signal days | Signal lost | Rescue feasibility |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---|
| 1 | 0 | 3 | +7 | 0 | 7 | 0.650 | 7.350 | 0.950 | 0.000 | Feasible lower bound |
| 2 | 0 | 0 | -4 | 0 | 3 | 0.680 | 6.670 | 1.950 | 0.000 | Feasible lower bound |
| 3 | 6 | 5 | +7 | 0 | 10 | 0.680 | 5.990 | 2.950 | 0.000 | Feasible lower bound |
| 4 | 4 | 0 | 0 | 0 | 10 | 0.680 | 5.310 | 3.950 | 0.000 | Feasible lower bound |
| 5 | 6 | 0 | +2 | 0 | 12 | 0.680 | 4.630 | 4.950 | 0.000 | Feasible lower bound |
| 6 | 0 | 0 | -4 | 0 | 8 | 0.680 | 3.950 | 5.950 | 0.000 | Feasible lower bound |
| 7 | 4 | 0 | 0 | 0 | 8 | 0.680 | 3.270 | 6.950 | 0.000 | Feasible lower bound |

Outcome: Rescued at D8 01:30; signal 7.000, lost 0.000, Fuel left 3.240; total missed meals 0. Fuel sufficient with these collection/refill timings.

Harvest ledger through termination: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 08:30 Cave Potato +3; D5 08:40 Cave Potato +3; D7 08:50 Glowcap +2; D7 09:00 Glowcap +2.

### Seed 499, 6 players, scheduled, regular care

Finite Food 8, surface Fuel 6 + 2 starter.

| Day | Farm | Field Food | Food supply − need | Missed meals | Food reserve | Fuel burned | Fuel left incl. unpicked | Signal days | Signal lost | Rescue feasibility |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---|
| 1 | 0 | 3 | +9 | 0 | 9 | 0.650 | 7.350 | 0.950 | 0.000 | Feasible lower bound |
| 2 | 0 | 0 | -6 | 0 | 3 | 0.680 | 6.670 | 1.950 | 0.000 | Feasible lower bound |
| 3 | 6 | 5 | +5 | 0 | 8 | 0.680 | 5.990 | 2.950 | 0.000 | Feasible lower bound |
| 4 | 4 | 0 | -2 | 0 | 6 | 0.680 | 5.310 | 3.950 | 0.000 | Feasible lower bound |
| 5 | 6 | 0 | 0 | 0 | 6 | 0.680 | 4.630 | 4.950 | 0.000 | Feasible lower bound |
| 6 | 0 | 0 | -6 | 0 | 0 | 0.680 | 3.950 | 5.950 | 0.000 | Feasible lower bound |
| 7 | 10 | 0 | +4 | 0 | 4 | 0.680 | 3.270 | 6.950 | 0.000 | Feasible lower bound |

Outcome: Rescued at D8 01:30; signal 7.000, lost 0.000, Fuel left 3.240; total missed meals 0. Fuel sufficient with these collection/refill timings.

Harvest ledger through termination: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 05:30 Cave Potato +3; D5 05:40 Cave Potato +3; D7 05:50 Glowcap +2; D7 06:00 Glowcap +2; D7 08:30 Cave Potato +3; D7 08:40 Cave Potato +3.

### Seed 499, 6 players, scheduled, delayed care

Finite Food 8, surface Fuel 6 + 2 starter.

| Day | Farm | Field Food | Food supply − need | Missed meals | Food reserve | Fuel burned | Fuel left incl. unpicked | Signal days | Signal lost | Rescue feasibility |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---|
| 1 | 0 | 3 | +9 | 0 | 9 | 0.650 | 7.350 | 0.950 | 0.000 | Feasible lower bound |
| 2 | 0 | 0 | -6 | 0 | 3 | 0.680 | 6.670 | 1.950 | 0.000 | Feasible lower bound |
| 3 | 6 | 5 | +5 | 0 | 8 | 0.680 | 5.990 | 2.950 | 0.000 | Feasible lower bound |
| 4 | 4 | 0 | -2 | 0 | 6 | 0.680 | 5.310 | 3.950 | 0.000 | Feasible lower bound |
| 5 | 6 | 0 | 0 | 0 | 6 | 0.680 | 4.630 | 4.950 | 0.000 | Feasible lower bound |
| 6 | 0 | 0 | -6 | 0 | 0 | 0.680 | 3.950 | 5.950 | 0.000 | Feasible lower bound |
| 7 | 4 | 0 | -2 | 2 | 0 | 0.680 | 3.270 | 6.950 | 0.000 | Feasible lower bound |

Outcome: Rescued at D8 01:30; signal 7.000, lost 0.000, Fuel left 3.240; total missed meals 2. Fuel sufficient with these collection/refill timings.

Harvest ledger through termination: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 08:30 Cave Potato +3; D5 08:40 Cave Potato +3; D7 08:50 Glowcap +2; D7 09:00 Glowcap +2.

### Seed 8, 2 players, wasteful startup

Finite Food 12, surface Fuel 5 + 2 starter.

| Day | Farm | Field Food | Food supply − need | Missed meals | Food reserve | Fuel burned | Fuel left incl. unpicked | Signal days | Signal lost | Rescue feasibility |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---|
| 1 | 0 | 4 | +6 | 0 | 6 | 0.570 | 6.430 | 0.000 | 0.000 | Feasible lower bound |
| 2 | 0 | 0 | -2 | 0 | 4 | 0.600 | 5.830 | 0.000 | 0.000 | Feasible lower bound |
| 3 | 6 | 8 | +12 | 0 | 16 | 0.600 | 5.230 | 0.000 | 0.000 | Feasible lower bound |
| 4 | 4 | 0 | +2 | 0 | 18 | 0.600 | 4.630 | 0.000 | 0.000 | Feasible lower bound |
| 5 | 6 | 0 | +4 | 0 | 22 | 0.600 | 4.030 | 0.000 | 0.000 | Unreachable |
| 6 | 0 | 0 | -2 | 0 | 20 | 0.600 | 3.430 | 0.950 | 0.000 | Unreachable |
| 7 | 10 | 0 | +8 | 0 | 28 | 0.600 | 2.830 | 1.950 | 0.000 | Unreachable |

Outcome: No rescue by end of Day 14; signal 6.158, lost 0.508, Fuel left 0.000; total missed meals 0. Finite Fuel falls below even beacon-only completion cost: unreachable without a new run or future supplies.

Harvest ledger through termination: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 05:30 Cave Potato +3; D5 05:40 Cave Potato +3; D7 05:50 Glowcap +2; D7 06:00 Glowcap +2; D7 08:30 Cave Potato +3; D7 08:40 Cave Potato +3; D10 03:00 Cave Potato +3; D10 03:10 Cave Potato +3; D10 08:50 Glowcap +2; D10 09:00 Glowcap +2; D12 05:30 Cave Potato +3; D12 05:40 Cave Potato +3; D14 03:20 Glowcap +2; D14 03:30 Glowcap +2; D14 08:30 Cave Potato +3; D14 08:40 Cave Potato +3.

### Seed 8, 4 players, wasteful startup

Finite Food 12, surface Fuel 5 + 2 starter.

| Day | Farm | Field Food | Food supply − need | Missed meals | Food reserve | Fuel burned | Fuel left incl. unpicked | Signal days | Signal lost | Rescue feasibility |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---|
| 1 | 0 | 4 | +8 | 0 | 8 | 0.570 | 6.430 | 0.000 | 0.000 | Feasible lower bound |
| 2 | 0 | 0 | -4 | 0 | 4 | 0.600 | 5.830 | 0.000 | 0.000 | Feasible lower bound |
| 3 | 6 | 8 | +10 | 0 | 14 | 0.600 | 5.230 | 0.000 | 0.000 | Feasible lower bound |
| 4 | 4 | 0 | 0 | 0 | 14 | 0.600 | 4.630 | 0.000 | 0.000 | Feasible lower bound |
| 5 | 6 | 0 | +2 | 0 | 16 | 0.600 | 4.030 | 0.000 | 0.000 | Unreachable |
| 6 | 0 | 0 | -4 | 0 | 12 | 0.600 | 3.430 | 0.950 | 0.000 | Unreachable |
| 7 | 10 | 0 | +6 | 0 | 18 | 0.600 | 2.830 | 1.950 | 0.000 | Unreachable |

Outcome: No rescue by end of Day 14; signal 6.158, lost 0.508, Fuel left 0.000; total missed meals 0. Finite Fuel falls below even beacon-only completion cost: unreachable without a new run or future supplies.

Harvest ledger through termination: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 05:30 Cave Potato +3; D5 05:40 Cave Potato +3; D7 05:50 Glowcap +2; D7 06:00 Glowcap +2; D7 08:30 Cave Potato +3; D7 08:40 Cave Potato +3; D10 03:00 Cave Potato +3; D10 03:10 Cave Potato +3; D10 08:50 Glowcap +2; D10 09:00 Glowcap +2; D12 05:30 Cave Potato +3; D12 05:40 Cave Potato +3; D14 03:20 Glowcap +2; D14 03:30 Glowcap +2; D14 08:30 Cave Potato +3; D14 08:40 Cave Potato +3.

### Seed 8, 6 players, wasteful startup

Finite Food 12, surface Fuel 5 + 2 starter.

| Day | Farm | Field Food | Food supply − need | Missed meals | Food reserve | Fuel burned | Fuel left incl. unpicked | Signal days | Signal lost | Rescue feasibility |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---|
| 1 | 0 | 4 | +10 | 0 | 10 | 0.570 | 6.430 | 0.000 | 0.000 | Feasible lower bound |
| 2 | 0 | 0 | -6 | 0 | 4 | 0.600 | 5.830 | 0.000 | 0.000 | Feasible lower bound |
| 3 | 6 | 8 | +8 | 0 | 12 | 0.600 | 5.230 | 0.000 | 0.000 | Feasible lower bound |
| 4 | 4 | 0 | -2 | 0 | 10 | 0.600 | 4.630 | 0.000 | 0.000 | Feasible lower bound |
| 5 | 6 | 0 | 0 | 0 | 10 | 0.600 | 4.030 | 0.000 | 0.000 | Unreachable |
| 6 | 0 | 0 | -6 | 0 | 4 | 0.600 | 3.430 | 0.950 | 0.000 | Unreachable |
| 7 | 10 | 0 | +4 | 0 | 8 | 0.600 | 2.830 | 1.950 | 0.000 | Unreachable |

Outcome: No rescue by end of Day 14; signal 6.158, lost 0.508, Fuel left 0.000; total missed meals 12. Finite Fuel falls below even beacon-only completion cost: unreachable without a new run or future supplies.

Harvest ledger through termination: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 05:30 Cave Potato +3; D5 05:40 Cave Potato +3; D7 05:50 Glowcap +2; D7 06:00 Glowcap +2; D7 08:30 Cave Potato +3; D7 08:40 Cave Potato +3; D10 03:00 Cave Potato +3; D10 03:10 Cave Potato +3; D10 08:50 Glowcap +2; D10 09:00 Glowcap +2; D12 05:30 Cave Potato +3; D12 05:40 Cave Potato +3; D14 03:20 Glowcap +2; D14 03:30 Glowcap +2; D14 08:30 Cave Potato +3; D14 08:40 Cave Potato +3.

## Limits and recovery

Finite Fuel can make rescue mathematically unreachable after wasted generation, avoidable high-load use or repeated loss episodes. There is no forced game-over or hidden refill. Stop optional loads early and conserve remaining Fuel; if it is already below the beacon-only lower bound, this prototype requires New Run/reset. Food shortages slow actors but do not kill them or disable farming/power, so the accepted six-player food deficit does not itself block rescue. Future surface danger, actual multiplayer labor and larger-map travel are not simulated. No Food, Fuel, farm or hunger balance values changed.
