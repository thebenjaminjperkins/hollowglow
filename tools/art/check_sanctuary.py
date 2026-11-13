#!/usr/bin/env python3
"""Verify palette/density metadata, terrain joins/masks, preserved gameplay and captures."""
import argparse
import hashlib
import json
from pathlib import Path
import shutil
from PIL import Image, ImageChops, ImageStat, ImageDraw

ROOT = Path(__file__).resolve().parents[2]
ASSETS = ROOT / 'assets/sanctuary'


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--captures', type=Path)
    parser.add_argument('--walk-captures', type=Path)
    args = parser.parse_args()
    manifest = json.loads((ASSETS / 'manifest.json').read_text())
    palette = {tuple(bytes.fromhex(c)) for c in manifest['palette']}
    assert len(palette) == 64 and manifest['native_pixels_per_world_unit'] == 2
    for path in ASSETS.glob('*.png'):
        image = Image.open(path).convert('RGBA')
        pixels = image.get_flattened_data() if hasattr(image, 'get_flattened_data') else image.getdata()
        assert all(p[3] in (0, 255) and (p[3] == 0 or p[:3] in palette) for p in set(pixels)), path
    for name, spec in manifest['sprites'].items():
        image = Image.open(ASSETS / spec['file'])
        assert image.size == (spec['size'][0] * spec['columns'], spec['size'][1]), name
        assert all(0 <= spec['anchor'][i] <= spec['size'][i] for i in (0, 1)), name
    character = Image.open(ASSETS / 'character.png').convert('RGBA')
    assert character.size == (240, 224)
    for row in range(4):
        for col in range(6):
            frame = character.crop((col * 40, row * 56, (col + 1) * 40, (row + 1) * 56))
            assert frame.getbbox()[3] == 52, (row, col, 'foot anchor drift')
    assert (ASSETS / 'machine.png').read_bytes() == (ROOT / 'assets/trials/machinery/candidate.png').read_bytes()
    floor = Image.open(ASSETS / 'floor.png').convert('RGBA')
    assert floor.size == (1024, 512)
    for material in (0, 16):
        patch = Image.new('RGBA', (128, 128))
        for v in range(16):
            index = (material + v) * 16
            x, y = index % 32 * 32, index // 32 * 32
            patch.paste(floor.crop((x, y, x + 32, y + 32)), (v % 4 * 32, v // 4 * 32))
        assert all(patch.getpixel((0, p)) == patch.getpixel((127, p)) for p in range(128))
        assert all(patch.getpixel((p, 0)) == patch.getpixel((p, 127)) for p in range(128))
    assert floor.getpixel((32, 0))[3] == 0 and floor.getpixel((48, 16))[3] == 255
    preserved = json.loads((ASSETS / 'source/preserved.json').read_text())
    # Preserve historical snapshots; only explicitly approved milestone runtime files evolve.
    evolved = json.loads((ROOT / 'tools/art/m2a_runtime_hashes.json').read_text())
    evolved.update(json.loads((ROOT / 'tools/art/inventory_ux_runtime_hashes.json').read_text()))
    evolved.update(json.loads((ROOT / 'tools/art/m2b_runtime_hashes.json').read_text()))
    evolved.update(json.loads((ROOT / 'tools/art/m3a_runtime_hashes.json').read_text()))
    evolved.update(json.loads((ROOT / 'tools/art/m3b_runtime_hashes.json').read_text()))
    evolved.update(json.loads((ROOT / 'tools/art/m4a_runtime_hashes.json').read_text()))
    for path, digest in preserved.items():
        digest = evolved.get(path, digest)
        contents = (ROOT / path).read_bytes()
        if path == 'project.godot':
            # Approved integration changes only the entry scene. Verify the old
            # snapshot after reversing that exact edit, rather than weakening it.
            assert b'run/main_scene="res://scenes/SanctuaryRoom.tscn"' in contents
            contents = contents.replace(b'run/main_scene="res://scenes/SanctuaryRoom.tscn"',
                                        b'run/main_scene="res://scenes/Threshold.tscn"')
        assert hashlib.sha256(contents).hexdigest() == digest, path
    print(f'PASS: common palette, 2px/world density, anchors, periodic terrain joins, corner masks; {len(preserved) - len(evolved) - 1} legacy files unchanged; approved M2a / inventory UX / M2b / M3a / M3b / M4a runtime and entry changes verified')
    if args.captures:
        out = ROOT / 'docs/screenshots/sanctuary-room'
        out.mkdir(parents=True, exist_ok=True)
        for name in ('room', 'neutral', 'home', 'storage', 'native'):
            image = Image.open(args.captures / (name + '.png'))
            assert image.size == ((768, 432) if name == 'native' else (1536, 864)), name
            shutil.copyfile(args.captures / (name + '.png'), out / (name + '.png'))
        lit = Image.open(out / 'room.png').convert('RGB')
        neutral = Image.open(out / 'neutral.png').convert('RGB')
        difference = ImageStat.Stat(ImageChops.difference(lit, neutral)).mean
        assert max(difference) > 2, 'Adjustable lighting must measurably change rendering'
        print('PASS: actual gameplay/native captures and independently adjustable light output')

    if args.walk_captures:
        out = ROOT / 'docs/screenshots/sanctuary-room/walk-gameplay.png'
        board = Image.new('RGB', (568, 1228), '#202832')
        draw = ImageDraw.Draw(board)
        draw.text((8, 6), 'Actual gameplay captures; 4 successive walk phases, no resizing', fill='white')
        labels = ('South', 'North', 'West', 'East', 'Southeast', 'Southwest', 'Northeast', 'Northwest')
        for row, label in enumerate(labels):
            draw.text((8, 38 + row * 148), label, fill='white')
            for phase in range(4):
                frame = Image.open(args.walk_captures / f'{row}-{phase}.png').convert('RGB')
                assert frame.size == (112, 144)
                board.paste(frame, (116 + phase * 112, 32 + row * 148))
        board.save(out)
        print('PASS: eight-direction runtime walk contact sheet at actual gameplay scale')


if __name__ == '__main__':
    main()
