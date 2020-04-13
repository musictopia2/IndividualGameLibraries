using SkiaSharp;
namespace RollEmCP.Data
{
    public class NumberInfo
    {
        public SKRect Bounds { get; set; }
        public int Number { get; set; }
        public bool IsCrossed { get; set; }
        public bool Recently { get; set; }
    }
}