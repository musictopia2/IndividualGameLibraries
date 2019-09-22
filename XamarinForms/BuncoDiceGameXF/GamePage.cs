using BasicGameFramework.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.Interfaces;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Pages;
using BaseGPXPagesAndControlsXF.BasicControls.GameFrames;
using BaseGPXPagesAndControlsXF.BasicControls.Misc;
using BaseGPXPagesAndControlsXF.GameGraphics.Dice;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.Dice;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BuncoDiceGameCP;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
using static BasicXFControlsAndPages.Helpers.GridHelper; //just in case
namespace BuncoDiceGameXF
{
    public class GamePage : SinglePlayerGamePage<BuncoDiceGameViewModel>
    {
        public GamePage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode) { }
        public override Task HandleAsync(LoadEventModel message)
        {
            var saveRoot = OurContainer!.Resolve<BuncoDiceGameSaveInfo>();
            _thisInfo!.BindingContext = saveRoot.ThisStats;
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _thisDice!.LoadDiceViewModel(ThisMod!.ThisCup!);
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            return Task.CompletedTask;
        }
        private ScoreBoardXF? _thisScore;
        private DetailGameInformationXF? _thisInfo;
        private DiceListControlXF<SimpleDice>? _thisDice;
        protected override async Task AfterGameButtonAsync()
        {
            Grid thisGrid = new Grid();
            AddLeftOverColumn(thisGrid, 70);
            AddLeftOverColumn(thisGrid, 55);
            AddAutoRows(thisGrid, 1);
            AddLeftOverRow(thisGrid, 50);            
            GameButton!.HorizontalOptions = LayoutOptions.Center;
            GameButton.VerticalOptions = LayoutOptions.Center;
            StackLayout thisStack = new StackLayout();
            thisStack.Children.Add(GameButton);
            thisStack = new StackLayout();
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            thisStack.Children.Add(otherStack);
            _thisDice = new DiceListControlXF<SimpleDice>();
            var thisBut = GetGamingButton("Roll", nameof(BuncoDiceGameViewModel.RollCommand));
            otherStack.Children.Add(thisBut);
            otherStack.Children.Add(_thisDice);
            otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Margin = new Thickness(0, 10, 0, 0);
            thisBut = GetGamingButton("Bunco", nameof(BuncoDiceGameViewModel.BuncoCommand));
            otherStack.Children.Add(thisBut);
            thisBut = GetGamingButton("Has 21", nameof(BuncoDiceGameViewModel.Has21Command));
            otherStack.Children.Add(thisBut);
            thisBut = GetGamingButton("End Turn", nameof(BuncoDiceGameViewModel.EndTurnCommand));
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
            await ThisMod!.StartNewGameAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterNonSavedClasses<BuncoDiceGameViewModel>(); //go ahead and use the custom processes for this.
            OurContainer!.RegisterSingleton<IProportionImage, StandardProportion>(StandardDiceXF.GetDiceTag);
            OurContainer.RegisterSingleton<IGenerateDice<int>, SimpleDice>();
        }
    }
}