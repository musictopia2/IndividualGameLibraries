using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using Pinochle2PlayerCP;
using Xamarin.Forms;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
using static BasicXFControlsAndPages.Helpers.GridHelper;
namespace Pinochle2PlayerXF
{
    public class GuideUI : BaseFrameXF
    {
        private Grid? _thisGrid;
        int _rowNumber;
        private Label GetLargestLabel(string text)
        {
            var output = GetDefaultLabel();
            output.FontAttributes = FontAttributes.Bold;
            output.FontSize = 30;
            output.Text = text;
            return output;
        }
        private Label GetClassLabel(string className)
        {
            var output = GetDefaultLabel();
            output.FontAttributes = FontAttributes.Bold;
            output.FontSize = 20;
            output.Text = "Class " + className;
            output.Margin = new Thickness(0, 20, 0, 0);
            return output;
        }
        private void AddStandardLabel(ScoreValuePair thisScore)
        {
            var output = GetDefaultLabel();
            AddControlToGrid(_thisGrid!, output, _rowNumber, 0);
            output.Text = thisScore.Description;
            output.Margin = new Thickness(0, 0, 10, 0);
            output = GetDefaultLabel();
            output.Margin = new Thickness(0, 0, 3, 0);
            var scoreText = thisScore.Score.ConvertToSpecificStrings(3); //hopefully this works.
            output.Text = scoreText;
            AddControlToGrid(_thisGrid!, output, _rowNumber, 1);
            _rowNumber++;
        }
        public void LoadList(Pinochle2PlayerViewModel thisMod)
        {
            Text = "Scoring Guide";
            _thisGrid = new Grid();
            SetUpMarginsOnParentControl(_thisGrid);
            AddAutoColumns(_thisGrid, 2);
            AddAutoRows(_thisGrid, 20);
            var thisLabel = GetLargestLabel("Point Values:");
            AddControlToGrid(_thisGrid, thisLabel, 0, 0);
            _rowNumber = 1;
            thisMod.Guide1!.PointValueList.ForEach(thisScore => AddStandardLabel(thisScore));
            thisLabel = GetLargestLabel("Value Of Melds:");
            AddControlToGrid(_thisGrid, thisLabel, _rowNumber, 0);
            _rowNumber++;
            thisLabel = GetClassLabel("A");
            AddControlToGrid(_thisGrid, thisLabel, _rowNumber, 0);
            _rowNumber++;
            thisMod.Guide1.ClassAList.ForEach(thisScore => AddStandardLabel(thisScore));
            thisLabel = GetClassLabel("B");
            AddControlToGrid(_thisGrid, thisLabel, _rowNumber, 0);
            _rowNumber++;
            thisMod.Guide1.ClassBList.ForEach(thisScore => AddStandardLabel(thisScore));
            thisLabel = GetClassLabel("C");
            AddControlToGrid(_thisGrid, thisLabel, _rowNumber, 0);
            _rowNumber++;
            thisMod.Guide1.ClassCList.ForEach(thisScore => AddStandardLabel(thisScore));
            Grid finalGrid = new Grid();
            finalGrid.Children.Add(ThisDraw);
            finalGrid.Children.Add(_thisGrid);
            Content = finalGrid;
        }
    }
}