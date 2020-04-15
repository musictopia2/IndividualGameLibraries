using BasicXFControlsAndPages.MVVMFramework.Controls;
using FluxxCP.Containers;
using Xamarin.Forms;

namespace FluxxXF.Views
{
    public class ActionTradeHandsView : CustomControlBase
    {
        public ActionTradeHandsView(FluxxVMData model, ActionContainer actionContainer, KeeperContainer keeperContainer)
        {
            StackLayout mainStack = new StackLayout();
            PlayerUI player = new PlayerUI(actionContainer, true, 50, 800);
            mainStack.Children.Add(player); // hopefully this simple.
            var button = ActionHelpers.GetKeeperButton();
            mainStack.Children.Add(button);
            Content = ActionHelpers.GetFinalStack(mainStack, model, actionContainer, keeperContainer);
        }

    }
}
