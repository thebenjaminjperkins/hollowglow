# Mountain-region modular art

M6a reuses the approved sanctuary sprites, character animation, crops, wall/floor
atlases, item icons and adjustable lights unchanged. New surface materials and a
small set of reusable environment props follow the same **2 native pixels/world
unit**, **32-pixel tile / 16-world-unit cell**, shared **64-color palette**, binary
transparency and nearest-neighbor rendering standard. Camera coverage stays
384×216 world units in normal play. One native pixel remains two screen pixels
at the default 1536×864 window.

The built-in `image_gen` tool produced two source sheets, using approved sanctuary
terrain and machinery as references. Exact prompts and reference paths are in
[source/prompts.json](source/prompts.json); original masters are retained beside it
and excluded from engine import with `.gdignore`. These are material/prop sheets,
not painted maps. No API key or generation service is required to rebuild exports.

`tools/art/build_region.py` extracts isolated props using their alpha, removes the
semitransparent halo, normalizes dimensions/anchors and reduces to the existing
palette without dithering. Terrain samples are quieted, reduced to native density,
periodized with mirrored edges, then exported into six material sets × 64 spatial
phases × 16 edge masks. Deck texture is extracted *after* native-density
normalization; a small support detail is never enlarged into terrain. Coherent
edge erosion breaks up transitions without random speckle or dark grid outlines.
`manifest.json` records dimensions, anchors, palette and source hashes.

```sh
python3 tools/art/build_region.py --check
python3 tools/art/check_region.py
python3 tools/art/check_sanctuary.py
```

Godot `TileMapLayer` assembles reusable PNG atlas cells from generated ground and
adjacency, while separate sprite nodes render tree, ruin, sign and cave-mouth
modules. A cave-mouth sprite identifies a threshold, without implementing a door.
Collision uses the ground mask and small prop footprints; the foliage canopy
never becomes a huge rectangular obstacle. Existing ferns retain their movement
reaction. No light pools or day/night illumination are painted into the new map;
ambient tint and existing point lights remain independently adjustable with L.

Future assets should preserve material cluster sizes against the character and
machine at actual gameplay zoom, use explicit foot anchors and collision
footprints, keep ground quieter than props, and export modular pieces through a
repeatable manifest. Source resolution alone does not establish pixel density.
The current mirrored material periods and repeated ruin pieces are visible in
some views; further variants should be a reviewed art improvement, not new
resource content or a static scene background.
