using CommonBasicStandardLibraries.Messenging;
using PaydayCP.Data;

namespace PaydayXF.Views
{
    public class LotteryView : BasicPickerView
    {
        public LotteryView(PaydayVMData model, IEventAggregator aggregator) : base(model, aggregator)
        {
        }
    }
}
