using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using CommonBasicStandardLibraries.CollectionClasses;
using PickelCardGameCP.Cards;
namespace PickelCardGameCP.Data
{
    [SingletonGame]
    public class PickelCardGameSaveInfo : BasicSavedTrickGamesClass<EnumSuitList, PickelCardGameCardInformation, PickelCardGamePlayerItem>
    { //anything needed for autoresume is here.
        public EnumStatusList GameStatus { get; set; }
        public int HighestBid { get; set; }
        public int WonSoFar { get; set; }
        public CustomBasicList<PickelCardGameCardInformation> CardList { get; set; } = new CustomBasicList<PickelCardGameCardInformation>(); //this holds the entire list of all cards played.

    }
}