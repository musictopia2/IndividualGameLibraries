using BasicGameFramework.BasicDrawables.Dictionary;
namespace GalaxyCardGameCP
{
    public enum EnumWhatSets
    {
        runs = 2,
        Kinds = 3
    }
    public enum EnumGameStatus
    {
        PlaceSets = 1,
        WinTrick = 2,
    }
    public class TrickCard
    {
        public int Deck { get; set; }
        public int Player { get; set; }
    }
    public class SendExpandedMoon
    {
        public int Deck { get; set; }
        public int MoonID { get; set; }
    }
    public interface INewWinCard
    {
        void ShowNewCard(); // this means new card.
    }
    public class SavedSet
    {
        public DeckRegularDict<GalaxyCardGameCardInformation> CardList { get; set; } = new DeckRegularDict<GalaxyCardGameCardInformation>();
        public EnumWhatSets WhatSet;
    }
}