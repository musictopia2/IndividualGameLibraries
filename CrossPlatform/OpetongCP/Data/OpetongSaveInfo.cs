using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using CommonBasicStandardLibraries.CollectionClasses;
namespace OpetongCP.Data
{
    [SingletonGame]
    public class OpetongSaveInfo : BasicSavedCardClass<OpetongPlayerItem, RegularRummyCard>
    { //anything needed for autoresume is here.
        public DeckRegularDict<RegularRummyCard> PoolList { get; set; } = new DeckRegularDict<RegularRummyCard>();
        public CustomBasicList<SavedSet> SetList { get; set; } = new CustomBasicList<SavedSet>();
        public bool FirstTurn { get; set; }
        public int WhichPart { get; set; }
    }
}