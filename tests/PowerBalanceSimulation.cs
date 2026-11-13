using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Hollowlight.Core;

public static class PowerBalanceSimulation
{
    private static void Check(bool value,string message) {if(!value)throw new Exception(message);}
    public static void Run(string output)
    {
        int minSeed=Enumerable.Range(0,4096).First(seed=>FuelPlacementTests.Generate(seed).Sources.Sum(x=>x.Site.Quantity)==5);
        int maxSeed=Enumerable.Range(0,4096).First(seed=>FuelPlacementTests.Generate(seed).Sources.Sum(x=>x.Site.Quantity)==8);
        var report=new StringBuilder("# M4a seven-day Fuel balance\n\n");
        report.AppendLine("Reproduce: `dotnet run --project tests/Hollowlight.Core.Tests.csproj -- --power-balance docs/M4A_FUEL_BALANCE.md`\n");
        report.AppendLine("Uses actual seeded near/far Fuel caches, authoritative gathering/storage/refueling, the shared clock and exact generator credits. Two shared starter Fuel; empty five-Fuel tank; four-PU capacity. No player-count scaling. This isolates power logistics rather than resimulating Food or farming; existing food ledgers remain applicable without changing their values.\n");
        report.AppendLine("A scavenger returns the near cache on Day 2 at 03:00 and the far cache on Day 4 at 03:00. Fuel is gathered into personal inventory, deposited in communal storage, explicitly withdrawn by the operator and manually loaded. Whole units that cannot fit stay in inventory and are deposited. No automatic refueling is simulated. Travel schedules are conservative fixture assumptions on the compact map, not measured group playtests.\n");
        report.AppendLine("Two policies: **scheduled** keeps the beacon on, runs the workbench for the first minute of each day, and lighting during the three-minute Night (no overlap with workbench). **full** runs beacon + workbench continuously; lighting stays off. Generator burn is 0.2 + 0.2 × supplied PU per complete ten-minute cycle. Scheduled costs 0.7/day = 4.9/week; full costs 1/day = 7/week.\n");
        foreach(int seed in new[]{minSeed,maxSeed}.Distinct())
        foreach(bool full in new[]{false,true})
        {
            report.AppendLine($"## Seed {seed}, surface Fuel {FuelPlacementTests.Generate(seed).Sources.Sum(x=>x.Site.Quantity)}, {(full?"continuous full load":"scheduled load")}\n");
            foreach(int count in new[]{2,4,6})Simulate(seed,count,full,report);
        }
        report.AppendLine("## Interpretation\n");
        report.AppendLine("Shared generation is independent of player count. Both minimum and maximum cache worlds meet these seven-day schedules with the planned return/refill times. The poorest world ends full-load service exactly empty at Day 8 dawn, with no buffer for delayed scavenging. Scheduled service leaves 2.1–5.1 Fuel across world extremes; full service leaves 0–3. Food still costs 14/28/42 units for 2/4/6 players independently.\n");
        report.AppendLine("Starting Fuel alone supports only two full-load days (or about 2.86 scheduled days). Surface collection is necessary for a powered week. Near Fuel extends the window, while full service requires the far cache. Disabling consumers saves Fuel; stopping the generator saves idle burn as well. There is no battery, second generator, crop-power requirement, crafting production or rescue-progress simulation in M4a.\n");
        File.WriteAllText(output,report.ToString().TrimEnd()+"\n");
        Console.WriteLine("PASS: actual 5/8-Fuel worlds, seven-day exact daily power balances for 2/4/6 actors, manual transfers/refills and scheduled/full service");
    }
    private static void Simulate(int seed,int players,bool full,StringBuilder report)
    {
        var world=FuelPlacementTests.Generate(seed);var supply=SupplySetup.Parse(ScavengingTests.Data("supplies"));
        var nodes=supply.Sites.Concat(world.Sources.Select(x=>x.Site)).Select(x=>new ResourceNodeDefinition(x.Id,x.ResourceId,x.Quantity));
        var s=new SharedSupplies(supply.Resources.Append(new("fuel","Fuel","Fuel")),nodes,Enumerable.Range(1,players));
        var c=new GameClock(new());s.InitializePower(PowerTests.Definition,c);
        var events=new SortedDictionary<int,List<Action>>();
        void At(int seconds,Action action) {if(!events.TryGetValue(seconds,out var list))events[seconds]=list=new();list.Add(action);}
        void Set(string consumer,bool enabled)
        {
            var r=PowerTests.Command(s,PowerAction.SetConsumer,enabled,consumer);
            Check(r.Status is PowerStatus.Applied or PowerStatus.NoChange,"Scheduled manual consumer change fits generation");
        }
        void Refill()
        {
            var version=s.Snapshot(1).Version;int available=s.Snapshot(1).Storage.Count("fuel");
            if(available>0)Check(s.Transfer(1,"fuel",available,true,version).Amount==available,"Manual withdrawal fits operator pack");
            PowerTests.Command(s,PowerAction.LoadFuel,quantity:int.MaxValue);
            s.DepositAll(1,s.Snapshot(1).Version);
        }
        Refill();Check(PowerTests.Command(s,PowerAction.SetRunning,true).Status==PowerStatus.Applied,"Manual initial start");Set("beacon",true);
        if(full)Set("workbench",true);
        foreach(var source in world.Sources)
        {
            int time=source.Band=="near"?780:1980;
            At(time,()=> {
                Check(s.Gather(2,source.Site.Id,int.MaxValue,s.Snapshot(2).Version).Amount==source.Site.Quantity,"Exactly one finite Fuel collection");
                s.DepositAll(2,s.Snapshot(2).Version);Refill();
            });
        }
        decimal previous=0;
        report.AppendLine($"### {players} players\n");
        report.AppendLine("| Completed day | Burn today | Burn to date | Tank | Stored/carried Fuel | Unpicked Fuel | Service |\n|---:|---:|---:|---:|---:|---:|:---|");
        for(int day=0;day<7;day++)
        {
            int d=day;
            if(!full)
            {
                At(day*600,()=>{Set("lighting",false);Set("workbench",true);});
                At(day*600+60,()=>Set("workbench",false));
                At(day*600+420,()=>Set("lighting",true));
            }
            At((day+1)*600,()=> {
                var p=s.Power();var f=s.PowerFuel();
                int items=Enumerable.Range(1,players).Sum(id=>s.Snapshot(id).Personal.Count("fuel"))+s.Snapshot(1).Storage.Count("fuel");
                int unpicked=world.Sources.Sum(x=>s.Snapshot(1).Remaining[x.Site.Id]);
                Check(f.TankCredits+f.ConsumedCredits+(items+unpicked)*f.CreditsPerFuel==(world.Sources.Sum(x=>x.Site.Quantity)+2)*f.CreditsPerFuel,"Daily exact whole-item + fractional Fuel conservation");
                decimal expected=(d+1)*(full?1m:.7m);
                Check(p.Consumed==expected,"Every day's service consumes its exact configured Fuel");
                Check(p.Running||d==6&&p.Fuel==0,"No premature service outage");
                report.AppendLine($"| {d+1} | {p.Consumed-previous:0.0} | {p.Consumed:0.0} | {p.Fuel:0.0} | {items} | {unpicked} | {(p.Running?"Running":"Empty at final boundary")} |");previous=p.Consumed;
            });
        }
        while(events.Count>0)
        {
            var pair=events.First();events.Remove(pair.Key);c.Advance(pair.Key-c.Snapshot().TotalSeconds);
            foreach(var action in pair.Value)action();
        }
        Console.WriteLine($"POWER BALANCE seed {seed}/{players} actors/{(full?"full":"scheduled")}: consumed {s.Power().Consumed}, tank {s.Power().Fuel}");
        report.AppendLine();
    }
}
