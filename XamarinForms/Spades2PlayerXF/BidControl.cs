using BaseGPXPagesAndControlsXF.BasicControls.ChoicePickers;
using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using Spades2PlayerCP;
using Xamarin.Forms;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
namespace Spades2PlayerXF
{
    public class BidControl : BaseFrameXF
    {
        public void LoadLists(Spades2PlayerViewModel thisMod)
        {
            NumberChooserXF firstBid = new NumberChooserXF();
            firstBid.TotalRows = 2;
            firstBid.Columns = 7;
            Text = "Bid Info";
            StackLayout thisStack = new StackLayout();
            SetUpMarginsOnParentControl(thisStack);
            firstBid.Margin = new Thickness(3, 3, 3, 3);
            firstBid.LoadLists(thisMod.Bid1!);
            thisStack.Children.Add(firstBid);
            Button thisBut = GetSmallerButton("Place Bid", nameof(Spades2PlayerViewModel.BidCommand));
            thisStack.Children.Add(thisBut);
            Grid thisGrid = new Grid();
            thisGrid.Children.Add(ThisDraw);
            thisGrid.Children.Add(thisStack);
            Binding ThisBind = new Binding(nameof(Spades2PlayerViewModel.BiddingVisible));
            SetBinding(IsVisibleProperty, ThisBind);
            Content = thisGrid;
        }
    }
}