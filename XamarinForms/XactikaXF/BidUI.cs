using BaseGPXPagesAndControlsXF.BasicControls.ChoicePickers;
using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using XactikaCP;
using Xamarin.Forms;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
namespace XactikaXF
{
    public class BidUI : BaseFrameXF
    {
        public void LoadLists(XactikaViewModel thisMod)
        {
            NumberChooserXF FirstBid = new NumberChooserXF();
            FirstBid.Rows = 2;
            FirstBid.Columns = 5;
            FirstBid.TotalRows = 2; //i think.
            Text = "Bid Info";
            StackLayout thisStack = new StackLayout();
            SetUpMarginsOnParentControl(thisStack);
            FirstBid.Margin = new Thickness(3, 3, 3, 3);
            FirstBid.LoadLists(thisMod.Bid1!);
            thisStack.Children.Add(FirstBid);
            Button thisBut = GetSmallerButton("Place Bid", nameof(XactikaViewModel.BidCommand));
            thisStack.Children.Add(thisBut);
            Grid thisGrid = new Grid();
            thisGrid.Children.Add(ThisDraw);
            thisGrid.Children.Add(thisStack);
            Binding thisBind = new Binding(nameof(XactikaViewModel.BidVisible));
            SetBinding(IsVisibleProperty, thisBind);
            Content = thisGrid;
        }
    }
}