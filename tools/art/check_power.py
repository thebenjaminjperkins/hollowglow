#!/usr/bin/env python3
"""Validate the bounded M4a PNG kit against the approved native art standard."""
import hashlib
import json
from pathlib import Path
from PIL import Image,ImageChops
ROOT=Path(__file__).resolve().parents[2]
D=ROOT/'assets/power'
meta=json.loads((D/'manifest.json').read_text())
palette={tuple(bytes.fromhex(c)) for c in json.loads((ROOT/'assets/sanctuary/manifest.json').read_text())['palette']}
assert meta['native_pixels_per_world_unit']==2 and meta['frame']==[40,40] and meta['anchor']==[20,36]
for name,size in [('fuel_cache',(80,40)),('fuel_icon',(24,24))]:
    im=Image.open(D/(name+'.png')).convert('RGBA');assert im.size==size
    pixels=im.get_flattened_data() if hasattr(im,'get_flattened_data') else im.getdata()
    assert all(p[3] in (0,255) and (p[3]==0 or p[:3] in palette) for p in set(pixels)),name
sheet=Image.open(D/'fuel_cache.png').convert('RGBA')
frames=[sheet.crop((40*i,0,40*i+40,40)) for i in range(2)]
assert all(f.getbbox()[3]==36 for f in frames)
assert ImageChops.difference(*frames).convert('RGB').getbbox()
assert hashlib.sha256((D/'source/fuel.png').read_bytes()).hexdigest()==meta['source_sha256']
assert (D/'source/prompt.txt').is_file()
print('PASS: M4a shared palette, binary alpha, 2px/world density, anchors, distinct states, Fuel icon and preserved source/prompt')
