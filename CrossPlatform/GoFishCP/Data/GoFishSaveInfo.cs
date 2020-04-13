using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
namespace GoFishCP.Data
{
    [SingletonGame]
    public class GoFishSaveInfo : BasicSavedCardClass<GoFishPlayerItem, RegularSimpleCard>
    { //anything needed for autoresume is here.
        public bool NumberAsked { get; set; }
        public bool RemovePairs { get; set; }
    }
}