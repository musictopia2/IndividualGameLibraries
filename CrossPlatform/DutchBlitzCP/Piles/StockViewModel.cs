using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.SpecializedGameTypes.StockClasses;
using CommonBasicStandardLibraries.Messenging;
using DutchBlitzCP.Cards;

namespace DutchBlitzCP.Piles
{
    public class StockViewModel : StockPileVM<DutchBlitzCardInformation>
    {
        public StockViewModel(CommandContainer command, IEventAggregator aggregator) : base(command, aggregator) { }

        public override string NextCardInStock()
        {
            if (DidGoOut() == true)
                return "0";
            var ThisCard = GetCard();
            return ThisCard.Display; // i think
        }
    }
}