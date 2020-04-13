using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using DutchBlitzCP.Cards;

namespace DutchBlitzCP.Data
{
    public class ComputerCards
    {
        public DeckRegularDict<DutchBlitzCardInformation> StockList = new DeckRegularDict<DutchBlitzCardInformation>();
        public DeckRegularDict<DutchBlitzCardInformation> DeckList = new DeckRegularDict<DutchBlitzCardInformation>();
        public DeckRegularDict<DutchBlitzCardInformation> Discard = new DeckRegularDict<DutchBlitzCardInformation>();
        public int Player { get; set; }
        public DeckRegularDict<DutchBlitzCardInformation> PileList = new DeckRegularDict<DutchBlitzCardInformation>();
    }
}