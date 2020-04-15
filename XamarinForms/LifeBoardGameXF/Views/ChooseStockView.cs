using CommonBasicStandardLibraries.Messenging;
using LifeBoardGameCP.Data;

namespace LifeBoardGameXF.Views
{
    public class ChooseStockView : BasicHandChooser
    {
        public ChooseStockView(LifeBoardGameVMData model, IEventAggregator aggregator) : base(model, aggregator)
        {
        }
    }
}