using AndyCristinaGamePackageCP.CommonProportionClasses;
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
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using BasicGameFramework.RegularDeckOfCards;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using OpetongCP;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
using static BasicXFControlsAndPages.Helpers.GridHelper; //just in case
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace OpetongXF
{
    public class GamePage : MultiPlayerPage<OpetongViewModel, OpetongPlayerItem, OpetongSaveInfo>
    {
        public GamePage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode) { }

        public override Task HandleAsync(LoadEventModel message)
        {
            OpetongSaveInfo saveRoot = OurContainer!.Resolve<OpetongSaveInfo>();
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _playerHand!.LoadList(ThisMod!.PlayerHand1!, ts.TagUsed); // i think
            _deckGPile!.Init(ThisMod.Deck1!, ts.TagUsed); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();
            _tempG!.Init(ThisMod.TempSets!, ts.TagUsed);
            _mainG!.Init(ThisMod.MainSets!, ts.TagUsed);
            _poolG!.LoadList(ThisMod.Pool1!, ts.TagUsed);
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            OpetongSaveInfo saveRoot = OurContainer!.Resolve<OpetongSaveInfo>();
            _thisScore!.UpdateLists(saveRoot.PlayerList);
            _playerHand!.UpdateList(ThisMod!.PlayerHand1!);
            _deckGPile!.UpdateDeck(ThisMod.Deck1!);
            _tempG!.Update(ThisMod.TempSets!);
            _mainG!.Update(ThisMod.MainSets!);
            _poolG!.UpdateList(ThisMod.Pool1!);
            return Task.CompletedTask;
        }
        private BaseDeckXF<RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>>? _deckGPile;
        private ScoreBoardXF? _thisScore;
        private BaseHandXF<RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>>? _playerHand;
        private TempRummySetsXF<EnumSuitList, EnumColorList, RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>>? _tempG;
        private MainRummySetsXF<EnumSuitList, EnumColorList, RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>, RummySet, SavedSet>? _mainG;
        private CardBoardXF<RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>>? _poolG;
        protected override async Task AfterGameButtonAsync()
        {
            BasicSetUp();
            //anything needed for ui is here.
            _deckGPile = new BaseDeckXF<RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>>();
            _thisScore = new ScoreBoardXF();
            _playerHand = new BaseHandXF<RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>>();
            _poolG = new CardBoardXF<RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>>();
            _tempG = new TempRummySetsXF<EnumSuitList, EnumColorList, RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>>();
            _mainG = new MainRummySetsXF<EnumSuitList, EnumColorList, RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>, RummySet, SavedSet>();
            Grid finalGrid = new Grid();
            AddAutoRows(finalGrid, 1);
            AddLeftOverRow(finalGrid, 50);
            AddAutoRows(finalGrid, 1);
            AddLeftOverRow(finalGrid, 50);
            GameButton!.HorizontalOptions = LayoutOptions.Center;
            GameButton.VerticalOptions = LayoutOptions.Center;
            AddControlToGrid(finalGrid, GameButton, 0, 0);
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_poolG);
            otherStack.Children.Add(_tempG);
            Button button = GetGamingButton($"Lay Down{Constants.vbCrLf}Single Set", nameof(OpetongViewModel.PlaySetCommand));
            otherStack.Children.Add(button);
            StackLayout tempStack = new StackLayout();
            _thisScore = new ScoreBoardXF();
            _thisScore.AddColumn("Cards Left", true, nameof(OpetongPlayerItem.ObjectCount));
            _thisScore.AddColumn("Sets Played", true, nameof(OpetongPlayerItem.SetsPlayed));
            _thisScore.AddColumn("Score", true, nameof(OpetongPlayerItem.TotalScore));
            tempStack.Children.Add(_thisScore);
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(OpetongViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(OpetongViewModel.Status));
            firstInfo.AddRow("Instructions", nameof(OpetongViewModel.Instructions));
            tempStack.Children.Add(firstInfo.GetContent);
            otherStack.Children.Add(tempStack);
            AddControlToGrid(finalGrid, otherStack, 1, 0);
            AddControlToGrid(finalGrid, _playerHand, 2, 0);
            AddControlToGrid(finalGrid, _mainG, 3, 0);
            MainGrid!.Children.Add(finalGrid);
            _deckGPile.Margin = new Thickness(5, 5, 5, 5);
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterType<BasicGameLoader<OpetongPlayerItem, OpetongSaveInfo>>(); //i think basic game loader gets done here still.
            OurContainer!.RegisterNonSavedClasses<OpetongViewModel>();
            OurContainer.RegisterSingleton<IProportionImage, SmallDrawableProportion>(ts.TagUsed);
            OurContainer.RegisterType<RegularCardsBasicShuffler<RegularRummyCard>>(true);
            OurContainer.RegisterType<DeckViewModel<RegularRummyCard>>(true); //i think
            bool rets = OurContainer.ObjectExist<ISortCategory>();
            if (rets == true)
            {
                ISortCategory ThisCat = OurContainer.Resolve<ISortCategory>();
                SortSimpleCards<RegularRummyCard> ThisSort = new SortSimpleCards<RegularRummyCard>();
                ThisSort.SuitForSorting = ThisCat.SortCategory;
                OurContainer.RegisterSingleton(ThisSort); //if we have a custom one, will already be picked up.
            }
            OurContainer.RegisterSingleton<IDeckCount, RegularAceLowSimpleDeck>();
            OurContainer.RegisterSingleton<IRegularAceCalculator, RegularLowAceCalculator>();
        }
    }
}