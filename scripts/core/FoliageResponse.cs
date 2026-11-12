using System;

namespace Hollowlight.Core;

public sealed class FoliageResponse
{
    private double _time = 1;
    private float _direction = 1;
    public int Column { get; private set; }
    public void Brush(float direction) { _direction = direction < 0 ? -1 : 1; _time = 0; Update(0); }
    public void Update(double delta)
    {
        _time += delta;
        float bend = _time >= 0.9 ? 0 : _direction * (float)(Math.Exp(-4 * _time) * Math.Cos(8 * _time));
        Column = Math.Abs(bend) < 0.12f ? 0 : bend < 0 ? (bend < -0.55f ? 1 : 2) : (bend > 0.55f ? 4 : 3);
    }
}
