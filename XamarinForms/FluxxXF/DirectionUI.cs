using BaseGPXPagesAndControlsXF.BasicControls.ChoicePickers;
using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using FluxxCP;
using Xamarin.Forms;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
namespace FluxxXF
{
    public class DirectionUI : BaseFrameXF
    {
        public void LoadList(ActionViewModel actionMod)
        {
            Text = "Direction";
            StackLayout thisStack = new StackLayout();
            SetUpMarginsOnParentControl(thisStack); //i think.
            ListChooserXF thisList = new ListChooserXF();
            thisList.ItemHeight = 60;
            thisList.LoadLists(actionMod.Direction1!);
            thisStack.Children.Add(thisList);
            var thisBut = GetGamingButton("Choose Direction", nameof(ActionViewModel.DirectionCommand));
            thisBut.HorizontalOptions = LayoutOptions.Start;
            thisBut.VerticalOptions = LayoutOptions.Start;
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            thisStack.Children.Add(otherStack);
            otherStack.Children.Add(thisBut);
            thisBut = ActionUI.GetKeeperButton();
            otherStack.Children.Add(thisBut);
            Grid ThisGrid = new Grid();
            ThisGrid.Children.Add(ThisDraw);
            ThisGrid.Children.Add(thisStack);
            Content = ThisGrid;
        }
    }
}
