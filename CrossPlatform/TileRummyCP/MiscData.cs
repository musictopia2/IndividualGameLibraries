using BasicGameFramework.BasicDrawables.Dictionary;
using CommonBasicStandardLibraries.CollectionClasses;
namespace TileRummyCP
{
    public class SendCreateSet
    {
        public EnumWhatSets WhatSet { get; set; }
        public int FirstNumber { get; set; }
        public int SecondNumber { get; set; }
        public CustomBasicList<int> CardList { get; set; } = new CustomBasicList<int>();
    }
    public class SendSet
    {
        public int Index { get; set; }
        public int Tile { get; set; }
        public int Position { get; set; }
    }
    public class SendDraw
    {
        public int Deck { get; set; }
        public bool FromEnd { get; set; }
    }
    public class SendCustom // well see if this is needed or not (?)
    {
        public bool DidPlay { get; set; }
        public bool ValidSets { get; set; }
    }
    public struct TempInfo
    {
        public EnumWhatSets WhatSet;
        public int FirstNumber;
        public int SecondNumber;
        public DeckRegularDict<TileInfo> CardList;
        public int TempSet; // this is the number 1 to 5 or 0.  only human needs this
    }
}