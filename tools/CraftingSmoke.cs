using System;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Hollowlight.Core;
using Hollowlight.Presentation;
using Hollowlight.World;

namespace Hollowlight.Tools;
public partial class CraftingSmoke : Node
{
    private static bool _reset;
    private readonly SmokeInput _input=new();
    private ThresholdWorld _world=null!;
    private PlayerCharacter _actor=null!;
    private SuppliesSnapshot Stock=>_world.SuppliesFor(_actor.PlayerId);
    private PowerSnapshot Power=>_world.Electrical!;
    private CraftingPanel Panel=>_world.CraftingControls;
    public override async void _Ready()
    {
        try
        {
            var room=GD.Load<PackedScene>(ProjectSettings.GetSetting("application/run/main_scene").AsString()).Instantiate<SanctuaryRoom>();
            room.InputOverride=_input;AddChild(room);_world=room.World;_actor=_world.GetNode<PlayerCharacter>("Player");await Frames(3);
            Check(Stock.Personal.Capacity==6&&Stock.Personal.HotbarSlots==6&&_world.GrowingPlots.Count==4&&Stock.Storage.Count("fuel")==2,"Unchanged six-slot/four-plot/starting-Fuel baseline");
            if(_reset)
            {
                Check(!Power.Running&&Power.Fuel==0&&Stock.Personal.Count("backpack_kit")==0&&Stock.Storage.Count("potato_seed")==2,"Reset clears upgrades, kits, expansion and crafting state");
                GD.Print("PASS: live crafting, personal-only materials, power/stale mouse, kit installation, eight-slot navigation, fifth plot, seed propagation, farming and reset");GetTree().Quit();return;
            }
            await Bench();Panel.ActivateRow(3);await Frames(2);Check(Panel.Warning.Contains("power"),"Unpowered workbench refuses craft with feedback");
            Panel.ActivateRow(4);await Frames(2);Check(_world.PowerVisible&&!_world.CraftingVisible&&_world.PowerControls.FocusedRow==4,"Crafting links to the existing focused workbench power switch");
            _world.ClosePower();await Bench();int focus=Panel.FocusedRow;
            await Joy(JoyButton.DpadDown,1);Check(Panel.FocusedRow==focus,"Other controller cannot navigate crafting");
            await Joy(JoyButton.DpadDown);Check(Panel.FocusedRow!=focus,"Assigned controller navigates recipes");await Joy(JoyButton.B);
            await Bench();var remote=new CraftRequest("backpack_extension",Stock.Version,Power.Version);
            _actor.Position=new Vector2(400,208);await Frames(3);
            Check(!_world.CraftingVisible&&!_actor.InventoryOpen&&_world.CraftAtWorkbench(_actor,remote).Status==CraftStatus.AccessDenied,"Range exit closes workbench and rejects remote crafting");
            foreach(string id in new[]{"radio-shell","tool-case","machine-housing"})
            {
                var node=_world.GetChildren().OfType<Interactable>().Single(n=>n.ObjectId==id);_actor.Position=node.Position+new Vector2(0,12);await Frames(3);_input.Tap=true;await Frames(3);
                Check(Stock.Remaining[id]==0,"Gather original finite crafting material: "+id);
            }
            await Chest();
            Check(_world.TransferSupplies(_actor,"scrap",26,false,Stock.Version)?.Amount==26&&_world.TransferSupplies(_actor,"organic",8,false,Stock.Version)?.Amount==8,"Material totals conserved through explicit deposits");
            _world.TransferSupplies(_actor,"fuel",2,true,Stock.Version);_world.CloseInventory();await Generator();
            _world.PowerControls.ActivateRow(2);await Frames(2);_world.PowerControls.ActivateRow(0);await Frames(2);_world.PowerControls.ActivateRow(3);await Frames(2);
            _world.PowerControls.ActivateRow(4);await Frames(2);Check(Power.Demand==4,"Existing beacon + workbench use four PU");
            _world.ClosePower();await Bench();Panel.ActivateRow(3);await Frames(2);
            Check(Panel.Warning.Contains("Missing")&&Stock.Storage.Count("scrap")==26,"Workbench never spends communal materials implicitly");
            _world.ClosePower();await Chest();_world.TransferSupplies(_actor,"scrap",8,true,Stock.Version);_world.TransferSupplies(_actor,"organic",3,true,Stock.Version);_world.CloseInventory();await Bench();
            var craftPosition=_actor.Position;long craftTicks=_world.Clock.ElapsedTicks;decimal craftFuel=Power.Fuel;
            _input.Movement=Vector2.Right;await Frames(30);_input.Movement=Vector2.Zero;
            Check(_actor.Position==craftPosition&&_world.Clock.ElapsedTicks>craftTicks&&Power.Fuel<craftFuel,"Crafting context holds only the owner; world and Fuel keep advancing");
            await Capture("recipes");
            await CraftMouse(true);
            var off=_world.PowerCommand(_actor,new(PowerAction.SetConsumer,Power.Version,Stock.Version,false,"workbench"));
            Check(off.Status==PowerStatus.Applied,"Manual power change can race a held craft click");await Frames(2);await CraftMouse(false);
            Check(Stock.Personal.Count("backpack_kit")==0&&Stock.Personal.Count("scrap")==8&&Panel.Warning.Contains("changed"),"Held stale craft preserves every input");
            _world.PowerCommand(_actor,new(PowerAction.SetConsumer,Power.Version,Stock.Version,true,"workbench"));await Frames(2);
            Panel.Select(0);await KeyPress(Key.Enter);Check(Stock.Personal.Count("backpack_kit")==1&&Stock.Personal.Count("scrap")==0&&Stock.Personal.Count("organic")==0,"Keyboard confirmation crafts exact backpack recipe");
            _world.ClosePower();await KeyPress(Key.Tab);
            var inventory=_world.InventoryPanel;int bag=Slot("backpack_kit");var old=Stock.Personal;inventory.FocusSlot(new(InventoryKind.Personal,bag));await Frames(2);await Capture("kit-install");
            if(OS.GetCmdlineUserArgs().Contains("--sanctuary-variant"))await Joy(JoyButton.LeftStick);else await KeyPress(Key.U);
            Check(Stock.Personal.Capacity==8&&Stock.Personal.IsStorageOnly(6)&&Stock.Personal.IsStorageOnly(7)&&Stock.Personal.HotbarSlots==6,"Inventory installs personal +2 storage without changing hotbar reservation");
            for(int i=0;i<6;i++)Check(i==bag?Stock.Personal.Slots[i]==null:old.Slots[i]==Stock.Personal.Slots[i],"Original slot stays in place: "+i);
            int food=Slot("food");inventory.BeginSplit(new(InventoryKind.Personal,food));inventory.ConfirmSplit();inventory.Activate(new(InventoryKind.Personal,7));await Frames(2);
            Check(Stock.Personal.Slots[7]?.ResourceId=="food"&&Stock.Personal.Slots[7]?.Quantity==1,"Existing split/move operations reach new storage slots");
            inventory.FocusSlot(new(InventoryKind.Personal,0));await Joy(JoyButton.DpadDown);await Joy(JoyButton.DpadDown);Check(inventory.Focused.Index==6,"Controller reaches added storage row");
            var position=_actor.Position;long ticks=_world.Clock.ElapsedTicks;_input.Movement=Vector2.Right;await Frames(10);_input.Movement=Vector2.Zero;
            Check(_actor.Position==position&&_world.Clock.ElapsedTicks>ticks&&_actor.CollisionLayer==2,"Expanded backpack holds owner stationary while simulation continues");
            await Capture("backpack");_world.CloseInventory();await Chest();await Capture("expanded-chest");
            _world.TransferSupplies(_actor,"scrap",12,true,Stock.Version);_world.TransferSupplies(_actor,"organic",4,true,Stock.Version);_world.CloseInventory();await Bench();
            Panel.Select(1);await Joy(JoyButton.A);Check(Stock.Personal.Count("bed_kit")==1&&_world.GrowingPlots.Count==4,"Controller crafts transferable bed kit without silently installing it");
            _world.ClosePower();_actor.Position=new Vector2(136,208);await Frames(3);await KeyPress(Key.Tab);inventory.FocusSlot(new(InventoryKind.Personal,Slot("bed_kit")));await KeyPress(Key.U);
            Check(Stock.Personal.Count("bed_kit")==1&&_world.GrowingPlots.Count==4,"Remote bed installation rejects without consuming kit");_world.CloseInventory();
            _actor.Position=_world.GardenSocket+new Vector2(0,12);await Frames(3);
            var query=new PhysicsShapeQueryParameters2D {Shape=new RectangleShape2D {Size=new Vector2(8,6)},Transform=new Transform2D(0,_actor.Position),CollisionMask=1};
            Check(_world.GetWorld2D().DirectSpaceState.IntersectShape(query).Count==0,"Expansion approach preserves existing feet collision");
            _input.Tap=true;await Frames(3);Check(_world.GrowingPlots.Count==5&&_world.CraftingFor(1).BedInstalled&&Stock.Personal.Count("bed_kit")==0,"Contextual marker installs exactly one shared fifth plot");
            await Capture("installed-bed");
            await Chest();_world.TransferSupplies(_actor,"potato_seed",1,true,Stock.Version);_world.TransferSupplies(_actor,"organic",1,true,Stock.Version);_world.CloseInventory();await Bench();
            Panel.Select(2);await CraftMouse(true);await CraftMouse(false);Check(Stock.Personal.Count("potato_seed")==2&&Stock.Personal.Count("organic")==0,"Propagation uses one seed + one Organic and yields two seeds");
            _world.ClosePower();_actor.Position=_world.GardenSocket+new Vector2(0,12);await Frames(3);_input.Tap=true;await Frames(3);
            Check(_world.Plot(SharedSupplies.ExpansionPlotId).Stage==CropStage.Seedling&&Stock.Personal.Count("potato_seed")==1,"New plot uses existing contextual planting and one seed");
            _world.Clock.Advance(1200);await Frames(3);Check(_world.Plot(SharedSupplies.ExpansionPlotId).Stage==CropStage.Mature,"New plot grows through ordinary elapsed time, even after Fuel exhaustion");
            await Capture("mature-plot");
            int before=Stock.Personal.Count("food");_input.Tap=true;await Frames(3);
            Check(Stock.Personal.Count("food")==before+3&&Stock.Personal.Count("potato_seed")==2,"Harvest uses unchanged Food and returned-seed yields");
            Check(Stock.Storage.Count("scrap")==6&&Stock.Storage.Count("organic")==0,"Revised combined investment leaves exactly six Scrap and zero original Organic");
            await Capture("harvest");
            _reset=true;_world._UnhandledKeyInput(new InputEventKey {PhysicalKeycode=Key.R,Pressed=true});
        }
        catch(Exception error){GD.PushError(error.ToString());GetTree().Quit(1);}
    }
    private int Slot(string item)=>Enumerable.Range(0,Stock.Personal.Capacity).First(i=>Stock.Personal.Slots[i]?.ResourceId==item);
    private async Task Bench(){var station=_world.PowerStations.Single(s=>s.Id=="workbench");_actor.Position=station.Position;await Frames(3);_input.Tap=true;await Frames(3);Check(_world.CraftingVisible,"Workbench interaction opens crafting context");}
    private async Task Generator(){var station=_world.PowerStations.Single(s=>s.Id=="generator");_actor.Position=station.Position;await Frames(3);_input.Tap=true;await Frames(3);Check(_world.PowerVisible,"Generator keeps manual allocation interface");}
    private async Task Chest(){_actor.Position=new Vector2(88,242);await Frames(3);_input.Tap=true;await Frames(3);Check(_world.ChestAccessible,"Explicit communal chest interaction");}
    private async Task CraftMouse(bool pressed)
    {
        var button=Panel.GetChildren().OfType<Button>().ElementAt(3);var point=GetViewport().GetFinalTransform()*(button.GetGlobalTransformWithCanvas()*(button.Size/2));
        Input.ParseInputEvent(new InputEventMouseMotion {Position=point,GlobalPosition=point});await Frames(1);
        Input.ParseInputEvent(new InputEventMouseButton {Position=point,GlobalPosition=point,ButtonIndex=MouseButton.Left,Pressed=pressed,ButtonMask=pressed?MouseButtonMask.Left:0});await Frames(2);
    }
    private async Task KeyPress(Key key){Input.ParseInputEvent(new InputEventKey {PhysicalKeycode=key,Pressed=true});await Frames(2);Input.ParseInputEvent(new InputEventKey {PhysicalKeycode=key,Pressed=false});await Frames(2);}
    private async Task Joy(JoyButton button,int device=0){Input.ParseInputEvent(new InputEventJoypadButton {Device=device,ButtonIndex=button,Pressed=true});await Frames(2);Input.ParseInputEvent(new InputEventJoypadButton {Device=device,ButtonIndex=button,Pressed=false});await Frames(2);}
    private async Task Frames(int count){for(int i=0;i<count;i++){await ToSignal(GetTree(),SceneTree.SignalName.PhysicsFrame);await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}}
    private async Task Capture(string name)
    {
        if(!OS.GetCmdlineUserArgs().Contains("--capture-crafting"))return;await Frames(4);RenderingServer.ForceDraw(false);
        string folder=ProjectSettings.GlobalizePath("user://crafting");DirAccess.MakeDirRecursiveAbsolute(folder);
        Check(GetViewport().GetTexture().GetImage().SavePng($"{folder}/{name}.png")==Error.Ok,"Capture "+name);
    }
    private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);GD.Print("PASS: "+message);}
}
