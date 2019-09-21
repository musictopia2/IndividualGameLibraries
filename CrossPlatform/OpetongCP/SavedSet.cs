using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.RegularDeckOfCards;
namespace OpetongCP
{
    public class SavedSet
    {
        public int Player { get; set; }
        public DeckRegularDict<RegularRummyCard> CardList { get; set; } = new DeckRegularDict<RegularRummyCard>();
    }
}