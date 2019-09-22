using BaseGPXPagesAndControlsXF.BasicControls.ChoicePickers;
using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using BaseGPXPagesAndControlsXF.GameGraphics.GamePieces;
using BasicGameFramework.ChooserClasses;
using BasicGameFramework.ColorCards;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using RookCP;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
namespace RookXF
{
    public class BidControl : BaseFrameXF
    {
        public void LoadLists(RookViewModel thisMod)
        {
            NumberChooserXF firstBid = new NumberChooserXF();
            firstBid.Columns = 7;
            Text = "Bid Info";
            StackLayout thisStack = new StackLayout();
            SetUpMarginsOnParentControl(thisStack);
            firstBid.LoadLists(thisMod.Bid1!);
            thisStack.Children.Add(firstBid);
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            thisStack.Children.Add(otherStack);
            Button thisBut = GetGamingButton("Place Bid", nameof(RookViewModel.BidCommand));
            otherStack.Children.Add(thisBut);
            thisBut = GetGamingButton("Pass", nameof(RookViewModel.PassCommand));
            otherStack.Children.Add(thisBut);
            Grid thisGrid = new Grid();
            thisGrid.Children.Add(ThisDraw);
            thisGrid.Children.Add(thisStack);
            Binding ThisBind = new Binding(nameof(RookViewModel.BiddingVisible));
            SetBinding(IsVisibleProperty, ThisBind);
            Content = thisGrid;
        }
    }
    public class TrumpControl : BaseFrameXF
    {
        public void LoadLists(RookViewModel thisMod)
        {
            EnumPickerXF<CheckerChoiceCP<EnumColorTypes>,
            CheckerChooserXF<EnumColorTypes>, EnumColorTypes,
            ColorListChooser<EnumColorTypes>> thisColor;
            Text = "Trump Info";
            StackLayout thisStack = new StackLayout();
            SetUpMarginsOnParentControl(thisStack);
            thisColor = new EnumPickerXF<CheckerChoiceCP<EnumColorTypes>, CheckerChooserXF<EnumColorTypes>, EnumColorTypes, ColorListChooser<EnumColorTypes>>();
            thisColor.LoadLists(thisMod.Color1!);
            thisStack.Children.Add(thisColor);
            Button thisBut = GetGamingButton("Choose Color", nameof(RookViewModel.TrumpCommand));
            thisStack.Children.Add(thisBut);
            Grid thisGrid = new Grid();
            thisGrid.Children.Add(ThisDraw);
            thisGrid.Children.Add(thisStack);
            Binding thisBind = new Binding(nameof(RookViewModel.ColorVisible));
            SetBinding(IsVisibleProperty, thisBind);
            Content = thisGrid;
        }
    }
}