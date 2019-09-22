using BasicGameFramework.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.Interfaces;;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Pages;
using BaseGPXPagesAndControlsXF.BasicControls.GameFrames;
using BaseGPXPagesAndControlsXF.BasicControls.Misc;
using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using BaseGPXPagesAndControlsXF.BasicControls.SingleCardFrames;
using BaseGPXPagesAndControlsXF.GameGraphics.Cards;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using BasicGameFramework.RegularDeckOfCards;
using FourSuitRummyCP;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
using static BasicXFControlsAndPages.Helpers.GridHelper; //just in case
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace FourSuitRummyXF
{
    public class GamePage : MultiPlayerPage<FourSuitRummyViewModel, FourSuitRummyPlayerItem, FourSuitRummySaveInfo>
    {
        public GamePage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode) { }

        public override Task HandleAsync(LoadEventModel message)
        {
            FourSuitRummySaveInfo saveRoot = OurContainer!.Resolve<FourSuitRummySaveInfo>();
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _playerHand!.LoadList(ThisMod!.PlayerHand1!, ts.TagUsed); // i think
            _discardGPile!.Init(ThisMod.Pile1!, ts.TagUsed); // may have to be here (well see)
            _discardGPile.StartListeningDiscardPile(); // its the main one.
            _deckGPile!.Init(ThisMod.Deck1!, ts.TagUsed); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();
            _tempG!.Init(ThisMod.TempSets!, ts.TagUsed);
            LoadPlayerMainSets();
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            FourSuitRummySaveInfo saveRoot = OurContainer!.Resolve<FourSuitRummySaveInfo>();
            _thisScore!.UpdateLists(saveRoot.PlayerList);
            _playerHand!.UpdateList(ThisMod!.PlayerHand1!);
            _deckGPile!.UpdateDeck(ThisMod.Deck1!);
            _discardGPile!.UpdatePile(ThisMod.Pile1!);
            _tempG!.Update(ThisMod.TempSets!);
            LoadPlayerMainSets();
            return Task.CompletedTask;
        }
        private void LoadPlayerMainSets()
        {
            _setGrid!.Children.Clear(); //i think its best to just start over this time.
            var thisPlayer = _mainGame!.PlayerList!.GetSelf();
            MainRummySetsXF<EnumSuitList, EnumColorList,
                RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>,
                SetInfo, SavedSet> mainG = new MainRummySetsXF<EnumSuitList, EnumColorList, RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>, SetInfo, SavedSet>();
            AddControlToGrid(_setGrid, mainG, 0, 0);
            mainG.Init(thisPlayer.MainSets!, ts.TagUsed);
            int newPlayer;
            if (thisPlayer.Id == 1)
                newPlayer = 2;
            else
                newPlayer = 1;
            thisPlayer = _mainGame.PlayerList[newPlayer];
            mainG = new MainRummySetsXF<EnumSuitList, EnumColorList, RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>, SetInfo, SavedSet>();
            AddControlToGrid(_setGrid, mainG, 0, 1);
            mainG.Init(thisPlayer.MainSets!, ts.TagUsed);
        }
        private BaseDeckXF<RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>>? _deckGPile;
        private BasePileXF<RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>>? _discardGPile;
        private ScoreBoardXF? _thisScore;
        private BaseHandXF<RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>>? _playerHand;
        private TempRummySetsXF<EnumSuitList, EnumColorList, RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>>? _tempG;
        private Grid? _setGrid;
        private FourSuitRummyMainGameClass? _mainGame;
        protected override async Task AfterGameButtonAsync()
        {
            BasicSetUp();
            _mainGame = OurContainer!.Resolve<FourSuitRummyMainGameClass>();
            _deckGPile = new BaseDeckXF<RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>>();
            _discardGPile = new BasePileXF<RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>>();
            _thisScore = new ScoreBoardXF();
            _playerHand = new BaseHandXF<RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>>();
            _tempG = new TempRummySetsXF<EnumSuitList, EnumColorList, RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>>();
            Grid finalGrid = new Grid();
            AddAutoRows(finalGrid, 3);
            Grid firstGrid = new Grid();
            AddLeftOverColumn(firstGrid, 40);
            AddAutoColumns(firstGrid, 1);
            AddLeftOverColumn(firstGrid, 15);
            AddLeftOverColumn(firstGrid, 30);
            RoundButton!.HorizontalOptions = LayoutOptions.Center;
            RoundButton.VerticalOptions = LayoutOptions.Center;
            GameButton!.HorizontalOptions = LayoutOptions.Center;
            GameButton.VerticalOptions = LayoutOptions.Center;
            AddControlToGrid(finalGrid, GameButton, 0, 0);
            AddControlToGrid(finalGrid, RoundButton, 0, 0);
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            AddControlToGrid(firstGrid, otherStack, 0, 1);
            StackLayout firstStack = new StackLayout();
            firstStack.Children.Add(_playerHand);
            var thisBut = GetSmallerButton("Play Sets", nameof(FourSuitRummyViewModel.PlaySetsCommand));
            firstStack.Children.Add(thisBut);
            AddControlToGrid(firstGrid, firstStack, 0, 0);
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(FourSuitRummyViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(FourSuitRummyViewModel.Status));
            AddControlToGrid(firstGrid, firstInfo.GetContent, 0, 2);
            _thisScore.AddColumn("Cards Left", true, nameof(FourSuitRummyPlayerItem.ObjectCount)); //very common.
            _thisScore.AddColumn("Total Score", true, nameof(FourSuitRummyPlayerItem.TotalScore));
            AddControlToGrid(firstGrid, _thisScore, 0, 3);
            AddControlToGrid(finalGrid, firstGrid, 1, 0);
            StackLayout thirdStack = new StackLayout();
            thirdStack.Orientation = StackOrientation.Horizontal;
            thirdStack.Children.Add(_tempG);
            _setGrid = new Grid();
            thirdStack.Children.Add(_setGrid);
            AddControlToGrid(finalGrid, thirdStack, 2, 0);
            AddLeftOverColumn(_setGrid, 50);
            AddLeftOverColumn(_setGrid, 50);
            MainGrid!.Children.Add(finalGrid);
            _deckGPile.Margin = new Thickness(5, 5, 5, 5);
            _discardGPile.Margin = new Thickness(5, 5, 5, 5);
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterType<BasicGameLoader<FourSuitRummyPlayerItem, FourSuitRummySaveInfo>>(); //i think basic game loader gets done here still.
            OurContainer!.RegisterCommonRegularCards<FourSuitRummyViewModel, RegularRummyCard>(registerCommonProportions: false);
            OurContainer.RegisterSingleton<IProportionImage, SmallDrawableProportion>(ts.TagUsed);
        }
    }
}