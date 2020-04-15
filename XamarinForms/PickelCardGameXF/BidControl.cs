using BasicGameFrameworkLibrary.GameGraphicsCP.GamePieces;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGamingUIXFLibrary.BasicControls.ChoicePickers;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicGamingUIXFLibrary.GameGraphics.GamePieces;
using BasicGamingUIXFLibrary.Helpers;
using PickelCardGameCP.Data;
using PickelCardGameCP.ViewModels;
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.

namespace PickelCardGameXF
{
    public class BidControl : BaseFrameXF
    {
        public void LoadLists(PickelCardGameVMData thisMod)
        {
            GamePackageViewModelBinder.ManuelElements.Clear();
            NumberChooserXF firstBid = new NumberChooserXF();
            Text = "Bid Info";
            EnumPickerXF<DeckPieceCP, DeckPieceXF, EnumSuitList> firstSuit = new EnumPickerXF<DeckPieceCP, DeckPieceXF, EnumSuitList>();
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
            Button thisBut = GetGamingButton("Place Bid", nameof(PickelBidViewModel.ProcessBidAsync));
            GamePackageViewModelBinder.ManuelElements.Add(thisBut);
            otherStack.Children.Add(thisBut);
            thisBut = GetGamingButton("Pass", nameof(PickelBidViewModel.PassAsync));
            GamePackageViewModelBinder.ManuelElements.Add(thisBut);
            otherStack.Children.Add(thisBut);
            thisStack.Children.Add(otherStack);
            Grid thisGrid = new Grid();
            thisGrid.Children.Add(ThisDraw);
            thisGrid.Children.Add(thisStack);
            Content = thisGrid;
        }
    }
}