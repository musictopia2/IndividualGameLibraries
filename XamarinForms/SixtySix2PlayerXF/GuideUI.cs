using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using SixtySix2PlayerCP;
using Xamarin.Forms;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
using static BasicXFControlsAndPages.Helpers.GridHelper;
namespace SixtySix2PlayerXF
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
        private void AddStandardLabel(ScoreValuePair thisScore)
        {
            var output = GetDefaultLabel();
            AddControlToGrid(_thisGrid!, output, _rowNumber, 0);
            output.Text = thisScore.Description;
            output.Margin = new Thickness(0, 0, 10, 0);
            output = GetDefaultLabel();
            output.Margin = new Thickness(0, 0, 3, 0);
            var scoreText = thisScore.Score.ConvertToSpecificStrings(2); //hopefully this works.
            output.Text = scoreText;
            AddControlToGrid(_thisGrid!, output, _rowNumber, 1);
            _rowNumber++;
        }
        public void LoadList(SixtySix2PlayerViewModel thisMod)
        {
            Text = "Scoring Guide";
            _thisGrid = new Grid();
            _thisGrid.RowSpacing = 0;
            _thisGrid.ColumnSpacing = 0;
            SetUpMarginsOnParentControl(_thisGrid);
            AddAutoColumns(_thisGrid, 2);
            AddAutoRows(_thisGrid, 9);
            var thisLabel = GetLargestLabel("Point Values:");
            AddControlToGrid(_thisGrid, thisLabel, 0, 0);
            _rowNumber = 1;
            var thisList = thisMod.GetDescriptionList();
            thisList.ForEach(thisscore => AddStandardLabel(thisscore));
            Grid finalGrid = new Grid();
            finalGrid.Children.Add(ThisDraw);
            finalGrid.Children.Add(_thisGrid);
            Content = finalGrid;
        }
    }
}