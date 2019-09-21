using BasicGameFramework.BasicDrawables.Dictionary;
using CommonBasicStandardLibraries.CollectionClasses;
namespace Phase10CP
{
    public class SetInfo
    {
        public EnumWhatSets SetType;
        public bool HasLaid;
        public bool DidSucceed;
        public int HowMany;
        public DeckRegularDict<Phase10CardInformation>? SetCards;
    }

    public class PhaseList
    {
        public string Description = "";
        public CustomBasicList<SetInfo> PhaseSets = new CustomBasicList<SetInfo>();
    }

    public class SendExpandedSet
    {
        public int Deck { get; set; }
        public int Position { get; set; }
        public int Number { get; set; }
    }

    public class SendNewSet
    {
        public string CardListData { get; set; } = ""; // so it can use the extension
        public EnumWhatSets WhatSet { get; set; }
        public int FirstNumber { get; set; }
        public int SecondNumber { get; set; } // not sure why it showed the second number.  but trust its needed.
    }
    public struct TempInfo
    {
        public EnumWhatSets WhatSet;
        public int FirstNumber;
        public int SecondNumber;
        public DeckRegularDict<Phase10CardInformation> CardList;
    }
}