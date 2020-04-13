using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackgammonCP.Graphics
{
    public class SpaceCP
    {
        public SKRect Bounds { get; set; }
        public SKRegion? Region { get; set; }
        public SKPath? Path { get; set; }
    }
}