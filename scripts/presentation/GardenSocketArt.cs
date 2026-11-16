using Godot;
using Hollowlight.Core;
using Hollowlight.World;

namespace Hollowlight.Presentation;

// A contextual placement marker. The installed tray reuses the approved bed PNG.
public partial class GardenSocketArt : Node2D
{
    public ThresholdWorld World {get;set;}=null!;
    public override void _Process(double delta)
    {
        Visible=World.GardenMarkerVisible;
        QueueRedraw();
    }
    public override void _Draw()
    {
        if(!Visible)return;
        foreach(int x in new[]{-1,1})foreach(int y in new[]{-1,1})
        {
            var corner=new Vector2(x*15,-16+y*5);
            DrawLine(corner,corner-new Vector2(x*4,0),new Color("93b9a0"),.5f);
            DrawLine(corner,corner-new Vector2(0,y*2),new Color("93b9a0"),.5f);
        }
    }
}
