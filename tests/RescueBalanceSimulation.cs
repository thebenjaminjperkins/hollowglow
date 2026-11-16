using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Hollowlight.Core;

public static class RescueBalanceSimulation
{
    private static void Check(bool condition,string message){if(!condition)throw new Exception(message);}
    private static string Data(string name)=>ScavengingTests.Data(name);
    public static void Run(string output)
    {
        var report=new StringBuilder("# M4b combined Food / Fuel / rescue ledger\n\n");
        report.AppendLine("Reproduce: `dotnet run --project tests/Hollowlight.Core.Tests.csproj -- --rescue-balance docs/M4B_COMBINED_BALANCE.md`\n");
        report.AppendLine("Actual seeded Food and Fuel worlds, unchanged four plots, personal meals, manual transfers, exact generator accounting and rescue authority share one clock. Two starting Food per actor; shared seeds 2+2 and Fuel 2. Rows settle each completed day at dawn. Food flow includes starters on Day 1; reserves cover negative flows until exhausted. Fuel left includes unpicked caches, so feasibility is optimistic, not a promise that carried Fuel reaches the generator in time.\n");
        report.AppendLine("Farm: seeds at 00:20, planting 01:00/01:10/01:20/01:30; daily visits 03:00/05:30/08:30 (delayed omits 05:30), ten seconds per plot and replant five seconds later. Deposit yields immediately, resupply one minute after visits in rotating fair rounds toward two personal Food. Near Food returns D1 02:00, far D3 05:00; near Fuel D2 03:00, far D4 03:00. Gather thirty to sixty seconds before returns. Operator visits/refills daily 03:00 and on cache deliveries; initial transmission starts D1 00:30. These scripted cooperation schedules are not measured travel or networked playtests.\n");
        report.AppendLine("Scheduled load: beacon continuously, workbench seconds 30–60 daily, lighting 420–600. Full load: beacon + workbench continuously. Startup delay means seven transmission days finish after seven calendar days. Simulations continue to victory or Day 14; terminal freeze stops consumption exactly at victory. Outage stress stops the generator from elapsed 15:00–40:00 while retaining consumer selections; manual refills during the outage do not restart it. Wasteful case runs workbench alone for five days before attempting transmission; it illustrates finite-Fuel failure without changing supplies. Feasibility compares remaining Fuel with the optimistic 0.6 Fuel per remaining signal day (no future losses/idle/extras).\n");
        foreach(int seed in new[]{8,1701,499})foreach(int count in new[]{2,4,6})
        {
            var food=ScavengingTests.Generate(seed);
            Simulate(count,false,report,food,false,false);
            if(seed==8){Simulate(count,false,report,food,true,false);Simulate(count,false,report,food,true,false,true);}
            if(seed==499)Simulate(count,true,report,food,false,false);
        }
        foreach(int count in new[]{2,4,6})Simulate(count,false,report,ScavengingTests.Generate(8),false,true);
        report.AppendLine("## Limits and recovery\n\nFinite Fuel can make rescue mathematically unreachable after wasted generation, avoidable high-load use or repeated loss episodes. There is no forced game-over or hidden refill. Stop optional loads early and conserve remaining Fuel; if it is already below the beacon-only lower bound, this prototype requires New Run/reset. Food shortages slow actors but do not kill them or disable farming/power, so the accepted six-player food deficit does not itself block rescue. Future surface danger, actual multiplayer labor and larger-map travel are not simulated. No Food, Fuel, farm or hunger balance values changed.\n");
        File.WriteAllText(output,report.ToString().TrimEnd()+"\n");
        Console.WriteLine("PASS: combined timed 2/4/6-player Food/Fuel/rescue conservation and completion/infeasibility ledgers");
    }
    private static void Simulate(int count, bool delayed, StringBuilder report, SourceWorld scavenging, bool full, bool waste, bool interrupted=false)
    {
        var ids = Enumerable.Range(1, count).ToArray(); var setup = AgricultureSetup.Parse(Data("farming"));
        var supplySetup = SupplySetup.Parse(Data("supplies"));
        supplySetup = supplySetup with { Sites = supplySetup.Sites.Concat(scavenging.Sources.Select(s => s.Site)).ToArray() };
        var fuelWorld = FuelPlacementTests.Generate(scavenging.Seed);
        supplySetup = supplySetup with { Sites = supplySetup.Sites.Concat(fuelWorld.Sources.Select(x=>x.Site)).ToArray() };
        var s = supplySetup.CreateState(ids, 24, setup.Resources.Append(new ResourceDefinition("fuel","Fuel","Fuel"))); var c = new GameClock(new());
        var plots = new[] { "a", "b", "c", "d" }; s.InitializeAgriculture(setup, c, plots);
        s.InitializeHunger(HungerDefinition.Parse(Data("hunger")), c);
        s.InitializePower(PowerDefinition.Parse(Data("power")),c);
        s.InitializeRescue(RescueDefinition.Parse(Data("rescue")));
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
        {
            foreach (string band in new[] { "near", "far" })
            {
                int returned = band == "near" ? 120 : 1500;
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

        bool serviceHold=false;
        void Command(PowerAction action,bool enabled=false,string consumer="",int quantity=1)
        {
            var r=PowerTests.Command(s,action,enabled,consumer,quantity);
            Check(r.Status is PowerStatus.Applied or PowerStatus.NoChange or PowerStatus.NoFuel or PowerStatus.NoSupply or PowerStatus.TankFull,"Valid scheduled manual power command");
        }
        void Refill()
        {
            int available=s.Snapshot(1).Storage.Count("fuel");Transfer(1,"fuel",available,true);
            Command(PowerAction.LoadFuel,quantity:int.MaxValue);
            Transfer(1,"fuel",s.Snapshot(1).Personal.Count("fuel"),false);
            if(!serviceHold && !s.Power().Running && s.Power().Fuel>0)Command(PowerAction.SetRunning,true);
        }
        foreach(var source in fuelWorld.Sources)
        {
            int time=source.Band=="near"?780:1980;
            At(time-30,()=>Check(s.Gather(2,source.Site.Id,int.MaxValue,s.Snapshot(2).Version).Amount==source.Site.Quantity,"Actual finite Fuel collection fits"));
            At(time,()=>{Transfer(2,"fuel",source.Site.Quantity,false);Refill();});
        }
        At(30,()=>{Refill();Command(PowerAction.SetConsumer,true,waste?"workbench":"beacon");if(full&&!waste)Command(PowerAction.SetConsumer,true,"workbench");});
        if(interrupted)
        {
            At(900,()=>{serviceHold=true;Command(PowerAction.SetRunning,false);});
            At(2400,()=>{serviceHold=false;Refill();});
        }
        if(waste)At(3030,()=>{Command(PowerAction.SetConsumer,false,"workbench");Command(PowerAction.SetConsumer,true,"beacon");});
        // Explicit operator visits, never automatic tank topping by the game.
        for(int day=1;day<14;day++)At(day*600+180,()=>{Refill();if(!waste||c.Snapshot().TotalSeconds>=3030)Command(PowerAction.SetConsumer,true,"beacon");});
        if(!full&&!waste)for(int day=0;day<14;day++)
        {
            At(day*600+30,()=>{Command(PowerAction.SetConsumer,false,"lighting");Command(PowerAction.SetConsumer,true,"workbench");});
            At(day*600+60,()=>Command(PowerAction.SetConsumer,false,"workbench"));
            At(day*600+420,()=>Command(PowerAction.SetConsumer,true,"lighting"));
            At(day*600+600,()=>Command(PowerAction.SetConsumer,false,"lighting"));
        }
        decimal lastBurn=0; bool unreachable=false;
        At(20, () => { Transfer(1, "potato_seed", 2, true); Transfer(1, "glowcap_seed", 2, true); });
        for (int i = 0; i < 4; i++)
        {
            int plot = i;
            At(60 + i * 10, () => Check(s.Plant(1, plots[plot], plot < 2 ? "cave_potato" : "glowcap", s.Plot(plots[plot]).Version).Status == FarmStatus.Planted, "Initial seed planting"));
        }
        for (int day = 0; day < 14; day++)
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
                int inTransit = scavenging.Sources.Sum(n => n.Site.Quantity - s.Snapshot(1).Remaining[n.Site.Id]) - scavenged;
                Check(personal + storage + consumed == count * 2 + produced + scavenged + inTransit, "Daily exact Food conservation");
                Check(consumed + missed == count * (d + 1), "Every completed daily obligation accounted for");
                string debt = string.Join(",", ids.Select(id => s.Hunger(id).MissedMeals));
                int dailyFarm=produced-lastProduced,dailyScav=scavenged-lastScavenged;
                int net=(d==0?count*2:0)+dailyFarm+dailyScav-count;
                var power=s.Power();var signal=s.Rescue();var fuel=s.PowerFuel();
                int items=ids.Sum(id=>s.Snapshot(id).Personal.Count("fuel"))+s.Snapshot(1).Storage.Count("fuel");
                int unpicked=fuelWorld.Sources.Sum(x=>s.Snapshot(1).Remaining[x.Site.Id]);
                Check(fuel.TankCredits+fuel.ConsumedCredits+(items+unpicked)*fuel.CreditsPerFuel==(2+fuelWorld.Sources.Sum(x=>x.Site.Quantity))*fuel.CreditsPerFuel,"Daily combined Fuel conservation");
                decimal left=power.Fuel+items+unpicked;
                bool impossible=left<(signal.RequiredDays-signal.SignalDays)*.6m;
                unreachable|=impossible;
                if(d<7)report.AppendLine($"| {d+1} | {dailyFarm} | {dailyScav} | {net:+0;-0;0} | {missed-lastMissed} | {personal+storage} | {power.Consumed-lastBurn:0.000} | {left:0.000} | {signal.SignalDays:0.000} | {signal.TotalLossDays:0.000} | {(impossible?"Unreachable":"Feasible lower bound")} |");
                lastProduced=produced;lastScavenged=scavenged;lastConsumed=consumed;lastMissed=missed;lastBurn=power.Consumed;
            });
        }
        report.AppendLine($"### Seed {scavenging.Seed}, {count} players, {(interrupted?"full load, 2.5-day outage":waste?"wasteful startup":full?"continuous full load":delayed?"scheduled, delayed care":"scheduled, regular care")}\n");
        report.AppendLine($"Finite Food {scavenging.Sources.Sum(x=>x.Site.Quantity)}, surface Fuel {fuelWorld.Sources.Sum(x=>x.Site.Quantity)} + 2 starter.\n");
        report.AppendLine("| Day | Farm | Field Food | Food supply − need | Missed meals | Food reserve | Fuel burned | Fuel left incl. unpicked | Signal days | Signal lost | Rescue feasibility |\n|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---|");
        while (events.Count > 0 && !s.Rescue().Completed)
        {
            var pair = events.First(); events.Remove(pair.Key);
            c.Advance(pair.Key - c.Snapshot().TotalSeconds);
            if(s.Rescue().Completed)break;
            foreach (var action in pair.Value) action();
        }
        var end=s.Rescue();decimal finalFuel=s.Power().Fuel+s.Snapshot(1).Storage.Count("fuel")+ids.Sum(id=>s.Snapshot(id).Personal.Count("fuel"))+fuelWorld.Sources.Sum(x=>s.Snapshot(1).Remaining[x.Site.Id]);
        Check(waste?!end.Completed:end.Completed||unreachable,"Useful policies rescue or explicitly report Fuel infeasibility");
        if(end.Completed)Check(end.CompletionCount==1&&c.Frozen,"Combined run ends exactly once");
        report.AppendLine($"\nOutcome: {(end.Completed?$"Rescued at D{c.Snapshot().DayNumber} {(long)c.Snapshot().TotalSeconds%600/60:00}:{(long)c.Snapshot().TotalSeconds%60:00}":"No rescue by end of Day 14")}; signal {end.SignalDays:0.000}, lost {end.TotalLossDays:0.000}, Fuel left {finalFuel:0.000}; total missed meals {ids.Sum(id=>s.Hunger(id).MissedDeadlines)}. {(unreachable?"Finite Fuel falls below even beacon-only completion cost: unreachable without a new run or future supplies.":"Fuel sufficient with these collection/refill timings.")}\n");
        report.AppendLine($"Harvest ledger through termination: {string.Join("; ",harvests)}.\n");
        Console.WriteLine($"COMBINED seed={scavenging.Seed} actors={count} full={full} waste={waste} interrupted={interrupted} delayed={delayed}: rescued={end.Completed}, signal={end.SignalDays:0.000}, Fuel={finalFuel:0.000}, unreachable={unreachable}");
    }
}
