using GalaxyCardGameCP.Cards;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
//i think this is the most common things i like to do
namespace GalaxyCardGameCP.Data
{
    public class SavedSet
    {
        public DeckRegularDict<GalaxyCardGameCardInformation> CardList { get; set; } = new DeckRegularDict<GalaxyCardGameCardInformation>();
        public EnumWhatSets WhatSet;
    }

}
