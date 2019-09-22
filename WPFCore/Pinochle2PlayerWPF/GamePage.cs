using BasicGameFramework.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BaseGPXWindowsAndControlsCore.BaseWindows;
using BaseGPXWindowsAndControlsCore.BasicControls.GameFrames;
using BaseGPXWindowsAndControlsCore.BasicControls.Misc;
using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using BaseGPXWindowsAndControlsCore.BasicControls.SingleCardFrames;
using BaseGPXWindowsAndControlsCore.BasicControls.TrickUIs;
using BaseGPXWindowsAndControlsCore.GameGraphics.Cards;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.Extensions;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using BasicGameFramework.RegularDeckOfCards;
using BasicGameFramework.SpecializedGameTypes.TrickClasses;
using Pinochle2PlayerCP;
using SkiaSharp;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //just in case
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace Pinochle2PlayerWPF
{
    public class GamePage : MultiPlayerWindow<Pinochle2PlayerViewModel, Pinochle2PlayerPlayerItem, Pinochle2PlayerSaveInfo>
    {
        public GamePage(IStartUp starts, EnumGamePackageMode mode) //this means something needs to put into here.
        {
            BuildXAML(starts, mode);
        }
        public override Task HandleAsync(LoadEventModel message)
        {
            Pinochle2PlayerSaveInfo saveRoot = OurContainer!.Resolve<Pinochle2PlayerSaveInfo>();
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _playerHandWPF!.LoadList(ThisMod!.PlayerHand1!, ts.TagUsed); // i think
            _discardGPile!.Init(ThisMod.Pile1!, ts.TagUsed); // may have to be here (well see)
            _discardGPile.StartListeningDiscardPile(); // its the main one.
            _trick1!.Init(_mainGame!.TrickArea1!, ts.TagUsed);
            _deckGPile!.Init(ThisMod.Deck1!, ts.TagUsed); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();
            _tempG!.Init(ThisMod.TempSets!, ts.TagUsed);
            _yourMeld!.LoadList(ThisMod.YourMelds!, ts.TagUsed);
            _opponentMeld!.LoadList(ThisMod.OpponentMelds!, ts.TagUsed);
            var thisCard = new Pinochle2PlayerCardInformation();
            IProportionImage thisP = OurContainer.Resolve<IProportionImage>(ts.TagUsed);
            SKSize thisSize = thisCard.DefaultSize.GetSizeUsed(thisP.Proportion);
            var heights = thisSize.Height / 1.5f;
            _deckGPile.Margin = new Thickness(6, heights * -1, 0, 0);
            _deckStack!.Children.Add(_deckGPile);
            _guide1.LoadList(ThisMod);
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            Pinochle2PlayerSaveInfo saveRoot = OurContainer!.Resolve<Pinochle2PlayerSaveInfo>();
            _thisScore!.UpdateLists(saveRoot.PlayerList);
            _playerHandWPF!.UpdateList(ThisMod!.PlayerHand1!);
            _deckGPile!.UpdateDeck(ThisMod.Deck1!);
            _discardGPile!.UpdatePile(ThisMod.Pile1!);
            _trick1!.Update(_mainGame!.TrickArea1!);
            _tempG!.Update(ThisMod.TempSets!);
            return Task.CompletedTask;
        }
        private BaseDeckWPF<Pinochle2PlayerCardInformation, ts, DeckOfCardsWPF<Pinochle2PlayerCardInformation>>? _deckGPile;
        private BasePileWPF<Pinochle2PlayerCardInformation, ts, DeckOfCardsWPF<Pinochle2PlayerCardInformation>>? _discardGPile;
        private ScoreBoardWPF? _thisScore;
        private BaseHandWPF<Pinochle2PlayerCardInformation, ts, DeckOfCardsWPF<Pinochle2PlayerCardInformation>>? _playerHandWPF;
        private TwoPlayerTrickWPF<EnumSuitList, Pinochle2PlayerCardInformation, ts, DeckOfCardsWPF<Pinochle2PlayerCardInformation>>? _trick1;
        private Pinochle2PlayerMainGameClass? _mainGame;
        private readonly GuideUI _guide1 = new GuideUI();
        private StackPanel? _deckStack;
        private BaseHandWPF<Pinochle2PlayerCardInformation, ts, DeckOfCardsWPF<Pinochle2PlayerCardInformation>>? _yourMeld;
        private BaseHandWPF<Pinochle2PlayerCardInformation, ts, DeckOfCardsWPF<Pinochle2PlayerCardInformation>>? _opponentMeld;
        private TempRummySetsWPF<EnumSuitList, EnumColorList, Pinochle2PlayerCardInformation, ts, DeckOfCardsWPF<Pinochle2PlayerCardInformation>>? _tempG;
        protected async override void AfterGameButton()
        {
            _mainGame = OurContainer!.Resolve<Pinochle2PlayerMainGameClass>();
            StackPanel thisStack = new StackPanel(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            _deckGPile = new BaseDeckWPF<Pinochle2PlayerCardInformation, ts, DeckOfCardsWPF<Pinochle2PlayerCardInformation>>();
            _discardGPile = new BasePileWPF<Pinochle2PlayerCardInformation, ts, DeckOfCardsWPF<Pinochle2PlayerCardInformation>>();
            _thisScore = new ScoreBoardWPF();
            _playerHandWPF = new BaseHandWPF<Pinochle2PlayerCardInformation, ts, DeckOfCardsWPF<Pinochle2PlayerCardInformation>>();
            _trick1 = new TwoPlayerTrickWPF<EnumSuitList, Pinochle2PlayerCardInformation, ts, DeckOfCardsWPF<Pinochle2PlayerCardInformation>>();
            _deckStack = new StackPanel();
            _yourMeld = new BaseHandWPF<Pinochle2PlayerCardInformation, ts, DeckOfCardsWPF<Pinochle2PlayerCardInformation>>();
            _opponentMeld = new BaseHandWPF<Pinochle2PlayerCardInformation, ts, DeckOfCardsWPF<Pinochle2PlayerCardInformation>>();
            _tempG = new TempRummySetsWPF<EnumSuitList, EnumColorList, Pinochle2PlayerCardInformation, ts, DeckOfCardsWPF<Pinochle2PlayerCardInformation>>();
            _trick1.Width = 500; // i think.
            _yourMeld.Divider = 1.5;
            _opponentMeld.Divider = 1.5;
            _yourMeld.HandType = HandViewModel<Pinochle2PlayerCardInformation>.EnumHandList.Vertical;
            _opponentMeld.HandType = HandViewModel<Pinochle2PlayerCardInformation>.EnumHandList.Vertical;
            _tempG.Height = 250; //i think.
            
            GameButton!.HorizontalAlignment = HorizontalAlignment.Center;
            GameButton.VerticalAlignment = VerticalAlignment.Center;
            RoundButton!.HorizontalAlignment = HorizontalAlignment.Center;
            RoundButton.VerticalAlignment = VerticalAlignment.Center;
            thisStack.Children.Add(GameButton);
            thisStack.Children.Add(RoundButton); //most has rounds.
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(_deckStack);
            _deckStack.Children.Add(_discardGPile);
            otherStack.Children.Add(_trick1);
            otherStack.Children.Add(_tempG);
            thisStack.Children.Add(otherStack);
            _thisScore = new ScoreBoardWPF();
            _thisScore.AddColumn("Cards Left", false, nameof(Pinochle2PlayerPlayerItem.ObjectCount));
            _thisScore.AddColumn("Tricks Won", false, nameof(Pinochle2PlayerPlayerItem.TricksWon));
            _thisScore.AddColumn("Current Score", false, nameof(Pinochle2PlayerPlayerItem.CurrentScore));
            _thisScore.AddColumn("Total Score", false, nameof(Pinochle2PlayerPlayerItem.TotalScore));
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(Pinochle2PlayerViewModel.NormalTurn));
            firstInfo.AddRow("Trump", nameof(Pinochle2PlayerViewModel.TrumpSuit)); //if trump is not needed, then comment.
            firstInfo.AddRow("Status", nameof(Pinochle2PlayerViewModel.Status));
            thisStack.Children.Add(_playerHandWPF);
            var endButton = GetGamingButton("End Turn", nameof(Pinochle2PlayerViewModel.EndTurnCommand));
            endButton.HorizontalAlignment = HorizontalAlignment.Left;
            var meldBut = GetGamingButton("Meld", nameof(Pinochle2PlayerViewModel.MeldCommand));
            otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            StackPanel TempStack = new StackPanel();
            TempStack.Children.Add(meldBut);
            TempStack.Children.Add(endButton);
            otherStack.Children.Add(TempStack);
            otherStack.Children.Add(_yourMeld);
            otherStack.Children.Add(_opponentMeld);
            thisStack.Children.Add(otherStack);
            StackPanel scoreStack = new StackPanel();
            scoreStack.Children.Add(_thisScore);
            scoreStack.Children.Add(firstInfo.GetContent);
            scoreStack.Children.Add(_guide1);
            Grid FinalGrid = new Grid();
            AddLeftOverColumn(FinalGrid, 70);
            AddLeftOverColumn(FinalGrid, 30); // hopefully that works.
            AddControlToGrid(FinalGrid, thisStack, 0, 0);
            AddControlToGrid(FinalGrid, scoreStack, 0, 1);
            MainGrid!.Children.Add(FinalGrid);
            _deckGPile.Margin = new Thickness(5, 5, 5, 5);
            _discardGPile.Margin = new Thickness(5, 5, 5, 5);
            AddRestoreCommand(thisStack); //i think.  if a stack can't be used, rethink.
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterNonSavedClasses<Pinochle2PlayerViewModel>();
            OurContainer!.RegisterType<DeckViewModel<Pinochle2PlayerCardInformation>>(true);
            OurContainer.RegisterType<BasicGameLoader<Pinochle2PlayerPlayerItem, Pinochle2PlayerSaveInfo>>();
            OurContainer.RegisterType<RegularCardsBasicShuffler<Pinochle2PlayerCardInformation>>();
            OurContainer.RegisterSingleton<IRegularAceCalculator, RegularAceHighCalculator>(); //most of the time, ace is high for trick taking games.
            bool rets = OurContainer.ObjectExist<ISortCategory>();
            if (rets == true)
            {
                ISortCategory ThisCat = OurContainer.Resolve<ISortCategory>();
                SortSimpleCards<Pinochle2PlayerCardInformation> ThisSort = new SortSimpleCards<Pinochle2PlayerCardInformation>();
                ThisSort.SuitForSorting = ThisCat.SortCategory;
                OurContainer.RegisterSingleton(ThisSort); //if we have a custom one, will already be picked up.
            }
            OurContainer.RegisterSingleton<IDeckCount, CustomDeck>();
            OurContainer.RegisterSingleton<IProportionImage, SmallDrawableProportion>(ts.TagUsed);
            OurContainer.RegisterType<TwoPlayerTrickViewModel<EnumSuitList, Pinochle2PlayerCardInformation, Pinochle2PlayerPlayerItem, Pinochle2PlayerSaveInfo>>();
        }
    }
}