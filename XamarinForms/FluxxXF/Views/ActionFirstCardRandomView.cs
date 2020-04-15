using BasicXFControlsAndPages.MVVMFramework.Controls;
using FluxxCP.Containers;
using FluxxCP.ViewModels;
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.

namespace FluxxXF.Views
{
    public class ActionFirstCardRandomView : CustomControlBase
    {
        public ActionFirstCardRandomView(FluxxVMData model, ActionContainer actionContainer, KeeperContainer keeperContainer)
        {
            StackLayout mainStack = new StackLayout();
            FluxxHandXF thisHand = new FluxxHandXF();
            thisHand.LoadList(actionContainer!.OtherHand!, "");
            thisHand.MinimumWidthRequest = 400;
            mainStack.Children.Add(thisHand);
            var button = GetGamingButton("Choose Card", nameof(ActionFirstCardRandomViewModel.ChooseCardAsync)); // i think
            button.HorizontalOptions = LayoutOptions.Start;
            button.VerticalOptions = LayoutOptions.Start;
            mainStack.Children.Add(button);
            button = ActionHelpers.GetKeeperButton();
            mainStack.Children.Add(button);
            Content = ActionHelpers.GetFinalStack(mainStack, model, actionContainer, keeperContainer);
        }

    }
}
