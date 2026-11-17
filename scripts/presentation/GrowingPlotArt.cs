using System;
using System.Collections.Generic;
using System.Text.Json;
using Godot;
using Hollowlight.Core;
using Hollowlight.World;

namespace Hollowlight.Presentation;

// Reusable view of a stable plot ID. Simulation and inventory stay owned by World.
public partial class GrowingPlotArt : Node2D
{
    public string PlotId { get; set; } = "";
    public ThresholdWorld World { get; set; } = null!;
    private static Dictionary<string, int>? _rows;
    private Sprite2D _crop = null!;
    public override void _Ready()
    {
        if (_rows == null)
        {
            using var manifest = JsonDocument.Parse(FileAccess.GetFileAsString("res://assets/farming/manifest.json"));
            _rows = new Dictionary<string, int>();
            foreach (var row in manifest.RootElement.GetProperty("rows").EnumerateObject()) _rows.Add(row.Name, row.Value.GetInt32());
        }
        _crop = new Sprite2D { Texture = GD.Load<Texture2D>("res://assets/farming/crops.png"), Centered = false,
            Hframes = 3, Vframes = 2, Position = new Vector2(-8, -35), Scale = Vector2.One * 0.5f,
            TextureFilter = TextureFilterEnum.Nearest };
        AddChild(_crop);
        World.GetNodeOrNull<WorldVisuals>("WorldVisuals")?.Register(this,"crop");
    }
    public override void _Process(double delta)
    {
        var state = World.Plot(PlotId);
        _crop.Visible = state.Stage != CropStage.Empty;
        if (_crop.Visible)
        {
            int row = _rows![state.CropId!];
            _crop.FrameCoords = new Vector2I((int)state.Stage - 1, row);
            float pulse = state.CropId == "glowcap" && state.Stage == CropStage.Mature
                ? 1 + 0.025f * (float)Math.Sin(World.Clock.Snapshot().TotalSeconds * 0.8) : 1;
            _crop.Modulate = new Color(pulse, pulse, pulse);
        }
        QueueRedraw();
    }
    public override void _Draw()
    {
        if (World.FocusedPlotId != PlotId || World.InventoryVisible) return;
        var color = new Color("d4ad73");
        foreach (int x in new[] { -1, 1 }) foreach (int y in new[] { -1, 1 })
        {
            Vector2 corner = new(x * 7.5f, -16 + y * 3.5f);
            DrawLine(corner, corner - new Vector2(x * 2, 0), color, 0.5f);
            DrawLine(corner, corner - new Vector2(0, y * 1.5f), color, 0.5f);
        }
    }
}
