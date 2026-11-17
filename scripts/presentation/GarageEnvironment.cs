using Godot;
using Hollowlight.Core;
using System.Linq;

namespace Hollowlight.Presentation;

public partial class GarageEnvironment : Node2D
{
    public GaragePlan Plan{get;set;}=null!;
    public override void _Ready()
    {
        var walls=Plan.Definition.Rows.SelectMany((row,y)=>row.Select((c,x)=>(c,x,y))).Where(p=>p.c=='#').ToArray();
        int west=walls.Min(p=>p.x),south=walls.Max(p=>p.y);
        AddChild(new LightingZone{Bounds=new((Plan.Origin.X+walls.Min(p=>p.x))*16,(Plan.Origin.Y+walls.Min(p=>p.y))*16,
            (walls.Max(p=>p.x)-walls.Min(p=>p.x)+1)*16,(walls.Max(p=>p.y)-walls.Min(p=>p.y)+1)*16),Shelter=1,Priority=10});
        var atlas=new TileSetAtlasSource{Texture=GD.Load<Texture2D>("res://assets/garage/floor.png"),TextureRegionSize=new(32,32)};
        for(int y=0;y<16;y++)for(int x=0;x<16;x++)atlas.CreateTile(new(x,y));
        var tiles=new TileSet{TileSize=new(32,32)};tiles.AddSource(atlas,0);
        var floor=new TileMapLayer{TileSet=tiles,Scale=Vector2.One*.5f,ZIndex=-4,CollisionEnabled=false,NavigationEnabled=false,TextureFilter=TextureFilterEnum.Nearest};AddChild(floor);
        for(int y=0;y<Plan.Definition.Height;y++)for(int x=0;x<Plan.Definition.Width;x++)
        {
            int wx=Plan.Origin.X+x,wy=Plan.Origin.Y+y;
            if(!Plan.Building(wx,wy))continue;
            int material=x<7?0:x>12?1:2;
            floor.SetCell(new(wx,wy),0,new((wx&7)+(material%2)*8,(wy&7)+(material/2)*8));
            if(!Plan.Wall(wx,wy))
            {
                if(y==south)WallAssembly.Sill(GetParent(),new(wx*16+8,wy*16+16));
                else if(x==west)WallAssembly.Sill(GetParent(),new(wx*16,wy*16+8),true);
                continue;
            }
            WallAssembly.Masonry(GetParent(),wx,wy,Plan.Wall);
        }
    }
}
