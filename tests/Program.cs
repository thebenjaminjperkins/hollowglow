using System;
using System.Collections.Generic;
using System.IO;
using Hollowlight.Core;

static void Check(bool condition, string message)
{
    if (!condition) throw new Exception(message);
}

ClockTests.Run();
ScavengingTests.Run();
HungerTests.Run();
if (Array.IndexOf(args, "--food-balance") is int balanceIndex && balanceIndex >= 0)
    FoodBalanceSimulation.Run(balanceIndex + 1 < args.Length ? args[balanceIndex + 1] : null);
if (Array.IndexOf(args, "--scavenging-balance") is int scavengingIndex && scavengingIndex >= 0)
    FoodBalanceSimulation.RunScavenging(scavengingIndex + 1 < args.Length ? args[scavengingIndex + 1] : "docs/M3B_FOOD_BALANCE.md");
FarmingTests.Run();
SuppliesTests.Run();
SlotOperationsTests.Run();

var objects = new Dictionary<string, int> { ["radio"] = 2, ["crate"] = 3 };
var state = new SalvageState(objects);
Check(state.Deposit(1) == 0 && state.Stored == 0, "Empty deposit changes nothing");
Check(state.TryCollect(1, "radio"), "First pickup succeeds");
Check(!state.TryCollect(2, "radio"), "Two players cannot collect the same object");
Check(!state.TryCollect(1, "missing"), "Unknown pickup rejected");
Check(state.CarriedBy(1) == 2 && state.CarriedBy(2) == 0, "Carry belongs to the actor");
Check(state.TryCollect(2, "crate"), "Second actor can collect another object");
Check(state.Deposit(1) == 2 && state.Stored == 2, "Deposit transfers quantity");
Check(state.CarriedBy(1) == 0 && state.CarriedBy(2) == 3, "Deposit preserves other actor");
Check(state.Deposit(1) == 0 && state.Stored == 2, "Repeated deposit cannot duplicate");
Check(state.Deposit(2) == 3 && state.Stored == 5, "Shared total conserves all salvage");
var reset = new SalvageState(objects);
Check(reset.Stored == 0 && reset.CarriedBy(1) == 0 && reset.IsAvailable("radio"), "Fresh state restores loop");
try
{
    _ = new SalvageState(new Dictionary<string, int> { ["bad"] = 0 });
    throw new Exception("Invalid quantity accepted");
}
catch (ArgumentOutOfRangeException) { }
Console.WriteLine("PASS: pickup uniqueness, actor isolation, deposits, conservation, reset, invalid quantities");

var walk = new WalkPresentation();
walk.Update(0.1, 80, 0);
Check(walk.Facing == WalkFacing.East && walk.Column >= 2, "Horizontal facing selects authored walk");
walk.Update(0.1, 56.56f, -56.56f);
Check(walk.Facing == WalkFacing.East, "Diagonal tie preserves previous axis");
walk.Update(0.1, 0, -80);
Check(walk.Facing == WalkFacing.North, "Vertical movement can change facing");
walk.Update(0.1, -56.56f, -56.56f);
Check(walk.Facing == WalkFacing.North, "Diagonal tie remains stable on vertical axis");
walk.Update(0.1, 0, 0);
Check(walk.Facing == WalkFacing.North && walk.Column < 2, "Idle preserves facing");
var fern = new FoliageResponse();
fern.Brush(-1);
Check(fern.Column == 1, "Left brush selects left bend");
fern.Update(2);
Check(fern.Column == 0, "Fern settles to authored rest pose");
fern.Brush(1);
Check(fern.Column == 4, "Right brush selects right bend");
Console.WriteLine("PASS: directional presentation, diagonal ties, idle and vegetation settling");

foreach (string path in Directory.GetFiles(Path.Combine(AppContext.BaseDirectory, "data"), "sanctuary*.json"))
{
    var layout = CaveLayout.Parse(File.ReadAllText(path));
    var visited = new HashSet<(int x, int y)>();
    var queue = new Queue<(int x, int y)>();
    queue.Enqueue((8, 13));
    while (queue.Count > 0)
    {
        var (x, y) = queue.Dequeue();
        if (!layout.IsFloor(x, y) || !visited.Add((x, y))) continue;
        queue.Enqueue((x - 1, y)); queue.Enqueue((x + 1, y));
        queue.Enqueue((x, y - 1)); queue.Enqueue((x, y + 1));
    }
    int floors = 0;
    for (int y = 0; y < 22; y++)
        for (int x = 0; x < 40; x++) if (layout.IsFloor(x, y)) floors++;
    Check(visited.Count == floors, $"{layout.Name}: every floor tile reachable");
    foreach (var point in new[] { (8, 13), (5, 14), (23, 11), (27, 10), (33, 7), (32, 16) })
        Check(visited.Contains(point), $"{layout.Name}: essential gameplay anchor reachable");
    foreach (var prop in layout.Props)
        if (prop.Width > 0 && prop.Height > 0)
            Check(layout.IsFloor(prop.X / 16, prop.Y / 16), $"{layout.Name}: solid prop anchored on floor");
    Console.WriteLine($"PASS: {layout.Name}: connected floor, closed boundary and usable gameplay anchors");
}
