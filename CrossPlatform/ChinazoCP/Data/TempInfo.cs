using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.SpecializedGameTypes.RummyClasses;

namespace ChinazoCP.Data
{
    public struct TempInfo
    {
        public DeckRegularDict<ChinazoCard> CardList;
        public bool UseSecond { get; set; } //has to be here, not in the rummy functions.
        public RummyProcesses<EnumSuitList, EnumColorList, ChinazoCard>.EnumRummyType WhatSet { get; set; } //we have to do it this way.  otherwise, casting errors.
    }
}