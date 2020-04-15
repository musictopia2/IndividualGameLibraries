using CommonBasicStandardLibraries.Messenging;
using LifeBoardGameCP.Data;

namespace LifeBoardGameXF.Views
{
    public class ChooseSalaryView : BasicHandChooser
    {
        public ChooseSalaryView(LifeBoardGameVMData model, IEventAggregator aggregator) : base(model, aggregator)
        {
        }
    }
}
