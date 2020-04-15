using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicXFControlsAndPages.Helpers;
using BladesOfSteelCP.Data;
using Xamarin.Forms;
namespace BladesOfSteelXF
{
    public class ScoringGuideXF : BaseFrameXF
    {
        protected override void FirstSetUp()
        {
            var thisScore = new ScoringGuideClassCP();
            ThisFrame.Text = "Scoring Guide";
            Grid firstGrid = new Grid();
            firstGrid.Children.Add(ThisDraw);
            var thisRect = ThisFrame.GetControlArea();
            Grid otherGrid = new Grid();
            otherGrid.Margin = new Thickness(thisRect.Left + 3, thisRect.Top + 3, 3, 5);
            GridHelper.AddLeftOverColumn(otherGrid, 2);
            GridHelper.AddLeftOverColumn(otherGrid, 2);
            StackLayout thisStack = new StackLayout();
            AddLabel(thisScore.OffenseText, true, thisStack);
            GridHelper.AddControlToGrid(otherGrid, thisStack, 0, 0);
            var thisList = thisScore.OffenseList();
            thisList.ForEach(item => AddLabel(item, false, thisStack));
            thisStack = new StackLayout();
            GridHelper.AddControlToGrid(otherGrid, thisStack, 0, 1);
            thisList = thisScore.DefenseList();
            AddLabel(thisScore.DefenseText, true, thisStack);
            thisList.ForEach(item => AddLabel(item, false, thisStack));
            firstGrid.Children.Add(otherGrid);
            Content = firstGrid;
        }
        private void AddLabel(string text, bool isBold, StackLayout thisStack)
        {
            Label thisLabel = new Label();
            if (isBold == true)
                thisLabel.FontAttributes = FontAttributes.Bold;
            thisLabel.TextColor = Color.Aqua;
            thisLabel.Margin = new Thickness(0, 0, 20, 0);
            thisLabel.Text = text;
            thisStack.Children.Add(thisLabel);
        }
    }
}