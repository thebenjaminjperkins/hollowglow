"""VS0.1 arithmetic worksheet; reads live definitions, never runs or modifies gameplay.

Two explicit real-minute schedules share values except proposed cycle/target.
Not a multiplayer, combat, movement, slot-operation or extraction simulation.
Run from repository root: python3 docs/vs0.1/timing_worksheet.py
"""
import csv
from fractions import Fraction as F
import json
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
OUT = Path(__file__).resolve().parent


def load(name):
    return json.loads((ROOT / 'data' / (name + '.json')).read_text())


def worksheet(cycle, target):
    farm, hunger, power, crafting = (load(n) for n in ('farming', 'hunger', 'power', 'crafting'))
    crop_defs = {c['id']: c for c in farm['crops']}
    # Four plots, 2/2 seeds. Start planting at 00:30 for a conservative common epoch.
    due = {key: F(1, 2) + cycle * spec['growthDays'] for key, spec in crop_defs.items()}
    crops = {key: 2 for key in crop_defs}
    personal = [hunger['startingFood']] * 2
    stored_food = organic = scrap = missed = 0
    tank = F(0)
    loose_fuel = F(power['startingFuel'])
    fuel_burned = signal = F(0)
    rows = []
    # The 08–10 bench reservation is a conservative load budget, NOT a successful craft.
    events = {F(t) for t in (0, 1, 4, 8, 9, 10, 12, 13, 16, 20)} | {F(1, 2), F(17, 4), F(9, 2)}
    events |= {cycle * k for k in range(1, int(20 / cycle) + 1)}
    for key, spec in crop_defs.items():
        t = due[key]
        while t <= 20:
            events.add(t)
            t += cycle * spec['growthDays']
    previous = F(0)
    for now in sorted(events):
        notes = []
        if previous >= 1:
            pu = 4 if 8 <= previous < 10 else 3
            rate = F(str(power['idleFuelPerDay'])) + F(str(power['fuelPerPuDay'])) * pu
            burn = rate * (now - previous) / cycle
            assert tank >= burn, ('unfunded fuel interval', now, tank, burn)
            tank -= burn
            fuel_burned += burn
            signal = min(F(target), signal + (now - previous) / cycle)
        if now >= cycle and now % cycle == 0:
            for i in range(2):
                consumed = min(personal[i], hunger['dailyFood'])
                personal[i] -= consumed
                missed += hunger['dailyFood'] - consumed
            notes.append('dawn: personal meals only')
        if now == 1:
            tank += 1
            loose_fuel -= 1
            notes.append('load/start; beacon+lights 3 PU')
        if now == 4:
            # Seed 1701 historical GARAGE profile loot. Hall expedition not measured.
            stored_food += 4
            loose_fuel += 4
            scrap += 10  # Preserve van. No invented Organic drop.
            notes.append('provisional expedition return; deposit Food4/Fuel4/Scrap10; preserve van')
        if now == F(17, 4):
            for i in range(2):
                assert stored_food >= 2
                stored_food -= 2
                personal[i] += 2
            notes.append('explicitly withdraw 2 Food each')
        if now in (F(9, 2), F(8), F(13)):
            assert loose_fuel >= 1 and tank + 1 <= power['tankCapacity']
            tank += 1
            loose_fuel -= 1
            notes.append('manual whole-unit refuel')
        for key, when in list(due.items()):
            if when == now:
                spec = crop_defs[key]
                stored_food += crops[key] * spec['yields'].get('food', 0)
                organic += crops[key] * spec['yields'].get('organic', 0)
                due[key] += cycle * spec['growthDays']
                notes.append(key + ': harvest both, retain seeds, replant; service duration idealized')
        if now == 9 and stored_food >= 6:
            stored_food -= 6
            personal = [n + 3 for n in personal]
            notes.append('explicitly withdraw 3 Food each after Potato harvest')
        if now == 8:
            notes.append('lights off, bench on: 4 PU; no ingredients/craft credited')
        if now == 10:
            notes.append('bench off, lights on: 3 PU')
        food = sum(personal) + stored_food
        assert min(personal + [stored_food, organic, scrap, loose_fuel, tank]) >= 0
        available = {r['id']: all({'scrap': scrap, 'organic': organic}.get(k, 0) >= v
                                   for k, v in r['inputs'].items())
                     for r in crafting['recipes'] if r['id'] != 'potato_propagation'}
        rows.append({'minute': float(now), 'event': '; '.join(notes), 'personal_food_A': personal[0],
                     'personal_food_B': personal[1], 'stored_food': stored_food, 'total_food': food,
                     'organic': organic, 'scrap': scrap, 'loose_fuel': float(loose_fuel),
                     'tank_fuel': float(tank), 'fuel_burned': float(fuel_burned),
                     'net_signal_days': float(signal), 'missed_meal_units': missed,
                     'bed_materials_available': available['growing_bed'],
                     'backpack_materials_available': available['backpack_extension']})
        previous = now
    return rows


def main():
    baseline_cycle = F(sum(load('clock').values()), 60)
    baseline_target = load('rescue')['targetDays']
    configs = [('unchanged', baseline_cycle, baseline_target), ('candidate-not-approved', F(4), 3)]
    summary = []
    for name, cycle, target in configs:
        rows = worksheet(cycle, target)
        with (OUT / ('timing-' + name + '.csv')).open('w', newline='') as handle:
            writer = csv.DictWriter(handle, fieldnames=rows[0].keys())
            writer.writeheader()
            writer.writerows(rows)
        last = rows[-1]
        assert last['missed_meal_units'] == 0
        assert not any(r['bed_materials_available'] or r['backpack_materials_available'] for r in rows)
        summary.append({'configuration': name, 'cycle_minutes': float(cycle), 'signal_target': target,
                        'uninterrupted_ready_minute_from_start_at_1': float(1 + cycle * target),
                        'by_minute_20': last})
    (OUT / 'timing-summary.json').write_text(json.dumps(summary, indent=2) + '\n')
    print('PASS: two chronological worksheets; personal meals, explicit resupply, exact fuel intervals; no unavailable construction credited')


if __name__ == '__main__':
    main()
