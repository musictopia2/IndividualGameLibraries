using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.RegularDeckOfCards;
using BasicGameFramework.SpecializedGameTypes.RummyClasses;
using CommonBasicStandardLibraries.CollectionClasses;
namespace ChinazoCP
{
    public struct TempInfo
    {
        public DeckRegularDict<ChinazoCard> CardList;
        public bool UseSecond { get; set; } //has to be here, not in the rummy functions.
        public RummyProcesses<EnumSuitList, EnumColorList, ChinazoCard>.EnumRummyType WhatSet { get; set; } //we have to do it this way.  otherwise, casting errors.
    }
    public class SetInfo
    {
        public bool DidSucceed { get; set; }
        public int HowMany { get; set; }
        public RummyProcesses<EnumSuitList, EnumColorList, ChinazoCard>.EnumRummyType WhatSet { get; set; }
    }
    public class SetList
    {
        public string Description { get; set; } = "";
        public CustomBasicList<SetInfo> PhaseSets = new CustomBasicList<SetInfo>();
    }
    public class SendExpandedSet
    {
        public int Deck { get; set; }
        public int Number { get; set; }
        public int Position { get; set; }
    }
    public class SendNewSet
    {
        public string CardListData { get; set; } = "";
        public bool UseSecond { get; set; }
        public RummyProcesses<EnumSuitList, EnumColorList, ChinazoCard>.EnumRummyType WhatSet { get; set; }
    }
}