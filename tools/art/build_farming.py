#!/usr/bin/env python3
"""Process the preserved imagegen kit into native, palette-shared farming PNGs."""
import argparse
import json
from pathlib import Path
from PIL import Image
from build_sanctuary import fit, png, extract_cell
ROOT = Path(__file__).resolve().parents[2]
DEST = ROOT / 'assets/farming'

def outputs():
    source = Image.open(DEST / 'source/kit.png').convert('RGBA')
    assert source.getchannel('A').getextrema()[0] == 0, 'Source must preserve transparency'
    sheet = Image.new('RGBA', (96, 80))
    limits = [(16, 18), (20, 25), (28, 34)]
    for row in range(2):
        for stage in range(3):
            image = fit(extract_cell(source, stage, row, 4, 3), (32, 40), limits[stage], (16, 38))
            sheet.paste(image, (stage * 32, row * 40))
    bed = fit(extract_cell(source, 0, 2, 4, 3), (72, 56), (64, 32), (36, 50))
    icons = Image.new('RGBA', (72, 24))
    for i, (x, y) in enumerate([(3, 0), (3, 1), (1, 2)]):
        icons.paste(fit(extract_cell(source, x, y, 4, 3), (24, 24), (22, 22), (12, 23)), (i * 24, 0))
    return {'crops.png': png(sheet), 'bed.png': png(bed), 'icons.png': png(icons),
            'manifest.json': (json.dumps({'rows': {'cave_potato': 0, 'glowcap': 1}, 'frame': [32, 40],
                                        'anchor': [16, 38], 'native_pixels_per_world_unit': 2}, indent=2) + '\n').encode()}

def main():
    parser = argparse.ArgumentParser(description=__doc__); parser.add_argument('--check', action='store_true'); args = parser.parse_args()
    for name, data in outputs().items():
        if args.check: assert (DEST / name).read_bytes() == data, name
        else: (DEST / name).write_bytes(data)
    print('PASS: reproducible farming stages, bed and inventory icons from preserved source')
if __name__ == '__main__': main()
