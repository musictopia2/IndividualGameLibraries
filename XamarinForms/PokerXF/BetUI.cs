using BasicGamingUIXFLibrary.BasicControls.ChoicePickers;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using PokerCP.ViewModels;
using Xamarin.Forms;
namespace PokerXF
{
    public class BetUI : BaseFrameXF
    {
        public void Init(PokerMainViewModel thisMod)
        {
            NumberChooserXF thisNumber = new NumberChooserXF();
            thisNumber.Columns = 3;
            thisNumber.Rows = 1;
            thisNumber.TotalRows = 1;
            thisNumber.LoadLists(thisMod.Bet1!);
            Text = "Bet Information";
            SetUpMarginsOnParentControl(thisNumber);
            Grid thisGrid = new Grid();
            thisGrid.Children.Add(ThisDraw);
            thisGrid.Children.Add(thisNumber);
            Content = thisGrid;
        }
    }
}