using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.SpecializedGameTypes.StockClasses;
using CommonBasicStandardLibraries.Messenging;
using SkipboCP.Cards;

namespace SkipboCP.Piles
{
    public class StockViewModel : StockPileVM<SkipboCardInformation>
    {
        public StockViewModel(CommandContainer command, IEventAggregator aggregator) : base(command, aggregator) { }

        public override string NextCardInStock()
        {
            if (DidGoOut() == true)
                return "0";
            var thisCard = GetCard();
            return thisCard.Display; // i think
        }
    }
}