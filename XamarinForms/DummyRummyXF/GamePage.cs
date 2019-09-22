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
using DummyRummyCP;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
using static BasicXFControlsAndPages.Helpers.GridHelper; //just in case
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace DummyRummyXF
{
    public class GamePage : MultiPlayerPage<DummyRummyViewModel, DummyRummyPlayerItem, DummyRummySaveInfo>
    {
        public GamePage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode) { }
        public override Task HandleAsync(LoadEventModel message)
        {
            DummyRummySaveInfo saveRoot = OurContainer!.Resolve<DummyRummySaveInfo>();
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _playerHand!.LoadList(ThisMod!.PlayerHand1!, ts.TagUsed); // i think
            _discardGPile!.Init(ThisMod.Pile1!, ts.TagUsed); // may have to be here (well see)
            _discardGPile.StartListeningDiscardPile(); // its the main one.
            _deckGPile!.Init(ThisMod.Deck1!, ts.TagUsed); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();
            _tempG!.Init(ThisMod!.TempSets!, ts.TagUsed);
            _mainG!.Init(ThisMod!.MainSets!, ts.TagUsed);
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            DummyRummySaveInfo saveRoot = OurContainer!.Resolve<DummyRummySaveInfo>();
            _thisScore!.UpdateLists(saveRoot.PlayerList);
            _playerHand!.UpdateList(ThisMod!.PlayerHand1!);
            _deckGPile!.UpdateDeck(ThisMod.Deck1!);
            _discardGPile!.UpdatePile(ThisMod.Pile1!);
            _tempG!.Update(ThisMod!.TempSets!);
            _mainG!.Update(ThisMod!.MainSets!);
            return Task.CompletedTask;
        }
        private BaseDeckXF<RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>>? _deckGPile;
        private BasePileXF<RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>>? _discardGPile;
        private ScoreBoardXF? _thisScore;
        private BaseHandXF<RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>>? _playerHand;
        private TempRummySetsXF<EnumSuitList, EnumColorList, RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>>? _tempG;
        private MainRummySetsXF<EnumSuitList, EnumColorList, RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>, DummySet, SavedSet>? _mainG;
        protected override async Task AfterGameButtonAsync()
        {
            BasicSetUp();
            _deckGPile = new BaseDeckXF<RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>>();
            _discardGPile = new BasePileXF<RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>>();
            _thisScore = new ScoreBoardXF();
            _playerHand = new BaseHandXF<RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>>();
            _tempG = new TempRummySetsXF<EnumSuitList, EnumColorList, RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>>();
            _mainG = new MainRummySetsXF<EnumSuitList, EnumColorList, RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>, DummySet, SavedSet>();
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
            StackLayout secondStack = new StackLayout();
            secondStack.Orientation = StackOrientation.Horizontal;
            firstStack.Children.Add(secondStack);
            var thisBut = GetSmallerButton("Lay Down", nameof(DummyRummyViewModel.LayDownSetsCommand));
            firstStack.Children.Add(thisBut);
            thisBut = GetSmallerButton("Back", nameof(DummyRummyViewModel.BackCommand));
            firstStack.Children.Add(thisBut);
            AddControlToGrid(firstGrid, firstStack, 0, 0);
            _thisScore.AddColumn("Cards Left", true, nameof(DummyRummyPlayerItem.ObjectCount)); //very common.
            _thisScore.AddColumn("Current Score", true, nameof(DummyRummyPlayerItem.CurrentScore));
            _thisScore.AddColumn("Total Score", true, nameof(DummyRummyPlayerItem.TotalScore));
            AddControlToGrid(firstGrid, _thisScore, 0, 3);
            AddControlToGrid(finalGrid, firstGrid, 1, 0);
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(DummyRummyViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(DummyRummyViewModel.Status));
            firstInfo.AddRow("Up To", nameof(DummyRummyViewModel.UpTo));
            AddControlToGrid(firstGrid, firstInfo.GetContent, 0, 2);
            _tempG.Divider = 1.1;
            StackLayout thirdStack = new StackLayout();
            thirdStack.Orientation = StackOrientation.Horizontal;
            thirdStack.Children.Add(_tempG);
            thirdStack.Children.Add(_mainG);
            AddControlToGrid(finalGrid, thirdStack, 2, 0); // i think
            MainGrid!.Children.Add(finalGrid);
            _deckGPile.Margin = new Thickness(5, 5, 5, 5);
            _discardGPile.Margin = new Thickness(5, 5, 5, 5);
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterType<BasicGameLoader<DummyRummyPlayerItem, DummyRummySaveInfo>>(); //i think basic game loader gets done here still.
            OurContainer!.RegisterCommonRegularCards<DummyRummyViewModel, RegularRummyCard>(registerCommonProportions: false);
            OurContainer.RegisterSingleton<IProportionImage, SmallDrawableProportion>(ts.TagUsed);
        }
    }
}