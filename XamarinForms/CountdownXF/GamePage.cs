using BasicGameFramework.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.Interfaces;;
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
using CountdownCP;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
namespace CountdownXF
{
    public class GamePage : MultiPlayerPage<CountdownViewModel, CountdownPlayerItem, CountdownSaveInfo>
    {
        public GamePage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode) { }
        private bool DidLoad;
        public override Task HandleAsync(LoadEventModel message)
        {
            if (DidLoad == true)
                return Task.CompletedTask;
            DidLoad = true;
            _diceControl!.LoadDiceViewModel(ThisMod!.ThisCup!);
            _playerStack.Orientation = StackOrientation.Horizontal;
            _playerStack.Margin = new Thickness(5, 5, 5, 5);
            //self goes on top.
            //however, if its pass and play, then 1 then 2.
            CountdownPlayerItem thisPlayer;
            CountdownMainGameClass mainGame = OurContainer!.Resolve<CountdownMainGameClass>();
            if (mainGame.ThisData!.MultiPlayer == false)
                thisPlayer = mainGame.PlayerList![1];
            else
                thisPlayer = mainGame.PlayerList!.GetSelf();
            PlayerBoardXF thisBoard = new PlayerBoardXF();
            thisBoard.Margin = new Thickness(3, 3, 3, 3);
            thisBoard.LoadBoard(thisPlayer);
            _playerStack.Children.Add(thisBoard);
            if (thisPlayer.Id == 1)
                thisPlayer = mainGame.PlayerList[2];
            else
                thisPlayer = mainGame.PlayerList[1];
            thisBoard = new PlayerBoardXF();
            thisBoard.Margin = new Thickness(3, 3, 3, 3);
            thisBoard.LoadBoard(thisPlayer);
            _playerStack.Children.Add(thisBoard);
            return Task.CompletedTask;
        }

        public override Task HandleAsync(UpdateEventModel message)
        {
            _diceControl!.UpdateDice(ThisMod!.ThisCup!);
            CountdownPlayerItem ThisPlayer;
            CountdownMainGameClass MainGame = OurContainer!.Resolve<CountdownMainGameClass>();
            if (MainGame.ThisData!.MultiPlayer == false)
                ThisPlayer = MainGame.PlayerList![1];
            else
                ThisPlayer = MainGame.PlayerList!.GetSelf();
            PlayerBoardXF? thisBoard = _playerStack.Children[0] as PlayerBoardXF;
            thisBoard!.UpdateBoard(ThisPlayer);
            if (ThisPlayer.Id == 1)
                ThisPlayer = MainGame.PlayerList[2];
            else
                ThisPlayer = MainGame.PlayerList[1];
            thisBoard = _playerStack.Children[1] as PlayerBoardXF;
            thisBoard!.UpdateBoard(ThisPlayer);
            EventAggregator thisE = OurContainer.Resolve<EventAggregator>();
            thisE.RepaintBoard(); //just in case.
            return Task.CompletedTask;
        }
        DiceListControlXF<CountdownDice>? _diceControl; //i think.
        private readonly StackLayout _playerStack = new StackLayout();
        protected override async Task AfterGameButtonAsync()
        {
            StackLayout thisStack = new StackLayout(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            MainGrid!.Children.Add(thisStack);
            GameButton!.HorizontalOptions = LayoutOptions.Center;
            GameButton.VerticalOptions = LayoutOptions.Center;
            thisStack.Children.Add(GameButton);
            thisStack.Children.Add(_playerStack);
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            _diceControl = new DiceListControlXF<CountdownDice>();
            otherStack.Children.Add(_diceControl);
            var endButton = GetGamingButton("End Turn", nameof(CountdownViewModel.EndTurnCommand));
            endButton.HorizontalOptions = LayoutOptions.Start;
            otherStack.Children.Add(endButton);
            var thisBut = GetGamingButton("Show Hints", nameof(CountdownViewModel.HintCommand));
            thisBut.HorizontalOptions = LayoutOptions.Start;
            otherStack.Children.Add(thisBut);
            thisStack.Children.Add(otherStack);
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(CountdownViewModel.NormalTurn)); // there is no roll number needed for this game.
            firstInfo.AddRow("Round", nameof(CountdownViewModel.Round)); //if you don't need, it comment it out.
            firstInfo.AddRow("Status", nameof(CountdownViewModel.Status));
            thisStack.Children.Add(firstInfo.GetContent);
            AddRestoreCommand(thisStack); //usually to this.  can be to another control if needed.
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterSingleton<IProportionBoard, CustomProportion>("");
            OurContainer!.RegisterType<BasicGameLoader<CountdownPlayerItem, CountdownSaveInfo>>(); //i think basic game loader gets done here still.
            OurContainer!.RegisterNonSavedClasses<CountdownViewModel>();
            OurContainer.RegisterType<StandardRollProcesses<CountdownDice, CountdownPlayerItem>>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(StandardDiceXF.GetDiceTag);
            OurContainer.RegisterSingleton<IGenerateDice<int>, CountdownDice>();
        }
    }
}