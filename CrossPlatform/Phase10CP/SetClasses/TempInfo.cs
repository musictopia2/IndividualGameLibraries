using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using Phase10CP.Cards;
using Phase10CP.Data;

namespace Phase10CP.SetClasses
{
    public struct TempInfo
    {
        public EnumWhatSets WhatSet;
        public int FirstNumber;
        public int SecondNumber;
        public DeckRegularDict<Phase10CardInformation> CardList;
    }
}