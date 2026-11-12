using System;
using System.Threading.Tasks;
using Godot;
using Hollowlight.Presentation;
using Hollowlight.World;

namespace Hollowlight.Tools;

// Actual movement/render captures at gameplay scale; no substitute display sprites.
public partial class RefugeWalkReview : Node
{
    public override async void _Ready()
    {
        try
        {
            var input = new SmokeInput();
            string entry = ProjectSettings.GetSetting("application/run/main_scene").AsString();
            var refuge = GD.Load<PackedScene>(entry).Instantiate<SanctuaryRoom>();
            refuge.InputOverride = input;
            AddChild(refuge);
            var actor = refuge.World.GetNode<PlayerCharacter>("Player");
            var art = actor.GetNode<RoomCharacter>("RoomCharacter");
            string folder = ProjectSettings.GlobalizePath("user://refuge-walk");
            DirAccess.MakeDirRecursiveAbsolute(folder);
            Vector2[] directions = { Vector2.Down, Vector2.Up, Vector2.Left, Vector2.Right,
                new(1, 1), new(-1, 1), new(1, -1), new(-1, -1) };
            for (int row = 0; row < directions.Length; row++)
            {
                input.Movement = Vector2.Zero;
                await Frames(2);
                actor.Position = new Vector2(176, 208);
                input.Movement = directions[row];
                for (int phase = 0; phase < 4; phase++)
                {
                    await Frames(8);
                    await ToSignal(RenderingServer.Singleton, RenderingServer.SignalName.FramePostDraw);
                    Image image = GetViewport().GetTexture().GetImage();
                    Vector2 anchor = actor.GetGlobalTransformWithCanvas().Origin;
                    float displayScale = (float)image.GetWidth() / GetViewport().GetVisibleRect().Size.X;
                    var rect = new Rect2I((int)(anchor.X * displayScale) - 56,
                        (int)(anchor.Y * displayScale) - 112, 112, 144);
                    if (image.GetRegion(rect).SavePng($"{folder}/{row}-{phase}.png") != Error.Ok)
                        throw new InvalidOperationException("Cannot save walk review frame");
                    if (actor.GetRealVelocity().Length() < 75)
                        throw new InvalidOperationException("Walk review path unexpectedly blocked");
                    GD.Print($"Walk {directions[row]}: frame {art.FrameCoords}, travel {actor.GetRealVelocity()}");
                }
            }
            GD.Print($"PASS: actual eight-direction gameplay walk captures: {folder}");
            GetTree().Quit();
        }
        catch (Exception error) { GD.PushError(error.ToString()); GetTree().Quit(1); }
    }

    private async Task Frames(int count)
    {
        for (int i = 0; i < count; i++)
        {
            await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);
            await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        }
    }
}
