using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.RegularDeckOfCards;

namespace Rummy500CP.Data
{
    public class SavedSet
    {
        public DeckRegularDict<RegularRummyCard> CardList { get; set; } = new DeckRegularDict<RegularRummyCard>();
        public EnumWhatSets SetType;
        public bool UseSecond;
    }
}