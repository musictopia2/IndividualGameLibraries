using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGamingUIWPFLibrary.BasicControls.ChoicePickers;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows;
using System.Windows.Controls;
using ThreeLetterFunCP.ViewModels;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
namespace ThreeLetterFunWPF.Views
{
    public class AdvancedOptionsView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly ListChooserWPF _list1;
        private readonly ListChooserWPF _list2;
        private readonly IEventAggregator _aggregator;

        public AdvancedOptionsView(IEventAggregator aggregator)
        {
            aggregator.Subscribe(this);
            StackPanel stack = new StackPanel();
            _list1 = new ListChooserWPF();
            _list2 = new ListChooserWPF();
            _list1.Orientation = Orientation.Horizontal;
            _list2.Orientation = Orientation.Horizontal;
            _list1.Margin = new Thickness(0);
            _list1.ItemWidth = 800; //well see when its on xamarin forms (lots of experimenting could be needed).
            _list2.ItemWidth = 800;
            _list1.ItemHeight = 150;
            _list2.ItemHeight = 150;
            _list2.Margin = new Thickness(10, 50, 0, 20);
            stack.Children.Add(_list1);
            stack.Children.Add(_list2);
            var thisBut = GetGamingButton("Submit", nameof(AdvancedOptionsViewModel.SubmitAsync));
            thisBut.FontSize = 200;
            thisBut.Margin = new Thickness(5, 50, 5, 5);
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
            AdvancedOptionsViewModel model = (AdvancedOptionsViewModel)DataContext;
            _list1.LoadLists(model.Easy1);
            _list2.LoadLists(model.Game1);
            return Task.CompletedTask;
        }

        Task IUIView.TryCloseAsync()
        {
            _aggregator.Unsubscribe(this);
            return Task.CompletedTask;
        }
    }
}
