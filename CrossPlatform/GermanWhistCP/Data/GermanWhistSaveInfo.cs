using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using GermanWhistCP.Cards;
namespace GermanWhistCP.Data
{
    [SingletonGame]
    public class GermanWhistSaveInfo : BasicSavedTrickGamesClass<EnumSuitList, GermanWhistCardInformation, GermanWhistPlayerItem>
    { //anything needed for autoresume is here.
        public bool WasEnd { get; set; }
    }
}