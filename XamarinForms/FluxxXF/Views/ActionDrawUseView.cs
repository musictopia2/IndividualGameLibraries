using BasicXFControlsAndPages.MVVMFramework.Controls;
using FluxxCP.Containers;
using FluxxCP.ViewModels;
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.

namespace FluxxXF.Views
{
    public class ActionDrawUseView : CustomControlBase
    {
        public ActionDrawUseView(FluxxVMData model, ActionContainer actionContainer, KeeperContainer keeperContainer)
        {
            StackLayout mainStack = new StackLayout();
            FluxxHandXF hand = new FluxxHandXF();
            hand.LoadList(actionContainer!.TempHand!, ""); // i think this is the only difference for this one.
            mainStack.Children.Add(hand);
            var button = GetGamingButton("Choose Card", nameof(ActionDrawUseViewModel.DrawUseAsync)); //hopefully this simple (?)
            button.HorizontalOptions = LayoutOptions.Start;
            button.VerticalOptions = LayoutOptions.Start;
            mainStack.Children.Add(button);
            button = ActionHelpers.GetKeeperButton();
            mainStack.Children.Add(button);
            Content = ActionHelpers.GetFinalStack(mainStack, model, actionContainer, keeperContainer);
        }

    }
}