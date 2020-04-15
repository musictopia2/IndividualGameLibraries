using BasicGamingUIXFLibrary.BasicControls.ChoicePickers;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using FluxxCP.Containers;
using FluxxCP.ViewModels;
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.

namespace FluxxXF.Views
{
    public class ActionDoAgainView : FrameUIViewXF
    {
        public ActionDoAgainView(FluxxVMData model, ActionContainer actionContainer, KeeperContainer keeperContainer)
        {
            Text = "Card List";
            StackLayout stack = new StackLayout();
            SetUpMarginsOnParentControl(stack); //i think.
            ListChooserXF list = new ListChooserXF();
            list.LoadLists(actionContainer.CardList1!); // i think
            ScrollView thisScroll = new ScrollView();
            thisScroll.Orientation = ScrollOrientation.Vertical;
            thisScroll.Content = list;
            thisScroll.HeightRequest = 500; // well see.
            stack.Orientation = StackOrientation.Horizontal;
            stack.Children.Add(thisScroll);
            StackLayout finalStack = new StackLayout();
            stack.Children.Add(finalStack);
            var button = GetGamingButton("Select Card", nameof(ActionDoAgainViewModel.SelectCardAsync)); // i think
            finalStack.Children.Add(button);
            button = GetGamingButton("View Card", nameof(ActionDoAgainViewModel.ViewCard));
            finalStack.Children.Add(button);
            button = ActionHelpers.GetKeeperButton();
            finalStack.Children.Add(button);
            Grid grid = new Grid();
            grid.Children.Add(ThisDraw);
            grid.Children.Add(stack);
            Content = ActionHelpers.GetFinalStack(grid, model, actionContainer, keeperContainer);
        }

    }
}
