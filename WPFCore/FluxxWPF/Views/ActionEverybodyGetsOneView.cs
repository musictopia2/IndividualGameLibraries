using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using FluxxCP.Containers;
using FluxxCP.ViewModels;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.

namespace FluxxWPF.Views
{
    public class ActionEverybodyGetsOneView : UserControl, IUIView
    {
        public ActionEverybodyGetsOneView(FluxxVMData model, ActionContainer actionContainer, KeeperContainer keeperContainer)
        {
            var mainStack = new StackPanel();
            mainStack.Orientation = Orientation.Horizontal;
            var player = new PlayerUI(actionContainer, false, 50, 800);
            mainStack.Children.Add(player);
            StackPanel finalStack = new StackPanel();
            mainStack.Children.Add(finalStack);
            FluxxHandWPF otherHand = new FluxxHandWPF();
            otherHand.LoadList(actionContainer.TempHand!, "");
            otherHand.Margin = new Thickness(3, 15, 0, 0);
            finalStack.Children.Add(otherHand);
            var button = GetGamingButton("Give Cards To Selected Player", nameof(ActionEverybodyGetsOneViewModel.GiveCardsAsync)); // i think
            button.HorizontalAlignment = HorizontalAlignment.Left;
            button.VerticalAlignment = VerticalAlignment.Top;
            finalStack.Children.Add(button);
            button = ActionHelpers.GetKeeperButton();
            finalStack.Children.Add(button);
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