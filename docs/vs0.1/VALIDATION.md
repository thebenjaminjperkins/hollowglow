# VS0.1 — New validation versus historical evidence

Date: 2026-11-17. These selected **new runs** use the captured working tree. No
gameplay/source/art/config changes were made to make them pass. Ignored engine/
compiler caches and isolated `/tmp` user data may be written by normal execution.

## Environment and actual results

Linux, .NET SDK **8.0.424**, Godot **4.4.1.stable.mono.official.49a5bc7b6**;
Python version recorded in [tool log](validation/python-version.txt).
Godot executable was available under the existing temporary installation, not on
the user's ordinary `godot` PATH. No engine/SDK/package upgrade was attempted.

| New run | Exit | Evidence / scope |
|---|---:|---|
| `dotnet build Hollowlight.csproj --no-restore` | 0 | [Build](validation/build.txt): zero warnings/errors |
| `dotnet run --project tests/Hollowlight.Core.Tests.csproj --no-restore` | 0 | [Core suite](validation/core-tests.txt): existing clock/region/garage/hall, inventory/slot, farm, hunger, power, crafting, rescue/conservation/concurrency assertions |
| Headless `HallSmoke.tscn`, fixed60 Hz | 0 | [Hall](validation/hall-smoke.txt): real scripted movement, six bodies/wide proxy, HUD bounds, planting, finite gathering, powered kit/fifth plot, harvest/reset |
| Headless `GarageSmoke.tscn`, seed1701, preserve van | 0 | [Garage](validation/garage-smoke.txt): live navigation/search/partial transfer/tool/cancellation/fog/depletion/return/reset; van preservation |
| Existing three art checkers | 0 each | [Sanctuary](validation/art-sanctuary.txt), [garage](validation/art-garage.txt), [region](validation/art-region.txt): source/export/palette/density/hash conventions |
| New VS0.1 arithmetic worksheet | 0 | [Worksheet log](validation/timing.txt), two CSVs and summary; no gameplay execution implied |

Machine-readable fresh run inventory: [validation/results.json](validation/results.json).

Commands were run without balance-export flags, so core tests did not regenerate
existing balance reports. Engine tests used separate user directories. The core
suite's concurrency cases are model validation, not connected multiplayer.

The fresh preserve-van garage run measures **89.58s**, including36.10s outbound,
16.92s site movement,35.07s return and test-speed interactions. It uses the existing
five-chamber garage profile; it does **not** remeasure the hall-to-garage trip,
two-person discovery, combat, or organic-source detours. Keep those unknowns in the
scenario worksheet. Do not pad timers to match the2–4-minute human target.

## Reproduce selected existing checks

From the repository root, set up the installed Godot .NET/.NET8 environment.
Example using this checkout's existing temporary tools:

```sh
export PATH="$HOME/.dotnet:$PATH"
export DOTNET_ROOT="$HOME/.dotnet"
export DOTNET_CLI_HOME=/tmp/hollowlight-m5-cli
export NUGET_PACKAGES="$HOME/.nuget/packages"
export XDG_DATA_HOME=/tmp/hollowlight-vs01-data
export XDG_CONFIG_HOME=/tmp/hollowlight-vs01-config
export XDG_CACHE_HOME=/tmp/hollowlight-vs01-cache
export HOLLOWLIGHT_ENGINE=/tmp/hollowlight-godot-m6b/Godot_v4.4.1-stable_mono_linux_x86_64/Godot_v4.4.1-stable_mono_linux.x86_64
dotnet build Hollowlight.csproj --no-restore
dotnet run --project tests/Hollowlight.Core.Tests.csproj --no-restore
"$HOLLOWLIGHT_ENGINE" --headless --fixed-fps 60 --path . res://tools/HallSmoke.tscn
"$HOLLOWLIGHT_ENGINE" --headless --fixed-fps 60 --path . res://tools/GarageSmoke.tscn -- --world-seed=1701 --preserve-van
python3 tools/art/check_sanctuary.py
python3 tools/art/check_garage.py
python3 tools/art/check_region.py
python3 docs/vs0.1/timing_worksheet.py
```

Fresh environments need the pinned engine/SDK and restored local dependencies
before `--no-restore` can work. Existing art checkers use their existing Pillow
dependency; the new timing worksheet is Python standard-library only. Do not assume
the `/tmp` executable survives a reboot. Raw versions are archived alongside logs.

## What was not revalidated

No new screenshots, graphics/art generation, GPU profile, physical-controller feel,
two-human session, host/client network, combat, recovery or extraction was tested.
The current hall/garage screenshots, light probes, old23-process regression matrix
and14-day study remain **historical** evidence. Selected smoke checks do not make
the unsatisfactory complete-world visuals acceptable. No new profile exists to play.

Pre-/post-work preservation results are in [preservation.json](preservation.json).
The baseline includes SHA-256 of ignored caches, but their regeneration is expected;
the no-change assertion applies to all existing nonignored source/evidence and
tracked-deletion status. No existing staged/index objects may change either.
