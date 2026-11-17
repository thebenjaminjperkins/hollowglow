using System;
using Godot;
using Hollowlight.Core;

namespace Hollowlight.World;

public partial class ThresholdWorld
{
    public MountainRegion? Region=>GetNode<ThresholdMap>("Map").Region;
    public bool OnSurface=>Region?.IsSurface(_controlledActor.Position.X,_controlledActor.Position.Y)??_controlledActor.Position.X>=ThresholdMap.SurfaceStart;
    public Vector2 StoragePosition=>Region==null?new(88,224):Point(Region.Anchor("chest"));
    public Vector2 HomePosition=>Home;
    public Vector2 SurfaceLanding=>Outside;
    private ColorRect? _thresholdFade;
    private double _portalCooldown,_fadeAmount;
    private static Vector2 Point(GridPoint p)=>new(p.WorldX,p.WorldY);
    private void InitializeRegionView()
    {
        if(Region==null)return;
        _controlledActor.Position=Home;SetRegionCamera();
        _thresholdFade=new ColorRect {Size=new Vector2(384,216),Color=new Color(0,0,0,0),MouseFilter=Control.MouseFilterEnum.Ignore};
        _hud.AddChild(_thresholdFade);
        Notify("Mountain region review. The lit approach leads outside.");
    }
    private void SetRegionCamera()
    {
        if(Region==null)return;
        var camera=_controlledActor.GetNode<Camera2D>("Camera2D");bool surface=OnSurface;
        camera.LimitLeft=surface?Region.Definition.SurfaceOffsetX*16:0;camera.LimitTop=0;
        camera.LimitRight=(surface?Region.Width:Region.Definition.SanctuaryWidth)*16;
        camera.LimitBottom=(surface?Region.Definition.SurfaceHeight:Region.Definition.SanctuaryHeight)*16;
        camera.ResetSmoothing();
    }
    private bool OutsideRegionBounds(Vector2 p)=>Region!=null&&(!Region.InBounds(p.X,p.Y)||
        !(Region.IsSurface(p.X,p.Y)||p.X<Region.Definition.SanctuaryWidth*16&&p.Y<Region.Definition.SanctuaryHeight*16));
    private void UpdateRegionTravel(double delta)
    {
        if(Region==null)return;
        _portalCooldown=Math.Max(0,_portalCooldown-delta);_fadeAmount=Math.Max(0,_fadeAmount-delta*5);
        if(_thresholdFade!=null)_thresholdFade.Color=new Color(.08f,.11f,.13f,(float)_fadeAmount);
        if(_portalCooldown>0||ModalOpen)return;
        Vector2 threshold=OnSurface?Point(Region.Entrance.Cell):Point(Region.Anchor("insidePortal"));
        if(_controlledActor.Position.DistanceTo(threshold)>9)return;
        bool wasOutside=OnSurface;
        Teleport(wasOutside?Point(Region.Anchor("insideLanding")):Point(Region.OutsideLanding));
        _portalCooldown=.6;_fadeAmount=.85;
        Notify(wasOutside?"Mountain sanctuary":"Mountain forecourt");
    }
}
