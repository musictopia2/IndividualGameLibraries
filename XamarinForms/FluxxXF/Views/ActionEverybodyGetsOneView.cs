using BasicXFControlsAndPages.MVVMFramework.Controls;
using FluxxCP.Containers;
using FluxxCP.ViewModels;
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.

namespace FluxxXF.Views
{
    public class ActionEverybodyGetsOneView : CustomControlBase
    {
        public ActionEverybodyGetsOneView(FluxxVMData model, ActionContainer actionContainer, KeeperContainer keeperContainer)
        {
            var mainStack = new StackLayout();
            mainStack.Orientation = StackOrientation.Horizontal;
            var player = new PlayerUI(actionContainer, false, 50, 800);
            mainStack.Children.Add(player);
            StackLayout finalStack = new StackLayout();
            mainStack.Children.Add(finalStack);
            FluxxHandXF otherHand = new FluxxHandXF();
            otherHand.LoadList(actionContainer.TempHand!, "");
            otherHand.Margin = new Thickness(3, 15, 0, 0);
            finalStack.Children.Add(otherHand);
            var button = GetGamingButton("Give Cards To Selected Player", nameof(ActionEverybodyGetsOneViewModel.GiveCardsAsync)); // i think
            button.HorizontalOptions = LayoutOptions.Start;
            button.VerticalOptions = LayoutOptions.Start;
            finalStack.Children.Add(button);
            button = ActionHelpers.GetKeeperButton();
            finalStack.Children.Add(button);
            Content = ActionHelpers.GetFinalStack(mainStack, model, actionContainer, keeperContainer);
        }

    }
}