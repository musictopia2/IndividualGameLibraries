using BaseGPXWindowsAndControlsCore.BasicControls.ChoicePickers;
using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using Spades2PlayerCP;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
namespace Spades2PlayerWPF
{
    public class BidControl : BaseFrameWPF
    {
        public void LoadLists(Spades2PlayerViewModel thisMod)
        {
            NumberChooserWPF firstBid = new NumberChooserWPF();
            firstBid.Columns = 7;
            Text = "Bid Info";
            var thisRect = ThisFrame.GetControlArea();
            StackPanel thisStack = new StackPanel();
            SetUpMarginsOnParentControl(thisStack, thisRect);
            firstBid.Margin = new Thickness(3, 3, 3, 3);
            firstBid.LoadLists(thisMod.Bid1!);
            thisStack.Children.Add(firstBid);
            Button thisBut = GetGamingButton("Place Bid", nameof(Spades2PlayerViewModel.BidCommand));
            thisStack.Children.Add(thisBut);
            Grid thisGrid = new Grid();
            thisGrid.Children.Add(ThisDraw);
            thisGrid.Children.Add(thisStack);
            Binding ThisBind = GetVisibleBinding(nameof(Spades2PlayerViewModel.BiddingVisible));
            SetBinding(VisibilityProperty, ThisBind);
            Content = thisGrid;
        }
    }
}
