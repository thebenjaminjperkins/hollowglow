using Godot;
using System.Collections.Generic;

using Hollowlight.Core;

namespace Hollowlight.Presentation;

public partial class ThresholdHud : CanvasLayer
{
    private Label _hunger = null!, _mealInfo = null!;
    private Label _clock = null!;
    private ColorRect _mealPanel = null!;
    private ColorRect _phaseBar = null!;
    private Label _status = null!, _location = null!, _prompt = null!, _feedback = null!, _debug = null!;
    private ColorRect _promptPanel = null!, _feedbackPanel = null!, _locationPanel = null!;
    private Control _help = null!;
    private string _lastLocation = "";
    private double _locationTime;
    public SuppliesPanel Supplies { get; private set; } = null!;

    public override void _Ready()
    {
        Panel(new Rect2(7, 7, 38, 19));
        AddChild(new Sprite2D { Texture = new AtlasTexture { Atlas = GD.Load<Texture2D>("res://assets/sanctuary/character.png"),
            Region = new Rect2(13, 76, 15, 17) }, Position = new Vector2(11, 11), Centered = false, Scale = Vector2.One * 0.5f,
            TextureFilter = CanvasItem.TextureFilterEnum.Nearest });
        _status = Label(new Vector2(23, 10), 9, "ead4a9");
        Panel(new Rect2(49, 7, 59, 19));
        _hunger = Label(new Vector2(54, 10), 8, "b8cbb5");
        _mealPanel = Panel(new Rect2(7, 190, 370, 19));
        _mealInfo = Label(new Vector2(12, 192), 8, "ead4a9");
        Panel(new Rect2(112, 7, 128, 19));
        _clock = Label(new Vector2(118, 10), 8, "ead4a9");
        _phaseBar = new ColorRect { Position = new Vector2(113, 24), Size = new Vector2(0, 1),
            Color = new Color("d4ad73"), MouseFilter = Control.MouseFilterEnum.Ignore };
        AddChild(_phaseBar);
        _locationPanel = Panel(new Rect2(247, 7, 130, 19));
        _location = Label(new Vector2(253, 10), 9, "b8cbb5");
        _promptPanel = Panel(new Rect2(62, 191, 260, 19));
        _prompt = Label(new Vector2(70, 194), 9, "f0d7a3");
        _feedbackPanel = Panel(new Rect2(16, 171, 352, 17));
        _feedback = Label(new Vector2(22, 172), 9, "c7d4b6");
        _debug = Label(new Vector2(9, 32), 9, "e1e3c2");
        _help = new Control { Visible = false, MouseFilter = Control.MouseFilterEnum.Ignore };
        AddChild(_help);
        _help.AddChild(new ColorRect { Position = new Vector2(77, 61), Size = new Vector2(230, 102),
            Color = new Color("202b30f5"), MouseFilter = Control.MouseFilterEnum.Ignore });
        var helpText = MakeLabel(new Vector2(88, 67), 10, "ead4a9");
        helpText.Text = "SHARED SUPPLIES\nWASD / stick Move   E / A Interact\nTab / View Backpack   Esc / B Close\nSelect a stack, then its destination.\nShift-click / Q / X Quick transfer\nRight-click / Space / Y Split";
        _help.AddChild(helpText);
        Supplies = new SuppliesPanel { Visible = false };
        AddChild(Supplies);
    }
    public void HideHelp() => _help.Hide();
    public void ToggleHelp() { if (!Supplies.Visible) _help.Visible = !_help.Visible; }
    public override void _Process(double delta)
    {
        _locationTime = System.Math.Max(0, _locationTime - delta);
        _location.Visible = _locationPanel.Visible = _locationTime > 0;
    }
    private ColorRect Panel(Rect2 rect)
    {
        var panel = new ColorRect { Position = rect.Position, Size = rect.Size, Color = new Color("222a2ee8"),
            MouseFilter = Control.MouseFilterEnum.Ignore };
        AddChild(panel);
        panel.AddChild(new ColorRect { Size = new Vector2(rect.Size.X, 1), Color = new Color("87785b"),
            MouseFilter = Control.MouseFilterEnum.Ignore });
        return panel;
    }
    private static Label MakeLabel(Vector2 position, int size, string color)
    {
        var label = new Label { Position = position, MouseFilter = Control.MouseFilterEnum.Ignore };
        label.AddThemeFontSizeOverride("font_size", size);
        label.AddThemeColorOverride("font_color", new Color(color));
        label.AddThemeColorOverride("font_shadow_color", new Color("1c242a"));
        label.AddThemeConstantOverride("shadow_offset_y", 1);
        return label;
    }
    private Label Label(Vector2 position, int size, string color)
    {
        var label = MakeLabel(position, size, color);
        AddChild(label);
        return label;
    }
    public void Present(string location, SuppliesSnapshot supplies,
        string prompt, string feedback, string debug, bool showDebug, ClockSnapshot clock, HungerSnapshot? hunger = null)
    {
        long seconds = clock.PhaseElapsedTicks / System.TimeSpan.TicksPerSecond;
        _clock.Text = $"Day {clock.DayNumber}  {clock.Phase} {seconds / 60:00}:{seconds % 60:00}";
        _phaseBar.Size = new Vector2(Mathf.Floor(126f * clock.PhaseElapsedTicks / clock.PhaseDurationTicks), 1);
        _phaseBar.Color = new Color(clock.Phase == DayPhase.Day ? "d4ad73" : clock.Phase == DayPhase.Dusk ? "ab7b4f" : "638e85");
        _status.Text = $"{supplies.Personal.UsedSlots}/6";
        _hunger.Text = hunger == null ? "" : $"{hunger.Stage}";
        _hunger.Modulate = hunger?.Stage is HungerStage.Critical or HungerStage.Starving ? new Color("e8ab85") : Colors.White;
        _mealInfo.Visible = _mealPanel.Visible = Supplies.Visible && hunger != null;
        if (hunger != null)
        {
            long until = System.Math.Max(0, (hunger.NextMealTicks - clock.ElapsedTicks) / System.TimeSpan.TicksPerSecond);
            _mealInfo.Text = $"Today {hunger.Eaten}/{hunger.Required} | Dawn {until / 60:00}:{until % 60:00} | Missed {hunger.MissedMeals} | Speed {hunger.SpeedMultiplier:0%}";
        }
        Supplies.RefreshMeal(hunger);
        Supplies.Refresh(supplies);
        if (location != _lastLocation)
        {
            _lastLocation = location;
            _location.Text = location;
            _locationTime = 3;
        }
        Toast(_prompt, _promptPanel, prompt, 191);
        _prompt.Visible = _promptPanel.Visible = prompt.Length > 0 && !Supplies.Visible;
        Toast(_feedback, _feedbackPanel, Supplies.Visible ? "" : feedback, 171);
        _feedback.Visible = _feedbackPanel.Visible = _feedback.Text.Length > 0;
        _debug.Text = debug;
        _debug.Visible = showDebug;
    }
    private static void Toast(Label label, ColorRect panel, string text, int y)
    {
        if (label.Text == text) return;
        label.Text = text;
        float width = Mathf.Clamp(label.GetThemeDefaultFont().GetStringSize(text, HorizontalAlignment.Left, -1, 9).X + 16, 80, 360);
        panel.Position = new Vector2(Mathf.Floor((384 - width) / 2), y);
        panel.Size = new Vector2(width, 19);
        label.Position = panel.Position + new Vector2(8, 2);
        panel.GetChild<ColorRect>(0).Size = new Vector2(width, 1);
    }
}
