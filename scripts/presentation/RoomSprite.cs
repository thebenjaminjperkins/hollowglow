using Godot;
using Hollowlight.Core;
using Hollowlight.World;

namespace Hollowlight.Presentation;

public partial class RoomSprite : Node2D
{
    public string Asset { get; set; } = "stones";
    public bool Reactive { get; set; }
    public Interactable? InteractionOwner { get; set; }
    public FoliageResponse Response { get; } = new();
    public int BrushCount { get; private set; }
    private Sprite2D _sprite = null!;
    public override void _Ready()
    {
        _sprite = RoomAssets.MakeSprite(Asset);
        AddChild(_sprite);
        if (Asset != "vine") RoomAssets.AddContactShadow(this,"sanctuary/"+Asset, Mathf.Min(RoomAssets.Get(Asset).Size.X * 0.35f, 23));
        if (Reactive)
        {
            var area = new Area2D { CollisionLayer = 0, CollisionMask = 2 };
            area.AddChild(new CollisionShape2D { Position = new Vector2(0, -3), Shape = new RectangleShape2D { Size = new Vector2(18, 12) } });
            area.BodyEntered += body =>
            {
                if (body is not CharacterBody2D actor || actor.Velocity.LengthSquared() < 1) return;
                BrushCount++;
                Response.Brush(Mathf.Abs(actor.Velocity.X) > 0.1f ? actor.Velocity.X : GlobalPosition.X - actor.GlobalPosition.X);
            };
            AddChild(area);
        }
        SetProcess(Reactive || InteractionOwner != null);
    }
    public override void _Process(double delta)
    {
        if (Reactive) { Response.Update(delta); _sprite.Frame = Response.Column; }
        if (InteractionOwner != null)
            _sprite.Modulate = InteractionOwner.Art.Highlight ? new Color(1.18f, 1.12f, 0.98f) : Colors.White;
    }
}
