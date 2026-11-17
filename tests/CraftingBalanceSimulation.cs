using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using Hollowlight.Core;

public static class CraftingBalanceSimulation
{
    private static void Check(bool value,string message){if(!value)throw new Exception(message);}
    private static string Data(string name)=>ScavengingTests.Data(name);
    public static void Run(string output)
    {
        var report=new StringBuilder("# M5a revised-cost seven-day investment balance\n\n");
        report.AppendLine("Reproduce: `dotnet run --project tests/Hollowlight.Core.Tests.csproj -- --crafting-balance docs/M5A_CRAFTING_BALANCE.md`\n");
        report.AppendLine("Before implementation, exact budget enumeration confirmed the revised costs allow one backpack plus a working bed (20 Scrap, 8 Organic), but two backpacks plus bed/propagation require 28 Scrap, above the world's finite 26. No new design conflict: the choice is group distribution versus production, rather than exclusive recipe categories. Two actors can outfit both instead of expanding; four/six can outfit at most three, with the third waiting for renewable Organic.\n");
        report.AppendLine("This post-implementation replay uses actual authoritative Craft/Install/Plant/Harvest/Gather/Transfer/Power/Hunger/Rescue operations, unchanged seeded Food/Fuel worlds, four starting plots, two seeds per crop, two starting Food per actor and two shared Fuel. No injected resources, altered source quantities or changed growth/yields. All inputs are explicitly withdrawn into the operator's backpack. Kits for other actors pass through the chest. Conservation includes materials intentionally consumed and the exact net seed created by propagation.\n");
        report.AppendLine("Timing: original farm plants D1 01:00/01:10/01:20/01:30; visits at 03:00/05:30/08:30 (delayed omits 05:30), ten seconds per plot, replant five seconds later. Near Food returns D1 02:00, far D3 05:00; original material nodes gathered D1 02:30/02:40/02:50 and deposited at 03:30. Backpack workshop visit 03:30–04:00; second kit visit 04:10–04:40. Expansion policy propagates the first returned potato seed D3 03:10, replants the original at 03:35 and plants the fifth at 04:00. Backpack-only groups of 4/6 craft the third after Glowcaps, D4 03:50–04:20. Each powered visit lasts thirty seconds; crafting itself is immediate. These are reproducible compact-map cooperation assumptions, not measured networked expeditions.\n");
        report.AppendLine("Power retains M4b's scheduled policy (beacon continuously, workbench seconds 30–60/day, lighting during Night), plus the explicit extra visits above. Near Fuel returns D2 03:00, far D4 03:00, manual refills daily 03:00 and on deliveries. Rows settle at dawn; simulation continues beyond Day 7 to the exact seven-signal-day victory. No automatic resource pooling/refueling or priorities.\n");
        foreach(int seed in new[]{499,1701,8})foreach(int count in new[]{2,4,6})foreach(bool expand in new[]{false,true})
            Simulate(count,false,report,ScavengingTests.Generate(seed),expand);
        foreach(int count in new[]{2,4,6})foreach(bool expand in new[]{false,true})Simulate(count,true,report,ScavengingTests.Generate(499),expand);
        report.AppendLine("## Findings\n\nThe revised costs permit one backpack and farming together, but not two backpacks and a farm expansion. Day-1 Organic pays exactly for the mixed choice. Maximum backpack-only counts are 2/3/3 for groups of 2/4/6; the third uses Organic from a Glowcap harvest. No stock scales with players. A fifth potato planted D3 04:00 yields six extra Food by Day 7 with regular care, or three with delayed care. In the minimum Food world, six actors with delayed care change from two missed meals to none and one Food left. Original crop yields and growth are unchanged. Food surpluses for smaller groups make carrying capacity a more plausible alternative, although the compact map limits the strength of the backpack benefit.\n\nAll tested policies reach rescue. Fuel remains finite: unnecessary full-load operation and outages retain the M4b unreachable-rescue cases. Crafting does not introduce extra Fuel or an instant Fuel fee; thirty powered seconds cost 0.02 extra Fuel at the existing load formula. These simulations do not establish human group balance or physical/network multiplayer.\n");
        File.WriteAllText(output,report.ToString().TrimEnd()+"\n");
        Console.WriteLine("PASS: revised 2/4/6-player crafting investments, timed expanded farm, material/seed/Food/Fuel conservation and rescue completion");
    }
    public static void RunRegion(string output,string measurements)
    {
        var report=new StringBuilder("# M6a seven-day geography / economy replay\n\n");
        report.AppendLine("Uses the unchanged authoritative M5a model and recipes, with actual Godot physics travel measurements for seeds 499, 1701 and 8. Two roles: one sanctuary operator/gardener, one sequential scavenger; additional actors consume Food normally. The scavenger starts near Food at D1 00:00, returns/deposits after each source, then retrieves the three material nodes individually. Near Fuel departs D2 03:00; far Food departs D3 00:00, sequentially; far Fuel departs D4 03:00. Collection occurs at measured outbound time, deposits at measured round-trip time. Each measured trip includes two seconds at the source and two at storage. Timings are rounded up to whole seconds. No teleport or extra stock in the economy replay.\n");
        report.AppendLine("Sanctuary duties retain the documented M5a ten-second plot spacing and twice/three-times-daily visits, with thirty-second workshop visits. Actual storage/workshop/generator/garden round trips are measured separately and fit these allowances. Crafting begins ten seconds after all materials return, then forty seconds later for the second kit. The operator explicitly turns lighting off for workshop visits and restores it at Night; capacity rules are unchanged. The scavenger skips shared resupply while away and explicitly withdraws on return. Other actors remain at the sanctuary; manual refueling retains M5a assumptions. These are coordinated core simulations, not network multiplayer or human navigation results.\n");
        foreach(int seed in new[]{499,1701,8})
        {
            using var json=JsonDocument.Parse(File.ReadAllText(Path.Combine(measurements,$"travel-{seed}.json")));
            var trips=json.RootElement.GetProperty("Trips").EnumerateArray().ToDictionary(t=>t.GetProperty("Source").GetString()!,t=>new Travel((int)Math.Ceiling(t.GetProperty("OutboundFromChestSeconds").GetDouble()),(int)Math.Ceiling(t.GetProperty("RoundTripWithInteractionsSeconds").GetDouble())));
            foreach(int count in new[]{2,4,6})foreach(bool delayed in new[]{false,true})foreach(bool expand in new[]{false,true})
                Simulate(count,delayed,report,ScavengingTests.Generate(seed),expand,trips);
        }
        report.AppendLine("## Interpretation\n\nDaily Food supply minus need is a flow, while reserve includes carried-over supplies; negative daily flow need not mean missed meals. Finite Fuel and resource budgets are unchanged. Each scenario asserts exact conservation, available crafting costs, unchanged original crop yields, and rescue completion. The six-player minimum-Food delayed-care case remains the tightest. Human detours, missed deposits or running the workshop unnecessarily can worsen these results; no economy correction is applied by M6a.\n");
        File.WriteAllText(output,report.ToString());Console.WriteLine("PASS: measured M6a expeditions replayed for 2/4/6 actors, all three seeds, regular/delayed care, conserved Food/materials/Fuel and rescue");
    }
    private record Travel(int Outbound,int RoundTrip);
    private static void Simulate(int count, bool delayed, StringBuilder report, SourceWorld scavenging, bool expand,System.Collections.Generic.Dictionary<string,Travel>? travel=null)
    {
        var crafting=CraftingSetup.Parse(Data("crafting"));
        var ids = Enumerable.Range(1, count).ToArray(); var setup = AgricultureSetup.Parse(Data("farming"));
        var supplySetup = SupplySetup.Parse(Data("supplies"));
        supplySetup = supplySetup with { Sites = supplySetup.Sites.Concat(scavenging.Sources.Select(s => s.Site)).ToArray() };
        var fuelWorld = FuelPlacementTests.Generate(scavenging.Seed);
        supplySetup = supplySetup with { Sites = supplySetup.Sites.Concat(fuelWorld.Sources.Select(x=>x.Site)).ToArray() };
        var s = supplySetup.CreateState(ids, 24, setup.Resources.Concat(crafting.Resources).Append(new ResourceDefinition("fuel","Fuel","Fuel"))); var c = new GameClock(new());
        var plots = expand?new[] {"a","b","c","d",SharedSupplies.ExpansionPlotId}:new[] {"a","b","c","d"}; s.InitializeAgriculture(setup, c, plots.Take(4));
        s.InitializeHunger(HungerDefinition.Parse(Data("hunger")), c);
        s.InitializePower(PowerDefinition.Parse(Data("power")),c);
        s.InitializeRescue(RescueDefinition.Parse(Data("rescue")));
        s.InitializeCrafting(crafting);
        var events = new SortedDictionary<int, List<Action>>();
        int scavenged = 0, lastProduced = 0, lastScavenged = 0; long lastConsumed = 0, lastMissed = 0;
        bool scavengerAway=false;
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
                    if(travel is not null&&id==2&&scavengerAway)continue;
                    int need = Math.Max(0, round - s.Snapshot(id).Personal.Count("food"));
                    int amount = Math.Min(need, s.Snapshot(id).Storage.Count("food"));
                    Transfer(id, "food", amount, true);
                }
        }
        int materialReturn=210;
        if(travel is null)
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
                At(returned, () => {Transfer(2, "food", delivered, false); scavenged += delivered;});
            }
        }
        else
        {
            int cursor=0;
            foreach(var node in scavenging.Sources.Where(x=>x.Band=="near"))cursor=FieldTrip(node,cursor);
            foreach(var site in SupplySetup.Parse(Data("supplies")).Sites)
            {
                var duration=travel[site.Id];int departure=cursor;
                At(departure,()=>scavengerAway=true);
                At(departure+duration.Outbound,()=>Check(s.Gather(2,site.Id,int.MaxValue,s.Snapshot(2).Version).Amount==site.Quantity,"Measured material collection fits"));
                cursor+=duration.RoundTrip;int returnDay=cursor/600;At(cursor,()=>{Transfer(2,site.ResourceId,site.Quantity,false);scavengerAway=false;Resupply(returnDay);});
            }
            materialReturn=cursor;
            cursor=1200;foreach(var node in scavenging.Sources.Where(x=>x.Band=="far"))cursor=FieldTrip(node,cursor);
        }
        int FieldTrip(GeneratedSourceSite node,int departure)
        {
            var duration=travel![node.Site.Id];
            At(departure,()=>scavengerAway=true);
            At(departure+duration.Outbound,()=>Check(s.Gather(2,node.Site.Id,int.MaxValue,s.Snapshot(2).Version).Amount==node.Site.Quantity,"Measured Food collection fits"));
            int returned=departure+duration.RoundTrip;
            At(returned,()=>{Transfer(2,"food",node.Site.Quantity,false);scavenged+=node.Site.Quantity;scavengerAway=false;Resupply(returned/600);});return returned;
        }

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
            if(!s.Power().Running && s.Power().Fuel>0)Command(PowerAction.SetRunning,true);
        }
        foreach(var source in fuelWorld.Sources)
        {
            int departure=source.Band=="near"?780:1980;
            int time=travel is null?departure:departure+travel[source.Site.Id].RoundTrip;
            if(travel is not null)At(departure,()=>scavengerAway=true);
            At(travel is null?time-30:departure+travel[source.Site.Id].Outbound,()=>Check(s.Gather(2,source.Site.Id,int.MaxValue,s.Snapshot(2).Version).Amount==source.Site.Quantity,"Actual finite Fuel collection fits"));
            At(time,()=>{Transfer(2,"fuel",source.Site.Quantity,false);if(travel is not null){scavengerAway=false;Resupply(time/600);}Refill();});
        }
        At(30,()=>{Refill();Command(PowerAction.SetConsumer,true,"beacon");});
        // Explicit operator visits, never automatic tank topping by the game.
        for(int day=1;day<14;day++)At(day*600+180,()=>{Refill();Command(PowerAction.SetConsumer,true,"beacon");});
        for(int day=0;day<14;day++)
        {
            At(day*600+30,()=>{Command(PowerAction.SetConsumer,false,"lighting");Command(PowerAction.SetConsumer,true,"workbench");});
            At(day*600+60,()=>Command(PowerAction.SetConsumer,false,"workbench"));
            At(day*600+420,()=>Command(PowerAction.SetConsumer,true,"lighting"));
            At(day*600+600,()=>Command(PowerAction.SetConsumer,false,"lighting"));
        }

        int scrapSpent=0,organicSpent=0,propagations=0,bagsInstalled=0;
        var material=SupplySetup.Parse(Data("supplies")).Sites;
        if(travel is null)
        {
        for(int i=0;i<material.Length;i++)
        {
            var node=material[i];At(150+i*10,()=>Check(s.Gather(2,node.Id,int.MaxValue,s.Snapshot(2).Version).Amount==node.Quantity,"Finite material trip fits six-slot scavenger"));
        }
        At(210,()=>{Transfer(2,"scrap",26,false);Transfer(2,"organic",8,false);});
        }
        void Make(string recipeId,int recipient=1)
        {
            var recipe=crafting.Recipes.Single(r=>r.Id==recipeId);
            foreach(var input in recipe.Inputs)
            {
                int need=Math.Max(0,input.Value-s.Snapshot(1).Personal.Count(input.Key));Transfer(1,input.Key,need,true);
            }
            Check(s.Craft(1,new(recipeId,s.Snapshot(1).Version,s.Power().Version)).Status==CraftStatus.Applied,"Scheduled powered recipe fits and uses personal materials");
            scrapSpent+=recipe.Inputs.GetValueOrDefault("scrap");organicSpent+=recipe.Inputs.GetValueOrDefault("organic");
            if(recipeId=="potato_propagation"){propagations++;return;}
            if(recipeId=="growing_bed")
            {
                Check(s.InstallBed(1,new(CraftingTests.Slot(s,"bed_kit"),s.Snapshot(1).Version)).Status==CraftStatus.Applied,"Shared fifth plot installed from crafted kit");return;
            }
            if(recipient!=1){Transfer(1,"backpack_kit",1,false);Transfer(recipient,"backpack_kit",1,true);}
            Check(s.InstallBackpack(recipient,new(CraftingTests.Slot(s,"backpack_kit",recipient),s.Snapshot(recipient).Version)).Status==CraftStatus.Applied,"Personal recipient installs transferable kit");bagsInstalled++;
        }
        void Workshop(int start,string recipe,int recipient=1)
        {
            At(start,()=>{if(travel is not null)Command(PowerAction.SetConsumer,false,"lighting");Command(PowerAction.SetConsumer,true,"workbench");});
            At(start+10,()=>Make(recipe,recipient));At(start+30,()=>{Command(PowerAction.SetConsumer,false,"workbench");if(travel is not null&&(start+30)%600>=420)Command(PowerAction.SetConsumer,true,"lighting");});
        }
        Workshop(travel is null?210:materialReturn+10,"backpack_extension");Workshop(travel is null?250:materialReturn+50,expand?"growing_bed":"backpack_extension",expand?1:2);
        if(expand)
        {
            Workshop(1380,"potato_propagation");
            At(1440,()=>Check(s.Plant(1,SharedSupplies.ExpansionPlotId,"cave_potato",s.Plot(SharedSupplies.ExpansionPlotId).Version).Status==FarmStatus.Planted,"Fifth plot planted D3 04:00 after returned-seed propagation"));
        }
        else if(count>2)Workshop(2030,"backpack_extension",3);
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
                for (int i = 0; i < plots.Length; i++)
                {
                    int p = i; int harvestTime = time + i * 10;
                    At(harvestTime, () => {
                        if(p==4&&!s.CraftingFor(1).BedInstalled)return;
                        var plot = s.Plot(plots[p]); if (plot.Stage != CropStage.Mature) return;
                        var crop = s.Crops.First(x => x.Id == plot.CropId);
                        Check(s.Harvest(1, plots[p], plot.Version).Status == FarmStatus.Harvested, "Timed complete harvest fits gardener pack");
                        int food = crop.Yields["food"]; produced += food; organic += crop.Yields.GetValueOrDefault("organic");
                        harvests.Add($"D{harvestTime / 600 + 1} {harvestTime % 600 / 60:00}:{harvestTime % 60:00} {crop.Name} +{food}");
                        Transfer(1, "food", food, false);
                        int extra = crop.Yields.GetValueOrDefault("organic"); Transfer(1, "organic", extra, false);
                        At(harvestTime + (expand&&p==0&&harvestTime==1380?35:5), () => Check(s.Plant(1, plots[p], crop.Id, s.Plot(plots[p]).Version).Status == FarmStatus.Planted, "Replant returned seed"));
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
                int Amount(string resource)=>ids.Sum(id=>s.Snapshot(id).Personal.Count(resource))+s.Snapshot(1).Storage.Count(resource);
                Check(Amount("scrap")+scrapSpent+material.Where(n=>n.ResourceId=="scrap").Sum(n=>s.Snapshot(1).Remaining[n.Id])==26,"Daily material Scrap conservation including crafted investment");
                Check(Amount("organic")+organicSpent+material.Where(n=>n.ResourceId=="organic").Sum(n=>s.Snapshot(1).Remaining[n.Id])==8+organic,"Daily Organic conservation including crop yields and recipe costs");
                Check(Amount("potato_seed")+s.Plots().Count(p=>p.CropId=="cave_potato")==2+propagations,"Seed conservation allows only explicitly crafted propagation");
                int dailyFarm=produced-lastProduced,dailyScav=scavenged-lastScavenged;
                int net=(d==0?count*2:0)+dailyFarm+dailyScav-count;
                var power=s.Power();var signal=s.Rescue();var fuel=s.PowerFuel();
                int items=ids.Sum(id=>s.Snapshot(id).Personal.Count("fuel"))+s.Snapshot(1).Storage.Count("fuel");
                int unpicked=fuelWorld.Sources.Sum(x=>s.Snapshot(1).Remaining[x.Site.Id]);
                Check(fuel.TankCredits+fuel.ConsumedCredits+(items+unpicked)*fuel.CreditsPerFuel==(2+fuelWorld.Sources.Sum(x=>x.Site.Quantity))*fuel.CreditsPerFuel,"Daily combined Fuel conservation");
                decimal left=power.Fuel+items+unpicked;
                bool impossible=left<(signal.RequiredDays-signal.SignalDays)*.6m;
                unreachable|=impossible;
                if(d<7)report.AppendLine($"| {d+1} | {dailyFarm} | {dailyScav} | {net:+0;-0;0} | {missed-lastMissed} | {personal+storage} | {power.Consumed-lastBurn:0.000} | {left:0.000} | {signal.SignalDays:0.000} | {signal.TotalLossDays:0.000} | {Amount("scrap")} / {Amount("organic")} | {bagsInstalled} / {s.Plots().Count} | {(impossible?"Unreachable":"Feasible lower bound")} |");
                lastProduced=produced;lastScavenged=scavenged;lastConsumed=consumed;lastMissed=missed;lastBurn=power.Consumed;
            });
        }
        report.AppendLine($"### Seed {scavenging.Seed}, {count} players, {(expand?"one backpack + fifth plot + propagation":"backpacks only")}, {(delayed?"delayed":"regular")} care\n");
        report.AppendLine("| Day | Farm Food | Field Food | Food supply − need | Missed meals | Food reserve | Fuel burned | Fuel left incl. unpicked | Signal days | Signal lost | Available Scrap / Organic | Installed packs / plots | Rescue feasibility |\n|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---|:---|:---|");
        while (events.Count > 0 && !s.Rescue().Completed)
        {
            var pair = events.First(); events.Remove(pair.Key);
            c.Advance(pair.Key - c.Snapshot().TotalSeconds);
            if(s.Rescue().Completed)break;
            foreach (var action in pair.Value) action();
        }
        var end=s.Rescue();decimal finalFuel=s.Power().Fuel+s.Snapshot(1).Storage.Count("fuel")+ids.Sum(id=>s.Snapshot(id).Personal.Count("fuel"))+fuelWorld.Sources.Sum(x=>s.Snapshot(1).Remaining[x.Site.Id]);
        Check(end.Completed&&!unreachable,"Tested M5a investment policies preserve achievable rescue");
        Check(scrapSpent==(expand?20:Math.Min(count,3)*8)&&organicSpent==(expand?8:Math.Min(count,3)*3),"Actual crafting uses revised approved costs");
        Check(produced==(expand?delayed?23:32:delayed?20:26)&&bagsInstalled==(expand?1:Math.Min(count,3)),"Timed farm production and group affordability match the approved model");
        if(end.Completed)Check(end.CompletionCount==1&&c.Frozen,"Combined run ends exactly once");
        report.AppendLine($"\nOutcome: {(end.Completed?$"Rescued at D{c.Snapshot().DayNumber} {(long)c.Snapshot().TotalSeconds%600/60:00}:{(long)c.Snapshot().TotalSeconds%60:00}":"No rescue by end of Day 14")}; signal {end.SignalDays:0.000}, lost {end.TotalLossDays:0.000}, Fuel left {finalFuel:0.000}; total missed meals {ids.Sum(id=>s.Hunger(id).MissedDeadlines)}. {(unreachable?"Finite Fuel falls below even beacon-only completion cost: unreachable without a new run or future supplies.":"Fuel sufficient with these collection/refill timings.")}\n");
        report.AppendLine($"Harvest ledger through termination: {string.Join("; ",harvests)}.\n");
        Console.WriteLine($"COMBINED seed={scavenging.Seed} actors={count} expansion={expand} delayed={delayed}: rescued={end.Completed}, signal={end.SignalDays:0.000}, Fuel={finalFuel:0.000}, unreachable={unreachable}");
    }}
