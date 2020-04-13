using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using BasicGamingUIWPFLibrary.BasicControls.SolitaireClasses;
using ClockSolitaireCP.ViewModels;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows;
using System.Windows.Controls;

namespace ClockSolitaireWPF.Views
{
    public class ClockSolitaireMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;

        private readonly MainClockWPF _clock;
        public ClockSolitaireMainView(IEventAggregator aggregator)
        {
            _aggregator = aggregator;
            _aggregator.Subscribe(this);
            _clock = new MainClockWPF();

            Grid grid = new Grid();

            SimpleLabelGrid label = new SimpleLabelGrid();
            label.AddRow("Cards", nameof(ClockSolitaireMainViewModel.CardsLeft));

            grid.Children.Add(label.GetContent);
            _clock.Margin = new Thickness(5, 10, 0, 0);
            grid.Children.Add(_clock);
            Content = grid;


        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {
            ClockSolitaireMainViewModel model = (ClockSolitaireMainViewModel)DataContext;
            _clock.LoadControls(model.Clock1!);
            model.Clock1!.SendSavedMessage();
            return Task.CompletedTask;
        }

        Task IUIView.TryActivateAsync()
        {
            return Task.CompletedTask;
        }

        Task IUIView.TryCloseAsync()
        {
            _aggregator.Unsubscribe(this); //looks like you have to unsubscribe each time. now.
            return Task.CompletedTask;
        }
    }
}
