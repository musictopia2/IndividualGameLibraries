using BasicGameFramework.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.BasicClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.Interfaces;;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Pages;
using BaseGPXPagesAndControlsXF.BasicControls.ChoicePickers;
using BaseGPXPagesAndControlsXF.BasicControls.GameFrames;
using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using BaseGPXPagesAndControlsXF.GameGraphics.Dominos;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.Dominos;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using LottoDominosCP;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
using ts = BasicGameFramework.GameGraphicsCP.Dominos.DominosCP;
namespace LottoDominosXF
{
    public class GamePage : MultiPlayerPage<LottoDominosViewModel, LottoDominosPlayerItem, LottoDominosSaveInfo>
    {
        public GamePage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode) { }
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
        private ScoreBoardXF? _score1;
        private CardBoardXF<SimpleDominoInfo, ts, DominosXF<SimpleDominoInfo>>? _gameBoard1;
        private NumberChooserXF? _number1;
        private LottoDominosMainGameClass? _mainGame;
        protected override async Task AfterGameButtonAsync()
        {
            _gameBoard1 = new CardBoardXF<SimpleDominoInfo, ts, DominosXF<SimpleDominoInfo>>();
            _number1 = new NumberChooserXF();
            _number1.TotalRows = 1;
            _score1 = new ScoreBoardXF();
            _mainGame = OurContainer!.Resolve<LottoDominosMainGameClass>();
            StackLayout thisStack = new StackLayout();
            BasicSetUp();
            GameButton!.HorizontalOptions = LayoutOptions.Center;
            GameButton.VerticalOptions = LayoutOptions.Center;
            thisStack.Children.Add(GameButton);
            thisStack.Children.Add(_gameBoard1);
            thisStack.Children.Add(_number1);
            var thisBind = new Binding(nameof(LottoDominosViewModel.NumberVisible));
            var thisBut = GetGamingButton("Choose Number", nameof(LottoDominosViewModel.ChooseNumberCommand));
            thisBut.SetBinding(IsVisibleProperty, thisBind);
            thisStack.Children.Add(thisBut);
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
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