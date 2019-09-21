using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using BasicControlsAndWindowsCore.Helpers;
using BladesOfSteelCP;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
namespace BladesOfSteelWPF
{
    public class ScoringGuideWPF : BaseFrameWPF
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
            StackPanel thisStack = new StackPanel();
            AddLabel(thisScore.OffenseText, true, thisStack);
            GridHelper.AddControlToGrid(otherGrid, thisStack, 0, 0);
            var thisList = thisScore.OffenseList();
            thisList.ForEach(item => AddLabel(item, false, thisStack));
            thisStack = new StackPanel();
            GridHelper.AddControlToGrid(otherGrid, thisStack, 0, 1);
            thisList = thisScore.DefenseList();
            AddLabel(thisScore.DefenseText, true, thisStack);
            thisList.ForEach(item => AddLabel(item, false, thisStack));
            firstGrid.Children.Add(otherGrid);
            Content = firstGrid;
        }
        private void AddLabel(string text, bool isBold, StackPanel thisStack)
        {
            TextBlock thisLabel = new TextBlock();
            if (isBold == true)
                thisLabel.FontWeight = FontWeights.Bold;
            thisLabel.Foreground = Brushes.Aqua;
            thisLabel.Margin = new Thickness(0, 0, 20, 0);
            thisLabel.Text = text;
            thisStack.Children.Add(thisLabel);
        }
    }
}