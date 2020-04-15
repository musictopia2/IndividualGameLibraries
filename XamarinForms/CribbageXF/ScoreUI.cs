using CommonBasicStandardLibraries.CollectionClasses;
using CribbageCP.Data;
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions;
using static BasicXFControlsAndPages.Helpers.GridHelper;
namespace CribbageXF
{
    public class ScoreUI : ContentView
    {
        private CustomBasicCollection<ScoreInfo>? _scoreList;
        private Grid? _thisGrid;
        private CribbageVMData? _model;
        public void LoadLists(CribbageVMData model)
        {
            _model = model;
            _thisGrid = new Grid();
            _scoreList = _model.ScoreBoard1!.ScoreList;
            _scoreList.CollectionChanged += ScoreList_CollectionChanged;
            AddAutoColumns(_thisGrid, 2);
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
            Label thisLabel;
            if (_scoreList!.Count == 0)
            {
                thisLabel = GetDefaultLabel();
                thisLabel.Text = "No Scores";
                thisLabel.FontSize = 30;
                thisLabel.FontAttributes = FontAttributes.Bold;
                AddControlToGrid(_thisGrid, thisLabel, 0, 0);
                return;
            }
            int x = 0;
            AddAutoRows(_thisGrid, _scoreList.Count + 2);
            thisLabel = GetDefaultLabel();
            thisLabel.Text = "Score Description";
            thisLabel.FontAttributes = FontAttributes.Bold;
            AddControlToGrid(_thisGrid, thisLabel, x, 0);
            thisLabel = GetDefaultLabel();
            thisLabel.Text = "Score";
            thisLabel.FontAttributes = FontAttributes.Bold;
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
            thisLabel.FontAttributes = FontAttributes.Bold;
            thisLabel.Text = "Total Score";
            AddControlToGrid(_thisGrid, thisLabel, x, 0);
            thisLabel = GetDefaultLabel(); //at least for this.
            thisLabel.FontAttributes = FontAttributes.Bold;
            thisLabel.Text = _model!.TotalScore.ToString();
            AddControlToGrid(_thisGrid, thisLabel, x, 1);
        }
    }
}
