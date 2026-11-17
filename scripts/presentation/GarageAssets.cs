using System.Collections.Generic;
using System.Text.Json;
using Godot;

namespace Hollowlight.Presentation;

public static class GarageAssets
{
    private static readonly Dictionary<string,RoomAssets.Definition> Cache=new();
    public static RoomAssets.Definition Get(string name)
    {
        if(Cache.Count==0)
        {
            using var doc=JsonDocument.Parse(FileAccess.GetFileAsString("res://assets/garage/manifest.json"));
            foreach(var entry in doc.RootElement.GetProperty("sprites").EnumerateObject())
            {
                var v=entry.Value;
                Cache[entry.Name]=new(GD.Load<Texture2D>("res://assets/garage/"+entry.Name+".png"),
                    new(v.GetProperty("size")[0].GetInt32(),v.GetProperty("size")[1].GetInt32()),
                    new(v.GetProperty("anchor")[0].GetInt32(),v.GetProperty("anchor")[1].GetInt32()),v.GetProperty("columns").GetInt32());
            }
        }
        return Cache[name];
    }
    public static Sprite2D Sprite(string name)
    {
        var a=Get(name);return new(){Texture=a.Texture,Hframes=a.Columns,Centered=false,Position=-a.Anchor*.5f,
            Scale=Vector2.One*.5f,TextureFilter=CanvasItem.TextureFilterEnum.Nearest};
    }
    public static void AddToolIcon(InventoryAppearance appearance)=>appearance.Icons[Hollowlight.Core.GaragePlan.Tool]=new(){Path="res://assets/garage/tool-icon.png",Region=new[]{0,0,32,40}};
}
