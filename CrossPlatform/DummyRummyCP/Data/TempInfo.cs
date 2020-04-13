using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.RegularDeckOfCards;

namespace DummyRummyCP.Data
{
    public struct TempInfo
    {
        public DeckRegularDict<RegularRummyCard> CardList;
        public int SetNumber { get; set; }
    }
}