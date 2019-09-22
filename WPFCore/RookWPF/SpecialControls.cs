using BaseGPXWindowsAndControlsCore.BasicControls.ChoicePickers;
using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using BaseGPXWindowsAndControlsCore.GameGraphics.GamePieces;
using BasicGameFramework.ChooserClasses;
using BasicGameFramework.ColorCards;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using RookCP;
using System.Windows.Controls;
using System.Windows.Data;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
namespace RookWPF
{
    public class BidControl : BaseFrameWPF
    {
        public void LoadLists(RookViewModel thisMod)
        {
            NumberChooserWPF firstBid = new NumberChooserWPF();
            firstBid.Columns = 7;
            Text = "Bid Info";
            var thisRect = ThisFrame.GetControlArea();
            StackPanel thisStack = new StackPanel();
            SetUpMarginsOnParentControl(thisStack, thisRect);
            firstBid.LoadLists(thisMod.Bid1!);
            thisStack.Children.Add(firstBid);
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            thisStack.Children.Add(otherStack);
            Button thisBut = GetGamingButton("Place Bid", nameof(RookViewModel.BidCommand));
            otherStack.Children.Add(thisBut);
            thisBut = GetGamingButton("Pass", nameof(RookViewModel.PassCommand));
            otherStack.Children.Add(thisBut);
            Grid thisGrid = new Grid();
            thisGrid.Children.Add(ThisDraw);
            thisGrid.Children.Add(thisStack);
            Binding ThisBind = GetVisibleBinding(nameof(RookViewModel.BiddingVisible));
            SetBinding(VisibilityProperty, ThisBind);
            Content = thisGrid;
        }
    }
    public class TrumpControl : BaseFrameWPF
    {
        public void LoadLists(RookViewModel thisMod)
        {
            EnumPickerWPF<CheckerChoiceCP<EnumColorTypes>,
            CheckerChooserWPF<EnumColorTypes>, EnumColorTypes,
            ColorListChooser<EnumColorTypes>> thisColor;
            Text = "Trump Info";
            var thisRect = ThisFrame.GetControlArea();
            StackPanel thisStack = new StackPanel();
            SetUpMarginsOnParentControl(thisStack, thisRect);
            thisColor = new EnumPickerWPF<CheckerChoiceCP<EnumColorTypes>, CheckerChooserWPF<EnumColorTypes>, EnumColorTypes, ColorListChooser<EnumColorTypes>>();
            thisColor.LoadLists(thisMod.Color1!);
            thisStack.Children.Add(thisColor);
            Button thisBut = GetGamingButton("Choose Color", nameof(RookViewModel.TrumpCommand));
            thisStack.Children.Add(thisBut);
            Grid thisGrid = new Grid();
            thisGrid.Children.Add(ThisDraw);
            thisGrid.Children.Add(thisStack);
            Binding thisBind = GetVisibleBinding(nameof(RookViewModel.ColorVisible));
            SetBinding(VisibilityProperty, thisBind);
            Content = thisGrid;
        }
    }
}