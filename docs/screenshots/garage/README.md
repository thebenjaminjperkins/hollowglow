# Cinder garage — runtime review captures

Captured from Godot 4.4.1 .NET, seed 1701. The PNGs are unresized 1536×864 window
captures at the normal gameplay camera zoom (2). Runtime art retains the shared
two-native-pixels-per-world-unit convention. Review positioning uses a developer
shortcut and is separate from the physically walked expedition measurements.

- [Forecourt and road apron](forecourt.png)
- [Office search and visual loot slots](office-search.png)
- [Workshop, tool rack and service bay](workshop.png)
- [Intact van](van-intact.png)
- [Preserve/dismantle inspection](van-inspection.png)
- [Partially dismantled van](van-partial.png)
- [Stripped van](van-stripped.png)
- [Explored terrain memory, dynamic objects hidden outside sight](explored-memory.png)
- [Wider composition overview](overview-review-zoom.png): **review zoom 1.25**, not
  normal gameplay scale; included only to show the building/apron relationship.

The container spacing issue found in the initial visual pass was corrected before
these captures. Current fog has hard tile edges and large objects use foot-anchor
visibility. State-frame source details have small registration differences;
further art polish remains a review decision. No composited background or painted
mockup replaces runtime rendering.

To recapture with a graphical editor:

```sh
godot --fixed-fps 60 --path . res://tools/GarageSmoke.tscn -- --capture-garage
```

Images are written to `user://garage/`. Normal measured runs have no capture flag.
