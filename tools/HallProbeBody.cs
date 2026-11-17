using Godot;

namespace Hollowlight.Tools;

// Test-only physical clearance body. Never registered as a player, inventory owner or machine.
public partial class HallProbeBody : CharacterBody2D
{
    public Vector2 Target{get;set;}
    public Vector2 Footprint{get;set;}=new(8,6);
    public bool ShowActor{get;set;}=true;
    private Sprite2D? _sprite;
    private int _frame;
    public override void _Ready()
    {
        MotionMode=MotionModeEnum.Floating;CollisionLayer=2;
        AddChild(new CollisionShape2D{Position=Footprint==new Vector2(8,6)?new(0,1):Vector2.Zero,Shape=new RectangleShape2D{Size=Footprint}});
        if(ShowActor)
        {
            _sprite=new Sprite2D{Texture=GD.Load<Texture2D>("res://assets/sanctuary/character.png"),Hframes=6,Vframes=4,Centered=false,Position=new(-10,-26),Scale=Vector2.One*.5f,TextureFilter=TextureFilterEnum.Nearest};AddChild(_sprite);
        }
    }
    public override void _PhysicsProcess(double delta)
    {
        var d=Target-Position;Velocity=d.Length()>1.5f?d.Normalized()*Mathf.Min(80,d.Length()/(float)delta):Vector2.Zero;MoveAndSlide();
        if(_sprite!=null){_frame++;_sprite.FrameCoords=new(Velocity.LengthSquared()>1?1+(_frame/9)%4:0,Velocity.Y<0?1:0);}
    }
}
