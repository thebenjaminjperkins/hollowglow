# Provisional art target — nature reclaiming the world

Owner direction, 2026-11-16. Reference: the attached 4×4 garage sprite sheet in
the owner's art-direction message, showing overgrown cupboards, painted metal
cabinets, tool racks, crates, compressors and masonry. It establishes material,
vegetation and atmosphere targets, not a ready-to-import atlas or new gameplay
contents. The reference is supplied in the conversation; this document does not
claim a repository copy or runtime import of it.

## Preserve and refine

Preserve the approved asset quality, detailed shading, deliberate pixel clusters,
material texture and common native density. Continue modular PNG state sheets,
terrain atlases, nearest filtering, two native pixels/world unit, existing display
scale and separate world-unit collision footprints. Environmental light remains
adjustable rather than baked into replacement sprites.

Reduce rust and corrosion slightly, exposing more original painted metal,
weathered wood and identifiable machinery. Keep wear specific to material and
exposure: seams, chips, edges and damp joints, rather than equal brown coverage.
The reference's bright greens should inform the presence of life without forcing
uniform saturation across Hollowlight's quieter palette.

Use moss, vines, ferns, small flowers and occasional mushrooms in varied amounts.
Some objects remain relatively clear; others are heavily colonized. Preserve
handles, openings, gauges, tool silhouettes and loot/depletion cues. Decorative
growth does not imply harvestable resources or block previously usable routes.

The Bloom is a restrained subset of vegetation: unusual shapes, muted violet
foliage and subtle teal bioluminescence. It should be beautiful in daylight, with
nighttime danger arising primarily from future behavior/environmental changes.
No new hazards, nighttime growth behavior, enemies or resource rules are authorized
by this update. Maintain melancholy, coziness and mystery without uniform desolation
or cartoon cheerfulness.

## Current M6b checkpoint

Garage functionality and validation were completed before this update. Preserve
that working review profile, its captures and all existing tests. This update
changes documentation only; no sprite, atlas, palette, light setting, world
generation, quantity, collision, timer or interaction has been replaced.

## Assets needing attention

| Family | Refinement | Priority |
|---|---|---|
| Painted fuel cabinet and compressor | Reveal original paint/metal; localize rust; add different levels of natural colonization | First comparison |
| Garage masonry modules | Moss in damp joints, trailing vines and sparse flowers; retain readable caps/joins | First comparison |
| Vegetation accents | Fern/flower accents plus one restrained violet/teal Bloom cluster | First comparison, minimal set |
| Office/parts cupboards, crate and tool rack | Show weathered wood grain, reduce orange corrosion and vary overgrowth | Later, after comparison |
| Intact/partial/stripped van | More surviving paint and selective vegetation; preserve state registration and readable removals | Later, after comparison |
| Concrete/apron transitions | Sparse moss/crack growth at edges, with quiet open walking areas | Evaluate in comparison; no initial atlas replacement |

The existing character and sanctuary machinery remain quality/density anchors;
they do not need a broad migration to evaluate this direction.

## Proposed small comparison — approval before asset production/replacement

Use the same garage bay, camera, daylight, lighting controls and object positions
for an **existing-versus-candidate** comparison. Preserve the current profile and
export candidates to an isolated trial directory until reviewed.

1. **Lightly colonized fuel cabinet:** three matching closed/open/depleted frames.
   Retain gray-green paint, restrained edge corrosion and a small moss/vine accent.
   Empty contents must remain immediately readable through all foliage.
2. **Heavily colonized compressor:** intact/stripped frames with surviving painted
   tank, localized wear and natural growth around its base. Keep gauge, motor and
   removed-part silhouette visible. Growth cannot disguise the salvage state.
3. **Moderately overgrown wall corner:** reuse the wall modules with detachable
   fern/flower and one small violet/teal Bloom accent. Keep floor routes open and
   joins reusable. The comparison should include clear, ordinary and Bloom-colonized
   surfaces together, rather than covering the whole room equally.

Process the supplied reference or newly derived isolated artwork through the
existing documented source-to-PNG workflow. Keep canvas dimensions, anchors,
footprints and native density fixed. Try the existing shared palette first;
if violet/teal cannot be represented faithfully, present a small explicit shared
palette amendment for review rather than adding unrelated per-asset colors.
Use separate emission/light controls for any glow test; do not paint large light
pools into diffuse sprites or change gameplay lighting requirements.

Evaluate native pixels, actual gameplay zoom and identical daylight/neutral/night
presentation captures. Night capture tests visual legibility only, not new danger.
Compare retained paint/wood, vegetation variety, supernatural restraint, silhouette,
search/salvage states, texture noise, character cohesion and modular joins. Preserve
resource and collision behavior throughout. Review the small trial before expanding
to the van, remaining furniture or terrain library. This proposal is not a completed
comparison or approval to replace the current assets.
