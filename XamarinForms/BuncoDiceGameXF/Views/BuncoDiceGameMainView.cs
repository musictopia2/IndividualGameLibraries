using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.Dice;
using BasicGamingUIXFLibrary.BasicControls.GameFrames;
using BasicGamingUIXFLibrary.BasicControls.Misc;
using BuncoDiceGameCP.Data;
using BuncoDiceGameCP.ViewModels;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using static BasicXFControlsAndPages.Helpers.GridHelper;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;

namespace BuncoDiceGameXF.Views
{
    public class BuncoDiceGameMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly ScoreBoardXF? _thisScore;
        private readonly DetailGameInformationXF? _thisInfo;
        private readonly DiceListControlXF<SimpleDice>? _thisDice;


        public BuncoDiceGameMainView(IEventAggregator aggregator)
        {
            _aggregator = aggregator;
            _aggregator.Subscribe(this);

            Grid thisGrid = new Grid();
            AddLeftOverColumn(thisGrid, 70);
            AddLeftOverColumn(thisGrid, 55);
            AddAutoRows(thisGrid, 1);
            AddLeftOverRow(thisGrid, 50);
            StackLayout thisStack = new StackLayout();
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            thisStack.Children.Add(otherStack);
            _thisDice = new DiceListControlXF<SimpleDice>();
            var thisBut = GetGamingButton("Roll", nameof(BuncoDiceGameMainViewModel.RollAsync));
            otherStack.Children.Add(thisBut);
            otherStack.Children.Add(_thisDice);
            otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Margin = new Thickness(0, 10, 0, 0);
            thisBut = GetGamingButton("Bunco", nameof(BuncoDiceGameMainViewModel.BuncoAsync));
            otherStack.Children.Add(thisBut);
            thisBut = GetGamingButton("Has 21", nameof(BuncoDiceGameMainViewModel.Human21Async));
            otherStack.Children.Add(thisBut);
            thisBut = GetGamingButton("End Turn", nameof(BuncoDiceGameMainViewModel.EndTurnAsync));
            otherStack.Children.Add(thisBut);
            thisStack.Children.Add(otherStack);
            _thisInfo = new DetailGameInformationXF();
            _thisInfo.Text = "Your Statistics";
            _thisInfo.AddRow("# To Get", nameof(StatisticsInfo.NumberToGet));
            _thisInfo.AddRow("Set", nameof(StatisticsInfo.Set));
            _thisInfo.AddRow("Your Team", nameof(StatisticsInfo.YourTeam));
            _thisInfo.AddRow("Your Points", nameof(StatisticsInfo.YourPoints));
            _thisInfo.AddRow("Opponent Score", nameof(StatisticsInfo.OpponentScore));
            _thisInfo.AddRow("Buncos", nameof(StatisticsInfo.Buncos));
            _thisInfo.AddRow("Wins", nameof(StatisticsInfo.Wins));
            _thisInfo.AddRow("Losses", nameof(StatisticsInfo.Losses));
            _thisInfo.AddRow("Your Table", nameof(StatisticsInfo.YourTable));
            _thisInfo.AddRow("Team Mate", nameof(StatisticsInfo.TeamMate));
            _thisInfo.AddRow("Opponent 1", nameof(StatisticsInfo.Opponent1));
            _thisInfo.AddRow("Opponent 2", nameof(StatisticsInfo.Opponent2));
            _thisInfo.AddRow("Status", nameof(StatisticsInfo.Status));
            thisStack.Children.Add(_thisInfo);
            AddControlToGrid(thisGrid, thisStack, 0, 0);
            AddControlToGrid(thisGrid, _thisInfo, 1, 0);
            _thisScore = new ScoreBoardXF();
            _thisScore.AddColumn("Points", false, nameof(PlayerItem.Points));
            _thisScore.AddColumn("Table", false, nameof(PlayerItem.Table));
            _thisScore.AddColumn("Team", false, nameof(PlayerItem.Team));
            _thisScore.AddColumn("Buncos", false, nameof(PlayerItem.Buncos));
            _thisScore.AddColumn("Wins", false, nameof(PlayerItem.Wins));
            _thisScore.AddColumn("Losses", false, nameof(PlayerItem.Losses));
            AddControlToGrid(thisGrid, _thisScore, 0, 1);
            Grid.SetRowSpan(_thisScore, 2);
            Content = thisGrid;

        }

        

        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {
            BuncoDiceGameSaveInfo thisSave = cons!.Resolve<BuncoDiceGameSaveInfo>();
            BuncoDiceGameMainViewModel mod = (BuncoDiceGameMainViewModel)BindingContext;
            _thisInfo!.BindingContext = thisSave.ThisStats;
            _thisScore!.LoadLists(thisSave.PlayerList);
            _thisDice!.LoadDiceViewModel(mod.ThisCup!);
            return this.RefreshBindingsAsync(_aggregator);
        }


        object IUIView.DataContext
        {
            get => BindingContext;
            set => BindingContext = value;
        }
        Task IUIView.TryActivateAsync()
        {
            return Task.CompletedTask;
        }

        Task IUIView.TryCloseAsync()
        {
            return Task.CompletedTask;
        }
    }
}
