# 14-day candidate economy: reproducible validation study

This is an isolated numerical experiment, not the Gameplay & Economy Brief or a playable balance change. All reported results use the frozen configuration, one deterministic seed and the documented policies. They do not establish real multiplayer balance.

## Baseline results

| World | Players | Strategy | Outcome / calendar date | Missed meals | Outage / loss | Food / Fuel / Biomass reserve | Underground operation confirmed |
|---|---:|---|---|---:|---|---|---|
| old_highway | 2 | infrastructure | rescued D15 00:23.00 | 0 | 0.0s / 0.0000d | 34 / 15.822 / 14.280 | D11 05:00.00 |
| old_highway | 4 | infrastructure | rescued D15 00:23.00 | 0 | 0.0s / 0.0000d | 25 / 17.234 / 10.848 | D10 05:00.00 |
| old_highway | 6 | infrastructure | rescued D15 00:23.00 | 0 | 0.0s / 0.0000d | 9 / 16.293 / 19.478 | Not confirmed |
| old_highway | 2 | exploration | rescued D15 00:23.00 | 0 | 0.0s / 0.0000d | 34 / 15.786 / 14.280 | D11 05:00.00 |
| old_highway | 4 | exploration | rescued D15 00:23.00 | 0 | 0.0s / 0.0000d | 25 / 17.129 / 11.153 | D10 05:00.00 |
| old_highway | 6 | exploration | rescued D15 00:23.00 | 0 | 0.0s / 0.0000d | 9 / 16.250 / 19.484 | Not confirmed |
| old_highway | 2 | balanced | rescued D15 00:23.00 | 0 | 0.0s / 0.0000d | 34 / 15.786 / 14.280 | D11 05:00.00 |
| old_highway | 4 | balanced | rescued D15 00:23.00 | 0 | 0.0s / 0.0000d | 25 / 17.081 / 11.124 | D10 05:00.00 |
| old_highway | 6 | balanced | rescued D15 00:23.00 | 0 | 0.0s / 0.0000d | 12 / 16.233 / 21.560 | D14 05:00.00 |
| overgrown_valley | 2 | infrastructure | rescued D15 00:23.00 | 0 | 0.0s / 0.0000d | 40 / 8.811 / 18.280 | D11 05:00.00 |
| overgrown_valley | 4 | infrastructure | rescued D15 00:23.00 | 0 | 0.0s / 0.0000d | 22 / 10.466 / 15.427 | D8 05:00.00 |
| overgrown_valley | 6 | infrastructure | rescued D15 00:23.00 | 0 | 0.0s / 0.0000d | 22 / 9.834 / 16.014 | D11 05:00.00 |
| overgrown_valley | 2 | exploration | rescued D15 00:23.00 | 0 | 0.0s / 0.0000d | 40 / 8.751 / 18.290 | D11 05:00.00 |
| overgrown_valley | 4 | exploration | rescued D15 00:23.00 | 0 | 0.0s / 0.0000d | 22 / 9.909 / 18.897 | D9 05:00.00 |
| overgrown_valley | 6 | exploration | rescued D15 00:23.00 | 0 | 0.0s / 0.0000d | 22 / 1.524 / 35.000 | Not confirmed |
| overgrown_valley | 2 | balanced | rescued D15 00:23.00 | 0 | 0.0s / 0.0000d | 40 / 8.754 / 20.280 | D11 05:00.00 |
| overgrown_valley | 4 | balanced | rescued D15 00:23.00 | 0 | 0.0s / 0.0000d | 22 / 10.322 / 15.660 | D8 05:00.00 |
| overgrown_valley | 6 | balanced | rescued D15 00:23.00 | 0 | 0.0s / 0.0000d | 22 / 10.076 / 20.200 | D11 05:00.00 |
| industrial_hollow | 2 | infrastructure | rescued D15 00:23.00 | 0 | 0.0s / 0.0000d | 34 / 10.837 / 14.280 | D11 05:00.00 |
| industrial_hollow | 4 | infrastructure | rescued D15 00:23.00 | 0 | 0.0s / 0.0000d | 25 / 11.516 / 10.766 | D11 05:00.00 |
| industrial_hollow | 6 | infrastructure | rescued D15 00:23.00 | 0 | 0.0s / 0.0000d | 16 / 10.436 / 9.103 | D11 05:00.00 |
| industrial_hollow | 2 | exploration | rescued D15 00:23.00 | 0 | 0.0s / 0.0000d | 34 / 10.750 / 14.280 | D11 05:00.00 |
| industrial_hollow | 4 | exploration | rescued D15 00:23.00 | 0 | 0.0s / 0.0000d | 16 / 11.502 / 14.704 | D11 05:00.00 |
| industrial_hollow | 6 | exploration | rescued D15 00:23.00 | 0 | 0.0s / 0.0000d | 16 / 10.377 / 9.097 | D11 05:00.00 |
| industrial_hollow | 2 | balanced | rescued D15 00:23.00 | 0 | 0.0s / 0.0000d | 34 / 10.771 / 14.280 | D11 05:00.00 |
| industrial_hollow | 4 | balanced | rescued D15 00:23.00 | 0 | 0.0s / 0.0000d | 16 / 11.503 / 14.651 | D11 05:00.00 |
| industrial_hollow | 6 | balanced | rescued D15 00:23.00 | 0 | 0.0s / 0.0000d | 16 / 10.311 / 9.291 | D11 05:00.00 |

**27/27 baseline rescues; 24/27 confirmed underground continuations.** Confirmation requires a repeatable no-surface/no-Fuel schedule witness AND at least 6.5 days of actual operation without new surface Food/Fuel/Biomass, missed meals, outages or sub-day reserves, ending with non-decreasing Food/Biomass stocks. The earlier feasible-certificate date and necessary capacity/reserve dates are separate CSV fields.

## Paired setbacks

| Event | Triggered | Rescued | Additional missed meals | Largest rescue delay | Confirmed underground operation |
|---|---:|---:|---:|---:|---:|
| missed_harvest | 27/27 | 27/27 | +0 | 0.0s | 24/27 |
| failed_expedition | 27/27 | 27/27 | +0 | 0.0s | 24/27 |

See [paired-setbacks.csv](paired-setbacks.csv) for each matching configuration. An untriggered setback is not counted as successful recovery. Failed expeditions leave site inventories intact. A cutoff is not an assertion of permanent defeat; model exceptions are simulation errors and terminate the runner.

## Investments and bottlenecks

- **old_highway / 2 / infrastructure:** bed D2 02:57.00, biogenerator D2 04:53.00, tool D2 05:41.00. Remaining Metal/Basic/Advanced: 26/3/0; crops {'potato': 2, 'marrowvine': 2, 'glowcap': 1}; next investment {'next_goal': None, 'in_progress': False, 'missing_from_held': {}, 'finite_material_shortfall_even_with_remaining_salvage': {}}; busy actor time 10.3%.
- **old_highway / 4 / infrastructure:** bed D1 03:11.00, bed D1 05:12.00, biogenerator D1 07:08.00, tool D1 07:56.00. Remaining Metal/Basic/Advanced: 5/3/0; crops {'potato': 3, 'glowcap': 1, 'marrowvine': 2}; next investment {'next_goal': None, 'in_progress': False, 'missing_from_held': {}, 'finite_material_shortfall_even_with_remaining_salvage': {}}; busy actor time 6.9%.
- **old_highway / 6 / infrastructure:** bed D1 03:11.00, bed D1 05:12.00, tool D1 06:19.00, bed D2 05:08.00, biogenerator D2 06:45.00. Remaining Metal/Basic/Advanced: 17/9/1; crops {'potato': 4, 'glowcap': 1, 'marrowvine': 2}; next investment {'next_goal': None, 'in_progress': False, 'missing_from_held': {}, 'finite_material_shortfall_even_with_remaining_salvage': {}}; busy actor time 6.6%.
- **old_highway / 2 / exploration:** tool D1 02:51.00, barrow D1 04:07.00, backpack D1 04:56.00, bed D2 03:08.00, biogenerator D2 05:43.00. Remaining Metal/Basic/Advanced: 26/10/3; crops {'potato': 2, 'marrowvine': 2, 'glowcap': 1}; next investment {'next_goal': None, 'in_progress': False, 'missing_from_held': {}, 'finite_material_shortfall_even_with_remaining_salvage': {}}; busy actor time 11.6%.
- **old_highway / 4 / exploration:** tool D1 02:17.00, barrow D1 03:25.00, backpack D1 04:14.00, bed D1 05:51.00, bed D1 07:52.00, biogenerator D2 00:00.00. Remaining Metal/Basic/Advanced: 5/5/1; crops {'potato': 3, 'glowcap': 1, 'marrowvine': 2}; next investment {'next_goal': None, 'in_progress': False, 'missing_from_held': {}, 'finite_material_shortfall_even_with_remaining_salvage': {}}; busy actor time 7.6%.
- **old_highway / 6 / exploration:** tool D1 02:17.00, barrow D1 03:25.00, backpack D1 04:14.00, bed D1 05:51.00, bed D1 07:52.00, bed D2 05:13.00, biogenerator D2 06:50.00. Remaining Metal/Basic/Advanced: 14/10/3; crops {'potato': 4, 'glowcap': 1, 'marrowvine': 2}; next investment {'next_goal': None, 'in_progress': False, 'missing_from_held': {}, 'finite_material_shortfall_even_with_remaining_salvage': {}}; busy actor time 7.2%.
- **old_highway / 2 / balanced:** tool D1 02:51.00, bed D2 02:55.00, biogenerator D2 04:51.00, barrow D2 05:58.00, backpack D2 06:47.00. Remaining Metal/Basic/Advanced: 26/8/2; crops {'potato': 2, 'marrowvine': 2, 'glowcap': 1}; next investment {'next_goal': None, 'in_progress': False, 'missing_from_held': {}, 'finite_material_shortfall_even_with_remaining_salvage': {}}; busy actor time 11.5%.
- **old_highway / 4 / balanced:** tool D1 02:17.00, bed D1 04:00.00, bed D1 06:01.00, biogenerator D1 07:57.00, barrow D2 04:08.00, backpack D2 05:09.00. Remaining Metal/Basic/Advanced: 2/4/2; crops {'potato': 3, 'glowcap': 1, 'marrowvine': 2}; next investment {'next_goal': None, 'in_progress': False, 'missing_from_held': {}, 'finite_material_shortfall_even_with_remaining_salvage': {}}; busy actor time 8.0%.
- **old_highway / 6 / balanced:** tool D1 02:17.00, bed D1 04:00.00, bed D1 06:01.00, bed D2 05:12.00, biogenerator D2 06:49.00, barrow D2 07:56.00, backpack D3 06:11.00. Remaining Metal/Basic/Advanced: 14/10/3; crops {'potato': 4, 'glowcap': 1, 'marrowvine': 2}; next investment {'next_goal': None, 'in_progress': False, 'missing_from_held': {}, 'finite_material_shortfall_even_with_remaining_salvage': {}}; busy actor time 7.6%.
- **overgrown_valley / 2 / infrastructure:** bed D1 03:18.00, biogenerator D1 05:48.00, tool D2 02:08.00. Remaining Metal/Basic/Advanced: 15/3/0; crops {'potato': 2, 'marrowvine': 2, 'glowcap': 1}; next investment {'next_goal': None, 'in_progress': False, 'missing_from_held': {}, 'finite_material_shortfall_even_with_remaining_salvage': {}}; busy actor time 9.9%.
- **overgrown_valley / 4 / infrastructure:** bed D1 03:11.00, bed D1 05:12.00, biogenerator D1 07:08.00. Remaining Metal/Basic/Advanced: 2/6/0; crops {'glowcap': 2, 'potato': 2, 'marrowvine': 2}; next investment {'next_goal': 'tool', 'in_progress': False, 'missing_from_held': {'metal': 3, 'basic': 0}, 'finite_material_shortfall_even_with_remaining_salvage': {}}; busy actor time 5.5%.
- **overgrown_valley / 6 / infrastructure:** bed D1 03:11.00, bed D1 05:12.00, bed D1 07:13.00, tool D1 08:13.00, biogenerator D2 04:23.00. Remaining Metal/Basic/Advanced: 3/6/1; crops {'potato': 4, 'glowcap': 1, 'marrowvine': 2}; next investment {'next_goal': None, 'in_progress': False, 'missing_from_held': {}, 'finite_material_shortfall_even_with_remaining_salvage': {}}; busy actor time 5.6%.
- **overgrown_valley / 2 / exploration:** tool D1 02:24.00, barrow D1 04:45.00, backpack D2 02:11.00, bed D2 05:25.00, biogenerator D3 04:52.00. Remaining Metal/Basic/Advanced: 11/4/2; crops {'potato': 2, 'marrowvine': 2, 'glowcap': 1}; next investment {'next_goal': None, 'in_progress': False, 'missing_from_held': {}, 'finite_material_shortfall_even_with_remaining_salvage': {}}; busy actor time 11.2%.
- **overgrown_valley / 4 / exploration:** tool D1 02:17.00, barrow D1 03:25.00, backpack D1 04:14.00, bed D1 06:37.00, bed D1 08:38.00, biogenerator D2 04:30.00. Remaining Metal/Basic/Advanced: 5/7/2; crops {'glowcap': 2, 'potato': 2, 'marrowvine': 2}; next investment {'next_goal': None, 'in_progress': False, 'missing_from_held': {}, 'finite_material_shortfall_even_with_remaining_salvage': {}}; busy actor time 7.1%.
- **overgrown_valley / 6 / exploration:** tool D1 02:17.00, barrow D1 03:25.00, backpack D1 04:14.00, bed D1 06:10.00, bed D1 08:11.00, bed D2 05:22.00. Remaining Metal/Basic/Advanced: 9/11/1; crops {'potato': 4, 'glowcap': 1, 'marrowvine': 2}; next investment {'next_goal': 'biogenerator', 'in_progress': False, 'missing_from_held': {'metal': 1, 'basic': 0}, 'finite_material_shortfall_even_with_remaining_salvage': {'metal': 1.0}}; busy actor time 5.6%.
- **overgrown_valley / 2 / balanced:** tool D1 02:24.00, bed D1 05:20.00, biogenerator D2 02:54.00, barrow D2 04:39.00, backpack D3 03:46.00. Remaining Metal/Basic/Advanced: 11/4/0; crops {'potato': 2, 'marrowvine': 2, 'glowcap': 1}; next investment {'next_goal': None, 'in_progress': False, 'missing_from_held': {}, 'finite_material_shortfall_even_with_remaining_salvage': {}}; busy actor time 11.3%.
- **overgrown_valley / 4 / balanced:** tool D1 02:17.00, bed D1 04:00.00, bed D1 06:01.00, biogenerator D1 07:57.00, barrow D1 09:04.00. Remaining Metal/Basic/Advanced: 13/9/2; crops {'glowcap': 2, 'potato': 2, 'marrowvine': 2}; next investment {'next_goal': 'backpack', 'in_progress': False, 'missing_from_held': {'metal': 0, 'basic': 0}, 'finite_material_shortfall_even_with_remaining_salvage': {}}; busy actor time 6.7%.
- **overgrown_valley / 6 / balanced:** tool D1 02:17.00, bed D1 04:00.00, bed D1 06:01.00, bed D1 08:02.00, biogenerator D1 09:51.00, barrow D2 03:53.00. Remaining Metal/Basic/Advanced: 7/9/2; crops {'potato': 4, 'glowcap': 1, 'marrowvine': 2}; next investment {'next_goal': 'backpack', 'in_progress': False, 'missing_from_held': {'metal': 1, 'basic': 0}, 'finite_material_shortfall_even_with_remaining_salvage': {'metal': 1.0}}; busy actor time 6.5%.
- **industrial_hollow / 2 / infrastructure:** bed D1 05:14.00, biogenerator D1 07:10.00, tool D1 08:10.00. Remaining Metal/Basic/Advanced: 49/20/2; crops {'potato': 2, 'marrowvine': 2, 'glowcap': 1}; next investment {'next_goal': None, 'in_progress': False, 'missing_from_held': {}, 'finite_material_shortfall_even_with_remaining_salvage': {}}; busy actor time 11.9%.
- **industrial_hollow / 4 / infrastructure:** tool D1 02:38.00, bed D1 04:55.00, bed D2 02:26.00, biogenerator D2 04:22.00. Remaining Metal/Basic/Advanced: 28/15/2; crops {'potato': 3, 'glowcap': 1, 'marrowvine': 2}; next investment {'next_goal': None, 'in_progress': False, 'missing_from_held': {}, 'finite_material_shortfall_even_with_remaining_salvage': {}}; busy actor time 8.1%.
- **industrial_hollow / 6 / infrastructure:** tool D1 02:17.00, bed D1 04:34.00, bed D2 02:26.00, bed D4 03:36.00, biogenerator D4 05:32.00. Remaining Metal/Basic/Advanced: 22/15/2; crops {'potato': 4, 'glowcap': 1, 'marrowvine': 2}; next investment {'next_goal': None, 'in_progress': False, 'missing_from_held': {}, 'finite_material_shortfall_even_with_remaining_salvage': {}}; busy actor time 5.9%.
- **industrial_hollow / 2 / exploration:** tool D1 02:22.00, barrow D1 04:46.00, backpack D1 05:35.00, bed D2 03:08.00, biogenerator D2 06:04.00. Remaining Metal/Basic/Advanced: 31/16/5; crops {'potato': 2, 'marrowvine': 2, 'glowcap': 1}; next investment {'next_goal': None, 'in_progress': False, 'missing_from_held': {}, 'finite_material_shortfall_even_with_remaining_salvage': {}}; busy actor time 12.5%.
- **industrial_hollow / 4 / exploration:** tool D1 02:17.00, barrow D1 03:59.00, backpack D1 04:48.00, bed D1 06:25.00, bed D2 02:30.00, biogenerator D2 04:26.00. Remaining Metal/Basic/Advanced: 10/11/2; crops {'potato': 2, 'glowcap': 2, 'marrowvine': 2}; next investment {'next_goal': None, 'in_progress': False, 'missing_from_held': {}, 'finite_material_shortfall_even_with_remaining_salvage': {}}; busy actor time 8.6%.
- **industrial_hollow / 6 / exploration:** tool D1 02:17.00, barrow D1 03:59.00, backpack D1 04:48.00, bed D1 06:25.00, bed D2 02:30.00, bed D4 03:36.00, biogenerator D4 05:32.00. Remaining Metal/Basic/Advanced: 19/16/4; crops {'potato': 4, 'glowcap': 1, 'marrowvine': 2}; next investment {'next_goal': None, 'in_progress': False, 'missing_from_held': {}, 'finite_material_shortfall_even_with_remaining_salvage': {}}; busy actor time 6.6%.
- **industrial_hollow / 2 / balanced:** tool D1 02:22.00, bed D1 05:21.00, biogenerator D1 07:17.00, barrow D2 02:18.00, backpack D2 04:39.00. Remaining Metal/Basic/Advanced: 31/16/4; crops {'potato': 2, 'marrowvine': 2, 'glowcap': 1}; next investment {'next_goal': None, 'in_progress': False, 'missing_from_held': {}, 'finite_material_shortfall_even_with_remaining_salvage': {}}; busy actor time 12.6%.
- **industrial_hollow / 4 / balanced:** tool D1 02:17.00, bed D1 04:34.00, bed D2 02:26.00, biogenerator D2 04:22.00, barrow D2 05:29.00, backpack D2 06:18.00. Remaining Metal/Basic/Advanced: 10/11/2; crops {'potato': 2, 'glowcap': 2, 'marrowvine': 2}; next investment {'next_goal': None, 'in_progress': False, 'missing_from_held': {}, 'finite_material_shortfall_even_with_remaining_salvage': {}}; busy actor time 8.3%.
- **industrial_hollow / 6 / balanced:** tool D1 02:17.00, bed D1 04:34.00, bed D2 02:26.00, bed D4 03:36.00, biogenerator D4 05:32.00, barrow D4 07:01.00, backpack D4 08:02.00. Remaining Metal/Basic/Advanced: 4/11/2; crops {'potato': 4, 'glowcap': 1, 'marrowvine': 2}; next investment {'next_goal': None, 'in_progress': False, 'missing_from_held': {}, 'finite_material_shortfall_even_with_remaining_salvage': {}}; busy actor time 6.2%.

Candidate costs are retained in [candidate-investments.csv](candidate-investments.csv). Every one of those ten projects is **unavailable for operational simulation**: effects, prerequisites or timing are unspecified. Three independent columns test standalone initial-world affordability, gross recovered/produced material coverage, and current uncommitted held material coverage. None means a completed investment. Advanced Parts have no active spending sink. Battery + efficiency + vehicle + resurrection together need six Advanced Parts, exceeding even Industrial Hollow’s five.

Exploration-first’s tool/cart/backpack, three added beds and one biomass generator require **51 Metal**, against Valley’s 50. This is an investment-bundle bottleneck, not necessarily a rescue failure. Finite Fuel and regenerative wild Biomass can support rescue independently of a completed underground loop.

## Representative daily ledgers

The following nine balanced-policy ledgers cover every world and group size. All 81 complete chronological ledgers are in [daily.csv](daily.csv), including resource recovery, production, consumption, reserves, upgrades and actor time. Daily net Food includes the initial personal allocation only on Day 1; negative flow can draw down an existing reserve.

### old_highway, 2 players

| Day | Crop Food | Field Food | Supply − need | Missed | Food reserve | Fuel burned | Biomass burned | Biomass reserve | Wild regrowth (cumulative) | Signal days |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| 1 | 0 | 7 | +9 | 0 | 9 | 0.659 | 0.000 | 3.000 | 1.27 | 0.962 |
| 2 | 0 | 4 | +2 | 0 | 11 | 0.785 | 0.000 | 4.000 | 4.16 | 1.962 |
| 3 | 6 | 1 | +5 | 0 | 16 | 0.660 | 0.000 | 7.000 | 7.80 | 2.962 |
| 4 | 4 | 0 | +2 | 0 | 18 | 0.110 | 1.643 | 10.357 | 11.55 | 3.962 |
| 5 | 6 | 0 | +4 | 0 | 22 | 0.000 | 2.000 | 11.357 | 13.75 | 4.962 |
| 6 | 0 | 0 | -2 | 0 | 20 | 0.000 | 2.000 | 9.357 | 15.83 | 5.962 |
| 7 | 8 | 0 | +6 | 0 | 26 | 0.000 | 2.000 | 11.357 | 16.00 | 6.962 |
| 8 | 0 | 0 | -2 | 0 | 24 | 0.000 | 2.000 | 12.357 | 16.00 | 7.962 |
| 9 | 6 | 0 | +4 | 0 | 28 | 0.000 | 2.000 | 10.357 | 16.00 | 8.962 |
| 10 | 2 | 0 | +0 | 0 | 28 | 0.000 | 2.000 | 12.357 | 16.00 | 9.962 |
| 11 | 6 | 0 | +4 | 0 | 32 | 0.000 | 2.000 | 13.357 | 16.00 | 10.962 |
| 12 | 0 | 0 | -2 | 0 | 30 | 0.000 | 2.000 | 11.357 | 16.00 | 11.962 |
| 13 | 8 | 0 | +6 | 0 | 36 | 0.000 | 2.000 | 13.357 | 16.00 | 12.962 |
| 14 | 0 | 0 | -2 | 0 | 34 | 0.000 | 2.000 | 14.357 | 16.00 | 13.962 |

### old_highway, 4 players

| Day | Crop Food | Field Food | Supply − need | Missed | Food reserve | Fuel burned | Biomass burned | Biomass reserve | Wild regrowth (cumulative) | Signal days |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| 1 | 0 | 12 | +16 | 0 | 16 | 0.721 | 0.174 | 1.826 | 3.39 | 0.962 |
| 2 | 0 | 0 | -4 | 0 | 12 | 0.166 | 1.690 | 3.136 | 7.14 | 1.962 |
| 3 | 6 | 0 | +2 | 0 | 14 | 0.033 | 1.936 | 4.201 | 10.89 | 2.962 |
| 4 | 4 | 0 | +0 | 0 | 14 | 0.000 | 2.000 | 10.201 | 14.64 | 3.962 |
| 5 | 6 | 0 | +2 | 0 | 16 | 0.000 | 2.000 | 8.201 | 18.19 | 4.962 |
| 6 | 3 | 0 | -1 | 0 | 15 | 0.000 | 2.000 | 6.201 | 19.00 | 5.962 |
| 7 | 8 | 0 | +4 | 0 | 19 | 0.000 | 2.000 | 11.201 | 19.00 | 6.962 |
| 8 | 3 | 0 | -1 | 0 | 18 | 0.000 | 2.000 | 9.201 | 19.00 | 7.962 |
| 9 | 6 | 0 | +2 | 0 | 20 | 0.000 | 2.000 | 7.201 | 19.00 | 8.962 |
| 10 | 5 | 0 | +1 | 0 | 21 | 0.000 | 2.000 | 12.201 | 19.00 | 9.962 |
| 11 | 6 | 0 | +2 | 0 | 23 | 0.000 | 2.000 | 10.201 | 19.00 | 10.962 |
| 12 | 3 | 0 | -1 | 0 | 22 | 0.000 | 2.000 | 8.201 | 19.00 | 11.962 |
| 13 | 8 | 0 | +4 | 0 | 26 | 0.000 | 2.000 | 13.201 | 19.00 | 12.962 |
| 14 | 3 | 0 | -1 | 0 | 25 | 0.000 | 2.000 | 11.201 | 19.00 | 13.962 |

### old_highway, 6 players

| Day | Crop Food | Field Food | Supply − need | Missed | Food reserve | Fuel burned | Biomass burned | Biomass reserve | Wild regrowth (cumulative) | Signal days |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| 1 | 0 | 12 | +18 | 0 | 18 | 0.757 | 0.000 | 2.000 | 3.42 | 0.962 |
| 2 | 0 | 0 | -6 | 0 | 12 | 0.754 | 0.000 | 0.000 | 7.17 | 1.962 |
| 3 | 6 | 0 | +0 | 0 | 12 | 0.223 | 1.428 | 1.572 | 10.92 | 2.962 |
| 4 | 4 | 0 | -2 | 0 | 10 | 0.000 | 2.000 | 11.572 | 14.67 | 3.962 |
| 5 | 9 | 0 | +3 | 0 | 13 | 0.033 | 1.936 | 11.637 | 18.42 | 4.962 |
| 6 | 0 | 0 | -6 | 0 | 7 | 0.000 | 2.000 | 9.637 | 22.17 | 5.962 |
| 7 | 13 | 0 | +7 | 0 | 14 | 0.000 | 2.000 | 21.637 | 25.92 | 6.962 |
| 8 | 0 | 0 | -6 | 0 | 8 | 0.000 | 2.000 | 19.637 | 29.67 | 7.962 |
| 9 | 12 | 0 | +6 | 0 | 14 | 0.000 | 2.000 | 17.637 | 31.62 | 8.962 |
| 10 | 2 | 0 | -4 | 0 | 10 | 0.000 | 2.000 | 22.637 | 32.00 | 9.962 |
| 11 | 12 | 0 | +6 | 0 | 16 | 0.000 | 2.000 | 20.637 | 32.00 | 10.962 |
| 12 | 0 | 0 | -6 | 0 | 10 | 0.000 | 2.000 | 18.637 | 32.00 | 11.962 |
| 13 | 14 | 0 | +8 | 0 | 18 | 0.000 | 2.000 | 23.637 | 32.00 | 12.962 |
| 14 | 0 | 0 | -6 | 0 | 12 | 0.000 | 2.000 | 21.637 | 32.00 | 13.962 |

### overgrown_valley, 2 players

| Day | Crop Food | Field Food | Supply − need | Missed | Food reserve | Fuel burned | Biomass burned | Biomass reserve | Wild regrowth (cumulative) | Signal days |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| 1 | 0 | 11 | +13 | 0 | 13 | 0.727 | 0.000 | 4.000 | 2.67 | 0.962 |
| 2 | 0 | 5 | +3 | 0 | 16 | 0.724 | 0.000 | 10.000 | 7.25 | 1.962 |
| 3 | 6 | 2 | +6 | 0 | 22 | 0.685 | 0.000 | 16.000 | 12.83 | 2.962 |
| 4 | 4 | 0 | +2 | 0 | 24 | 0.110 | 1.643 | 19.357 | 16.63 | 3.962 |
| 5 | 6 | 0 | +4 | 0 | 28 | 0.000 | 2.000 | 17.357 | 19.59 | 4.962 |
| 6 | 0 | 0 | -2 | 0 | 26 | 0.000 | 2.000 | 15.357 | 21.86 | 5.962 |
| 7 | 8 | 0 | +6 | 0 | 32 | 0.000 | 2.000 | 20.357 | 22.00 | 6.962 |
| 8 | 0 | 0 | -2 | 0 | 30 | 0.000 | 2.000 | 18.357 | 22.00 | 7.962 |
| 9 | 6 | 0 | +4 | 0 | 34 | 0.000 | 2.000 | 16.357 | 22.00 | 8.962 |
| 10 | 2 | 0 | +0 | 0 | 34 | 0.000 | 2.000 | 21.357 | 22.00 | 9.962 |
| 11 | 6 | 0 | +4 | 0 | 38 | 0.000 | 2.000 | 19.357 | 22.00 | 10.962 |
| 12 | 0 | 0 | -2 | 0 | 36 | 0.000 | 2.000 | 17.357 | 22.00 | 11.962 |
| 13 | 8 | 0 | +6 | 0 | 42 | 0.000 | 2.000 | 22.357 | 22.00 | 12.962 |
| 14 | 0 | 0 | -2 | 0 | 40 | 0.000 | 2.000 | 20.357 | 22.00 | 13.962 |

### overgrown_valley, 4 players

| Day | Crop Food | Field Food | Supply − need | Missed | Food reserve | Fuel burned | Biomass burned | Biomass reserve | Wild regrowth (cumulative) | Signal days |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| 1 | 0 | 18 | +22 | 0 | 22 | 0.678 | 0.263 | 9.737 | 5.12 | 0.962 |
| 2 | 0 | 0 | -4 | 0 | 18 | 0.000 | 2.000 | 7.737 | 10.83 | 1.962 |
| 3 | 6 | 0 | +2 | 0 | 20 | 0.000 | 2.000 | 5.737 | 16.55 | 2.962 |
| 4 | 4 | 0 | +0 | 0 | 20 | 0.000 | 2.000 | 11.737 | 19.64 | 3.962 |
| 5 | 6 | 0 | +2 | 0 | 22 | 0.000 | 2.000 | 9.737 | 20.00 | 4.962 |
| 6 | 0 | 0 | -4 | 0 | 18 | 0.000 | 2.000 | 7.737 | 20.00 | 5.962 |
| 7 | 10 | 0 | +6 | 0 | 24 | 0.000 | 2.000 | 13.737 | 20.00 | 6.962 |
| 8 | 0 | 0 | -4 | 0 | 20 | 0.000 | 2.000 | 11.737 | 20.00 | 7.962 |
| 9 | 6 | 0 | +2 | 0 | 22 | 0.000 | 2.000 | 9.737 | 20.00 | 8.962 |
| 10 | 4 | 0 | +0 | 0 | 22 | 0.000 | 2.000 | 15.737 | 20.00 | 9.962 |
| 11 | 6 | 0 | +2 | 0 | 24 | 0.000 | 2.000 | 13.737 | 20.00 | 10.962 |
| 12 | 0 | 0 | -4 | 0 | 20 | 0.000 | 2.000 | 11.737 | 20.00 | 11.962 |
| 13 | 10 | 0 | +6 | 0 | 26 | 0.000 | 2.000 | 17.737 | 20.00 | 12.962 |
| 14 | 0 | 0 | -4 | 0 | 22 | 0.000 | 2.000 | 15.737 | 20.00 | 13.962 |

### overgrown_valley, 6 players

| Day | Crop Food | Field Food | Supply − need | Missed | Food reserve | Fuel burned | Biomass burned | Biomass reserve | Wild regrowth (cumulative) | Signal days |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| 1 | 0 | 18 | +24 | 0 | 24 | 0.776 | 0.000 | 5.000 | 5.22 | 0.962 |
| 2 | 0 | 0 | -6 | 0 | 18 | 0.081 | 1.854 | 9.146 | 10.93 | 1.962 |
| 3 | 6 | 0 | +0 | 0 | 18 | 0.068 | 1.869 | 10.277 | 16.65 | 2.962 |
| 4 | 4 | 0 | -2 | 0 | 16 | 0.000 | 2.000 | 19.277 | 22.36 | 3.962 |
| 5 | 9 | 0 | +3 | 0 | 19 | 0.000 | 2.000 | 17.277 | 28.07 | 4.962 |
| 6 | 3 | 0 | -3 | 0 | 16 | 0.000 | 2.000 | 15.277 | 32.88 | 5.962 |
| 7 | 11 | 0 | +5 | 0 | 21 | 0.000 | 2.000 | 20.277 | 34.00 | 6.962 |
| 8 | 3 | 0 | -3 | 0 | 18 | 0.000 | 2.000 | 18.277 | 34.00 | 7.962 |
| 9 | 9 | 0 | +3 | 0 | 21 | 0.000 | 2.000 | 16.277 | 34.00 | 8.962 |
| 10 | 5 | 0 | -1 | 0 | 20 | 0.000 | 2.000 | 21.277 | 34.00 | 9.962 |
| 11 | 9 | 0 | +3 | 0 | 23 | 0.000 | 2.000 | 19.277 | 34.00 | 10.962 |
| 12 | 3 | 0 | -3 | 0 | 20 | 0.000 | 2.000 | 17.277 | 34.00 | 11.962 |
| 13 | 11 | 0 | +5 | 0 | 25 | 0.000 | 2.000 | 22.277 | 34.00 | 12.962 |
| 14 | 3 | 0 | -3 | 0 | 22 | 0.000 | 2.000 | 20.277 | 34.00 | 13.962 |

### industrial_hollow, 2 players

| Day | Crop Food | Field Food | Supply − need | Missed | Food reserve | Fuel burned | Biomass burned | Biomass reserve | Wild regrowth (cumulative) | Signal days |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| 1 | 0 | 7 | +9 | 0 | 9 | 0.753 | 0.000 | 0.000 | 1.14 | 0.962 |
| 2 | 0 | 4 | +2 | 0 | 11 | 0.706 | 0.000 | 5.000 | 3.66 | 1.962 |
| 3 | 6 | 1 | +5 | 0 | 16 | 0.660 | 0.000 | 7.000 | 6.50 | 2.962 |
| 4 | 4 | 0 | +2 | 0 | 18 | 0.110 | 1.643 | 13.357 | 9.33 | 3.962 |
| 5 | 6 | 0 | +4 | 0 | 22 | 0.000 | 2.000 | 11.357 | 12.16 | 4.962 |
| 6 | 0 | 0 | -2 | 0 | 20 | 0.000 | 2.000 | 9.357 | 14.63 | 5.962 |
| 7 | 8 | 0 | +6 | 0 | 26 | 0.000 | 2.000 | 14.357 | 16.00 | 6.962 |
| 8 | 0 | 0 | -2 | 0 | 24 | 0.000 | 2.000 | 12.357 | 16.00 | 7.962 |
| 9 | 6 | 0 | +4 | 0 | 28 | 0.000 | 2.000 | 10.357 | 16.00 | 8.962 |
| 10 | 2 | 0 | +0 | 0 | 28 | 0.000 | 2.000 | 15.357 | 16.00 | 9.962 |
| 11 | 6 | 0 | +4 | 0 | 32 | 0.000 | 2.000 | 13.357 | 16.00 | 10.962 |
| 12 | 0 | 0 | -2 | 0 | 30 | 0.000 | 2.000 | 11.357 | 16.00 | 11.962 |
| 13 | 8 | 0 | +6 | 0 | 36 | 0.000 | 2.000 | 16.357 | 16.00 | 12.962 |
| 14 | 0 | 0 | -2 | 0 | 34 | 0.000 | 2.000 | 14.357 | 16.00 | 13.962 |

### industrial_hollow, 4 players

| Day | Crop Food | Field Food | Supply − need | Missed | Food reserve | Fuel burned | Biomass burned | Biomass reserve | Wild regrowth (cumulative) | Signal days |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| 1 | 0 | 12 | +16 | 0 | 16 | 0.717 | 0.000 | 4.000 | 2.33 | 0.962 |
| 2 | 0 | 0 | -4 | 0 | 12 | 0.682 | 0.542 | 0.458 | 5.16 | 1.962 |
| 3 | 6 | 0 | +2 | 0 | 14 | 0.097 | 1.730 | 1.728 | 7.99 | 2.962 |
| 4 | 4 | 0 | +0 | 0 | 14 | 0.000 | 2.000 | 7.728 | 10.83 | 3.962 |
| 5 | 6 | 0 | +2 | 0 | 16 | 0.000 | 2.000 | 8.728 | 13.66 | 4.962 |
| 6 | 0 | 0 | -4 | 0 | 12 | 0.000 | 2.000 | 6.728 | 16.49 | 5.962 |
| 7 | 10 | 0 | +6 | 0 | 18 | 0.000 | 2.000 | 9.728 | 18.00 | 6.962 |
| 8 | 0 | 0 | -4 | 0 | 14 | 0.000 | 2.000 | 10.728 | 18.00 | 7.962 |
| 9 | 6 | 0 | +2 | 0 | 16 | 0.000 | 2.000 | 8.728 | 18.00 | 8.962 |
| 10 | 4 | 0 | +0 | 0 | 16 | 0.000 | 2.000 | 11.728 | 18.00 | 9.962 |
| 11 | 6 | 0 | +2 | 0 | 18 | 0.000 | 2.000 | 12.728 | 18.00 | 10.962 |
| 12 | 0 | 0 | -4 | 0 | 14 | 0.000 | 2.000 | 10.728 | 18.00 | 11.962 |
| 13 | 10 | 0 | +6 | 0 | 20 | 0.000 | 2.000 | 13.728 | 18.00 | 12.962 |
| 14 | 0 | 0 | -4 | 0 | 16 | 0.000 | 2.000 | 14.728 | 18.00 | 13.962 |

### industrial_hollow, 6 players

| Day | Crop Food | Field Food | Supply − need | Missed | Food reserve | Fuel burned | Biomass burned | Biomass reserve | Wild regrowth (cumulative) | Signal days |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| 1 | 0 | 12 | +18 | 0 | 18 | 0.729 | 0.000 | 4.000 | 2.49 | 0.962 |
| 2 | 0 | 0 | -6 | 0 | 12 | 0.705 | 0.000 | 2.000 | 5.32 | 1.962 |
| 3 | 6 | 0 | +0 | 0 | 12 | 0.693 | 0.000 | 2.000 | 8.15 | 2.962 |
| 4 | 4 | 0 | -2 | 0 | 10 | 0.562 | 0.632 | 5.368 | 10.99 | 3.962 |
| 5 | 6 | 0 | +0 | 0 | 10 | 0.000 | 2.000 | 6.368 | 13.82 | 4.962 |
| 6 | 6 | 0 | +0 | 0 | 10 | 0.000 | 2.000 | 4.368 | 16.65 | 5.962 |
| 7 | 8 | 0 | +2 | 0 | 12 | 0.000 | 2.000 | 6.368 | 18.37 | 6.962 |
| 8 | 6 | 0 | +0 | 0 | 12 | 0.000 | 2.000 | 7.368 | 19.00 | 7.962 |
| 9 | 6 | 0 | +0 | 0 | 12 | 0.000 | 2.000 | 5.368 | 19.00 | 8.962 |
| 10 | 8 | 0 | +2 | 0 | 14 | 0.000 | 2.000 | 7.368 | 19.00 | 9.962 |
| 11 | 6 | 0 | +0 | 0 | 14 | 0.000 | 2.000 | 8.368 | 19.00 | 10.962 |
| 12 | 6 | 0 | +0 | 0 | 14 | 0.000 | 2.000 | 6.368 | 19.00 | 11.962 |
| 13 | 8 | 0 | +2 | 0 | 16 | 0.000 | 2.000 | 8.368 | 19.00 | 12.962 |
| 14 | 6 | 0 | +0 | 0 | 16 | 0.000 | 2.000 | 9.368 | 19.00 | 13.962 |

## Interpretation limits and recommendations

- Read **rescue**, **Food coverage**, **renewable-power capacity**, **stored reserves**, and **confirmed underground continuation** separately. The machine or crop count alone never certifies independence.
- Wild patches regrow gradually and are a continuing surface opportunity. Initial generated stock, later regrowth, recovered stock and underground crop production are different ledger quantities. Reserve-origin lower bounds are conservative (all fungible consumption charged against crop output first); they are not a claim that a particular carried unit has a tracked physical origin.
- Short M6a journeys, no combat, no fatigue/sleep, and coordinated scheduling make these optimistic logistical scenarios. The distant sixth route and collection/assembly durations are provisional, not measured gameplay.
- The sustainability witness conservatively rounds crop service intervals to 1300/1900 seconds and verifies an exact repeated inventory/crop/meal-phase/hopper state. Its batched maintenance schedule is a feasible continuation, not a replay of multiplayer behavior. The additional 6.5-day operational confirmation tests the actual policy before assigning the confirmed date.
- Proportional generation sharing preserves Fuel burn whenever its generator remains running. Manual Fuel shutdown is essential to renewable-only operation. Undefined battery/efficiency effects are deliberately not used to improve outcomes.
- Evaluate whether observed independence dates, resource leftovers and labor utilization match the intended late-game investment. Do not adjust costs or world totals automatically. Define the missing project effects before assessing Advanced Parts progression, vehicles, combat, batteries or resurrection value.

Reproduce and inspect assumptions using [the study guide](../README.md).
