#!/usr/bin/env python3
"""Approved deterministic extraction of existing reference pixels; no generation/redraw.

Requires Pillow. Source image and existing runtime assets are read-only inputs.
The editable full-resolution mask, crop, extracted pixels, native candidate and
palette/metrics are saved separately from the working benchmark.
"""
import argparse
from collections import deque
import hashlib
import io
import json
from pathlib import Path

from PIL import Image, __version__ as pillow_version

ROOT = Path(__file__).resolve().parents[2]
TRIAL = ROOT / 'assets/trials/machinery'


def png(image):
    stream = io.BytesIO()
    image.save(stream, format='PNG', optimize=False)
    return stream.getvalue()


def background_mask(crop, recipe):
    width, height = crop.size
    pixels = crop.load()
    outside = set()
    queue = deque()
    for x in range(width):
        queue.append((x, 0))
        queue.append((x, height - 1))
    for y in range(height):
        queue.append((0, y))
        queue.append((width - 1, y))
    for x, y in recipe['background_hole_seeds']:
        queue.append((x - recipe['crop'][0], y - recipe['crop'][1]))
    while queue:
        x, y = queue.popleft()
        if (x, y) in outside or not (0 <= x < width and 0 <= y < height):
            continue
        r, g, b = pixels[x, y][:3]
        # Cool, near-neutral backdrop. Warm dark metal and enclosed grille shadows
        # stay opaque; the exhaust opening is an explicitly recorded background seed.
        if not (max(r, g, b) < 83 and b >= g >= r and 1 <= b - r <= 20):
            continue
        outside.add((x, y))
        queue.extend(((x - 1, y), (x + 1, y), (x, y - 1), (x, y + 1)))
    mask = Image.new('L', crop.size, 255)
    for x, y in outside:
        mask.putpixel((x, y), 0)
    return mask


def build():
    recipe = json.loads((TRIAL / 'source/recipe.json').read_text())
    original = Image.open(ROOT / recipe['source']).convert('RGB')
    crop = original.crop(tuple(recipe['crop']))
    mask_path = TRIAL / 'source/mask.png'
    mask = Image.open(mask_path).convert('L') if mask_path.exists() else background_mask(crop, recipe)
    assert mask.size == crop.size, 'Mask and recorded crop must match'
    extracted = crop.convert('RGBA')
    extracted.putalpha(mask)
    bounds = extracted.getbbox()
    cutout = extracted.crop(bounds)
    limit_w, limit_h = recipe['content_limit']
    ratio = min(limit_w / cutout.width, limit_h / cutout.height)
    size = (round(cutout.width * ratio), round(cutout.height * ratio))
    # Premultiplied resampling avoids mixing transparent background RGB into edges.
    small = cutout.convert('RGBa').resize(size, Image.Resampling.LANCZOS).convert('RGBA')
    alpha = small.getchannel('A').point(lambda value: 255 if value >= recipe['alpha_threshold'] else 0)
    small.putalpha(alpha)
    pixels = small.get_flattened_data() if hasattr(small, 'get_flattened_data') else small.getdata()
    opaque_pixels = [rgb[:3] for rgb in pixels if rgb[3]]
    strip = Image.new('RGB', (len(opaque_pixels), 1))
    strip.putdata(opaque_pixels)
    original_palette = list(json.loads((ROOT / recipe['preserved_palette']).read_text())['colors'].values())
    learned = strip.quantize(colors=recipe['palette_size'] - len(original_palette), method=Image.Quantize.MEDIANCUT)
    learned_rgb = learned.getpalette()
    palette = [tuple(bytes.fromhex(color)) for color in original_palette]
    for index in range(recipe['palette_size'] - len(palette)):
        palette.append(tuple(learned_rgb[index * 3:index * 3 + 3]))
    palette_image = Image.new('P', (1, 1))
    palette_image.putpalette([value for color in palette * 4 for value in color])
    indexed = small.convert('RGB').quantize(palette=palette_image, dither=Image.Dither.NONE)
    canvas = Image.new('P', tuple(recipe['canvas']), 0)
    colors = [(0, 0, 0)] + palette
    canvas.putpalette([value for color in colors for value in color])
    offset = (recipe['anchor'][0] - size[0] // 2, recipe['anchor'][1] - size[1])
    source_indices = indexed.load()
    alpha_pixels = alpha.load()
    for y in range(size[1]):
        for x in range(size[0]):
            if alpha_pixels[x, y]:
                canvas.putpixel((offset[0] + x, offset[1] + y), source_indices[x, y] % 64 + 1)
    canvas.info['transparency'] = 0
    indices = canvas.get_flattened_data() if hasattr(canvas, 'get_flattened_data') else canvas.getdata()
    actual = len(set(indices) - {0})
    metadata = dict(
        source=recipe['source'], source_sha256=hashlib.sha256((ROOT / recipe['source']).read_bytes()).hexdigest(),
        crop=recipe['crop'], extracted_bounds=list(bounds), content_size=list(size), content_offset=list(offset),
        canvas=recipe['canvas'], anchor=recipe['anchor'], palette_size=len(palette), used_colors=actual,
        palette=['%02x%02x%02x' % color for color in palette], dither=False,
        resample='premultiplied-alpha Lanczos followed by binary alpha threshold',
        alpha_threshold=recipe['alpha_threshold'], pillow_version=pillow_version,
        provenance='Direct reference crop pixels, masked/resampled/quantized; no redraw or image generation')
    return {
        'source/crop.png': png(crop), 'source/mask.png': png(mask),
        'source/extracted.png': png(extracted), 'candidate.png': png(canvas),
        'source/processing.json': (json.dumps(metadata, indent=2) + '\n').encode()
    }


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--check', action='store_true')
    args = parser.parse_args()
    for relative, data in build().items():
        target = TRIAL / relative
        if args.check:
            assert target.exists() and target.read_bytes() == data, f'Stale processing output: {relative}'
        else:
            target.write_bytes(data)
    print('PASS: reproducible reference extraction, native 64x64 candidate and provisional 64-color palette')


if __name__ == '__main__':
    main()
