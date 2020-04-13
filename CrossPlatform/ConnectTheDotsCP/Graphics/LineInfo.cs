using SkiaSharp;

namespace ConnectTheDotsCP.Graphics
{
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
}
