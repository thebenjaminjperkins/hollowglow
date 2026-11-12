#!/usr/bin/env python3
"""Normalize the small matched source kit into palette-shared runtime PNGs.

Source masters are preserved; no procedural texture/noise or geometric prop redraw.
Only extraction, masks, material normalization, pixel-aligned pose offsets, atlas
packing and seam preparation occur here. Pillow is an authoring dependency only.
"""
import argparse
import hashlib
import io
import json
from pathlib import Path

from PIL import Image, ImageEnhance, ImageFilter, ImageOps

from process_machinery_trial import background_mask

ROOT = Path(__file__).resolve().parents[2]
DEST = ROOT / 'assets/sanctuary'
SRC = DEST / 'source'
PALETTE = json.loads((ROOT / 'assets/trials/machinery/source/processing.json').read_text())['palette']


def png(image):
    buf = io.BytesIO()
    image.save(buf, format='PNG')
    return buf.getvalue()


def quantize(image):
    palette = Image.new('P', (1, 1))
    rgb = [tuple(bytes.fromhex(color)) for color in PALETTE]
    palette.putpalette([c for color in rgb * 4 for c in color])
    alpha = image.getchannel('A').point(lambda a: 255 if a >= 128 else 0)
    out = image.convert('RGB').quantize(palette=palette, dither=Image.Dither.NONE).convert('RGBA')
    out.putalpha(alpha)
    return out


def fit(image, canvas, limit, anchor):
    bounds = image.getbbox()
    if bounds is None:
        raise ValueError('Source extraction is empty')
    image = image.crop(bounds)
    ratio = min(limit[0] / image.width, limit[1] / image.height)
    size = (round(image.width * ratio), round(image.height * ratio))
    image = image.convert('RGBa').resize(size, Image.Resampling.LANCZOS).convert('RGBA')
    result = Image.new('RGBA', canvas)
    result.alpha_composite(image, (anchor[0] - size[0] // 2, anchor[1] - size[1]))
    return quantize(result)


def extract_cell(master, col, row, cols, rows):
    x0, y0 = round(master.width * col / cols), round(master.height * row / rows)
    x1, y1 = round(master.width * (col + 1) / cols), round(master.height * (row + 1) / rows)
    image = master.crop((x0 + 6, y0 + 6, x1 - 6, y1 - 6)).convert('RGBA')
    image.putalpha(image.getchannel('A').point(lambda a: 255 if a >= 176 else 0))
    return image


def make_outputs():
    outputs = {}
    manifest = {'native_pixels_per_world_unit': 2, 'palette': PALETTE, 'sprites': {}}
    furniture = Image.open(SRC / 'furniture.png')
    vegetation = Image.open(SRC / 'vegetation.png')
    reference = Image.open(ROOT / 'docs/screenshots/benchmark/reference.png').convert('RGB')

    specs = [
        ('workbench', furniture, 0, 0, 3, 2, (80, 72), (72, 64), (40, 66)),
        ('storage', furniture, 1, 0, 3, 2, (64, 56), (56, 48), (32, 50)),
        ('shelf', furniture, 2, 0, 3, 2, (72, 80), (64, 72), (36, 74)),
        ('garden', furniture, 0, 1, 3, 2, (72, 56), (64, 48), (36, 50)),
        ('lamp', furniture, 1, 1, 3, 2, (40, 76), (30, 68), (20, 70)),
        ('bedroll', furniture, 2, 1, 3, 2, (72, 56), (64, 48), (36, 50)),
        ('fern', vegetation, 0, 0, 3, 1, (64, 56), (48, 48), (32, 52)),
        ('vine', vegetation, 1, 0, 3, 1, (64, 80), (48, 72), (32, 76)),
        ('mushrooms', vegetation, 2, 0, 3, 1, (56, 44), (48, 36), (28, 40))
    ]
    fitted = {}
    for name, master, col, row, cols, rows, canvas, limit, anchor in specs:
        source = extract_cell(master, col, row, cols, rows)
        outputs[f'source/{name}-extracted.png'] = png(source)
        image = fit(source, canvas, limit, anchor)
        fitted[name] = image
        manifest['sprites'][name] = dict(file=name + '.png', size=canvas, anchor=anchor, columns=1)
        if name != 'fern':
            outputs[name + '.png'] = png(image)

    # Brush poses preserve the actual source fronds, translating rows by whole native pixels.
    fern = fitted['fern']
    sheet = Image.new('RGBA', (fern.width * 5, fern.height))
    for i, bend in enumerate((0, -2, -1, 1, 2)):
        frame = Image.new('RGBA', fern.size)
        for y in range(fern.height):
            shift = round(bend * max(0, 52 - y) / 52)
            frame.paste(fern.crop((0, y, fern.width, y + 1)), (shift, y))
        sheet.paste(frame, (i * fern.width, 0))
    outputs['fern.png'] = png(sheet)
    manifest['sprites']['fern']['columns'] = 5

    # Approved machinery is reused byte-for-byte; its old benchmark path is untouched.
    outputs['machine.png'] = (ROOT / 'assets/trials/machinery/candidate.png').read_bytes()
    manifest['sprites']['machine'] = dict(file='machine.png', size=[64, 64], anchor=[32, 58], columns=1)

    # A textured stone and a salvage offcut reuse existing reference regions, not new asset families.
    stone = fitted['fern'].crop((4, 38, 60, 56))
    outputs['stones.png'] = png(fit(stone, (56, 32), (48, 24), (28, 28)))
    manifest['sprites']['stones'] = dict(file='stones.png', size=[56, 32], anchor=[28, 28], columns=1)
    offcut = reference.crop((1098, 208, 1252, 377)).convert('RGBA')
    outputs['salvage.png'] = png(fit(offcut, (36, 36), (28, 28), (18, 32)))
    manifest['sprites']['salvage'] = dict(file='salvage.png', size=[36, 36], anchor=[18, 32], columns=1)

    # Four original reference views, registered to one foot anchor. Retain source detail.
    cuts = [(64, 100, 266, 480), (280, 100, 470, 480), (482, 100, 662, 480), (674, 100, 840, 480)]
    character_sheet = Image.new('RGBA', (240, 224))
    for row, bounds in enumerate(cuts):
        cut = reference.crop(bounds).convert('RGBA')
        cut.putalpha(background_mask(cut, {'crop': bounds, 'background_hole_seeds': []}))
        outputs[f'source/character-{row}-extracted.png'] = png(cut)
        base = fit(cut, (40, 56), (32, 48), (20, 52))
        upper = base.crop((0, 0, 40, 38))
        left_leg = base.crop((0, 35, 20, 56))
        right_leg = base.crop((20, 35, 40, 56))
        poses = [(0, 0, 0, 0), (0, 0, 0, 0), (-1, 0, 1, -1), (0, -1, 0, 0), (1, -1, -1, 0), (0, 0, 0, -1)]
        for column, (lx, ly, rx, ry) in enumerate(poses):
            frame = Image.new('RGBA', (40, 56))
            # Both boots must not float above the common ground line, especially
            # in side views where the rear boot is shorter in the reference.
            legs = Image.new('RGBA', (40, 56))
            legs.alpha_composite(left_leg, (lx, 35 + ly))
            legs.alpha_composite(right_leg, (20 + rx, 35 + ry))
            ground_offset = 52 - legs.getbbox()[3]
            frame.alpha_composite(legs, (0, ground_offset))
            frame.alpha_composite(upper, (0, -1 if column in (1, 3, 5) else 0))
            character_sheet.paste(frame, (column * 40, row * 56))
    outputs['character.png'] = png(character_sheet)

    # Terrain source studies become a small repeating atlas. They remain independent of room shape.
    terrain = Image.open(SRC / 'terrain.png').convert('RGB')
    h = terrain.width // 2
    samples = []
    for cx, cy in [(0, 0), (1, 0), (0, 1), (1, 1)]:
        patch = terrain.crop((cx * h + 8, cy * h + 8, (cx + 1) * h - 8, (cy + 1) * h - 8))
        if cy == 0:
            # Keep floor value changes quieter than the textured props.
            patch = ImageEnhance.Contrast(patch).enhance(0.55)
            patch = ImageEnhance.Color(patch).enhance(0.7)
        samples.append(patch.resize((64, 64), Image.Resampling.LANCZOS).convert('RGBA'))
    floors = []
    for patch in samples[:2]:
        # Mirror the full material study to make opposing boundaries meet. Cut the
        # resulting continuous patch into cells; never add dark borders to each tile.
        patch = patch.filter(ImageFilter.GaussianBlur(0.45))
        seamless = Image.new('RGBA', (128, 128))
        seamless.paste(patch, (0, 0))
        seamless.paste(ImageOps.mirror(patch), (64, 0))
        seamless.paste(ImageOps.flip(patch), (0, 64))
        seamless.paste(ImageOps.flip(ImageOps.mirror(patch)), (64, 64))
        seamless = quantize(seamless)
        for y in range(4):
            for x in range(4):
                floors.append(seamless.crop((x * 32, y * 32, x * 32 + 32, y * 32 + 32)))
    floor_atlas = Image.new('RGBA', (1024, 512))
    for variant, tile in enumerate(floors):
        for mask in range(16):
            image = tile.copy()
            for y in range(32):
                for x in range(32):
                    if ((mask & 1 and x + y < 8) or (mask & 2 and 31 - x + y < 8)
                        or (mask & 4 and 62 - x - y < 8) or (mask & 8 and x + 31 - y < 8)):
                        image.putpixel((x, y), (0, 0, 0, 0))
            index = variant * 16 + mask
            floor_atlas.paste(image, (index % 32 * 32, index // 32 * 32))
    outputs['floor.png'] = png(floor_atlas)
    wall_atlas = Image.new('RGBA', (128, 64))
    for kind, patch in enumerate(samples[2:]):
        for y in range(2):
            for x in range(2):
                index = kind * 4 + y * 2 + x
                wall_atlas.paste(quantize(patch.crop((x * 32, y * 32, x * 32 + 32, y * 32 + 32))), (index % 4 * 32, index // 4 * 32))
    outputs['wall.png'] = png(wall_atlas)
    manifest['terrain'] = dict(cell=32, floor_variants=32, corner_masks=16, wall_variants=8)
    manifest['sources'] = {name: hashlib.sha256((SRC / (name + '.png')).read_bytes()).hexdigest() for name in ('furniture', 'vegetation', 'terrain')}
    outputs['manifest.json'] = (json.dumps(manifest, indent=2) + '\n').encode()
    return outputs


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--check', action='store_true')
    args = parser.parse_args()
    for name, data in make_outputs().items():
        path = DEST / name
        if args.check:
            assert path.exists() and path.read_bytes() == data, f'Stale sanctuary asset: {name}'
        else:
            path.write_bytes(data)
    print('PASS: matched room sprites, directional sheet, reactive poses, and modular terrain atlas exports')


if __name__ == '__main__':
    main()
