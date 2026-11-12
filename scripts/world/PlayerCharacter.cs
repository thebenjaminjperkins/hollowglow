using System;
using Godot;
using Hollowlight.Inputs;

namespace Hollowlight.World;

public partial class PlayerCharacter : CharacterBody2D
{
    [Export] public int PlayerId { get; set; } = 1;
    [Export] public WorldConfig Config { get; set; } = null!;
    [Export] public NodePath InputPath { get; set; } = new("KeyboardInput");
    public event Action<PlayerCharacter>? InteractionRequested;
    public int ControllerDevice => _input is KeyboardInput keyboard ? keyboard.ControllerDevice : 0;
    public bool InventoryOpen { get; set; }
    private PlayerInput _input = null!;

    public override void _Ready() => _input = GetNode<PlayerInput>(InputPath);

    public override void _PhysicsProcess(double delta)
    {
        Velocity = InventoryOpen ? Vector2.Zero : _input.ReadMovement() * Config.MoveSpeed;
        MoveAndSlide();
        bool interact = _input.InteractPressed(); // Consume edges while the local menu is open.
        if (interact && !InventoryOpen) InteractionRequested?.Invoke(this);
    }
}
