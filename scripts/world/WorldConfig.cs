using Godot;

namespace Hollowlight.World;

[GlobalClass]
public partial class WorldConfig : Resource
{
    [Export(PropertyHint.Range, "30,180,1")] public float MoveSpeed { get; set; } = 80;
    [Export(PropertyHint.Range, "8,32,1")] public float InteractionRange { get; set; } = 22;
    [Export(PropertyHint.Range, "1,256,1")] public int StorageSlots { get; set; } = 24;
    [Export] public string LayoutProfile { get; set; } = "compact";
    [Export] public bool CraftingEnabled { get; set; } = true;
    [Export] public bool RescueEnabled { get; set; } = true;
    [Export] public bool PowerEnabled { get; set; } = true;
    [Export] public bool HungerEnabled { get; set; } = true;
    [Export] public bool FarmingEnabled { get; set; } = true;
    [Export] public bool ScavengingEnabled { get; set; } = true;
    [Export] public int WorldSeed { get; set; } = 1701;
    // Reserved seed for reproducible decoration only. This slice has an authored map.
    [Export] public int DecorationSeed { get; set; } = 1701;
}
