using Godot;
using Hollowlight.World;

namespace Hollowlight.Presentation;

// PNG material shading stays separate from live ambient/point lighting. No simulation here.
public partial class ScavengingArt : Node2D
{
    public Interactable Source { get; set; } = null!;
    private Sprite2D _sprite = null!;
    public int StateFrame => _sprite.Frame;
    public override void _Ready()
    {
        _sprite = new Sprite2D { Texture = GD.Load<Texture2D>($"res://assets/scavenging/{Source.PresentationAsset}.png"),
            Hframes = 2, Centered = false, Position = new Vector2(-10, -18), Scale = Vector2.One * 0.5f,
            TextureFilter = CanvasItem.TextureFilterEnum.Nearest };
        AddChild(_sprite);
        RoomAssets.AddContactShadow(this, 12);
    }
    public override void _Process(double delta)
    {
        _sprite.Frame = Source.Depleted ? 1 : 0;
        _sprite.Modulate = Source.Art.Highlight && !Source.Depleted ? new Color(1.18f, 1.12f, 0.98f) : Colors.White;
    }
}
