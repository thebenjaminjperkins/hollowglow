using Godot;
using Hollowlight.Core;

namespace Hollowlight.Presentation;

// Presentation selects authored sheet frames; it never changes actor motion or collision.
public partial class CharacterArt : Node2D
{
    public WalkPresentation Animation { get; } = new();
    private CharacterBody2D _body = null!;
    private Texture2D _sheet = null!;
    public override void _Ready()
    {
        _body = GetParent<CharacterBody2D>();
        _sheet = GD.Load<Texture2D>("res://assets/benchmark/character.png");
        TextureFilter = TextureFilterEnum.Nearest;
    }
    public override void _Process(double delta)
    {
        var facing = Animation.Facing;
        int column = Animation.Column;
        Animation.Update(delta, _body.Velocity.X, _body.Velocity.Y);
        if (facing != Animation.Facing || column != Animation.Column) QueueRedraw();
    }
    public override void _Draw()
    {
        DrawRect(new Rect2(-5, -1, 10, 3), new Color("20283266"));
        DrawTextureRectRegion(_sheet, new Rect2(-10, -26, 20, 28),
            new Rect2(Animation.Column * 20, (int)Animation.Facing * 28, 20, 28));
    }
}
