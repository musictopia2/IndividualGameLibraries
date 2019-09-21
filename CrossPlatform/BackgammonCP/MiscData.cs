using CommonBasicStandardLibraries.CollectionClasses;
using SkiaSharp;
namespace BackgammonCP
{
    public class SpaceCP
    {
        public SKRect Bounds { get; set; }
        public SKRegion? Region { get; set; }
        public SKPath? Path { get; set; }
    }
    public class MoveInfo
    {
        public int SpaceFrom; // this is the real move i think.
        public int SpaceTo;
        public int DiceNumber;
        public EnumStatusType Results;
    }
    public class TriangleClass
    {
        public int NumberOfTiles { get; set; }
        public int PlayerOwns { get; set; } // this is the player who owns it.  0 means nobody owns it.
        public CustomBasicList<SKPoint> Locations { get; set; } = new CustomBasicList<SKPoint>(); // each tile will have a location associated to it.
    }
}