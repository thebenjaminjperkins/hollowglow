using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Hollowlight.Core;
using Hollowlight.Presentation;

namespace Hollowlight.World;

public partial class ThresholdWorld
{
    public GaragePlan? Garage=>Region?.Garage;
    public WorldDiscovery? Discovery=>_state?.Discovery;
    private SuppliesPanel? _lootPanel;
    private VehicleInspection? _vehiclePanel;
    private string? _openObject,_workingObject;
    private SupplyVersion? _vanApproval;
    private Guid _workTicket;
    private double _workElapsed;
    private bool _awaitVanRelease;
    private Sprite2D? _heldTool;
    public bool GarageMenuVisible=>(_lootPanel?.Visible??false)||(_vehiclePanel?.Visible??false);
    public bool Dismantling=>_workTicket!=Guid.Empty;
    public SuppliesPanel LootControls=>_lootPanel!;
    public VehicleInspection VehicleControls=>_vehiclePanel!;
    public ExplorationSnapshot ObjectState(string id)=>_state.InspectObject(id);
    public bool CurrentlyVisible(Vector2 position)=>Discovery?.Visible(_controlledActor.PlayerId,MountainRegion.CellAt(position.X,position.Y))??true;
    public bool PresentationVisible(Node2D owner)=>GetNodeOrNull<WorldVisuals>("WorldVisuals")?.IsVisible(owner)??CurrentlyVisible(owner.Position);

    private void InitializeGarage()
    {
        if(Garage==null)return;
        _state.InitializeExploration(Garage.Objects.Select(o=>o.Definition));
        var opaque=new bool[Region!.Width,Region.Height];
        for(int y=0;y<Region.Height;y++)for(int x=0;x<Region.Width;x++)opaque[x,y]=Region.Ground[x,y]==GroundKind.Rock;
        _state.InitializeDiscovery(opaque);
        Discovery!.Observe(_controlledActor.PlayerId,MountainRegion.CellAt(_controlledActor.Position.X,_controlledActor.Position.Y),10);
        AddChild(new ExplorationFog{Name="WorldFog",Discovery=Discovery,Observer=_controlledActor});
        _lootPanel=new SuppliesPanel{SourceOnly=true,StorageTitle="LOOT"};_hud.AddChild(_lootPanel);
        _lootPanel.Appearance.ChestColumns=2;_lootPanel.Appearance.ChestRows=2;
        _lootPanel.ControllerDevice=_controlledActor.ControllerDevice;
        GarageAssets.AddToolIcon(_lootPanel.Appearance);GarageAssets.AddToolIcon(_hud.Supplies.Appearance);
        _lootPanel.CloseRequested+=CloseGarage;
        _lootPanel.OperationRequested+=request=>_lootPanel.ShowResult(ApplyLootCommand(request));
        _vehiclePanel=new VehicleInspection{ControllerDevice=_controlledActor.ControllerDevice};_hud.AddChild(_vehiclePanel);
        _vehiclePanel.PreserveRequested+=()=>{CloseGarage();Notify("Vehicle left intact. Restoration is not implemented.");};
        _vehiclePanel.DismantleRequested+=ConfirmVan;
        var map=GetNode<ThresholdMap>("Map");
        foreach(var o in Garage.Objects)map.AddChild(new GarageObjectArt{Object=o,World=this,Position=new(o.Foot.WorldX,o.Foot.WorldY)});
        _heldTool=new Sprite2D{Texture=GD.Load<Texture2D>("res://assets/garage/tool-icon.png"),Position=new(8,-9),Scale=Vector2.One*.5f,Visible=false};_controlledActor.AddChild(_heldTool);
    }
    private GarageObject? FindGarageObject(PlayerCharacter actor)
    {
        if(Garage==null)return null;
        return Garage.Objects.Where(o=>GarageAccess(actor,o)).OrderBy(o=>actor.Position.DistanceSquaredTo(ObjectPoint(o))).FirstOrDefault();
    }
    private static Vector2 ObjectPoint(GarageObject o)=>new(o.Foot.WorldX,o.Foot.WorldY+12);
    private bool GarageAccess(PlayerCharacter actor,GarageObject o,bool ray=true)
    {
        if(actor!=_controlledActor||!CurrentlyVisible(new(o.Foot.WorldX,o.Foot.WorldY))||actor.Position.DistanceTo(ObjectPoint(o))>Config.InteractionRange)return false;
        return !ray||GetWorld2D().DirectSpaceState.IntersectRay(PhysicsRayQueryParameters2D.Create(actor.Position,ObjectPoint(o),1)).Count==0;
    }
    private bool UseGarageObject(PlayerCharacter actor)
    {
        var o=FindGarageObject(actor);if(o==null)return false;
        if(_awaitVanRelease)return true;
        var s=ObjectState(o.Id);
        if(o.Rule.Kind=="container")OpenLoot(o);
        else if(o.Rule.Kind=="tool-rack")
        {
            var result=s.Remaining>0?SearchAndTakeTool(o):_state.ReturnTool(actor.PlayerId,o.Id,_state.Snapshot(actor.PlayerId).Version,s.Version);
            Notify(result.Status==ExplorationStatus.Applied?(s.Remaining>0?"Salvage tool carried: one slot. No durability.":"Tool returned to its rack."):ExplorationMessage(result.Status));
        }
        else if(o.Rule.Kind=="vehicle"&&s.Stage==0&&_vanApproval==null)
        {
            ClosePower();CloseInventory();_hud.HideHelp();_openObject=o.Id;
            _vehiclePanel!.Open(s.Version);actor.InventoryOpen=true;
        }
        else BeginGarageWork(o.Id);
        return true;
    }
    private ExplorationResult SearchAndTakeTool(GarageObject o)
    {
        _state.SearchObject(_controlledActor.PlayerId,o.Id,ObjectState(o.Id).Version);
        return _state.TakeObject(_controlledActor.PlayerId,o.Id,1,_state.Snapshot(_controlledActor.PlayerId).Version,ObjectState(o.Id).Version);
    }
    public void CloseGarage()
    {
        if(_workTicket!=Guid.Empty)_state.CancelDismantle(_controlledActor.PlayerId,_workTicket);
        _workTicket=Guid.Empty;_workingObject=null;_workElapsed=0;_vanApproval=null;_openObject=null;_awaitVanRelease=false;
        _lootPanel?.Close();_vehiclePanel?.Hide();if(_heldTool!=null)_heldTool.Hide();
        if(_controlledActor!=null)_controlledActor.InventoryOpen=_inventoryVisible||PowerVisible||BeaconVisible||CraftingVisible;
    }
    private void OpenLoot(GarageObject o)
    {
        ClosePower();CloseInventory();_hud.HideHelp();
        var result=_state.SearchObject(_controlledActor.PlayerId,o.Id,ObjectState(o.Id).Version);
        if(result.Status!=ExplorationStatus.Applied){Notify(ExplorationMessage(result.Status));return;}
        _openObject=o.Id;_controlledActor.InventoryOpen=true;
        _lootPanel!.Open(LootSnapshot(o.Id),_state.Resources,true);
        Notify(o.Rule.Name+" — take supplies; leftovers remain.");
    }
    private SuppliesSnapshot LootSnapshot(string id)
    {
        var stock=_state.Snapshot(_controlledActor.PlayerId);var state=ObjectState(id);
        var slots=new ItemStack?[4];if(state.Remaining>0)slots[0]=new(state.Definition.ResourceId,state.Remaining);
        return stock with{Storage=new InventorySnapshot(slots)};
    }
    public ExplorationResult TakeGarageLoot(string id,int amount,SupplyVersion inventory,SupplyVersion source,int destination=-1)
    {
        var o=Garage?.Objects.FirstOrDefault(o=>o.Id==id);
        if(o==null||_openObject!=id||!(_lootPanel?.Visible??false)||!GarageAccess(_controlledActor,o,false))return new(ExplorationStatus.AccessDenied);
        return _state.TakeObject(_controlledActor.PlayerId,id,amount,inventory,source,destination);
    }
    private SupplyResult ApplyLootCommand(SlotRequest request)
    {
        if(_openObject==null||!(_lootPanel?.Visible??false))return new(SupplyStatus.AccessDenied,0,_state.Snapshot(_controlledActor.PlayerId).Version);
        var o=Garage!.Objects.Single(o=>o.Id==_openObject);
        if(!GarageAccess(_controlledActor,o,false))return new(SupplyStatus.AccessDenied,0,_state.Snapshot(_controlledActor.PlayerId).Version);
        if(request.Action==SlotAction.Move&&request.Source.Container==InventoryKind.Personal&&request.Destination.Container==InventoryKind.Personal)
            return _state.ApplySlots(_controlledActor.PlayerId,request);
        bool takeAll=request.Action==SlotAction.DepositAll;
        if(!takeAll&&(request.Source.Container!=InventoryKind.Storage||request.Source.Index!=0||
            request.Action==SlotAction.Move&&request.Destination.Container!=InventoryKind.Personal))
        {Notify("Loot sources are take-only. Use the chest to store supplies.");return new(SupplyStatus.AccessDenied,0,_state.Snapshot(_controlledActor.PlayerId).Version);}
        var result=TakeGarageLoot(_openObject,takeAll?int.MaxValue:request.Quantity,request.Version,ObjectState(_openObject).Version,
            request.Action==SlotAction.Move?request.Destination.Index:-1);
        return new(result.Status switch{ExplorationStatus.Applied=>SupplyStatus.Applied,ExplorationStatus.Stale=>SupplyStatus.Stale,
            ExplorationStatus.NoCapacity=>SupplyStatus.NoCapacity,ExplorationStatus.Empty=>SupplyStatus.Empty,_=>SupplyStatus.AccessDenied},result.Amount,_state.Snapshot(_controlledActor.PlayerId).Version);
    }
    private void ConfirmVan(SupplyVersion expected)
    {
        var o=Garage!.Objects.Single(o=>o.Rule.Kind=="vehicle");
        if(!GarageAccess(_controlledActor,o)||ObjectState(o.Id).Version!=expected){CloseGarage();Notify("Vehicle changed. Inspect it again.");return;}
        _vehiclePanel!.Hide();_openObject=null;_controlledActor.InventoryOpen=false;_vanApproval=expected;
        _awaitVanRelease=_controlledActor.InteractionHeld;
        Notify("Hold E / A at the van. First removed part ends restoration.");
    }
    public ExplorationResult BeginGarageWork(string id)
    {
        var o=Garage?.Objects.FirstOrDefault(o=>o.Id==id);
        if(o==null||GarageMenuVisible||_awaitVanRelease||!GarageAccess(_controlledActor,o))return new(ExplorationStatus.AccessDenied);
        var state=ObjectState(id);
        var result=_state.BeginDismantle(_controlledActor.PlayerId,id,_state.Snapshot(_controlledActor.PlayerId).Version,
            state.RestorationAvailable&&_vanApproval!=null?_vanApproval.Value:state.Version,!state.RestorationAvailable||_vanApproval!=null);
        if(result.Status==ExplorationStatus.Working)
        {
            _workingObject=id;_workTicket=result.Ticket;_workElapsed=0;_controlledActor.InventoryOpen=true;_heldTool?.Show();
        }
        else Notify(ExplorationMessage(result.Status));
        return result;
    }
    private void UpdateGarage(double delta)
    {
        if(Garage==null)return;
        if(_awaitVanRelease&&!_controlledActor.InteractionHeld)_awaitVanRelease=false;
        Discovery!.Observe(_controlledActor.PlayerId,MountainRegion.CellAt(_controlledActor.Position.X,_controlledActor.Position.Y),10);
        foreach(var target in _interactables)target.Visible=PresentationVisible(target)&&(!target.Depleted||target.IsFiniteSource);
        foreach(var prop in GetNode<ThresholdMap>("Map").GetChildren().OfType<SanctuaryProp>())prop.Visible=PresentationVisible(prop);
        foreach(var plot in _plots)plot.Visible=PresentationVisible(plot);
        if(_openObject!=null)
        {
            var o=Garage.Objects.Single(o=>o.Id==_openObject);
            if(!GarageAccess(_controlledActor,o,false)){CloseGarage();return;}
            if(_lootPanel!.Visible)_lootPanel.Refresh(LootSnapshot(_openObject));
        }
        if(_workingObject!=null)
        {
            var o=Garage.Objects.Single(o=>o.Id==_workingObject);
            if(!_controlledActor.InteractionHeld||_controlledActor.RequestedMovement!=Vector2.Zero||!GarageAccess(_controlledActor,o))
            {CloseGarage();Notify("Cancelled. Completed parts stay removed.");return;}
            _workElapsed+=delta;
            var result=_state.AdvanceDismantle(_controlledActor.PlayerId,_workTicket,(long)Math.Round(delta*TimeSpan.TicksPerSecond));
            if(result.Status!=ExplorationStatus.Working)
            {CloseGarage();Notify(result.Status==ExplorationStatus.Applied?$"Salvaged {result.Amount} Scrap.":ExplorationMessage(result.Status));}
        }
        if(_vanApproval!=null)
        {
            var van=Garage.Objects.Single(o=>o.Rule.Kind=="vehicle");if(!GarageAccess(_controlledActor,van,false))_vanApproval=null;
        }
    }
    private string GaragePrompt()
    {
        if(_workingObject!=null)
        {
            var s=ObjectState(_workingObject);return $"Hold E / A  Dismantling {Math.Min(100,(int)(_workElapsed/s.Definition.Seconds*100))}% | Release cancels";
        }
        if(GarageMenuVisible)return "";
        var o=FindGarageObject(_controlledActor);if(o==null)return "";
        var state=ObjectState(o.Id);
        return o.Rule.Kind switch {
            "container"=>$"E / A  {(state.Opened?state.Remaining==0?"Empty":"Open":"Search")} {o.Rule.Name}",
            "tool-rack"=>state.Remaining>0?"E / A  Take salvage tool (1 slot)":"E / A  Tool rack / return carried tool",
            "vehicle" when state.Stage==0&&_vanApproval==null=>"E / A  Inspect van: preserve or dismantle",
            _=>state.Remaining>0?$"Hold E / A  Dismantle {o.Rule.Name}":$"{o.Rule.Name} | Stripped"};
    }
    public static string ExplorationMessage(ExplorationStatus s)=>s switch{
        ExplorationStatus.NoTool=>"Carry the salvage tool. Look for the workshop rack.",
        ExplorationStatus.NoCapacity=>"Need room for this whole part. Nothing removed.",
        ExplorationStatus.Stale=>"Object changed. Inspect and try again.",ExplorationStatus.Empty=>"Nothing remains here.",
        ExplorationStatus.Busy=>"Another actor is working here.",ExplorationStatus.ConsentRequired=>"Inspect the van and confirm dismantling first.",
        _=>"That interaction is unavailable."};
}
