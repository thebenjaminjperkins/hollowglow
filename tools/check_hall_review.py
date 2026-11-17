#!/usr/bin/env python3
"""Verify preserved geometry, real walking, clearance and rendered wall-light evidence."""
import hashlib,json,statistics
from pathlib import Path
from PIL import Image,ImageChops,ImageStat
ROOT=Path(__file__).resolve().parents[1]
M=ROOT/'docs/measurements/sanctuary-hall';P=ROOT/'docs/screenshots/sanctuary-hall'
def main():
    before=json.loads((M/'before.json').read_text());after=json.loads((M/'after.json').read_text());hall=json.loads((M/'hall.json').read_text())
    for key in ('Collisions','Ground','Sources','MoveSpeed','InteractionRange','Views'):assert before[key]==after[key],key
    assert len(hall['Walking'])==30 and len({(r['From'],r['To']) for r in hall['Walking']})==30
    assert max(r['Seconds'] for r in hall['Walking'])<6
    assert hall['SimulatedActors']==6 and hall['MinimumActorSeparation']>=8 and hall['LiveActorCollisionMask']==1
    assert hall['StartingPlots']==4 and hall['InstalledPlots']==5 and len(hall['Hud'])==6 and all(r['Passed'] for r in hall['Hud'])
    lighting={}
    for side in ['left','right']:
        on=Image.open(P/'after'/f'occlusion-{side}.png').convert('RGB');off=Image.open(P/'after'/'occlusion-off.png').convert('RGB')
        lighting[side]={}
        for name,(x,y) in {'left_floor':(640,304),'right_floor':(896,304),'blocked_right':(832,240),'doorway':(640,496)}.items():
            box=(x-3,y-3,x+4,y+4)
            lighting[side][name]=round(statistics.mean(ImageStat.Stat(ImageChops.difference(on.crop(box),off.crop(box))).mean),3)
    assert lighting['left']['blocked_right']<1 and lighting['right']['left_floor']<1
    assert lighting['left']['doorway']>5 and lighting['right']['right_floor']>10
    for p,h in json.loads((M/'preserved-inputs.json').read_text()).items():assert hashlib.sha256((ROOT/p).read_bytes()).hexdigest()==h,p
    for tag in ['before','after']:
        for view in before['Views']:assert Image.open(P/tag/(view['Name']+'.png')).size==(1536,864)
    report={'LegacyCollisionShapesIdentical':len(after['Collisions']),'AllMatchedViewsAndClockTimesIdentical':True,
        'LightingRgbDelta':lighting,'MeasuredStorageWalkSeconds':{r['To']:round(r['Seconds'],3) for r in hall['Walking'] if r['From']=='storage'},
        'MinimumSimulatedActorSeparation':hall['MinimumActorSeparation'],'MaximumPairwiseWalkSeconds':max(r['Seconds'] for r in hall['Walking']),
        'HudCapturePixels':{p.name:list(Image.open(p).size) for p in (P/'hall').glob('hud-*.png')},
        'Notes':'Captures retain native integer-scaled game-content pixels; letterboxing and OS frame are not part of viewport PNGs. Wide-body and six-actor tests are physical proxies, not implemented multiplayer/vehicles.'}
    (M/'summary.json').write_text(json.dumps(report,indent=2)+'\n');print(json.dumps(report,indent=2));print('PASS: hall follow-up preservation, exact matched views, walking/clearance and rendered lighting')
if __name__=='__main__':main()
