using BaseGPXWindowsAndControlsCore.BasicControls.ChoicePickers;
using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using FluxxCP;
using SkiaSharp;
using System.Windows.Controls;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
namespace FluxxWPF
{
    public class CardListUI : BaseFrameWPF
    {
        public void LoadControls(ActionViewModel thisAction)
        {
            Text = "Card List";
            StackPanel thisStack = new StackPanel();
            SKRect thisRect = ThisFrame.GetControlArea();
            SetUpMarginsOnParentControl(thisStack, thisRect); //i think.
            ListChooserWPF thisList = new ListChooserWPF();
            thisList.LoadLists(thisAction.CardList1!); // i think
            ScrollViewer thisScroll = new ScrollViewer();
            thisScroll.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            thisScroll.Content = thisList;
            thisScroll.Height = 500; // well see.
            thisStack.Orientation = Orientation.Horizontal;
            thisStack.Children.Add(thisScroll);
            StackPanel finalStack = new StackPanel();
            thisStack.Children.Add(finalStack);
            var thisBut = GetGamingButton("Select Card", nameof(ActionViewModel.SelectCardCommand)); // i think
            finalStack.Children.Add(thisBut);
            thisBut = GetGamingButton("View Card", nameof(ActionViewModel.ViewCardCommand));
            finalStack.Children.Add(thisBut);
            thisBut = ActionUI.GetKeeperButton();
            finalStack.Children.Add(thisBut);
            Grid thisGrid = new Grid();
            thisGrid.Children.Add(ThisDraw);
            thisGrid.Children.Add(thisStack);
            Content = thisGrid;
        }
    }
}