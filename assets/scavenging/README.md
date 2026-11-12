# Food scavenging sprite kit

Two reusable PNG families follow Hollowlight's approved sanctuary standard:

- `food_container.png`: available provisions / empty container.
- `food_plant.png`: edible pods/leaves / picked stems and roots.

Each sheet has two 40×40 native frames with anchor (20,36), drawn at 0.5 world
scale, nearest filtering and the shared sanctuary 64-swatch palette. Sources are
small ground-level salvage with no additional blocking body. Live ambient/point
lighting remains adjustable; sprites contain only material-form shading. Source
and depleted states use one scale per family, preserving relative root/box size.
A picked plant is a finite source, not a new crop or regrowth system.

The **built-in imagegen tool**, following the imagegen skill, produced original
matched transparent source families using existing storage/mushroom PNGs as style
references. Originals remain in `source/container.png` and `source/plant.png`;
complete prompts are `source/container-prompt.txt` and `source/plant-prompt.txt`.
No external game's art or new palette was used.

`tools/art/build_scavenging.py` records the reproducible processing recipe:
extract two equal source cells, threshold alpha at 176, crop transparent padding,
apply one shared family scale within a 32×30 native silhouette budget, register
at the common anchor, quantize to the approved palette without dithering, and pack.
The manifest records source hashes, anchors, frame sizes and state order. Runtime
loads the resulting PNGs directly; no geometry/noise draws these props.

```sh
python3 tools/art/build_scavenging.py --check
python3 tools/art/check_scavenging.py
```

Actual gameplay-scale captures are in `docs/screenshots/scavenging`. Earlier
benchmark, sanctuary and farming images remain unchanged.
