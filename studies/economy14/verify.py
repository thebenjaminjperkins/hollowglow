#!/usr/bin/env python3
"""Verify report integrity, ledger balances, actor schedules and paired provenance."""
import argparse
from collections import Counter
import gzip
import hashlib
import json
from pathlib import Path
from model import HERE, ROOT, RESOURCES, load_config


def verify(folder,compare=None,preserved=None):
    checks=json.loads((folder/'checksums.json').read_text())
    for name,digest in checks.items():
        assert hashlib.sha256((folder/name).read_bytes()).hexdigest()==digest,name
    manifest=json.loads((folder/'inputs.json').read_text())
    for name,digest in manifest['source_hashes'].items():
        p=Path(name) if Path(name).is_absolute() else ROOT/name
        assert hashlib.sha256(p.read_bytes()).hexdigest()==digest,('source changed since run',name)
    config=manifest['config'];rows=json.loads((folder/'results.json').read_text())
    assert len(rows)==81 and len({r['scenario'] for r in rows})==81
    assert Counter(r['setback'] for r in rows)=={'baseline':27,'missed_harvest':27,'failed_expedition':27}
    lookup={r['scenario']:r for r in rows}
    for r in rows:
        assert r['outcome'] in ('rescued','observation_cutoff')
        assert r['rescued']==(r['completion_events']==1)
        if r['rescued']:assert abs(r['signal_days']-14)<1e-8
        else:assert r['second']==28*600
        for d in [*r['daily'],r]:
            for key in RESOURCES:
                left=config['starting_storage'].get(key,0)+(config['starting_food_per_player']*r['players'] if key=='food' else 0)
                left+=d['recovered'].get(key,0)+d['crop_and_propagation_output'].get(key,0)
                right=d['held'].get(key,0)+d['spent'].get(key,0)+(d['consumed_food'] if key=='food' else 0)
                if key in ('fuel','biomass'):
                    tank=d[key+'_reserve']-(d['held'].get(key,0)-d['cargo'].get(key,0))
                    right+=tank+d['burned'][key]
                assert abs(left-right)<3e-6,(r['scenario'],d['date'],key,left,right)
            for key,total in r['initial_world'].items():
                if key=='biomass':total+=d['wild_regrown']
                assert abs(total-d['recovered'].get(key,0)-d['remaining'].get(key,0))<3e-6,(r['scenario'],key,'world recovery')
            assert sum(d['personal_food'].values())<=d['food_reserve']
        end={};location={i:'chest' for i in range(r['players'])};visited=set()
        events=[json.loads(line) for line in gzip.open(folder/'events'/f"{r['scenario']}.jsonl.gz",'rt')]
        for e in events:
            if e['event']!='action':continue
            actor=e['actor'];assert e['start']+1e-6>=end.get(actor,0);end[actor]=e['end']
            if e['kind']=='move':location[actor]=e['where']
            else:assert location[actor]==e['where'],('remote action',e)
            if e['kind']=='discover':visited.add(e['where'])
            if e['kind']=='collect':assert e['where'] in visited
            if e['kind']=='assemble' and e['applied']:
                assert e['details']['recipe'] not in config['candidate_projects']
        first=[e['crop'] for e in events if e['event']=='plant'][:4]
        assert first==['potato','potato','glowcap','glowcap'],r['scenario']
        if r['underground_self_sufficient_second'] is not None:
            assert r['certificates'] and r['trial']
            for c in r['certificates']:
                assert c['certified'] and c['cycle_seconds']>0 and c['surface_inputs']==c['fuel_used']==0
                assert c['farm_worker_max_seconds_per_window']<=100
                assert c['service_worker_max_seconds_per_window']<=100
            start=r['trial']['start']
            assert r['underground_self_sufficient_second']-start>=3900
            for e in events:
                if e['event']=='action' and e['kind']=='collect' and e['second']>start:
                    assert e['details']['key'] not in ('food','fuel','biomass','marrowvine_seed')
            assert r['missed_meals']==r['trial']['missed'] and r['outage_seconds']==r['trial']['outages']
        if r['setback']!='baseline':
            b=lookup[r['scenario'].replace('__'+r['setback'],'__baseline')]
            assert r['initial_world']==b['initial_world'] and r['seed_opportunities']==b['seed_opportunities']
    if compare:
        other=json.loads((compare/'checksums.json').read_text())
        assert checks==other,'Replayed artifacts differ'
    if preserved:
        old=json.loads(preserved.read_text())
        for name,digest in old.items():assert hashlib.sha256((ROOT/name).read_bytes()).hexdigest()==digest,name
    print(f'PASS: {len(rows)} outcomes, exact-input hashes, all artifact hashes, independent daily resource balances, actor/location schedules, paired generation and underground witnesses'+('; byte-identical replay' if compare else ''))

if __name__=='__main__':
    p=argparse.ArgumentParser(description=__doc__);p.add_argument('--reports',type=Path,default=HERE/'reports')
    p.add_argument('--compare-dir',type=Path);p.add_argument('--preserved-snapshot',type=Path)
    a=p.parse_args();verify(a.reports,a.compare_dir,a.preserved_snapshot)
