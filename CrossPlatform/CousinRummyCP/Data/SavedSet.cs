using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.SpecializedGameTypes.RummyClasses;

namespace CousinRummyCP.Data
{
    public class SavedSet
    {
        public DeckRegularDict<RegularRummyCard> CardList = new DeckRegularDict<RegularRummyCard>();
    }
}