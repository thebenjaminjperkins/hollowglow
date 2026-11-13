using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Hollowlight.Core;

// Uses the same authoritative clock, farm, inventory transfers and meal settlement
// as gameplay. No synthetic food injections or changed crop definitions.
public static class FoodBalanceSimulation
{
    private static void Check(bool value, string message) { if (!value) throw new Exception(message); }
    private static string Data(string name) => File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "data", name + ".json"));
    public static void Run(string? output)
    {
        var report = new StringBuilder("# M3a seven-day food balance\n\n");
        report.AppendLine("Reproduce: `dotnet run --project tests/Hollowlight.Core.Tests.csproj -- --food-balance docs/M3A_FOOD_BALANCE.md`\n");
        report.AppendLine("Each scenario uses the actual four plots, two shared seeds per crop, unchanged 2/3-day growth and 3/2 Food yields, two starting personal Food per player, and one Food/day. Day 1 starts at elapsed 0; rows settle at the following dawn. Food is never auto-withdrawn from storage. No exploration food exists in this milestone.\n");
        report.AppendLine("A designated gardener withdraws seeds at 00:20, plants at 01:00, 01:10, 01:20 and 01:30. Scheduled visits occur at 03:00, 05:30 and 08:30 of each ten-minute cycle. Ready plots take ten seconds each to harvest/replant, with replanting five seconds after harvesting. After each visit, the gardener deposits harvested Food/Organic; a group resupply occurs one minute after the scheduled visit. Players explicitly withdraw toward two personal Food each, rotating first priority by day. This represents regular cooperation with exploration intervals, not instant harvesting or automatic communal feeding. A delayed-care scenario omits the midday visit, retaining the same planting and resupply rules. These are reproducible timing scenarios, not measured human playtests.\n");
        foreach (bool delayed in new[] { false, true })
        {
            report.AppendLine(delayed ? "## Delayed care: two visits per day\n" : "## Regular care: three visits per day\n");
            foreach (int count in new[] { 2, 4, 6 }) Simulate(count, delayed, report);
        }
        report.AppendLine("## Interpretation\n");
        report.AppendLine("A positive final group balance does not guarantee every meal deadline is met: production arrives in discrete harvests, and Food left in the chest is not eaten. Missed deadlines below count individual player-meals, not lost Food. Supplied meals gradually recover accumulated missed-meal scores. Priority rotation distributes scarce supplies without creating food.\n");
        report.AppendLine("The mixed four-plot farm averages 4⅓ Food/day over the long run, below six players' 6/day. Surface exploration currently rewards Scrap and Organic only and cannot repair a Food shortage. Future scavenging, outdoor agriculture and infrastructure are explicitly deferred. No crop, starter-seed, plot-count or economy rebalance was made.\n");
        if (output != null) File.WriteAllText(output, report.ToString());
        else Console.WriteLine(report.ToString());
        Console.WriteLine("PASS: timed seven-day 2/4/6-player balances, regular and delayed care, farm/meal/distribution operations and exact Food conservation");
    }
    public static void RunScavenging(string output)
    {
        SourceWorld? minimum = null, maximum = null;
        for (int seed = 0; seed < 4096 && (minimum == null || maximum == null); seed++)
        {
            var world = ScavengingTests.Generate(seed); int total = world.Sources.Sum(s => s.Site.Quantity);
            if (total == 8) minimum ??= world;
            if (total == 17) maximum ??= world;
        }
        Check(minimum != null && maximum != null, "Find real seeded worlds at both approved quantity extremes");
        var normal = ScavengingTests.Generate(1701);
        var report = new StringBuilder("# M3b seven-day Food scavenging balance\n\n");
        report.AppendLine("Reproduce: `dotnet run --project tests/Hollowlight.Core.Tests.csproj -- --scavenging-balance docs/M3B_FOOD_BALANCE.md`\n");
        report.AppendLine("Uses the authoritative seeded source generator, finite gathering, personal inventories, explicit storage transfers, unchanged crop simulation and dawn meals. All worlds have six sources independent of player count. The approved explicit ranges sum to **8–17 Food**, not 9–17; no minimum-stock top-up is applied. The expected uniform total is 12.5 Food.\n");
        report.AppendLine("Farm schedules match the M3a ledger: withdraw seeds at D1 00:20; plant at 01:00/01:10/01:20/01:30; check plots each day at 03:00, 05:30, 08:30 (regular) or 03:00 and 08:30 (delayed). Harvests take ten-second plot visits, replant five seconds after collection. Farm Food is deposited; every visit is followed by group resupply one minute later. Players explicitly withdraw toward two personal Food, giving everyone the next meal before a reserve and rotating priority by day. Starting Food remains two per actor; daily requirement remains one.\n");
        report.AppendLine("A second actor scavenges the three near nodes at D1 01:00/01:10/01:20 and deposits only those gathered quantities at D1 02:00. Far nodes are collected at D3 04:00/04:10/04:20 and deposited at D3 05:00. These conservative collection/return schedules allow search and travel time on the compact map; they are reproducible scenarios, not measured multiplayer trips or simulations of future large worlds. Sources and gathered quantities come from actual generated worlds, never fabricated deposits.\n");
        report.AppendLine("Each row settles the listed day at the next dawn. **Net supply − need** includes initial Food on Day 1, plus that day's farm/scavenging deliveries minus the full group requirement. A negative flow can be covered by an existing reserve; **Unmet meals today** reports actual personal shortages. Personal/chest reserve is the positive stock left after eating. Unpicked Food is not counted as available meals. Every daily row checks exact Food conservation and accounts for every obligation.\n");
        foreach (var world in new[] { minimum!, normal, maximum! })
        {
            report.AppendLine($"## World seed {world.Seed}: {world.Sources.Sum(s => s.Site.Quantity)} Food\n");
            report.AppendLine(string.Join("; ", world.Sources.Select(s => $"{s.Site.Id}: {s.Site.Quantity}")) + ".\n");
            foreach (bool delayed in new[] { false, true })
            {
                report.AppendLine(delayed ? "### Delayed farm care\n" : "### Regular farm care\n");
                foreach (int players in new[] { 2, 4, 6 }) Simulate(players, delayed, report, world);
            }
        }
        report.AppendLine("## Late expedition stress case: minimum-stock world, delayed farm care\n");
        report.AppendLine("Near trip unchanged; postpone the far trip until Day 7, returning at 05:00. This demonstrates that a finite cache discovered late cannot retroactively cure missed daily requirements.\n");
        foreach (int players in new[] { 2, 4, 6 }) Simulate(players, true, report, minimum, true);
        report.AppendLine("## Interpretation\n");
        report.AppendLine("Scavenging supplies immediate relief but is permanently exhausted for that world. The farm remains renewable at unchanged rates; the mixed four plots average 4⅓ Food/day, below six players' need. With no farming at all, starting Food plus the finite cache supplies only 12–21 Food for two players, 16–25 for four, and 20–29 for six, against seven-day needs of 14/28/42. Even a rich cache does not establish long-term sustainability.\n");
        report.AppendLine("No quantities scale with players, no crops or hunger rules were rebalanced, and no enemies, respawning, power or other survival systems were added. Six-player shortages in poor/delayed scenarios are reported rather than patched. Core group simulations are not a networked or human multiplayer playtest.\n");
        File.WriteAllText(output, report.ToString());
        Console.WriteLine($"PASS: actual seeded 8/default/17-Food worlds, daily 2/4/6-player ledgers, regular/delayed farm care and late expedition stress; report {output}");
    }
    private static void Simulate(int count, bool delayed, StringBuilder report, SourceWorld? scavenging = null, bool late = false)
    {
        var ids = Enumerable.Range(1, count).ToArray(); var setup = AgricultureSetup.Parse(Data("farming"));
        var supplySetup = SupplySetup.Parse(Data("supplies"));
        if (scavenging != null) supplySetup = supplySetup with { Sites = supplySetup.Sites.Concat(scavenging.Sources.Select(s => s.Site)).ToArray() };
        var s = supplySetup.CreateState(ids, 24, setup.Resources); var c = new GameClock(new());
        var plots = new[] { "a", "b", "c", "d" }; s.InitializeAgriculture(setup, c, plots);
        s.InitializeHunger(HungerDefinition.Parse(Data("hunger")), c);
        var events = new SortedDictionary<int, List<Action>>();
        int scavenged = 0, lastProduced = 0, lastScavenged = 0; long lastConsumed = 0, lastMissed = 0;
        int produced = 0, organic = 0; var harvests = new List<string>();
        void At(int second, Action action)
        { if (!events.TryGetValue(second, out var list)) events[second] = list = new(); list.Add(action); }
        void Transfer(int id, string resource, int amount, bool withdraw)
        {
            if (amount <= 0) return;
            var result = s.Transfer(id, resource, amount, withdraw, s.Snapshot(id).Version);
            Check(result.Amount == amount, "Simulated transfer fits and conserves all stock");
        }
        void Resupply(int day)
        {
            // Fair one-unit rounds: give everyone this day's need before a reserve.
            for (int round = 1; round <= 2; round++)
                for (int i = 0; i < count; i++)
                {
                    int id = (i + day) % count + 1;
                    int need = Math.Max(0, round - s.Snapshot(id).Personal.Count("food"));
                    int amount = Math.Min(need, s.Snapshot(id).Storage.Count("food"));
                    Transfer(id, "food", amount, true);
                }
        }
        if (scavenging != null)
        {
            foreach (string band in new[] { "near", "far" })
            {
                int returned = band == "near" ? 120 : late ? 3900 : 1500;
                int delivered = 0, index = 0;
                foreach (var source in scavenging.Sources.Where(s => s.Band == band))
                {
                    var node = source;
                    At(returned - 60 + index++ * 10, () => {
                        var result = s.Gather(2, node.Site.Id, int.MaxValue, s.Snapshot(2).Version);
                        Check(result.Amount == node.Site.Quantity, "Timed field collection fits personal backpack");
                        delivered += result.Amount;
                    });
                }
                At(returned, () => {
                    Transfer(2, "food", delivered, false); scavenged += delivered;
                });
            }
        }
        At(20, () => { Transfer(1, "potato_seed", 2, true); Transfer(1, "glowcap_seed", 2, true); });
        for (int i = 0; i < 4; i++)
        {
            int plot = i;
            At(60 + i * 10, () => Check(s.Plant(1, plots[plot], plot < 2 ? "cave_potato" : "glowcap", s.Plot(plots[plot]).Version).Status == FarmStatus.Planted, "Initial seed planting"));
        }
        for (int day = 0; day < 7; day++)
        {
            int d = day;
            foreach (int visit in delayed ? new[] { 180, 510 } : new[] { 180, 330, 510 })
            {
                int time = day * 600 + visit;
                for (int i = 0; i < 4; i++)
                {
                    int p = i; int harvestTime = time + i * 10;
                    At(harvestTime, () => {
                        var plot = s.Plot(plots[p]); if (plot.Stage != CropStage.Mature) return;
                        var crop = s.Crops.First(x => x.Id == plot.CropId);
                        Check(s.Harvest(1, plots[p], plot.Version).Status == FarmStatus.Harvested, "Timed complete harvest fits gardener pack");
                        int food = crop.Yields["food"]; produced += food; organic += crop.Yields.GetValueOrDefault("organic");
                        harvests.Add($"D{harvestTime / 600 + 1} {harvestTime % 600 / 60:00}:{harvestTime % 60:00} {crop.Name} +{food}");
                        Transfer(1, "food", food, false);
                        int extra = crop.Yields.GetValueOrDefault("organic"); Transfer(1, "organic", extra, false);
                        At(harvestTime + 5, () => Check(s.Plant(1, plots[p], crop.Id, s.Plot(plots[p]).Version).Status == FarmStatus.Planted, "Replant returned seed"));
                    });
                }
                At(time + 60, () => Resupply(d));
            }
            At((day + 1) * 600, () => {
                long consumed = ids.Sum(id => s.Hunger(id).Consumed), missed = ids.Sum(id => s.Hunger(id).MissedDeadlines);
                int personal = ids.Sum(id => s.Snapshot(id).Personal.Count("food")); int storage = s.Snapshot(1).Storage.Count("food");
                int inTransit = scavenging == null ? 0 : scavenging.Sources.Sum(n => n.Site.Quantity - s.Snapshot(1).Remaining[n.Site.Id]) - scavenged;
                Check(personal + storage + consumed == count * 2 + produced + scavenged + inTransit, "Daily exact Food conservation");
                Check(consumed + missed == count * (d + 1), "Every completed daily obligation accounted for");
                string debt = string.Join(",", ids.Select(id => s.Hunger(id).MissedMeals));
                if (scavenging == null) report.AppendLine($"| {d + 1} | {produced} | {consumed} | {missed} | {personal} | {storage} | {debt} |");
                else
                {
                    int dailyFarm = produced - lastProduced, dailyScavenged = scavenged - lastScavenged;
                    int net = (d == 0 ? count * 2 : 0) + dailyFarm + dailyScavenged - count;
                    int unpicked = scavenging.Sources.Sum(n => s.Snapshot(1).Remaining[n.Site.Id]);
                    report.AppendLine($"| {d + 1} | {dailyFarm} | {dailyScavenged} | {net:+0;-0;0} | {consumed - lastConsumed} | {missed - lastMissed} | {personal} | {storage} | {unpicked} | {debt} |");
                    lastProduced = produced; lastScavenged = scavenged; lastConsumed = consumed; lastMissed = missed;
                }
            });
        }
        report.AppendLine($"{(scavenging == null ? "###" : "####")} {count} players\n");
        report.AppendLine(scavenging == null
            ? "| Completed day | Farm Food to date | Eaten to date | Missed meals to date | Personal Food | Chest Food | Missed-meal scores by player |\n|---:|---:|---:|---:|---:|---:|:---|"
            : "| Day | Farm today | Scavenged today | Net supply − need | Eaten today | Unmet meals today | Personal reserve | Chest reserve | Unpicked | Missed-meal scores |\n|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---|");
        while (events.Count > 0)
        {
            var pair = events.First(); events.Remove(pair.Key);
            c.Advance(pair.Key - c.Snapshot().TotalSeconds);
            foreach (var action in pair.Value) action();
        }
        Check(produced > 0 && produced <= 26 && (delayed || produced == 26) && organic == 4, "Unchanged yields; visit delays may defer a potato harvest beyond day seven");
        report.AppendLine($"\nHarvest ledger: {string.Join("; ", harvests)}.\n");
        if (scavenging != null)
        {
            Check(scavenged == scavenging.Sources.Sum(s => s.Site.Quantity) && scavenging.Sources.All(n => s.Snapshot(1).Remaining[n.Site.Id] == 0), "Only the world's finite Food was collected, once");
            long expectedMissed = late && count == 6 ? 5 : Math.Max(0, count * 7 - (count * 2 + produced + scavenged));
            Check(ids.Sum(id => s.Hunger(id).MissedDeadlines) == expectedMissed, "Scheduled daily shortages match the finite budget and late-expedition stress expectation");
            report.AppendLine($"Total scavenged {scavenged}; missed meals {ids.Sum(id => s.Hunger(id).MissedDeadlines)}; ending reserve {ids.Sum(id => s.Snapshot(id).Personal.Count("food")) + s.Snapshot(1).Storage.Count("food")}.\n");
        }
        Console.WriteLine($"BALANCE seed={scavenging?.Seed.ToString() ?? "none"}, far={(late ? "late" : "early")} {count} players/{(delayed ? "delayed" : "regular")}: produced {produced}, consumed {ids.Sum(id => s.Hunger(id).Consumed)}, missed {ids.Sum(id => s.Hunger(id).MissedDeadlines)}, remaining {ids.Sum(id => s.Snapshot(id).Personal.Count("food")) + s.Snapshot(1).Storage.Count("food")}");
    }
}
