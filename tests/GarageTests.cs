using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Hollowlight.Core;

public static class GarageTests
{
    private static void Check(bool v,string message){if(!v)throw new Exception(message);}
    public static GarageDefinition Definition=>GarageDefinition.Parse(File.ReadAllText(Path.Combine(AppContext.BaseDirectory,"data/locations/garage.json")));
    private static SharedSupplies State()
    {
        var s=new SharedSupplies(new[]{new ResourceDefinition("scrap","Scrap","Scrap"),new("food","Food","Food"),new("salvage_tool","Tool","Tool")},
            new[]{new ResourceNodeDefinition("parts","scrap",8),new("rack","salvage_tool",1),new("van","scrap",4),new("machine","scrap",2),
                new("bulk","scrap",100),new("meals","food",100),new("future-tool-source","salvage_tool",2)},new[]{1,2},24);
        s.InitializeExploration(new[]{new ExplorationDefinition("parts","Parts","container","parts","scrap",Array.Empty<int>()),
            new("rack","Rack","tool-rack","rack","salvage_tool",Array.Empty<int>()),new("van","Van","vehicle","van","scrap",new[]{2,2},"salvage_tool",1.2),
            new("machine","Machine","machinery","machine","scrap",new[]{2},"salvage_tool",.8)});return s;
    }
    private static ExplorationResult Take(SharedSupplies s,int actor,string id,int n)=>s.TakeObject(actor,id,n,s.Snapshot(actor).Version,s.InspectObject(id).Version);
    private static void Tool(SharedSupplies s)
    {s.SearchObject(1,"rack",s.InspectObject("rack").Version);Check(Take(s,1,"rack",1).Amount==1,"Fixture tool picked up once");}
    private static ExplorationResult Begin(SharedSupplies s,int actor=1,bool consent=true)=>s.BeginDismantle(actor,"van",s.Snapshot(actor).Version,s.InspectObject("van").Version,consent);
    public static void Run()
    {
        var definition=Definition;
        for(int seed=0;seed<32;seed++)
        {
            var region=RegionTests.Generate(seed);var originalFood=region.Sources.Where(s=>s.Site.ResourceId=="food").Sum(s=>s.Site.Quantity);
            var originalFuel=region.Sources.Where(s=>s.Site.ResourceId=="fuel").Sum(s=>s.Site.Quantity);
            region.InstallGarage(definition);var garage=region.Garage!;
            Check(region.Sources.Where(s=>s.Site.ResourceId=="scrap").Sum(s=>s.Site.Quantity)==26,"No extra Scrap");
            Check(region.Sources.Where(s=>s.Site.ResourceId=="organic").Sum(s=>s.Site.Quantity)==8,"Organic unchanged");
            Check(region.Sources.Where(s=>s.Site.ResourceId=="food").Sum(s=>s.Site.Quantity)==originalFood,"Food quantity rolls preserved");
            Check(region.Sources.Where(s=>s.Site.ResourceId=="fuel").Sum(s=>s.Site.Quantity)==originalFuel,"Fuel quantity rolls preserved");
            Check(region.Sources.Where(s=>s.Site.ResourceId=="salvage_tool").Sum(s=>s.Site.Quantity)==1,"One approved fixture tool");
            Check(!region.Sources.Any(s=>s.Site.Id=="machine-housing"),"Original budget not duplicated beside new objects");
            Check(region.Sources.Where(s=>s.Site.ResourceId=="scrap"&&s.Site.Id!="garage-van").Sum(s=>s.Site.Quantity)==22,"Preservation leaves exactly four Scrap deferred");
            foreach(var o in garage.Objects)Check(region.Path(region.Entrance.Cell,o.Approach).Count>0,"Every object has a walkable interaction approach");
            Check(region.Path(region.Entrance.Cell,garage.Entry).Count>0&&region.Routes.Any(r=>r.Id=="garage-apron"),"Road apron connects");
        }
        var state=State();
        Check(Take(state,1,"parts",1).Status==ExplorationStatus.Invalid,"Cannot take unsearched contents");
        state.SearchObject(1,"parts",state.InspectObject("parts").Version);
        var oldInventory=state.Snapshot(1).Version;var oldObject=state.InspectObject("parts").Version;
        Check(Take(state,1,"parts",3).Amount==3&&state.InspectObject("parts").Remaining==5,"Partial collection retains source remainder");
        Check(state.TakeObject(1,"parts",3,oldInventory,oldObject).Status==ExplorationStatus.Stale,"Stale loot request cannot repeat");
        var loot=State();loot.SearchObject(1,"parts",loot.InspectObject("parts").Version);
        loot.Gather(1,"bulk",59,loot.Snapshot(1).Version);
        Check(Take(loot,1,"parts",8).Amount==1&&loot.InspectObject("parts").Remaining==7,"Loot fills the last stack space and preserves the remainder");
        Check(Take(loot,1,"parts",8).Status==ExplorationStatus.NoCapacity&&loot.InspectObject("parts").Remaining==7,"Full backpack leaves searched contents untouched");
        Check(state.Gather(1,"van",4,state.Snapshot(1).Version).Status==SupplyStatus.AccessDenied,"Generic pickup cannot bypass tool/consent gates");
        Check(Begin(state).Status==ExplorationStatus.NoTool,"Carried tool required");Tool(state);
        Check(Begin(state,consent:false).Status==ExplorationStatus.ConsentRequired,"Explicit van consent required");
        var work=Begin(state);Check(work.Status==ExplorationStatus.Working&&state.InspectObject("van").RestorationAvailable,"Beginning does not destroy van");
        Check(state.AdvanceDismantle(2,work.Ticket,100).Status==ExplorationStatus.AccessDenied,"Other actor cannot advance held work");
        Check(state.AdvanceDismantle(1,work.Ticket,1).Status==ExplorationStatus.Working,"No instantaneous dismantle");
        state.CancelDismantle(1,work.Ticket);Check(state.InspectObject("van").RestorationAvailable,"Cancellation before first commit preserves van");
        Check(state.AdvanceDismantle(1,work.Ticket,long.MaxValue/2).Status==ExplorationStatus.Stale,"Cancelled ticket cannot replay");
        work=Begin(state);var clock=new GameClock(new());clock.Advance(60000);
        Check(state.AdvanceDismantle(1,work.Ticket,0).Status==ExplorationStatus.Working,"Clock skip does not fabricate active work time");
        state.Gather(2,"meals",1,state.Snapshot(2).Version);
        Check(state.AdvanceDismantle(1,work.Ticket,TimeSpan.TicksPerSecond*2).Amount==2,"Unrelated actor inventory changes do not invalidate held work");
        Check(!state.InspectObject("van").RestorationAvailable&&state.InspectObject("van").Stage==1,"First completed stage irreversibly removes opportunity");
        Check(state.AdvanceDismantle(1,work.Ticket,100).Status==ExplorationStatus.Stale,"Completed ticket awards once");
        work=Begin(state,consent:false);Check(state.AdvanceDismantle(1,work.Ticket,TimeSpan.TicksPerSecond*2).Amount==2,"Second stage awards remaining two Scrap");
        Check(state.InspectObject("van").Remaining==0&&state.InspectObject("van").Stage==2,"Final chassis state persists");
        var full=State();Tool(full);full.Gather(1,"bulk",50,full.Snapshot(1).Version);
        Check(Begin(full).Status==ExplorationStatus.NoCapacity,"Full pack cannot start destructive removal");
        full.Transfer(1,"scrap",2,false,full.Snapshot(1).Version);work=Begin(full);
        full.Gather(1,"bulk",1,full.Snapshot(1).Version);
        Check(full.AdvanceDismantle(1,work.Ticket,TimeSpan.TicksPerSecond*2).Status==ExplorationStatus.NoCapacity&&full.InspectObject("van").RestorationAvailable,"Capacity rechecked at commit without loss");
        var transfer=State();Tool(transfer);transfer.Transfer(1,"salvage_tool",1,false,transfer.Snapshot(1).Version);
        transfer.Transfer(2,"salvage_tool",1,true,transfer.Snapshot(2).Version);
        Check(Begin(transfer,1).Status==ExplorationStatus.NoTool,"Communal/team possession is not personal possession");
        Check(transfer.ReturnTool(2,"rack",transfer.Snapshot(2).Version,transfer.InspectObject("rack").Version).Amount==1,"Another actor can return the shared tool");
        var separate=State();Tool(separate);separate.Gather(2,"future-tool-source",1,separate.Snapshot(2).Version);
        var first=Begin(separate);
        var second=separate.BeginDismantle(2,"machine",separate.Snapshot(2).Version,separate.InspectObject("machine").Version,true);
        Check(first.Status==ExplorationStatus.Working&&second.Status==ExplorationStatus.Working,"Different actors can work on separate objects simultaneously");
        separate.Transfer(1,"salvage_tool",1,false,separate.Snapshot(1).Version);
        Check(separate.AdvanceDismantle(1,first.Ticket,TimeSpan.TicksPerSecond*2).Status==ExplorationStatus.NoTool&&separate.InspectObject("van").RestorationAvailable,"Losing carried tool safely cancels before commit");
        Check(separate.AdvanceDismantle(2,second.Ticket,TimeSpan.TicksPerSecond).Amount==2,"Other actor work survives unrelated cancellation");
        for(int i=0;i<32;i++)
        {
            var race=State();race.SearchObject(1,"rack",race.InspectObject("rack").Version);
            var inv=race.Snapshot(1).Version;var obj=race.InspectObject("rack").Version;
            var results=new ExplorationResult[2];Parallel.Invoke(()=>results[0]=race.TakeObject(1,"rack",1,inv,obj),()=>results[1]=race.TakeObject(2,"rack",1,inv,obj));
            Check(results.Sum(r=>r.Amount)==1,"Conflicting tool pickups conserve one tool");
            race.Gather(1,"future-tool-source",1,race.Snapshot(1).Version);race.Gather(2,"future-tool-source",1,race.Snapshot(2).Version);
            var v=race.Snapshot(1).Version;var source=race.InspectObject("van").Version;
            Parallel.Invoke(()=>results[0]=race.BeginDismantle(1,"van",v,source,true),()=>results[1]=race.BeginDismantle(2,"van",v,source,true));
            Check(results.Count(r=>r.Status==ExplorationStatus.Working)==1,"One worker owns a stage");
            int winner=results[0].Status==ExplorationStatus.Working?1:2;
            Check(race.AdvanceDismantle(winner,results[winner-1].Ticket,TimeSpan.TicksPerSecond*2).Amount==2,"Conflicting work awards one stage");
        }
        var walls=new bool[12,12];for(int y=0;y<12;y++)walls[5,y]=true;
        var discovery=new WorldDiscovery(walls);discovery.Observe(1,new(2,5),9);
        Check(discovery.Visible(1,new(5,5))&&!discovery.Visible(1,new(7,5)),"Opaque wall visible; contents beyond it hidden");
        discovery.Observe(2,new(9,5),5);
        long revision=discovery.Revision;discovery.Observe(2,new(9,5),5);
        Check(discovery.Revision==revision,"Unchanged sight does not rebuild presentation masks");
        Check(discovery.Explored(new(7,5))&&!discovery.Visible(1,new(7,5))&&discovery.Visible(2,new(7,5)),"Shared geography does not grant remote current visibility");
        discovery.RemoveObserver(2);Check(discovery.Revision>revision&&discovery.Explored(new(7,5))&&!discovery.Visible(2,new(7,5)),"Exploration memory survives observer departure; presentation revision reflects changed current sight");
        Check(new WorldDiscovery(walls).Snapshot(1).Explored.Count==0,"New session resets exploration");
        Console.WriteLine("PASS: 32 seeded garage layouts, source budgets/road/approaches, searching/partial/full capacity, carried/shared tools, consent/cancel/clock-skip/staged salvage, 64 races, reusable shared discovery versus current sight and reset");
    }
}
