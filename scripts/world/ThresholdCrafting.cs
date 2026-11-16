using System;
using System.Linq;
using Godot;
using Hollowlight.Core;
using Hollowlight.Presentation;

namespace Hollowlight.World;

public partial class ThresholdWorld
{
    private CraftingSetup? _craftSetup;
    private CraftingPanel _craftPanel=null!;
    private Vector2 _gardenSocket;
    private bool _expansionShown;
    public bool CraftingEnabled=>Config.CraftingEnabled&&Config.PowerEnabled&&Config.FarmingEnabled;
    public bool CraftingVisible=>_craftPanel!=null&&_craftPanel.Visible;
    public CraftingPanel CraftingControls=>_craftPanel;
    public CraftingSnapshot CraftingFor(int playerId)=>_state.CraftingFor(playerId);
    public Vector2 GardenSocket=>_gardenSocket;
    public bool GardenMarkerVisible=>CraftingEnabled&&!_expansionShown&&_controlledActor.Position.DistanceTo(_gardenSocket)<32;
    private bool AtGardenSocket(PlayerCharacter actor)=>CraftingEnabled&&!_expansionShown&&actor.Position.DistanceTo(_gardenSocket)<=Config.InteractionRange&&
        GetWorld2D().DirectSpaceState.IntersectRay(PhysicsRayQueryParameters2D.Create(actor.Position,_gardenSocket,1)).Count==0;
    private void PrepareCrafting()
    {if(CraftingEnabled)_craftSetup=CraftingSetup.Parse(FileAccess.GetFileAsString("res://data/crafting.json"));}
    private void InitializeCraftingWorld()
    {
        if(_craftSetup==null)return;
        _state.InitializeCrafting(_craftSetup);
        var garden=GetNode<ThresholdMap>("Map").GetChildren().OfType<SanctuaryProp>().First(p=>p.Kind==PixelSprite.Motif.Garden);
        _gardenSocket=garden.Position+new Vector2(_craftSetup.SocketOffset.X,_craftSetup.SocketOffset.Y);
        var layout=GetNode<ThresholdMap>("Map").Layout;
        if(!layout.IsFloor((int)_gardenSocket.X/16,(int)_gardenSocket.Y/16))throw new InvalidOperationException("Garden expansion requires floor.");
        AddChild(new GardenSocketArt {World=this,Position=_gardenSocket});
    }
    private void InitializeCraftingPanel()
    {
        _craftPanel=new CraftingPanel();_hud.AddChild(_craftPanel);
        _craftPanel.CloseRequested+=ClosePower;_craftPanel.AllocationRequested+=OpenCraftAllocation;
        _craftPanel.CraftRequested+=HandleCraft;
        _hud.Supplies.InstallRequested+=HandleInstall;
    }
    private void OpenCrafting(PowerStation station)
    {
        ClosePower();CloseInventory();_hud.HideHelp();_openStation=station;_controlledActor.InventoryOpen=true;
        _craftPanel.ControllerDevice=_controlledActor.ControllerDevice;
        _craftPanel.Open(_state.CraftingFor(_controlledActor.PlayerId),_state.Snapshot(_controlledActor.PlayerId),_state.Resources);
    }
    private void OpenCraftAllocation(){if(_openStation!=null)OpenPower(_openStation);}
    private void HandleCraft(CraftRequest request)=>_craftPanel.ShowResult(CraftAtWorkbench(_controlledActor,request));
    public CraftResult CraftAtWorkbench(PlayerCharacter actor,CraftRequest request)
    {
        if(!CraftingEnabled||actor!=_controlledActor||!CraftingVisible||_openStation?.Id!="workbench"||
            actor.Position.DistanceTo(_openStation.Position)>Config.InteractionRange)
            return new(CraftStatus.AccessDenied,_state.Snapshot(_controlledActor.PlayerId).Version);
        return _state.Craft(actor.PlayerId,request);
    }
    public CraftResult InstallUpgrade(PlayerCharacter actor,InstallRequest request)
    {
        CraftResult Denied()=>new(CraftStatus.AccessDenied,_state.Snapshot(_controlledActor.PlayerId).Version);
        if(!CraftingEnabled||actor!=_controlledActor||_victoryShown||CraftingVisible||PowerVisible||BeaconVisible)return Denied();
        var inventory=_state.Snapshot(actor.PlayerId).Personal;
        if(request.Slot<0||request.Slot>=inventory.Capacity)return new(CraftStatus.InvalidSlot,_state.Snapshot(actor.PlayerId).Version);
        string? item=inventory.Slots[request.Slot]?.ResourceId;
        if(item==_craftSetup!.BedKit)
        {
            if(!AtGardenSocket(actor))return Denied();
            var result=_state.InstallBed(actor.PlayerId,request);RefreshExpansion();return result;
        }
        if(!_inventoryVisible)return Denied();
        if(item!=_craftSetup.BackpackKit)return new(CraftStatus.WrongItem,_state.Snapshot(actor.PlayerId).Version);
        return _state.InstallBackpack(actor.PlayerId,request);
    }
    private void HandleInstall(InstallRequest request)
    {
        if(!_inventoryVisible)return;
        var result=InstallUpgrade(_controlledActor,request);_hud.Supplies.ShowInstallResult(result);
        Notify(result.Status==CraftStatus.Applied?"Upgrade installed.":CraftingPanel.Message(result.Status));
    }
    private void UseGardenSocket(PlayerCharacter actor)
    {
        var stock=_state.Snapshot(actor.PlayerId);
        int slot=Enumerable.Range(0,stock.Personal.Capacity).FirstOrDefault(i=>stock.Personal.Slots[i]?.ResourceId==_craftSetup!.BedKit,-1);
        if(slot<0){Notify("Craft a Growing-Bed Kit at the powered workbench.");return;}
        var result=InstallUpgrade(actor,new(slot,stock.Version));Notify(result.Status==CraftStatus.Applied?"Shared growing plot installed. Plant a seed.":CraftingPanel.Message(result.Status));
    }
    private void RefreshExpansion()
    {
        if(!CraftingEnabled||_expansionShown||!_state.CraftingFor(_controlledActor.PlayerId).BedInstalled)return;
        _expansionShown=true;
        var plot=new GrowingPlotArt {Name=SharedSupplies.ExpansionPlotId,PlotId=SharedSupplies.ExpansionPlotId,World=this,Position=_gardenSocket};
        plot.AddChild(new Sprite2D {Texture=GD.Load<Texture2D>("res://assets/farming/bed.png"),Centered=false,
            Position=new Vector2(-18,-31),Scale=Vector2.One*.5f,TextureFilter=CanvasItem.TextureFilterEnum.Nearest,ZIndex=-1});
        _plots.Add(plot);AddChild(plot);
    }
    private void RefreshCrafting()
    {
        if(!CraftingEnabled)return;
        RefreshExpansion();
        if(CraftingVisible)_craftPanel.Refresh(_state.CraftingFor(_controlledActor.PlayerId),_state.Snapshot(_controlledActor.PlayerId),_state.Resources);
    }
}
