using BasicGameFramework.Attributes;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
using BasicGameFramework.RegularDeckOfCards;
namespace FourSuitRummyCP
{
    [SingletonGame]
    public class FourSuitRummySaveInfo : BasicSavedCardClass<FourSuitRummyPlayerItem, RegularRummyCard>
    { //anything needed for autoresume is here.
        public int TimesReshuffled { get; set; }
    }
}