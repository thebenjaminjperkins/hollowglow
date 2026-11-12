#!/usr/bin/env python3
"""Check native sprite density, shared palette, distinct states and source provenance."""
import hashlib
import json
from pathlib import Path
from PIL import Image, ImageChops
ROOT = Path(__file__).resolve().parents[2]
DEST = ROOT / 'assets/scavenging'
meta = json.loads((DEST / 'manifest.json').read_text())
palette = {tuple(bytes.fromhex(c)) for c in json.loads((ROOT / 'assets/sanctuary/manifest.json').read_text())['palette']}
assert meta['native_pixels_per_world_unit'] == 2 and meta['frame'] == [40, 40] and meta['anchor'] == [20, 36]
for family in ('plant', 'container'):
    image = Image.open(DEST / ('food_' + family + '.png')).convert('RGBA')
    assert image.size == (80, 40)
    pixels = image.get_flattened_data() if hasattr(image, 'get_flattened_data') else image.getdata()
    assert all(p[3] in (0, 255) and (p[3] == 0 or p[:3] in palette) for p in set(pixels))
    states = [image.crop((c * 40, 0, c * 40 + 40, 40)) for c in range(2)]
    assert all(s.getbbox() and s.getbbox()[3] == 36 for s in states), 'Ground anchor drift'
    assert ImageChops.difference(states[0], states[1]).convert('RGB').getbbox(), 'States must differ'
    assert hashlib.sha256((DEST / 'source' / (family + '.png')).read_bytes()).hexdigest() == meta['source_sha256'][family]
    assert (DEST / 'source' / (family + '-prompt.txt')).is_file()
print('PASS: scavenging 2px/world density, shared palette, binary alpha, ground anchors, distinct states and preserved generated sources/prompts')
