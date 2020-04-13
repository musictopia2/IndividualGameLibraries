using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;

namespace MonasteryCardGameCP.Data
{
    public class SavedSet
    {
        public EnumWhatSets WhatType { get; set; }
        public DeckRegularDict<MonasteryCardInfo> CardList { get; set; } = new DeckRegularDict<MonasteryCardInfo>();
    }
}