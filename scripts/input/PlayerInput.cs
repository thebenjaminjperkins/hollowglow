using Godot;

namespace Hollowlight.Inputs;

// Each actor receives an input source; world state does not read keyboard state.
public abstract partial class PlayerInput : Node
{
    public abstract Vector2 ReadMovement();
    public abstract bool InteractPressed();
    public virtual bool InteractHeld() => false;
}
