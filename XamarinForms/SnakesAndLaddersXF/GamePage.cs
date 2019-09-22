using BasicGameFramework.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.Interfaces;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Pages;
using BaseGPXPagesAndControlsXF.BasicControls.Misc;
using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using BaseGPXPagesAndControlsXF.GameGraphics.Dice;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.Dice;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using CommonBasicStandardLibraries.Messenging;
using SnakesAndLaddersCP;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
namespace SnakesAndLaddersXF
{
    public class GamePage : MultiPlayerPage<SnakesAndLaddersViewModel, SnakesAndLaddersPlayerItem, SnakesAndLaddersSaveInfo>, IHandle<NewTurnEventModel>
    {
        public GamePage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode) { }
        public override Task HandleAsync(LoadEventModel message)
        {
            _diceControl!.LoadDiceViewModel(ThisMod!.ThisCup!);
            privateBoard!.LoadBoard();
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            _diceControl!.UpdateDice(ThisMod!.ThisCup!);
            return Task.CompletedTask;
        }
        private DiceListControlXF<SimpleDice>? _diceControl;
        private GamePieceXF? _pieceTurn;
        private GameboardXF? privateBoard;
        private SnakesAndLaddersMainGameClass? _mainGame;
        protected override async Task AfterGameButtonAsync()
        {
            StackLayout thisStack = new StackLayout();
            BasicSetUp();
            GameButton!.HorizontalOptions = LayoutOptions.Center;
            GameButton.VerticalOptions = LayoutOptions.Center;
            thisStack.Children.Add(GameButton);
            privateBoard = new GameboardXF();
            privateBoard.Margin = new Thickness(5, 5, 5, 5);
            privateBoard.HorizontalOptions = LayoutOptions.Start;
            privateBoard.VerticalOptions = LayoutOptions.Start;
            thisStack.Children.Add(privateBoard);
            var thisRoll = GetGamingButton("Roll Dice", nameof(SnakesAndLaddersViewModel.RollDiceCommand));
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            _diceControl = new DiceListControlXF<SimpleDice>();
            otherStack.Children.Add(thisRoll);
            _pieceTurn = new GamePieceXF();
            if (ScreenUsed == EnumScreen.SmallPhone)
            {
                _pieceTurn.WidthRequest = 40;
                _pieceTurn.HeightRequest = 40;
            }
            else
            {
                _pieceTurn.WidthRequest = 90;
                _pieceTurn.HeightRequest = 90;
            }
            _pieceTurn.Margin = new Thickness(10, 0, 0, 0);
            _pieceTurn.NeedsSubscribe = false; // won't notify in this case.  just let them know when new turn.  otherwise, when this number changes, it will trigger for the gameboard (which is not good)
            _pieceTurn.Init();
            otherStack.Children.Add(_pieceTurn);
            otherStack.Children.Add(_diceControl);
            thisStack.Children.Add(otherStack);
            EventAggregator thisE = OurContainer!.Resolve<EventAggregator>();
            thisE.Subscribe(this);
            _mainGame = OurContainer.Resolve<SnakesAndLaddersMainGameClass>();
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
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
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(StandardDiceXF.GetDiceTag);
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