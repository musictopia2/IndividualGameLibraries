using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
namespace FourSuitRummyCP.Data
{
    [SingletonGame]
    public class FourSuitRummySaveInfo : BasicSavedCardClass<FourSuitRummyPlayerItem, RegularRummyCard>
    { //anything needed for autoresume is here.
        public int TimesReshuffled { get; set; }
    }
}