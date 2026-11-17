# Cinder Maintenance Garage — M6b review assets

This is a modular location kit, not a background painting. Production sprites
come directly from processed reference artwork using the approved sanctuary
machine and M6a ruin as references. Existing art is unchanged.

The [2026-11-16 art-direction update](../../docs/ART_DIRECTION_UPDATE.md) sets the
next provisional target: more surviving materials and varied natural/Bloom
overgrowth, with less uniform corrosion. This completed M6b export is retained as
the comparison baseline; replacement assets require review of the small trial.

## Reproduce

```sh
python tools/art/build_garage.py
python tools/art/check_garage.py
```

Requires Pillow, as do the existing art tools. The three untouched image-generation
masters live in `source/`, excluded from Godot import by `.gdignore`. Exact prompts
and reference paths are recorded in [prompts.json](source/prompts.json); the tool
was built-in `image_gen`. Regeneration of a master is not deterministic; processing
the stored masters is deterministic and checked byte for byte.

The exporter extracts isolated cells, applies one aspect-preserving scale per
sprite family, aligns their bottom anchors, reduces to the existing shared
64-color palette without dithering and thresholds alpha to binary transparency.
No runtime polygon substitutes or per-object drawing recipes produce these assets.

## Runtime convention

- Two native pixels per world unit; sprites use scale 0.5 and nearest filtering.
  This is the existing character/machine convention, with unchanged camera coverage.
- `manifest.json` defines dimensions, family frame counts, foot anchors, palette
  and source hashes. The van has three 128×192 frames, anchor 64,184. Collision
  uses separate world-unit footprints in `data/locations/garage.json`.
- Cupboard, fuel cabinet and crate: closed, searched/nonempty, depleted frames.
  Rack: tool present/absent. Compressor: complete/stripped. Van: intact,
  partial, stripped. Common canvas/scale preserves state readability, although
  source-generated details are not perfectly registered hand animation.
- Floors are a 512×512 atlas containing four periodic 256×256 materials, sliced
  into 32×32 native tiles and assembled by `TileMapLayer`. Floor contrast is
  reduced before palette conversion. Walls are reusable PNG modules assembled
  from the same walkability template; Y sorting preserves object/actor depth.
- Ambient lighting remains adjustable through the existing world renderer.
  Sprites contain material shading and wear, not baked environmental light pools.

Future location templates can reuse floor periods, walls and object families.
Search contents and dismantling state belong to authoritative session objects,
not textures. Footprint-based navigation stays independent of art resolution.
Only this approved location's small kit is added; no general asset migration.
