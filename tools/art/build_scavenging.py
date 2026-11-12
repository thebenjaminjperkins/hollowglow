#!/usr/bin/env python3
"""Export two small matched PNG source families; retain shared scale between states."""
import argparse
import hashlib
import json
from pathlib import Path
from PIL import Image
from build_sanctuary import png, quantize
ROOT = Path(__file__).resolve().parents[2]
DEST = ROOT / 'assets/scavenging'


def outputs():
    results = {}
    sources = {}
    for family in ('container', 'plant'):
        path = DEST / 'source' / (family + '.png')
        source = Image.open(path).convert('RGBA')
        assert source.getchannel('A').getextrema()[0] == 0, 'Source must have real transparency'
        cells = []
        for col in range(2):
            cut = source.crop((source.width * col // 2, 0, source.width * (col + 1) // 2, source.height))
            cut.putalpha(cut.getchannel('A').point(lambda a: 255 if a >= 176 else 0))
            cells.append(cut.crop(cut.getbbox()))
        ratio = min(32 / max(c.width for c in cells), 30 / max(c.height for c in cells))
        atlas = Image.new('RGBA', (80, 40))
        for col, cut in enumerate(cells):
            size = (round(cut.width * ratio), round(cut.height * ratio))
            cut = cut.convert('RGBa').resize(size, Image.Resampling.LANCZOS).convert('RGBA')
            frame = Image.new('RGBA', (40, 40))
            frame.alpha_composite(cut, (20 - size[0] // 2, 36 - size[1]))
            atlas.paste(quantize(frame), (col * 40, 0))
        results['food_' + family + '.png'] = png(atlas)
        sources[family] = hashlib.sha256(path.read_bytes()).hexdigest()
    results['manifest.json'] = (json.dumps({'native_pixels_per_world_unit': 2, 'frame': [40, 40], 'anchor': [20, 36],
        'states': ['available', 'depleted'], 'palette': 'assets/sanctuary/manifest.json', 'source_sha256': sources}, indent=2) + '\n').encode()
    return results


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--check', action='store_true'); args = parser.parse_args()
    for name, data in outputs().items():
        path = DEST / name
        if args.check: assert path.read_bytes() == data, name
        else: path.write_bytes(data)
    print('PASS: reproducible reference-derived Food source PNGs; shared scale and roots across available/depleted states')

if __name__ == '__main__': main()
