using Godot;

namespace Hollowlight.World;

[GlobalClass]
public partial class WorldConfig : Resource
{
    [Export(PropertyHint.Range, "30,180,1")] public float MoveSpeed { get; set; } = 80;
    [Export(PropertyHint.Range, "8,32,1")] public float InteractionRange { get; set; } = 22;
    [Export(PropertyHint.Range, "1,256,1")] public int StorageSlots { get; set; } = 24;
    [Export] public bool FarmingEnabled { get; set; } = true;
    // Reserved seed for reproducible decoration only. This slice has an authored map.
    [Export] public int DecorationSeed { get; set; } = 1701;
}
