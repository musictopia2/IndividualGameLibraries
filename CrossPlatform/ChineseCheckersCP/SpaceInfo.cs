namespace ChineseCheckersCP
{
    public class SpaceInfo
    {
        public int LeftOnly { get; set; }
        public int RightOnly { get; set; }
        public int UpLeft { get; set; }
        public int UpRight { get; set; }
        public int DownRight { get; set; }
        public int DownLeft { get; set; }
        public int Player { get; set; } // this is the player who owns the space
        public EnumColorList WhatColor { get; set; } // color of the starting spaces
        public EnumColorList DestinationColor { get; set; } // color of the destination spaces
        public int SpaceNumber { get; set; }
    }
}