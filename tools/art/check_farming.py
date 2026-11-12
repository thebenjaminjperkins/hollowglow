#!/usr/bin/env python3
"""Check farming native pixels, shared palette, anchors and preserved source recipe."""
import hashlib
import json
from pathlib import Path
from PIL import Image
ROOT = Path(__file__).resolve().parents[2]
ASSETS = ROOT / 'assets/farming'
palette = {tuple(bytes.fromhex(c)) for c in json.loads((ROOT / 'assets/trials/machinery/source/processing.json').read_text())['palette']}
for name, size in [('crops', (96, 80)), ('bed', (72, 56)), ('icons', (72, 24))]:
    im = Image.open(ASSETS / (name + '.png')).convert('RGBA'); assert im.size == size
    data = im.get_flattened_data() if hasattr(im, 'get_flattened_data') else im.getdata()
    assert all(p[3] in (0, 255) and (p[3] == 0 or p[:3] in palette) for p in set(data)), name
sheet = Image.open(ASSETS / 'crops.png')
for row in range(2):
    bounds = [sheet.crop((stage * 32, row * 40, (stage + 1) * 32, (row + 1) * 40)).getbbox() for stage in range(3)]
    assert all(b and b[3] == 38 for b in bounds), 'Root anchor drift'
    assert (bounds[0][2] - bounds[0][0]) < (bounds[2][2] - bounds[2][0]), 'Mature silhouette should grow'
recipe = json.loads((ASSETS / 'source/recipe.json').read_text())
assert hashlib.sha256((ASSETS / 'source/kit.png').read_bytes()).hexdigest() == recipe['source_sha256']
assert json.loads((ASSETS / 'manifest.json').read_text())['native_pixels_per_world_unit'] == 2
print('PASS: farming shared palette, binary alpha, native resolution, root anchors, growth silhouettes and source provenance')
