using SkiaSharp;

namespace ConnectTheDotsCP.Graphics
{
    public class DotInfo
    {
        public SKRect Dot { get; set; }
        public int Column { get; set; }
        public int Row { get; set; }
        public bool IsSelected { get; set; }
        public SKRect Bounds { get; set; }
    }
}