#!/usr/bin/env python3
"""Reproduce the isolated 27-baseline / 54-paired-setback study."""
import argparse
import csv
import gzip
import hashlib
import io
import json
from pathlib import Path
import sys
from model import HERE, ROOT, RESOURCES, load_config, make_sites, number, stamp
from engine import Study


def encode(value):
    return json.dumps(value,sort_keys=True,separators=(',',':'),ensure_ascii=True)


def csv_file(path,rows):
    rows=list(rows)
    if not rows:return
    with path.open('w',newline='') as f:
        out=csv.DictWriter(f,fieldnames=list(rows[0]));out.writeheader()
        out.writerows({k:encode(v) if isinstance(v,(dict,list)) else v for k,v in row.items()} for row in rows)


def write_events(path,events):
    with path.open('wb') as raw:
        with gzip.GzipFile(filename='',mode='wb',fileobj=raw,mtime=0) as f:
            for e in events:f.write((encode(e)+'\n').encode())


def source_manifest(config_path):
    inputs=[config_path,*HERE.glob('*.py'),ROOT/'docs/measurements/region/travel-1701.json',
            *[ROOT/f'data/{n}.json' for n in ('farming','power','hunger','rescue','crafting','supplies')]]
    return {str(p.relative_to(ROOT)) if p.is_relative_to(ROOT) else str(p):hashlib.sha256(p.read_bytes()).hexdigest() for p in sorted(set(inputs))}


def candidate_rows(config,result):
    initial=dict(result['initial_world']);initial['fuel']+=config['starting_storage']['fuel']
    for name,cost in config['candidate_projects'].items():
        held={k:max(0,result['held'].get(k,0)-result['reserved'].get(k,0)) for k in cost}
        gross={k:result['recovered'].get(k,0)+result['crop_and_propagation_output'].get(k,0)+config['starting_storage'].get(k,0) for k in cost}
        yield {'scenario':result['scenario'],'project':name,'status':'unavailable_effects_prerequisites_and_timing_unspecified',
               'cost':cost,'initial_world_materials_cover_cost':all(initial.get(k,0)>=n for k,n in cost.items()),
               'gross_recovered_and_produced_cover_cost':all(gross.get(k,0)>=n for k,n in cost.items()),
               'uncommitted_held_materials_cover_cost':all(held[k]>=n for k,n in cost.items()),
               'missing_from_held':{k:max(0,n-held[k]) for k,n in cost.items()},'completed':False}


def summary_row(r):
    return {k:r[k] for k in ('scenario','world','players','strategy','setback','outcome','date','second','missed_meals',
              'signal_days','lost_days','outage_seconds','food_reserve','fuel_reserve','biomass_reserve',
              'food_capacity_reserve_second','power_capacity_reserve_second','feasible_underground_second',
              'underground_self_sufficient_second','first_confirmed_second','investment_status','busy_fraction','crop_food_reserve_lower_bound','crop_biomass_reserve_lower_bound',
              'setback_triggered','setback_second','seed_opportunities','discoveries','activity','projects','rejections')}


def markdown(config,results,out):
    base=[r for r in results if r['setback']=='baseline'];paired={r['scenario']:r for r in results}
    fmt=lambda t:stamp(t) if t is not None else 'Not confirmed'
    lines=['# 14-day candidate economy: reproducible validation study','',
      'This is an isolated numerical experiment, not the Gameplay & Economy Brief or a playable balance change. '
      'All reported results use the frozen configuration, one deterministic seed and the documented policies. '
      'They do not establish real multiplayer balance.','',
      '## Baseline results','',
      '| World | Players | Strategy | Outcome / calendar date | Missed meals | Outage / loss | Food / Fuel / Biomass reserve | Underground operation confirmed |',
      '|---|---:|---|---|---:|---|---|---|']
    for r in base:
        lines.append(f"| {r['world']} | {r['players']} | {r['strategy']} | {r['outcome']} {r['date']} | {r['missed_meals']} | {r['outage_seconds']:.1f}s / {r['lost_days']:.4f}d | {r['food_reserve']} / {r['fuel_reserve']:.3f} / {r['biomass_reserve']:.3f} | {fmt(r['underground_self_sufficient_second'])} |")
    successes=sum(r['rescued'] for r in base);confirmed=sum(r['underground_self_sufficient_second'] is not None for r in base)
    lines+=['',f'**{successes}/27 baseline rescues; {confirmed}/27 confirmed underground continuations.** '
            'Confirmation requires a repeatable no-surface/no-Fuel schedule witness AND at least 6.5 days of actual operation '
            'without new surface Food/Fuel/Biomass, missed meals, outages or sub-day reserves, ending with non-decreasing Food/Biomass stocks. '
            'The earlier feasible-certificate date and necessary capacity/reserve dates are separate CSV fields.','',
            '## Paired setbacks','',
            '| Event | Triggered | Rescued | Additional missed meals | Largest rescue delay | Confirmed underground operation |',
            '|---|---:|---:|---:|---:|---:|']
    for name in config['setbacks'][1:]:
        rows=[r for r in results if r['setback']==name]
        originals=[paired[r['scenario'].replace('__'+name,'__baseline')] for r in rows]
        delay=max((r['second']-b['second'] for r,b in zip(rows,originals) if r['rescued'] and b['rescued']),default=0)
        lines.append(f"| {name} | {sum(r['setback_triggered'] for r in rows)}/27 | {sum(r['rescued'] for r in rows)}/27 | {sum(r['missed_meals']-b['missed_meals'] for r,b in zip(rows,originals)):+d} | {delay:.1f}s | {sum(r['underground_self_sufficient_second'] is not None for r in rows)}/27 |")
    lines+=['','See [paired-setbacks.csv](paired-setbacks.csv) for each matching configuration. '
            'An untriggered setback is not counted as successful recovery. Failed expeditions leave site inventories intact. '
            'A cutoff is not an assertion of permanent defeat; model exceptions are simulation errors and terminate the runner.','',
            '## Investments and bottlenecks','']
    for r in base:
        projects=', '.join(f"{u['project']} {u['date']}" for u in r['projects']) or 'none'
        lines.append(f"- **{r['world']} / {r['players']} / {r['strategy']}:** {projects}. Remaining Metal/Basic/Advanced: {r['held'].get('metal',0)}/{r['held'].get('basic',0)}/{r['held'].get('advanced',0)}; crops {r['crop_counts']}; next investment {r['investment_status']}; busy actor time {100*r['busy_fraction']:.1f}%.")
    lines+=['','Candidate costs are retained in [candidate-investments.csv](candidate-investments.csv). '
       'Every one of those ten projects is **unavailable for operational simulation**: effects, prerequisites or timing are unspecified. '
       'Three independent columns test standalone initial-world affordability, gross recovered/produced material coverage, '
       'and current uncommitted held material coverage. None means a completed investment. Advanced Parts have no active spending sink. '
       'Battery + efficiency + vehicle + resurrection together need six Advanced Parts, exceeding even Industrial Hollow’s five.','',
       'Exploration-first’s tool/cart/backpack, three added beds and one biomass generator require **51 Metal**, '
       'against Valley’s 50. This is an investment-bundle bottleneck, not necessarily a rescue failure. '
       'Finite Fuel and regenerative wild Biomass can support rescue independently of a completed underground loop.','',
       '## Representative daily ledgers','',
       'The following nine balanced-policy ledgers cover every world and group size. All 81 complete chronological ledgers '
       'are in [daily.csv](daily.csv), including resource recovery, production, consumption, reserves, upgrades and actor time. '
       'Daily net Food includes the initial personal allocation only on Day 1; negative flow can draw down an existing reserve.','']
    for r in base:
        if r['strategy']!='balanced':continue
        lines += [f"### {r['world']}, {r['players']} players",'',
          '| Day | Crop Food | Field Food | Supply − need | Missed | Food reserve | Fuel burned | Biomass burned | Biomass reserve | Wild regrowth (cumulative) | Signal days |',
          '|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|']
        previous=CounterRow()
        for d in r['daily']:
            farm=d['crop_and_propagation_output'].get('food',0);field=d['recovered'].get('food',0)
            net=farm-previous.farm+field-previous.field-r['players']+(2*r['players'] if d['day']==1 else 0)
            lines.append(f"| {d['day']} | {farm-previous.farm} | {field-previous.field} | {net:+d} | {d['missed_meals']-previous.missed} | {d['food_reserve']} | {d['burned']['fuel']-previous.fuel:.3f} | {d['burned']['biomass']-previous.bio:.3f} | {d['biomass_reserve']:.3f} | {d['wild_regrown']:.2f} | {d['signal_days']:.3f} |")
            previous=CounterRow(farm,field,d['missed_meals'],d['burned']['fuel'],d['burned']['biomass'])
        lines+=['']
    lines+=['## Interpretation limits and recommendations','',
      '- Read **rescue**, **Food coverage**, **renewable-power capacity**, **stored reserves**, and **confirmed underground continuation** separately. '
      'The machine or crop count alone never certifies independence.',
      '- Wild patches regrow gradually and are a continuing surface opportunity. Initial generated stock, later regrowth, '
      'recovered stock and underground crop production are different ledger quantities. Reserve-origin lower bounds are conservative '
      '(all fungible consumption charged against crop output first); they are not a claim that a particular carried unit has a tracked physical origin.',
      '- Short M6a journeys, no combat, no fatigue/sleep, and coordinated scheduling make these optimistic logistical scenarios. '
      'The distant sixth route and collection/assembly durations are provisional, not measured gameplay.',
      '- The sustainability witness conservatively rounds crop service intervals to 1300/1900 seconds and verifies an exact repeated '
      'inventory/crop/meal-phase/hopper state. Its batched maintenance schedule is a feasible continuation, not a replay of multiplayer behavior. '
      'The additional 6.5-day operational confirmation tests the actual policy before assigning the confirmed date.',
      '- Proportional generation sharing preserves Fuel burn whenever its generator remains running. Manual Fuel shutdown is essential '
      'to renewable-only operation. Undefined battery/efficiency effects are deliberately not used to improve outcomes.',
      '- Evaluate whether observed independence dates, resource leftovers and labor utilization match the intended late-game investment. '
      'Do not adjust costs or world totals automatically. Define the missing project effects before assessing Advanced Parts progression, '
      'vehicles, combat, batteries or resurrection value.',
      '', 'Reproduce and inspect assumptions using [the study guide](../README.md).']
    (out/'RESULTS.md').write_text('\n'.join(lines)+'\n')

class CounterRow:
    def __init__(self,farm=0,field=0,missed=0,fuel=0,bio=0):
        self.farm,self.field,self.missed,self.fuel,self.bio=farm,field,missed,fuel,bio


def main():
    ap=argparse.ArgumentParser(description=__doc__)
    ap.add_argument('--config',type=Path,default=HERE/'config.json')
    ap.add_argument('--output',type=Path,default=HERE/'reports')
    ap.add_argument('--baseline-only',action='store_true')
    args=ap.parse_args();config=load_config(args.config);out=args.output;out.mkdir(parents=True,exist_ok=True);(out/'events').mkdir(exist_ok=True)
    (out/'inputs.json').write_text(json.dumps({'config':config,'source_hashes':source_manifest(args.config)},indent=2,sort_keys=True)+'\n')
    locations=[]
    for world in config['worlds']:
        for site in make_sites(config,world):
            locations.append({'world':world,'site':site.id+1,'name':site.name,**{k:site.initial[k] for k in RESOURCES if k!='biomass'},
                'biomass':site.patch.capacity if site.patch else 0,'regrowth_seconds':site.patch.period if site.patch else None,
                'road':site.road,'outbound_seconds':site.out,'return_seconds':site.back,
                'mechanical_piles_require_tool':site.id>=4,'advanced_parts_require_tool':True})
    csv_file(out/'location-inventories.csv',locations)
    results=[];candidates=[];daily=[];errors=[]
    for world in config['worlds']:
        for strategy in config['strategies']:
            for n in config['players']:
                for setback in (['baseline'] if args.baseline_only else config['setbacks']):
                    s=Study(config,world,n,strategy,setback)
                    try:r=s.run()
                    except Exception as e:
                        errors.append({'scenario':s.id,'simulation_error':repr(e)});(out/'ERRORS.json').write_text(json.dumps(errors,indent=2));raise
                    goal=s.next_project(s.actors[0])
                    costs=s.cost(goal) if goal else {}
                    structural={k:max(0,n-r['held'].get(k,0)-r['remaining'].get(k,0)) for k,n in costs.items() if k in ('metal','basic','advanced')}
                    r['investment_status']={'next_goal':goal,'in_progress':s.busy_project is not None,
                        'missing_from_held':{k:max(0,n-r['held'].get(k,0)) for k,n in costs.items()},
                        'finite_material_shortfall_even_with_remaining_salvage':{k:v for k,v in structural.items() if v}}
                    r['busy_fraction']=round(sum(r['activity'].values())/(r['second']*n),6)
                    r['first_crop_harvests']={crop:min((e['second'] for e in s.events if e['event']=='harvest' and e['crop']==crop),default=None) for crop in config['crops']}
                    r['daily']=s.daily;results.append(r)
                    write_events(out/'events'/f'{s.id}.jsonl.gz',s.events)
                    daily.extend({'scenario':s.id,**d} for d in s.daily)
                    candidates.extend(candidate_rows(config,r))
                    print(f"{s.id}: {r['outcome']} {r['date']}; missed={r['missed_meals']}; confirmed={r['underground_self_sufficient_second']}",flush=True)
    csv_file(out/'summary.csv',[summary_row(r) for r in results]);csv_file(out/'daily.csv',daily);csv_file(out/'candidate-investments.csv',candidates)
    pairs=[];lookup={r['scenario']:r for r in results}
    for r in results:
        if r['setback']=='baseline':continue
        b=lookup[r['scenario'].replace('__'+r['setback'],'__baseline')]
        pairs.append({'scenario':r['scenario'],'triggered':r['setback_triggered'],'baseline_rescued':b['rescued'],'setback_rescued':r['rescued'],
          'additional_missed_meals':r['missed_meals']-b['missed_meals'],'rescue_delay_seconds':r['second']-b['second'] if r['rescued'] and b['rescued'] else None,
          'additional_outage_seconds':r['outage_seconds']-b['outage_seconds'],'additional_signal_loss_days':r['lost_days']-b['lost_days'],
          'baseline_confirmed_second':b['underground_self_sufficient_second'],'setback_confirmed_second':r['underground_self_sufficient_second']})
    csv_file(out/'paired-setbacks.csv',pairs)
    (out/'results.json').write_text(json.dumps(results,indent=2,sort_keys=True)+'\n')
    markdown(config,results,out)
    manifest={str(p.relative_to(out)):hashlib.sha256(p.read_bytes()).hexdigest() for p in sorted(out.rglob('*')) if p.is_file() and p.name!='checksums.json'}
    (out/'checksums.json').write_text(json.dumps(manifest,indent=2,sort_keys=True)+'\n')
    print(f'PASS: {len(results)} scenarios, {len(daily)} daily rows, zero simulation errors; {out}')

if __name__=='__main__':main()
