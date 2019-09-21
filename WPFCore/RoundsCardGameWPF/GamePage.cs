using AndyCristinaGamePackageCP.CommonProportionClasses;
using AndyCristinaGamePackageCP.ExtensionClasses;
using BaseGPXWindowsAndControlsCore.BaseWindows;
using BaseGPXWindowsAndControlsCore.BasicControls.GameFrames;
using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using BaseGPXWindowsAndControlsCore.BasicControls.SingleCardFrames;
using BaseGPXWindowsAndControlsCore.BasicControls.TrickUIs;
using BaseGPXWindowsAndControlsCore.GameGraphics.Cards;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using BasicGameFramework.RegularDeckOfCards;
using BasicGameFramework.SpecializedGameTypes.TrickClasses;
using RoundsCardGameCP;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace RoundsCardGameWPF
{
    public class GamePage : MultiPlayerWindow<RoundsCardGameViewModel, RoundsCardGamePlayerItem, RoundsCardGameSaveInfo>
    {
        public GamePage(IStartUp starts, EnumGamePackageMode mode) //this means something needs to put into here.
        {
            BuildXAML(starts, mode);
        }
        public override Task HandleAsync(LoadEventModel message)
        {
            RoundsCardGameSaveInfo saveRoot = OurContainer!.Resolve<RoundsCardGameSaveInfo>();
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _playerHandWPF!.LoadList(ThisMod!.PlayerHand1!, ts.TagUsed); // i think
            _discardGPile!.Init(ThisMod.Pile1!, ts.TagUsed); // may have to be here (well see)
            _discardGPile.StartListeningDiscardPile(); // its the main one.
            _trick1!.Init(_mainGame!.TrickArea1!, ts.TagUsed);
            _deckGPile!.Init(ThisMod.Deck1!, ts.TagUsed); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            RoundsCardGameSaveInfo saveRoot = OurContainer!.Resolve<RoundsCardGameSaveInfo>();
            _thisScore!.UpdateLists(saveRoot.PlayerList);
            _playerHandWPF!.UpdateList(ThisMod!.PlayerHand1!);
            _deckGPile!.UpdateDeck(ThisMod.Deck1!);
            _discardGPile!.UpdatePile(ThisMod.Pile1!);
            _trick1!.Update(_mainGame!.TrickArea1!);
            return Task.CompletedTask;
        }
        private BaseDeckWPF<RoundsCardGameCardInformation, ts, DeckOfCardsWPF<RoundsCardGameCardInformation>>? _deckGPile;
        private BasePileWPF<RoundsCardGameCardInformation, ts, DeckOfCardsWPF<RoundsCardGameCardInformation>>? _discardGPile;
        private ScoreBoardWPF? _thisScore;
        private BaseHandWPF<RoundsCardGameCardInformation, ts, DeckOfCardsWPF<RoundsCardGameCardInformation>>? _playerHandWPF;
        private TwoPlayerTrickWPF<EnumSuitList, RoundsCardGameCardInformation, ts, DeckOfCardsWPF<RoundsCardGameCardInformation>>? _trick1;
        private RoundsCardGameMainGameClass? _mainGame;
        protected async override void AfterGameButton()
        {
            _mainGame = OurContainer!.Resolve<RoundsCardGameMainGameClass>();
            StackPanel thisStack = new StackPanel(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            _deckGPile = new BaseDeckWPF<RoundsCardGameCardInformation, ts, DeckOfCardsWPF<RoundsCardGameCardInformation>>();
            _discardGPile = new BasePileWPF<RoundsCardGameCardInformation, ts, DeckOfCardsWPF<RoundsCardGameCardInformation>>();
            _thisScore = new ScoreBoardWPF();
            _playerHandWPF = new BaseHandWPF<RoundsCardGameCardInformation, ts, DeckOfCardsWPF<RoundsCardGameCardInformation>>();
            _trick1 = new TwoPlayerTrickWPF<EnumSuitList, RoundsCardGameCardInformation, ts, DeckOfCardsWPF<RoundsCardGameCardInformation>>();
            GameButton!.HorizontalAlignment = HorizontalAlignment.Center;
            GameButton.VerticalAlignment = VerticalAlignment.Center;
            RoundButton!.HorizontalAlignment = HorizontalAlignment.Center;
            RoundButton.VerticalAlignment = VerticalAlignment.Center;
            thisStack.Children.Add(GameButton);
            thisStack.Children.Add(RoundButton); //most has rounds.
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            thisStack.Children.Add(otherStack);
            _thisScore = new ScoreBoardWPF();
            _thisScore.AddColumn("# In Hand", true, nameof(RoundsCardGamePlayerItem.ObjectCount));
            _thisScore.AddColumn("Tricks Won", true, nameof(RoundsCardGamePlayerItem.TricksWon));
            _thisScore.AddColumn("Rounds Won", true, nameof(RoundsCardGamePlayerItem.RoundsWon));
            _thisScore.AddColumn("Points", true, nameof(RoundsCardGamePlayerItem.CurrentPoints));
            _thisScore.AddColumn("Total Score", true, nameof(RoundsCardGamePlayerItem.TotalScore));
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(RoundsCardGameViewModel.NormalTurn));
            firstInfo.AddRow("Trump", nameof(RoundsCardGameViewModel.TrumpSuit)); //if trump is not needed, then comment.
            firstInfo.AddRow("Status", nameof(RoundsCardGameViewModel.Status));
            thisStack.Children.Add(_trick1);
            MainGrid!.Children.Add(thisStack);
            thisStack.Children.Add(_playerHandWPF);
            thisStack.Children.Add(firstInfo.GetContent);
            otherStack.Children.Add(_thisScore);
            _deckGPile.Margin = new Thickness(5, 5, 5, 5);
            _discardGPile.Margin = new Thickness(5, 5, 5, 5);
            AddRestoreCommand(thisStack); //i think.  if a stack can't be used, rethink.
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterNonSavedClasses<RoundsCardGameViewModel>();
            OurContainer!.RegisterType<DeckViewModel<RoundsCardGameCardInformation>>(true);
            OurContainer.RegisterType<BasicGameLoader<RoundsCardGamePlayerItem, RoundsCardGameSaveInfo>>();
            OurContainer.RegisterType<RegularCardsBasicShuffler<RoundsCardGameCardInformation>>();
            OurContainer.RegisterSingleton<IRegularAceCalculator, RegularAceHighCalculator>(); //most of the time, ace is high for trick taking games.
            bool rets = OurContainer.ObjectExist<ISortCategory>();
            if (rets == true)
            {
                ISortCategory ThisCat = OurContainer.Resolve<ISortCategory>();
                SortSimpleCards<RoundsCardGameCardInformation> ThisSort = new SortSimpleCards<RoundsCardGameCardInformation>();
                ThisSort.SuitForSorting = ThisCat.SortCategory;
                OurContainer.RegisterSingleton(ThisSort); //if we have a custom one, will already be picked up.
            }
            OurContainer.RegisterSingleton<IDeckCount, RegularAceHighSimpleDeck>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(ts.TagUsed);
            OurContainer.RegisterType<TwoPlayerTrickViewModel<EnumSuitList, RoundsCardGameCardInformation, RoundsCardGamePlayerItem, RoundsCardGameSaveInfo>>();
        }
    }
}