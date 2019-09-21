using BasicGameFramework.BasicDrawables.Dictionary;
namespace MonasteryCardGameCP
{
    public class SavedSet
    {
        public EnumWhatSets WhatType { get; set; }
        public DeckRegularDict<MonasteryCardInfo> CardList { get; set; } = new DeckRegularDict<MonasteryCardInfo>();
    }
}