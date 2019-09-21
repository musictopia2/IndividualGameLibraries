using AndyCristinaGamePackageCP.CommonProportionClasses;
using AndyCristinaGamePackageCP.DataClasses;
using AndyCristinaGamePackageCP.ExtensionClasses;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Interfaces;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Pages;
using BaseGPXPagesAndControlsXF.BasicControls.GameFrames;
using BaseGPXPagesAndControlsXF.BasicControls.Misc;
using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using BaseGPXPagesAndControlsXF.BasicControls.SingleCardFrames;
using BaseGPXPagesAndControlsXF.GameGraphics.Cards;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using BasicGameFramework.RegularDeckOfCards;
using ChinazoCP;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static AndyCristinaGamePackageCP.DataClasses.GlobalStaticClass;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
using static BasicXFControlsAndPages.Helpers.GridHelper; //just in case
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace ChinazoXF
{
    public class GamePage : MultiPlayerPage<ChinazoViewModel, ChinazoPlayerItem, ChinazoSaveInfo>
    {
        public GamePage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode) { }
        public override Task HandleAsync(LoadEventModel message)
        {
            ChinazoSaveInfo saveRoot = OurContainer!.Resolve<ChinazoSaveInfo>();
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _playerHand!.LoadList(ThisMod!.PlayerHand1!, ts.TagUsed); // i think
            _discardGPile!.Init(ThisMod.Pile1!, ts.TagUsed); // may have to be here (well see)
            _discardGPile.StartListeningDiscardPile(); // its the main one.
            _deckGPile!.Init(ThisMod.Deck1!, ts.TagUsed); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();
            _tempG!.Init(ThisMod.TempSets!, ts.TagUsed);
            _mainG!.Init(ThisMod.MainSets!, ts.TagUsed);
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            ChinazoSaveInfo saveRoot = OurContainer!.Resolve<ChinazoSaveInfo>();
            _thisScore!.UpdateLists(saveRoot.PlayerList);
            _playerHand!.UpdateList(ThisMod!.PlayerHand1!);
            _deckGPile!.UpdateDeck(ThisMod.Deck1!);
            _discardGPile!.UpdatePile(ThisMod.Pile1!);
            _tempG!.Update(ThisMod.TempSets!);
            _mainG!.Update(ThisMod.MainSets!);
            return Task.CompletedTask;
        }
        private BaseDeckXF<ChinazoCard, ts, DeckOfCardsXF<ChinazoCard>>? _deckGPile;
        private BasePileXF<ChinazoCard, ts, DeckOfCardsXF<ChinazoCard>>? _discardGPile;
        private ScoreBoardXF? _thisScore;
        private BaseHandXF<ChinazoCard, ts, DeckOfCardsXF<ChinazoCard>>? _playerHand;
        private TempRummySetsXF<EnumSuitList, EnumColorList, ChinazoCard, ts, DeckOfCardsXF<ChinazoCard>>? _tempG;
        private MainRummySetsXF<EnumSuitList, EnumColorList, ChinazoCard, ts, DeckOfCardsXF<ChinazoCard>, PhaseSet, SavedSet>? _mainG;
        protected override async Task AfterGameButtonAsync()
        {
            StackLayout thisStack = new StackLayout(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            _deckGPile = new BaseDeckXF<ChinazoCard, ts, DeckOfCardsXF<ChinazoCard>>();
            _discardGPile = new BasePileXF<ChinazoCard, ts, DeckOfCardsXF<ChinazoCard>>();
            _thisScore = new ScoreBoardXF();
            _playerHand = new BaseHandXF<ChinazoCard, ts, DeckOfCardsXF<ChinazoCard>>();
            _tempG = new TempRummySetsXF<EnumSuitList, EnumColorList, ChinazoCard, ts, DeckOfCardsXF<ChinazoCard>>();
            _mainG = new MainRummySetsXF<EnumSuitList, EnumColorList, ChinazoCard, ts, DeckOfCardsXF<ChinazoCard>, PhaseSet, SavedSet>();
            GameButton!.HorizontalOptions = LayoutOptions.Center;
            GameButton.VerticalOptions = LayoutOptions.Center;
            RoundButton!.HorizontalOptions = LayoutOptions.Center;
            RoundButton.VerticalOptions = LayoutOptions.Center;
            thisStack.Children.Add(GameButton);
            thisStack.Children.Add(RoundButton);
            Grid finalGrid = new Grid();
            AddAutoRows(finalGrid, 2); // has to be this way because of scoreboard.
            AddLeftOverRow(finalGrid, 1);
            AddControlToGrid(finalGrid, GameButton, 0, 0);
            AddControlToGrid(finalGrid, RoundButton, 0, 0);
            Grid firstGrid = new Grid();
            if (ScreenUsed == EnumScreen.SmallTablet)
            {
                AddLeftOverColumn(firstGrid, 35);
                AddAutoColumns(firstGrid, 1);
                AddLeftOverColumn(firstGrid, 20);
                AddLeftOverColumn(firstGrid, 20);
            }
            else
            {
                AddLeftOverColumn(firstGrid, 60);
                AddAutoColumns(firstGrid, 1);
                AddLeftOverColumn(firstGrid, 30);
                AddLeftOverColumn(firstGrid, 20);
            }
            thisStack.Children.Add(finalGrid);
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            AddControlToGrid(firstGrid, otherStack, 0, 1);
            StackLayout firstStack = new StackLayout();
            firstStack.Children.Add(_playerHand);
            StackLayout secondStack = new StackLayout();
            secondStack.Orientation = StackOrientation.Horizontal;
            firstStack.Children.Add(secondStack);
            var thisBut = GetSmallerButton("Pass", nameof(ChinazoViewModel.PassCommand));
            secondStack.Children.Add(thisBut);
            thisBut = GetSmallerButton("Take", nameof(ChinazoViewModel.TakeCommand));
            secondStack.Children.Add(thisBut);
            thisBut = GetSmallerButton("Lay Down Sets", nameof(ChinazoViewModel.InitSetsCommand));
            firstStack.Children.Add(thisBut);
            AddControlToGrid(firstGrid, firstStack, 0, 0);
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(ChinazoViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(ChinazoViewModel.Status));
            firstInfo.AddRow("Other Turn", nameof(ChinazoViewModel.OtherLabel));
            firstInfo.AddRow("Phase", nameof(ChinazoViewModel.PhaseData));
            AddControlToGrid(firstGrid, firstInfo.GetContent, 0, 2);
            _thisScore.AddColumn("Cards", false, nameof(ChinazoPlayerItem.ObjectCount)); //very common.
            _thisScore.AddColumn("C Score", false, nameof(ChinazoPlayerItem.CurrentScore), rightMargin: 5);
            _thisScore.AddColumn("T Score", false, nameof(ChinazoPlayerItem.TotalScore));
            AddControlToGrid(firstGrid, _thisScore, 0, 3);
            AddControlToGrid(finalGrid, firstGrid, 1, 0);
            StackLayout thirdStack = new StackLayout();
            thirdStack.Orientation = StackOrientation.Horizontal;
            thirdStack.Children.Add(_tempG);
            thirdStack.Children.Add(_mainG);
            AddControlToGrid(finalGrid, thirdStack, 2, 0);
            MainGrid!.Children.Add(finalGrid);
            _deckGPile.Margin = new Thickness(5, 5, 5, 5);
            _discardGPile.Margin = new Thickness(5, 5, 5, 5);
            AddRestoreCommand(thisStack); //i think.  if a stack can't be used, rethink.
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterType<BasicGameLoader<ChinazoPlayerItem, ChinazoSaveInfo>>(); //i think basic game loader gets done here still.
            OurContainer!.RegisterCommonRegularCards<ChinazoViewModel, ChinazoCard>(registerCommonProportions: false, customDeck: true);
            OurContainer.RegisterSingleton<IDeckCount, CustomDeck>(); //forgot to use a custom deck for this one.
            OurContainer.RegisterSingleton<IRegularAceCalculator, RegularAceHighCalculator>();
            OurContainer.RegisterSingleton<IProportionImage, SmallDrawableProportion>(ts.TagUsed);
        }
    }
}