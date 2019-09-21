using BasicGameFramework.BasicDrawables.Dictionary;
namespace CribbageCP
{
    public class ScoreInfo
    {
        public string Description { get; set; } = "";
        public int Score { get; set; }
    }
    public class SendCrib
    {
        public DeckRegularDict<CribbageCard> CardList { get; set; } = new DeckRegularDict<CribbageCard>();
        public int Player { get; set; }
    }
}