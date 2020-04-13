using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGamingUIWPFLibrary.BasicControls.ChoicePickers;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Threading.Tasks;
using System.Windows.Controls;
using XactikaCP.Data;
using XactikaCP.ViewModels;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions;
namespace XactikaWPF.Views
{
    public class XactikaModeView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        public XactikaModeView(IEventAggregator aggregator, XactikaVMData model)
        {
            _aggregator = aggregator;
            _aggregator.Subscribe(this);
            StackPanel stack = new StackPanel();
            ListChooserWPF list = new ListChooserWPF();
            list.ItemHeight = 200;
            list.ItemWidth = 600;
            Button button = GetGamingButton("Submit Game Option", nameof(XactikaModeViewModel.ModeAsync));
            button.FontSize = 150;
            stack.Children.Add(list);
            stack.Children.Add(button);
            Content = stack;
            list.LoadLists(model.ModeChoose1);
        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {
            return Task.CompletedTask;
        }

        Task IUIView.TryActivateAsync()
        {
            return Task.CompletedTask;
        }

        Task IUIView.TryCloseAsync()
        {
            _aggregator.Unsubscribe(this);
            return Task.CompletedTask;
        }
    }
}