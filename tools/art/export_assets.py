#!/usr/bin/env python3
"""Export the authored palette-indexed layers to lossless PNG sheets. No image packages.

This copies explicit source pixels; it does not invent texture, resize an image,
or use random noise. Edit the source layer rows, then export and review in game.
"""
import argparse
import json
from pathlib import Path
import struct
import zlib

ROOT = Path(__file__).resolve().parents[2]
DEST = ROOT / 'assets' / 'benchmark'


def chunk(kind, payload):
    return struct.pack('>I', len(payload)) + kind + payload + struct.pack('>I', zlib.crc32(kind + payload))


def export(source, palette):
    width, height = source['size']
    columns = source['columns']
    frames = source['frames']
    sheet_width = width * columns
    sheet_height = height * ((len(frames) + columns - 1) // columns)
    canvas = [bytearray(sheet_width) for _ in range(sheet_height)]
    indices = {symbol: i + 1 for i, symbol in enumerate(palette)}
    indices['.'] = 0
    for index, frame in enumerate(frames):
        left, top = (index % columns) * width, (index // columns) * height
        for layer in frame['layers']:
            for y, row in enumerate(layer['pixels']):
                for x, symbol in enumerate(row):
                    if symbol not in indices:
                        raise ValueError(f"Unknown palette symbol {symbol!r}: {frame['name']}")
                    if symbol == '.':
                        continue
                    px, py = layer['x'] + x, layer['y'] + y
                    if not (0 <= px < width and 0 <= py < height):
                        raise ValueError(f"Clipped pixel in {frame['name']}: {px},{py}")
                    canvas[top + py][left + px] = indices[symbol]
    colors = b'\x00\x00\x00' + b''.join(bytes.fromhex(color) for color in palette.values())
    raw = b''.join(b'\x00' + bytes(row) for row in canvas)
    return (b'\x89PNG\r\n\x1a\n'
            + chunk(b'IHDR', struct.pack('>IIBBBBB', sheet_width, sheet_height, 8, 3, 0, 0, 0))
            + chunk(b'PLTE', colors)
            + chunk(b'tRNS', b'\x00' + bytes([255] * len(palette)))
            + chunk(b'IDAT', zlib.compress(raw, 9)) + chunk(b'IEND', b''))


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--check', action='store_true', help='Fail if committed exports differ from source pixels')
    args = parser.parse_args()
    palette = json.loads((DEST / 'source' / 'palette.json').read_text())['colors']
    assert len(palette) == 32 and len(set(palette.values())) == 32
    for name in ('character', 'machine', 'fern', 'wall'):
        source = json.loads((DEST / 'source' / (name + '.json')).read_text())
        image = export(source, palette)
        target = DEST / (name + '.png')
        if args.check:
            assert target.is_file() and target.read_bytes() == image, f'Stale export: {target}'
        else:
            target.write_bytes(image)
        print(f"PASS: {name}: {len(source['frames'])} frames, shared 32-color palette, no clipped pixels")


if __name__ == '__main__':
    main()
