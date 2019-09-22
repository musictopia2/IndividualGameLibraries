using BasicGameFramework.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BaseGPXWindowsAndControlsCore.BaseWindows;
using BaseGPXWindowsAndControlsCore.BasicControls.GameFrames;
using BaseGPXWindowsAndControlsCore.BasicControls.Misc;
using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using BaseGPXWindowsAndControlsCore.BasicControls.SingleCardFrames;
using BaseGPXWindowsAndControlsCore.GameGraphics.Cards;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using BasicGameFramework.RegularDeckOfCards;
using OpetongCP;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace OpetongWPF
{
    public class GamePage : MultiPlayerWindow<OpetongViewModel, OpetongPlayerItem, OpetongSaveInfo>
    {
        public GamePage(IStartUp starts, EnumGamePackageMode mode) //this means something needs to put into here.
        {
            BuildXAML(starts, mode);
        }
        public override Task HandleAsync(LoadEventModel message)
        {
            OpetongSaveInfo saveRoot = OurContainer!.Resolve<OpetongSaveInfo>();
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _playerHandWPF!.LoadList(ThisMod!.PlayerHand1!, ts.TagUsed); // i think
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
            _playerHandWPF!.UpdateList(ThisMod!.PlayerHand1!);
            _deckGPile!.UpdateDeck(ThisMod.Deck1!);
            _tempG!.Update(ThisMod.TempSets!);
            _mainG!.Update(ThisMod.MainSets!);
            _poolG!.UpdateList(ThisMod.Pool1!);
            return Task.CompletedTask;
        }
        private BaseDeckWPF<RegularRummyCard, ts, DeckOfCardsWPF<RegularRummyCard>>? _deckGPile;
        private ScoreBoardWPF? _thisScore;
        private BaseHandWPF<RegularRummyCard, ts, DeckOfCardsWPF<RegularRummyCard>>? _playerHandWPF;
        private TempRummySetsWPF<EnumSuitList, EnumColorList, RegularRummyCard, ts, DeckOfCardsWPF<RegularRummyCard>>? _tempG;
        private MainRummySetsWPF<EnumSuitList, EnumColorList, RegularRummyCard, ts, DeckOfCardsWPF<RegularRummyCard>, RummySet, SavedSet>? _mainG;
        private CardBoardWPF<RegularRummyCard, ts, DeckOfCardsWPF<RegularRummyCard>>? _poolG;
        protected async override void AfterGameButton()
        {
            StackPanel thisStack = new StackPanel(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            _deckGPile = new BaseDeckWPF<RegularRummyCard, ts, DeckOfCardsWPF<RegularRummyCard>>();
            _thisScore = new ScoreBoardWPF();
            _playerHandWPF = new BaseHandWPF<RegularRummyCard, ts, DeckOfCardsWPF<RegularRummyCard>>();
            _tempG = new TempRummySetsWPF<EnumSuitList, EnumColorList, RegularRummyCard, ts, DeckOfCardsWPF<RegularRummyCard>>();
            _mainG = new MainRummySetsWPF<EnumSuitList, EnumColorList, RegularRummyCard, ts, DeckOfCardsWPF<RegularRummyCard>, RummySet, SavedSet>();
            _poolG = new CardBoardWPF<RegularRummyCard, ts, DeckOfCardsWPF<RegularRummyCard>>();
            GameButton!.HorizontalAlignment = HorizontalAlignment.Center;
            GameButton.VerticalAlignment = VerticalAlignment.Center;
            thisStack.Children.Add(GameButton);
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            _deckGPile.HorizontalAlignment = HorizontalAlignment.Left;
            _deckGPile.VerticalAlignment = VerticalAlignment.Top;
            otherStack.Children.Add(_deckGPile);
            _poolG.HorizontalAlignment = HorizontalAlignment.Left;
            _poolG.VerticalAlignment = VerticalAlignment.Top;
            otherStack.Children.Add(_poolG);
            _tempG.Height = 350;
            otherStack.Children.Add(_tempG);
            var thisBut = GetGamingButton("Lay Down Single Set", nameof(OpetongViewModel.PlaySetCommand));
            otherStack.Children.Add(thisBut);
            _thisScore = new ScoreBoardWPF();
            _thisScore.AddColumn("Cards Left", true, nameof(OpetongPlayerItem.ObjectCount));
            _thisScore.AddColumn("Sets Played", true, nameof(OpetongPlayerItem.SetsPlayed));
            _thisScore.AddColumn("Score", true, nameof(OpetongPlayerItem.TotalScore));
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(OpetongViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(OpetongViewModel.Status));
            firstInfo.AddRow("Instructions", nameof(OpetongViewModel.Instructions));
            StackPanel finalStack = new StackPanel();
            finalStack.Children.Add(_thisScore);
            finalStack.Children.Add(firstInfo.GetContent);
            otherStack.Children.Add(finalStack);
            thisStack.Children.Add(otherStack);
            thisStack.Children.Add(_playerHandWPF);
            thisStack.Children.Add(_mainG);
            MainGrid!.Children.Add(thisStack);
            _deckGPile.Margin = new Thickness(5, 5, 5, 5);
            AddRestoreCommand(thisStack); //i think.  if a stack can't be used, rethink.
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