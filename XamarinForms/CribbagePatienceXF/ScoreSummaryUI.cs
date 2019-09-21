using System;
using System.Text;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using System.Linq;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using fs = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.FileHelpers;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using CribbagePatienceCP;
using Xamarin.Forms;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
using static BasicXFControlsAndPages.Helpers.GridHelper;
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace CribbagePatienceXF
{
    public class ScoreSummaryUI : BaseFrameXF
    {
        private Grid? _thisGrid;
        private CustomBasicCollection<int>? _scoreList;
        private CribbagePatienceViewModel? _thisMod;
        public void Init(CribbagePatienceViewModel thisMod)
        {
            _thisMod = thisMod;
            Text = "Score Summary";
            _scoreList = thisMod.Scores!.ScoreList;
            _scoreList.CollectionChanged += CollectionChanged;
            Grid parentGrid = new Grid();
            _thisGrid = new Grid();
            AddPixelColumn(_thisGrid, 150);
            AddAutoColumns(_thisGrid, 1);
            SetUpMarginsOnParentControl(_thisGrid);
            parentGrid.Children.Add(ThisDraw);
            parentGrid.Children.Add(_thisGrid);
            HorizontalOptions = LayoutOptions.Fill; //i think
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
            finalLabel.FontAttributes = FontAttributes.Bold;
            finalLabel.Margin = new Thickness(0, 10, 0, 0);
            finalLabel.Text = "Final Score";
            AddControlToGrid(_thisGrid, finalLabel, x, 0);
            finalLabel = GetDefaultLabel();
            finalLabel.FontAttributes = FontAttributes.Bold;
            finalLabel.Margin = new Thickness(0, 10, 0, 0);
            finalLabel.Text = _thisMod!.Scores!.TotalScore.ToString();
            AddControlToGrid(_thisGrid, finalLabel, x, 1);
        }
    }
}
