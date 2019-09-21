using BaseGPXPagesAndControlsXF.BasicControls.ChoicePickers;
using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using BaseGPXPagesAndControlsXF.GameGraphics.GamePieces;
using BasicGameFramework.ChooserClasses;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using BasicGameFramework.RegularDeckOfCards;
using PickelCardGameCP;
using Xamarin.Forms;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
namespace PickelCardGameXF
{
    public class BidControl : BaseFrameXF
    {
        public void LoadLists(PickelCardGameViewModel thisMod)
        {
            NumberChooserXF firstBid = new NumberChooserXF();
            Text = "Bid Info";
            EnumPickerXF<DeckPieceCP, DeckPieceXF, EnumSuitList, SuitListChooser> firstSuit = new EnumPickerXF<DeckPieceCP, DeckPieceXF, EnumSuitList, SuitListChooser>();
            StackLayout thisStack = new StackLayout();
            SetUpMarginsOnParentControl(thisStack);
            firstBid.Margin = new Thickness(3, 3, 3, 3);
            firstBid.TotalRows = 1;
            firstSuit.Margin = new Thickness(3, 3, 3, 3);
            firstSuit.Spacing = 0;
            firstSuit.LoadLists(thisMod.Suit1!);
            firstBid.LoadLists(thisMod.Bid1!);
            thisStack.Children.Add(firstBid);
            thisStack.Children.Add(firstSuit);
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
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