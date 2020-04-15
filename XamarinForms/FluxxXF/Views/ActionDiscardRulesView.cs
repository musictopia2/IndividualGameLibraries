using BasicGamingUIXFLibrary.BasicControls.ChoicePickers;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using FluxxCP.Containers;
using FluxxCP.ViewModels;
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.

namespace FluxxXF.Views
{
    public class ActionDiscardRulesView : CustomControlBase
    {
        public ActionDiscardRulesView(FluxxVMData model, ActionContainer actionContainer, KeeperContainer keeperContainer)
        {
            StackLayout mainStack = new StackLayout();

            var firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Rules To Discard", nameof(ActionDiscardRulesViewModel.RulesToDiscard)); // i think
            mainStack.Children.Add(firstInfo.GetContent);
            var nextLabel = GetDefaultLabel();
            nextLabel.Margin = new Thickness(3, 15, 0, 2);
            nextLabel.Text = "Choose Rules To Discard";
            ScrollView scroll = new ScrollView(); // could have to scroll
            scroll.Orientation = ScrollOrientation.Vertical;
            scroll.HeightRequest = 400;
            ListChooserXF rule1 = new ListChooserXF();
            rule1.LoadLists(actionContainer!.Rule1!);
            rule1.ItemHeight = 40;
            rule1.ItemWidth = 600;
            scroll.Content = rule1;
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            mainStack.Children.Add(otherStack);
            otherStack.Children.Add(scroll);
            var button = GetGamingButton("View Card", nameof(ActionDiscardRulesViewModel.ViewRuleCard));
            StackLayout finalStack = new StackLayout();
            finalStack.Children.Add(button);
            button = GetGamingButton("Discard Selected Rule(s)", nameof(ActionDiscardRulesViewModel.DiscardRulesAsync));
            finalStack.Children.Add(button);
            button = ActionHelpers.GetKeeperButton();
            finalStack.Children.Add(button);
            otherStack.Children.Add(finalStack);
            Content = ActionHelpers.GetFinalStack(mainStack, model, actionContainer, keeperContainer);
        }



    }
}
