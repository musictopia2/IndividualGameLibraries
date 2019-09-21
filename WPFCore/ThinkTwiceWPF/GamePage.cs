using AndyCristinaGamePackageCP.CommonProportionClasses;
using AndyCristinaGamePackageCP.ExtensionClasses;
using BaseGPXWindowsAndControlsCore.BaseWindows;
using BaseGPXWindowsAndControlsCore.BasicControls.GameFrames;
using BaseGPXWindowsAndControlsCore.BasicControls.Misc;
using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using BaseGPXWindowsAndControlsCore.GameGraphics.Dice;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.Dice;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ThinkTwiceCP;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //just in case
namespace ThinkTwiceWPF
{
    public class GamePage : MultiPlayerWindow<ThinkTwiceViewModel, ThinkTwicePlayerItem, ThinkTwiceSaveInfo>
    {
        public GamePage(IStartUp starts, EnumGamePackageMode mode) //this means something needs to put into here.
        {
            BuildXAML(starts, mode);
        }
        public override Task HandleAsync(LoadEventModel message)
        {
            ThinkTwiceSaveInfo saveRoot = OurContainer!.Resolve<ThinkTwiceSaveInfo>();
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _diceControl!.LoadDiceViewModel(ThisMod!.ThisCup!);
            _score1 = OurContainer.Resolve<ScoreViewModel>();
            CategoriesDice thisCat = OurContainer.Resolve<CategoriesDice>(); //i think
            Multiplier thisMul = OurContainer.Resolve<Multiplier>();
            _otherScore!.LoadList(_score1);
            CategoryWPF FirstCon = new CategoryWPF();
            FirstCon.SendDiceInfo(thisCat);
            FirstCon.Margin = new Thickness(3, 3, 100, 3); //well see on tablets.
            _multStack!.Children.Add(FirstCon);
            MultWPF SecondCon = new MultWPF();
            SecondCon.SendDiceInfo(thisMul);
            SecondCon.Margin = new Thickness(0, 3, 0, 3);
            _multStack.Children.Add(SecondCon);
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            ThinkTwiceSaveInfo saveRoot = OurContainer!.Resolve<ThinkTwiceSaveInfo>();
            _thisScore!.UpdateLists(saveRoot.PlayerList);
            _diceControl!.UpdateDice(ThisMod!.ThisCup!);
            return Task.CompletedTask;
        }
        ScoreBoardWPF? _thisScore;
        DiceListControlWPF<SimpleDice>? _diceControl; //i think.
        private ScoreViewModel? _score1;
        private StackPanel? _multStack;
        private ScoreWPF? _otherScore;
        protected async override void AfterGameButton()
        {
            StackPanel thisStack = new StackPanel();
            BasicSetUp();
            GameButton!.HorizontalAlignment = HorizontalAlignment.Center;
            GameButton.VerticalAlignment = VerticalAlignment.Center;
            thisStack.Children.Add(GameButton);
            Grid FinalGrid = new Grid();
            AddLeftOverColumn(FinalGrid, 70);
            AddLeftOverColumn(FinalGrid, 30);
            thisStack.Children.Add(FinalGrid);
            _otherScore = new ScoreWPF();
            AddControlToGrid(FinalGrid, _otherScore, 0, 1);
            StackPanel columnStack = new StackPanel();
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            StackPanel firsts = new StackPanel();
            _multStack = new StackPanel();
            _multStack.Orientation = Orientation.Horizontal;
            firsts.Children.Add(_multStack);
            _diceControl = new DiceListControlWPF<SimpleDice>();
            firsts.Children.Add(_diceControl);
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(ThinkTwiceViewModel.NormalTurn));
            firstInfo.AddRow("Roll", nameof(ThinkTwiceViewModel.RollNumber));
            firstInfo.AddRow("Category", nameof(ThinkTwiceViewModel.CategoryChosen));
            firstInfo.AddRow("Score", nameof(ThinkTwiceViewModel.Score));
            firstInfo.AddRow("Status", nameof(ThinkTwiceViewModel.Status));
            firsts.Children.Add(firstInfo.GetContent);
            otherStack.Children.Add(firsts);
            StackPanel seconds = new StackPanel();
            var thisBut = GetGamingButton("Roll Multiplier Dice", nameof(ThinkTwiceViewModel.RollMultCommand));
            seconds.Children.Add(thisBut);
            var thisRoll = GetGamingButton("Roll Dice", nameof(ThinkTwiceViewModel.RollCommand));
            seconds.Children.Add(thisRoll);
            var endButton = GetGamingButton("End Turn", nameof(ThinkTwiceViewModel.EndTurnCommand));
            endButton.HorizontalAlignment = HorizontalAlignment.Left;
            seconds.Children.Add(endButton);
            otherStack.Children.Add(seconds);
            columnStack.Children.Add(otherStack);
            _thisScore = new ScoreBoardWPF();
            columnStack.Children.Add(_thisScore);
            AddControlToGrid(FinalGrid, columnStack, 0, 0); // try this
            _thisScore.AddColumn("Score Round", true, nameof(ThinkTwicePlayerItem.ScoreRound));
            _thisScore.AddColumn("Score Game", true, nameof(ThinkTwicePlayerItem.ScoreGame));
            AddRestoreCommand(thisStack);
            MainGrid!.Children.Add(thisStack);
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterType<BasicGameLoader<ThinkTwicePlayerItem, ThinkTwiceSaveInfo>>(); //i think basic game loader gets done here still.
            OurContainer!.RegisterNonSavedClasses<ThinkTwiceViewModel>();
            OurContainer.RegisterType<StandardRollProcesses<SimpleDice, ThinkTwicePlayerItem>>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(StandardDiceWPF.GetDiceTag);
            OurContainer.RegisterSingleton<IGenerateDice<int>, SimpleDice>();
        }
    }
}