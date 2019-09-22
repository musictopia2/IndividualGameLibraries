using A21DiceGameCP;
using BasicGameFramework.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
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
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
namespace A21DiceGameWPF
{
    public class GamePage : MultiPlayerWindow<A21DiceGameViewModel, A21DiceGamePlayerItem, A21DiceGameSaveInfo>
    {
        public GamePage(IStartUp starts, EnumGamePackageMode mode) //this means something needs to put into here.
        {
            BuildXAML(starts, mode);
        }
        public override Task HandleAsync(LoadEventModel message)
        {
            A21DiceGameSaveInfo SaveRoot = OurContainer!.Resolve<A21DiceGameSaveInfo>();
            _thisScore!.LoadLists(SaveRoot.PlayerList);
            _diceControl!.LoadDiceViewModel(ThisMod!.ThisCup!);
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            A21DiceGameSaveInfo SaveRoot = OurContainer!.Resolve<A21DiceGameSaveInfo>();
            _thisScore!.UpdateLists(SaveRoot.PlayerList);
            _diceControl!.UpdateDice(ThisMod!.ThisCup!);
            return Task.CompletedTask;
        }
        ScoreBoardWPF? _thisScore;
        DiceListControlWPF<SimpleDice>? _diceControl; //i think.
        protected async override void AfterGameButton()
        {
            StackPanel thisStack = new StackPanel(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            MainGrid!.Children.Add(thisStack);
            GameButton!.HorizontalAlignment = HorizontalAlignment.Center;
            GameButton.VerticalAlignment = VerticalAlignment.Center;
            thisStack.Children.Add(GameButton);
            var thisRoll = GetGamingButton("Roll Dice", nameof(A21DiceGameViewModel.RollCommand));
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            _diceControl = new DiceListControlWPF<SimpleDice>();
            otherStack.Children.Add(_diceControl);
            otherStack.Children.Add(thisRoll);
            var endButton = GetGamingButton("End Turn", nameof(A21DiceGameViewModel.EndTurnCommand));
            endButton.HorizontalAlignment = HorizontalAlignment.Left;
            otherStack.Children.Add(endButton);
            thisStack.Children.Add(otherStack);
            _thisScore = new ScoreBoardWPF();
            _thisScore.AddColumn("# Of Rolls", true, nameof(A21DiceGamePlayerItem.NumberOfRolls));
            _thisScore.AddColumn("Score", true, nameof(A21DiceGamePlayerItem.Score));
            _thisScore.AddColumn("Was Tie", true, nameof(A21DiceGamePlayerItem.IsFaceOff), useTrueFalse: true);
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(A21DiceGameViewModel.NormalTurn)); // there is no roll number needed for this game.
            firstInfo.AddRow("Status", nameof(A21DiceGameViewModel.Status));
            thisStack.Children.Add(firstInfo.GetContent);
            thisStack.Children.Add(_thisScore);
            AddRestoreCommand(thisStack); //usually to this.  can be to another control if needed.
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterType<BasicGameLoader<A21DiceGamePlayerItem, A21DiceGameSaveInfo>>(); //i think basic game loader gets done here still.
            OurContainer!.RegisterNonSavedClasses<A21DiceGameViewModel>();
            OurContainer.RegisterType<StandardRollProcesses<SimpleDice, A21DiceGamePlayerItem>>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(StandardDiceWPF.GetDiceTag);
            OurContainer.RegisterSingleton<IGenerateDice<int>, SimpleDice>();
        }
    }
}