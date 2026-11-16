# M5a: Basic crafting and resource investment

Implemented and validated 2026-11-15; subsequently approved by the owner. Accepted M4b is preserved in
`2690c74`, with a report-only time-label correction in `ad4684b`. The accepted M5a milestone is preserved in Git history.

## Recipes and ownership

[data/crafting.json](../data/crafting.json) owns recipe IDs, inputs, outputs,
required consumer, descriptions, kit IDs, upgraded capacity and garden socket offset.

| Recipe | Personal ingredients | Output / installation |
|---|---|---|
| Backpack Extension | 8 Scrap + 3 Organic Material | One transferable kit; install once per player for eight slots |
| Growing-Bed Kit | 12 Scrap + 4 Organic Material | One transferable kit; install at garden marker for one shared fifth plot |
| Cave Potato Seed Propagation | 1 Cave Potato Seed + 1 Organic Material | Two Cave Potato Seeds |

The first six personal slots keep their indices and are reserved for future hotbar
use. Slots 7–8 are explicitly **storage-only**, marked with teal upper edges and
tooltips. No hotbar or equipment system is implemented. Expanded slots support
normal storage, gathering, transfers, splits, meals and crafting ingredients;
manual inventory installation is not a hotbar action. Stack limit remains ten.

Kits can be put in the communal chest and withdrawn by another actor. Ownership
is established at installation: the backpack belongs to the installing actor,
while the fifth plot belongs to the shared session. One bed kit may exist or be
installed in the world; backpack kit production is capped by the current party's
remaining unupgraded players. Already-upgraded actors may craft for teammates.
Duplicate/failed installation keeps the kit. No starting upgrades or free seeds.

Propagation is an approved prototype recipe rather than a change to crop yields.
It uses generic recipe inputs/outputs and a configured consumer; it can move to a
future gardening/seed-processing station without rewriting crop growth/harvesting.
All original four plots, growth times, yields and one-seed harvest returns remain.

## Play

1. Gather Scrap and Organic Material from the existing finite sources. Withdraw any
   stored ingredients explicitly; the workbench never consumes communal contents.
2. Withdraw Fuel, load/start the generator, and manually enable **Workbench: 2 PU**.
   Beacon + workbench fits 4 PU; adding sanctuary lighting would exceed capacity.
   Existing capacity rejection and manual allocation remain unchanged.
3. Approach the existing workbench and press **E / A**. Select a recipe with the
   mouse or up/down / controller stick/D-pad. Enter/A selects the Craft button;
   confirm again to craft. Clicking a recipe selects it; click **Craft selected**
   to confirm. Requirements show carried/required counts, with benefit and feedback.
4. Power is required at the authoritative crafting instant. Crafting is immediate;
   there is no queue, timed hold, ingredient reservation or cursor inventory.
   The owner stands still in the panel while the world, Fuel, meals and crops run.
   Esc/B/View/Tab closes; leaving interaction range closes and revokes access.
   **Power allocation** opens the existing manual generator/consumer controls.
5. For a backpack kit, open the backpack, focus/select that kit and use **Install**,
   **U**, or the controller's **left-stick click**. Original slots stay in place;
   only the consumed kit changes. Chest transfers retain their existing controls.
6. Bring a growing-bed kit to the garden marker below the original beds. Press
   **E / A** to install, or use Install in inventory while nearby. Remote attempts
   retain the kit. The low-profile tray reuses the approved PNG and does not change
   existing wall/prop collision or passage access. There is one prevalidated socket,
   not free placement, land clearing or world generation.
7. Plant/harvest through the existing plot interactions. To fill the fifth plot
   while keeping four originals planted, propagate a returned potato seed before
   replanting, then put the extra seed in the new plot. No grow-light requirement.
8. R resets the run: six slots per actor, four original plots, no kits or installed
   upgrades, unchanged starting supplies. Rescue still ends and freezes the run;
   crafting and installation also reject post-victory commands.

New tray positions derive from the first garden and configured offset, currently
(184,182) / (184,198) in the two authored layouts. There is no new solid body;
existing collision footprints, camera coverage and movement remain unchanged.

## Revised-cost balance

The required pre-implementation budget check found **no significant new conflict**:
revised prices allow one backpack plus bed/propagation at **20 Scrap + 8 Organic**,
exactly the initial Organic supply. A second backpack with that investment requires
28 Scrap, above the finite 26. The choice shifts to group allocation versus farming,
rather than making the two upgrade categories mutually exclusive.

| Group size | Backpacks without expansion | Backpacks with working fifth plot |
|---:|---:|---:|
| 2 | 2 | 1 |
| 4 | 3 | 1 |
| 6 | 3 | 1 |

The third backpack needs nine Organic, so it waits for a Glowcap harvest. Material
supplies do not scale with players. Baseline Organic is eight finite plus four from
seven-day Glowcap harvests. Scrap remains 26; surface Food remains 8–17; total Fuel
remains 7–10; personal starting Food remains two each. No source or crop rebalance.

[Actual transaction simulations](M5A_CRAFTING_BALANCE.md) use timed material trips,
manual withdrawals, powered crafting visits, kit transfers/installation, farming,
Food distribution, Fuel and rescue for 2/4/6 actors. A fifth potato planted D3 04:00
adds six Food by Day 7 under regular visits or three under delayed visits. In the
minimum-Food six-actor delayed case, two missed meals become zero, with one Food
remaining. The original four plots keep their yields and growth times.

All tested investment schedules reach rescue. A thirty-second powered visit adds
0.02 Fuel compared with beacon-only operation; this is an estimate of time spent
in controls, not a separate crafting fee or required duration. Leaving the bench
powered keeps burning Fuel, even after closing the panel. Existing waste/outage
scenarios can still exhaust finite Fuel and make rescue unreachable. The compact
map limits backpack payoff; human group balance remains to be evaluated.

## Transactions and validation

Pure C# `Crafting.cs` / `SharedCrafting.cs` extend the existing authority owner.
One lock protects time/power/inventory/plot state. Requests carry session/inventory
and power revisions; world-owned adapters validate actor, modal access and range.
Crafting checks required power and personal inputs, subtracts inputs in a private
copy, checks **all outputs after subtraction**, then commits once. A full output
rejection spends nothing; a full pack may succeed if consumed inputs free space.
Only explicit propagation creates the configured net extra seed. Installation
consumes one kit and applies one upgrade under the same lock. UI previews hold no
resources. Stale pointer-down requests cannot spend against newer state.

```sh
dotnet build Hollowlight.csproj
dotnet run --project tests/Hollowlight.Core.Tests.csproj -- --crafting-balance docs/M5A_CRAFTING_BALANCE.md
godot --headless --fixed-fps 60 --path . res://tools/CraftingSmoke.tscn
godot --headless --fixed-fps 60 --path . res://tools/CraftingSmoke.tscn -- --sanctuary-variant
godot --fixed-fps 60 --path . res://tools/CraftingSmoke.tscn -- --capture-crafting
```

The graphical tool captures recipes, eight-slot backpack/chest, installed tray and
harvest into `user://crafting`. Run without capture flags for headless assertions.
Reviewed captures: [recipes](screenshots/crafting/recipes.png),
[kit installation](screenshots/crafting/kit-install.png),
[backpack](screenshots/crafting/backpack.png),
[chest](screenshots/crafting/expanded-chest.png),
[installed bed](screenshots/crafting/installed-bed.png),
[mature crop](screenshots/crafting/mature-plot.png),
[harvest](screenshots/crafting/harvest.png).
Normal launch remains `godot --path .`; see README for this workspace's tool paths.

Tests cover exact costs, personal-only ingredients, configurable propagation,
post-consumption capacity, full seed stacks, ownership/transfer, original slot
indices/storage-only metadata, shared installation cap, 96 crafting/install/power
races, exhaustion/stale sessions, victory/reset and all prior core systems. Live
checks cover mouse/keyboard/controller events, recipe confirmation, range exit,
modal time/Fuel, stale held mouse requests, installation, expanded slot navigation,
seed propagation, unchanged harvests and reset on both layouts.

The existing PNG assets and crop/food/Fuel/hunger/rescue data are unchanged. Icons
are approved sprite/bed atlas regions. No new manufacturing intermediates, equipment,
recipes beyond these three, clearing, grow lights, combat or terrain generation.
No physical-controller, human multiplayer, network session or save/load claim.
M5a is accepted. M6a geography remains a proposal pending separate design approval.
