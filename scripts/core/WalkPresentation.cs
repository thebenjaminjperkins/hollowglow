using System;

namespace Hollowlight.Core;

public enum WalkFacing { South, North, West, East }

public sealed class WalkPresentation
{
    public WalkFacing Facing { get; private set; }
    public int Column { get; private set; }
    private bool _walking;
    private double _clock;
    public void Update(double delta, float x, float y)
    {
        bool walking = x * x + y * y > 1;
        if (walking != _walking) { _clock = 0; _walking = walking; }
        _clock += delta;
        if (walking)
        {
            // Keep the previous axis around exact diagonals; sign still follows travel.
            float difference = Math.Abs(x) - Math.Abs(y);
            bool horizontal = difference > 0.05f ||
                (Math.Abs(difference) <= 0.05f && Facing is WalkFacing.West or WalkFacing.East);
            Facing = horizontal ? (x < 0 ? WalkFacing.West : WalkFacing.East)
                : (y < 0 ? WalkFacing.North : WalkFacing.South);
        }
        Column = walking ? 2 + (int)(_clock * 8) % 4 : (int)(_clock * 0.7) % 2;
    }
}
