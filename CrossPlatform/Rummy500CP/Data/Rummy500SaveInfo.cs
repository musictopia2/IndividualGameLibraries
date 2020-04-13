using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using CommonBasicStandardLibraries.CollectionClasses;
//i think this is the most common things i like to do
namespace Rummy500CP.Data
{
    [SingletonGame]
    public class Rummy500SaveInfo : BasicSavedCardClass<Rummy500PlayerItem, RegularRummyCard>
    { //anything needed for autoresume is here.
        public CustomBasicList<SavedSet> SetList { get; set; } = new CustomBasicList<SavedSet>();
        public DeckRegularDict<RegularRummyCard> DiscardList { get; set; } = new DeckRegularDict<RegularRummyCard>(); //i can do this way this time.
        public bool MoreThanOne { get; set; }
    }
}