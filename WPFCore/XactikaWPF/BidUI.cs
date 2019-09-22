using BaseGPXWindowsAndControlsCore.BasicControls.ChoicePickers;
using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using XactikaCP;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
namespace XactikaWPF
{
    public class BidUI : BaseFrameWPF
    {
        public void LoadLists(XactikaViewModel thisMod)
        {
            NumberChooserWPF FirstBid = new NumberChooserWPF();
            FirstBid.Rows = 2;
            FirstBid.Columns = 5;
            Text = "Bid Info";
            var thisRect = ThisFrame.GetControlArea();
            StackPanel thisStack = new StackPanel();
            SetUpMarginsOnParentControl(thisStack, thisRect);
            FirstBid.Margin = new Thickness(3, 3, 3, 3);
            FirstBid.LoadLists(thisMod.Bid1!);
            thisStack.Children.Add(FirstBid);
            Button thisBut = GetGamingButton("Place Bid", nameof(XactikaViewModel.BidCommand));
            thisStack.Children.Add(thisBut);
            Grid thisGrid = new Grid();
            thisGrid.Children.Add(ThisDraw);
            thisGrid.Children.Add(thisStack);
            Binding thisBind = GetVisibleBinding(nameof(XactikaViewModel.BidVisible));
            SetBinding(VisibilityProperty, thisBind);
            Content = thisGrid;
        }
    }
}