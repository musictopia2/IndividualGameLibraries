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
using RollEmCP;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
namespace RollEmWPF
{
    public class GamePage : MultiPlayerWindow<RollEmViewModel, RollEmPlayerItem, RollEmSaveInfo>
    {
        public GamePage(IStartUp starts, EnumGamePackageMode mode) //this means something needs to put into here.
        {
            BuildXAML(starts, mode);
        }
        public override Task HandleAsync(LoadEventModel message)
        {
            RollEmSaveInfo SaveRoot = OurContainer!.Resolve<RollEmSaveInfo>();
            _thisScore!.LoadLists(SaveRoot.PlayerList);
            _diceControl!.LoadDiceViewModel(ThisMod!.ThisCup!);
            _thisBoard.LoadBoard();
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            RollEmSaveInfo SaveRoot = OurContainer!.Resolve<RollEmSaveInfo>();
            _thisScore!.UpdateLists(SaveRoot.PlayerList);
            _diceControl!.UpdateDice(ThisMod!.ThisCup!);
            return Task.CompletedTask;
        }
        ScoreBoardWPF? _thisScore;
        DiceListControlWPF<SimpleDice>? _diceControl; //i think.
        readonly GameBoardWPF _thisBoard = new GameBoardWPF();
        protected async override void AfterGameButton()
        {
            StackPanel thisStack = new StackPanel(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            MainGrid!.Children.Add(thisStack);
            GameButton!.HorizontalAlignment = HorizontalAlignment.Center;
            GameButton.VerticalAlignment = VerticalAlignment.Center;
            thisStack.Children.Add(GameButton);
            var thisRoll = GetGamingButton("Roll Dice", nameof(RollEmViewModel.RollCommand));
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            _diceControl = new DiceListControlWPF<SimpleDice>();
            StackPanel tempStack = new StackPanel();
            thisStack.Children.Add(tempStack);
            tempStack.Orientation = Orientation.Horizontal;
            tempStack.Children.Add(_thisBoard);
            _thisScore = new ScoreBoardWPF();
            _thisScore.AddColumn("Score Round", true, nameof(RollEmPlayerItem.ScoreRound));
            _thisScore.AddColumn("Score Game", true, nameof(RollEmPlayerItem.ScoreGame));
            tempStack.Children.Add(_thisScore);
            otherStack.Children.Add(_diceControl);
            otherStack.Children.Add(thisRoll);
            var endButton = GetGamingButton("End Turn", nameof(RollEmViewModel.EndTurnCommand));
            endButton.HorizontalAlignment = HorizontalAlignment.Left;
            otherStack.Children.Add(endButton);
            thisStack.Children.Add(otherStack);
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(RollEmViewModel.NormalTurn)); // there is no roll number needed for this game.
            firstInfo.AddRow("Round", nameof(RollEmViewModel.Round)); //if you don't need, it comment it out.
            firstInfo.AddRow("Status", nameof(RollEmViewModel.Status));
            thisStack.Children.Add(firstInfo.GetContent);
            AddRestoreCommand(thisStack); //usually to this.  can be to another control if needed.
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterType<BasicGameLoader<RollEmPlayerItem, RollEmSaveInfo>>(); //i think basic game loader gets done here still.
            OurContainer!.RegisterNonSavedClasses<RollEmViewModel>();
            OurContainer.RegisterType<StandardRollProcesses<SimpleDice, RollEmPlayerItem>>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(StandardDiceWPF.GetDiceTag);
            OurContainer.RegisterSingleton<IGenerateDice<int>, SimpleDice>();
            OurContainer.RegisterSingleton<IProportionBoard, StandardProportion>(""); //here too.
            OurContainer.RegisterSingleton(_thisBoard.ThisElement, "");
        }
    }
}