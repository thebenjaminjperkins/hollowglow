using System;
using Godot;

namespace Hollowlight.Presentation;

// All wall renderers use a ground-contact sort origin, independent of texture dimensions or collision bodies.
public static class WallAssembly
{
    public static Node2D Face(Node parent,Vector2 ground,Texture2D texture,Rect2 region,Vector2 offset,Vector2? scale=null,Rect2? footprint=null,float rotation=0)
    {
        var root=new Node2D{Position=ground};root.SetMeta("wall_surface",true);
        if(footprint!=null)root.SetMeta("surface_footprint",footprint.Value);
        root.AddChild(new Sprite2D{Texture=new AtlasTexture{Atlas=texture,Region=region},Centered=false,Position=offset,
            Scale=scale??Vector2.One*.5f,Rotation=rotation,TextureFilter=CanvasItem.TextureFilterEnum.Nearest});parent.AddChild(root);return root;
    }
    public static void CaveFace(Node parent,int x,int y,Texture2D texture,int patch)
    {Face(parent,new(x*16+8,y*16+16),texture,new(patch*32,0,32,32),new(-8,-16));}
    public static Node2D Overhead(Node parent,Vector2 ground,Texture2D texture,Rect2 region,Vector2 offset,Rect2 footprint)
    {
        var root=Face(parent,ground,texture,region,offset,footprint:footprint);
        root.ZIndex=-1;root.SetMeta("overhead_surface",true);return root;
    }
    public static void Sill(Node parent,Vector2 ground,bool vertical=false)
    {
        var root=Face(parent,ground,GarageAssets.Get("fuel-cabinet").Texture,new(18,54,32,8),vertical?new(2,-8):new(-8,-2),
            footprint:vertical?new Rect2(-2,-8,4,16):new Rect2(-8,-2,16,4),rotation:vertical?Mathf.Pi/2:0);
        root.ZIndex=-2;
    }
    public static void Masonry(Node parent,int x,int y,Func<int,int,bool> wall)
    {
        bool left=wall(x-1,y),right=wall(x+1,y),up=wall(x,y-1),down=wall(x,y+1);
        var ground=new Vector2(x*16+8,y*16+16);
        var material=GarageAssets.Get("wall").Texture;int sample=(x&1)==0?4:28;
        // A complete 16-unit thickness in either orientation; no narrow posts or overlapping L crops.
        Face(parent,ground,material,new(sample,32,32,28),new(-8,-14));
        Face(parent,ground,material,new(sample,32,32,4),new(-8,-16));
        if(!up)Face(parent,ground,material,new(16,27,32,4),new(-8,-16));
        if(!left)Face(parent,ground,material,new(16,27,32,4),new(-6,-16),rotation:Mathf.Pi/2);
        if(!right)Face(parent,ground,material,new(16,27,32,4),new(8,-16),rotation:Mathf.Pi/2);
        if(!down)Face(parent,ground,material,new(sample,58,32,2),new(-8,-1));
    }
}
