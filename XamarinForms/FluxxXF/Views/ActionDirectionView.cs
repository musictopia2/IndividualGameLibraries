using BasicGamingUIXFLibrary.BasicControls.ChoicePickers;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using FluxxCP.Containers;
using FluxxCP.ViewModels;
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
namespace FluxxXF.Views
{
    public class ActionDirectionView : FrameUIViewXF
    {
        public ActionDirectionView(FluxxVMData model, ActionContainer actionContainer, KeeperContainer keeperContainer)
        {
            Text = "Direction";
            StackLayout stack = new StackLayout();
            SetUpMarginsOnParentControl(stack); //i think.
            ListChooserXF list = new ListChooserXF();
            list.ItemHeight = 60;
            list.LoadLists(actionContainer.Direction1!);
            stack.Children.Add(list);
            var button = GetGamingButton("Choose Direction", nameof(ActionDirectionViewModel.DirectionAsync));
            button.HorizontalOptions = LayoutOptions.Start;
            button.VerticalOptions = LayoutOptions.Start;
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            stack.Children.Add(otherStack);
            otherStack.Children.Add(button);
            button = ActionHelpers.GetKeeperButton();
            otherStack.Children.Add(button);
            Grid grid = new Grid();
            grid.Children.Add(ThisDraw);
            grid.Children.Add(stack);
            Content = ActionHelpers.GetFinalStack(grid, model, actionContainer, keeperContainer);
        }

    }
}
