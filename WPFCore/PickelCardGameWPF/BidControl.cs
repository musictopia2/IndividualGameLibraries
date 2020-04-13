using BasicGameFrameworkLibrary.GameGraphicsCP.GamePieces;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGamingUIWPFLibrary.BasicControls.ChoicePickers;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using BasicGamingUIWPFLibrary.GameGraphics.GamePieces;
using BasicGamingUIWPFLibrary.Helpers;
using PickelCardGameCP.Data;
using PickelCardGameCP.ViewModels;
using System.Windows;
using System.Windows.Controls;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.

namespace PickelCardGameWPF
{
    public class BidControl : BaseFrameWPF
    {
        public void LoadLists(PickelCardGameVMData model)
        {
            NumberChooserWPF firstBid = new NumberChooserWPF();
            firstBid.Columns = 7;
            Text = "Bid Info";
            EnumPickerWPF<DeckPieceCP, DeckPieceWPF, EnumSuitList> firstSuit = new EnumPickerWPF<DeckPieceCP, DeckPieceWPF, EnumSuitList>();
            var thisRect = ThisFrame.GetControlArea();
            StackPanel thisStack = new StackPanel();
            SetUpMarginsOnParentControl(thisStack, thisRect);
            firstBid.Margin = new Thickness(3, 3, 3, 3);
            firstSuit.Margin = new Thickness(3, 3, 3, 3);
            firstSuit.LoadLists(model.Suit1!);
            firstBid.LoadLists(model.Bid1!);
            thisStack.Children.Add(firstBid);
            thisStack.Children.Add(firstSuit);
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            GamePackageViewModelBinder.ManuelElements.Clear();
            Button button = GetGamingButton("Place Bid", nameof(PickelBidViewModel.ProcessBidAsync));
            GamePackageViewModelBinder.ManuelElements.Add(button);
            otherStack.Children.Add(button);
            button = GetGamingButton("Pass", nameof(PickelBidViewModel.PassAsync));
            otherStack.Children.Add(button);
            GamePackageViewModelBinder.ManuelElements.Add(button);
            thisStack.Children.Add(otherStack);
            Grid thisGrid = new Grid();
            thisGrid.Children.Add(ThisDraw);
            thisGrid.Children.Add(thisStack);
            Content = thisGrid;
        }
    }
}
