using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using RageCardGameCP.Cards;
namespace RageCardGameCP.Data
{
    [SingletonGame]
    public class RageCardGameSaveInfo : BasicSavedTrickGamesClass<EnumColor, RageCardGameCardInformation, RageCardGamePlayerItem>
    { //anything needed for autoresume is here.
        public DeckRegularDict<RageCardGameCardInformation> CardList = new DeckRegularDict<RageCardGameCardInformation>();
        public int CardsToPassOut { get; set; }
        public EnumStatus Status { get; set; }
    }
}