using BasicGameFramework.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
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
using RoundsCardGameCP;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace RoundsCardGameXF
{
    public class GamePage : MultiPlayerPage<RoundsCardGameViewModel, RoundsCardGamePlayerItem, RoundsCardGameSaveInfo>
    {
        public GamePage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode) { }
        public override Task HandleAsync(LoadEventModel message)
        {
            RoundsCardGameSaveInfo saveRoot = OurContainer!.Resolve<RoundsCardGameSaveInfo>();
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _playerHand!.LoadList(ThisMod!.PlayerHand1!, ts.TagUsed); // i think
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
            _playerHand!.UpdateList(ThisMod!.PlayerHand1!);
            _deckGPile!.UpdateDeck(ThisMod.Deck1!);
            _discardGPile!.UpdatePile(ThisMod.Pile1!);
            _trick1!.Update(_mainGame!.TrickArea1!);
            return Task.CompletedTask;
        }
        private BaseDeckXF<RoundsCardGameCardInformation, ts, DeckOfCardsXF<RoundsCardGameCardInformation>>? _deckGPile;
        private BasePileXF<RoundsCardGameCardInformation, ts, DeckOfCardsXF<RoundsCardGameCardInformation>>? _discardGPile;
        private ScoreBoardXF? _thisScore;
        private BaseHandXF<RoundsCardGameCardInformation, ts, DeckOfCardsXF<RoundsCardGameCardInformation>>? _playerHand;
        private TwoPlayerTrickXF<EnumSuitList, RoundsCardGameCardInformation, ts, DeckOfCardsXF<RoundsCardGameCardInformation>>? _trick1;
        private RoundsCardGameMainGameClass? _mainGame;
        protected override async Task AfterGameButtonAsync()
        {
            _mainGame = OurContainer!.Resolve<RoundsCardGameMainGameClass>();
            StackLayout thisStack = new StackLayout(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            _deckGPile = new BaseDeckXF<RoundsCardGameCardInformation, ts, DeckOfCardsXF<RoundsCardGameCardInformation>>();
            _discardGPile = new BasePileXF<RoundsCardGameCardInformation, ts, DeckOfCardsXF<RoundsCardGameCardInformation>>();
            _thisScore = new ScoreBoardXF();
            _playerHand = new BaseHandXF<RoundsCardGameCardInformation, ts, DeckOfCardsXF<RoundsCardGameCardInformation>>();
            _trick1 = new TwoPlayerTrickXF<EnumSuitList, RoundsCardGameCardInformation, ts, DeckOfCardsXF<RoundsCardGameCardInformation>>();
            GameButton!.HorizontalOptions = LayoutOptions.Center;
            GameButton.VerticalOptions = LayoutOptions.Center;
            RoundButton!.HorizontalOptions = LayoutOptions.Center;
            RoundButton.VerticalOptions = LayoutOptions.Center;
            thisStack.Children.Add(GameButton);
            thisStack.Children.Add(RoundButton);//most has rounds.
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            thisStack.Children.Add(otherStack);
            _thisScore = new ScoreBoardXF();
            otherStack.Children.Add(_thisScore);
            _thisScore.AddColumn("# In Hand", true, nameof(RoundsCardGamePlayerItem.ObjectCount));
            _thisScore.AddColumn("Tricks Won", true, nameof(RoundsCardGamePlayerItem.TricksWon));
            _thisScore.AddColumn("Rounds Won", true, nameof(RoundsCardGamePlayerItem.RoundsWon));
            _thisScore.AddColumn("Points", true, nameof(RoundsCardGamePlayerItem.CurrentPoints));
            _thisScore.AddColumn("Total Score", true, nameof(RoundsCardGamePlayerItem.TotalScore));
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(RoundsCardGameViewModel.NormalTurn));
            firstInfo.AddRow("Trump", nameof(RoundsCardGameViewModel.TrumpSuit)); //if trump is not needed, then comment.
            firstInfo.AddRow("Status", nameof(RoundsCardGameViewModel.Status));
            thisStack.Children.Add(_trick1);
            MainGrid!.Children.Add(thisStack);
            thisStack.Children.Add(_playerHand);
            thisStack.Children.Add(firstInfo.GetContent);
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