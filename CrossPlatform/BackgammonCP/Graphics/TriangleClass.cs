using CommonBasicStandardLibraries.CollectionClasses;
using SkiaSharp;

namespace BackgammonCP.Graphics
{
    public class TriangleClass
    {
        public int NumberOfTiles { get; set; }
        public int PlayerOwns { get; set; } // this is the player who owns it.  0 means nobody owns it.
        public CustomBasicList<SKPoint> Locations { get; set; } = new CustomBasicList<SKPoint>(); // each tile will have a location associated to it.
    }
}