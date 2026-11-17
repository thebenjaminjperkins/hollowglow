# M6c follow-up — native gameplay captures

Before/after pairs below use the preserved five-chamber garage profile, seed 1701,
identical actor/camera positions and clock times. All paired PNGs are 1536×864.
New hall images separately show the intentional geometry change at the same
gameplay zoom/world scale. No game artwork was replaced or screenshot resized.

| Matched view | Before | After |
|---|---|---|
| Sanctuary storage area | [PNG](before/sanctuary.png) | [PNG](after/sanctuary.png) |
| Cave wall/vegetation and fog | [PNG](before/cave-vegetation.png) | [PNG](after/cave-vegetation.png) |
| Garage threshold | [PNG](before/garage-entry.png) | [PNG](after/garage-entry.png) |
| Garage props and wall | [PNG](before/garage-bay.png) | [PNG](after/garage-bay.png) |
| Dusk | [PNG](before/garage-dusk.png) | [PNG](after/garage-dusk.png) |
| Night | [PNG](before/garage-night.png) | [PNG](after/garage-night.png) |
| Lamp left | [PNG](before/occlusion-left.png) | [PNG](after/occlusion-left.png) |
| Lamp right | [PNG](before/occlusion-right.png) | [PNG](after/occlusion-right.png) |
| Lamp off | [PNG](before/occlusion-off.png) | [PNG](after/occlusion-off.png) |

Lamp probes disable fog presentation only to measure static-floor light blocking.
Other views retain normal discovery. Developer messages are retained in matched pairs.

The new opt-in hall:

- [Actual footprint and future reservations](footprint.svg) — a diagram, not game artwork.
- [Storage and workshop](hall/hall-storage.png)
- [Garden](hall/hall-garden.png)
- [Workshop](hall/hall-workshop.png)
- [Power](hall/hall-power.png)
- [Radio](hall/hall-radio.png)
- [Entrance frame and open clearance](hall/hall-entrance.png)
- [Inert damaged resurrection shell](hall/resurrection-alcove.png)
- [Six simulated actors](hall/six-actors.png)
- [Manually powered sanctuary lighting](hall/hall-powered.png)
- [Surface arrival after threshold crossing](hall/entrance-crossing.png)

HUD stress captures use long contextual/location text and these requested window
sizes. Integer-scaled content can be smaller than the window; the PNG contains
the native game-content pixels, excluding surrounding letterboxing/OS borders.

| Window request | Capture |
|---|---|
| 960×540 | [768×432 content](hall/hud-960x540.png) |
| 1280×720 | [768×432 content](hall/hud-1280x720.png) |
| 1536×864 | [1536×864 content](hall/hud-1536x864.png) |
| 1920×1080 | [1536×864 content](hall/hud-1920x1080.png) |
| 1024×768 | [768×432 content](hall/hud-1024x768.png) |
| 2560×1080 | [1536×864 content](hall/hud-2560x1080.png) |

[Run/test guide and measurements](../../SANCTUARY_HALL_REVIEW.md).
