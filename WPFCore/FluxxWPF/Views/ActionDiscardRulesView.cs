using BasicGamingUIWPFLibrary.BasicControls.ChoicePickers;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using FluxxCP.Containers;
using FluxxCP.ViewModels;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.

namespace FluxxWPF.Views
{
    public class ActionDiscardRulesView : UserControl, IUIView
    {
        public ActionDiscardRulesView(FluxxVMData model, ActionContainer actionContainer, KeeperContainer keeperContainer)
        {
            StackPanel mainStack = new StackPanel();

            var firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Rules To Discard", nameof(ActionDiscardRulesViewModel.RulesToDiscard)); // i think
            mainStack.Children.Add(firstInfo.GetContent);
            var nextLabel = GetDefaultLabel();
            nextLabel.Margin = new Thickness(3, 15, 0, 2);
            nextLabel.Text = "Choose Rules To Discard";
            ScrollViewer scroll = new ScrollViewer(); // could have to scroll
            scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            scroll.Height = 400;
            ListChooserWPF rule1 = new ListChooserWPF();
            rule1.LoadLists(actionContainer!.Rule1!);
            rule1.ItemHeight = 40;
            rule1.ItemWidth = 600;
            scroll.Content = rule1;
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            mainStack.Children.Add(otherStack);
            otherStack.Children.Add(scroll);
            var button = GetGamingButton("View Card", nameof(ActionDiscardRulesViewModel.ViewRuleCard));
            StackPanel finalStack = new StackPanel();
            finalStack.Children.Add(button);
            button = GetGamingButton("Discard Selected Rule(s)", nameof(ActionDiscardRulesViewModel.DiscardRulesAsync));
            finalStack.Children.Add(button);
            button = ActionHelpers.GetKeeperButton();
            finalStack.Children.Add(button);
            otherStack.Children.Add(finalStack);
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
