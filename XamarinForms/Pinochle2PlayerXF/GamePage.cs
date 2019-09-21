using AndyCristinaGamePackageCP.CommonProportionClasses;
using AndyCristinaGamePackageCP.ExtensionClasses;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Interfaces;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Pages;
using BaseGPXPagesAndControlsXF.BasicControls.GameFrames;
using BaseGPXPagesAndControlsXF.BasicControls.Misc;
using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using BaseGPXPagesAndControlsXF.BasicControls.SingleCardFrames;
using BaseGPXPagesAndControlsXF.BasicControls.TrickUIs;
using BaseGPXPagesAndControlsXF.GameGraphics.Cards;
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
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
using static BasicXFControlsAndPages.Helpers.GridHelper; //just in case
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace Pinochle2PlayerXF
{
    public class GamePage : MultiPlayerPage<Pinochle2PlayerViewModel, Pinochle2PlayerPlayerItem, Pinochle2PlayerSaveInfo>
    {
        public GamePage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode) { }
        public override Task HandleAsync(LoadEventModel message)
        {
            Pinochle2PlayerSaveInfo saveRoot = OurContainer!.Resolve<Pinochle2PlayerSaveInfo>();
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _playerHand!.LoadList(ThisMod!.PlayerHand1!, ts.TagUsed); // i think
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
            _deckGPile.Margin = new Thickness(9, heights * -1, 0, 0);
            _deckStack!.Children.Add(_deckGPile);
            _guide1.LoadList(ThisMod);
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            Pinochle2PlayerSaveInfo saveRoot = OurContainer!.Resolve<Pinochle2PlayerSaveInfo>();
            _thisScore!.UpdateLists(saveRoot.PlayerList);
            _playerHand!.UpdateList(ThisMod!.PlayerHand1!);
            _deckGPile!.UpdateDeck(ThisMod.Deck1!);
            _discardGPile!.UpdatePile(ThisMod.Pile1!);
            _trick1!.Update(_mainGame!.TrickArea1!);
            _tempG!.Update(ThisMod.TempSets!);
            return Task.CompletedTask;
        }
        private BaseDeckXF<Pinochle2PlayerCardInformation, ts, DeckOfCardsXF<Pinochle2PlayerCardInformation>>? _deckGPile;
        private BasePileXF<Pinochle2PlayerCardInformation, ts, DeckOfCardsXF<Pinochle2PlayerCardInformation>>? _discardGPile;
        private ScoreBoardXF? _thisScore;
        private BaseHandXF<Pinochle2PlayerCardInformation, ts, DeckOfCardsXF<Pinochle2PlayerCardInformation>>? _playerHand;
        private TwoPlayerTrickXF<EnumSuitList, Pinochle2PlayerCardInformation, ts, DeckOfCardsXF<Pinochle2PlayerCardInformation>>? _trick1;
        private Pinochle2PlayerMainGameClass? _mainGame;
        private readonly GuideUI _guide1 = new GuideUI();
        private StackLayout? _deckStack;
        private BaseHandXF<Pinochle2PlayerCardInformation, ts, DeckOfCardsXF<Pinochle2PlayerCardInformation>>? _yourMeld;
        private BaseHandXF<Pinochle2PlayerCardInformation, ts, DeckOfCardsXF<Pinochle2PlayerCardInformation>>? _opponentMeld;
        private TempRummySetsXF<EnumSuitList, EnumColorList, Pinochle2PlayerCardInformation, ts, DeckOfCardsXF<Pinochle2PlayerCardInformation>>? _tempG;
        protected override async Task AfterGameButtonAsync()
        {
            _mainGame = OurContainer!.Resolve<Pinochle2PlayerMainGameClass>();
            StackLayout thisStack = new StackLayout(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            _deckGPile = new BaseDeckXF<Pinochle2PlayerCardInformation, ts, DeckOfCardsXF<Pinochle2PlayerCardInformation>>();
            _discardGPile = new BasePileXF<Pinochle2PlayerCardInformation, ts, DeckOfCardsXF<Pinochle2PlayerCardInformation>>();
            _thisScore = new ScoreBoardXF();
            _playerHand = new BaseHandXF<Pinochle2PlayerCardInformation, ts, DeckOfCardsXF<Pinochle2PlayerCardInformation>>();
            _trick1 = new TwoPlayerTrickXF<EnumSuitList, Pinochle2PlayerCardInformation, ts, DeckOfCardsXF<Pinochle2PlayerCardInformation>>();
            _yourMeld = new BaseHandXF<Pinochle2PlayerCardInformation, ts, DeckOfCardsXF<Pinochle2PlayerCardInformation>>();
            _opponentMeld = new BaseHandXF<Pinochle2PlayerCardInformation, ts, DeckOfCardsXF<Pinochle2PlayerCardInformation>>();
            _tempG = new TempRummySetsXF<EnumSuitList, EnumColorList, Pinochle2PlayerCardInformation, ts, DeckOfCardsXF<Pinochle2PlayerCardInformation>>();
            _deckStack = new StackLayout();
            _tempG.HeightRequest = 200;
            _opponentMeld.Divider = 1.5;
            _yourMeld.Divider = 1.5;
            _tempG.Divider = 1.5;
            _yourMeld.HandType = HandViewModel<Pinochle2PlayerCardInformation>.EnumHandList.Vertical;
            _opponentMeld.HandType = HandViewModel<Pinochle2PlayerCardInformation>.EnumHandList.Vertical;
            GameButton!.HorizontalOptions = LayoutOptions.Center;
            GameButton.VerticalOptions = LayoutOptions.Center;
            RoundButton!.HorizontalOptions = LayoutOptions.Center;
            RoundButton.VerticalOptions = LayoutOptions.Center;
            thisStack.Children.Add(GameButton);
            thisStack.Children.Add(RoundButton);//most has rounds.
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_deckStack);
            _deckStack.Children.Add(_discardGPile);
            otherStack.Children.Add(_trick1);
            otherStack.Children.Add(_tempG);
            thisStack.Children.Add(otherStack);
            _thisScore = new ScoreBoardXF();
            _thisScore.AddColumn("Cards", false, nameof(Pinochle2PlayerPlayerItem.ObjectCount));
            _thisScore.AddColumn("T Won", false, nameof(Pinochle2PlayerPlayerItem.TricksWon));
            _thisScore.AddColumn("C Score", false, nameof(Pinochle2PlayerPlayerItem.CurrentScore));
            _thisScore.AddColumn("T Score", false, nameof(Pinochle2PlayerPlayerItem.TotalScore));
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(Pinochle2PlayerViewModel.NormalTurn));
            firstInfo.AddRow("Trump", nameof(Pinochle2PlayerViewModel.TrumpSuit)); //if trump is not needed, then comment.
            firstInfo.AddRow("Status", nameof(Pinochle2PlayerViewModel.Status));
            thisStack.Children.Add(_playerHand);
            var endButton = GetGamingButton("End Turn", nameof(Pinochle2PlayerViewModel.EndTurnCommand));
            endButton.HorizontalOptions = LayoutOptions.Start;
            var meldBut = GetGamingButton("Meld", nameof(Pinochle2PlayerViewModel.MeldCommand));
            otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            StackLayout tempStack = new StackLayout();
            tempStack.Children.Add(meldBut);
            tempStack.Children.Add(endButton);
            otherStack.Children.Add(tempStack); //i think this too.
            tempStack = new StackLayout();
            tempStack.Children.Add(firstInfo.GetContent);
            tempStack.Children.Add(_thisScore);
            otherStack.Children.Add(tempStack);
            otherStack.Children.Add(_yourMeld);
            otherStack.Children.Add(_opponentMeld);
            thisStack.Children.Add(otherStack);
            Grid finalGrid = new Grid();
            AddLeftOverColumn(finalGrid, 60);
            AddLeftOverColumn(finalGrid, 40); // hopefully that works.
            AddControlToGrid(finalGrid, thisStack, 0, 0);
            AddControlToGrid(finalGrid, _guide1, 0, 1);
            MainGrid!.Children.Add(finalGrid);
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