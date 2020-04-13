using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using MilkRunCP.Cards;
namespace MilkRunCP.Data
{
    [SingletonGame]
    public class MilkRunSaveInfo : BasicSavedCardClass<MilkRunPlayerItem, MilkRunCardInformation>
    { //anything needed for autoresume is here.
        public int CardsDrawn { get; set; } //will even increase by 1 if a card is picked up from discard
        public bool DrawnFromDiscard { get; set; } //if one was drawn; then cannot draw another one from discard
    }
}