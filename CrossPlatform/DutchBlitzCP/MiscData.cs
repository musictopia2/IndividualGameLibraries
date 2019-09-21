using BasicGameFramework.BasicDrawables.Dictionary;
namespace DutchBlitzCP
{
    public class ComputerCards
    {
        public DeckRegularDict<DutchBlitzCardInformation> StockList = new DeckRegularDict<DutchBlitzCardInformation>();
        public DeckRegularDict<DutchBlitzCardInformation> DeckList = new DeckRegularDict<DutchBlitzCardInformation>();
        public DeckRegularDict<DutchBlitzCardInformation> Discard = new DeckRegularDict<DutchBlitzCardInformation>();
        public int Player { get; set; }
        public DeckRegularDict<DutchBlitzCardInformation> PileList = new DeckRegularDict<DutchBlitzCardInformation>();
    }
    public class SendExpand
    {
        public int Deck { get; set; }
        public int Pile { get; set; }
    }
}