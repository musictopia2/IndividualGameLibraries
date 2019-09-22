using BaseGPXWindowsAndControlsCore.BasicControls.ChoicePickers;
using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using BaseGPXWindowsAndControlsCore.GameGraphics.GamePieces;
using BasicGameFramework.ChooserClasses;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using BasicGameFramework.RegularDeckOfCards;
using SkuckCardGameCP;
using System.Windows.Controls;
using System.Windows.Data;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
namespace SkuckCardGameWPF
{
    public class BidControl : BaseFrameWPF
    {
        public void LoadList(SkuckCardGameViewModel thisMod)
        {
            Text = "Bid Info";
            Grid thisGrid = new Grid();
            var thisRect = ThisFrame.GetControlArea();
            StackPanel thisStack = new StackPanel();
            SetUpMarginsOnParentControl(thisStack, thisRect);
            NumberChooserWPF thisBid = new NumberChooserWPF();
            thisBid.Columns = 9;
            thisBid.LoadLists(thisMod.Bid1!);
            thisStack.Children.Add(thisBid);
            Button ThisBut = GetGamingButton("Place Bid", nameof(SkuckCardGameViewModel.BidCommand));
            thisStack.Children.Add(ThisBut);
            thisGrid.Children.Add(ThisDraw);
            thisGrid.Children.Add(thisStack);
            Binding ThisBind = GetVisibleBinding(nameof(SkuckCardGameViewModel.BiddingVisible));
            SetBinding(VisibilityProperty, ThisBind);
            Content = thisGrid;
        }
    }
    public class TrumpControl : BaseFrameWPF
    {
        public void LoadList(SkuckCardGameViewModel thisMod)
        {
            Text = "Trump Info";
            Grid thisGrid = new Grid();
            var thisRect = ThisFrame.GetControlArea();
            StackPanel thisStack = new StackPanel();
            SetUpMarginsOnParentControl(thisStack, thisRect);
            EnumPickerWPF<DeckPieceCP, DeckPieceWPF, EnumSuitList, SuitListChooser> thisSuit = new EnumPickerWPF<DeckPieceCP, DeckPieceWPF, EnumSuitList, SuitListChooser>();
            thisStack.Children.Add(thisSuit);
            thisSuit.LoadLists(thisMod.Suit1!);
            Button thisBut = GetGamingButton("Choose Trump Suit", nameof(SkuckCardGameViewModel.TrumpCommand));
            thisStack.Children.Add(thisBut);
            thisGrid.Children.Add(ThisDraw);
            thisGrid.Children.Add(thisStack);
            Binding thisBind = GetVisibleBinding(nameof(SkuckCardGameViewModel.SuitVisible));
            SetBinding(VisibilityProperty, thisBind);
            Content = thisGrid;
        }
    }
    public class PlayControl : BaseFrameWPF
    {
        public void LoadList()
        {
            Text = "Choose Play Or Pass Options";
            Grid thisGrid = new Grid();
            var thisRect = ThisFrame.GetControlArea();
            StackPanel thisStack = new StackPanel();
            SetUpMarginsOnParentControl(thisStack, thisRect);
            Button thisBut = GetGamingButton("Play", nameof(SkuckCardGameViewModel.FirstPlayCommand));
            thisBut.CommandParameter = EnumChoiceOption.Play;
            thisStack.Children.Add(thisBut);
            thisBut = GetGamingButton("Pass", nameof(SkuckCardGameViewModel.FirstPlayCommand));
            thisBut.CommandParameter = EnumChoiceOption.Pass;
            thisStack.Children.Add(thisBut);
            thisGrid.Children.Add(ThisDraw);
            Binding ThisBind = GetVisibleBinding(nameof(SkuckCardGameViewModel.FirstOptionsVisible));
            SetBinding(VisibilityProperty, ThisBind);
            thisGrid.Children.Add(thisStack);
            Content = thisGrid;
        }
    }
}