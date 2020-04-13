using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGamingUIWPFLibrary.BasicControls.ChoicePickers;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows.Controls;
using ThreeLetterFunCP.ViewModels;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
namespace ThreeLetterFunWPF.Views
{
    public class CardsPlayerView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly ListChooserWPF _list;
        private readonly IEventAggregator _aggregator;

        public CardsPlayerView(IEventAggregator aggregator)
        {
            aggregator.Subscribe(this);
            _list = new ListChooserWPF();
            _list.ItemWidth = 400;
            _list.ItemHeight = 100;
            _list.Orientation = Orientation.Horizontal;
            StackPanel stack = new StackPanel();
            stack.Children.Add(_list);
            var thisBut = GetGamingButton("Submit", nameof(CardsPlayerViewModel.SubmitAsync));
            thisBut.FontSize = 200;
            stack.Children.Add(thisBut);
            Content = stack;
            _aggregator = aggregator;
        }

        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {
            return Task.CompletedTask; //needs this.  otherwise, could get error because it needs something that can load something.
        }

        Task IUIView.TryActivateAsync()
        {
            CardsPlayerViewModel model = (CardsPlayerViewModel)DataContext;
            _list.LoadLists(model.CardList1);
            return Task.CompletedTask;
        }

        Task IUIView.TryCloseAsync()
        {
            _aggregator.Unsubscribe(this);
            return Task.CompletedTask;
        }
    }
}
