using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.RegularDeckOfCards;
namespace GolfCardGameCP
{
    public class SendBeginning
    {
        public int Player { get; set; }
        public DeckRegularDict<RegularSimpleCard> SelectList { get; set; } = new DeckRegularDict<RegularSimpleCard>();
        public DeckRegularDict<RegularSimpleCard> UnsSelectList { get; set; } = new DeckRegularDict<RegularSimpleCard>();
    }
}