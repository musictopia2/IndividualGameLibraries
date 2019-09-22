using BaseGPXWindowsAndControlsCore.BasicControls.ChoicePickers;
using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using BaseGPXWindowsAndControlsCore.GameGraphics.GamePieces;
using BasicGameFramework.ChooserClasses;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using BasicGameFramework.RegularDeckOfCards;
using PickelCardGameCP;
using System.Windows;
using System.Windows.Controls;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
namespace PickelCardGameWPF
{
    public class BidControl : BaseFrameWPF
    {
        public void LoadLists(PickelCardGameViewModel thisMod)
        {
            NumberChooserWPF firstBid = new NumberChooserWPF();
            firstBid.Columns = 6;
            Text = "Bid Info";
            EnumPickerWPF<DeckPieceCP, DeckPieceWPF, EnumSuitList, SuitListChooser> firstSuit = new EnumPickerWPF<DeckPieceCP, DeckPieceWPF, EnumSuitList, SuitListChooser>();
            var thisRect = ThisFrame.GetControlArea();
            StackPanel thisStack = new StackPanel();
            SetUpMarginsOnParentControl(thisStack, thisRect);
            firstBid.Margin = new Thickness(3, 3, 3, 3);
            firstSuit.Margin = new Thickness(3, 3, 3, 3);
            firstSuit.LoadLists(thisMod.Suit1!);
            firstBid.LoadLists(thisMod.Bid1!);
            thisStack.Children.Add(firstBid);
            thisStack.Children.Add(firstSuit);
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            Button thisBut = GetGamingButton("Place Bid", nameof(PickelCardGameViewModel.ProcessBidCommand));
            otherStack.Children.Add(thisBut);
            thisBut = GetGamingButton("Pass", nameof(PickelCardGameViewModel.PassCommand));
            otherStack.Children.Add(thisBut);
            thisStack.Children.Add(otherStack);
            Grid thisGrid = new Grid();
            thisGrid.Children.Add(ThisDraw);
            thisGrid.Children.Add(thisStack);
            Content = thisGrid;
        }
    }
}