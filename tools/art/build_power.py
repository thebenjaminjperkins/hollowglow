#!/usr/bin/env python3
"""Process the original matched canister source into two runtime frames and an icon."""
import argparse
import hashlib
import json
from pathlib import Path
from PIL import Image
from build_sanctuary import png, quantize
ROOT = Path(__file__).resolve().parents[2]
DEST = ROOT / 'assets/power'
def outputs():
    source = Image.open(DEST / 'source/fuel.png').convert('RGBA')
    assert source.getchannel('A').getextrema()[0] == 0
    cuts = []
    for col in range(2):
        cut = source.crop((source.width * col // 2, 0, source.width * (col + 1) // 2, source.height))
        cut.putalpha(cut.getchannel('A').point(lambda a: 255 if a >= 176 else 0))
        cuts.append(cut.crop(cut.getbbox()))
    ratio = min(30 / max(c.width for c in cuts), 30 / max(c.height for c in cuts))
    sheet = Image.new('RGBA', (80, 40))
    for i, cut in enumerate(cuts):
        size = (round(cut.width * ratio), round(cut.height * ratio))
        resized = cut.convert('RGBa').resize(size, Image.Resampling.LANCZOS).convert('RGBA')
        frame = Image.new('RGBA', (40, 40)); frame.alpha_composite(resized, (20-size[0]//2,36-size[1]))
        sheet.paste(quantize(frame),(40*i,0))
    icon = cuts[0].convert('RGBa'); ratio = min(22/icon.width,22/icon.height)
    size = (round(icon.width*ratio),round(icon.height*ratio)); icon=icon.resize(size,Image.Resampling.LANCZOS).convert('RGBA')
    framed = Image.new('RGBA',(24,24)); framed.alpha_composite(icon,(12-size[0]//2,23-size[1]))
    manifest = {'frame':[40,40],'anchor':[20,36],'states':['available','depleted'],'native_pixels_per_world_unit':2,
        'source_sha256':hashlib.sha256((DEST / 'source/fuel.png').read_bytes()).hexdigest(),'palette':'assets/sanctuary/manifest.json'}
    return {'fuel_cache.png':png(sheet),'fuel_icon.png':png(quantize(framed)), 'manifest.json':(json.dumps(manifest,indent=2)+'\n').encode()}
if __name__=='__main__':
    p=argparse.ArgumentParser();p.add_argument('--check',action='store_true');args=p.parse_args()
    for name,data in outputs().items():
        if args.check: assert (DEST/name).read_bytes()==data,name
        else:(DEST/name).write_bytes(data)
    print('PASS: reproducible shared-palette Fuel cache states and inventory icon from preserved generated source')
