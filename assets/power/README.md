# M4a Fuel art

`fuel_cache.png` has available/depleted 40×40 frames, anchor (20,36), 2 native
pixels/world unit and nearest display. `fuel_icon.png` is a 24×24 item icon.
The shared sanctuary palette and live ambient/point lighting remain in use.

The built-in imagegen tool, following the imagegen skill, produced the original
matched transparent canister family using `assets/sanctuary/machine.png` as a style
reference. Preserve `source/fuel.png` and the full prompt `source/prompt.txt`.
`tools/art/build_power.py` records extraction, binary-alpha threshold, shared family
scale, palette quantization and packing. The manifest records anchors and source
hash. No existing art is replaced; generator, workbench, lamps and communications
shelf are reused with live status diodes/lights. No large asset library was generated.

```sh
python3 tools/art/build_power.py --check
python3 tools/art/check_power.py
```
