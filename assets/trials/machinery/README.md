# Machinery-first reference-processing trial

This is a separate, approved experiment. Nothing in `assets/benchmark`, the existing
benchmark viewer, gameplay scripts/scenes/data, or `project.godot` was migrated or
replaced. This candidate is loaded only by `tools/MachineryTrial.tscn`.

## Run and compare

From the repository root, using the same environment as normal play:

```sh
godot --path . res://tools/MachineryTrial.tscn
```

For the isolated engine installed in this workspace, substitute:

```sh
/tmp/hollowlight-godot/Godot_v4.4.1-stable_mono_linux_x86_64/Godot_v4.4.1-stable_mono_linux.x86_64 --path . res://tools/MachineryTrial.tscn
```

The existing README has the temporary .NET/XDG environment variables if needed.
T toggles original/candidate; N toggles 1×/2× display; normal movement, interaction,
reset, help and debug keys continue working. The trial starts near the workshop.
The ordinary game and four-asset benchmark retain their previous launch commands.

## What was processed

Input: `docs/screenshots/benchmark/reference.png`, with its SHA256 recorded in
`source/processing.json`. No new image generation, reconstruction or redraw was used.

1. Crop the source rectangle `(842,8)–(1492,502)` containing the existing machine.
2. Mask the connected cool neutral backdrop, including a recorded background seed
   inside the exhaust loop. Enclosed grille shadows stay opaque. The full-resolution
   mask is saved separately and can be edited without altering the reference.
3. Trim the transparent margins; fit the silhouette proportionally into 60×47 pixels.
   Resample premultiplied-alpha colors with Lanczos, then threshold alpha to binary
   for crisp pixel edges. There is no dithering or invented surface noise.
4. Quantize to a provisional 64-swatch shared palette: preserve the old 32 swatches
   and add 32 learned from the extracted machine. This candidate uses 47 colors.
   This is not a finalized palette for future vegetation/terrain/characters.
5. Place the result in a 64×64 indexed PNG, with foot anchor `(32,58)`. All runtime
   sampling is nearest-neighbor. The PNG is imported losslessly without mipmaps.

`source/crop.png`, `mask.png`, `extracted.png`, `recipe.json` and `processing.json`
retain the actual processing evidence. The reference source image stays unchanged.
The direct crop retains the exhaust loop, side casing, vents, chipped paint,
corrosion, foundation and attached growth instead of replacing them with simpler shapes.

## Resolution without gameplay changes

| Measurement | Existing game | Separate trial |
| --- | --- | --- |
| Logical render grid | 384×216 | 768×432 |
| Camera zoom | 1 | 2 |
| World coverage | 384×216 units | 384×216 units |
| Default display | 1152×648 (3×) | 1536×864 (2×) |
| Original source frame | 32×32 | Same original 32×32 |
| Candidate source frame | — | 64×64 |
| Machine world draw bounds | (-16,-29), 32×32 | Identical for both images |
| Machine solid footprint | 26×9 | Identical original collider |
| Player feet collider | 8×6 | Identical original collider |
| Movement / interaction range | 80 units/s / 22 units | Identical |

The trial changes only its own Window presentation settings in memory, doubles camera
zoom, and scales the existing HUD layout by two. It never saves project settings or
production resources. The candidate's 64 texels occupy the same 32 world units; at
2× display each candidate texel occupies 2 screen pixels. Each old texel occupies
4 screen pixels in that same view. N switches to a native 768×432 window, where
candidate texels occupy exactly 1 screen pixel, with world coverage unchanged.

## Evidence and reproduction

Reviewed comparisons are in `docs/screenshots/machinery-trial/`:

- `native-comparison.png`: original 32×32 and candidate 64×64 at one image pixel
  per source pixel, aligned to the same foot baseline. No enlargement.
- `gameplay-comparison.png`: unscaled crops from actual gameplay captures, using
  equal camera/player positions and the intended 2× display. Both draw 32×32 world units.
- `original-gameplay.png`, `candidate-gameplay.png`: full 1536×864 viewport captures.
- `original-native-gameplay.png`, `candidate-native-gameplay.png`: full 768×432 captures.
- `audit.json`: measured world coverage, camera center, player position, display
  sizes and assertions about movement, diagonals, colliders, interaction range and counts.

Processing uses Python and Pillow (12.3.0 was used). These are authoring tools, not
game runtime dependencies. Reproduce and verify the candidate with:

```sh
python3 tools/art/process_machinery_trial.py
python3 tools/art/process_machinery_trial.py --check
python3 tools/art/review_machinery_trial.py
```

For new graphical captures and automated measurements:

```sh
godot --path . res://tools/MachineryTrial.tscn -- --capture-machinery
python3 tools/art/review_machinery_trial.py --captures /path/to/user-data/machinery-trial
```

Godot writes captures to `user://machinery-trial`. In this session that resolves to
`/tmp/hollowlight-data/godot/app_userdata/Hollowlight/machinery-trial`.
The capture run temporarily uses scripted actor input, verifies horizontal and
diagonal travel, captures original/candidate at both display scales, then exits.
The review script checks source hashes, palette/alpha constraints, camera positions,
image dimensions, and copies full captures. Comparison crops are not resized.

`source/baseline_hashes.json` protects 61 pre-existing benchmark, code, scene, data,
project and reference files. All still match byte-for-byte. Build, deterministic
logic tests and original live gameplay smoke tests for both layouts passed.

## Remaining compromises and review gate

- The reference has much more information than a 64×64 frame. Fine corrosion and
  leaf detail merge; very thin features can disappear after alpha thresholding.
- Its wide, low silhouette is retained. Inside the unchanged 32×32 world bounds it
  reads shorter than the old upright machine. No aspect stretching or larger collider
  was introduced to hide that difference.
- Baked form lighting and internal ambient occlusion remain in the reference pixels.
  They cannot be independently relit; existing local glow can brighten them further.
- The illustration's frontal/three-quarter perspective is retained. More directions
  or a changed viewing angle would require additional asset work, not automatic cropping.
- The machine now has finer pixel density than the surrounding unmigrated assets.
  That mixture is deliberate comparison evidence, not a completed world-art migration.
- The mask and palette are reproducible trial outputs, not a universal background
  extractor or final game-wide art palette. Pillow versions may affect quantization.

Stop for owner review of silhouette fidelity, material/wear readability, native pixel
quality, gameplay-scale size and integration. Do not migrate or expand other assets.
