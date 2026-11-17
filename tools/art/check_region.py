#!/usr/bin/env python3
"""Validate M6a exports independently of the Godot import cache."""
import hashlib,json
from pathlib import Path
from PIL import Image
ROOT=Path(__file__).resolve().parents[2]
ASSETS=ROOT/'assets/region'
m=json.loads((ASSETS/'manifest.json').read_text())
assert m['palette']==json.loads((ROOT/'assets/sanctuary/manifest.json').read_text())['palette']
assert m['native_pixels_per_world_unit']==2 and len(m['palette'])==64
palette={tuple(bytes.fromhex(c)) for c in m['palette']}
for p in ASSETS.glob('*.png'):
    im=Image.open(p).convert('RGBA')
    assert all(a in (0,255) and (not a or (r,g,b) in palette) for r,g,b,a in set(im.get_flattened_data() if hasattr(im,"get_flattened_data") else im.getdata())),p
for name,spec in m['sprites'].items():
    im=Image.open(ASSETS/spec['file'])
    assert list(im.size)==spec['size'],name
    assert all(0<=spec['anchor'][i]<=spec['size'][i] for i in (0,1)),name
for name,digest in m['sources'].items():
    assert hashlib.sha256((ASSETS/'source'/f'{name}.png').read_bytes()).hexdigest()==digest
atlas=Image.open(ASSETS/'terrain.png')
assert atlas.size==(2048,3072)
for material in range(6):
    patch=Image.new('RGBA',(256,256))
    for phase in range(64):
        index=(material*64+phase)*16
        x,y=index%64*32,index//64*32
        patch.paste(atlas.crop((x,y,x+32,y+32)),(phase%8*32,phase//8*32))
    assert all(patch.getpixel((0,p))==patch.getpixel((255,p)) for p in range(256)),material
    assert all(patch.getpixel((p,0))==patch.getpixel((p,255)) for p in range(256)),material
assert json.loads((ASSETS/'source/prompts.json').read_text())['tool']=='built-in image_gen'
print('PASS: shared 64-color palette, binary alpha, native density, anchors, source hashes, six periodic materials and image provenance')
