using Godot;

namespace Hollowlight.Inputs;

public partial class KeyboardInput : PlayerInput
{
    [Export] public int ControllerDevice { get; set; } = 0;
    private bool _wasPressed;
    public override bool InteractHeld() => Input.IsPhysicalKeyPressed(Key.E) || Input.IsJoyButtonPressed(ControllerDevice, JoyButton.A);
    public override Vector2 ReadMovement()
    {
        float x = (Held(Key.D, Key.Right) ? 1 : 0) - (Held(Key.A, Key.Left) ? 1 : 0);
        float y = (Held(Key.S, Key.Down) ? 1 : 0) - (Held(Key.W, Key.Up) ? 1 : 0);
        var keyboard = new Vector2(x, y).LimitLength();
        if (keyboard != Vector2.Zero) return keyboard;
        foreach (int device in Input.GetConnectedJoypads())
        {
            if (device != ControllerDevice) continue;
            var stick = new Vector2(Input.GetJoyAxis(device, JoyAxis.LeftX), Input.GetJoyAxis(device, JoyAxis.LeftY));
            if (stick.Length() > 0.25f) return stick.Normalized();
        }
        return Vector2.Zero;
    }

    public override bool InteractPressed()
    {
        bool pressed = Input.IsPhysicalKeyPressed(Key.E);
        foreach (int device in Input.GetConnectedJoypads())
            if (device == ControllerDevice) pressed |= Input.IsJoyButtonPressed(device, JoyButton.A);
        bool edge = pressed && !_wasPressed;
        _wasPressed = pressed;
        return edge;
    }

    private static bool Held(Key first, Key second) =>
        Input.IsPhysicalKeyPressed(first) || Input.IsPhysicalKeyPressed(second);
}
