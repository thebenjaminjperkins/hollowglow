using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Hollowlight.Core;

namespace Hollowlight.Presentation;

// Generated cells and adjacency select PNG atlas regions; no painted whole-map background.
// Static collision runs are built from the same region ground field by ThresholdMap.
public partial class MountainTerrain : Node2D
{
    public MountainRegion Region {get;set;}=null!;
    public TileMapLayer Surface {get;private set;}=null!;
    public override void _Ready()
    {
        var set=Atlas("res://assets/region/terrain.png",64,96);
        Surface=Layer("Meadow",set,-5);
        var overlays=Layer("TerrainTransitions",set,-4);
        var rockSet=Atlas("res://assets/sanctuary/wall.png",4,2);
        var caveRock=Layer("CaveMass",rockSet,-6);caveRock.Modulate=new Color(.23f,.25f,.27f);
        var surfaceRock=Layer("MountainMass",rockSet,-6);surfaceRock.Modulate=new Color(.76f,.80f,.74f);
        var faceTexture=GD.Load<Texture2D>("res://assets/sanctuary/wall.png");
        var caveFloor=Layer("SanctuaryFloor",Atlas("res://assets/sanctuary/floor.png",32,16),-5);
        for(int y=0;y<Region.Height;y++)for(int x=0;x<Region.Width;x++)
        {
            if(Region.Garage?.Building(x,y)==true)continue;
            bool outside=x>=Region.Definition.SurfaceOffsetX;
            if(!outside&&(x>=Region.Definition.SanctuaryWidth||y>=Region.Definition.SanctuaryHeight))continue;
            var cell=new Vector2I(x,y);var kind=Region.Ground[x,y];int phase=(x&7)+(y&7)*8;
            if(kind==GroundKind.Rock)
            {
                int patch=(x&1)+(y&1)*2;
                (outside?surfaceRock:caveRock).SetCell(cell,0,new Vector2I(patch,1));
                if(Region.Layout.IsFloor(x,y+1))WallAssembly.CaveFace(GetParent(),x,y,faceTexture,patch);
                continue;
            }
            if(!outside)
            {
                int mask=(Region.Layout.CutsCorner(x,y,-1,-1)?1:0)|(Region.Layout.CutsCorner(x,y,1,-1)?2:0)|
                    (Region.Layout.CutsCorner(x,y,1,1)?4:0)|(Region.Layout.CutsCorner(x,y,-1,1)?8:0);
                int index=((x&3)+(y&3)*4)*16+mask;caveFloor.SetCell(cell,0,new Vector2I(index%32,index/32));continue;
            }
            Set(Surface,cell,0,phase,0);
            int material=kind switch {GroundKind.Road=>1,GroundKind.Water=>2,GroundKind.Gravel=>3,GroundKind.Soil=>4,GroundKind.Bridge=>5,_=>0};
            if(material==0)continue;
            bool Same(int xx,int yy)=>xx>=0&&yy>=0&&xx<Region.Width&&yy<Region.Height&&Region.Ground[xx,yy]==kind;
            int edge=kind==GroundKind.Bridge?0:(!Same(x,y-1)?1:0)|(!Same(x+1,y)?2:0)|(!Same(x,y+1)?4:0)|(!Same(x-1,y)?8:0);
            Set(overlays,cell,material,phase,edge);
        }
    }
    private static void Set(TileMapLayer layer,Vector2I cell,int material,int phase,int mask)
    {int index=(material*64+phase)*16+mask;layer.SetCell(cell,0,new Vector2I(index%64,index/64));}
    private static TileSet Atlas(string path,int cols,int rows)
    {
        var source=new TileSetAtlasSource {Texture=GD.Load<Texture2D>(path),TextureRegionSize=new Vector2I(32,32)};
        for(int y=0;y<rows;y++)for(int x=0;x<cols;x++)source.CreateTile(new Vector2I(x,y));
        var set=new TileSet {TileSize=new Vector2I(32,32)};set.AddSource(source,0);return set;
    }
    private TileMapLayer Layer(string name,TileSet set,int z)
    {
        var layer=new TileMapLayer {Name=name,TileSet=set,Scale=Vector2.One*.5f,ZIndex=z,
            CollisionEnabled=false,NavigationEnabled=false,OcclusionEnabled=false,TextureFilter=CanvasItem.TextureFilterEnum.Nearest};
        AddChild(layer);return layer;
    }
}
