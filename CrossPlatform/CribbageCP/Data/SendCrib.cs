using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;

namespace CribbageCP.Data
{
    public class SendCrib
    {
        public DeckRegularDict<CribbageCard> CardList { get; set; } = new DeckRegularDict<CribbageCard>();
        public int Player { get; set; }
    }
}