using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using CommonBasicStandardLibraries.CollectionClasses;
using CribbagePatienceCP.ViewModels;
using System.Windows;
using System.Windows.Controls;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //since i use the grid a lot too.
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
namespace CribbagePatienceWPF
{
    public class ScoreSummaryUI : BaseFrameWPF
    {
        private Grid? _thisGrid;
        private CustomBasicCollection<int>? _scoreList;
        private CribbagePatienceMainViewModel? _thisMod;
        public void Init(CribbagePatienceMainViewModel thisMod)
        {
            _thisMod = thisMod;
            Text = "Score Summary";
            _scoreList = thisMod.Scores!.ScoreList;
            _scoreList.CollectionChanged += CollectionChanged;
            Grid parentGrid = new Grid();
            _thisGrid = new Grid();
            AddPixelColumn(_thisGrid, 150);
            AddAutoColumns(_thisGrid, 1);
            var thisRect = ThisFrame.GetControlArea();
            SetUpMarginsOnParentControl(_thisGrid, thisRect);
            parentGrid.Children.Add(ThisDraw);
            parentGrid.Children.Add(_thisGrid);
            HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            PopulateList();
            Content = parentGrid;
        }

        private void CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            PopulateList();
        }

        private void PopulateList()
        {
            _thisGrid!.Children.Clear();
            _thisGrid.RowDefinitions.Clear();
            if (_scoreList!.Count == 0)
                return;
            AddAutoRows(_thisGrid, _scoreList.Count + 1); //i learned the lesson the hard way if i forget this part.
            int x = 0;
            _scoreList.ForEach(y =>
            {
                x++;
                var thisLabel = GetDefaultLabel();
                thisLabel.Text = $"Round {x}";
                AddControlToGrid(_thisGrid, thisLabel, x - 1, 0);
                thisLabel = GetDefaultLabel();
                thisLabel.Text = y.ToString();
                AddControlToGrid(_thisGrid, thisLabel, x - 1, 1);
            });
            var finalLabel = GetDefaultLabel();
            finalLabel.FontWeight = FontWeights.Bold;
            finalLabel.Margin = new Thickness(0, 10, 0, 0);
            finalLabel.Text = "Final Score";
            AddControlToGrid(_thisGrid, finalLabel, x, 0);
            finalLabel = GetDefaultLabel();
            finalLabel.FontWeight = FontWeights.Bold;
            finalLabel.Margin = new Thickness(0, 10, 0, 0);
            finalLabel.Text = _thisMod!.Scores!.TotalScore.ToString();
            AddControlToGrid(_thisGrid, finalLabel, x, 1);
        }
    }
}
