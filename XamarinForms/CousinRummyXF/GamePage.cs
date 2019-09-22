using BasicGameFramework.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.Interfaces;
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
using CousinRummyCP;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
using static BasicXFControlsAndPages.Helpers.GridHelper; //just in case
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace CousinRummyXF
{
    public class GamePage : MultiPlayerPage<CousinRummyViewModel, CousinRummyPlayerItem, CousinRummySaveInfo>
    {
        public GamePage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode) { }

        public override Task HandleAsync(LoadEventModel message)
        {
            CousinRummySaveInfo saveRoot = OurContainer!.Resolve<CousinRummySaveInfo>();
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
            CousinRummySaveInfo saveRoot = OurContainer!.Resolve<CousinRummySaveInfo>();
            _thisScore!.UpdateLists(saveRoot.PlayerList);
            _playerHand!.UpdateList(ThisMod!.PlayerHand1!);
            _deckGPile!.UpdateDeck(ThisMod.Deck1!);
            _discardGPile!.UpdatePile(ThisMod.Pile1!);
            _tempG!.Update(ThisMod.TempSets!);
            _mainG!.Update(ThisMod.MainSets!);
            return Task.CompletedTask;
        }
        private BaseDeckXF<RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>>? _deckGPile;
        private BasePileXF<RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>>? _discardGPile;
        private ScoreBoardXF? _thisScore;
        private BaseHandXF<RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>>? _playerHand;
        private TempRummySetsXF<EnumSuitList, EnumColorList, RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>>? _tempG;
        private MainRummySetsXF<EnumSuitList, EnumColorList, RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>, PhaseSet, SavedSet>? _mainG;
        protected override async Task AfterGameButtonAsync()
        {
            StackLayout thisStack = new StackLayout(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            _deckGPile = new BaseDeckXF<RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>>();
            _discardGPile = new BasePileXF<RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>>();
            _thisScore = new ScoreBoardXF();
            _playerHand = new BaseHandXF<RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>>();
            _tempG = new TempRummySetsXF<EnumSuitList, EnumColorList, RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>>();
            _mainG = new MainRummySetsXF<EnumSuitList, EnumColorList, RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>, PhaseSet, SavedSet>();
            GameButton!.HorizontalOptions = LayoutOptions.Center;
            GameButton.VerticalOptions = LayoutOptions.Center;
            RoundButton!.HorizontalOptions = LayoutOptions.Center;
            RoundButton.VerticalOptions = LayoutOptions.Center;
            thisStack.Children.Add(GameButton);
            thisStack.Children.Add(RoundButton);
            Grid buyGrid = new Grid();
            AddAutoColumns(buyGrid, 2);
            AddAutoRows(buyGrid, 1);
            AddPixelRow(buyGrid, 100);
            Button thisBut;
            thisBut = GetSmallerButton("Pass", nameof(CousinRummyViewModel.PassCommand));
            AddControlToGrid(buyGrid, thisBut, 0, 0);
            thisBut = GetSmallerButton("Buy", nameof(CousinRummyViewModel.BuyCommand));
            AddControlToGrid(buyGrid, thisBut, 0, 1);
            AddControlToGrid(buyGrid, _deckGPile, 1, 0);
            AddControlToGrid(buyGrid, _discardGPile, 1, 1);
            Grid gameGrid = new Grid();
            AddLeftOverRow(gameGrid, 45); // try that
            AddAutoRows(gameGrid, 1);
            AddLeftOverRow(gameGrid, 30);
            var otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_tempG);
            AddControlToGrid(gameGrid, otherStack, 0, 0);
            _thisScore = new ScoreBoardXF();
            _thisScore.AddColumn("Cards", false, nameof(CousinRummyPlayerItem.ObjectCount)); //very common.
            _thisScore.AddColumn("Tokens", false, nameof(CousinRummyPlayerItem.TokensLeft));
            _thisScore.AddColumn("C Score", false, nameof(CousinRummyPlayerItem.CurrentScore), rightMargin: 5);
            _thisScore.AddColumn("T Score", false, nameof(CousinRummyPlayerItem.TotalScore));
            otherStack.Children.Add(_thisScore); //may require experimenting for rotated as well (?)
            Grid bottomGrid = new Grid();
            AddLeftOverColumn(bottomGrid, 30);
            AddLeftOverColumn(bottomGrid, 70);
            otherStack = new StackLayout();
            thisBut = GetSmallerButton("Lay Down Initial Sets", nameof(CousinRummyViewModel.InitSetsCommand));
            otherStack.Children.Add(thisBut);
            thisBut = GetSmallerButton("Lay Down Other Sets", nameof(CousinRummyViewModel.OtherSetsCommand)); // i think its othersets commands (?)
            otherStack.Children.Add(thisBut);
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Normal Turn", nameof(CousinRummyViewModel.NormalTurn));
            firstInfo.AddRow("Other Turn", nameof(CousinRummyViewModel.OtherLabel));
            firstInfo.AddRow("Phase", nameof(CousinRummyViewModel.PhaseData));
            AddControlToGrid(bottomGrid, otherStack, 0, 0);
            AddControlToGrid(bottomGrid, _mainG, 0, 1);
            AddControlToGrid(gameGrid, bottomGrid, 2, 0);
            otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            _playerHand.HorizontalOptions = LayoutOptions.StartAndExpand;
            otherStack.Children.Add(buyGrid);
            StackLayout tempStack = new StackLayout();
            tempStack.Children.Add(_playerHand);
            tempStack.Children.Add(firstInfo.GetContent);
            otherStack.Children.Add(tempStack);
            AddControlToGrid(gameGrid, otherStack, 1, 0);
            thisStack.Children.Add(gameGrid);
            MainGrid!.Children.Add(thisStack);
            _deckGPile.Margin = new Thickness(5, 5, 5, 5);
            _discardGPile.Margin = new Thickness(5, 5, 5, 5);
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterType<BasicGameLoader<CousinRummyPlayerItem, CousinRummySaveInfo>>(); //i think basic game loader gets done here still.
            OurContainer!.RegisterCommonRegularCards<CousinRummyViewModel, RegularRummyCard>(registerCommonProportions: false, customDeck: true);
            OurContainer.RegisterSingleton<IDeckCount, CustomDeck>(); //forgot to use a custom deck for this one.
            OurContainer.RegisterSingleton<IRegularAceCalculator, RegularAceHighCalculator>();
            OurContainer.RegisterSingleton<IProportionImage, SmallDrawableProportion>(ts.TagUsed);
        }
    }
}