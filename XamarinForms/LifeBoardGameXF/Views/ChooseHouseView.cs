using CommonBasicStandardLibraries.Messenging;
using LifeBoardGameCP.Data;

namespace LifeBoardGameXF.Views
{
    public class ChooseHouseView : BasicHandChooser
    {
        public ChooseHouseView(LifeBoardGameVMData model, IEventAggregator aggregator) : base(model, aggregator)
        {
        }
    }
}