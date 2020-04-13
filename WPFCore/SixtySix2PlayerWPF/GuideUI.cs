using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using SixtySix2PlayerCP.Data;
using System.Windows;
using System.Windows.Controls;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //usually needs this as well for grid helpers.
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
namespace SixtySix2PlayerWPF
{
    public class GuideUI : BaseFrameWPF
    {
        private Grid? _thisGrid;
        int _rowNumber;
        private TextBlock GetLargestLabel(string text)
        {
            var output = GetDefaultLabel();
            output.FontWeight = FontWeights.Bold;
            output.FontSize = 25;
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
            output.HorizontalAlignment = HorizontalAlignment.Right;
            AddControlToGrid(_thisGrid!, output, _rowNumber, 1);
            _rowNumber++;
        }
        public void LoadList(SixtySix2PlayerVMData model)
        {
            Text = "Scoring Guide";
            _thisGrid = new Grid();
            var thisRect = ThisFrame.GetControlArea();
            SetUpMarginsOnParentControl(_thisGrid, thisRect);
            AddAutoColumns(_thisGrid, 2);
            AddAutoRows(_thisGrid, 9);
            var thisLabel = GetLargestLabel("Point Values:");
            AddControlToGrid(_thisGrid, thisLabel, 0, 0);
            _rowNumber = 1;
            var thisList = model.GetDescriptionList();
            thisList.ForEach(thisscore => AddStandardLabel(thisscore));
            Grid finalGrid = new Grid();
            finalGrid.Children.Add(ThisDraw);
            finalGrid.Children.Add(_thisGrid);
            Content = finalGrid;
        }
    }
}
