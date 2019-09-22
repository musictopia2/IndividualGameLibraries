using BasicGameFramework.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.BasicClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.Interfaces;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Pages;
using BaseGPXPagesAndControlsXF.BasicControls.GameFrames;
using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using BaseGPXPagesAndControlsXF.BasicControls.SingleCardFrames;
using BaseGPXPagesAndControlsXF.BasicControls.TrickUIs;
using BaseGPXPagesAndControlsXF.GameGraphics.Cards;
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
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace Spades2PlayerXF
{
    public class GamePage : MultiPlayerPage<Spades2PlayerViewModel, Spades2PlayerPlayerItem, Spades2PlayerSaveInfo>
    {
        public GamePage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode) { }
        public override Task HandleAsync(LoadEventModel message)
        {
            Spades2PlayerSaveInfo saveRoot = OurContainer!.Resolve<Spades2PlayerSaveInfo>();
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _playerHand!.LoadList(ThisMod!.PlayerHand1!, ts.TagUsed); // i think
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
            _playerHand!.UpdateList(ThisMod!.PlayerHand1!);
            _deckGPile!.UpdateDeck(ThisMod.Deck1!);
            _discardGPile!.UpdatePile(ThisMod.Pile1!);
            _trick1!.Update(_mainGame!.TrickArea1!);
            _pile2!.UpdatePile(ThisMod.Pile2!);
            return Task.CompletedTask;
        }
        private BaseDeckXF<Spades2PlayerCardInformation, ts, DeckOfCardsXF<Spades2PlayerCardInformation>>? _deckGPile;
        private BasePileXF<Spades2PlayerCardInformation, ts, DeckOfCardsXF<Spades2PlayerCardInformation>>? _discardGPile;
        private ScoreBoardXF? _thisScore;
        private BaseHandXF<Spades2PlayerCardInformation, ts, DeckOfCardsXF<Spades2PlayerCardInformation>>? _playerHand;
        private TwoPlayerTrickXF<EnumSuitList, Spades2PlayerCardInformation, ts, DeckOfCardsXF<Spades2PlayerCardInformation>>? _trick1;
        private Spades2PlayerMainGameClass? _mainGame;
        private BasePileXF<Spades2PlayerCardInformation, ts, DeckOfCardsXF<Spades2PlayerCardInformation>>? _pile2;
        private BidControl? _bid1;
        protected async override Task AfterGameButtonAsync()
        {
            _mainGame = OurContainer!.Resolve<Spades2PlayerMainGameClass>();
            StackLayout thisStack = new StackLayout(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            _deckGPile = new BaseDeckXF<Spades2PlayerCardInformation, ts, DeckOfCardsXF<Spades2PlayerCardInformation>>();
            _discardGPile = new BasePileXF<Spades2PlayerCardInformation, ts, DeckOfCardsXF<Spades2PlayerCardInformation>>();
            _thisScore = new ScoreBoardXF();
            _playerHand = new BaseHandXF<Spades2PlayerCardInformation, ts, DeckOfCardsXF<Spades2PlayerCardInformation>>();
            _trick1 = new TwoPlayerTrickXF<EnumSuitList, Spades2PlayerCardInformation, ts, DeckOfCardsXF<Spades2PlayerCardInformation>>();
            _pile2 = new BasePileXF<Spades2PlayerCardInformation, ts, DeckOfCardsXF<Spades2PlayerCardInformation>>();
            _bid1 = new BidControl();
            GameButton!.HorizontalOptions = LayoutOptions.Center;
            GameButton.VerticalOptions = LayoutOptions.Center;
            RoundButton!.HorizontalOptions = LayoutOptions.Center;
            RoundButton.VerticalOptions = LayoutOptions.Center;
            thisStack.Children.Add(GameButton);
            thisStack.Children.Add(RoundButton); //most has rounds.
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            var thisBut = GetGamingButton("Take Card", nameof(Spades2PlayerViewModel.TakeCardCommand));
            otherStack.Children.Add(thisBut);
            otherStack.Children.Add(_pile2);
            thisStack.Children.Add(otherStack);
            Binding thisBind = new Binding(nameof(Spades2PlayerViewModel.BeginningVisible));
            otherStack.SetBinding(IsVisibleProperty, thisBind);
            thisStack.Children.Add(_trick1);
            thisStack.Children.Add(_bid1);
            _thisScore = new ScoreBoardXF();
            _thisScore.AddColumn("Cards", false, nameof(Spades2PlayerPlayerItem.ObjectCount)); //very common.
            _thisScore.AddColumn("Bidded", false, nameof(Spades2PlayerPlayerItem.HowManyBids));
            _thisScore.AddColumn("Won", false, nameof(Spades2PlayerPlayerItem.TricksWon));
            _thisScore.AddColumn("Bags", false, nameof(Spades2PlayerPlayerItem.Bags));
            _thisScore.AddColumn("C Score", false, nameof(Spades2PlayerPlayerItem.CurrentScore));
            _thisScore.AddColumn("T Score", false, nameof(Spades2PlayerPlayerItem.TotalScore));
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(Spades2PlayerViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(Spades2PlayerViewModel.Status));
            firstInfo.AddRow("Round", nameof(Spades2PlayerViewModel.RoundNumber));
            MainGrid!.Children.Add(thisStack);
            thisStack.Children.Add(_playerHand);
            otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            thisStack.Children.Add(otherStack);
            otherStack.Children.Add(firstInfo.GetContent);
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