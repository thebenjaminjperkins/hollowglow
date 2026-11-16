using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hollowlight.Core;

public static class RescueTests
{
    public static (SharedSupplies S,GameClock C) Create(RescueDefinition? definition=null)
    {
        var setup=SupplySetup.Parse(ScavengingTests.Data("supplies"));var farm=AgricultureSetup.Parse(ScavengingTests.Data("farming"));
        var resources=setup.Resources.Concat(farm.Resources).Append(new ResourceDefinition("fuel","Fuel","Fuel"));
        var nodes=setup.Sites.Select(n=>new ResourceNodeDefinition(n.Id,n.ResourceId,n.Quantity)).Append(new ResourceNodeDefinition("fixture-fuel","fuel",100));
        var s=new SharedSupplies(resources,nodes,new[]{1,2});var c=new GameClock(new());
        s.InitializeAgriculture(farm,c,new[]{"a","b","c","d"});s.InitializeHunger(new(),c);s.InitializePower(PowerTests.Definition,c);
        s.InitializeRescue(definition??RescueDefinition.Parse(ScavengingTests.Data("rescue")));return(s,c);
    }
    public static void Start(SharedSupplies s,int fuel=5)
    {
        s.Gather(1,"fixture-fuel",fuel,s.Snapshot(1).Version);
        PowerTests.Command(s,PowerAction.LoadFuel,quantity:fuel);PowerTests.Command(s,PowerAction.SetRunning,true);PowerTests.Command(s,PowerAction.SetConsumer,true,"beacon");
    }
    private static void Check(bool value,string message){if(!value)throw new Exception(message);}
    public static void Run()
    {
        var(s,c)=Create();c.Advance(6000);
        Check(s.Rescue().Status==SignalStatus.Offline&&s.Rescue().SignalDays==0&&s.Rescue().TotalLossDays==0&&s.Rescue().CurrentOutageTicks==0,"Startup delay causes no penalty or accidental rescue progress");
        Start(s);c.Advance(600);Check(s.Rescue().SignalDays==1,"Continuous transmission earns a complete elapsed signal day");
        PowerTests.Command(s,PowerAction.SetConsumer,false,"beacon");c.Advance(150);
        Check(s.Rescue().SignalDays==1&&s.Rescue().TotalLossDays==0,"Full quarter-day outage grace preserves progress");
        c.Advance(450);Check(s.Rescue().SignalDays==.8125m,"One-day outage loses only 0.1875 signal days after grace");
        PowerTests.Command(s,PowerAction.SetConsumer,true,"beacon");
        Check(s.Rescue().Status==SignalStatus.Transmitting&&s.Rescue().CurrentOutageTicks==0,"Restoration resumes status immediately");
        c.Advance(60);var before=s.Rescue();PowerTests.Command(s,PowerAction.SetConsumer,false,"beacon");c.Advance(60);
        Check(s.Rescue().SignalDays==before.SignalDays-.025m&&s.Rescue().GraceRemainingTicks==0,"Short restorations do not renew grace; subsequent outage incurs decay");
        PowerTests.Command(s,PowerAction.SetConsumer,true,"beacon");c.Advance(150);
        Check(s.Rescue().EpisodeOutageTicks==0&&s.Rescue().EpisodeLossDays==0&&s.Rescue().GraceRemainingTicks==150*TimeSpan.TicksPerSecond,
            "Quarter-day stable transmission renews grace and loss cap");
        var(cap,cc)=Create();Start(cap);cc.Advance(1800);PowerTests.Command(cap,PowerAction.SetRunning,false);cc.Advance(6000);
        Check(cap.Rescue().SignalDays==2&&cap.Rescue().EpisodeLossDays==1&&cap.Rescue().LossCapped,"Ten-day continuous outage loses at most one signal day, without full reset");
        cc.Advance(6000);Check(cap.Rescue().SignalDays==2,"Further outage beyond cap cannot destroy more progress");
        var(zero,zc)=Create();Start(zero);zc.Advance(60);PowerTests.Command(zero,PowerAction.SetRunning,false);zc.Advance(6000);
        Check(zero.Rescue().SignalDays==0&&zero.Rescue().TotalLossDays==.1m,"Loss cannot reduce signal below zero");
        var(jump,jc)=Create();var(step,sc)=Create();Start(jump,1);Start(step,1);
        jc.Advance(1200);for(int i=0;i<1200;i++)sc.Advance(1);
        Check(jump.Rescue()==step.Rescue()&&jump.Rescue().SignalDays==987.5m/600&&jump.Power().Fuel==0,
            "Time skip credits exactly 1000 powered seconds, then 200 outage seconds and grace-adjusted loss");
        jump.Gather(1,"fixture-fuel",1,jump.Snapshot(1).Version);PowerTests.Command(jump,PowerAction.LoadFuel);PowerTests.Command(jump,PowerAction.SetRunning,true);jc.Advance(150);
        Check(jump.Rescue().Status==SignalStatus.Transmitting&&jump.Rescue().EpisodeOutageTicks==0,"Fuel restoration recovers transmission and stable grace naturally");
        var(win,wc)=Create();Start(win);int events=0;win.RescueCompleted+=()=>Interlocked.Increment(ref events);
        win.Transfer(1,"potato_seed",1,true,win.Snapshot(1).Version);win.Plant(1,"a","cave_potato",win.Plot("a").Version);
        wc.Advance(99999);
        Check(win.Rescue().Completed&&win.Rescue().SignalDays==7&&events==1&&wc.ElapsedTicks==4200L*TimeSpan.TicksPerSecond&&wc.Frozen,
            "Large skip stops the shared run exactly at seven signal days, with one completion event");
        Check(win.Power().Consumed==4.2m&&win.Power().Fuel==.8m,"Fuel after victory reflects only the pre-victory powered interval");
        var saved=wc.SaveState();var stock=win.Snapshot(1);var power=win.PowerFuel();var meal=win.Hunger(1);wc.Advance(99999);
        Check(wc.SaveState()==saved&&win.PowerFuel()==power&&win.Hunger(1)==meal&&events==1,"Completed run freezes clock, Fuel, meals and rescue on repeated advances");
        Check(win.Gather(1,"radio-shell",1,stock.Version).Status==SupplyStatus.AccessDenied&&
            win.DepositAll(1,stock.Version).Status==SupplyStatus.AccessDenied&&
            win.Transfer(1,"fuel",1,true,stock.Version).Status==SupplyStatus.AccessDenied&&
            win.ApplySlots(1,new(SlotAction.Move,new(InventoryKind.Personal,0),new(InventoryKind.Personal,1),1,stock.Version)).Status==SupplyStatus.AccessDenied&&
            win.Eat(1,new(0,stock.Version,win.Hunger(1).Day)).Status==EatStatus.AccessDenied&&
            win.Harvest(1,"a",win.Plot("a").Version).Status==FarmStatus.AccessDenied&&
            PowerTests.Command(win,PowerAction.SetRunning,false).Status==PowerStatus.AccessDenied,
            "Terminal authority rejects inventory, food, farm and power mutations");
        var(fresh,fc)=Create();Check(!fc.Frozen&&!fresh.Rescue().Completed&&fresh.Rescue().SignalDays==0&&fresh.Rescue().TotalLossDays==0,"New session resets victory, outage history and clock freeze");
        var(boundary,bc)=Create();Start(boundary);PowerTests.Command(boundary,PowerAction.SetConsumer,false,"beacon");bc.Advance(2400); // idle burns 0.8 Fuel before first transmission
        PowerTests.Command(boundary,PowerAction.SetConsumer,true,"beacon");bc.Advance(99999);
        Check(boundary.Rescue().Completed&&boundary.Power().Fuel==0&&bc.ElapsedTicks==6600L*TimeSpan.TicksPerSecond,"Victory at exact Fuel exhaustion is still awarded once");
        for(int i=0;i<16;i++)
        {
            var(race,rc)=Create();Start(race);int count=0;race.RescueCompleted+=()=>Interlocked.Increment(ref count);
            Parallel.Invoke(()=>rc.Advance(4200),()=>rc.Advance(4200));
            Check(count==1&&race.Rescue().CompletionCount==1&&rc.ElapsedTicks==4200L*TimeSpan.TicksPerSecond,"Competing time advances cannot complete or consume twice");
        }
        var(flaps,flapClock)=Create();Start(flaps);flapClock.Advance(600);
        for(int i=0;i<8;i++)
        {
            PowerTests.Command(flaps,PowerAction.SetRunning,false);flapClock.Advance(30);
            PowerTests.Command(flaps,PowerAction.SetRunning,true);flapClock.Advance(1);
        }
        Check(flaps.Rescue().TotalLossDays==.0375m&&flaps.Rescue().EpisodeOutageTicks==240L*TimeSpan.TicksPerSecond,
            "Repeated sub-grace interruptions aggregate rather than renewing grace on each blip");
        PowerTests.Command(cap,PowerAction.SetRunning,true);cc.Advance(150);PowerTests.Command(cap,PowerAction.SetRunning,false);cc.Advance(6000);
        Check(cap.Rescue().SignalDays==1.25m&&cap.Rescue().TotalLossDays==2,"Stable recovery renews the cap for a genuinely separate outage");
        var(custom,customClock)=Create(new(.5m,.1m,.5m,.2m,.1m));Start(custom);customClock.Advance(120);
        PowerTests.Command(custom,PowerAction.SetRunning,false);customClock.Advance(120);
        Check(custom.Rescue().SignalDays==.15m&&custom.Rescue().TotalLossDays==.05m&&custom.Rescue().MaxEpisodeLossDays==.2m,
            "Non-default grace, rate, target and cap are data-driven");
        PowerTests.Command(custom,PowerAction.SetRunning,true);customClock.Advance(9999);
        Check(custom.Rescue().Completed&&custom.Rescue().SignalDays==.5m&&customClock.Snapshot().TotalSeconds==450,
            "Configured target clips shared time at its exact completion boundary");
        Console.WriteLine("PASS: startup, continuous/repeated outages, anti-flap recovery, capped/floored loss, Fuel-boundary skips, recovery, terminal freeze/reset and exactly-once concurrent victory");
    }
}
