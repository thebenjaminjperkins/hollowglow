"""Study-only models. No import, write or patch of playable game modules."""
from __future__ import annotations
from collections import Counter
from dataclasses import dataclass, field
from fractions import Fraction as F
import hashlib
import json
import math
from pathlib import Path

HERE = Path(__file__).resolve().parent
ROOT = HERE.parents[1]
RESOURCES = ('metal', 'basic', 'advanced', 'fuel', 'food', 'biomass',
             'potato_seed', 'glowcap_seed', 'marrowvine_seed')
GEAR = ('tool', 'backpack', 'barrow', 'bed_kit', 'biogenerator_kit')

def stable(seed, label):
    return int.from_bytes(hashlib.sha256(f'{seed}:{label}'.encode()).digest()[:8], 'big')

def load_config(path=None):
    return json.loads(Path(path or HERE/'config.json').read_text())

def number(x):
    return round(float(x), 6)

def stamp(t):
    t = float(t)
    return f'D{int(t//600)+1} {int(t%600)//60:02}:{t%60:05.2f}'

class Inventory:
    def __init__(self, slots, stack=10):
        self.slots, self.stack = slots, stack
        self.items = Counter()
    def limit(self, key):
        return 1 if key in GEAR else self.stack
    def occupied(self, items=None):
        return sum(math.ceil(n/self.limit(k)) for k,n in (self.items if items is None else items).items() if n)
    def room(self, key):
        n = self.items[key]
        partial = (-n) % self.limit(key)
        return partial + (self.slots-self.occupied())*self.limit(key)
    def fits(self, incoming):
        return self.occupied(self.items + Counter(incoming)) <= self.slots
    def add(self, key, n):
        assert isinstance(n, int) and n >= 0
        amount = min(n, self.room(key))
        self.items[key] += amount
        return amount
    def add_all(self, incoming):
        if not self.fits(incoming): return False
        self.items.update(incoming)
        return True
    def has(self, costs):
        return all(self.items[k] >= v for k,v in costs.items())
    def take(self, key, n):
        assert n >= 0 and self.items[key] >= n, (key, n, self.items)
        self.items[key] -= n
    def validate(self):
        assert all(isinstance(v,int) and v >= 0 for v in self.items.values())
        assert self.occupied() <= self.slots

@dataclass
class Signal:
    target: F = F(14*600)
    grace: F = F(150)
    rate: F = F(1,4)
    cap: F = F(600)
    recovery: F = F(150)
    progress: F = F(0)
    lost: F = F(0)
    started: bool = False
    episode_off: F = F(0)
    episode_loss: F = F(0)
    healthy: F = F(0)
    current_off: F = F(0)
    total_off: F = F(0)
    interruptions: int = 0
    transmitting: bool = False
    def advance(self, dt, powered):
        dt = F(dt)
        if powered:
            if not self.transmitting: self.current_off = F(0)
            self.started = True
            self.transmitting = True
            used = min(dt, self.target-self.progress)
            self.progress += used
            self.healthy += used
            if self.healthy >= self.recovery:
                self.episode_off = self.episode_loss = F(0)
            return used
        if self.transmitting: self.interruptions += 1
        self.transmitting = False
        if self.started:
            self.total_off += dt
            self.current_off += dt
            self.healthy = F(0)
            before = max(F(0), self.episode_off-self.grace)
            self.episode_off += dt
            after = max(F(0), self.episode_off-self.grace)
            loss = min((after-before)*self.rate, self.cap-self.episode_loss, self.progress)
            self.progress -= loss
            self.episode_loss += loss
            self.lost += loss
        return dt

@dataclass
class Generator:
    kind: str
    capacity: int
    maximum: int
    tank: F = F(0)
    burned: F = F(0)
    running: bool = False
    def load(self, inventory, quantity):
        room = math.floor(self.maximum-self.tank)
        n = min(quantity, room, inventory.items[self.kind])
        inventory.take(self.kind,n)
        self.tank += n
        return n

class Power:
    def __init__(self, signal):
        self.signal = signal
        self.generators = [Generator('fuel',4,5)]
        self.enabled = {'beacon':False, 'workbench':False, 'lighting':False}
        self.demands = {'beacon':2, 'workbench':2, 'lighting':1}
        self.brownout_seconds = F(0)
        self.rejections = 0
    @property
    def capacity(self):
        return sum(g.capacity for g in self.generators if g.running and g.tank > 0)
    @property
    def demand(self):
        return sum(self.demands[k] for k,v in self.enabled.items() if v)
    @property
    def supplied(self):
        return self.capacity > 0 and self.capacity >= self.demand
    def switch(self,key,value):
        wanted = self.demand + (self.demands[key] if value else -self.demands[key]) if self.enabled[key] != value else self.demand
        if value and (wanted > self.capacity or self.capacity == 0):
            self.rejections += 1
            return False
        self.enabled[key] = value
        return True
    def rates(self):
        cap = self.capacity
        return [(F(1,5) if g.kind=='fuel' else F(0)) +
                ((F(1,5) if g.kind=='fuel' else F(1))*F(self.demand*g.capacity,cap) if self.supplied else F(0))
                if g.running else F(0) for g in self.generators]
    def advance(self,seconds):
        """Split exactly at exhaustion/victory; return elapsed and powered workbench seconds."""
        left = F(seconds); elapsed = bench = F(0)
        while left > 0 and self.signal.progress < self.signal.target:
            rates = [x/600 for x in self.rates()]
            dt = left
            for g,r in zip(self.generators,rates):
                if r: dt = min(dt,g.tank/r)
            tx = self.supplied and self.enabled['beacon']
            if tx: dt = min(dt,self.signal.target-self.signal.progress)
            if dt == 0:
                for g in self.generators:
                    if g.tank == 0: g.running=False
                continue
            powered = self.supplied
            if self.demand > self.capacity: self.brownout_seconds += dt
            if powered and self.enabled['workbench']: bench += dt
            for g,r in zip(self.generators,rates):
                amount = r*dt
                g.tank -= amount; g.burned += amount
                assert g.tank >= 0
            self.signal.advance(dt,tx)
            for g in self.generators:
                if g.tank == 0: g.running=False
            elapsed += dt; left -= dt
        return elapsed,bench

@dataclass
class Patch:
    capacity: int
    period: int
    stock: F = field(init=False)
    regrown: F = F(0)
    harvested: int = 0
    def __post_init__(self): self.stock=F(self.capacity)
    def advance(self,dt):
        extra = min(F(self.capacity)-self.stock,F(dt)*self.capacity/self.period)
        self.stock += extra; self.regrown += extra
    def gather(self,n):
        amount=min(n,math.floor(self.stock))
        self.stock-=amount; self.harvested+=amount
        return amount

@dataclass
class Site:
    id: int
    name: str
    road: bool
    stock: Counter
    initial: Counter
    out: int
    back: int
    patch: Patch | None = None
    visited: bool = False
    reserved: int | None = None
    def available(self,key):
        return math.floor(self.patch.stock) if key=='biomass' and self.patch else self.stock[key]

@dataclass
class Plot:
    id: int
    crop: str | None = None
    planted: float = 0
    owner: int | None = None
    delayed_until: float = 0
    ever_planted: bool = False

@dataclass
class Action:
    kind: str
    where: str
    seconds: float
    data: dict = field(default_factory=dict)
    category: str = 'handling'

@dataclass
class Actor:
    id: int
    inventory: Inventory
    where: str = 'chest'
    plan: list = field(default_factory=list)
    action: Action | None = None
    remaining: float = 0
    missed: int = 0
    consumed: int = 0
    missed_total: int = 0
    equipped: bool = False
    cart: bool = False
    trip_site: int | None = None
    activity: Counter = field(default_factory=Counter)
    last_end: float = 0
    @property
    def speed(self): return .8 if self.missed>=3 else .9 if self.missed>=2 else 1.

def make_sites(config,world):
    spec=config['worlds'][world]
    measurement=json.loads((ROOT/'docs/measurements/region/travel-1701.json').read_text())
    ids=['radio-shell','food-near-wild_plant-1','tool-case','food-far-container-0','machine-housing']
    trips={t['Source']:t for t in measurement['Trips']}
    seed=config['seed']
    lo,hi=spec['seed_range']; count=lo+stable(seed,world+':seed-count')%(hi-lo+1)
    eligible=sorted(spec['seed_locations'],key=lambda n:stable(seed,f'{world}:seed:{n}'))[:count]
    sites=[]
    for i,s in enumerate(spec['sites']):
        stock=Counter(dict(zip(config['site_columns'],s['stock'])))
        stock['marrowvine_seed']=int(i in eligible)
        if i<5:
            t=trips[ids[i]];out=math.ceil(t['OutboundFromChestSeconds']);back=math.ceil(t['RoundTripWithInteractionsSeconds']-t['OutboundFromChestSeconds']-4)
        else: out=back=config['assumptions']['surface_travel_6_seconds_each_way']
        patch=None
        if s.get('wild'):
            days=config['assumptions']['wild_regrowth_days'][stable(seed,f'{world}:patch:{i}')%3]
            patch=Patch(s['wild'],round(days*600))
        sites.append(Site(i,s['name'],s['road'],stock.copy(),stock.copy(),out,back,patch))
    return sites
