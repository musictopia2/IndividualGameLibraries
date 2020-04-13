using CommonBasicStandardLibraries.CollectionClasses;

namespace TileRummyCP.Data
{
    public class SendCreateSet
    {
        public EnumWhatSets WhatSet { get; set; }
        public int FirstNumber { get; set; }
        public int SecondNumber { get; set; }
        public CustomBasicList<int> CardList { get; set; } = new CustomBasicList<int>();
    }
}
