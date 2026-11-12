using Godot;
using Hollowlight.Core;

namespace Hollowlight.Presentation;

public partial class ReactiveFern : Node2D
{
    public FoliageResponse Response { get; } = new();
    public int BrushCount { get; private set; }
    private Texture2D _sheet = null!;
    public override void _Ready()
    {
        _sheet = GD.Load<Texture2D>("res://assets/benchmark/fern.png");
        TextureFilter = TextureFilterEnum.Nearest;
        var area = new Area2D { CollisionLayer = 0, CollisionMask = 2 };
        area.AddChild(new CollisionShape2D { Position = new Vector2(0, -3), Shape = new RectangleShape2D { Size = new Vector2(18, 12) } });
        area.BodyEntered += body =>
        {
            if (body is not CharacterBody2D actor || actor.Velocity.LengthSquared() < 1) return;
            Brush(Mathf.Abs(actor.Velocity.X) > 0.1f ? actor.Velocity.X : GlobalPosition.X - actor.GlobalPosition.X);
        };
        AddChild(area);
    }
    public void Brush(float direction) { BrushCount++; Response.Brush(direction); QueueRedraw(); }
    public override void _Process(double delta)
    {
        int before = Response.Column;
        Response.Update(delta);
        if (before != Response.Column) QueueRedraw();
    }
    public override void _Draw()
    {
        DrawRect(new Rect2(-7, -1, 14, 3), new Color("20283244"));
        DrawTextureRectRegion(_sheet, new Rect2(-16, -26, 32, 28), new Rect2(Response.Column * 32, 0, 32, 28));
    }
}
