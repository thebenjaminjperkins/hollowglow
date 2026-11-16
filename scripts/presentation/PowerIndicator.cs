using Godot;
using Hollowlight.World;

namespace Hollowlight.Presentation;

// Small status diode, not a replacement machine asset. Body material art stays unchanged.
public partial class PowerIndicator : Node2D
{
    public ThresholdWorld World { get; set; } = null!;
    public string Consumer { get; set; } = "generator";
    private Color _color = new("79533e");
    public override void _Ready() { Position=new Vector2(8,-12);ZIndex=1; }
    public override void _Process(double delta)
    {
        var p=World.Electrical;
        bool next=p!=null && (Consumer=="generator"?p.Running:System.Linq.Enumerable.Any(p.Consumers,c=>c.Id==Consumer&&c.Powered));
        var signal=Consumer=="beacon"?World.Signal:null;
        var color=new Color(signal?.Status==Hollowlight.Core.SignalStatus.Interrupted?"d4ad73":next?"93b9a0":"79533e");
        if(color!=_color){_color=color;QueueRedraw();}
    }
    public override void _Draw()
    {
        DrawRect(new Rect2(-1,-1,4,3),new Color("202832"));
        DrawRect(new Rect2(0,0,2,1),_color);
    }
}
