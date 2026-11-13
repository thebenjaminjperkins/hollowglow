using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Hollowlight.Core;

public static class PowerTests
{
    public static PowerDefinition Definition => PowerDefinition.Parse(ScavengingTests.Data("power"));
    public static (SharedSupplies S, GameClock C) Create(int players = 2, bool hunger = false)
    {
        var food = ScavengingTests.Generate(1701); var setup = SupplySetup.Parse(ScavengingTests.Data("supplies"));
        var nodes = setup.Sites.Concat(food.Sources.Select(s => s.Site)).Select(s => new ResourceNodeDefinition(s.Id,s.ResourceId,s.Quantity)).ToList();
        nodes.Add(new("fixture-fuel", "fuel", 100));
        var s = new SharedSupplies(setup.Resources.Append(new("fuel","Fuel","Fuel")), nodes, Enumerable.Range(1,players));
        var c = new GameClock(new());
        if (hunger) s.InitializeHunger(new(),c);
        s.InitializePower(Definition,c); return (s,c);
    }
    public static PowerResult Command(SharedSupplies s, PowerAction action, bool enabled = false, string consumer = "", int quantity = 1, int actor = 1) =>
        s.ApplyPower(actor,new(action,s.Power().Version,s.Snapshot(actor).Version,enabled,consumer,quantity));
    public static void Supply(SharedSupplies s, int amount = 10, int actor = 1) => s.Gather(actor,"fixture-fuel",amount,s.Snapshot(actor).Version);
    private static void Check(bool value, string message) { if (!value) throw new Exception(message); }
    public static void Run()
    {
        var (s,c)=Create(hunger:true);
        Check(!s.Power().Running && s.Power().Fuel==0 && s.Snapshot(1).Storage.Count("fuel")==2 && s.Snapshot(1).Personal.Count("food")==2,"Empty stopped generator, two shared Fuel, unchanged personal Food");
        Check(Command(s,PowerAction.SetRunning,true).Status==PowerStatus.NoFuel,"Cannot start empty generator");
        Check(Command(s,PowerAction.LoadFuel).Status==PowerStatus.NoFuel && s.Snapshot(1).Storage.Count("fuel")==2,"No automatic chest refueling");
        Supply(s); Check(Command(s,PowerAction.LoadFuel,quantity:10).Amount==5 && s.Power().Fuel==5 && s.Snapshot(1).Personal.Count("fuel")==5,"Tank accepts only five whole units, remainder personal");
        var token=s.Power().Version;
        Check(Command(s,PowerAction.LoadFuel).Status==PowerStatus.TankFull && s.Power().Version==token,"Full tank changes nothing");
        Check(Command(s,PowerAction.SetConsumer,true,"beacon").Status==PowerStatus.NoSupply,"Start generator before enabling new consumer");
        Command(s,PowerAction.SetRunning,true); Command(s,PowerAction.SetConsumer,true,"beacon"); Command(s,PowerAction.SetConsumer,true,"workbench");
        var before=s.Power(); var stock=s.Snapshot(1).Version;
        var rejected=Command(s,PowerAction.SetConsumer,true,"lighting");
        Check(rejected.Status==PowerStatus.OverCapacity && rejected.RequestedDemand==5 && rejected.Capacity==4 &&
            s.Power().Version==before.Version && s.Power().Running && s.Power().Consumers.Count(x=>x.Powered)==2 && s.Power().Fuel==5,
            "5/4 PU activation rejected without trip, reprioritization or mutation");
        c.Advance(60); Check(s.Power().Fuel==4.9m && s.Power().Consumed==.1m && s.Snapshot(1).Version==stock,"Exact continuous full-load burn; inventory preview remains valid");
        Command(s,PowerAction.SetConsumer,false,"workbench"); Command(s,PowerAction.SetConsumer,true,"lighting");
        Check(s.Power().Demand==3 && s.Power().Available==1 && s.Power().RatePerDay==.8m,"Manual reallocation changes load and rate");
        c.Advance(300); Check(s.Power().Fuel==4.5m && s.Power().Consumed==.5m,"Fractional half-day burn at three PU");
        Check(Command(s,PowerAction.LoadFuel).Status==PowerStatus.TankFull && s.Power().Fuel==4.5m,"Half-unit free space cannot consume a whole Fuel item");
        Command(s,PowerAction.SetRunning,false); c.Advance(1200);
        Check(s.Power().Fuel==4.5m && s.Power().Consumers.All(x=>!x.Powered) && s.Power().Consumers.Count(x=>x.Enabled)==2,"Stop preserves fractional Fuel and switch selections without supplying consumers");
        Command(s,PowerAction.SetRunning,true); c.Advance(60000);
        Check(!s.Power().Running && s.Power().Fuel==0 && s.Power().Consumed==5 && s.Power().Consumers.All(x=>!x.Powered),"Long skip stops exactly at empty, without negative Fuel or rounded loss");
        Check(s.Hunger(1).MissedDeadlines>0,"Meal settlement and power share continuous clock");
        Command(s,PowerAction.LoadFuel); Check(!s.Power().Running && s.Power().Fuel==1,"Refueling never restarts automatically");
        var request=new PowerRequest(PowerAction.SetRunning,s.Power().Version,s.Snapshot(1).Version,true);
        s.ApplyPower(1,request); Check(s.ApplyPower(1,request).Status==PowerStatus.Stale,"Replayed power command rejected");
        Check(JsonSerializer.Deserialize<PowerFuelState>(JsonSerializer.Serialize(s.PowerFuel()))==s.PowerFuel(),"Exact credit counters serialize without engine objects");
        var (jump,jc)=Create(); var (step,sc)=Create();
        foreach(var state in new[]{jump,step}) { Supply(state); Command(state,PowerAction.LoadFuel,quantity:5); Command(state,PowerAction.SetRunning,true); Command(state,PowerAction.SetConsumer,true,"beacon"); }
        jc.Advance(600); for(int i=0;i<36000;i++) sc.Advance(1.0/60);
        Check(jump.PowerFuel()==step.PowerFuel() && jump.Power().Fuel==4.4m,"Frame subdivision preserves exact fractional credits");
        var (idle,ic)=Create(); Supply(idle); Command(idle,PowerAction.LoadFuel); Command(idle,PowerAction.SetRunning,true); ic.Advance(600);
        Check(idle.Power().Fuel==.8m && idle.Power().RatePerDay==.2m,"Idle running generator still burns 0.2 per day");
        var (fresh,_) = Create(); Check(fresh.Power().Consumed==0 && fresh.Power().Demand==0 && fresh.ApplyPower(1,request).Status==PowerStatus.Stale,"Reset clears generator/switches and old session requests");
        for(int n=0;n<64;n++)
        {
            var (race,rc)=Create(); Supply(race,10,1); Supply(race,10,2);
            var r=new PowerRequest(PowerAction.LoadFuel,race.Power().Version,race.Snapshot(1).Version,Quantity:5);
            PowerResult? a=null,b=null;
            Parallel.Invoke(()=>a=race.ApplyPower(1,r),()=>b=race.ApplyPower(2,r));
            Check(new[]{a!,b!}.Count(x=>x.Status==PowerStatus.Applied)==1 && race.Power().Fuel==5 &&
                race.Snapshot(1).Personal.Count("fuel")+race.Snapshot(2).Personal.Count("fuel")==15,"Concurrent loading has one winner and conserves units");
            Command(race,PowerAction.SetRunning,true); Command(race,PowerAction.SetConsumer,true,"beacon");
            var rv=race.Power().Version;
            Parallel.Invoke(()=>race.ApplyPower(1,new(PowerAction.SetConsumer,rv,race.Snapshot(1).Version,true,"workbench")),
                ()=>race.ApplyPower(2,new(PowerAction.SetConsumer,rv,race.Snapshot(2).Version,true,"lighting")));
            Check(race.Power().Demand<=4 && race.Power().Consumers.Single(x=>x.Id=="beacon").Powered,"Conflicting allocations never overload or shut down existing beacon");
            rc.Advance(60000);
            var fuel=race.PowerFuel(); Check(fuel.TankCredits+fuel.ConsumedCredits==5*fuel.CreditsPerFuel,"Exact Fuel conservation after concurrent control and exhaustion");
        }
        for(int n=0;n<32;n++)
        {
            var (racer,time)=Create(); Supply(racer,2); Command(racer,PowerAction.LoadFuel);Command(racer,PowerAction.SetRunning,true);Command(racer,PowerAction.SetConsumer,true,"beacon");
            var load=new PowerRequest(PowerAction.LoadFuel,racer.Power().Version,racer.Snapshot(1).Version);
            Parallel.Invoke(()=>racer.ApplyPower(1,load),()=>time.Advance(1000));
            var credits=racer.PowerFuel();
            Check(credits.TankCredits+credits.ConsumedCredits+racer.Snapshot(1).Personal.Count("fuel")*credits.CreditsPerFuel==2*credits.CreditsPerFuel,
                "Refuel versus exhaustion conserves every credit/item regardless of ordering");
            var (transfer,_) = Create();Supply(transfer,1);load=new(PowerAction.LoadFuel,transfer.Power().Version,transfer.Snapshot(1).Version);
            Parallel.Invoke(()=>transfer.ApplyPower(1,load),()=>transfer.DepositAll(1,load.InventoryVersion));
            Check(transfer.Power().Fuel+transfer.Snapshot(1).Personal.Count("fuel")+transfer.Snapshot(1).Storage.Count("fuel")==3,
                "Refuel versus storage transfer cannot spend or move the same unit twice");
        }
        Console.WriteLine("PASS: power capacity rejection, exact fractional/idle/load burn, tank limits, personal loading, no auto refill/restart, clock subdivision/skips, reset and 64 concurrent load/allocation races");
    }
}
