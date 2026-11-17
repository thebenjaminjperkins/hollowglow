using Godot;
using Hollowlight.Inputs;

namespace Hollowlight.Tools;

// Used only by the headless integration scene. Normal play uses KeyboardInput.
public partial class SmokeInput : PlayerInput
{
    public Vector2 Movement { get; set; }
    public bool Tap { get; set; }
    public bool Hold {get;set;}
    public override bool InteractHeld()=>Hold;
    public override Vector2 ReadMovement() => Movement.LimitLength();
    public override bool InteractPressed()
    {
        bool result = Tap;
        Tap = false;
        return result;
    }
}
