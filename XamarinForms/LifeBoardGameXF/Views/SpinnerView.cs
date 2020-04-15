using BasicXFControlsAndPages.MVVMFramework.Controls;
using CommonBasicStandardLibraries.Messenging;

namespace LifeBoardGameXF.Views
{
    public class SpinnerView : CustomControlBase
    {
        private readonly SpinnerXF _spin;

        public SpinnerView(IEventAggregator aggregator)
        {
            _spin = new SpinnerXF(aggregator);
            Content = _spin;
        }

    }
}
