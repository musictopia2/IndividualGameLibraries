using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
using BasicGameFramework.RegularDeckOfCards;
namespace SnagCardGameCP
{
    [SingletonGame]
    public class SnagCardGameSaveInfo : BasicSavedTrickGamesClass<EnumSuitList, SnagCardGameCardInformation, SnagCardGamePlayerItem>
    { //anything needed for autoresume is here.
        public EnumStatusList GameStatus { get; set; }
        public DeckRegularDict<SnagCardGameCardInformation> CardList = new DeckRegularDict<SnagCardGameCardInformation>();
        public DeckRegularDict<SnagCardGameCardInformation> BarList = new DeckRegularDict<SnagCardGameCardInformation>(); //this is needed too.
        public bool FirstCardPlayed { get; set; }
    }
}