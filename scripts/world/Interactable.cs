using Godot;
using Hollowlight.Presentation;

namespace Hollowlight.World;

public partial class Interactable : StaticBody2D
{
    public string ObjectId { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public string ResourceId { get; set; } = "";
    public string PresentationAsset { get; set; } = "salvage";
    public bool IsFiniteSource { get; set; }
    public bool Depleted { get; private set; }
    public bool IsStorage { get; set; }
    public PixelSprite Art { get; private set; } = null!;

    public void Initialize(PixelSprite.Motif motif)
    {
        CollisionLayer = 1;
        CollisionMask = 0;
        Art = new PixelSprite { Kind = motif };
        AddChild(Art);
        AddChild(new CollisionShape2D {
            Shape = new RectangleShape2D { Size = new Vector2(12, 7) },
            Position = new Vector2(0, 1)
        });
    }

    public void SetCollected()
    {
        Depleted = true;
        if (!IsFiniteSource) Hide();
        CollisionLayer = 0;
    }
}
