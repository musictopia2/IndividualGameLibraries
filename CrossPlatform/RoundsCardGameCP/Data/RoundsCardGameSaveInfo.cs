using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using RoundsCardGameCP.Cards;
namespace RoundsCardGameCP.Data
{
    [SingletonGame]
    public class RoundsCardGameSaveInfo : BasicSavedTrickGamesClass<EnumSuitList, RoundsCardGameCardInformation, RoundsCardGamePlayerItem>
    { //anything needed for autoresume is here.
        public int PartRound { get; set; } //from 1 to 5.
        public DeckRegularDict<RoundsCardGameCardInformation> CardList = new DeckRegularDict<RoundsCardGameCardInformation>();
    }
}