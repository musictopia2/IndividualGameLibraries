using CommonBasicStandardLibraries.CollectionClasses;
using CribbageCP;
using System.Windows;
using System.Windows.Controls;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
using static BasicControlsAndWindowsCore.Helpers.GridHelper;
namespace CribbageWPF
{
    public class ScoreUI : UserControl
    {
        private CustomBasicCollection<ScoreInfo>? _scoreList;
        private Grid? _thisGrid;
        private CribbageViewModel? _thisMod;
        public void LoadLists(CribbageViewModel thisMod)
        {
            _thisMod = thisMod;
            _thisGrid = new Grid();
            _scoreList = thisMod.ScoreBoard1!.ScoreList;
            _scoreList.CollectionChanged += ScoreList_CollectionChanged;
            AddPixelColumn(_thisGrid, 300);
            AddAutoColumns(_thisGrid, 1);
            PopulateList();
            Content = _thisGrid;
        }
        private void ScoreList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            PopulateList();
        }
        private void PopulateList()
        {
            _thisGrid!.Children.Clear();
            _thisGrid.RowDefinitions.Clear();
            TextBlock thisLabel;
            if (_scoreList!.Count == 0)
            {
                thisLabel = GetDefaultLabel();
                thisLabel.Text = "No Scores";
                thisLabel.FontSize = 30;
                thisLabel.FontWeight = FontWeights.Bold;
                AddControlToGrid(_thisGrid, thisLabel, 0, 0);
                return;
            }
            int x = 0;
            AddAutoRows(_thisGrid, _scoreList.Count + 2);
            thisLabel = GetDefaultLabel();
            thisLabel.Text = "Score Description";
            thisLabel.FontWeight = FontWeights.Bold;
            AddControlToGrid(_thisGrid, thisLabel, x, 0);
            thisLabel = GetDefaultLabel();
            thisLabel.Text = "Score";
            thisLabel.FontWeight = FontWeights.Bold;
            AddControlToGrid(_thisGrid, thisLabel, x, 1);
            x++;
            _scoreList.ForEach(thisScore =>
            {
                thisLabel = GetDefaultLabel();
                thisLabel.Text = thisScore.Description;
                AddControlToGrid(_thisGrid, thisLabel, x, 0);
                thisLabel = GetDefaultLabel();
                thisLabel.Text = thisScore.Score.ToString();
                AddControlToGrid(_thisGrid, thisLabel, x, 1);
                x++;
            });
            thisLabel = GetDefaultLabel();
            thisLabel.FontWeight = FontWeights.Bold;
            thisLabel.Text = "Total Score";
            AddControlToGrid(_thisGrid, thisLabel, x, 0);
            thisLabel = GetDefaultLabel(); //at least for this.
            thisLabel.FontWeight = FontWeights.Bold;
            thisLabel.Text = _thisMod!.TotalScore.ToString();
            AddControlToGrid(_thisGrid, thisLabel, x, 1);
        }
    }
}
