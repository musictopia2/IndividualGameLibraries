using CommonBasicStandardLibraries.Messenging;
using PaydayCP.Data;

namespace PaydayXF.Views
{
    public class ChooseDealView : BasicPickerView
    {
        public ChooseDealView(PaydayVMData model, IEventAggregator aggregator) : base(model, aggregator)
        {
        }
    }
}