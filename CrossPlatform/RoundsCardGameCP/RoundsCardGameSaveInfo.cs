using BasicGameFramework.Attributes;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
using BasicGameFramework.RegularDeckOfCards;
namespace RoundsCardGameCP
{
    [SingletonGame]
    public class RoundsCardGameSaveInfo : BasicSavedTrickGamesClass<EnumSuitList, RoundsCardGameCardInformation, RoundsCardGamePlayerItem>
    { //anything needed for autoresume is here.
        public int PartRound { get; set; } //from 1 to 5.
    }
}