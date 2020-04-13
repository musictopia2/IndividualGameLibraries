using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using FluxxCP.Containers;
using FluxxCP.ViewModels;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.

namespace FluxxWPF.Views
{
    public class ActionFirstCardRandomView : UserControl, IUIView
    {
        public ActionFirstCardRandomView(FluxxVMData model, ActionContainer actionContainer, KeeperContainer keeperContainer)
        {
            StackPanel mainStack = new StackPanel();
            FluxxHandWPF thisHand = new FluxxHandWPF();
            thisHand.LoadList(actionContainer!.OtherHand!, "");
            thisHand.MinWidth = 400;
            mainStack.Children.Add(thisHand);
            var button = GetGamingButton("Choose Card", nameof(ActionFirstCardRandomViewModel.ChooseCardAsync)); // i think
            button.HorizontalAlignment = HorizontalAlignment.Left;
            button.VerticalAlignment = VerticalAlignment.Top;
            mainStack.Children.Add(button);
            button = ActionHelpers.GetKeeperButton();
            mainStack.Children.Add(button);
            Content = ActionHelpers.GetFinalStack(mainStack, model, actionContainer, keeperContainer);
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
