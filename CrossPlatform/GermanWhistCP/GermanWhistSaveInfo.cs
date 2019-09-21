using BasicGameFramework.Attributes;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
using BasicGameFramework.RegularDeckOfCards;
namespace GermanWhistCP
{
    [SingletonGame]
    public class GermanWhistSaveInfo : BasicSavedTrickGamesClass<EnumSuitList, GermanWhistCardInformation, GermanWhistPlayerItem>
    { //anything needed for autoresume is here.
        public bool WasEnd { get; set; }
    }
}