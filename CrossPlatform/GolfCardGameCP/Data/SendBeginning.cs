using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.RegularDeckOfCards;

namespace GolfCardGameCP.Data
{
    public class SendBeginning
    {
        public int Player { get; set; }
        public DeckRegularDict<RegularSimpleCard> SelectList { get; set; } = new DeckRegularDict<RegularSimpleCard>();
        public DeckRegularDict<RegularSimpleCard> UnsSelectList { get; set; } = new DeckRegularDict<RegularSimpleCard>();
    }
}