using BasicGameFramework.Attributes;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
namespace MilkRunCP
{
    [SingletonGame]
    public class MilkRunSaveInfo : BasicSavedCardClass<MilkRunPlayerItem, MilkRunCardInformation>
    {
        public int CardsDrawn { get; set; } //will even increase by 1 if a card is picked up from discard
        public bool DrawnFromDiscard { get; set; } //if one was drawn; then cannot draw another one from discard
    }
}