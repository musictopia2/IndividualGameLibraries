using BasicGameFramework.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BaseGPXWindowsAndControlsCore.BaseWindows;
using BaseGPXWindowsAndControlsCore.BasicControls.GameFrames;
using BaseGPXWindowsAndControlsCore.BasicControls.Misc;
using BaseGPXWindowsAndControlsCore.GameGraphics.Dice;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.Dice;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BuncoDiceGameCP;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //just in case
namespace BuncoDiceGameWPF
{
    public class GamePage : SinglePlayerWindow<BuncoDiceGameViewModel>
    {
        public GamePage(IStartUp starts, EnumGamePackageMode mode) //this means something needs to put into here.
        {
            BuildXAML(starts, mode);
        }
        public override Task HandleAsync(LoadEventModel message)
        {
            var saveRoot = OurContainer!.Resolve<BuncoDiceGameSaveInfo>();
            _thisInfo!.DataContext = saveRoot.ThisStats;
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _thisDice!.LoadDiceViewModel(ThisMod!.ThisCup!);
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            return Task.CompletedTask;
        }
        private ScoreBoardWPF? _thisScore;
        private DetailGameInformationWPF? _thisInfo;
        private DiceListControlWPF<SimpleDice>? _thisDice;
        protected async override void AfterGameButton()
        {
            Grid thisGrid = new Grid();
            AddAutoColumns(thisGrid, 2); // i think 2 columns will be okay for this one.
            GameButton!.HorizontalAlignment = HorizontalAlignment.Center;
            GameButton.VerticalAlignment = VerticalAlignment.Center;
            StackPanel thisStack = new StackPanel();
            thisStack.Children.Add(GameButton);
            _thisScore = new ScoreBoardWPF();
            _thisScore.AddColumn("Points", false, nameof(PlayerItem.Points));
            _thisScore.AddColumn("Table", false, nameof(PlayerItem.Table));
            _thisScore.AddColumn("Team", false, nameof(PlayerItem.Team));
            _thisScore.AddColumn("Buncos", false, nameof(PlayerItem.Buncos));
            _thisScore.AddColumn("Wins", false, nameof(PlayerItem.Wins));
            _thisScore.AddColumn("Losses", false, nameof(PlayerItem.Losses));
            thisStack.Children.Add(_thisScore);
            thisGrid.Children.Add(thisStack);
            thisStack = new StackPanel();
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            thisStack.Children.Add(otherStack);
            _thisDice = new DiceListControlWPF<SimpleDice>();
            var thisBut = GetGamingButton("Roll", nameof(BuncoDiceGameViewModel.RollCommand));
            otherStack.Children.Add(thisBut);
            otherStack.Children.Add(_thisDice);
            otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Margin = new Thickness(0, 10, 0, 0);
            thisBut = GetGamingButton("Bunco", nameof(BuncoDiceGameViewModel.BuncoCommand));
            otherStack.Children.Add(thisBut);
            thisBut = GetGamingButton("Has 21", nameof(BuncoDiceGameViewModel.Has21Command));
            otherStack.Children.Add(thisBut);
            thisBut = GetGamingButton("End Turn", nameof(BuncoDiceGameViewModel.EndTurnCommand));
            otherStack.Children.Add(thisBut);
            thisStack.Children.Add(otherStack);
            _thisInfo = new DetailGameInformationWPF();
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
            AddControlToGrid(thisGrid, thisStack, 0, 1);
            Content = thisGrid;
            await ThisMod!.StartNewGameAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterNonSavedClasses<BuncoDiceGameViewModel>(); //go ahead and use the custom processes for this.  decided to mention non saved classes.
            OurContainer!.RegisterSingleton<IProportionImage, StandardProportion>(StandardDiceWPF.GetDiceTag);
            OurContainer.RegisterSingleton<IGenerateDice<int>, SimpleDice>();
        }
    }
}