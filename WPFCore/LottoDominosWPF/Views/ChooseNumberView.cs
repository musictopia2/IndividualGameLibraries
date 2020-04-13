using BasicGamingUIWPFLibrary.BasicControls.ChoicePickers;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using LottoDominosCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows.Controls;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions;
namespace LottoDominosWPF.Views
{
    public class ChooseNumberView : UserControl, IUIView
    {
        private readonly NumberChooserWPF _numberui;
        public ChooseNumberView()
        {
            StackPanel stack = new StackPanel();
            _numberui = new NumberChooserWPF();
            stack.Children.Add(_numberui);
            Button button = GetGamingButton("Choose Number", nameof(ChooseNumberViewModel.ChooseNumberAsync));
            stack.Children.Add(button);
            Content = stack;
        }
        Task IUIView.TryActivateAsync()
        {
            ChooseNumberViewModel model = (ChooseNumberViewModel)DataContext;
            _numberui.LoadLists(model.Number1);
            return Task.CompletedTask;
        }

        Task IUIView.TryCloseAsync()
        {
            return Task.CompletedTask;
        }
    }
}
