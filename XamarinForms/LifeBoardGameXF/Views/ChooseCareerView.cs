using CommonBasicStandardLibraries.Messenging;
using LifeBoardGameCP.Data;

namespace LifeBoardGameXF.Views
{
    public class ChooseCareerView : BasicHandChooser
    {
        public ChooseCareerView(LifeBoardGameVMData model, IEventAggregator aggregator) : base(model, aggregator)
        {
        }
    }
}