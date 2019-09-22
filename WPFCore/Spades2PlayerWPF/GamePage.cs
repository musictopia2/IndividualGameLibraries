using BasicGameFramework.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BaseGPXWindowsAndControlsCore.BaseWindows;
using BaseGPXWindowsAndControlsCore.BasicControls.ChoicePickers;
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
using Spades2PlayerCP;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace Spades2PlayerWPF
{
    public class GamePage : MultiPlayerWindow<Spades2PlayerViewModel, Spades2PlayerPlayerItem, Spades2PlayerSaveInfo>
    {
        public GamePage(IStartUp starts, EnumGamePackageMode mode) //this means something needs to put into here.
        {
            BuildXAML(starts, mode);
        }
        public override Task HandleAsync(LoadEventModel message)
        {
            Spades2PlayerSaveInfo saveRoot = OurContainer!.Resolve<Spades2PlayerSaveInfo>();
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _playerHandWPF!.LoadList(ThisMod!.PlayerHand1!, ts.TagUsed); // i think
            _discardGPile!.Init(ThisMod.Pile1!, ts.TagUsed); // may have to be here (well see)
            _discardGPile.StartListeningDiscardPile(); // its the main one.
            _trick1!.Init(_mainGame!.TrickArea1!, ts.TagUsed);
            _deckGPile!.Init(ThisMod.Deck1!, ts.TagUsed); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();
            _pile2!.Init(ThisMod.Pile2!, ts.TagUsed);
            _pile2.StartAnimationListener("otherpile");
            _bid1!.LoadLists(ThisMod);
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            Spades2PlayerSaveInfo saveRoot = OurContainer!.Resolve<Spades2PlayerSaveInfo>();
            _thisScore!.UpdateLists(saveRoot.PlayerList);
            _playerHandWPF!.UpdateList(ThisMod!.PlayerHand1!);
            _deckGPile!.UpdateDeck(ThisMod.Deck1!);
            _discardGPile!.UpdatePile(ThisMod.Pile1!);
            _trick1!.Update(_mainGame!.TrickArea1!);
            _pile2!.UpdatePile(ThisMod.Pile2!);
            return Task.CompletedTask;
        }
        private BaseDeckWPF<Spades2PlayerCardInformation, ts, DeckOfCardsWPF<Spades2PlayerCardInformation>>? _deckGPile;
        private BasePileWPF<Spades2PlayerCardInformation, ts, DeckOfCardsWPF<Spades2PlayerCardInformation>>? _discardGPile;
        private ScoreBoardWPF? _thisScore;
        private BaseHandWPF<Spades2PlayerCardInformation, ts, DeckOfCardsWPF<Spades2PlayerCardInformation>>? _playerHandWPF;
        private TwoPlayerTrickWPF<EnumSuitList, Spades2PlayerCardInformation, ts, DeckOfCardsWPF<Spades2PlayerCardInformation>>? _trick1;
        private Spades2PlayerMainGameClass? _mainGame;
        private BasePileWPF<Spades2PlayerCardInformation, ts, DeckOfCardsWPF<Spades2PlayerCardInformation>>? _pile2;
        private BidControl? _bid1;
        protected async override void AfterGameButton()
        {
            _mainGame = OurContainer!.Resolve<Spades2PlayerMainGameClass>();
            StackPanel thisStack = new StackPanel(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            _deckGPile = new BaseDeckWPF<Spades2PlayerCardInformation, ts, DeckOfCardsWPF<Spades2PlayerCardInformation>>();
            _discardGPile = new BasePileWPF<Spades2PlayerCardInformation, ts, DeckOfCardsWPF<Spades2PlayerCardInformation>>();
            _thisScore = new ScoreBoardWPF();
            _playerHandWPF = new BaseHandWPF<Spades2PlayerCardInformation, ts, DeckOfCardsWPF<Spades2PlayerCardInformation>>();
            _playerHandWPF.Divider = 1.3;
            _trick1 = new TwoPlayerTrickWPF<EnumSuitList, Spades2PlayerCardInformation, ts, DeckOfCardsWPF<Spades2PlayerCardInformation>>();
            _pile2 = new BasePileWPF<Spades2PlayerCardInformation, ts, DeckOfCardsWPF<Spades2PlayerCardInformation>>();
            _bid1 = new BidControl();
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
            var thisBut = GetGamingButton("Take Card", nameof(Spades2PlayerViewModel.TakeCardCommand));
            otherStack.Children.Add(thisBut);
            otherStack.Children.Add(_pile2);
            thisStack.Children.Add(otherStack);
            Binding thisBind = GetVisibleBinding(nameof(Spades2PlayerViewModel.BeginningVisible));
            otherStack.SetBinding(VisibilityProperty, thisBind);
            thisStack.Children.Add(_trick1);
            thisStack.Children.Add(_bid1);
            _thisScore = new ScoreBoardWPF();
            _thisScore.AddColumn("Cards Left", false, nameof(Spades2PlayerPlayerItem.ObjectCount)); //very common.
            _thisScore.AddColumn("# Bidded", false, nameof(Spades2PlayerPlayerItem.HowManyBids));
            _thisScore.AddColumn("Tricks Won", false, nameof(Spades2PlayerPlayerItem.TricksWon));
            _thisScore.AddColumn("Bags", false, nameof(Spades2PlayerPlayerItem.Bags));
            _thisScore.AddColumn("Current Score", false, nameof(Spades2PlayerPlayerItem.CurrentScore));
            _thisScore.AddColumn("Total Score", false, nameof(Spades2PlayerPlayerItem.TotalScore));
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(Spades2PlayerViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(Spades2PlayerViewModel.Status));
            firstInfo.AddRow("Round", nameof(Spades2PlayerViewModel.RoundNumber));
            MainGrid!.Children.Add(thisStack);
            thisStack.Children.Add(_playerHandWPF);
            thisStack.Children.Add(firstInfo.GetContent);
            otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            thisStack.Children.Add(otherStack);
            otherStack.Children.Add(_thisScore);
            _deckGPile.Margin = new Thickness(5, 5, 5, 5);
            _discardGPile.Margin = new Thickness(5, 5, 5, 5);
            AddRestoreCommand(thisStack); //i think.  if a stack can't be used, rethink.
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterNonSavedClasses<Spades2PlayerViewModel>();
            OurContainer!.RegisterType<DeckViewModel<Spades2PlayerCardInformation>>(true);
            OurContainer.RegisterType<BasicGameLoader<Spades2PlayerPlayerItem, Spades2PlayerSaveInfo>>();
            OurContainer.RegisterType<RegularCardsBasicShuffler<Spades2PlayerCardInformation>>();
            OurContainer.RegisterSingleton<IRegularAceCalculator, RegularAceHighCalculator>(); //most of the time, ace is high for trick taking games.
            bool rets = OurContainer.ObjectExist<ISortCategory>();
            if (rets == true)
            {
                ISortCategory ThisCat = OurContainer.Resolve<ISortCategory>();
                SortSimpleCards<Spades2PlayerCardInformation> ThisSort = new SortSimpleCards<Spades2PlayerCardInformation>();
                ThisSort.SuitForSorting = ThisCat.SortCategory;
                OurContainer.RegisterSingleton(ThisSort); //if we have a custom one, will already be picked up.
            }
            OurContainer.RegisterSingleton<IDeckCount, RegularAceHighSimpleDeck>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(ts.TagUsed);
            OurContainer.RegisterType<TwoPlayerTrickViewModel<EnumSuitList, Spades2PlayerCardInformation, Spades2PlayerPlayerItem, Spades2PlayerSaveInfo>>();
            OurContainer.RegisterType<StandardWidthHeight>();
        }
    }
}