using System.Collections.Generic;
using System.Text.Json;
using Godot;
using Hollowlight.Core;

namespace Hollowlight.Presentation;

public partial class RegionProp : Node2D
{
    public RegionDecoration Placement {get;set;}=null!;
    private static readonly Dictionary<string,(Vector2 Anchor,Texture2D Texture)> Cache=new();
    public override void _Ready()
    {
        if(Placement.Asset is "fern" or "stones" or "mushrooms")
        {AddChild(new RoomSprite {Asset=Placement.Asset,Reactive=Placement.Asset=="fern"});return;}
        string key=Placement.Asset=="ruin"&&Placement.Variant==1?"corner":Placement.Asset;
        if(!Cache.TryGetValue(key,out var asset))
        {
            using var manifest=JsonDocument.Parse(FileAccess.GetFileAsString("res://assets/region/manifest.json"));
            var item=manifest.RootElement.GetProperty("sprites").GetProperty(key);
            asset=(new Vector2(item.GetProperty("anchor")[0].GetInt32(),item.GetProperty("anchor")[1].GetInt32()),
                GD.Load<Texture2D>("res://assets/region/"+item.GetProperty("file").GetString()));Cache.Add(key,asset);
        }
        AddChild(new Sprite2D {Texture=asset.Texture,Centered=false,Position=-asset.Anchor*.5f,Scale=Vector2.One*.5f,TextureFilter=TextureFilterEnum.Nearest});
        RoomAssets.AddContactShadow(this,Placement.Asset=="tree"?20:32);
    }
}
