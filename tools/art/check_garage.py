#!/usr/bin/env python3
"""Verify garage exports and reproducible source processing without Godot."""
import hashlib, json
from pathlib import Path
from PIL import Image
from build_garage import exports

ROOT=Path(__file__).resolve().parents[2]
ASSETS=ROOT/'assets/garage'
m=json.loads((ASSETS/'manifest.json').read_text())
assert m['palette']==json.loads((ROOT/'assets/sanctuary/manifest.json').read_text())['palette']
assert len(m['palette'])==64 and m['native_pixels_per_world_unit']==2
palette={tuple(bytes.fromhex(c)) for c in m['palette']}
for p in ASSETS.glob('*.png'):
    im=Image.open(p).convert('RGBA')
    pixels=im.get_flattened_data() if hasattr(im,'get_flattened_data') else im.getdata()
    assert all(a in (0,255) and (not a or (r,g,b) in palette) for r,g,b,a in set(pixels)),p
for name,spec in m['sprites'].items():
    im=Image.open(ASSETS/(name+'.png')).convert('RGBA')
    w,h=spec['size']
    assert im.size==(w*spec['columns'],h)
    assert all(0<=spec['anchor'][i]<=spec['size'][i] for i in (0,1))
    for col in range(spec['columns']):
        assert im.crop((col*w,0,(col+1)*w,h)).getbbox(),(name,col)
assert m['sprites']['van']=={'size':[128,192],'anchor':[64,184],'columns':3}
floor=Image.open(ASSETS/'floor.png')
assert floor.size==(512,512)
for y in (0,256):
    for x in (0,256):
        assert all(floor.getpixel((x,p+y))==floor.getpixel((x+255,p+y)) for p in range(256))
        assert all(floor.getpixel((p+x,y))==floor.getpixel((p+x,y+255)) for p in range(256))
for name,digest in m['sources'].items():
    assert hashlib.sha256((ASSETS/'source'/name).read_bytes()).hexdigest()==digest
assert json.loads((ASSETS/'source/prompts.json').read_text())['tool']=='built-in image_gen'
for name,data in exports().items():assert (ASSETS/name).read_bytes()==data,name
print('PASS: deterministic exports, approved 64 colors, binary alpha, density, shared state-frame anchors, periodic floors and image provenance')
