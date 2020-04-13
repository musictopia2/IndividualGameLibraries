using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.RegularDeckOfCards;

namespace OpetongCP.Data
{
    public class SavedSet
    {
        public int Player { get; set; }
        public DeckRegularDict<RegularRummyCard> CardList { get; set; } = new DeckRegularDict<RegularRummyCard>();
    }
}