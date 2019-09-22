using BaseGPXPagesAndControlsXF.BasicControls.ChoicePickers;
using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using BaseGPXPagesAndControlsXF.GameGraphics.GamePieces;
using BasicGameFramework.ChooserClasses;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using BasicGameFramework.RegularDeckOfCards;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using SkuckCardGameCP;
using Xamarin.Forms;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
using static BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;
using BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses;

namespace SkuckCardGameXF
{
    public class BidControl : BaseFrameXF
    {
        public void LoadList(SkuckCardGameViewModel thisMod)
        {
            Text = "Bid Info";
            Grid thisGrid = new Grid();
            StackLayout thisStack = new StackLayout();
            SetUpMarginsOnParentControl(thisStack);
            NumberChooserXF thisBid = new NumberChooserXF();
            thisBid.TotalRows = 3;
            thisBid.Columns = 9; //may need custom proportions now.
            thisBid.LoadLists(thisMod.Bid1!);
            thisStack.Children.Add(thisBid);
            Button thisBut = GetSmallerButton($"Place Bid", nameof(SkuckCardGameViewModel.BidCommand));
            thisBut.Margin = new Thickness(3, 5, 3, 3);
            thisStack.Children.Add(thisBut);
            thisGrid.Children.Add(ThisDraw);
            thisGrid.Children.Add(thisStack);
            Binding thisBind = new Binding(nameof(SkuckCardGameViewModel.BiddingVisible));
            SetBinding(IsVisibleProperty, thisBind);
            Content = thisGrid;
        }
    }
    public class TrumpControl : BaseFrameXF
    {
        public void LoadList(SkuckCardGameViewModel thisMod)
        {
            Text = "Trump Info";
            Grid thisGrid = new Grid();
            StackLayout thisStack = new StackLayout();
            SetUpMarginsOnParentControl(thisStack);
            EnumPickerXF<DeckPieceCP, DeckPieceXF, EnumSuitList, SuitListChooser> thisSuit = new EnumPickerXF<DeckPieceCP, DeckPieceXF, EnumSuitList, SuitListChooser>();
            thisSuit.Spacing = 0;
            thisStack.Children.Add(thisSuit);
            thisSuit.LoadLists(thisMod.Suit1!);
            Button thisBut = GetGamingButton("Choose Trump Suit", nameof(SkuckCardGameViewModel.TrumpCommand));
            thisStack.Children.Add(thisBut);
            thisGrid.Children.Add(ThisDraw);
            thisGrid.Children.Add(thisStack);
            Binding thisBind = new Binding(nameof(SkuckCardGameViewModel.SuitVisible));
            SetBinding(IsVisibleProperty, thisBind);
            Content = thisGrid;
        }
    }
    public class PlayControl : BaseFrameXF
    {
        public void LoadList()
        {
            Text = "Choose Play Or Pass Options";
            Grid thisGrid = new Grid();
            StackLayout thisStack = new StackLayout();
            SetUpMarginsOnParentControl(thisStack); //has 3 rows.
            Button thisBut = GetGamingButton("Play", nameof(SkuckCardGameViewModel.FirstPlayCommand));
            thisBut.CommandParameter = EnumChoiceOption.Play;
            thisStack.Children.Add(thisBut);
            thisBut = GetGamingButton("Pass", nameof(SkuckCardGameViewModel.FirstPlayCommand));
            thisBut.CommandParameter = EnumChoiceOption.Pass;
            thisStack.Children.Add(thisBut);
            thisGrid.Children.Add(ThisDraw);
            Binding ThisBind = new Binding(nameof(SkuckCardGameViewModel.FirstOptionsVisible));
            SetBinding(IsVisibleProperty, ThisBind);
            thisGrid.Children.Add(thisStack);
            Content = thisGrid;
        }
    }
    public class CustomSize : IWidthHeight
    { //for now, only normal and small no large.  could add large if necessary (?)  of course, you are open 
        int IWidthHeight.GetWidthHeight
        {
            get
            {
                if (ScreenUsed == EnumScreen.SmallPhone)
                    return 40;
                if (ScreenUsed == EnumScreen.SmallTablet)
                    return 50;
                return 80;
            }
        }
    }
}