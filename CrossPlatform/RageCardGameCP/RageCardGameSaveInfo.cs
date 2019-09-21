using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
namespace RageCardGameCP
{
    [SingletonGame]
    public class RageCardGameSaveInfo : BasicSavedTrickGamesClass<EnumColor, RageCardGameCardInformation, RageCardGamePlayerItem>
    { //anything needed for autoresume is here.
        public DeckRegularDict<RageCardGameCardInformation> CardList = new DeckRegularDict<RageCardGameCardInformation>();
        public int CardsToPassOut { get; set; }
        public EnumStatus Status { get; set; }
    }
}