using BasicGamingUIXFLibrary.Views;
using CommonBasicStandardLibraries.Messenging;
using RookCP.ViewModels;

namespace RookXF.Views
{
    public class RookNestView : BasicSubmitView
    {
        public RookNestView(IEventAggregator aggregator) : base(aggregator)
        {
        }

        protected override string CommandText => nameof(RookNestViewModel.NestAsync);
    }
}