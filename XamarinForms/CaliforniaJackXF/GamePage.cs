using AndyCristinaGamePackageCP.CommonProportionClasses;
using AndyCristinaGamePackageCP.ExtensionClasses;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Interfaces;
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
using CaliforniaJackCP;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace CaliforniaJackXF
{
    public class GamePage : MultiPlayerPage<CaliforniaJackViewModel, CaliforniaJackPlayerItem, CaliforniaJackSaveInfo>
    {
        public GamePage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode) { }
        public override Task HandleAsync(LoadEventModel message)
        {
            CaliforniaJackSaveInfo saveRoot = OurContainer!.Resolve<CaliforniaJackSaveInfo>();
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _playerHand!.LoadList(ThisMod!.PlayerHand1!, ts.TagUsed); // i think
            _trick1!.Init(_mainGame!.TrickArea1!, ts.TagUsed);
            _deckGPile!.Init(ThisMod.Deck1!, ts.TagUsed); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            CaliforniaJackSaveInfo saveRoot = OurContainer!.Resolve<CaliforniaJackSaveInfo>();
            _thisScore!.UpdateLists(saveRoot.PlayerList);
            _playerHand!.UpdateList(ThisMod!.PlayerHand1!);
            _deckGPile!.UpdateDeck(ThisMod.Deck1!);
            _trick1!.Update(_mainGame!.TrickArea1!);
            return Task.CompletedTask;
        }
        private BaseDeckXF<CaliforniaJackCardInformation, ts, DeckOfCardsXF<CaliforniaJackCardInformation>>? _deckGPile;

        private ScoreBoardXF? _thisScore;
        private BaseHandXF<CaliforniaJackCardInformation, ts, DeckOfCardsXF<CaliforniaJackCardInformation>>? _playerHand;
        private TwoPlayerTrickXF<EnumSuitList, CaliforniaJackCardInformation, ts, DeckOfCardsXF<CaliforniaJackCardInformation>>? _trick1;
        private CaliforniaJackMainGameClass? _mainGame;
        protected override async Task AfterGameButtonAsync()
        {
            _mainGame = OurContainer!.Resolve<CaliforniaJackMainGameClass>();
            StackLayout thisStack = new StackLayout(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            _deckGPile = new BaseDeckXF<CaliforniaJackCardInformation, ts, DeckOfCardsXF<CaliforniaJackCardInformation>>();
            _thisScore = new ScoreBoardXF();
            _playerHand = new BaseHandXF<CaliforniaJackCardInformation, ts, DeckOfCardsXF<CaliforniaJackCardInformation>>();
            _trick1 = new TwoPlayerTrickXF<EnumSuitList, CaliforniaJackCardInformation, ts, DeckOfCardsXF<CaliforniaJackCardInformation>>();
            GameButton!.HorizontalOptions = LayoutOptions.Center;
            GameButton.VerticalOptions = LayoutOptions.Center;
            RoundButton!.HorizontalOptions = LayoutOptions.Center;
            RoundButton.VerticalOptions = LayoutOptions.Center;
            thisStack.Children.Add(GameButton);
            thisStack.Children.Add(RoundButton);//most has rounds.
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            thisStack.Children.Add(otherStack);
            _thisScore = new ScoreBoardXF();
            _thisScore.AddColumn("Cards Left", false, nameof(CaliforniaJackPlayerItem.ObjectCount)); //very common.
            _thisScore.AddColumn("Tricks Won", false, nameof(CaliforniaJackPlayerItem.TricksWon), rightMargin: 10);
            _thisScore.AddColumn("Points", false, nameof(CaliforniaJackPlayerItem.Points), rightMargin: 10);
            _thisScore.AddColumn("Total Score", false, nameof(CaliforniaJackPlayerItem.TotalScore), rightMargin: 10);
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(CaliforniaJackViewModel.NormalTurn));
            firstInfo.AddRow("Trump", nameof(CaliforniaJackViewModel.TrumpSuit)); //if trump is not needed, then comment.
            firstInfo.AddRow("Status", nameof(CaliforniaJackViewModel.Status));
            thisStack.Children.Add(_trick1);
            MainGrid!.Children.Add(thisStack);
            thisStack.Children.Add(_playerHand);
            thisStack.Children.Add(firstInfo.GetContent);
            otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            thisStack.Children.Add(otherStack);
            otherStack.Children.Add(_thisScore);
            _deckGPile.Margin = new Thickness(5, 5, 5, 5);
            AddRestoreCommand(thisStack); //i think.  if a stack can't be used, rethink.
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterNonSavedClasses<CaliforniaJackViewModel>();
            OurContainer!.RegisterType<DeckViewModel<CaliforniaJackCardInformation>>(true);
            OurContainer.RegisterType<BasicGameLoader<CaliforniaJackPlayerItem, CaliforniaJackSaveInfo>>();
            OurContainer.RegisterType<RegularCardsBasicShuffler<CaliforniaJackCardInformation>>();
            OurContainer.RegisterSingleton<IRegularAceCalculator, RegularAceHighCalculator>(); //most of the time, ace is high for trick taking games.
            bool rets = OurContainer.ObjectExist<ISortCategory>();
            if (rets == true)
            {
                ISortCategory ThisCat = OurContainer.Resolve<ISortCategory>();
                SortSimpleCards<CaliforniaJackCardInformation> ThisSort = new SortSimpleCards<CaliforniaJackCardInformation>();
                ThisSort.SuitForSorting = ThisCat.SortCategory;
                OurContainer.RegisterSingleton(ThisSort); //if we have a custom one, will already be picked up.
            }
            OurContainer.RegisterSingleton<IDeckCount, RegularAceHighSimpleDeck>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(ts.TagUsed);
            OurContainer.RegisterType<TwoPlayerTrickViewModel<EnumSuitList, CaliforniaJackCardInformation, CaliforniaJackPlayerItem, CaliforniaJackSaveInfo>>();
        }
    }
}