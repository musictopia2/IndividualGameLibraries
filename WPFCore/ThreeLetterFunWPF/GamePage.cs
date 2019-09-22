using BasicGameFramework.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BaseGPXWindowsAndControlsCore.BaseWindows;
using BaseGPXWindowsAndControlsCore.BasicControls.GameFrames;
using BasicGameFramework.BasicDrawables.BasicClasses;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ThreeLetterFunCP;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
namespace ThreeLetterFunWPF
{
    public class GamePage : MultiPlayerWindow<ThreeLetterFunViewModel, ThreeLetterFunPlayerItem, ThreeLetterFunSaveInfo>, INewCard, INewGame
    {
        public enum EnumTestCategory
        {
            None = 0,
            FirstOption = 1,
            CardsPlayer = 2,
            Advanced = 3
        }
        public EnumTestCategory TestMode = EnumTestCategory.None; // want to start with this.
        public GamePage(IStartUp starts, EnumGamePackageMode mode) //this means something needs to put into here.
        {
            BuildXAML(starts, mode);
        }
        public override Task HandleAsync(LoadEventModel message)
        {
            ThreeLetterFunMainGameClass mainGame = OurContainer!.Resolve<ThreeLetterFunMainGameClass>();
            GameBoard tempBoard = OurContainer.Resolve<GameBoard>();
            if (mainGame.ThisData!.MultiPlayer == true)
            {
                _thisScore!.LoadLists(mainGame.PlayerList!);
                _first1!.Init();
                _cardsPlayer1!.Init();
                _advanced1!.Init();
            }
            else
                _thisScore!.Visibility = Visibility.Collapsed;
            _tileBoard1.Init();
            _gameBoard1.LoadList(tempBoard, ThreeLetterFunCardGraphicsCP.TagUsed); //i think
            return Task.CompletedTask;
        }

        public override Task HandleAsync(UpdateEventModel message)
        {
            ThreeLetterFunMainGameClass mainGame = OurContainer!.Resolve<ThreeLetterFunMainGameClass>();
            GameBoard TempBoard = OurContainer.Resolve<GameBoard>();
            if (mainGame.ThisData!.MultiPlayer == true)
                _thisScore!.UpdateLists(mainGame.PlayerList!);
            _gameBoard1.UpdateList(TempBoard);
            _tileBoard1.Update(); //i think.
            return Task.CompletedTask;
        }
        private ScoreBoardWPF? _thisScore;
        private AdvancedWPF? _advanced1;
        private CardsPlayerWPF? _cardsPlayer1;
        private FirstOptionWPF? _first1;
        private readonly TileHandWPF _tileBoard1 = new TileHandWPF();
        private readonly CardBoardWPF<ThreeLetterFunCardData, ThreeLetterFunCardGraphicsCP, CardGraphicsWPF> _gameBoard1 = new CardBoardWPF<ThreeLetterFunCardData, ThreeLetterFunCardGraphicsCP, CardGraphicsWPF>();
        private CardGraphicsWPF? _currentCard; //not sure why
        private ThreeLetterFunCardData? _tempCard; //not sure.
        protected async override void AfterGameButton()
        {
            StackPanel thisStack = new StackPanel();
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
            _currentCard = new CardGraphicsWPF();
            _currentCard.SendSize(ThreeLetterFunCardGraphicsCP.TagUsed, _tempCard); //i think.
            _currentCard.DataContext = _tempCard;
            GameButton!.HorizontalAlignment = HorizontalAlignment.Center;
            GameButton.VerticalAlignment = VerticalAlignment.Center;
            thisStack.Children.Add(GameButton);
            var winLabel = GetDefaultLabel();
            winLabel.SetBinding(TextBlock.TextProperty, new Binding(nameof(ThreeLetterFunViewModel.PlayerWon)));
            _thisScore = new ScoreBoardWPF();
            _thisScore.AddColumn("Cards Won", true, nameof(ThreeLetterFunPlayerItem.CardsWon));
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            thisStack.Children.Add(_currentCard);
            thisStack.Children.Add(winLabel);
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
            _advanced1 = new AdvancedWPF();
            _cardsPlayer1 = new CardsPlayerWPF();
            _first1 = new FirstOptionWPF();
            if (TestMode != EnumTestCategory.None)
                return;
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
        //protected override void RegisterTests()
        //{
        //    ThisTest.SaveOption = BasicGameFramework.TestUtilities.EnumTestSaveCategory.RestoreOnly;
        //}
        void INewCard.ShowNewCard()
        {
            if (ThisMod!.CurrentCard != null)
                _currentCard!.DataContext = ThisMod.CurrentCard;
            else
                _currentCard!.DataContext = _tempCard; //hopefully even here, setting to nothing won't hurt.
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