using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using FluxxCP.Containers;
using FluxxCP.ViewModels;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.

namespace FluxxWPF.Views
{
    public class ActionDrawUseView : UserControl, IUIView
    {
        public ActionDrawUseView(FluxxVMData model, ActionContainer actionContainer, KeeperContainer keeperContainer)
        {
            StackPanel mainStack = new StackPanel();
            FluxxHandWPF hand = new FluxxHandWPF();
            hand.LoadList(actionContainer!.TempHand!, ""); // i think this is the only difference for this one.
            mainStack.Children.Add(hand);
            var button = GetGamingButton("Choose Card", nameof(ActionDrawUseViewModel.DrawUseAsync)); //hopefully this simple (?)
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