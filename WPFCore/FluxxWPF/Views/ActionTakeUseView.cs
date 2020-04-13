using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using FluxxCP.Containers;
using FluxxCP.ViewModels;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.

namespace FluxxWPF.Views
{
    public class ActionTakeUseView : UserControl, IUIView
    {
        public ActionTakeUseView(FluxxVMData model, ActionContainer actionContainer, KeeperContainer keeperContainer)
        {
            var mainStack = new StackPanel();
            var player = new PlayerUI(actionContainer, true, 50, 800);
            //player.ItemHeight = 50;
            //player.ItemWidth = 1300;
            mainStack.Children.Add(player);
            var otherHand = new FluxxHandWPF();
            otherHand.LoadList(actionContainer!.OtherHand!, "");
            otherHand.MinWidth = 400;
            mainStack.Children.Add(otherHand);
            var button = GetGamingButton("Choose Card", nameof(ActionTakeUseViewModel.ChooseCardAsync));
            var thisBind = GetVisibleBinding(nameof(ActionTakeUseViewModel.ButtonChooseCardVisible));
            button.SetBinding(Button.VisibilityProperty, thisBind);
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
