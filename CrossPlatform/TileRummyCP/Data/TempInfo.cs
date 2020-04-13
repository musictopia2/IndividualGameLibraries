using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;

namespace TileRummyCP.Data
{
    public struct TempInfo
    {
        public EnumWhatSets WhatSet;
        public int FirstNumber;
        public int SecondNumber;
        public DeckRegularDict<TileInfo> CardList;
        public int TempSet; // this is the number 1 to 5 or 0.  only human needs this
    }
}