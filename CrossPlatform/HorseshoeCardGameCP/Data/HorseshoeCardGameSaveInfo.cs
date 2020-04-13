using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using HorseshoeCardGameCP.Cards;
namespace HorseshoeCardGameCP.Data
{
    [SingletonGame]
    public class HorseshoeCardGameSaveInfo : BasicSavedTrickGamesClass<EnumSuitList, HorseshoeCardGameCardInformation, HorseshoeCardGamePlayerItem>
    { //anything needed for autoresume is here.
        public bool FirstCardPlayed { get; set; }
    }
}