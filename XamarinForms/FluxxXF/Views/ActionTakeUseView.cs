using BasicXFControlsAndPages.MVVMFramework.Controls;
using FluxxCP.Containers;
using FluxxCP.ViewModels;
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.

namespace FluxxXF.Views
{
    public class ActionTakeUseView : CustomControlBase
    {
        public ActionTakeUseView(FluxxVMData model, ActionContainer actionContainer, KeeperContainer keeperContainer)
        {
            var mainStack = new StackLayout();
            var player = new PlayerUI(actionContainer, true, 50, 800);
            //player.ItemHeight = 50;
            //player.ItemWidth = 1300;
            mainStack.Children.Add(player);
            var otherHand = new FluxxHandXF();
            otherHand.LoadList(actionContainer!.OtherHand!, "");
            otherHand.MinimumWidthRequest = 400;
            mainStack.Children.Add(otherHand);
            var button = GetGamingButton("Choose Card", nameof(ActionTakeUseViewModel.ChooseCardAsync));
            var thisBind = new Binding(nameof(ActionTakeUseViewModel.ButtonChooseCardVisible));
            button.SetBinding(Button.IsVisibleProperty, thisBind);
            button.HorizontalOptions = LayoutOptions.Start;
            button.VerticalOptions = LayoutOptions.Start;
            mainStack.Children.Add(button);
            button = ActionHelpers.GetKeeperButton();
            mainStack.Children.Add(button);
            Content = ActionHelpers.GetFinalStack(mainStack, model, actionContainer, keeperContainer);
        }


    }
}
