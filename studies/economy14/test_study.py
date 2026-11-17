import copy
from fractions import Fraction as F
import sys
import unittest
from pathlib import Path
sys.path.insert(0,str(Path(__file__).resolve().parent))
from model import Inventory, Signal, Power, Generator, Patch, load_config, make_sites, ROOT
from engine import Study
from sustainability import witness
import json

class ModelTests(unittest.TestCase):
    def test_slots_partial_and_whole_harvest(self):
        bag=Inventory(2);self.assertEqual(bag.add('food',12),12)
        self.assertEqual(bag.add('fuel',1),0)
        before=bag.items.copy();self.assertFalse(bag.add_all({'food':1,'potato_seed':1}))
        self.assertEqual(before,bag.items)
        bag.take('food',2);self.assertEqual(bag.add('fuel',11),10);bag.validate()
    def test_fractional_regrowth_is_bounded_and_conserved(self):
        patch=Patch(4,1800);self.assertEqual(patch.gather(9),4)
        patch.advance(449);self.assertEqual(patch.gather(10),0)
        patch.advance(1);self.assertEqual(patch.gather(10),1)
        patch.advance(100000);self.assertEqual(patch.stock,4)
        self.assertEqual(patch.stock+patch.harvested,patch.capacity+patch.regrown)
    def test_manual_pool_proportions_and_fractional_conservation(self):
        p=Power(Signal());p.generators=[Generator('fuel',4,5,F(5),running=True),Generator('biomass',2,6,F(6),running=True)]
        p.switch('beacon',True);p.advance(600)
        self.assertEqual(p.generators[0].burned,F(7,15));self.assertEqual(p.generators[1].burned,F(2,3))
        self.assertEqual(p.signal.progress,600)
        for g in p.generators:self.assertEqual(g.tank+g.burned,g.maximum)
    def test_capacity_rejection_and_exhaustion_brownout(self):
        p=Power(Signal());p.generators=[Generator('fuel',4,5,F(1,1000),running=True),Generator('biomass',2,6,F(3),running=True)]
        self.assertTrue(p.switch('beacon',True));self.assertTrue(p.switch('workbench',True))
        p.advance(60);self.assertFalse(p.supplied);self.assertTrue(p.enabled['beacon']);self.assertTrue(p.enabled['workbench'])
        self.assertFalse(p.generators[0].running);self.assertFalse(p.switch('lighting',True))
        self.assertTrue(p.switch('workbench',False));self.assertTrue(p.supplied)
        progress=p.signal.progress;p.advance(10);self.assertEqual(p.signal.progress,progress+10)
    def test_hopper_manual_whole_loading(self):
        bag=Inventory(2);bag.add('biomass',10)
        g=Generator('biomass',2,6,F(11,2));self.assertEqual(g.load(bag,99),0)
        g.tank=F(5);self.assertEqual(g.load(bag,99),1);self.assertFalse(g.running)
        self.assertEqual(bag.items['biomass'],9)
    def test_outage_cap_and_blip_anti_exploit(self):
        s=Signal();s.advance(1800,True)
        for i in range(4):s.advance(100,False);s.advance(1,True)
        self.assertEqual(s.lost,F(250,4))
        s.advance(10000,False);self.assertEqual(s.lost,600)
        s.advance(150,True);old=s.lost;s.advance(150,False);self.assertEqual(s.lost,old)
    def test_startup_and_once_only_victory_clipping(self):
        p=Power(Signal(target=F(100)));p.advance(10000);self.assertEqual(p.signal.lost,0)
        p.generators[0].tank=F(5);p.generators[0].running=True;p.switch('beacon',True)
        elapsed,_=p.advance(10000);self.assertEqual(elapsed,100)
        before=p.generators[0].tank;self.assertEqual(p.advance(100)[0],0);self.assertEqual(p.generators[0].tank,before)
    def test_time_subdivision_equivalence(self):
        a=Power(Signal());a.generators[0].tank=F(1);a.generators[0].running=True;a.switch('beacon',True)
        b=copy.deepcopy(a);a.advance(2500)
        for i in range(2500):b.advance(1)
        self.assertEqual(a.signal,b.signal);self.assertEqual(a.generators,b.generators)

class ConfigurationTests(unittest.TestCase):
    def test_exact_budgets_and_seed_opportunities(self):
        config=load_config()
        for seed in range(64):
            config['seed']=seed
            for world,spec in config['worlds'].items():
                sites=make_sites(config,world)
                for key,total in spec['totals'].items():
                    got=sum(s.patch.capacity if key=='biomass' and s.patch else s.initial[key] for s in sites)
                    self.assertEqual(got,total)
                count=sum(s.stock['marrowvine_seed'] for s in sites)
                self.assertTrue(spec['seed_range'][0]<=count<=spec['seed_range'][1])
                self.assertTrue(all(s.id<4 for s in sites if s.stock['marrowvine_seed']))
                self.assertTrue(all(1800<=s.patch.period<=2400 for s in sites if s.patch))
    def test_implemented_rules_are_only_read(self):
        c=load_config();runtime=json.loads((ROOT/'data/farming.json').read_text())
        for crop,old in zip(('potato','glowcap'),runtime['crops']):
            self.assertEqual(c['crops'][crop]['days'],old['growthDays'])
            mapped={('biomass' if k=='organic' else k):v for k,v in old['yields'].items()}
            self.assertEqual(c['crops'][crop]['yield'],mapped)
        self.assertEqual(json.loads((ROOT/'data/rescue.json').read_text())['targetDays'],7)
        self.assertEqual(c['signal_days'],14)
    def test_legal_construction_footprints(self):
        s=Study(load_config(),'old_highway',2,'balanced');occupied=set()
        for x,y in s.sockets:
            for xx in (x,x+1):
                for yy in (y,y+1):
                    self.assertTrue(0<=xx<10 and 0<=yy<6)
                    self.assertNotIn(xx,(4,5));self.assertNotIn(yy,(2,3))
                    self.assertNotIn((xx,yy),occupied);occupied.add((xx,yy))
    def test_disabled_candidate_catalog_costs(self):
        c=load_config();self.assertEqual(len(c['candidate_projects']),10)
        self.assertEqual(c['candidate_projects']['Vehicle Restoration']['advanced'],2)
        self.assertFalse(set(c['candidate_projects'])&set(c['recipes']))

class SchedulerTests(unittest.TestCase):
    def test_determinism_and_actor_feasibility(self):
        c=load_config();a=Study(c,'old_highway',2,'infrastructure');b=Study(c,'old_highway',2,'infrastructure')
        self.assertEqual(a.run(),b.run());self.assertEqual(a.events,b.events)
        end={};discovered=set()
        for e in a.events:
            if e['event']!='action':continue
            who=e['actor'];self.assertGreaterEqual(e['start']+1e-6,end.get(who,0));end[who]=e['end']
            if e['kind']=='discover':discovered.add(e['where'])
            if e['kind']=='collect':self.assertIn(e['where'],discovered)
        a.audit();self.assertEqual(a.signal.progress,a.signal.target)
        for person in a.actors:
            occupied=sum(e['end']-e['start'] for e in a.events if e['event']=='action' and e['actor']==person.id)
            if person.action:occupied+=float(a.t)-person.action.data['_start']
            self.assertAlmostEqual(sum(person.activity.values()),occupied,places=5)
        first=[e['crop'] for e in a.events if e['event']=='plant'][:4]
        self.assertEqual(first,['potato','potato','glowcap','glowcap'])
        self.assertIsNotNone(a.self_date)
        proof=a.certificates[0];self.assertEqual(proof['surface_inputs'],0);self.assertEqual(proof['fuel_used'],0)
        self.assertGreater(proof['cycle_seconds'],0)
    def test_machinery_alone_is_not_sustainability(self):
        s=Study(load_config(),'old_highway',2,'balanced');s.power.generators.append(Generator('biomass',2,6,F(6),running=True))
        self.assertFalse(witness(s)['certified'])
    def test_setbacks_preserve_initial_generation(self):
        c=load_config();a=Study(c,'industrial_hollow',4,'balanced');b=Study(c,'industrial_hollow',4,'balanced','failed_expedition')
        self.assertEqual([s.initial for s in a.sites],[s.initial for s in b.sites])
        r=b.run();self.assertTrue(r['setback_triggered']);b.audit()
    def test_unvisited_contents_do_not_influence_strategy(self):
        c=load_config();a=Study(c,'old_highway',2,'balanced');b=Study(c,'old_highway',2,'balanced')
        b.sites[0].stock['marrowvine_seed']=99;b.sites[0].stock['food']=999
        self.assertEqual(a.expedition_plan(a.actors[1]),b.expedition_plan(b.actors[1]))
    def test_rejected_withdrawal_never_partially_mutates(self):
        from model import Action
        s=Study(load_config(),'old_highway',2,'balanced');a=s.actors[0]
        before=s.chest.items.copy();a.action=Action('withdraw','chest',2,{'key':'fuel','n':3,'_start':0})
        s.finish(a);self.assertEqual(s.chest.items,before);self.assertEqual(a.inventory.items['fuel'],0)
    def test_confirmed_run_uses_no_future_surface_food_or_power_inputs(self):
        s=Study(load_config(),'old_highway',2,'infrastructure');r=s.run()
        self.assertIsNotNone(r['underground_self_sufficient_second'])
        trial=r['trial']['start']
        for e in s.events:
            if e['event']=='action' and e.get('kind')=='collect' and e['second']>trial:
                self.assertNotIn(e['details']['key'],('food','fuel','biomass','marrowvine_seed'))
    def test_infrastructure_can_unlock_needed_mechanical_materials(self):
        s=Study(load_config(),'overgrown_valley',6,'infrastructure');a=s.actors[0]
        s.plots.extend([type(s.plots[0])(i) for i in range(4,7)])
        s.chest.add('metal',6);s.chest.add('basic',4);s.sites[4].visited=True
        self.assertEqual(s.next_project(a),'tool')
    def test_capacity_full_crop_remains_intact(self):
        from model import Action
        s=Study(load_config(),'old_highway',2,'balanced');a=s.actors[0]
        a.inventory=Inventory(1);a.inventory.add('food',10)
        p=s.plots[0];p.crop='potato';p.planted=-1200
        a.where='plot:0';a.action=Action('harvest','plot:0',3,{'plot':0,'_start':0})
        s.finish(a);self.assertEqual(p.crop,'potato');self.assertEqual(a.inventory.items['food'],10)
    def test_balanced_transport_goal_requests_actual_salvage(self):
        s=Study(load_config(),'old_highway',4,'balanced')
        s.plots.extend([type(s.plots[0])(i) for i in range(4,6)])
        s.upgrades=[{'project':'tool'},{'project':'biogenerator'}]
        s.chest.add('metal',5)
        self.assertEqual(s.next_project(s.actors[0]),'barrow')
        self.assertEqual(s.needs()['metal'],5)
    def test_certificate_checks_reserve_trough_before_harvest(self):
        from model import Plot
        s=Study(load_config(),'old_highway',2,'balanced')
        s.plots=[Plot(i,crop,-1800,ever_planted=True) for i,crop in enumerate(['potato','potato','glowcap','marrowvine','marrowvine'])]
        s.chest.add('food',20)
        s.power.generators.append(Generator('biomass',2,6,F(2),running=True));s.power.enabled['beacon']=True
        proof=witness(s)
        self.assertFalse(proof['certified']);self.assertEqual(proof['reason'],'one_day_reserve_not_maintained')
    def test_cutoff_is_not_a_simulator_error(self):
        c=load_config();c['observation_days']=1
        s=Study(c,'old_highway',2,'infrastructure');r=s.run()
        self.assertEqual(r['outcome'],'observation_cutoff');self.assertEqual(r['completion_events'],0)

if __name__=='__main__':unittest.main()
