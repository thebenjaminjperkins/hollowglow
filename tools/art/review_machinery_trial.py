#!/usr/bin/env python3
"""Validate the isolated trial and assemble labeled, unscaled comparison evidence."""
import argparse
import hashlib
import json
from pathlib import Path
import shutil

from PIL import Image, ImageDraw, ImageFont

ROOT = Path(__file__).resolve().parents[2]
TRIAL = ROOT / 'assets/trials/machinery'
OUT = ROOT / 'docs/screenshots/machinery-trial'


def check_sources():
    baseline = json.loads((TRIAL / 'source/baseline_hashes.json').read_text())
    evolved = json.loads((ROOT / 'tools/art/m2a_runtime_hashes.json').read_text())
    evolved.update(json.loads((ROOT / 'tools/art/inventory_ux_runtime_hashes.json').read_text()))
    evolved.update(json.loads((ROOT / 'tools/art/m2b_runtime_hashes.json').read_text()))
    for relative, digest in baseline.items():
        digest = evolved.get(relative, digest)
        contents = (ROOT / relative).read_bytes()
        if relative == 'project.godot':
            contents = contents.replace(b'run/main_scene="res://scenes/SanctuaryRoom.tscn"',
                                        b'run/main_scene="res://scenes/Threshold.tscn"')
        assert hashlib.sha256(contents).hexdigest() == digest, f'Protected file changed: {relative}'
    candidate = Image.open(TRIAL / 'candidate.png').convert('RGBA')
    metadata = json.loads((TRIAL / 'source/processing.json').read_text())
    assert candidate.size == (64, 64)
    pixels = list(candidate.get_flattened_data() if hasattr(candidate, 'get_flattened_data') else candidate.getdata())
    assert {pixel[3] for pixel in pixels} <= {0, 255}, 'Candidate must have crisp binary alpha'
    colors = {pixel[:3] for pixel in pixels if pixel[3]}
    palette = {tuple(bytes.fromhex(color)) for color in metadata['palette']}
    assert len(palette) == 64 and colors <= palette
    assert len(colors) == metadata['used_colors']
    mask = Image.open(TRIAL / 'source/mask.png')
    assert mask.getpixel((1250 - 842, 90 - 8)) == 0, 'Exhaust opening must remain transparent'
    print(f'PASS: {len(baseline)} protected snapshots match (approved gameplay / UX hashes and default-entry change normalized); 64-color palette, {len(colors)} used colors, crisp native pixels')


def text(draw, xy, value, size=12):
    draw.text(xy, value, font=ImageFont.load_default(size=size), fill='#f1dfad')


def native_board():
    board = Image.new('RGB', (432, 160), '#202832')
    draw = ImageDraw.Draw(board)
    text(draw, (12, 10), 'Native asset pixels - no enlargement', 14)
    original = Image.open(ROOT / 'assets/benchmark/machine.png').convert('RGBA')
    candidate = Image.open(TRIAL / 'candidate.png').convert('RGBA')
    draw.line((20, 94, 412, 94), fill='#47555e')
    board.paste(original, (72, 64), original)
    board.paste(candidate, (240, 35), candidate)
    text(draw, (35, 113), 'Original: 32 x 32')
    text(draw, (208, 113), 'Candidate: 64 x 64')
    text(draw, (16, 140), 'Same foot baseline; source aspect ratio retained.', 11)
    board.save(OUT / 'native-comparison.png')


def capture_boards(folder):
    audit = json.loads((folder / 'audit.json').read_text())
    assert audit['world_coverage'] == [384, 216]
    assert audit['original_actor_position'] == audit['candidate_actor_position']
    assert audit['original_camera_center'] == audit['candidate_camera_center']
    for name in ('original', 'candidate'):
        assert audit[name + '_image_size'] == [1536, 864]
        assert audit[name + '-native_image_size'] == [768, 432]
        assert audit[name + '_display_scale'] == 2
        assert audit[name + '-native_display_scale'] == 1
    board = Image.new('RGB', (624, 336), '#202832')
    draw = ImageDraw.Draw(board)
    text(draw, (12, 5), 'Intended gameplay zoom - unscaled screenshot crops', 14)
    for name, x in (('original', 16), ('candidate', 320)):
        image = Image.open(folder / (name + '.png')).convert('RGB')
        ax, ay = audit[name + '_machine_screen_anchor']
        crop = image.crop((round(ax - 144), round(ay - 180), round(ax + 144), round(ay + 76)))
        board.paste(crop, (x, 48))
        text(draw, (x, 29), 'Original runtime' if name == 'original' else 'Processed reference candidate')
        shutil.copyfile(folder / (name + '.png'), OUT / (name + '-gameplay.png'))
        shutil.copyfile(folder / (name + '-native.png'), OUT / (name + '-native-gameplay.png'))
    text(draw, (16, 312), 'Same 32 x 32 world bounds. Old pixels: 4px; candidate pixels: 2px.', 11)
    board.save(OUT / 'gameplay-comparison.png')
    shutil.copyfile(folder / 'audit.json', OUT / 'audit.json')
    print('PASS: native-window and intended-zoom captures; equal camera center and player position')


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--captures', type=Path)
    args = parser.parse_args()
    check_sources()
    OUT.mkdir(parents=True, exist_ok=True)
    native_board()
    if args.captures:
        capture_boards(args.captures)


if __name__ == '__main__':
    main()
