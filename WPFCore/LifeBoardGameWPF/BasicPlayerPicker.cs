using BasicGameFrameworkLibrary.ViewModels;
using BasicGamingUIWPFLibrary.BasicControls.ChoicePickers;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using LifeBoardGameCP.Data;
using System.Threading.Tasks;
using System.Windows.Controls;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.

namespace LifeBoardGameWPF
{
    public abstract class BasicPlayerPicker : UserControl, IUIView
    {
        public BasicPlayerPicker(LifeBoardGameVMData model)
        {
            //i think just the listview and submit
            //hopefully this one does not need end turn (?)
            //this is desktop anyways.
            ListChooserWPF list = new ListChooserWPF();
            StackPanel stack = new StackPanel()
            {
                Orientation = Orientation.Horizontal
            };
            stack.Children.Add(list);
            list.LoadLists(model.PlayerPicker);
            Button button = GetGamingButton("Submit", nameof(BasicSubmitViewModel.SubmitAsync));
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
