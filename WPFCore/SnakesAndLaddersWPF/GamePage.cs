using BasicGameFramework.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BaseGPXWindowsAndControlsCore.BaseWindows;
using BaseGPXWindowsAndControlsCore.BasicControls.Misc;
using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using BaseGPXWindowsAndControlsCore.GameGraphics.Dice;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.Dice;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using CommonBasicStandardLibraries.Messenging;
using SnakesAndLaddersCP;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
namespace SnakesAndLaddersWPF
{
    public class GamePage : MultiPlayerWindow<SnakesAndLaddersViewModel, SnakesAndLaddersPlayerItem, SnakesAndLaddersSaveInfo>, IHandle<NewTurnEventModel>
    {
        public GamePage(IStartUp starts, EnumGamePackageMode mode) //this means something needs to put into here.
        {
            BuildXAML(starts, mode);
        }
        public override Task HandleAsync(LoadEventModel message)
        {
            _diceControl!.LoadDiceViewModel(ThisMod!.ThisCup!);
            privateBoard!.LoadBoard();
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            _diceControl!.UpdateDice(ThisMod!.ThisCup!);
            privateBoard!.UpdateBoard();
            return Task.CompletedTask;
        }
        private DiceListControlWPF<SimpleDice>? _diceControl;
        private GamePieceWPF? _pieceTurn;
        private GameboardWPF? privateBoard;
        private SnakesAndLaddersMainGameClass? _mainGame;
        protected async override void AfterGameButton()
        {
            StackPanel thisStack = new StackPanel();
            BasicSetUp();
            GameButton!.HorizontalAlignment = HorizontalAlignment.Center;
            GameButton.VerticalAlignment = VerticalAlignment.Center;
            thisStack.Children.Add(GameButton);
            privateBoard = new GameboardWPF();
            privateBoard.Margin = new Thickness(5, 5, 5, 5);
            privateBoard.HorizontalAlignment = HorizontalAlignment.Left;
            privateBoard.VerticalAlignment = VerticalAlignment.Top;
            thisStack.Children.Add(privateBoard);
            var thisRoll = GetGamingButton("Roll Dice", nameof(SnakesAndLaddersViewModel.RollDiceCommand));
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            _diceControl = new DiceListControlWPF<SimpleDice>();
            otherStack.Children.Add(thisRoll);
            _pieceTurn = new GamePieceWPF();
            _pieceTurn.Width = 80;
            _pieceTurn.Height = 80; // try this way.
            _pieceTurn.Margin = new Thickness(10, 0, 0, 0);
            _pieceTurn.NeedsSubscribe = false; // won't notify in this case.  just let them know when new turn.  otherwise, when this number changes, it will trigger for the gameboard (which is not good)
            _pieceTurn.Init();
            otherStack.Children.Add(_pieceTurn);
            otherStack.Children.Add(_diceControl);
            thisStack.Children.Add(otherStack);
            EventAggregator thisE = OurContainer!.Resolve<EventAggregator>();
            thisE.Subscribe(this);
            _mainGame = OurContainer.Resolve<SnakesAndLaddersMainGameClass>();
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(SnakesAndLaddersViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(SnakesAndLaddersViewModel.Status));
            thisStack.Children.Add(firstInfo.GetContent);
            MainGrid!.Children.Add(thisStack);
            AddRestoreCommand(thisStack);
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterType<BasicGameLoader<SnakesAndLaddersPlayerItem, SnakesAndLaddersSaveInfo>>(); //i think basic game loader gets done here still.
            OurContainer!.RegisterNonSavedClasses<SnakesAndLaddersViewModel>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(StandardDiceWPF.GetDiceTag);
            OurContainer.RegisterSingleton<IGenerateDice<int>, SimpleDice>();
            OurContainer.RegisterType<StandardRollProcesses<SimpleDice, SnakesAndLaddersPlayerItem>>();
        }
        public void Handle(NewTurnEventModel message)
        {
            _pieceTurn!.Index = _mainGame!.SaveRoot!.PlayOrder.WhoTurn;
            if (_mainGame.SingleInfo!.SpaceNumber == 0)
                _pieceTurn.Number = _mainGame.SaveRoot.PlayOrder.WhoTurn; // i think needs to be this so something can show up.
            else
                _pieceTurn.Number = _mainGame.SingleInfo.SpaceNumber;
        }
    }
}