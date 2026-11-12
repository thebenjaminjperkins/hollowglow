# Farming sprite kit

One bounded addition to the approved sanctuary art standard: Cave Potato and Glowcap
seedling/growing/mature stages, an empty salvaged planter, two seed icons and Food.
The original sanctuary PNGs and source masters remain unchanged.

The built-in `image_gen` tool produced `source/kit.png` using the approved machine,
garden and mushrooms as style references. The exact prompt is `source/prompt.txt`;
`source/recipe.json` records references, source SHA256, cell layout and pixel metrics.
These are reusable sprites, not a painted scene/background.

`tools/art/build_farming.py` extracts the transparent master cells, fits native
frames, quantizes to the existing 64-color palette without dithering and exports
binary alpha. Native density remains two pixels/world unit. The crop sheet has
32×40 frames with a fixed root anchor (16,38); stage budgets are 16×18, 20×25 and
28×34. The 72×56 bed frame retains the existing garden anchor (36,50). Icons are
24×24. All runtime sprites use nearest filtering and the existing integer zoom.

`manifest.json` maps crop IDs to sprite rows independently of simulation definition
order. `GrowingPlotArt` reads plot snapshots; it never advances growth, consumes
seeds or owns inventory. The crop root sits on the soil above the accessible front
interaction point. A restrained mature Glowcap pulse changes material brightness;
lamp/ambient lighting remains adjustable. No power system is implied.

```sh
python3 tools/art/build_farming.py --check
python3 tools/art/check_farming.py
```

Omit `--check` to regenerate from the preserved master and reimport in Godot. Inspect
native sheet and actual gameplay views before further assets or animation changes.
Actual reviewed captures are in `docs/screenshots/farming`.
