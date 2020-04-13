using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows.Controls;
namespace LifeBoardGameWPF.Views
{
    public class SpinnerView : UserControl, IUIView
    {
        private readonly SpinnerWPF _spin;
        public SpinnerView(IEventAggregator aggregator)
        {
            _spin = new SpinnerWPF(aggregator);
            Content = _spin;
        }

        Task IUIView.TryActivateAsync()
        {
            return Task.CompletedTask;
        }

        Task IUIView.TryCloseAsync()
        {
            return Task.CompletedTask;
        }
    }
}
