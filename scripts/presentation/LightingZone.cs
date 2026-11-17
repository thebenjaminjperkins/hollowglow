using Godot;

namespace Hollowlight.Presentation;

// Reusable presentation-only region. Future generated rooms emit these alongside their terrain.
// Bounds are local to the node; no overlap with simulation occupancy or power allocation.
public partial class LightingZone : Node2D
{
    [Export] public Rect2 Bounds {get;set;}
    [Export(PropertyHint.Range,"0,1")] public float SkyExposure {get;set;}=1;
    [Export(PropertyHint.Range,"0,1")] public float Shelter {get;set;}
    [Export] public int Priority {get;set;}
}
