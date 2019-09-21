using BasicGameFramework.Attributes;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
using BasicGameFramework.RegularDeckOfCards;
using CommonBasicStandardLibraries.CollectionClasses;
namespace CousinRummyCP
{
    [SingletonGame]
    public class CousinRummySaveInfo : BasicSavedCardClass<CousinRummyPlayerItem, RegularRummyCard>
    { //anything needed for autoresume is here.  don't need beginnings anymore because we have immediately start turn.
        public CustomBasicList<SavedSet> SetList { get; set; } = new CustomBasicList<SavedSet>();
        public int Round { get; set; }
        public int WhoDiscarded { get; set; } //0 means nobody.
    }
}