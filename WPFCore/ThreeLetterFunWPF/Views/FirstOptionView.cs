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
    public class FirstOptionView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly ListChooserWPF _list;
        private readonly IEventAggregator _aggregator;

        public FirstOptionView(IEventAggregator aggregator)
        {
            aggregator.Subscribe(this);
            _list = new ListChooserWPF();
            _list.ItemWidth = 700;
            _list.ItemHeight = 200;
            StackPanel stack = new StackPanel();
            stack.Children.Add(_list);
            var thisBut = GetGamingButton("Submit", nameof(FirstOptionViewModel.SubmitAsync));
            thisBut.FontSize = 150;
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(thisBut);
            thisBut = GetGamingButton("Description", nameof(FirstOptionViewModel.DescriptionAsync));
            thisBut.FontSize = 150;
            otherStack.Children.Add(thisBut);
            stack.Children.Add(otherStack);
            Content = stack;
            _aggregator = aggregator;
        }

        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {
            return Task.CompletedTask; //needs this.  otherwise, could get error because it needs something that can load something.
        }

        Task IUIView.TryActivateAsync()
        {
            FirstOptionViewModel first = (FirstOptionViewModel)DataContext;
            _list.LoadLists(first.Option1);
            return Task.CompletedTask;
        }

        Task IUIView.TryCloseAsync()
        {
            _aggregator.Unsubscribe(this);
            return Task.CompletedTask;
        }
    }
}
