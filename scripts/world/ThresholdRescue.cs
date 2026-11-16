using Godot;
using Hollowlight.Core;
using Hollowlight.Presentation;

namespace Hollowlight.World;

public partial class ThresholdWorld
{
    private BeaconPanel _beaconPanel=null!;
    private RescuedScreen _rescuedScreen=null!;
    private bool _victoryShown;
    public bool RescueEnabled=>Config.RescueEnabled&&Config.PowerEnabled;
    public RescueSnapshot? Signal=>RescueEnabled?_state.Rescue():null;
    public bool BeaconVisible=>_beaconPanel!=null&&_beaconPanel.Visible;
    public BeaconPanel BeaconControls=>_beaconPanel;
    public RescuedScreen VictoryScreen=>_rescuedScreen;
    private void InitializeRescueWorld()
    {
        if(RescueEnabled)_state.InitializeRescue(RescueDefinition.Parse(FileAccess.GetFileAsString("res://data/rescue.json")));
    }
    private void InitializeBeaconPanel()
    {
        _beaconPanel=new BeaconPanel();_hud.AddChild(_beaconPanel);
        _beaconPanel.CloseRequested+=ClosePower;
        _beaconPanel.OperationRequested+=HandleBeaconCommand;
        _beaconPanel.AllocationRequested+=OpenBeaconAllocation;
        _rescuedScreen=new RescuedScreen();_hud.AddChild(_rescuedScreen);
    }
    private void OpenBeacon(PowerStation station)
    {
        ClosePower();CloseInventory();_hud.HideHelp();_openStation=station;
        _controlledActor.InventoryOpen=true;_beaconPanel.ControllerDevice=_controlledActor.ControllerDevice;
        _beaconPanel.Open(_state.Rescue(),_state.Power(),_state.Snapshot(_controlledActor.PlayerId).Version);
    }
    private void OpenBeaconAllocation(){if(_openStation!=null)OpenPower(_openStation);}
    private void HandleBeaconCommand(PowerRequest request)=>_beaconPanel.ShowResult(PowerCommand(_controlledActor,request));
    private void RefreshBeacon()
    {
        if(BeaconVisible)_beaconPanel.Refresh(_state.Rescue(),_state.Power(),_state.Snapshot(_controlledActor.PlayerId).Version);
    }
    private void ShowRescued()
    {
        if(_victoryShown)return;_victoryShown=true;
        ClosePower();CloseInventory();_hud.HideHelp();
        _controlledActor.Velocity=Vector2.Zero;
        foreach(var child in _hud.GetChildren())if(child is CanvasItem item && item!=_rescuedScreen)item.Hide();
        _rescuedScreen.ControllerDevice=_controlledActor.ControllerDevice;_rescuedScreen.Present();
        GetTree().Paused=true;
    }
}
