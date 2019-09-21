using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.RegularDeckOfCards;
namespace Rummy500CP
{
    public class SavedSet
    {
        public DeckRegularDict<RegularRummyCard> CardList { get; set; } = new DeckRegularDict<RegularRummyCard>();
        public EnumWhatSets SetType;
        public bool UseSecond;
    }
}