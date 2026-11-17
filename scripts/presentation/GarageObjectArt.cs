using Godot;
using Hollowlight.Core;
using Hollowlight.World;

namespace Hollowlight.Presentation;

public partial class GarageObjectArt : Node2D
{
    public GarageObject Object{get;set;}=null!;
    public ThresholdWorld World{get;set;}=null!;
    private Sprite2D _sprite=null!;
    public override void _Ready()
    {_sprite=GarageAssets.Sprite(Object.Rule.Asset);AddChild(_sprite);RoomAssets.AddContactShadow(this,"garage/"+Object.Rule.Asset,Object.Rule.Kind=="vehicle"?46:22);}
    public override void _Process(double delta)
    {
        var s=World.ObjectState(Object.Id);
        _sprite.Frame=Object.Rule.Kind switch {"vehicle"=>s.Stage,"machinery"=>s.Stage,"tool-rack"=>s.Remaining==0?1:0,_=>s.Remaining==0?2:s.Opened?1:0};
        Visible=World.PresentationVisible(this);
    }
}
