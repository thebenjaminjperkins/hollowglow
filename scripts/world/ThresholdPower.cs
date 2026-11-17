using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Hollowlight.Core;
using Hollowlight.Presentation;

namespace Hollowlight.World;

public sealed record PowerStation(string Id, string Label, Vector2 Position);
public partial class ThresholdWorld
{
    private PowerDefinition? _powerDefinition;
    private readonly List<PowerStation> _stations = new();
    private PowerStation? _focusedStation, _openStation;
    private bool _wasGeneratorRunning;
    private PowerPanel _powerPanel = null!;
    public SourceWorld? FuelWorld { get; private set; }
    public PowerSnapshot? Electrical => Config.PowerEnabled ? _state.Power() : null;
    public IReadOnlyList<PowerStation> PowerStations => _stations;
    public bool PowerVisible => _powerPanel != null && _powerPanel.Visible;
    public PowerPanel PowerControls => _powerPanel;
    private bool ModalOpen => _inventoryVisible || PowerVisible || BeaconVisible || CraftingVisible || GarageMenuVisible || Dismantling;

    private void PreparePowerSources()
    {
        if(!Config.PowerEnabled)return;
        _powerDefinition=PowerDefinition.Parse(FileAccess.GetFileAsString("res://data/power.json"));
        if(Region!=null){FuelWorld=new(Config.WorldSeed,Region.Sources.Where(s=>s.Site.ResourceId=="fuel").ToArray());return;}
        var ground=ScavengingWorld?.Sources.Select(s=>s.Site).ToArray() ?? Array.Empty<GatheringSite>();
        var ids=ground.Select(g=>g.Id).ToHashSet();
        FuelWorld=FiniteScavenging.Generate(ScavengingSetup.Parse(FileAccess.GetFileAsString("res://data/fuel_scavenging.json")),
            GetNode<ThresholdMap>("Map").Layout,_setup.Sites.Where(s=>!ids.Contains(s.Id)),Config.WorldSeed,ground);
        _setup=_setup with { Sites=_setup.Sites.Concat(FuelWorld.Sources.Select(s=>s.Site)).ToArray() };
    }
    private void InitializeElectrical()
    {
        if(_powerDefinition==null)return;
        _state.InitializePower(_powerDefinition,Clock);
        var props=GetNode<ThresholdMap>("Map").GetChildren().OfType<SanctuaryProp>();
        foreach(var prop in props)
        {
            string? id=prop.Kind switch { PixelSprite.Motif.Generator=>"generator",PixelSprite.Motif.Bench=>"workbench",PixelSprite.Motif.Shelf=>"beacon",_=>null };
            if(id==null)continue;
            _stations.Add(new(id,id=="generator"?"generator":id=="beacon"?"radio beacon":"workbench",prop.Position+new Vector2(0,12)));
            prop.AddChild(new PowerIndicator { World=this, Consumer=id });
        }
        var lamp=props.First(p=>p.Kind==PixelSprite.Motif.Lamp);
        _stations.Add(new("lighting","lighting controls",lamp.Position+new Vector2(0,12)));
    }
    private void InitializePowerPanel()
    {
        _powerPanel=new PowerPanel();_hud.AddChild(_powerPanel);
        _powerPanel.CloseRequested+=ClosePower;
        _powerPanel.OperationRequested+=HandlePowerCommand;
    }
    private PowerStation? FindStation(PlayerCharacter actor)
    {
        PowerStation? nearest=null;float best=Config.InteractionRange*Config.InteractionRange;
        foreach(var station in _stations)
        {
            if(!CurrentlyVisible(station.Position))continue;
            float distance=actor.GlobalPosition.DistanceSquaredTo(station.Position);
            if(distance>best)continue;
            if(GetWorld2D().DirectSpaceState.IntersectRay(PhysicsRayQueryParameters2D.Create(actor.GlobalPosition,station.Position,1)).Count>0)continue;
            nearest=station;best=distance;
        }
        return nearest;
    }
    private void OpenPower(PowerStation station)
    {
        ClosePower();CloseInventory();_hud.HideHelp();_openStation=station;
        _controlledActor.InventoryOpen=true;_powerPanel.ControllerDevice=_controlledActor.ControllerDevice;
        _powerPanel.Open(_state.Power(),_state.Snapshot(_controlledActor.PlayerId).Version,station.Id);
    }
    public void ClosePower()
    {
        if(_craftPanel!=null)_craftPanel.Close();if(_powerPanel!=null)_powerPanel.Close();if(_beaconPanel!=null)_beaconPanel.Close();_openStation=null;
        if(_controlledActor!=null)_controlledActor.InventoryOpen=_inventoryVisible;
    }
    public PowerResult PowerCommand(PlayerCharacter actor, PowerRequest request)
    {
        if(actor!=_controlledActor || !(PowerVisible||BeaconVisible||CraftingVisible) || _openStation==null || actor.GlobalPosition.DistanceTo(_openStation.Position)>Config.InteractionRange)
            return new(PowerStatus.AccessDenied,0,0,_powerDefinition?.Capacity??0);
        return _state.ApplyPower(actor.PlayerId,request);
    }
    private void HandlePowerCommand(PowerRequest request) => _powerPanel.ShowResult(PowerCommand(_controlledActor,request));
    private void UpdateElectrical()
    {
        var state=Electrical;
        if(state!=null)
        {
            if(_wasGeneratorRunning&&!state.Running&&state.Fuel==0)Notify("Generator out of Fuel. Refill and restart manually.");
            _wasGeneratorRunning=state.Running;
        }
        _focusedStation=FindStation(_controlledActor);
        float distance=_focusedStation==null?float.MaxValue:_controlledActor.Position.DistanceSquaredTo(_focusedStation.Position);
        if(_focused!=null && _controlledActor.Position.DistanceSquaredTo(_focused.Position)<=distance) _focusedStation=null;
        if(_focusedPlot!=null && _controlledActor.Position.DistanceSquaredTo(_focusedPlot.Position)<=distance) _focusedStation=null;
        if(_focusedStation!=null) { _focused=null;_focusedPlot=null; }
        if((PowerVisible||BeaconVisible||CraftingVisible) && (_openStation==null || _controlledActor.Position.DistanceTo(_openStation.Position)>Config.InteractionRange))ClosePower();
    }
}
