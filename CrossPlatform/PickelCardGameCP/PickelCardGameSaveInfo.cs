using BasicGameFramework.Attributes;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
using BasicGameFramework.RegularDeckOfCards;
using CommonBasicStandardLibraries.CollectionClasses;
namespace PickelCardGameCP
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