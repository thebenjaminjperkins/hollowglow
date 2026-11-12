using Godot;

namespace Hollowlight.Presentation;

// Original modular pixel motifs; origin is at the feet. No baked room backgrounds.
public partial class PixelSprite : Node2D
{
    public enum Motif { Person, Storage, Salvage, Generator, Garden, Plant, Ruin, Lamp, Vine, Shelf, Bench, Bedroll, Fern, Mushrooms, Cable, Stones, Root, Rug, Deck }
    [Export] public Motif Kind { get; set; }
    [Export] public int Variant { get; set; }
    private bool _highlight;
    public bool Highlight
    {
        get => _highlight;
        set { if (_highlight != value) { _highlight = value; QueueRedraw(); } }
    }
    private void Box(float x, float y, float w, float h, string color) => DrawRect(new Rect2(x, y, w, h), new Color(color));
    private void Line(int x, int y, int x2, int y2, string color) => DrawLine(new Vector2(x, y), new Vector2(x2, y2), new Color(color));
    public override void _Draw()
    {
        if (Kind != Motif.Vine && Kind != Motif.Cable && Kind != Motif.Root && Kind != Motif.Rug && Kind != Motif.Deck)
            Box(-9, -1, 18, 4, "20252c55");
        switch (Kind)
        {
            case Motif.Storage:
                Box(-12, -16, 24, 16, "302d31"); Box(-11, -15, 22, 5, "b18b5d");
                Box(-10, -14, 20, 2, "d4ae70"); Box(-11, -9, 22, 8, "765441");
                Box(-9, -8, 18, 6, "906747"); Box(-8, -7, 2, 5, "ad8052");
                Box(6, -7, 2, 5, "ad8052"); Box(-12, -10, 24, 2, "48424a");
                Box(-2, -10, 4, 6, "dfbb78"); Box(-1, -8, 2, 2, "56504b");
                Box(-12, -3, 3, 3, "606b6b"); Box(9, -3, 3, 3, "606b6b");
                break;
            case Motif.Salvage:
                Box(-9, -9, 18, 9, "333d42"); Box(-8, -10, 14, 7, "7d8a82");
                Box(-7, -9, 12, 2, "a1ad99"); Box(-6, -6, 6, 3, "3c5053");
                Box(-5, -6, 2, 2, "a6c7b0"); Box(3, -6, 2, 2, "b18b5d");
                Box(-8, -2, 15, 2, "515f5b"); Line(6, -9, 10, -14, "9da995");
                Box(-11, -1, 3, 2, "9b947c"); Box(9, -3, 3, 2, "737f72");
                break;
            case Motif.Generator:
                Box(-15, -20, 30, 19, "30353d"); Box(-13, -19, 26, 17, "586967");
                Box(-13, -19, 26, 3, "9aab95"); Box(-13, -15, 26, 2, "71877b");
                Box(-10, -13, 12, 9, "283d42"); Box(-9, -12, 10, 7, "72b3a3");
                Line(-8, -8, -5, -8, "b4dec1"); Line(-5, -8, -3, -11, "b4dec1");
                Box(5, -12, 5, 3, "cca16b"); Box(5, -7, 2, 2, "b96c50");
                Box(9, -7, 2, 2, "c9cf9e"); Box(-10, -3, 18, 2, "3b4c50");
                Box(-11, 0, 4, 2, "35363c"); Box(7, 0, 4, 2, "35363c");
                Box(-11, -28, 5, 8, "343e43"); Box(-10, -27, 3, 6, "778278");
                Box(12, -14, 4, 2, "c49061"); Box(14, -12, 2, 8, "7e6656");
                break;
            case Motif.Garden:
                Box(-15, -11, 30, 13, "433d37"); Box(-14, -10, 28, 10, "735b42");
                Box(-12, -9, 24, 7, "3c3c34"); Box(-15, -11, 30, 2, "b09061");
                Box(-15, -3, 30, 4, "8f6e4c"); Box(-13, -2, 26, 1, "b99562");
                for (int i = -8; i <= 8; i += 8) { Leaf(i, -6, i % 3); Box(i, -4, 1, 2, "7c8f59"); }
                break;
            case Motif.Lamp:
                Box(-2, -24, 4, 24, "484447"); Box(-1, -23, 1, 21, "9d8059");
                Box(-5, -25, 10, 3, "5b5b53"); Box(-4, -22, 8, 9, "be945d");
                Box(-3, -21, 6, 6, "eac68a"); Box(-2, -20, 4, 4, "fff0b1");
                Box(-5, -14, 10, 2, "655d50"); Box(-5, 0, 10, 2, "45464a");
                break;
            case Motif.Plant:
            case Motif.Fern:
                for (int i = -2; i <= 2; i++)
                {
                    Line(0, 0, i * 4, -8 - (2 - System.Math.Abs(i)) * 3, "577b60");
                    Leaf(i * 4, -6 - (2 - System.Math.Abs(i)) * 3, i);
                }
                Box(-3, -2, 6, 2, "739174");
                break;
            case Motif.Mushrooms:
                for (int i = 0; i < 3; i++)
                {
                    int x = -7 + i * 6, y = -5 - ((i + Variant) % 2) * 5;
                    Box(x, y, 2, -y, "6d9690"); Box(x - 3, y - 3, 8, 3, "5c9f98");
                    Box(x - 2, y - 4, 6, 2, "8fcebb"); Box(x, y - 4, 2, 1, "c3e8cd");
                }
                break;
            case Motif.Vine:
                for (int i = 0; i < 3; i++)
                {
                    int x = i * 6 - 6, length = 14 + ((i + Variant) % 3) * 5;
                    Line(x, -30, x + 2, -30 + length, "466b58");
                    for (int y = -28; y < -30 + length; y += 5) Leaf(x + ((y & 1) == 0 ? 2 : -1), y, y);
                }
                break;
            case Motif.Shelf:
                Box(-16, -27, 3, 29, "554b40"); Box(13, -27, 3, 29, "554b40");
                for (int y = -24; y <= -2; y += 11)
                {
                    Box(-14, y, 28, 3, "a3865c"); Box(-14, y + 3, 28, 1, "6d5946");
                    Box(-11, y - 7, 7, 7, "677f75"); Box(-10, y - 6, 5, 2, "9eaf97");
                    Box(0, y - 5, 5, 5, "b49364"); Box(7, y - 8, 4, 8, "8b7765");
                }
                break;
            case Motif.Bench:
                Box(-18, -11, 36, 4, "c0a078"); Box(-17, -7, 34, 2, "7b5d47");
                Box(-15, -5, 3, 7, "534b43"); Box(12, -5, 3, 7, "534b43");
                Box(-12, -17, 10, 6, "576d69"); Box(-11, -16, 8, 2, "93a59b");
                Line(3, -13, 12, -13, "d2b67b"); Box(4, -17, 2, 5, "958d74");
                break;
            case Motif.Bedroll:
                Box(-12, -18, 24, 19, "504b44"); Box(-11, -17, 22, 17, "8c7860");
                Box(-10, -16, 20, 5, "c8b691"); Box(-10, -9, 20, 9, "66796f");
                Box(-9, -8, 18, 1, "8d9a7d"); Box(5, -8, 2, 7, "56675f");
                break;
            case Motif.Cable:
                Line(-14, -7, -7, -7, "3f4241"); Line(-7, -7, -7, 0, "3f4241");
                Line(-7, 0, 14, 0, "3f4241"); Line(-6, 1, 14, 1, "867052");
                break;
            case Motif.Ruin:
                Box(-10, -25, 17, 25, "4a5b58"); Box(-9, -26, 15, 23, "7a8b7c");
                Box(-8, -25, 12, 3, "a2ad92"); Box(-7, -21, 3, 16, "65786e");
                Line(-1, -22, -4, -16, "465b56"); Line(-4, -16, -1, -10, "465b56");
                Box(-12, -3, 25, 4, "708272"); Box(8, -6, 5, 5, "8c9a83");
                Leaf(3, -12, 1); Leaf(-4, -4, 1);
                break;
            case Motif.Stones:
                Box(-10, -5, 11, 6, "5c665e"); Box(-8, -7, 7, 3, "87907a");
                Box(2, -3, 7, 4, "6b7768"); Box(3, -4, 5, 2, "98a087");
                break;
            case Motif.Root:
                Line(-10, -9, -2, -1, "6c6550"); Line(-2, -1, 10, 3, "6c6550");
                Line(-2, -1, -4, 9, "5d614c"); Line(2, 1, 8, -3, "5d614c");
                break;
            case Motif.Rug:
                Box(-18, -9, 36, 18, "675649"); Box(-16, -7, 32, 14, "7d6550");
                Box(-14, -6, 28, 1, "b18b5d"); Box(-14, 5, 28, 1, "b18b5d");
                for (int x = -16; x <= 16; x += 4) { Box(x, -10, 1, 2, "a2845b"); Box(x, 8, 1, 2, "a2845b"); }
                break;
            case Motif.Deck:
                Box(-4, -12, 7, 24, "827259"); Box(-3, -11, 5, 1, "ad9470");
                Box(-3, 7, 5, 1, "685c4b"); Box(-2, -8, 1, 1, "44484a");
                break;
        }
        if (_highlight)
        {
            Line(-15, -13, -15, -19, "edce94"); Line(-15, -19, -10, -19, "edce94");
            Line(15, -13, 15, -19, "edce94"); Line(15, -19, 10, -19, "edce94");
            Box(-2, -23, 4, 2, "f3deac");
        }
    }
    private void Leaf(int x, int y, int variant)
    {
        Box(x - 3, y - 2, 6, 3, "3d6553"); Box(x - 2, y - 3, 4, 3, "62896b");
        Box(x - 1, y - 3, 2, 1, (variant & 1) == 0 ? "93ad7b" : "779e80");
    }
}
