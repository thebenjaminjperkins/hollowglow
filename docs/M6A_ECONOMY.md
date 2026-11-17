# M6a seven-day geography / economy replay

Uses the unchanged authoritative M5a model and recipes, with actual Godot physics travel measurements for seeds 499, 1701 and 8. Two roles: one sanctuary operator/gardener, one sequential scavenger; additional actors consume Food normally. The scavenger starts near Food at D1 00:00, returns/deposits after each source, then retrieves the three material nodes individually. Near Fuel departs D2 03:00; far Food departs D3 00:00, sequentially; far Fuel departs D4 03:00. Collection occurs at measured outbound time, deposits at measured round-trip time. Each measured trip includes two seconds at the source and two at storage. Timings are rounded up to whole seconds. No teleport or extra stock in the economy replay.

Sanctuary duties retain the documented M5a ten-second plot spacing and twice/three-times-daily visits, with thirty-second workshop visits. Actual storage/workshop/generator/garden round trips are measured separately and fit these allowances. Crafting begins ten seconds after all materials return, then forty seconds later for the second kit. The operator explicitly turns lighting off for workshop visits and restores it at Night; capacity rules are unchanged. The scavenger skips shared resupply while away and explicitly withdraws on return. Other actors remain at the sanctuary; manual refueling retains M5a assumptions. These are coordinated core simulations, not network multiplayer or human navigation results.

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

### Seed 1701, 2 players, backpacks only, delayed care

| Day | Farm Food | Field Food | Food supply − need | Missed meals | Food reserve | Fuel burned | Fuel left incl. unpicked | Signal days | Signal lost | Available Scrap / Organic | Installed packs / plots | Rescue feasibility |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---|:---|:---|
| 1 | 0 | 4 | +6 | 0 | 6 | 0.690 | 7.310 | 0.950 | 0.000 | 10 / 2 | 2 / 4 | Feasible lower bound |
| 2 | 0 | 0 | -2 | 0 | 4 | 0.680 | 6.630 | 1.950 | 0.000 | 10 / 2 | 2 / 4 | Feasible lower bound |
| 3 | 6 | 10 | +14 | 0 | 18 | 0.680 | 5.950 | 2.950 | 0.000 | 10 / 2 | 2 / 4 | Feasible lower bound |
| 4 | 4 | 0 | +2 | 0 | 20 | 0.680 | 5.270 | 3.950 | 0.000 | 10 / 4 | 2 / 4 | Feasible lower bound |
| 5 | 6 | 0 | +4 | 0 | 24 | 0.680 | 4.590 | 4.950 | 0.000 | 10 / 4 | 2 / 4 | Feasible lower bound |
| 6 | 0 | 0 | -2 | 0 | 22 | 0.680 | 3.910 | 5.950 | 0.000 | 10 / 4 | 2 / 4 | Feasible lower bound |
| 7 | 4 | 0 | +2 | 0 | 24 | 0.680 | 3.230 | 6.950 | 0.000 | 10 / 6 | 2 / 4 | Feasible lower bound |

Outcome: Rescued at D8 00:30; signal 7.000, lost 0.000, Fuel left 3.200; total missed meals 0. Fuel sufficient with these collection/refill timings.

Harvest ledger through termination: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 08:30 Cave Potato +3; D5 08:40 Cave Potato +3; D7 08:50 Glowcap +2; D7 09:00 Glowcap +2.

### Seed 1701, 2 players, one backpack + fifth plot + propagation, delayed care

| Day | Farm Food | Field Food | Food supply − need | Missed meals | Food reserve | Fuel burned | Fuel left incl. unpicked | Signal days | Signal lost | Available Scrap / Organic | Installed packs / plots | Rescue feasibility |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---|:---|:---|
| 1 | 0 | 4 | +6 | 0 | 6 | 0.690 | 7.310 | 0.950 | 0.000 | 6 / 1 | 1 / 5 | Feasible lower bound |
| 2 | 0 | 0 | -2 | 0 | 4 | 0.680 | 6.630 | 1.950 | 0.000 | 6 / 1 | 1 / 5 | Feasible lower bound |
| 3 | 6 | 10 | +14 | 0 | 18 | 0.700 | 5.930 | 2.950 | 0.000 | 6 / 0 | 1 / 5 | Feasible lower bound |
| 4 | 4 | 0 | +2 | 0 | 20 | 0.680 | 5.250 | 3.950 | 0.000 | 6 / 2 | 1 / 5 | Feasible lower bound |
| 5 | 9 | 0 | +7 | 0 | 27 | 0.680 | 4.570 | 4.950 | 0.000 | 6 / 2 | 1 / 5 | Feasible lower bound |
| 6 | 0 | 0 | -2 | 0 | 25 | 0.680 | 3.890 | 5.950 | 0.000 | 6 / 2 | 1 / 5 | Feasible lower bound |
| 7 | 4 | 0 | +2 | 0 | 27 | 0.680 | 3.210 | 6.950 | 0.000 | 6 / 4 | 1 / 5 | Feasible lower bound |

Outcome: Rescued at D8 00:30; signal 7.000, lost 0.000, Fuel left 3.180; total missed meals 0. Fuel sufficient with these collection/refill timings.

Harvest ledger through termination: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 08:30 Cave Potato +3; D5 08:40 Cave Potato +3; D5 09:10 Cave Potato +3; D7 08:50 Glowcap +2; D7 09:00 Glowcap +2.

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

### Seed 1701, 4 players, backpacks only, delayed care

| Day | Farm Food | Field Food | Food supply − need | Missed meals | Food reserve | Fuel burned | Fuel left incl. unpicked | Signal days | Signal lost | Available Scrap / Organic | Installed packs / plots | Rescue feasibility |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---|:---|:---|
| 1 | 0 | 4 | +8 | 0 | 8 | 0.690 | 7.310 | 0.950 | 0.000 | 10 / 2 | 2 / 4 | Feasible lower bound |
| 2 | 0 | 0 | -4 | 0 | 4 | 0.680 | 6.630 | 1.950 | 0.000 | 10 / 2 | 2 / 4 | Feasible lower bound |
| 3 | 6 | 10 | +12 | 0 | 16 | 0.680 | 5.950 | 2.950 | 0.000 | 10 / 2 | 2 / 4 | Feasible lower bound |
| 4 | 4 | 0 | 0 | 0 | 16 | 0.700 | 5.250 | 3.950 | 0.000 | 2 / 1 | 3 / 4 | Feasible lower bound |
| 5 | 6 | 0 | +2 | 0 | 18 | 0.680 | 4.570 | 4.950 | 0.000 | 2 / 1 | 3 / 4 | Feasible lower bound |
| 6 | 0 | 0 | -4 | 0 | 14 | 0.680 | 3.890 | 5.950 | 0.000 | 2 / 1 | 3 / 4 | Feasible lower bound |
| 7 | 4 | 0 | 0 | 0 | 14 | 0.680 | 3.210 | 6.950 | 0.000 | 2 / 3 | 3 / 4 | Feasible lower bound |

Outcome: Rescued at D8 00:30; signal 7.000, lost 0.000, Fuel left 3.180; total missed meals 0. Fuel sufficient with these collection/refill timings.

Harvest ledger through termination: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 08:30 Cave Potato +3; D5 08:40 Cave Potato +3; D7 08:50 Glowcap +2; D7 09:00 Glowcap +2.

### Seed 1701, 4 players, one backpack + fifth plot + propagation, delayed care

| Day | Farm Food | Field Food | Food supply − need | Missed meals | Food reserve | Fuel burned | Fuel left incl. unpicked | Signal days | Signal lost | Available Scrap / Organic | Installed packs / plots | Rescue feasibility |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---|:---|:---|
| 1 | 0 | 4 | +8 | 0 | 8 | 0.690 | 7.310 | 0.950 | 0.000 | 6 / 1 | 1 / 5 | Feasible lower bound |
| 2 | 0 | 0 | -4 | 0 | 4 | 0.680 | 6.630 | 1.950 | 0.000 | 6 / 1 | 1 / 5 | Feasible lower bound |
| 3 | 6 | 10 | +12 | 0 | 16 | 0.700 | 5.930 | 2.950 | 0.000 | 6 / 0 | 1 / 5 | Feasible lower bound |
| 4 | 4 | 0 | 0 | 0 | 16 | 0.680 | 5.250 | 3.950 | 0.000 | 6 / 2 | 1 / 5 | Feasible lower bound |
| 5 | 9 | 0 | +5 | 0 | 21 | 0.680 | 4.570 | 4.950 | 0.000 | 6 / 2 | 1 / 5 | Feasible lower bound |
| 6 | 0 | 0 | -4 | 0 | 17 | 0.680 | 3.890 | 5.950 | 0.000 | 6 / 2 | 1 / 5 | Feasible lower bound |
| 7 | 4 | 0 | 0 | 0 | 17 | 0.680 | 3.210 | 6.950 | 0.000 | 6 / 4 | 1 / 5 | Feasible lower bound |

Outcome: Rescued at D8 00:30; signal 7.000, lost 0.000, Fuel left 3.180; total missed meals 0. Fuel sufficient with these collection/refill timings.

Harvest ledger through termination: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 08:30 Cave Potato +3; D5 08:40 Cave Potato +3; D5 09:10 Cave Potato +3; D7 08:50 Glowcap +2; D7 09:00 Glowcap +2.

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

### Seed 1701, 6 players, backpacks only, delayed care

| Day | Farm Food | Field Food | Food supply − need | Missed meals | Food reserve | Fuel burned | Fuel left incl. unpicked | Signal days | Signal lost | Available Scrap / Organic | Installed packs / plots | Rescue feasibility |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---|:---|:---|
| 1 | 0 | 4 | +10 | 0 | 10 | 0.690 | 7.310 | 0.950 | 0.000 | 10 / 2 | 2 / 4 | Feasible lower bound |
| 2 | 0 | 0 | -6 | 0 | 4 | 0.680 | 6.630 | 1.950 | 0.000 | 10 / 2 | 2 / 4 | Feasible lower bound |
| 3 | 6 | 10 | +10 | 0 | 14 | 0.680 | 5.950 | 2.950 | 0.000 | 10 / 2 | 2 / 4 | Feasible lower bound |
| 4 | 4 | 0 | -2 | 0 | 12 | 0.700 | 5.250 | 3.950 | 0.000 | 2 / 1 | 3 / 4 | Feasible lower bound |
| 5 | 6 | 0 | 0 | 0 | 12 | 0.680 | 4.570 | 4.950 | 0.000 | 2 / 1 | 3 / 4 | Feasible lower bound |
| 6 | 0 | 0 | -6 | 0 | 6 | 0.680 | 3.890 | 5.950 | 0.000 | 2 / 1 | 3 / 4 | Feasible lower bound |
| 7 | 4 | 0 | -2 | 0 | 4 | 0.680 | 3.210 | 6.950 | 0.000 | 2 / 3 | 3 / 4 | Feasible lower bound |

Outcome: Rescued at D8 00:30; signal 7.000, lost 0.000, Fuel left 3.180; total missed meals 0. Fuel sufficient with these collection/refill timings.

Harvest ledger through termination: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 08:30 Cave Potato +3; D5 08:40 Cave Potato +3; D7 08:50 Glowcap +2; D7 09:00 Glowcap +2.

### Seed 1701, 6 players, one backpack + fifth plot + propagation, delayed care

| Day | Farm Food | Field Food | Food supply − need | Missed meals | Food reserve | Fuel burned | Fuel left incl. unpicked | Signal days | Signal lost | Available Scrap / Organic | Installed packs / plots | Rescue feasibility |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---|:---|:---|
| 1 | 0 | 4 | +10 | 0 | 10 | 0.690 | 7.310 | 0.950 | 0.000 | 6 / 1 | 1 / 5 | Feasible lower bound |
| 2 | 0 | 0 | -6 | 0 | 4 | 0.680 | 6.630 | 1.950 | 0.000 | 6 / 1 | 1 / 5 | Feasible lower bound |
| 3 | 6 | 10 | +10 | 0 | 14 | 0.700 | 5.930 | 2.950 | 0.000 | 6 / 0 | 1 / 5 | Feasible lower bound |
| 4 | 4 | 0 | -2 | 0 | 12 | 0.680 | 5.250 | 3.950 | 0.000 | 6 / 2 | 1 / 5 | Feasible lower bound |
| 5 | 9 | 0 | +3 | 0 | 15 | 0.680 | 4.570 | 4.950 | 0.000 | 6 / 2 | 1 / 5 | Feasible lower bound |
| 6 | 0 | 0 | -6 | 0 | 9 | 0.680 | 3.890 | 5.950 | 0.000 | 6 / 2 | 1 / 5 | Feasible lower bound |
| 7 | 4 | 0 | -2 | 0 | 7 | 0.680 | 3.210 | 6.950 | 0.000 | 6 / 4 | 1 / 5 | Feasible lower bound |

Outcome: Rescued at D8 00:30; signal 7.000, lost 0.000, Fuel left 3.180; total missed meals 0. Fuel sufficient with these collection/refill timings.

Harvest ledger through termination: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 08:30 Cave Potato +3; D5 08:40 Cave Potato +3; D5 09:10 Cave Potato +3; D7 08:50 Glowcap +2; D7 09:00 Glowcap +2.

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

### Seed 8, 2 players, backpacks only, delayed care

| Day | Farm Food | Field Food | Food supply − need | Missed meals | Food reserve | Fuel burned | Fuel left incl. unpicked | Signal days | Signal lost | Available Scrap / Organic | Installed packs / plots | Rescue feasibility |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---|:---|:---|
| 1 | 0 | 4 | +6 | 0 | 6 | 0.690 | 6.310 | 0.950 | 0.000 | 10 / 2 | 2 / 4 | Feasible lower bound |
| 2 | 0 | 0 | -2 | 0 | 4 | 0.680 | 5.630 | 1.950 | 0.000 | 10 / 2 | 2 / 4 | Feasible lower bound |
| 3 | 6 | 8 | +12 | 0 | 16 | 0.680 | 4.950 | 2.950 | 0.000 | 10 / 2 | 2 / 4 | Feasible lower bound |
| 4 | 4 | 0 | +2 | 0 | 18 | 0.680 | 4.270 | 3.950 | 0.000 | 10 / 4 | 2 / 4 | Feasible lower bound |
| 5 | 6 | 0 | +4 | 0 | 22 | 0.680 | 3.590 | 4.950 | 0.000 | 10 / 4 | 2 / 4 | Feasible lower bound |
| 6 | 0 | 0 | -2 | 0 | 20 | 0.680 | 2.910 | 5.950 | 0.000 | 10 / 4 | 2 / 4 | Feasible lower bound |
| 7 | 4 | 0 | +2 | 0 | 22 | 0.680 | 2.230 | 6.950 | 0.000 | 10 / 6 | 2 / 4 | Feasible lower bound |

Outcome: Rescued at D8 00:30; signal 7.000, lost 0.000, Fuel left 2.200; total missed meals 0. Fuel sufficient with these collection/refill timings.

Harvest ledger through termination: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 08:30 Cave Potato +3; D5 08:40 Cave Potato +3; D7 08:50 Glowcap +2; D7 09:00 Glowcap +2.

### Seed 8, 2 players, one backpack + fifth plot + propagation, delayed care

| Day | Farm Food | Field Food | Food supply − need | Missed meals | Food reserve | Fuel burned | Fuel left incl. unpicked | Signal days | Signal lost | Available Scrap / Organic | Installed packs / plots | Rescue feasibility |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---|:---|:---|
| 1 | 0 | 4 | +6 | 0 | 6 | 0.690 | 6.310 | 0.950 | 0.000 | 6 / 1 | 1 / 5 | Feasible lower bound |
| 2 | 0 | 0 | -2 | 0 | 4 | 0.680 | 5.630 | 1.950 | 0.000 | 6 / 1 | 1 / 5 | Feasible lower bound |
| 3 | 6 | 8 | +12 | 0 | 16 | 0.700 | 4.930 | 2.950 | 0.000 | 6 / 0 | 1 / 5 | Feasible lower bound |
| 4 | 4 | 0 | +2 | 0 | 18 | 0.680 | 4.250 | 3.950 | 0.000 | 6 / 2 | 1 / 5 | Feasible lower bound |
| 5 | 9 | 0 | +7 | 0 | 25 | 0.680 | 3.570 | 4.950 | 0.000 | 6 / 2 | 1 / 5 | Feasible lower bound |
| 6 | 0 | 0 | -2 | 0 | 23 | 0.680 | 2.890 | 5.950 | 0.000 | 6 / 2 | 1 / 5 | Feasible lower bound |
| 7 | 4 | 0 | +2 | 0 | 25 | 0.680 | 2.210 | 6.950 | 0.000 | 6 / 4 | 1 / 5 | Feasible lower bound |

Outcome: Rescued at D8 00:30; signal 7.000, lost 0.000, Fuel left 2.180; total missed meals 0. Fuel sufficient with these collection/refill timings.

Harvest ledger through termination: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 08:30 Cave Potato +3; D5 08:40 Cave Potato +3; D5 09:10 Cave Potato +3; D7 08:50 Glowcap +2; D7 09:00 Glowcap +2.

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

### Seed 8, 4 players, backpacks only, delayed care

| Day | Farm Food | Field Food | Food supply − need | Missed meals | Food reserve | Fuel burned | Fuel left incl. unpicked | Signal days | Signal lost | Available Scrap / Organic | Installed packs / plots | Rescue feasibility |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---|:---|:---|
| 1 | 0 | 4 | +8 | 0 | 8 | 0.690 | 6.310 | 0.950 | 0.000 | 10 / 2 | 2 / 4 | Feasible lower bound |
| 2 | 0 | 0 | -4 | 0 | 4 | 0.680 | 5.630 | 1.950 | 0.000 | 10 / 2 | 2 / 4 | Feasible lower bound |
| 3 | 6 | 8 | +10 | 0 | 14 | 0.680 | 4.950 | 2.950 | 0.000 | 10 / 2 | 2 / 4 | Feasible lower bound |
| 4 | 4 | 0 | 0 | 0 | 14 | 0.700 | 4.250 | 3.950 | 0.000 | 2 / 1 | 3 / 4 | Feasible lower bound |
| 5 | 6 | 0 | +2 | 0 | 16 | 0.680 | 3.570 | 4.950 | 0.000 | 2 / 1 | 3 / 4 | Feasible lower bound |
| 6 | 0 | 0 | -4 | 0 | 12 | 0.680 | 2.890 | 5.950 | 0.000 | 2 / 1 | 3 / 4 | Feasible lower bound |
| 7 | 4 | 0 | 0 | 0 | 12 | 0.680 | 2.210 | 6.950 | 0.000 | 2 / 3 | 3 / 4 | Feasible lower bound |

Outcome: Rescued at D8 00:30; signal 7.000, lost 0.000, Fuel left 2.180; total missed meals 0. Fuel sufficient with these collection/refill timings.

Harvest ledger through termination: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 08:30 Cave Potato +3; D5 08:40 Cave Potato +3; D7 08:50 Glowcap +2; D7 09:00 Glowcap +2.

### Seed 8, 4 players, one backpack + fifth plot + propagation, delayed care

| Day | Farm Food | Field Food | Food supply − need | Missed meals | Food reserve | Fuel burned | Fuel left incl. unpicked | Signal days | Signal lost | Available Scrap / Organic | Installed packs / plots | Rescue feasibility |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---|:---|:---|
| 1 | 0 | 4 | +8 | 0 | 8 | 0.690 | 6.310 | 0.950 | 0.000 | 6 / 1 | 1 / 5 | Feasible lower bound |
| 2 | 0 | 0 | -4 | 0 | 4 | 0.680 | 5.630 | 1.950 | 0.000 | 6 / 1 | 1 / 5 | Feasible lower bound |
| 3 | 6 | 8 | +10 | 0 | 14 | 0.700 | 4.930 | 2.950 | 0.000 | 6 / 0 | 1 / 5 | Feasible lower bound |
| 4 | 4 | 0 | 0 | 0 | 14 | 0.680 | 4.250 | 3.950 | 0.000 | 6 / 2 | 1 / 5 | Feasible lower bound |
| 5 | 9 | 0 | +5 | 0 | 19 | 0.680 | 3.570 | 4.950 | 0.000 | 6 / 2 | 1 / 5 | Feasible lower bound |
| 6 | 0 | 0 | -4 | 0 | 15 | 0.680 | 2.890 | 5.950 | 0.000 | 6 / 2 | 1 / 5 | Feasible lower bound |
| 7 | 4 | 0 | 0 | 0 | 15 | 0.680 | 2.210 | 6.950 | 0.000 | 6 / 4 | 1 / 5 | Feasible lower bound |

Outcome: Rescued at D8 00:30; signal 7.000, lost 0.000, Fuel left 2.180; total missed meals 0. Fuel sufficient with these collection/refill timings.

Harvest ledger through termination: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 08:30 Cave Potato +3; D5 08:40 Cave Potato +3; D5 09:10 Cave Potato +3; D7 08:50 Glowcap +2; D7 09:00 Glowcap +2.

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

### Seed 8, 6 players, backpacks only, delayed care

| Day | Farm Food | Field Food | Food supply − need | Missed meals | Food reserve | Fuel burned | Fuel left incl. unpicked | Signal days | Signal lost | Available Scrap / Organic | Installed packs / plots | Rescue feasibility |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---|:---|:---|
| 1 | 0 | 4 | +10 | 0 | 10 | 0.690 | 6.310 | 0.950 | 0.000 | 10 / 2 | 2 / 4 | Feasible lower bound |
| 2 | 0 | 0 | -6 | 0 | 4 | 0.680 | 5.630 | 1.950 | 0.000 | 10 / 2 | 2 / 4 | Feasible lower bound |
| 3 | 6 | 8 | +8 | 0 | 12 | 0.680 | 4.950 | 2.950 | 0.000 | 10 / 2 | 2 / 4 | Feasible lower bound |
| 4 | 4 | 0 | -2 | 0 | 10 | 0.700 | 4.250 | 3.950 | 0.000 | 2 / 1 | 3 / 4 | Feasible lower bound |
| 5 | 6 | 0 | 0 | 0 | 10 | 0.680 | 3.570 | 4.950 | 0.000 | 2 / 1 | 3 / 4 | Feasible lower bound |
| 6 | 0 | 0 | -6 | 0 | 4 | 0.680 | 2.890 | 5.950 | 0.000 | 2 / 1 | 3 / 4 | Feasible lower bound |
| 7 | 4 | 0 | -2 | 0 | 2 | 0.680 | 2.210 | 6.950 | 0.000 | 2 / 3 | 3 / 4 | Feasible lower bound |

Outcome: Rescued at D8 00:30; signal 7.000, lost 0.000, Fuel left 2.180; total missed meals 0. Fuel sufficient with these collection/refill timings.

Harvest ledger through termination: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 08:30 Cave Potato +3; D5 08:40 Cave Potato +3; D7 08:50 Glowcap +2; D7 09:00 Glowcap +2.

### Seed 8, 6 players, one backpack + fifth plot + propagation, delayed care

| Day | Farm Food | Field Food | Food supply − need | Missed meals | Food reserve | Fuel burned | Fuel left incl. unpicked | Signal days | Signal lost | Available Scrap / Organic | Installed packs / plots | Rescue feasibility |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---|:---|:---|
| 1 | 0 | 4 | +10 | 0 | 10 | 0.690 | 6.310 | 0.950 | 0.000 | 6 / 1 | 1 / 5 | Feasible lower bound |
| 2 | 0 | 0 | -6 | 0 | 4 | 0.680 | 5.630 | 1.950 | 0.000 | 6 / 1 | 1 / 5 | Feasible lower bound |
| 3 | 6 | 8 | +8 | 0 | 12 | 0.700 | 4.930 | 2.950 | 0.000 | 6 / 0 | 1 / 5 | Feasible lower bound |
| 4 | 4 | 0 | -2 | 0 | 10 | 0.680 | 4.250 | 3.950 | 0.000 | 6 / 2 | 1 / 5 | Feasible lower bound |
| 5 | 9 | 0 | +3 | 0 | 13 | 0.680 | 3.570 | 4.950 | 0.000 | 6 / 2 | 1 / 5 | Feasible lower bound |
| 6 | 0 | 0 | -6 | 0 | 7 | 0.680 | 2.890 | 5.950 | 0.000 | 6 / 2 | 1 / 5 | Feasible lower bound |
| 7 | 4 | 0 | -2 | 0 | 5 | 0.680 | 2.210 | 6.950 | 0.000 | 6 / 4 | 1 / 5 | Feasible lower bound |

Outcome: Rescued at D8 00:30; signal 7.000, lost 0.000, Fuel left 2.180; total missed meals 0. Fuel sufficient with these collection/refill timings.

Harvest ledger through termination: D3 03:00 Cave Potato +3; D3 03:10 Cave Potato +3; D4 03:20 Glowcap +2; D4 03:30 Glowcap +2; D5 08:30 Cave Potato +3; D5 08:40 Cave Potato +3; D5 09:10 Cave Potato +3; D7 08:50 Glowcap +2; D7 09:00 Glowcap +2.

## Interpretation

Daily Food supply minus need is a flow, while reserve includes carried-over supplies; negative daily flow need not mean missed meals. Finite Fuel and resource budgets are unchanged. Each scenario asserts exact conservation, available crafting costs, unchanged original crop yields, and rescue completion. The six-player minimum-Food delayed-care case remains the tightest. Human detours, missed deposits or running the workshop unnecessarily can worsen these results; no economy correction is applied by M6a.
