using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hollowlight.Core;

public static class CraftingTests
{
    public static (SharedSupplies S,GameClock C) Create(int count=2,CraftingSetup? configuration=null)
    {
        var supply=SupplySetup.Parse(ScavengingTests.Data("supplies"));var farm=AgricultureSetup.Parse(ScavengingTests.Data("farming"));
        var craft=configuration??CraftingSetup.Parse(ScavengingTests.Data("crafting"));
        var resources=supply.Resources.Concat(farm.Resources).Concat(craft.Resources).Append(new ResourceDefinition("fuel","Fuel","Fuel")).ToArray();
        var nodes=resources.Select(r=>new ResourceNodeDefinition("fixture-"+r.Id,r.Id,1000));
        var s=new SharedSupplies(resources,nodes,Enumerable.Range(1,count));var c=new GameClock(new());
        s.InitializeAgriculture(farm,c,new[]{"a","b","c","d"});s.InitializeHunger(new(),c);s.InitializePower(PowerTests.Definition,c);s.InitializeCrafting(craft);return(s,c);
    }
    public static void Give(SharedSupplies s,string resource,int amount,int id=1)
    {Check(s.Gather(id,"fixture-"+resource,amount,s.Snapshot(id).Version).Amount==amount,"Fixture supplies fit");}
    public static void Start(SharedSupplies s)
    {Give(s,"fuel",1);PowerTests.Command(s,PowerAction.LoadFuel);PowerTests.Command(s,PowerAction.SetRunning,true);PowerTests.Command(s,PowerAction.SetConsumer,true,"workbench");}
    public static CraftRequest Request(SharedSupplies s,string recipe,int id=1)=>new(recipe,s.Snapshot(id).Version,s.Power().Version);
    public static CraftResult Make(SharedSupplies s,string recipe,int id=1)=>s.Craft(id,Request(s,recipe,id));
    public static int Slot(SharedSupplies s,string item,int id=1)=>Enumerable.Range(0,s.Snapshot(id).Personal.Capacity).First(i=>s.Snapshot(id).Personal.Slots[i]?.ResourceId==item);
    private static void Check(bool value,string message){if(!value)throw new Exception(message);}
    private static int Total(SharedSupplies s,string item,int count=2)=>Enumerable.Range(1,count).Sum(id=>s.Snapshot(id).Personal.Count(item))+s.Snapshot(1).Storage.Count(item);
    public static void Run()
    {
        var(s,c)=Create();Give(s,"scrap",8);Give(s,"organic",3);var before=s.Snapshot(1);
        Check(Make(s,"backpack_extension").Status==CraftStatus.NoPower&&s.Snapshot(1).Version==before.Version,"Unpowered crafting cannot spend materials");
        Start(s);Check(Make(s,"backpack_extension").Status==CraftStatus.Applied&&s.Snapshot(1).Personal.Count("scrap")==0&&s.Snapshot(1).Personal.Count("organic")==0,"Exact revised backpack cost, output fits space freed by consumption");
        var old=s.Snapshot(1).Personal;int slot=Slot(s,"backpack_kit");
        Check(s.InstallBackpack(1,new(slot,s.Snapshot(1).Version)).Status==CraftStatus.Applied,"Backpack installation succeeds");
        var expanded=s.Snapshot(1).Personal;
        Check(expanded.Capacity==8&&expanded.HotbarSlots==6&&expanded.IsStorageOnly(6)&&expanded.IsStorageOnly(7)&&!expanded.IsStorageOnly(5),"First six slots reserved, two added storage-only slots");
        for(int i=0;i<6;i++)Check(i==slot?expanded.Slots[i]==null:expanded.Slots[i]==old.Slots[i],"Installation preserves every original slot except consumed kit");
        Check(s.Snapshot(2).Personal.Capacity==6,"Personal upgrade does not affect another actor");
        Give(s,"scrap",8);Give(s,"organic",3);Check(Make(s,"backpack_extension").Status==CraftStatus.Applied,"An upgraded crafter can make a kit for another actor");
        var duplicate=new InstallRequest(Slot(s,"backpack_kit"),s.Snapshot(1).Version);
        Check(s.InstallBackpack(1,duplicate).Status==CraftStatus.AlreadyInstalled&&s.Snapshot(1).Personal.Count("backpack_kit")==1,"Duplicate installation retains transferable kit");
        s.Transfer(1,"backpack_kit",1,false,s.Snapshot(1).Version);s.Transfer(2,"backpack_kit",1,true,s.Snapshot(2).Version);
        Check(s.InstallBackpack(2,new(Slot(s,"backpack_kit",2),s.Snapshot(2).Version)).Status==CraftStatus.Applied,"Kit transfers through chest and belongs to installing actor");
        Check(Make(s,"backpack_extension").Status==CraftStatus.LimitReached,"No unusable excess kits after the current party is upgraded");
        Give(s,"organic",1);Give(s,"potato_seed",1);var source=s.Snapshot(1).Personal.Slots[0];
        var move=s.ApplySlots(1,new(SlotAction.Move,new(InventoryKind.Personal,Slot(s,"potato_seed")),new(InventoryKind.Personal,7),1,s.Snapshot(1).Version));
        Check(move.Status==SupplyStatus.Applied&&Make(s,"potato_propagation").Status==CraftStatus.Applied,"Storage-only slots participate in inventory recipes; no hotbar action implied");
        var(full,fc)=Create();Start(full);Give(full,"potato_seed",10);Give(full,"organic",2);Give(full,"scrap",10);Give(full,"fuel",10);Give(full,"glowcap_seed",10);
        Check(full.Snapshot(1).Personal.UsedSlots==6,"Full six-slot seed-overflow fixture");
        before=full.Snapshot(1);Check(Make(full,"potato_propagation").Status==CraftStatus.NoCapacity&&full.Snapshot(1).Version==before.Version&&full.Snapshot(1).Personal.Count("potato_seed")==10&&full.Snapshot(1).Personal.Count("organic")==2,"Seed overflow rejects atomically without lost inputs");
        full.Transfer(1,"organic",1,false,full.Snapshot(1).Version);
        Check(Make(full,"potato_propagation").Status==CraftStatus.Applied&&full.Snapshot(1).Personal.Count("potato_seed")==11&&full.Snapshot(1).Personal.Count("organic")==0,"Propagation uses space freed by its last Organic input");
        var(storage,_) = Create();Start(storage);Give(storage,"scrap",8);Give(storage,"organic",3);storage.DepositAll(1,storage.Snapshot(1).Version);
        before=storage.Snapshot(1);Check(Make(storage,"backpack_extension").Status==CraftStatus.Insufficient&&storage.Snapshot(1).Version==before.Version,"Communal ingredients are never spent without explicit withdrawal");
        var(bed,bc)=Create();Start(bed);Give(bed,"scrap",12);Give(bed,"organic",5);Give(bed,"potato_seed",1);
        Check(Make(bed,"growing_bed").Status==CraftStatus.Applied&&Make(bed,"potato_propagation").Status==CraftStatus.Applied,"Bed and propagation use 12 Scrap, five Organic and return two seeds");
        before=bed.Snapshot(1);Check(Make(bed,"growing_bed").Status==CraftStatus.LimitReached&&bed.Plots().Count==4,"One outstanding bed kit; no plot before installation");
        Check(bed.InstallBed(1,new(Slot(bed,"bed_kit"),bed.Snapshot(1).Version)).Status==CraftStatus.Applied&&bed.Plots().Count==5,"Shared fifth plot installs once");
        Check(bed.Plant(1,SharedSupplies.ExpansionPlotId,"cave_potato",bed.Plot(SharedSupplies.ExpansionPlotId).Version).Status==FarmStatus.Planted,"New plot uses unchanged seed planting");
        bc.Advance(1200);int foodBeforeHarvest=bed.Snapshot(1).Personal.Count("food");var grown=bed.Plot(SharedSupplies.ExpansionPlotId);
        Check(grown.Stage==CropStage.Mature&&bed.Harvest(1,grown.Id,grown.Version).Status==FarmStatus.Harvested&&bed.Snapshot(1).Personal.Count("food")==foodBeforeHarvest+3,"Expansion crop keeps two-day growth and three-Food yield");
        for(int i=0;i<32;i++)
        {
            var(race,rc)=Create();Start(race);foreach(int id in new[]{1,2}){Give(race,"scrap",8,id);Give(race,"organic",3,id);}
            var first=Request(race,"backpack_extension");var second=Request(race,"backpack_extension",2);CraftResult? a=null,b=null;
            Parallel.Invoke(()=>a=race.Craft(1,first),()=>b=race.Craft(2,second));
            Check(new[]{a!.Status,b!.Status}.Count(x=>x==CraftStatus.Applied)==1&&Total(race,"backpack_kit")==1&&Total(race,"scrap")==8&&Total(race,"organic")==3,"Conflicting crafting consumes one exact recipe, once");
            var(install,ic)=Create();Give(install,"bed_kit",1);Give(install,"bed_kit",1,2);var v=install.Snapshot(1).Version;
            int x=Slot(install,"bed_kit"),y=Slot(install,"bed_kit",2);
            Parallel.Invoke(()=>a=install.InstallBed(1,new(x,v)),()=>b=install.InstallBed(2,new(y,v)));
            Check(install.Plots().Count==5&&Total(install,"bed_kit")==1&&new[]{a!.Status,b!.Status}.Count(z=>z==CraftStatus.Applied)==1,"Competing installers create one shared plot and preserve other kit");
            var(power,pc)=Create();Start(power);Give(power,"scrap",8);Give(power,"organic",3);var request=Request(power,"backpack_extension");
            Parallel.Invoke(()=>a=power.Craft(1,request),()=>PowerTests.Command(power,PowerAction.SetRunning,false));
            bool crafted=a!.Status==CraftStatus.Applied;
            Check(Total(power,"scrap")== (crafted?0:8)&&Total(power,"organic")== (crafted?0:3)&&Total(power,"backpack_kit")== (crafted?1:0),"Power interruption cannot cause partial crafting");
        }
        var(stale,staleClock)=Create();Start(stale);Give(stale,"scrap",8);Give(stale,"organic",3);var held=Request(stale,"backpack_extension");
        staleClock.Advance(1000);
        Check(stale.Craft(1,held).Status==CraftStatus.Stale&&Make(stale,"backpack_extension").Status==CraftStatus.NoPower,"Exhaustion invalidates held commands and prevents powerless craft");
        var(win,wc)=Create();win.InitializeRescue(new(.001m));Start(win);PowerTests.Command(win,PowerAction.SetConsumer,true,"beacon");wc.Advance(100);
        Check(Make(win,"potato_propagation").Status==CraftStatus.AccessDenied&&win.InstallBed(1,new(0,win.Snapshot(1).Version)).Status==CraftStatus.AccessDenied,"Rescued run rejects new crafting and installation");
        var(fresh,nc)=Create();Check(fresh.Snapshot(1).Personal.Capacity==6&&fresh.Plots().Count==4&&Total(fresh,"backpack_kit")==0&&Total(fresh,"bed_kit")==0,"Reset restores six slots, four plots and no kits");
        Check(fresh.Craft(1,held).Status==CraftStatus.Stale,"Previous session crafting token cannot affect a new run");
        var configured=CraftingSetup.Parse(ScavengingTests.Data("crafting"));
        configured=configured with {Recipes=configured.Recipes.Select(r=>r.Id=="potato_propagation"?r with {
            Inputs=new Dictionary<string,int>{{"potato_seed",1},{"organic",2}},Outputs=new Dictionary<string,int>{{"potato_seed",3}}}:r).ToArray()};
        var(custom,customClock)=Create(configuration:configured);Start(custom);Give(custom,"potato_seed",1);Give(custom,"organic",2);
        Check(Make(custom,"potato_propagation").Status==CraftStatus.Applied&&custom.Snapshot(1).Personal.Count("potato_seed")==3&&custom.Snapshot(1).Personal.Count("organic")==0,"Propagation costs/results are recipe data, independent of crop harvesting");
        Console.WriteLine("PASS: revised recipes, personal-only ingredients, power/overflow safety, kit transfers/ownership, hotbar indices, fifth-plot growth, 96 craft/install/power races, exhaustion, victory and reset");
    }
}
