using Godot;

namespace Hollowlight.Presentation;

public partial class BenchmarkSprite : Node2D
{
    private Texture2D _sheet = null!;
    public override void _Ready()
    {
        _sheet = GD.Load<Texture2D>("res://assets/benchmark/machine.png");
        TextureFilter = TextureFilterEnum.Nearest;
    }
    public override void _Draw()
    {
        DrawRect(new Rect2(-12, -1, 24, 4), new Color("20283266"));
        DrawTextureRect(_sheet, new Rect2(-16, -29, 32, 32), false);
    }
}
