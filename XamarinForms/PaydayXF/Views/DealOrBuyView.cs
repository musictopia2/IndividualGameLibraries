using CommonBasicStandardLibraries.Messenging;
using PaydayCP.Data;

namespace PaydayXF.Views
{
    public class DealOrBuyView : BasicPickerView
    {
        public DealOrBuyView(PaydayVMData model, IEventAggregator aggregator) : base(model, aggregator)
        {
        }
    }
}