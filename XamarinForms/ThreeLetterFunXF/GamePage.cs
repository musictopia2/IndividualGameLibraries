using AndyCristinaGamePackageCP.CommonProportionClasses;
using AndyCristinaGamePackageCP.ExtensionClasses;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Interfaces;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Pages;
using BaseGPXPagesAndControlsXF.BasicControls.GameFrames;
using BasicGameFramework.BasicDrawables.BasicClasses;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using ThreeLetterFunCP;
using Xamarin.Forms;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
namespace ThreeLetterFunXF
{
    public class GamePage : MultiPlayerPage<ThreeLetterFunViewModel, ThreeLetterFunPlayerItem, ThreeLetterFunSaveInfo>, INewCard, INewGame
    {
        public enum EnumTestCategory
        {
            None = 0,
            FirstOption = 1,
            CardsPlayer = 2,
            Advanced = 3
        }
        public EnumTestCategory TestMode = EnumTestCategory.None; // want to start with this.
        public GamePage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode) { }
        public override Task HandleAsync(LoadEventModel message)
        {
            ThreeLetterFunMainGameClass mainGame = OurContainer!.Resolve<ThreeLetterFunMainGameClass>();
            GameBoard TempBoard = OurContainer.Resolve<GameBoard>();
            if (mainGame.ThisData!.MultiPlayer == true)
            {
                _thisScore!.LoadLists(mainGame.PlayerList!);
                _first1!.Init();
                _cardsPlayer1!.Init();
                _advanced1!.Init();
            }
            else
                _thisScore!.IsVisible = false;
            _tileBoard1.Init();
            _gameBoard1.LoadList(TempBoard, ThreeLetterFunCardGraphicsCP.TagUsed); //i think
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            ThreeLetterFunMainGameClass mainGame = OurContainer!.Resolve<ThreeLetterFunMainGameClass>();
            GameBoard tempBoard = OurContainer.Resolve<GameBoard>();
            if (mainGame.ThisData!.MultiPlayer == true)
                _thisScore!.UpdateLists(mainGame.PlayerList!);
            _gameBoard1.UpdateList(tempBoard);
            _tileBoard1.Update(); //i think.
            return Task.CompletedTask;
        }
        private ScoreBoardXF? _thisScore;
        private AdvancedXF? _advanced1;
        private CardsPlayerXF? _cardsPlayer1;
        private FirstOptionXF? _first1;
        private readonly TileHandXF _tileBoard1 = new TileHandXF();
        private readonly CardBoardXF<ThreeLetterFunCardData, ThreeLetterFunCardGraphicsCP, CardGraphicsXF> _gameBoard1 = new CardBoardXF<ThreeLetterFunCardData, ThreeLetterFunCardGraphicsCP, CardGraphicsXF>();
        private CardGraphicsXF? _currentCard; //not sure why
        private ThreeLetterFunCardData? _tempCard; //not sure.
        protected override async Task AfterGameButtonAsync()
        {
            StackLayout thisStack = new StackLayout();
            ThisMod!.NewUI = this;
            BasicSetUp();
            //we did have ways to test the loading of the ui (even though you can't do anything).
            if (TestMode != EnumTestCategory.None)
            {
                if (TestMode == EnumTestCategory.FirstOption)
                {
                    FirstOptionViewModel temps = OurContainer!.Resolve<FirstOptionViewModel>();
                    temps.Visible = true;
                    _first1!.Init();
                    ThisMod.CommandContainer!.ManuelFinish = false;
                    ThisMod.CommandContainer.IsExecuting = false;
                    Content = _first1;
                    return;
                }
                if (TestMode == EnumTestCategory.CardsPlayer)
                {
                    CardsPlayerViewModel temps = OurContainer!.Resolve<CardsPlayerViewModel>();
                    temps.Visible = true;
                    _cardsPlayer1!.Init();
                    ThisMod.CommandContainer!.IsExecuting = false;
                    ThisMod.CommandContainer.ManuelFinish = false;
                    Content = _cardsPlayer1;
                    return;
                }
                if (TestMode == EnumTestCategory.Advanced)
                {
                    AdvancedOptionsViewModel temps = OurContainer!.Resolve<AdvancedOptionsViewModel>();
                    temps.Visible = true;
                    _advanced1!.Init();
                    ThisMod.CommandContainer!.IsExecuting = false;
                    ThisMod.CommandContainer.ManuelFinish = false;
                    Content = _advanced1;
                    return;
                }
            }
            _tempCard = new ThreeLetterFunCardData();
            _tempCard.Visible = false;
            _currentCard = new CardGraphicsXF();
            _currentCard.SendSize(ThreeLetterFunCardGraphicsCP.TagUsed, _tempCard); //i think.
            _currentCard.BindingContext = _tempCard;
            GameButton!.HorizontalOptions = LayoutOptions.Center;
            GameButton.VerticalOptions = LayoutOptions.Center;
            thisStack.Children.Add(GameButton);
            var winLabel = GetDefaultLabel();
            winLabel.SetBinding(Label.TextProperty, new Binding(nameof(ThreeLetterFunViewModel.PlayerWon)));
            _thisScore = new ScoreBoardXF();
            _thisScore.AddColumn("Cards Won", true, nameof(ThreeLetterFunPlayerItem.CardsWon));
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            thisStack.Children.Add(_currentCard);
            thisStack.Children.Add(winLabel);
            //Grid grid = new Grid();
            //_gameBoard1.HeightRequest = 500;
            //_gameBoard1.WidthRequest = 500;
            //_gameBoard1.BackgroundColor = Color.Black;
            //AddLeftOverRow(grid, 80);
            //AddLeftOverRow(grid, 20);
            //thisStack.Children.Add(grid);
            //AddControlToGrid(grid, _gameBoard1, 0, 0);
            //_tileBoard1.Margin = new Thickness(5, 5, 5, 5);
            //AddControlToGrid(grid, _tileBoard1, 1, 0);
            thisStack.Children.Add(_gameBoard1);
            thisStack.Children.Add(_tileBoard1);
            var thisBut = GetGamingButton("Play", nameof(ThreeLetterFunViewModel.PlayCommand));
            otherStack.Children.Add(thisBut);
            thisBut = GetGamingButton("Give Up", nameof(ThreeLetterFunViewModel.GiveUpCommand));
            otherStack.Children.Add(thisBut);
            thisBut = GetGamingButton("Take Back", nameof(ThreeLetterFunViewModel.TakeBackCommand));
            otherStack.Children.Add(thisBut);
            thisStack.Children.Add(otherStack);
            thisStack.Children.Add(_thisScore);
            AddRestoreCommand(thisStack); //i think
            MainGrid!.Children.Add(thisStack);
            await FinishUpAsync();
        }
        protected override void ComplexStartControls(Grid thisGrid)
        {
            _advanced1 = new AdvancedXF();
            _cardsPlayer1 = new CardsPlayerXF();
            _first1 = new FirstOptionXF();
            if (TestMode != EnumTestCategory.None)
                return;
            _advanced1.InputTransparent = true;
            _cardsPlayer1.InputTransparent = true;
            _first1.InputTransparent = true;
            thisGrid.Children.Add(_advanced1);
            thisGrid.Children.Add(_cardsPlayer1);
            thisGrid.Children.Add(_first1);
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterType<BasicGameLoader<ThreeLetterFunPlayerItem, ThreeLetterFunSaveInfo>>(); //i think basic game loader gets done here still.
            OurContainer!.RegisterNonSavedClasses<ThreeLetterFunViewModel>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(TileCP.TagUsed);
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(ThreeLetterFunCardGraphicsCP.TagUsed);
            OurContainer.RegisterType<GenericCardShuffler<ThreeLetterFunCardData>>();
            OurContainer.RegisterSingleton<IDeckCount, ThreeLetterFunDeckInfo>();
        }
        void INewCard.ShowNewCard()
        {
            if (ThisMod!.CurrentCard != null)
                _currentCard!.BindingContext = ThisMod.CurrentCard;
            else
                _currentCard!.BindingContext = _tempCard; //hopefully even here, setting to nothing won't hurt.
        }
        public void NewGame()
        {
            _tileBoard1.Update(); //i think.
        }
        public void UpdateBoard()
        {
            GameBoard TempBoard = OurContainer!.Resolve<GameBoard>();
            _gameBoard1.UpdateList(TempBoard);
        }
    }
}