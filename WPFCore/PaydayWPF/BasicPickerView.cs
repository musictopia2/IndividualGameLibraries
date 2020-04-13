using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using PaydayCP.Data;
using BasicGameFrameworkLibrary.ViewModels;
using BasicGamingUIWPFLibrary.BasicControls.ChoicePickers;
using System.Threading.Tasks;
using System.Windows.Controls;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using System.Windows;
namespace PaydayWPF
{
    public abstract class BasicPickerView : UserControl, IUIView
    {
        public BasicPickerView(PaydayVMData model)
        {
            ListChooserWPF list = new ListChooserWPF();
            StackPanel stack = new StackPanel()
            {
                Orientation = Orientation.Horizontal
            };
            stack.Children.Add(list);
            list.LoadLists(model.PopUpList);
            Button button = GetGamingButton("Submit", nameof(BasicSubmitViewModel.SubmitAsync));
            button.HorizontalAlignment = HorizontalAlignment.Left;
            button.VerticalAlignment = VerticalAlignment.Top;
            button.FontSize = 100; //make 100 instead of 200 now.
            stack.Children.Add(button); //can always adjust as needed anyways.
            Content = stack;
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
