using BasicGameFramework.BasicDrawables.Dictionary;
namespace Phase10CP
{
    public class SavedSet
    {
        public DeckRegularDict<Phase10CardInformation> CardList { get; set; } = new DeckRegularDict<Phase10CardInformation>();
        public EnumWhatSets WhatSet { get; set; }
    }
}