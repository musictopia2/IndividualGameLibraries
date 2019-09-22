using BaseGPXWindowsAndControlsCore.BasicControls.ChoicePickers;
using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using PokerCP;
using System.Windows.Controls;
namespace PokerWPF
{
    public class BetUI : BaseFrameWPF
    {
        public void Init(PokerViewModel thisMod)
        {
            NumberChooserWPF thisNumber = new NumberChooserWPF();
            thisNumber.Columns = 3;
            thisNumber.Rows = 1;
            thisNumber.LoadLists(thisMod.Bet1!);
            Text = "Bet Information";
            var thisRect = ThisFrame.GetControlArea();
            SetUpMarginsOnParentControl(thisNumber, thisRect);
            Grid thisGrid = new Grid();
            thisGrid.Children.Add(ThisDraw);
            thisGrid.Children.Add(thisNumber);
            Content = thisGrid;
        }
    }
}
