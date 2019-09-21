using AndyCristinaGamePackageCP.CommonProportionClasses;
using AndyCristinaGamePackageCP.ExtensionClasses;
using BaseGPXWindowsAndControlsCore.BaseWindows;
using BaseGPXWindowsAndControlsCore.BasicControls.ChoicePickers;
using BaseGPXWindowsAndControlsCore.BasicControls.GameFrames;
using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using BaseGPXWindowsAndControlsCore.GameGraphics.Dominos;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.Dominos;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using LottoDominosCP;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
using ts = BasicGameFramework.GameGraphicsCP.Dominos.DominosCP;
namespace LottoDominosWPF
{
    public class GamePage : MultiPlayerWindow<LottoDominosViewModel, LottoDominosPlayerItem, LottoDominosSaveInfo>
    {
        public GamePage(IStartUp starts, EnumGamePackageMode mode) //this means something needs to put into here.
        {
            BuildXAML(starts, mode);
        }
        public override Task HandleAsync(LoadEventModel message)
        {
            _score1!.LoadLists(_mainGame!.SaveRoot!.PlayerList);
            _gameBoard1!.LoadList(_mainGame.GameBoard1, ts.TagUsed);
            _number1!.LoadLists(ThisMod!.Number1!);
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            _score1!.UpdateLists(_mainGame!.SaveRoot!.PlayerList);
            _gameBoard1!.UpdateList(_mainGame.GameBoard1);
            return Task.CompletedTask;
        }
        private ScoreBoardWPF? _score1;
        private CardBoardWPF<SimpleDominoInfo, ts, DominosWPF<SimpleDominoInfo>>? _gameBoard1;
        private NumberChooserWPF? _number1;
        private LottoDominosMainGameClass? _mainGame;
        protected async override void AfterGameButton()
        {
            _gameBoard1 = new CardBoardWPF<SimpleDominoInfo, ts, DominosWPF<SimpleDominoInfo>>();
            _number1 = new NumberChooserWPF();
            _score1 = new ScoreBoardWPF();
            _mainGame = OurContainer!.Resolve<LottoDominosMainGameClass>();
            StackPanel thisStack = new StackPanel();
            BasicSetUp();
            GameButton!.HorizontalAlignment = HorizontalAlignment.Center;
            GameButton.VerticalAlignment = VerticalAlignment.Center;
            thisStack.Children.Add(GameButton);
            thisStack.Children.Add(_gameBoard1);
            thisStack.Children.Add(_number1);
            var thisBind = GetVisibleBinding(nameof(LottoDominosViewModel.NumberVisible));
            var thisBut = GetGamingButton("Choose Number", nameof(LottoDominosViewModel.ChooseNumberCommand));
            thisBut.SetBinding(Button.VisibilityProperty, thisBind);
            thisStack.Children.Add(thisBut);
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(LottoDominosViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(LottoDominosViewModel.Status));
            thisStack.Children.Add(firstInfo.GetContent);
            _score1.AddColumn("# Chosen", true, nameof(LottoDominosPlayerItem.NumberChosen));
            _score1.AddColumn("# Won", true, nameof(LottoDominosPlayerItem.NumberWon));
            thisStack.Children.Add(_score1);
            MainGrid!.Children.Add(thisStack);
            AddRestoreCommand(thisStack); //i think.  if a stack can't be used, rethink.
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterType<BasicGameLoader<LottoDominosPlayerItem, LottoDominosSaveInfo>>(); //i think basic game loader gets done here still.
            OurContainer!.RegisterNonSavedClasses<LottoDominosViewModel>();
            OurContainer.RegisterType<StandardWidthHeight>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(ts.TagUsed);
            OurContainer.RegisterType<DominosBasicShuffler<SimpleDominoInfo>>(true);
            OurContainer.RegisterSingleton<IDeckCount, SimpleDominoInfo>();
        }
    }
}