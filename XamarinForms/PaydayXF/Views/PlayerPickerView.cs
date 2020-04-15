using CommonBasicStandardLibraries.Messenging;
using PaydayCP.Data;

namespace PaydayXF.Views
{
    public class PlayerPickerView : BasicPickerView
    {
        public PlayerPickerView(PaydayVMData model, IEventAggregator aggregator) : base(model, aggregator)
        {
        }
    }
}
