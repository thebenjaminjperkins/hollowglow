"""Conservative repeatable-schedule witness; never substitutes mean yields for reserves.

A separate continuation removes surface access and Fuel use. One worker batches
farm care into 100-second windows; a second services the hopper. Growth intervals
are conservatively rounded UP to 1300/1900 seconds, retaining replacement seeds.
A matching bounded state at the same meal phase proves a repeatable continuation.
No disposal or storage expansion is allowed. Inability to find a cycle is NOT a
proof that the actual strategy cannot become sustainable.
"""
from collections import Counter
from copy import deepcopy
from fractions import Fraction as F
import math


def capacity(study):
    c=study.counts()
    food=F(c['potato']*3*600,1300)+F(c['glowcap']*2*600,1900)
    biomass=F((c['glowcap']+c['marrowvine']*3)*600,1900)
    return food,biomass


def witness(study,limit=20000):
    food_rate,bio_rate=capacity(study)
    if food_rate<study.n or bio_rate<2:return {'certified':False,'reason':'insufficient_conservative_production_capacity'}
    if any(p.crop is None for p in study.plots):return {'certified':False,'reason':'unplanted_plot'}
    if study.busy_project is not None or any(a.trip_site is not None for a in study.actors):
        return {'certified':False,'reason':'actors_or_materials_committed'}
    bio=next((g for g in study.power.generators if g.kind=='biomass'),None)
    if not bio or not bio.running or study.power.generators[0].running or study.power.enabled!={'beacon':True,'workbench':False,'lighting':False}:
        return {'certified':False,'reason':'not_operating_on_underground_beacon_power_alone'}
    if study.total('food')<study.n or study.full_stock('biomass')<2 or bio.tank<F(1,3):
        return {'certified':False,'reason':'insufficient_actual_reserve'}
    chest=deepcopy(study.chest);people=[deepcopy(a.inventory) for a in study.actors]
    tank=F(bio.tank)
    def deposit(inv):
        for k,n in list(inv.items.items()):
            keep=2 if k=='food' else 0
            moved=min(max(0,n-keep),chest.room(k))
            inv.take(k,moved);chest.add(k,moved)
    for inv in people:deposit(inv)
    if hasattr(study,'_cart'):
        for k,n in study.cart.items.items():
            if chest.add(k,n)!=n:return {'certified':False,'reason':'cargo_cannot_be_unloaded'}
    remaining=[math.ceil(max(0,p.planted+study.cfg['crops'][p.crop]['days']*600-float(study.t))/100)+1 for p in study.plots]
    periods=[13 if p.crop=='potato' else 19 for p in study.plots]
    # Batch route upper bound: home roundtrip <=8s, <=2s between plots,
    # 6s harvest/replant per plot, <=6s final deposits. At 8 plots: 76s.
    farm_labor_bound=8+2*(len(study.plots)-1)+6*len(study.plots)+6
    other_labor_bound=14+2*study.n
    if farm_labor_bound>100 or other_labor_bound>100:return {'certified':False,'reason':'labor_window_infeasible'}
    phase=int(study.t//100)%6
    if study.t%600:return {'certified':False,'reason':'certificate_requires_dawn_boundary'}
    seen={};minimum_food=study.total('food');minimum_bio=study.full_stock('biomass')
    loads=harvests=0
    for tick in range(limit):
        key=(phase,tuple(remaining),tuple(sorted((k,v) for k,v in chest.items.items() if v)),
             tuple(tuple(sorted((k,v) for k,v in inv.items.items() if v)) for inv in people),tank)
        if key in seen:
            start=seen[key]
            return {'certified':True,'reason':'identical_state_cycle','warmup_seconds':start*100,
                    'cycle_seconds':(tick-start)*100,'min_food':minimum_food,'min_biomass':float(minimum_bio),
                    'food_capacity_per_day':float(food_rate),'biomass_capacity_per_day':float(bio_rate),
                    'farm_worker_max_seconds_per_window':farm_labor_bound,
                    'service_worker_max_seconds_per_window':other_labor_bound,
                    'harvests_checked':harvests,'manual_loads_checked':loads,
                    'surface_inputs':0,'fuel_used':0}
        seen[key]=tick
        # Manual whole-unit refuel; reserve is enough to run during loading.
        n=min(math.floor(6-tank),chest.items['biomass'])
        if n and tank>=F(1,150):chest.take('biomass',n);tank+=n;loads+=1
        if tank<F(1,3):return {'certified':False,'reason':'projected_hopper_exhaustion'}
        tank-=F(1,3)
        phase=(phase+1)%6
        # Meals at dawn precede end-of-window harvests, matching the main scheduler.
        if phase==0:
            for inv in people:
                if not inv.items['food']:
                    for other in people:
                        if other.items['food']>1 and chest.room('food'):
                            other.take('food',1);chest.add('food',1)
                    if chest.items['food'] and inv.room('food'):
                        chest.take('food',1);inv.add('food',1)
                if not inv.items['food']:return {'certified':False,'reason':'projected_meal_shortfall'}
                inv.take('food',1)
        # Check the trough BEFORE harvests replenish stocks. Checking only after
        # the batch would conceal a reserve violation immediately before maturity.
        trough_food=chest.items['food']+sum(p.items['food'] for p in people)
        trough_bio=chest.items['biomass']+sum(p.items['biomass'] for p in people)+tank
        minimum_food=min(minimum_food,trough_food);minimum_bio=min(minimum_bio,trough_bio)
        if trough_food<study.n or trough_bio<2:
            return {'certified':False,'reason':'one_day_reserve_not_maintained'}
        remaining=[max(0,n-1) for n in remaining]
        # Biomass-bearing crops first; avoid indefinite byproduct accumulation.
        order=sorted(range(len(remaining)),key=lambda i:study.plots[i].crop=='potato')
        for i in order:
            if remaining[i]:continue
            crop=study.plots[i].crop;definition=study.cfg['crops'][crop]
            total_food=chest.items['food']+sum(p.items['food'] for p in people)
            total_bio=chest.items['biomass']+sum(p.items['biomass'] for p in people)+tank
            useful=(crop!='marrowvine' and total_food<3*study.n+3) or (crop!='potato' and total_bio<10)
            if not useful:continue
            inv=people[0];deposit(inv)
            if not inv.add_all(definition['yield']):continue
            inv.take(definition['seed'],1)
            remaining[i]=periods[i];harvests+=1;deposit(inv)
        for inv in people:
            n=min(max(0,2-inv.items['food']),chest.items['food'],inv.room('food'))
            chest.take('food',n);inv.add('food',n)
            inv.validate()
        chest.validate()
        total_food=chest.items['food']+sum(p.items['food'] for p in people)
        total_bio=chest.items['biomass']+sum(p.items['biomass'] for p in people)+tank
        minimum_food=min(minimum_food,total_food);minimum_bio=min(minimum_bio,total_bio)
        if total_food<study.n or total_bio<2:
            return {'certified':False,'reason':'one_day_reserve_not_maintained'}
    return {'certified':False,'reason':'no_repeating_state_within_certificate_search_limit'}
