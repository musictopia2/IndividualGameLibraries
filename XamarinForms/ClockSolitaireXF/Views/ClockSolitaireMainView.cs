using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicGamingUIXFLibrary.BasicControls.SolitaireClasses;
using ClockSolitaireCP.ViewModels;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;

namespace ClockSolitaireXF.Views
{
    public class ClockSolitaireMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;

        private readonly MainClockXF _clock;
        public ClockSolitaireMainView(IEventAggregator aggregator)
        {
            _aggregator = aggregator;
            _aggregator.Subscribe(this);
            _clock = new MainClockXF();

            Grid grid = new Grid();

            SimpleLabelGridXF label = new SimpleLabelGridXF();
            label.AddRow("Cards", nameof(ClockSolitaireMainViewModel.CardsLeft));

            grid.Children.Add(label.GetContent);
            _clock.Margin = new Thickness(5, 10, 0, 0);
            grid.Children.Add(_clock);
            Content = grid;
        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {
            ClockSolitaireMainViewModel model = (ClockSolitaireMainViewModel)BindingContext;
            _clock.LoadControls(model.Clock1!);
            model.Clock1!.SendSavedMessage();
            return Task.CompletedTask;
        }


        object IUIView.DataContext
        {
            get => BindingContext;
            set => BindingContext = value;
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
