using System;
using System.Collections.Generic;
using Godot;

namespace Hollowlight.Presentation;

// Viewport-space chrome. Inventory/modal grids retain their existing explicit coordinate system.
public partial class HudChrome : Control
{
    private readonly Dictionary<string,(ColorRect Panel,Label Text)> _parts=new();
    private ColorRect _phase=null!;
    public IEnumerable<ColorRect> Panels {get{foreach(var p in _parts.Values)yield return p.Panel;}}
    public override void _Ready()
    {
        MouseFilter=MouseFilterEnum.Ignore;TextureFilter=TextureFilterEnum.Nearest;
        foreach(string id in new[]{"bag","hunger","clock","location","prompt","feedback","meal","debug"})
        {
            var panel=new ColorRect{Color=new("222a2ee8"),MouseFilter=MouseFilterEnum.Ignore};AddChild(panel);
            var label=new Label{Position=new(9,4),MouseFilter=MouseFilterEnum.Ignore,ClipText=true,
                AutowrapMode=TextServer.AutowrapMode.WordSmart,MaxLinesVisible=2,TextOverrunBehavior=TextServer.OverrunBehavior.TrimEllipsis};
            label.AddThemeFontSizeOverride("font_size",12);label.AddThemeColorOverride("font_color",new(id is "hunger" or "location"?"b8cbb5":id=="feedback"?"c7d4b6":id=="prompt"?"f0d7a3":"ead4a9"));panel.AddChild(label);
            _parts[id]=(panel,label);
        }
        _phase=new ColorRect{Color=new("d4ad73"),MouseFilter=MouseFilterEnum.Ignore};_parts["clock"].Panel.AddChild(_phase);
    }
    private void Place(string id,Rect2 rect)
    {var p=_parts[id];p.Panel.Position=rect.Position;p.Panel.Size=rect.Size;p.Text.Size=rect.Size-new Vector2(18,8);}
    public void Present(string bag,string hunger,string clock,string location,string prompt,string feedback,string meal,string debug,
        bool locationVisible,bool mealVisible,bool showDebug,float phase,Color phaseColor)
    {
        var size=GetViewportRect().Size;Size=size;
        foreach(var pair in new[]{("bag",bag),("hunger",hunger),("clock",clock),("location",location),("prompt",prompt),("feedback",feedback),("meal",meal),("debug",debug)})
            _parts[pair.Item1].Text.Text=pair.Item2;
        Place("bag",new(12,12,52,28));Place("hunger",new(72,12,92,28));Place("clock",new(size.X-204,12,192,28));
        _parts["hunger"].Panel.Visible=hunger!="";
        _parts["hunger"].Text.AddThemeColorOverride("font_color",new(hunger is "Critical" or "Starving"?"e8ab85":"b8cbb5"));
        float width=Math.Min(560,size.X-24);
        float TextWidth(string id,string text)=>_parts[id].Text.GetThemeDefaultFont().GetStringSize(text,HorizontalAlignment.Left,-1,12).X+20;
        float locationWidth=Math.Clamp(TextWidth("location",location),100,Math.Min(420,width));
        Place("location",new((size.X-locationWidth)/2,48,locationWidth,TextWidth("location",location)>locationWidth?44:28));
        float promptWidth=Math.Clamp(TextWidth("prompt",prompt),180,width),promptHeight=TextWidth("prompt",prompt)>width?44:28;
        Place("prompt",new((size.X-promptWidth)/2,size.Y-12-promptHeight,promptWidth,promptHeight));
        float feedbackWidth=Math.Clamp(TextWidth("feedback",feedback),180,width),feedbackHeight=TextWidth("feedback",feedback)>width?44:28;
        Place("feedback",new((size.X-feedbackWidth)/2,size.Y-12-feedbackHeight-(prompt!=""?promptHeight+8:0),feedbackWidth,feedbackHeight));
        Place("meal",new((size.X-width)/2,size.Y-44,width,32));
        Place("debug",new(12,94,size.X-24,58));
        _parts["location"].Panel.Visible=locationVisible;
        _parts["prompt"].Panel.Visible=prompt!="";_parts["feedback"].Panel.Visible=feedback!="";
        _parts["meal"].Panel.Visible=mealVisible;_parts["debug"].Panel.Visible=showDebug;
        _phase.Position=new(2,26);_phase.Size=new(Mathf.Floor(188*Math.Clamp(phase,0,1)),2);_phase.Color=phaseColor;
    }
}
