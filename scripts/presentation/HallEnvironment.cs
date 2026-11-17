using Godot;
using Hollowlight.Core;
using Hollowlight.World;

namespace Hollowlight.Presentation;

// Only architecture and inert dressing. Reservations are data, not installed plots/machines.
public partial class HallEnvironment : Node2D
{
    public SanctuaryPlan Plan{get;set;}=null!;
    public override void _Ready()
    {
        foreach(var fixture in Plan.Fixtures)
        {
            var p=Plan.FixtureFoot(fixture);var root=new Node2D{Name=fixture.Id,Position=new(p.WorldX,p.WorldY)};
            root.SetMeta("surface_family",fixture.Asset);
            var shell=RoomAssets.MakeSprite(fixture.Asset);shell.Modulate=new(.68f,.76f,.78f);root.AddChild(shell);
            var pad=RoomAssets.MakeSprite("bedroll");pad.Position+=new Vector2(0,18);root.AddChild(pad);
            RoomAssets.AddContactShadow(root,"sanctuary/"+fixture.Asset,32);GetParent().AddChild(root);
        }
        int doorX=(Plan.Offset[0]+Plan.Entrance[0])*16,doorY=(Plan.Offset[1]+Plan.Entrance[1])*16;
        var steel=GarageAssets.Get("fuel-cabinet").Texture;
        // Thick masonry/steel abutments sit outside the six-tile opening. No leaf, collider or trigger spans it.
        foreach(int x in new[]{doorX-16,doorX+96})
        {
            for(int y=doorY-32;y<doorY+16;y+=16)WallAssembly.Masonry(GetParent(),x/16,y/16,(xx,yy)=>xx==x/16&&yy>=doorY/16-2&&yy<doorY/16+1);
            for(int segment=0;segment<2;segment++)
                WallAssembly.Face(GetParent(),new(x+8,doorY+16),steel,new(9,34,8,48),new(x<doorX?4:-8,-48+segment*24),footprint:new(-8,-48,16,48));
        }
        // The overhead lintel is shown in cutaway. It casts no artificial ground-level sight/light barrier.
        for(int i=0;i<6;i++)WallAssembly.Overhead(GetParent(),new(doorX+i*16+8,doorY+16),steel,new(18,36,32,24),new(-8,-48),new(-8,-1,16,2));
    }
}
