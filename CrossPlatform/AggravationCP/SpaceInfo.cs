namespace AggravationCP
{
    public class SpaceInfo
    {
        public int SpaceNumber { get; set; }
        public EnumColorChoice ColorOwner { get; set; } //decided to reuse the color choices this time.
        public int Player { get; set; }
        public EnumBoardStatus WhatBoard { get; set; }
        public int Index { get; set; }
    }
}