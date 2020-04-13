using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using Phase10CP.Cards;
using Phase10CP.Data;

namespace Phase10CP.SetClasses
{
    public class SavedSet
    {
        public DeckRegularDict<Phase10CardInformation> CardList { get; set; } = new DeckRegularDict<Phase10CardInformation>();
        public EnumWhatSets WhatSet { get; set; }
    }
}