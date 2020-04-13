using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using FluxxCP.Containers;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace FluxxWPF.Views
{
    public class ActionTradeHandsView : UserControl, IUIView
    {
        public ActionTradeHandsView(FluxxVMData model, ActionContainer actionContainer, KeeperContainer keeperContainer)
        {
            StackPanel mainStack = new StackPanel();
            PlayerUI player = new PlayerUI(actionContainer, true, 50, 800);
            mainStack.Children.Add(player); // hopefully this simple.
            var button = ActionHelpers.GetKeeperButton();
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
