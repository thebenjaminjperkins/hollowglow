using Godot;
using Hollowlight.Core;
using Hollowlight.World;

namespace Hollowlight.Presentation;

// Mapping/sight stay in WorldDiscovery. Surface materials mask ground-projected artwork;
// no high-Z rectangles clip wall tops or canopies. Hiding this node is a diagnostic switch.
public partial class ExplorationFog : Node2D
{
    public WorldDiscovery Discovery{get;set;}=null!;
    public PlayerCharacter Observer{get;set;}=null!;
    public override void _Process(double delta)
    {
        var visuals=GetParent().GetNodeOrNull<WorldVisuals>("WorldVisuals");
        if(visuals!=null)visuals.DiscoveryEnabled=IsVisibleInTree();
    }
}
