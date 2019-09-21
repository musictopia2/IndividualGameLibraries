using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
using BasicGameFramework.RegularDeckOfCards;
using CommonBasicStandardLibraries.CollectionClasses;
namespace Rummy500CP
{
    [SingletonGame]
    public class Rummy500SaveInfo : BasicSavedCardClass<Rummy500PlayerItem, RegularRummyCard>
    { //anything needed for autoresume is here.
        public CustomBasicList<SavedSet> SetList { get; set; } = new CustomBasicList<SavedSet>();
        public DeckRegularDict<RegularRummyCard> DiscardList { get; set; } = new DeckRegularDict<RegularRummyCard>(); //i can do this way this time.
        public bool MoreThanOne { get; set; }
    }
}