using Godot;
using Hollowlight.Core;
using Hollowlight.World;

namespace Hollowlight.Presentation;

public partial class RoomCharacter : Node2D
{
    private readonly WalkPresentation _animation = new();
    private PlayerCharacter _body = null!;
    private Sprite2D _sprite = null!;
    public override void _Ready()
    {
        _body = GetParent<PlayerCharacter>();
        _sprite = new Sprite2D { Texture = GD.Load<Texture2D>("res://assets/sanctuary/character.png"),
            Centered = false, Hframes = 6, Vframes = 4, Position = new Vector2(-10, -26),
            Scale = Vector2.One * 0.5f, TextureFilter = TextureFilterEnum.Nearest };
        AddChild(_sprite);
        RoomAssets.AddContactShadow(this, 12);
    }
    public Vector2I FrameCoords => _sprite.FrameCoords;

    public override void _PhysicsProcess(double delta)
    {
        // Animate travel after MoveAndSlide, including wall slides, rather than input intent.
        Vector2 travel = _body.GetRealVelocity();
        float speed = travel.Length();
        double cadence = speed > 1 ? speed / _body.Config.MoveSpeed : 1;
        _animation.Update(delta * cadence, travel.X, travel.Y);
        _sprite.FrameCoords = new Vector2I(_animation.Column, (int)_animation.Facing);
    }
}
