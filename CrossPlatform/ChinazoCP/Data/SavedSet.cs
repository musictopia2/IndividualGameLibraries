using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.SpecializedGameTypes.RummyClasses;

namespace ChinazoCP.Data
{
    public class SavedSet
    {
        public DeckRegularDict<ChinazoCard> CardList = new DeckRegularDict<ChinazoCard>();
        public RummyProcesses<EnumSuitList, EnumColorList, ChinazoCard>.EnumRummyType WhatSet;
        public bool UseSecond;
        public int FirstNumber;
    }
}