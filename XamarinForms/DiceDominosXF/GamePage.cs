using BasicGameFramework.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.Interfaces;;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Pages;
using BaseGPXPagesAndControlsXF.BasicControls.GameFrames;
using BaseGPXPagesAndControlsXF.BasicControls.Misc;
using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using BaseGPXPagesAndControlsXF.GameGraphics.Dice;
using BaseGPXPagesAndControlsXF.GameGraphics.Dominos;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.Dice;
using BasicGameFramework.Dominos;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using DiceDominosCP;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
using ts = BasicGameFramework.GameGraphicsCP.Dominos.DominosCP;
namespace DiceDominosXF
{
    public class GamePage : MultiPlayerPage<DiceDominosViewModel, DiceDominosPlayerItem, DiceDominosSaveInfo>
    {
        public GamePage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode) { }
        public override Task HandleAsync(LoadEventModel message)
        {
            DiceDominosSaveInfo SaveRoot = OurContainer!.Resolve<DiceDominosSaveInfo>();
            _thisScore!.LoadLists(SaveRoot.PlayerList);
            _diceControl!.LoadDiceViewModel(ThisMod!.ThisCup!);
            _gameBoard1!.LoadList(_mainGame!.GameBoard1!, ts.TagUsed);
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            DiceDominosSaveInfo SaveRoot = OurContainer!.Resolve<DiceDominosSaveInfo>();
            _thisScore!.UpdateLists(SaveRoot.PlayerList);
            _diceControl!.UpdateDice(ThisMod!.ThisCup!);
            _gameBoard1!.UpdateList(_mainGame!.GameBoard1!);
            return Task.CompletedTask;
        }
        ScoreBoardXF? _thisScore;
        DiceListControlXF<SimpleDice>? _diceControl; //i think.
        private DiceDominosMainGameClass? _mainGame;
        private CardBoardXF<SimpleDominoInfo, ts, DominosXF<SimpleDominoInfo>>? _gameBoard1;
        protected override async Task AfterGameButtonAsync()
        {
            _gameBoard1 = new CardBoardXF<SimpleDominoInfo, ts, DominosXF<SimpleDominoInfo>>();
            _thisScore = new ScoreBoardXF();
            _mainGame = OurContainer!.Resolve<DiceDominosMainGameClass>();
            StackLayout thisStack = new StackLayout();
            BasicSetUp();
            GameButton!.HorizontalOptions = LayoutOptions.Center;
            GameButton.VerticalOptions = LayoutOptions.Center;
            thisStack.Children.Add(GameButton);
            thisStack.Children.Add(_gameBoard1);
            var thisRoll = GetPrivateButton("Roll Dice", nameof(DiceDominosViewModel.RollCommand));
            thisRoll.VerticalOptions = LayoutOptions.Start;
            thisRoll.HorizontalOptions = LayoutOptions.Start;
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            thisStack.Children.Add(otherStack);
            _diceControl = new DiceListControlXF<SimpleDice>();
            var endButton = GetPrivateButton("End Turn", nameof(DiceDominosViewModel.EndTurnCommand));
            endButton.HorizontalOptions = LayoutOptions.Start;
            endButton.VerticalOptions = LayoutOptions.Start;
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(DiceDominosViewModel.NormalTurn));
            _thisScore.AddColumn("Dominos Won", true, nameof(DiceDominosPlayerItem.DominosWon), rightMargin: 10);
            if (ScreenUsed == EnumScreen.SmallPhone)
            {
                otherStack.Children.Add(_thisScore);
                otherStack.Children.Add(firstInfo.GetContent);
                otherStack.Children.Add(thisRoll);
                otherStack.Children.Add(endButton);
                otherStack.Children.Add(_diceControl);
            }
            else
            {
                otherStack.Children.Add(thisRoll);
                otherStack.Children.Add(endButton);
                otherStack.Children.Add(_diceControl);
                thisStack.Children.Add(firstInfo.GetContent);
                thisStack.Children.Add(_thisScore);
            }
            MainGrid!.Children.Add(thisStack);
            AddRestoreCommand(thisStack); //i think.  if a stack can't be used, rethink.
            await FinishUpAsync();
        }
        private Button GetPrivateButton(string content, string path)
        {
            if (ScreenUsed == EnumScreen.SmallPhone)
                return GetSmallerButton(content, path);
            return GetGamingButton(content, path);
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterType<BasicGameLoader<DiceDominosPlayerItem, DiceDominosSaveInfo>>(); //i think basic game loader gets done here still.
            OurContainer!.RegisterNonSavedClasses<DiceDominosViewModel>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(ts.TagUsed);
            OurContainer.RegisterType<DominosBasicShuffler<SimpleDominoInfo>>(true);
            OurContainer.RegisterSingleton<IDeckCount, SimpleDominoInfo>();
            OurContainer.RegisterType<StandardRollProcesses<SimpleDice, DiceDominosPlayerItem>>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(StandardDiceXF.GetDiceTag);
            OurContainer.RegisterSingleton<IGenerateDice<int>, SimpleDice>();
        }
    }
}