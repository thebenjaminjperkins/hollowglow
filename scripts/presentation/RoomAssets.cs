using System.Collections.Generic;
using System.Text.Json;
using Godot;

namespace Hollowlight.Presentation;

public static class RoomAssets
{
    public sealed record Definition(Texture2D Texture, Vector2 Size, Vector2 Anchor, int Columns);
    private static readonly Dictionary<string, Definition> Cache = new();
    public static Definition Get(string name)
    {
        if (Cache.Count == 0)
        {
            using var document = JsonDocument.Parse(FileAccess.GetFileAsString("res://assets/sanctuary/manifest.json"));
            foreach (var entry in document.RootElement.GetProperty("sprites").EnumerateObject())
            {
                var value = entry.Value;
                Cache[entry.Name] = new Definition(
                    GD.Load<Texture2D>("res://assets/sanctuary/" + value.GetProperty("file").GetString()),
                    new Vector2(value.GetProperty("size")[0].GetInt32(), value.GetProperty("size")[1].GetInt32()),
                    new Vector2(value.GetProperty("anchor")[0].GetInt32(), value.GetProperty("anchor")[1].GetInt32()),
                    value.GetProperty("columns").GetInt32());
            }
        }
        return Cache[name];
    }
    public static Sprite2D MakeSprite(string name)
    {
        var definition = Get(name);
        return new Sprite2D { Texture = definition.Texture, Centered = false, Hframes = definition.Columns,
            Position = -definition.Anchor * 0.5f, Scale = Vector2.One * 0.5f,
            TextureFilter = CanvasItem.TextureFilterEnum.Nearest };
    }
    public static void AddContactShadow(Node2D owner, float width)
    {
        // An adjustable soft contact shadow is an effect, not baked room illumination.
        var gradient = new Gradient { Colors = new[] { new Color(0.05f, 0.07f, 0.09f, 0.35f), new Color(0, 0, 0, 0) } };
        owner.AddChild(new Sprite2D { ZIndex = -1, Texture = new GradientTexture2D {
            Width = 64, Height = 16, Gradient = gradient, Fill = GradientTexture2D.FillEnum.Radial,
            FillFrom = Vector2.One * 0.5f, FillTo = new Vector2(1, 0.5f)
        }, Scale = new Vector2(width / 64, 0.25f), Position = new Vector2(0, 1) });
    }
}
