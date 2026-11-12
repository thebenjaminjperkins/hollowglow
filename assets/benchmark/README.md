# Four-asset visual standard

This is the approved bounded benchmark: player, salvaged generator, reactive fern,
and modular cave wall. Other assets retain the previous presentation pending review.
The PNGs here are real runtime spritesheets, replacing native rectangle drawing for
these four elements. Layouts, collision and resource rules remain independent.

## Resolution and sheets

One source pixel equals one logical game pixel. Never pre-enlarge a sprite or mix
pixel densities. Keep the 384×216 viewport, integer scaling (default 3×), nearest
sampling and pixel-snapped presentation. Simulation positions remain continuous.

| Sheet | Cell | Arrangement | Anchor per cell |
| --- | --- | --- | --- |
| character.png | 20×28 padded; roughly 16×24 visible | 6 columns × 4 rows | (10,26), feet |
| machine.png | 32×32 | One sprite | (16,29), feet |
| fern.png | 32×28 padded; roughly 24×24 visible | 5 columns | (16,26), rooted base |
| wall.png | 16×16 | 4 columns × 2 rows | (0,0), tile corner |

Character rows are south, north, west, east. Columns are idle0, idle1, then four
walk frames. Walking uses 8 frames/second; idle alternates slowly. Each direction
has authored head, coat/backpack and leg pixels, with the same proportions and foot
anchor. Keep grounded contact poses and alternating legs; do not stretch limbs.
Diagonals retain full movement and normalized speed. Presentation keeps its previous
axis at ties and follows the sign of travel, avoiding facing jitter.

Fern columns are rest, strong left, gentle left, gentle right, strong right. Its
nonblocking Area2D reacts only to a moving character entering. A short damped response
selects those authored poses; no sprite rotation, stretch, movement force or slowdown.

Wall cells 0–2 are top-rock variations, 3–5 are exposed faces, 6/7 are side modules.
Build connected stone masses with coherent strata. Keep every tile reusable and
align the visible ground boundary with existing collision; never bake a room image.

## Palette, materials and lighting

`source/palette.json` defines exactly 32 opaque base swatches plus transparency.
Characters, machinery, foliage and stone share those swatches. Do not add similar
colors independently. Most materials should need 3–5 shades selected from the palette.

- Use connected clusters to describe broad planes before adding small details.
- Metal: recessed dark panels, a restrained brighter bevel, and a few connected
  exposed-metal/rust patches where edges or joints would wear.
- Cloth/leather: broad muted folds, seam accents, and small contact shadows.
- Foliage: asymmetric overlapping fronds, darker undergrowth and rooted stems.
- Stone: broad irregular planes, stepped strata and sparse connected fissures;
  avoid repeated outlined gems, enclosed dark holes and scattered noise.
- Reserve isolated pixels for meaningful highlights, fasteners or facial features.
  Check silhouettes and value separation before adding texture.

Paint neutral form light from the upper left. Use selective outlines/contact shadows,
not a uniform heavy border. Validate under neutral light first. Existing unshadowed
amber/teal PointLight2D pools add atmosphere; avoid bleaching the sprite's material
shades. Lit pixels can have intermediate colors; the 32-color constraint applies to
the source sheets, not the final lighting composite. No new lighting/power mechanics.

## Editing and exporting

`source/*.json` are editable palette-indexed pixel layers, with named frames/layers,
offsets and explicit rows of pixels. Symbols 0–9/A–V map to the shared palette;
`.` is transparent. Edit clusters directly in the layer rows. Sources are ignored
by Godot's importer but retained in Git. They are not runtime drawing instructions.

```sh
python3 tools/art/export_assets.py
python3 tools/art/export_assets.py --check
```

The exporter copies the authored pixels into indexed lossless PNG sheets. It does
not synthesize texture, use noise, resize concept art, or interpolate pixels. It
checks palette membership, frame bounds and byte-for-byte source/export agreement.
Keep PNGs, their import descriptors and editable source together in review. Reimport
in Godot after changing PNGs. Use lossless import, no mipmaps, and nearest sampling.

## Review

```sh
godot --path . res://tools/ArtBenchmark.tscn
```

The top row is unlit; the lower row uses an actual amber light. SPACE brushes both
ferns; 1 switches walking/idle. Compare native pixels and the 3× view. Then play
both sanctuary layouts, walk through ferns, and complete the existing salvage loop.
The benchmark should remain readable without texture becoming clutter. Artist/owner
quality acceptance is separate from automated checks; stop for review before expansion.

## Provenance

Production sheets are original palette-indexed pixel layers authored for this
repository and exported by the checked-in script. They are independent of the
AI-generated reference image and were not cropped/downscaled from it. No external
asset packs or paid authoring dependencies were added.

The built-in imagegen tool produced a visual reference, saved unchanged at
`docs/screenshots/benchmark/reference.png`. It is concept reference, not a gameplay
screenshot or a runtime asset. Its exact prompt is preserved in
`docs/benchmark_reference_prompt.txt`. Actual runtime captures are named separately.
