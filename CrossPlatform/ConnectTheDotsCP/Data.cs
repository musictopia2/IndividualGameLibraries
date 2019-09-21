using SkiaSharp;
namespace ConnectTheDotsCP
{
    public class SquareInfo
    {
        public int Player { get; set; }
        public SKRect Rectangle { get; set; }
        public int Color { get; set; } // blue or red.
        public int Column { get; set; }
        public int Row { get; set; }
    }
    public class LineInfo
    {
        internal SKPoint StartingPoint { get; set; }
        internal SKPoint FinishingPoint { get; set; }
        public bool IsTaken { get; set; } = false;
        public int DotRow1 { get; set; }
        public int DotRow2 { get; set; }
        public int DotColumn1 { get; set; }
        public int DotColumn2 { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
        public bool Horizontal { get; set; }
        public int Index { get; set; } // needed for autoresume
    }
    public class DotInfo
    {
        public SKRect Dot { get; set; }
        public int Column { get; set; }
        public int Row { get; set; }
        public bool IsSelected { get; set; }
        public SKRect Bounds { get; set; }
    }
}