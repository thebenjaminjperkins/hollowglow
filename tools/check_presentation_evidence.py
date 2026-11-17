#!/usr/bin/env python3
"""Check archived M6c gameplay-scale captures against the physically identical M6b baseline."""
import json,statistics
from pathlib import Path
from PIL import Image,ImageChops,ImageStat

ROOT=Path(__file__).resolve().parents[1]
CAPTURES=ROOT/'docs/screenshots/m6c'
MEASURES=ROOT/'docs/measurements/m6c'

def main():
    before=json.loads((MEASURES/'before.json').read_text())
    after=json.loads((MEASURES/'after.json').read_text())
    for key in ('Collisions','Ground','Sources','MoveSpeed','InteractionRange'):
        assert before[key]==after[key],key
    assert before['Views']==after['Views'],'Capture positions and clock times must match'
    samples={}
    # These are open floor probes in the fixed seed-1701 camera, not props or wall pixels.
    probes={'left_floor':(640,304),'right_floor':(896,304),'behind_divider':(832,240),'through_door':(640,496)}
    for tag in ('before','after'):
        off=Image.open(CAPTURES/tag/'occlusion-off.png').convert('RGB')
        assert off.size==(1536,864)
        for side in ('left','right'):
            im=Image.open(CAPTURES/tag/f'occlusion-{side}.png').convert('RGB')
            samples[tag+'_'+side]={}
            for name,(x,y) in probes.items():
                box=(x-3,y-3,x+4,y+4)
                delta=statistics.mean(ImageStat.Stat(ImageChops.difference(im.crop(box),off.crop(box))).mean)
                samples[tag+'_'+side][name]=round(delta,3)
    assert samples['before_left']['behind_divider']>10,'Baseline must reproduce wall leakage'
    assert samples['after_left']['left_floor']>10,'Lamp must light its own side'
    assert samples['after_left']['behind_divider']<1,'Solid wall must block diagnostic light'
    assert samples['after_left']['through_door']>5,'Open doorway must transmit light'
    assert samples['after_right']['left_floor']<1,'Reversed source must be occluded in reverse'
    assert samples['after_right']['right_floor']>10,'Reversed source must still illuminate its own side'
    for tag in ('before','after'):
        for view in before['Views']:
            assert Image.open(CAPTURES/tag/(view['Name']+'.png')).size==(1536,864)
    cpu=after['AdapterCpuMilliseconds']
    report={'identicalCollisionShapes':len(after['Collisions']),'geometrySourcesMovementAndCaptureTimesUnchanged':True,
            'lightDeltaRgbMean':samples,'presentationAdapterCpuMs':{'median':statistics.median(cpu),'p95':sorted(cpu)[int(len(cpu)*.95)],'max':max(cpu)},
            'limitations':'CPU adapter timings exclude rendering/GPU/capture costs. Fixed simulation delta is not a measured frame rate. Photometry uses static floor with discovery masking disabled only in the diagnostic shots.'}
    (MEASURES/'validation.json').write_text(json.dumps(report,indent=2)+'\n')
    print(json.dumps(report,indent=2))
    print('PASS: matched gameplay-scale evidence, unchanged physical world, bidirectional wall occlusion and transmitting doorway')

if __name__=='__main__':main()
