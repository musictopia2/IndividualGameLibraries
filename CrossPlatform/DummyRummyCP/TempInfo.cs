using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.RegularDeckOfCards;
namespace DummyRummyCP
{
    public struct TempInfo
    {
        public DeckRegularDict<RegularRummyCard> CardList;
        public int SetNumber { get; set; }
    }
}