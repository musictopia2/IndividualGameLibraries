using SkiaSharp;
namespace PassOutDiceGameCP
{
    public class SpaceInfo
    {
        public SKRect Bounds { get; set; }
        public EnumColorChoice Color { get; set; } //maybe this way this time.
        public int Player { get; set; }
        public bool IsEnabled { get; set; }
        public int FirstValue { get; set; }
        public int SecondValue { get; set; }
    }
}