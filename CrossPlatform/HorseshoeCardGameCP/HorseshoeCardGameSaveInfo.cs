using BasicGameFramework.Attributes;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
using BasicGameFramework.RegularDeckOfCards;
namespace HorseshoeCardGameCP
{
    [SingletonGame]
    public class HorseshoeCardGameSaveInfo : BasicSavedTrickGamesClass<EnumSuitList, HorseshoeCardGameCardInformation, HorseshoeCardGamePlayerItem>
    { //anything needed for autoresume is here.
        public bool FirstCardPlayed { get; set; }
    }
}