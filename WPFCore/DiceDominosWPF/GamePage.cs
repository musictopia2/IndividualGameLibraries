using AndyCristinaGamePackageCP.CommonProportionClasses;
using AndyCristinaGamePackageCP.ExtensionClasses;
using BaseGPXWindowsAndControlsCore.BaseWindows;
using BaseGPXWindowsAndControlsCore.BasicControls.GameFrames;
using BaseGPXWindowsAndControlsCore.BasicControls.Misc;
using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using BaseGPXWindowsAndControlsCore.GameGraphics.Dice;
using BaseGPXWindowsAndControlsCore.GameGraphics.Dominos;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.Dice;
using BasicGameFramework.Dominos;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using DiceDominosCP;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
using ts = BasicGameFramework.GameGraphicsCP.Dominos.DominosCP;
namespace DiceDominosWPF
{
    public class GamePage : MultiPlayerWindow<DiceDominosViewModel, DiceDominosPlayerItem, DiceDominosSaveInfo>
    {
        public GamePage(IStartUp starts, EnumGamePackageMode mode) //this means something needs to put into here.
        {
            BuildXAML(starts, mode);
        }
        public override Task HandleAsync(LoadEventModel message)
        {
            DiceDominosSaveInfo saveRoot = OurContainer!.Resolve<DiceDominosSaveInfo>();
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _diceControl!.LoadDiceViewModel(ThisMod!.ThisCup!);
            _gameBoard1!.LoadList(_mainGame!.GameBoard1!, ts.TagUsed);
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            DiceDominosSaveInfo saveRoot = OurContainer!.Resolve<DiceDominosSaveInfo>();
            _thisScore!.UpdateLists(saveRoot.PlayerList);
            _diceControl!.UpdateDice(ThisMod!.ThisCup!);
            _gameBoard1!.UpdateList(_mainGame!.GameBoard1!);
            return Task.CompletedTask;
        }
        ScoreBoardWPF? _thisScore;
        DiceListControlWPF<SimpleDice>? _diceControl; //i think.
        private DiceDominosMainGameClass? _mainGame;
        private CardBoardWPF<SimpleDominoInfo, ts, DominosWPF<SimpleDominoInfo>>? _gameBoard1;
        protected async override void AfterGameButton()
        {
            _gameBoard1 = new CardBoardWPF<SimpleDominoInfo, ts, DominosWPF<SimpleDominoInfo>>();
            _thisScore = new ScoreBoardWPF();
            _mainGame = OurContainer!.Resolve<DiceDominosMainGameClass>();
            StackPanel thisStack = new StackPanel();
            BasicSetUp();
            GameButton!.HorizontalAlignment = HorizontalAlignment.Center;
            GameButton.VerticalAlignment = VerticalAlignment.Center;
            thisStack.Children.Add(GameButton);
            thisStack.Children.Add(_gameBoard1);
            var thisRoll = GetGamingButton("Roll Dice", nameof(DiceDominosViewModel.RollCommand));
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            _diceControl = new DiceListControlWPF<SimpleDice>();
            otherStack.Children.Add(_diceControl);
            otherStack.Children.Add(thisRoll);
            var endButton = GetGamingButton("End Turn", nameof(DiceDominosViewModel.EndTurnCommand));
            endButton.HorizontalAlignment = HorizontalAlignment.Left;
            thisStack.Children.Add(otherStack);
            thisStack.Children.Add(endButton);
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(DiceDominosViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(DiceDominosViewModel.Status));
            thisStack.Children.Add(firstInfo.GetContent);
            _thisScore.AddColumn("Dominos Won", true, nameof(DiceDominosPlayerItem.DominosWon), rightMargin: 10);
            thisStack.Children.Add(_thisScore);
            MainGrid!.Children.Add(thisStack);
            AddRestoreCommand(thisStack); //i think.  if a stack can't be used, rethink.
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterType<BasicGameLoader<DiceDominosPlayerItem, DiceDominosSaveInfo>>(); //i think basic game loader gets done here still.
            OurContainer!.RegisterNonSavedClasses<DiceDominosViewModel>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(ts.TagUsed);
            OurContainer.RegisterType<DominosBasicShuffler<SimpleDominoInfo>>(true);
            OurContainer.RegisterSingleton<IDeckCount, SimpleDominoInfo>();
            OurContainer.RegisterType<StandardRollProcesses<SimpleDice, DiceDominosPlayerItem>>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(StandardDiceWPF.GetDiceTag);
            OurContainer.RegisterSingleton<IGenerateDice<int>, SimpleDice>();
        }
    }
}