using Godot;

namespace Hollowlight.Presentation;

// Reusable editor-placeable scene. Lights are decorative, without power simulation.
public partial class SanctuaryProp : Node2D
{
    [Export] public PixelSprite.Motif Kind { get; set; } = PixelSprite.Motif.Fern;
    [Export] public int Variant { get; set; }
    [Export] public Vector2 Footprint { get; set; } = Vector2.Zero;
    [Export] public float GlowRadius { get; set; }
    [Export] public Color GlowColor { get; set; } = new("efbb79");
    [Export] public float GlowEnergy { get; set; } = 0.5f;
    private PointLight2D? _light;
    private double _clock;
    public override void _Ready()
    {
        if (Kind == PixelSprite.Motif.Generator) AddChild(new BenchmarkSprite());
        else if (Kind == PixelSprite.Motif.Fern) AddChild(new ReactiveFern { Name = "ReactiveFern" });
        else AddChild(new PixelSprite { Kind = Kind, Variant = Variant });
        if (Footprint.X > 0 && Footprint.Y > 0)
        {
            var body = new StaticBody2D { CollisionLayer = 1, CollisionMask = 0 };
            body.AddChild(new CollisionShape2D { Shape = new RectangleShape2D { Size = Footprint } });
            AddChild(body);
        }
        if (GlowRadius > 0)
        {
            var gradient = new Gradient { Colors = new[] {
                new Color(1, 1, 1, 0.8f), new Color(1, 1, 1, 0.3f), new Color(1, 1, 1, 0)
            }, Offsets = new[] { 0f, 0.35f, 1f } };
            var texture = new GradientTexture2D {
                Width = 128, Height = 128, Gradient = gradient,
                Fill = GradientTexture2D.FillEnum.Radial,
                FillFrom = new Vector2(0.5f, 0.5f), FillTo = new Vector2(1, 0.5f)
            };
            _light = new PointLight2D {
                Texture = texture, TextureScale = GlowRadius / 64,
                Position = new Vector2(0, Kind == PixelSprite.Motif.Lamp ? -18 : -6),
                Color = GlowColor, Energy = GlowEnergy, BlendMode = Light2D.BlendModeEnum.Add,
                RangeZMin = -10, RangeZMax = 10
            };
            AddChild(_light);
        }
        SetProcess(Kind == PixelSprite.Motif.Mushrooms && _light != null);
    }
    public override void _Process(double delta)
    {
        _clock += delta;
        if (_light != null) _light.Energy = GlowEnergy * (1 + 0.06f * Mathf.Sin((float)_clock * 0.8f + Position.X));
    }
}
