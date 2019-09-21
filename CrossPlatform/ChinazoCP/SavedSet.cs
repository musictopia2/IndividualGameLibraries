using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.RegularDeckOfCards;
using BasicGameFramework.SpecializedGameTypes.RummyClasses;
namespace ChinazoCP
{
    public class SavedSet
    {
        public DeckRegularDict<ChinazoCard> CardList = new DeckRegularDict<ChinazoCard>();
        public RummyProcesses<EnumSuitList, EnumColorList, ChinazoCard>.EnumRummyType WhatSet;
        public bool UseSecond;
        public int FirstNumber;
    }
}