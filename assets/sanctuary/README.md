# Approved sanctuary art standard

This is the approved provisional art direction, integrated into normal play as one small, playable environment using the approved machinery
candidate as its material/detail target. It includes natural cave walls, quiet
textured ground, a workshop, shared chest, garden beds, foliage, lamps, bedding and
a reference-derived character. No survival system or procedural world was added.

## Open the room

From the repository root with the usual Godot .NET environment:

```sh
godot --path .
```

With this workspace's temporary engine installation:

```sh
/tmp/hollowlight-godot/Godot_v4.4.1-stable_mono_linux_x86_64/Godot_v4.4.1-stable_mono_linux.x86_64 --path .
```

L switches between neutral material inspection and the adjustable lamp/ambient
lighting. Ambient tint, lamp energy and lamp radius scale are editor-exposed on
the room scene root. WASD/arrows, E, R, H and F1/F2/F3 keep their existing functions. The room
starts at the original refuge home position. The historical scene, four-asset benchmark and machinery
trial remain available through their previous scene paths.

## Production workflow and consistency rules

1. **Use a common quality reference.** The approved machine is copied byte-for-byte
   from `assets/trials/machinery/candidate.png`. Four character views are extracted
   from the original reference. A small matched furniture sheet, vegetation sheet,
   and terrain material sheet were produced with the built-in imagegen tool using
   those same references. These are individual sources, not a whole-room painting.
2. **Keep source masters and recipes.** `source/` contains the three unchanged
   generated masters, exact prompts, extracted props/character views and preservation
   hashes. `tools/art/build_sanctuary.py` records the extraction, mask thresholds,
   dimensions, anchors, palette reduction, pose registration and atlas construction.
   Generated sources stay out of Godot imports; runtime PNGs stay alongside them.
3. **Normalize native density.** Every native pixel represents half a world unit.
   Terrain cells are 32×32 native pixels / 16×16 world units. The character uses
   40×56 padded cells, roughly 32×48 maximum visible pixels, with one fixed foot
   anchor and four facings. Props use appropriately sized cells listed in
   `manifest.json`; all render at exactly 0.5 world scale, not arbitrary per-asset zoom.
4. **Use one palette and material hierarchy.** Every opaque runtime PNG pixel is
   from the same 64-color palette. Resampling uses premultiplied alpha, no dithering,
   and binary final alpha. Texture should follow wood grain, metal wear, stone
   strata and leaf overlap. Quiet, lower-contrast floor texture supports stronger
   prop silhouettes; do not add random noise or bright detail everywhere.
5. **Keep room lighting adjustable.** Sources have form shading, while warm pools,
   cool ambient treatment and subtle mushroom light are actual Godot lighting.
   L toggles these for comparison. Soft contact shadows are separate effects; no
   warm light pools are painted into a static room image. Some form/occlusion
   shading remains inherent in generated/reference sources; this is not a normal-map
   physically relightable material system.
6. **Review in context before expanding.** Verify exports, palette/density/anchors,
   tile joins and collision, then inspect native and actual gameplay views. Any
   future asset should pass this same room test before joining the kit. Keep the
   camera coverage fixed during comparison and inspect motion, not only still crops.

The character has registered reference cutout poses with small pixel-aligned leg
and torso offsets. Boots now retain one ground baseline through every pose.
Cadence follows actual post-collision travel (8 frames/second at full speed);
walking stops at walls and diagonal ties keep a stable axis. It preserves four-facing walking, idle and diagonal travel;
it is not a newly illustrated full anatomical walk cycle. Fern brush poses likewise
reuse the actual fronds with small native-pixel offsets. Both can be refined after
owner review without touching player simulation.

## TileMapLayer evaluation and implementation

Adopted Godot's `TileMapLayer` for terrain. Separate ceiling, floor and exposed-wall
layers use `TileSetAtlasSource` textures. Tiles are assembled by occupancy/adjacency
rules from the existing authored layout; they are not individually drawn polygons
or a single background. Godot handles the tile rendering/batching. See the official
[TileMapLayer documentation](https://docs.godotengine.org/en/4.4/classes/class_tilemaplayer.html)
and [atlas source documentation](https://docs.godotengine.org/en/4.4/classes/class_tilesetatlassource.html).

Two quiet source-floor studies are prepared as periodic 4×4 material patches and
split into 32-pixel cells. Opposing patch boundaries match. Sixteen corner masks
support the existing four-pixel world chamfers. These derived combinations are
atlas entries, not hundreds of separately generated assets. Four wall faces and
four cap modules come from the material sheet. Placement uses global coordinate
phase so terrain assembly preserves joins; do not randomize material subtiles
independently. This supports changing room occupancy without repainting the scene.

The current implementation uses explicit adjacency selection, not editor terrain
peering/autoconnect metadata. Tilesets are initialized from the PNG atlases at room
startup. A future generator can provide occupancy/placements, but world generation
and a broad terrain library are outside this benchmark.

TileMap collision/navigation/occlusion are disabled. The original wall, chamfer and
prop collision bodies remain authoritative; this avoids duplicate physics and
preserves the existing collision behavior. The same kit was tested on both existing
authored layouts as a reuse check, without creating a new game region.

## Runtime composition

`SanctuaryRoom` mounts the existing `Threshold` simulation and attaches the approved
`RoomPresentation`. Original renderers are hidden; PNG sprites and tile layers
replace visible geometric placeholders. Old render nodes remain hidden for
non-regression/comparison, not as visible room artwork. Purely decorative old cable,
root-line and rug marks are omitted to keep the ground readable.

All original solid footprints, interaction distances, input and recovery remain
in place. M2a now supplies typed finite nodes and shared-chest transfers; runtime
PNG artwork, density and terrain assembly are unchanged. The character's feet stay 8×6, speed stays 80 units/second and
interaction range stays 22. New fern triggers are nonblocking. Existing props and
interactables retain their footprints and positions; visual roots and cloth may
overhang those feet. Depth ordering uses the existing foot-based Y sorting.

The window uses a 768×432 logical grid, 2× camera zoom, and default 2× display
(1536×864), preserving 384×216 world coverage. The default entry now mounts this presentation; earlier scenes remain available. The original surface loop remains functional and reuses this kit;
the art benchmark being evaluated is the refuge, not a newly authored overworld.

## Build, checks and captures

```sh
python3 tools/art/build_sanctuary.py --check
python3 tools/art/check_sanctuary.py
dotnet build Hollowlight.csproj
dotnet run --project tests/Hollowlight.Core.Tests.csproj
godot --headless --fixed-fps 60 --path . res://tools/ThresholdSmoke.tscn -- --playable-refuge
godot --headless --fixed-fps 60 --path . res://tools/ThresholdSmoke.tscn -- --playable-refuge --sanctuary-variant
```

To regenerate after editing sources, omit `--check` and let Godot reimport PNGs.
Pillow is an authoring dependency (12.3.0 was used); it is not required to run the game.

Graphical, viewport-only capture run:

```sh
godot --fixed-fps 60 --path . -- --capture-room
python3 tools/art/check_sanctuary.py --captures /path/to/user-data/sanctuary-room
```

Eight-direction actual walking review (scripted input, fixed frame rate):

```sh
godot --fixed-fps 60 --path . res://tools/RefugeWalkReview.tscn
python3 tools/art/check_sanctuary.py --walk-captures /path/to/user-data/refuge-walk
```

The tool captures 1536×864 gameplay/neutral/storage/home views and a 768×432 native
view, then exits. Curated actual captures are in `docs/screenshots/sanctuary-room`.
The checker validates shared colors/alpha, anchors, periodic floor joins, chamfer
masks, approved-machine identity and preserved legacy hashes, explicitly recorded M2a runtime changes and the exact approved main-scene change.

Owner playtest: walk around the workshop, look behind/in front of furniture, brush
ferns, compare L lighting, collect all three salvage pieces outside and deposit
them in the new chest. Test R while carrying and after depositing. Judge cohesion,
room mood, scale and ground readability at normal gameplay zoom. Future assets must match this approved quality standard. Stop for review at each
meaningful gameplay milestone.

## Source provenance

New generated masters: `source/furniture.png`, `source/vegetation.png`,
`source/terrain.png`. Tool mode: built-in `image_gen`; exact prompt set:
`source/prompts.json`. Both references were the project's existing machine extraction
and original four-asset reference image. These masters were cropped/masked and
normalized into the runtime kit; no external asset pack or runtime service was added.
The machine remains the approved existing candidate. Character and salvage source
pixels are from the preserved original reference. The resulting room is assembled
from reusable texture assets and live objects, not painted into the generated masters.


## Inventory icon reuse

The approved inventory UX uses native AtlasTexture regions of `salvage.png` and
`mushrooms.png`, plus a backpack region from `character.png` for the minimal HUD.
`data/inventory_ui.json` records exact source rectangles and grid presentation.
Icons draw at half the base interface unit (one source pixel per native render
pixel after the HUD's 2× transform), with nearest filtering and integer display.
No new raster artwork, palette or world textures were introduced. Godot slot frames
and focus outlines use the existing UI colors. PNG sources and resource data remain
unchanged. UI capacities are owned by SharedSupplies, not icon/grid dimensions.
