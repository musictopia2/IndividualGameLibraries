using SkiaSharp;
namespace PaydayCP
{
    public class SpaceCP
    {
        public SKRect Bounds { get; set; }
        public SKRegion? Region { get; set; }
        public SKPath? Path { get; set; }
        public int Number { get; set; }
    }
}