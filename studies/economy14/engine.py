"""Deterministic actor/task economy experiment, deliberately separate from Godot."""
from __future__ import annotations
from collections import Counter
from fractions import Fraction as F
import math
from sustainability import capacity, witness
from model import (Action, Actor, Generator, Inventory, Plot, Power, RESOURCES,
                   Signal, make_sites, number, stamp)

class Study:
    def __init__(self, config, world, players, strategy, setback='baseline'):
        self.cfg,self.world,self.n,self.strategy,self.setback=config,world,players,strategy,setback
        self.id=f'{world}__{strategy}__{players}p__{setback}'
        self.t=F(0);self.chest=Inventory(config['chest_slots'])
        self.chest.add_all(config['starting_storage'])
        self.actors=[Actor(i,Inventory(config['personal_slots'])) for i in range(players)]
        for a in self.actors:a.inventory.add('food',config['starting_food_per_player'])
        self.initial=Counter(config['starting_storage']);self.initial['food']+=players*config['starting_food_per_player']
        self.sites=make_sites(config,world);self.known={0,1}
        self.plots=[Plot(i) for i in range(4)]
        self.signal=Signal(target=F(config['signal_days']*600));self.power=Power(self.signal)
        self.recovered=Counter();self.produced=Counter();self.spent=Counter();self.upgrades=[]
        self.jobs=[];self.daily=[];self.events=[];self.rejected=Counter();self.busy_project=None
        self.bench_owner=None;self.power_owner=None;self.barrow_owner=None;self.have_barrow=False
        self.bed_pending=False;self.cleared_sockets=set(range(4));self.project_reserved={}
        self.certificates=[];self.food_date=None;self.power_date=None;self.self_date=None;self.trial=None;self.candidate_date=None;self.confirmation_failures=[];self.first_confirmed=None
        self.triggered=False;self.setback_time=None;self.last_dawn=0;self.next_day=600
        self.care_count=0;self.plant_count=0;self.site_visits=0;self.expense=Counter()
        self.target_potatoes={2:2,4:3,6:4}[players]
        self.target_plots=self.target_potatoes+3
        # Legal 2x2 footprints around a two-cell cross aisle. First four are prepared.
        self.sockets=[(2,0),(6,0),(2,4),(6,4),(0,0),(8,0),(0,4),(8,4)]
        self.station_seconds={'chest':0,'workbench':3,'fuel':5,'biomass':5}
        self.log('configuration',None,world=world,players=players,strategy=strategy,setback=setback)

    def log(self,event,a=None,**data):
        self.events.append({'second':number(self.t),'actor':None if a is None else a.id,
                            'event':event,'where':None if a is None else a.where,**data})
    def total(self,key):
        return self.chest.items[key]+sum(a.inventory.items[key] for a in self.actors)
    def available_home(self,key,a):
        return self.chest.items[key]+a.inventory.items[key]
    def counts(self):
        return Counter(p.crop for p in self.plots if p.crop)
    def mature(self,p):
        return p.crop is not None and float(self.t)>=p.planted+self.cfg['crops'][p.crop]['days']*600
    def distance(self,start,end):
        if start==end:return 0
        def home(x,out):
            if x.startswith('site:'):
                s=self.sites[int(x[5:])];return s.out if out else s.back
            if x.startswith('plot:'):
                i=int(x[5:]);x,y=self.sockets[i]
                return 3+math.ceil((abs(x+1-5)+abs(y+1-3))*.2)
            return self.station_seconds.get(x,5)
        if start.startswith('plot:') and end.startswith('plot:'):
            a,b=self.sockets[int(start[5:])],self.sockets[int(end[5:])]
            return max(1,math.ceil((abs(a[0]-b[0])+abs(a[1]-b[1]))*.2))
        return home(start,False)+home(end,True)
    def step(self,plan,kind,where,seconds=0,category='handling',**data):
        plan.append(Action(kind,where,seconds,data,category))
    def move(self,a,plan,where):
        start=plan[-1].where if plan else a.where
        if start!=where:self.step(plan,'move',where,self.distance(start,where),'travel')
    def home(self,a,plan,deposit=True):
        self.move(a,plan,'chest')
        if deposit:self.step(plan,'deposit','chest')
        self.step(plan,'meal','chest',2)
    def withdraw(self,plan,key,n):
        if n>0:self.step(plan,'withdraw','chest',2,key=key,n=n)
    def reject(self,a,reason):
        self.rejected[reason]+=1
        if self.rejected[reason]<=3:self.log('blocked_action',a,reason=reason)
        self.release(a)
        a.plan=[]
    def release(self,a):
        for p in self.plots:
            if p.owner==a.id:p.owner=None
        for s in self.sites:
            if s.reserved==a.id:s.reserved=None
        if self.bench_owner==a.id:self.bench_owner=None
        if self.power_owner==a.id:self.power_owner=None
        if self.busy_project==a.id:
            self.busy_project=None;self.project_reserved.pop(a.id,None)
            self.bed_pending=False
        if a.cart and a.where=='chest':self.barrow_owner=None;a.cart=False
        a.trip_site=None

    def cost(self,recipe):
        if recipe.startswith('prop:'):return {recipe[5:]:1,'biomass':1}
        return self.cfg['recipes'][recipe]['cost']
    def affordable(self,recipe,a):
        return all(self.available_home(k,a)>=v for k,v in self.cost(recipe).items())
    def reserved_materials(self):
        return Counter({k:sum(cost.get(k,0) for cost in self.project_reserved.values()) for k in RESOURCES})
    def full_stock(self,key):
        amount=F(self.total(key))
        amount+=sum(g.tank for g in self.power.generators if g.kind==key)
        return amount

    def make_power_plan(self,a):
        """Policy-issued physical load/start/switch visits, never instantaneous remote control."""
        p=self.power;gens=p.generators
        bench_needed=self.bench_owner is not None or self.busy_project is not None
        bio=next((g for g in gens if g.kind=='biomass'),None)
        sustainable_bio=(self.counts()['marrowvine']>=2 and self.counts()['glowcap']>=1)
        prefer_bio=bio is not None and sustainable_bio and bio.tank>=2 and not bench_needed
        for g in gens:
            needs_load=(g.tank < (1 if g.kind=='fuel' else 2))
            # Do not burn the farm's construction budget before renewable operation is plausible.
            use_bio=g.kind=='fuel' or sustainable_bio
            if needs_load and use_bio and self.total(g.kind)>0 and math.floor(g.maximum-g.tank)>0:
                plan=[];self.home(a,plan)
                self.withdraw(plan,g.kind,min(g.maximum-math.floor(g.tank),self.chest.items[g.kind]))
                self.move(a,plan,g.kind)
                self.step(plan,'load',g.kind,2,generator=gens.index(g))
                if not g.running:self.step(plan,'start',g.kind,1,generator=gens.index(g))
                self.move(a,plan,'chest');return plan
        if bio and sustainable_bio and bio.tank>=2 and not bio.running:
            plan=[];self.move(a,plan,'biomass');self.step(plan,'start','biomass',1,generator=gens.index(bio));return plan
        fuel=gens[0]
        if prefer_bio and fuel.running:
            plan=[];self.move(a,plan,'fuel')
            if p.enabled['lighting']:self.step(plan,'switch','fuel',1,key='lighting',value=False)
            self.step(plan,'switch','fuel',1,key='workbench',value=False)
            self.step(plan,'stop','fuel',1,generator=0);return plan
        need_fuel=(not prefer_bio and (not bio or not bio.running or bench_needed))
        if need_fuel and not fuel.running and fuel.tank>0:
            plan=[];self.move(a,plan,'fuel');self.step(plan,'start','fuel',1,generator=0);return plan
        if not p.enabled['beacon'] and p.capacity>=2:
            plan=[];self.move(a,plan,'fuel');self.step(plan,'switch','fuel',1,key='beacon',value=True);return plan
        if p.demand>p.capacity and p.enabled['lighting']:
            plan=[];self.move(a,plan,'fuel');self.step(plan,'switch','fuel',1,key='lighting',value=False);return plan
        if p.demand>p.capacity and p.enabled['workbench']:
            plan=[];self.move(a,plan,'fuel');self.step(plan,'switch','fuel',1,key='workbench',value=False);return plan
        if self.bench_owner is not None and not p.enabled['workbench'] and p.capacity>=4:
            plan=[]
            if p.enabled['lighting'] and p.demand+2>p.capacity:
                self.move(a,plan,'fuel');self.step(plan,'switch','fuel',1,key='lighting',value=False)
            self.move(a,plan,'workbench');self.step(plan,'switch','workbench',1,key='workbench',value=True);return plan
        if self.bench_owner is None and p.enabled['workbench']:
            plan=[];self.move(a,plan,'workbench');self.step(plan,'switch','workbench',1,key='workbench',value=False);return plan
        night=int(self.t)%600>=420
        lights=night and fuel.running and p.capacity>=p.demand+(0 if p.enabled['lighting'] else 1) and not bench_needed
        if lights!=p.enabled['lighting'] and (not lights or p.capacity>=p.demand+1):
            plan=[];self.move(a,plan,'fuel');self.step(plan,'switch','fuel',1,key='lighting',value=lights);return plan
        return None

    def choose_crop(self,p):
        if p.id<4 and not p.ever_planted:return 'potato' if p.id<2 else 'glowcap'
        counts=self.counts();old=p.crop
        if old:counts[old]-=1
        seed=lambda c:self.total(self.cfg['crops'][c]['seed'])+(1 if c==old else 0)>0
        if old=='potato' and counts['potato']<self.target_potatoes:return old
        if old=='glowcap' and counts['glowcap']<1:return old
        if old=='marrowvine':return old
        if counts['marrowvine']<1 and seed('marrowvine'):return 'marrowvine'
        if counts['potato']<self.target_potatoes and seed('potato'):return 'potato'
        if counts['marrowvine']<2 and seed('marrowvine'):return 'marrowvine'
        if counts['glowcap']<1 and seed('glowcap'):return 'glowcap'
        if old:return old
        if seed('glowcap'):return 'glowcap'
        return None

    def farm_plan(self,a):
        for p in sorted(self.plots,key=lambda p:(not self.mature(p),p.id)):
            if p.owner is not None or float(self.t)<p.delayed_until:continue
            if p.crop and not self.mature(p):continue
            if self.mature(p) and self.setback=='missed_harvest' and not self.triggered and self.t>=1800:
                self.triggered=True;self.setback_time=number(self.t);p.delayed_until=float(self.t)+300
                self.log('setback_harvest_delayed',a,plot=p.id,until=p.delayed_until);continue
            crop=self.choose_crop(p)
            if not crop:continue
            seed=self.cfg['crops'][crop]['seed']
            if p.crop!=crop and self.total(seed)<1:continue
            plan=[];self.home(a,plan)
            if p.crop!=crop:self.withdraw(plan,seed,1)
            self.move(a,plan,f'plot:{p.id}')
            if p.crop:self.step(plan,'harvest',f'plot:{p.id}',3,'farming',plot=p.id)
            goal=self.target_potatoes if p.crop=='potato' else 2 if p.crop=='marrowvine' else 0
            bank_seed=bool(p.crop and self.counts()[p.crop]<goal and self.total(self.cfg['crops'][p.crop]['seed'])==0 and self.total('biomass')>=1)
            if not bank_seed:self.step(plan,'plant',f'plot:{p.id}',3,'farming',plot=p.id,crop=crop)
            self.step(plan,'release_plot',f'plot:{p.id}',plot=p.id)
            self.home(a,plan);p.owner=a.id
            return plan
        return None

    def next_project(self,a):
        done=Counter(u['project'] for u in self.upgrades)
        goals=[]
        if self.strategy=='exploration':goals=['tool','barrow','backpack']
        elif self.strategy=='balanced':goals=['tool']
        for project in goals:
            if not done[project]:return project
        primary='bed' if len(self.plots)<self.target_plots else 'biogenerator' if not done['biogenerator'] else None
        if primary:
            # Approved infrastructure policy buys a tool WHEN NEEDED. Do not strand
            # an affordable tool behind the generator it must unlock materials for.
            locked_known=any(site.visited and site.id>=4 and (site.stock['metal'] or site.stock['basic']) for site in self.sites)
            if self.strategy=='infrastructure' and not done['tool'] and locked_known and not self.affordable(primary,a) and self.affordable('tool',a):
                return 'tool'
            return primary
        if self.strategy=='infrastructure':
            if not done['tool']:return 'tool'
        # Infrastructure does not buy transport for its own sake; balanced must retain
        # the next production investment before optional transport (goals already met).
        if self.strategy=='balanced':
            for project in ['barrow','backpack']:
                if not done[project]:return project
        return None

    def propagation_needed(self):
        if any(not p.ever_planted for p in self.plots[:4]):return None
        counts=self.counts()
        for crop,goal in [('marrowvine',2),('potato',self.target_potatoes)]:
            key=self.cfg['crops'][crop]['seed']
            # Need an extra unplanted seed, not the same seed simultaneously promised twice.
            if counts[crop]+self.total(key)<goal and self.total(key)>0:
                return 'prop:'+key
        return None

    def project_plan(self,a):
        if self.busy_project is not None or self.bench_owner is not None:return None
        recipe=self.propagation_needed()
        if recipe is None or not self.affordable(recipe,a):recipe=self.next_project(a)
        if recipe is None or not self.affordable(recipe,a) or self.trial is not None:return None
        if recipe=='biogenerator' and self.counts()['marrowvine']<1:return None
        self.busy_project=self.bench_owner=a.id;self.project_reserved[a.id]=dict(self.cost(recipe))
        plan=[];self.home(a,plan)
        for key,n in self.cost(recipe).items():self.withdraw(plan,key,n)
        if recipe=='bed':
            self.bed_pending=True
            self.move(a,plan,f'plot:{len(self.plots)}')
            if len(self.plots) not in self.cleared_sockets:self.step(plan,'clear',f'plot:{len(self.plots)}',30,'constructing')
        self.move(a,plan,'fuel')
        if self.power.enabled['lighting']:self.step(plan,'switch','fuel',1,key='lighting',value=False)
        self.step(plan,'start','fuel',1,generator=0)
        self.move(a,plan,'workbench')
        self.step(plan,'switch','workbench',1,key='workbench',value=True)
        duration=5 if recipe.startswith('prop:') else self.cfg['recipes'][recipe]['assembly']
        self.step(plan,'assemble','workbench',duration,'constructing',recipe=recipe)
        self.step(plan,'switch','workbench',1,key='workbench',value=False)
        if recipe=='bed':
            self.move(a,plan,f'plot:{len(self.plots)}');self.step(plan,'install',f'plot:{len(self.plots)}',15,'constructing',recipe=recipe)
        elif recipe=='biogenerator':
            self.move(a,plan,'biomass');self.step(plan,'install','biomass',30,'constructing',recipe=recipe)
        elif recipe in ('barrow','backpack'):
            self.move(a,plan,'chest');self.step(plan,'install','chest',5 if recipe=='barrow' else 2,'constructing',recipe=recipe)
        self.home(a,plan);self.step(plan,'release_project','chest')
        return plan

    def needs(self):
        # Targets are policy reserves, not free resources or facts about hidden loot.
        desired={'food':self.n*3,'fuel':max(2,math.ceil(float(self.signal.target-self.signal.progress)/600*.6)),
                 'biomass':6+max(0,self.target_plots-len(self.plots))*5,
                 'metal':0,'basic':0,'advanced':0,'marrowvine_seed':max(0,2-self.counts()['marrowvine']),
                 'potato_seed':max(0,self.target_potatoes-self.counts()['potato'])}
        done=Counter(u['project'] for u in self.upgrades)
        for name in (['tool','barrow','backpack'] if self.strategy=='exploration' else ['tool']):
            if not done[name]:
                for key,n in self.cost(name).items():desired[key]+=n
        for name,count in [('bed',max(0,self.target_plots-len(self.plots))),('biogenerator',int(not done['biogenerator']))]:
            for key,n in self.cost(name).items():
                if key in ('metal','basic'):desired[key]+=n*count
        next_goal=self.next_project(self.actors[0])
        if self.strategy=='balanced' and next_goal in ('barrow','backpack'):
            for key,n in self.cost(next_goal).items():desired[key]+=n
        if self.trial is not None:
            for key in ('food','fuel','biomass','marrowvine_seed','potato_seed'):desired[key]=0
        return {k:max(0,v-self.total(k)) for k,v in desired.items()}

    def expedition_plan(self,a):
        if a.id==0:return None
        need=self.needs();candidates=[]
        for i in sorted(self.known):
            s=self.sites[i]
            if s.reserved is not None:continue
            can_tool=a.inventory.items['tool']>0 or self.chest.items['tool']>0
            possible={k:s.available(k) for k in RESOURCES if not (k in ('metal','basic') and i>=4 and not can_tool) and not(k=='advanced' and not can_tool)}
            if not s.visited:
                # Unvisited inventories are not strategy information. A named vegetated
                # landmark is a clue, not knowledge of its seed yield or contents.
                score=1000 if self.strategy=='exploration' else 100
                vegetation=any(word in s.name.lower() for word in ('verge','nursery','greenhouse','farm','seep','thicket'))
                if vegetation and self.total('marrowvine_seed')+self.counts()['marrowvine']==0:score+=20
                work=120  # fixed provisional exploration allowance; no hidden-quantity lookup
            else:
                useful=sum(min(need.get(k,0),v) for k,v in possible.items())
                score=useful*5
                if self.total('food')<self.n:score+=100*min(possible.get('food',0),self.n)
                if self.total('marrowvine_seed')+self.counts()['marrowvine']==0 and possible.get('marrowvine_seed'):score+=300
                work=20+sum(v*(2 if k=='biomass' or i>=4 else 1) for k,v in possible.items())
            if score<=0:continue
            speed=a.speed
            # Conservative daylight admission includes source labor and handling.
            duration=(s.out+s.back)/speed+work+30
            if int(self.t)%600+duration>360:continue
            candidates.append((score/(s.out+s.back+4),-i,s))
        if not candidates:return None
        s=max(candidates,key=lambda x:(x[0],x[1]))[2]
        plan=[];self.home(a,plan)
        if not a.equipped and self.chest.items['backpack']:
            self.withdraw(plan,'backpack',1);self.step(plan,'equip','chest',2)
        if self.chest.items['tool'] and a.inventory.items['tool']==0:self.withdraw(plan,'tool',1)
        if self.have_barrow and self.barrow_owner is None and s.road:
            self.barrow_owner=a.id;a.cart=True
            self.step(plan,'take_cart','chest',2)
        self.move(a,plan,f'site:{s.id}');self.step(plan,'discover',f'site:{s.id}')
        order=['marrowvine_seed','food','fuel','biomass','metal','basic','advanced']
        if self.trial is not None:order=['metal','basic','advanced']
        # After first discoveries, fetch the most urgent missing construction type first.
        if s.visited:
            order=sorted(order,key=lambda k:(not(k=='food' and self.total('food')<self.n),-min(s.available(k),need.get(k,0)),order.index(k)))
        for key in order:self.step(plan,'collect',f'site:{s.id}',key=key,site=s.id)
        self.move(a,plan,'chest');self.step(plan,'deposit','chest');self.step(plan,'release_site','chest',site=s.id)
        s.reserved=a.id;a.trip_site=s.id
        return plan

    def assign(self,a):
        if a.where.startswith('site:'):
            plan=[];self.home(a,plan);self.step(plan,'release_site','chest',site=int(a.where[5:]));return plan
        # Any free home actor may recover a paused operator's electricity.
        power=self.make_power_plan(a) if self.power_owner is None else None
        if power:
            self.power_owner=a.id
            self.step(power,"release_power",power[-1].where)
            return power
        if a.inventory.items['food']<1 and self.chest.items['food']:
            plan=[];self.home(a,plan);return plan
        if a.id==0:
            prop=self.propagation_needed()
            if prop and self.affordable(prop,a) and self.trial is None:
                project=self.project_plan(a)
                if project:return project
            farm=self.farm_plan(a)
            if farm:return farm
            project=self.project_plan(a)
            if project:return project
            if a.where!='chest':
                plan=[];self.home(a,plan);return plan
            return None
        return self.expedition_plan(a)

    def start_action(self,a):
        while a.action is None:
            if not a.plan:
                a.plan=self.assign(a) or []
                if not a.plan:return
            action=a.plan.pop(0)
            if action.kind!='move' and a.where!=action.where:
                raise AssertionError(('remote action',a.id,a.where,action))
            if action.kind=='collect':
                s=self.sites[action.data['site']];key=action.data['key']
                if key=='advanced' or key in ('metal','basic') and s.id>=4:
                    if not a.inventory.items['tool']:continue
                target=self.cart if a.cart else a.inventory
                qty=min(s.available(key),target.room(key))
                if not qty:continue
                action.data['n']=qty
                rate=2 if key=='biomass' else 5 if key=='advanced' else (2 if s.id>=4 else 1) if key in ('metal','basic') else 0
                action.seconds=2+qty*rate;action.category='gathering'
            if action.kind=='deposit':
                types=set(k for inv in self.inventories(a) for k,n in inv.items.items() if n and (k!='food' or inv is not a.inventory or n>2))
                action.seconds=2*len(types)
            if action.kind=='assemble' and not a.inventory.has(self.cost(action.data['recipe'])):
                self.reject(a,'ingredients_changed');continue
            assert float(self.t)+1e-7>=a.last_end, 'overlapping actor jobs'
            a.action=action;a.remaining=action.seconds
            action.data['_start']=number(self.t)
            if a.remaining>0:return
            self.finish(a)

    def inventories(self,a):
        return [a.inventory]+([self.cart] if a.cart else [])
    @property
    def cart(self):
        if not hasattr(self,'_cart'):self._cart=Inventory(8)
        return self._cart
    def deposit(self,a):
        moved=Counter()
        for inv in self.inventories(a):
            for key,n in list(inv.items.items()):
                keep=2 if key=='food' and inv is a.inventory else 0
                amount=min(max(0,n-keep),self.chest.room(key))
                self.chest.add(key,amount);inv.take(key,amount);moved[key]+=amount
        return dict(moved)
    def finish(self,a):
        action=a.action;assert action is not None
        k,d=action.kind,action.data
        valid=True
        if k=='move':a.where=action.where
        elif k=='deposit':d['moved']=self.deposit(a)
        elif k=='meal':
            n=min(max(0,2-a.inventory.items['food']),self.chest.items['food'],a.inventory.room('food'))
            self.chest.take('food',n);a.inventory.add('food',n);d['withdrawn']=n
        elif k=='withdraw':
            n=min(d['n'],self.chest.items[d['key']],a.inventory.room(d['key']))
            valid=n==d['n'];d['transferred']=n if valid else 0
            if valid:
                self.chest.take(d['key'],n);a.inventory.add(d['key'],n)
        elif k=='discover':
            s=self.sites[int(a.where[5:])];s.visited=True;self.site_visits+=1
            if s.id+2<6:self.known.add(s.id+2)
            if self.setback=='failed_expedition' and not self.triggered and s.id>=4:
                self.triggered=True;self.setback_time=number(self.t)
                self.log('setback_empty_return',a,site=s.id)
                a.plan=[];self.move(a,a.plan,'chest');self.step(a.plan,'deposit','chest');self.step(a.plan,'release_site','chest',site=s.id)
        elif k=='collect':
            s=self.sites[d['site']];key=d['key'];target=self.cart if a.cart else a.inventory
            n=min(d['n'],s.available(key),target.room(key))
            if key=='biomass' and s.patch:s.patch.gather(n)
            else:s.stock[key]-=n
            assert target.add(key,n)==n
            self.recovered[key]+=n;d['collected']=n
        elif k=='harvest':
            p=self.plots[d['plot']]
            assert self.mature(p),'premature harvest'
            yields=self.cfg['crops'][p.crop]['yield']
            if a.inventory.add_all(yields):
                self.produced.update(yields);self.log('harvest',a,plot=p.id,crop=p.crop,yields=yields)
                p.crop=None;self.care_count+=1
            else:valid=False
        elif k=='plant':
            p=self.plots[d['plot']];seed=self.cfg['crops'][d['crop']]['seed']
            if p.crop or not a.inventory.items[seed]:valid=False
            else:
                a.inventory.take(seed,1);self.spent[seed]+=1;p.crop=d['crop'];p.planted=float(self.t);p.ever_planted=True
                self.plant_count+=1;self.log('plant',a,plot=p.id,crop=p.crop)
        elif k=='release_plot':self.plots[d['plot']].owner=None
        elif k=='load':
            g=self.power.generators[d['generator']];d['loaded']=g.load(a.inventory,100)
        elif k=='start':
            g=self.power.generators[d['generator']]
            if g.tank>0:g.running=True
        elif k=='stop':self.power.generators[d['generator']].running=False
        elif k=='switch':
            valid=self.power.switch(d['key'],d['value']);d['capacity']=self.power.capacity;d['demand']=self.power.demand
        elif k=='assemble':
            recipe=d['recipe'];cost=self.cost(recipe);d['cost']=cost
            output={recipe[5:]:2} if recipe.startswith('prop:') else {recipe+'_kit' if recipe in ('bed','biogenerator') else recipe:1}
            after=a.inventory.items.copy();after.subtract(cost)
            if any(n<0 for n in after.values()) or a.inventory.occupied(after+Counter(output))>a.inventory.slots:valid=False
            else:
                for key,n in cost.items():a.inventory.take(key,n);self.spent[key]+=n
                assert a.inventory.add_all(output);d['output']=output
                if recipe.startswith('prop:'):self.produced.update(output)
                else:self.log('assembled',a,project=recipe,cost=cost)
                if recipe=='tool':self.upgrade(recipe,a)
                self.bench_owner=None
        elif k=='install':
            recipe=d['recipe'];item=recipe+'_kit' if recipe in ('bed','biogenerator') else recipe
            if recipe=='barrow':a.inventory.take(item,1);self.have_barrow=True
            elif recipe=='backpack':
                # Store for the next expedition actor; it remains a transferable asset.
                n=self.chest.add('backpack',1)
                if n:a.inventory.take('backpack',1)
                else:valid=False
            elif not a.inventory.items[item]:valid=False
            elif recipe=='bed':
                assert len(self.plots)<self.cfg['maximum_plots'] and len(self.plots) in self.cleared_sockets
                a.inventory.take(item,1);self.plots.append(Plot(len(self.plots)))
            elif recipe=='biogenerator':
                a.inventory.take(item,1);self.power.generators.append(Generator('biomass',2,6))
            if valid:self.upgrade(recipe,a)
        elif k=='equip':
            if not a.equipped and a.inventory.items['backpack']:
                a.inventory.take('backpack',1);a.inventory.slots=8;a.equipped=True
        elif k=='release_power':self.power_owner=None
        elif k=='release_project':
            self.busy_project=None;self.bench_owner=None;self.project_reserved.pop(a.id,None);self.bed_pending=False
        elif k=='release_site':
            self.sites[d['site']].reserved=None;a.trip_site=None
            if a.cart:self.barrow_owner=None;a.cart=False
        elif k=='clear':self.cleared_sockets.add(int(a.where[5:]))
        elif k=='take_cart':pass
        else:raise AssertionError(k)
        a.last_end=float(self.t)
        self.log('action',a,kind=k,start=d['_start'],end=number(self.t),details={x:y for x,y in d.items() if not x.startswith('_')},applied=valid)
        a.action=None
        if not valid:self.reject(a,k+'_precondition')
    def upgrade(self,recipe,a):
        self.upgrades.append({'project':recipe,'second':number(self.t),'date':stamp(self.t)})
        self.log('installed',a,project=recipe)

    def settle_day(self):
        for a in self.actors:
            if a.inventory.items['food']:
                a.inventory.take('food',1);a.consumed+=1;a.missed=max(0,a.missed-1)
            else:a.missed+=1;a.missed_total+=1
        food_rate,bio_rate=capacity(self)
        if self.food_date is None and food_rate>=self.n and self.total('food')>=self.n:self.food_date=number(self.t)
        if self.power_date is None and bio_rate>=2 and len(self.power.generators)>1 and self.full_stock('biomass')>=2:self.power_date=number(self.t)
        self.check_trial()
        next_project=self.next_project(self.actors[0])
        if self.self_date is None and self.trial is None and (next_project is None or not self.affordable(next_project,self.actors[0])):
            proof=witness(self)
            if proof['certified']:
                if self.candidate_date is None:self.candidate_date=number(self.t)
                self.certificates.append(proof)
                self.trial={'start':number(self.t),'food':self.total('food'),'biomass':self.full_stock('biomass'),
                            'missed':sum(a.missed_total for a in self.actors),'outages':self.signal.total_off}
                self.log('underground_cycle_certified',None,certificate=proof)
            else:self.expense['certificate_'+proof['reason']]+=1
        self.daily.append(self.snapshot(day=int(self.t//600)))
        self.audit()

    def check_trial(self):
        if self.trial is not None:
            trial=self.trial
            failed=(sum(a.missed_total for a in self.actors)>trial['missed'] or self.signal.total_off>trial['outages'] or
                    self.total('food')<self.n or self.full_stock('biomass')<2)
            if failed:
                self.confirmation_failures.append({'second':number(self.t),'reason':'reserve_or_service_shortfall'})
                self.log('underground_confirmation_failed',None);self.trial=None;self.self_date=None
            elif self.self_date is None and float(self.t)-trial['start']>=3900 and self.total('food')>=trial['food'] and self.full_stock('biomass')>=trial['biomass']:
                self.self_date=number(self.t)
                if self.first_confirmed is None:self.first_confirmed=self.self_date
                self.log('underground_operation_confirmed',None,start=trial['start'],food=self.total('food'),biomass=number(self.full_stock('biomass')))

    def snapshot(self,day=None):
        burned={k:sum(g.burned for g in self.power.generators if g.kind==k) for k in ('fuel','biomass')}
        return {'day':day,'second':number(self.t),'date':stamp(self.t),
                'signal_days':number(self.signal.progress/600),'lost_days':number(self.signal.lost/600),
                'consumed_food':sum(a.consumed for a in self.actors),
                'personal_food':{str(a.id):a.inventory.items['food'] for a in self.actors},
                'missed_streaks':{str(a.id):a.missed for a in self.actors},
                'outage_seconds':number(self.signal.total_off),'missed_meals':sum(a.missed_total for a in self.actors),
                'food_reserve':self.total('food'),'fuel_reserve':number(self.full_stock('fuel')),
                'biomass_reserve':number(self.full_stock('biomass')),
                'recovered':dict(self.recovered),'crop_and_propagation_output':dict(self.produced),'spent':dict(self.spent),
                'burned':{k:number(v) for k,v in burned.items()},
                'remaining':{k:number(sum(F(s.available(k)) if k!='biomass' or not s.patch else s.patch.stock for s in self.sites)) for k in RESOURCES},
                'wild_regrown':number(sum(s.patch.regrown for s in self.sites if s.patch)),
                'projects':[dict(u) for u in self.upgrades],
                'activity':{k:round(sum(a.activity[k] for a in self.actors),3) for k in ('travel','gathering','farming','constructing','handling','paused')},
                'cargo':dict(self.cart.items) if hasattr(self,'_cart') else {},
                'held':{k:self.total(k)+(self.cart.items[k] if hasattr(self,'_cart') else 0) for k in RESOURCES},
                'reserved':dict(self.reserved_materials()) if self.bench_owner is not None else {},
                'food_capacity_reserve_second':self.food_date,'power_capacity_reserve_second':self.power_date,
                'feasible_underground_second':self.candidate_date,'underground_self_sufficient_second':self.self_date,
                'crop_counts':dict(self.counts()),
                'crop_food_reserve_lower_bound':max(0,self.produced['food']-sum(a.consumed for a in self.actors)),
                'crop_biomass_reserve_lower_bound':max(0,number(F(self.produced['biomass']-self.spent['biomass'])-burned['biomass']))}

    def audit(self):
        self.chest.validate()
        for a in self.actors:a.inventory.validate()
        if hasattr(self,'_cart'):self.cart.validate()
        for key in RESOURCES:
            initial=F(self.initial[key]);generated=F(self.produced[key]);recovered=F(self.recovered[key])
            stored=F(self.total(key))+(self.cart.items[key] if hasattr(self,'_cart') else 0)
            tank=sum(g.tank for g in self.power.generators if g.kind==key)
            burned=sum(g.burned for g in self.power.generators if g.kind==key)
            consumed=sum(a.consumed for a in self.actors) if key=='food' else 0
            assert initial+generated+recovered==stored+tank+burned+self.spent[key]+consumed,('conservation',key,self.id,self.t,initial+generated+recovered,stored+tank+burned+self.spent[key]+consumed)
        for s in self.sites:
            assert all(n>=0 for n in s.stock.values())
            if s.patch:assert s.patch.stock+s.patch.harvested==s.patch.capacity+s.patch.regrown
        for key in RESOURCES:
            if key=='biomass':continue
            assert sum(s.initial[key]-s.stock[key] for s in self.sites)==self.recovered[key]
        assert len(self.plots)<=8
        assert len([a for a in self.actors if a.cart])<=1

    def run(self):
        horizon=self.cfg['observation_days']*600
        while self.t<horizon and self.signal.progress<self.signal.target:
            for a in self.actors:
                if a.action is None:self.start_action(a)
            elapsed,bench=self.power.advance(1)
            if elapsed==0:break
            self.t+=elapsed;dt=float(elapsed)
            for s in self.sites:
                if s.patch:s.patch.advance(elapsed)
            for a in self.actors:
                if a.action:
                    amount=dt*a.speed if a.action.kind=='move' else float(bench) if a.action.kind=='assemble' else dt
                    used=min(amount,a.remaining);a.remaining-=used
                    powered_time=float(bench) if a.action.kind=='assemble' else dt
                    a.activity[a.action.category]+=powered_time
                    if a.action.kind=='assemble':a.activity['paused']+=dt-powered_time
            if self.t>=self.next_day:
                self.settle_day();self.next_day+=600
            self.check_trial()
            if self.signal.progress>=self.signal.target:break
            for a in self.actors:
                if a.action and a.remaining<=1e-7:self.finish(a)
            self.check_trial()
        self.audit()
        final=self.snapshot();final.update(scenario=self.id,world=self.world,players=self.n,strategy=self.strategy,setback=self.setback,
             rescued=self.signal.progress>=self.signal.target,outcome='rescued' if self.signal.progress>=self.signal.target else 'observation_cutoff',
             setback_triggered=self.triggered,setback_second=self.setback_time,rejections=dict(self.rejected),
             discoveries=sum(s.visited for s in self.sites),seed_opportunities=sum(s.initial['marrowvine_seed'] for s in self.sites),
             initial_world=dict(self.cfg['worlds'][self.world]['totals']),
             food_capacity_reserve_second=self.food_date,power_capacity_reserve_second=self.power_date,
             underground_self_sufficient_second=self.self_date,first_confirmed_second=self.first_confirmed,feasible_underground_second=self.candidate_date,
             confirmation_failures=self.confirmation_failures,trial=self.trial and {k:number(v) if isinstance(v,F) else v for k,v in self.trial.items()},certificates=self.certificates,
             certificate_attempts=dict(self.expense),completion_events=int(self.signal.progress>=self.signal.target))
        return final
