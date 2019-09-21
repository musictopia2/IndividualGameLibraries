using AndyCristinaGamePackageCP.CommonProportionClasses;
using AndyCristinaGamePackageCP.ExtensionClasses;
using AndyCristinaGamePackageXF;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Interfaces;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Pages;
using BaseGPXPagesAndControlsXF.BasicControls.GameFrames;
using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
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
using PickelCardGameCP;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace PickelCardGameXF
{
    public class GamePage : MultiPlayerPage<PickelCardGameViewModel, PickelCardGamePlayerItem, PickelCardGameSaveInfo>
    {
        public GamePage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode) { }
        public override Task HandleAsync(LoadEventModel message)
        {
            var temps = OurContainer!.Resolve<SeveralPlayersTrickViewModel<EnumSuitList, PickelCardGameCardInformation, PickelCardGamePlayerItem, PickelCardGameSaveInfo>>();
            PickelCardGameSaveInfo saveRoot = OurContainer!.Resolve<PickelCardGameSaveInfo>();
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _playerHand!.LoadList(ThisMod!.PlayerHand1!, ts.TagUsed); // i think
            _trick1!.Init(_mainGame!.TrickArea1!, temps, ts.TagUsed);
            _bidHand!.LoadList(ThisMod.PlayerHand1!, ts.TagUsed);
            _bid1!.LoadLists(ThisMod);
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            PickelCardGameSaveInfo saveRoot = OurContainer!.Resolve<PickelCardGameSaveInfo>();
            _thisScore!.UpdateLists(saveRoot.PlayerList);
            _playerHand!.UpdateList(ThisMod!.PlayerHand1!);
            _trick1!.Update(_mainGame!.TrickArea1!);
            _bidHand!.UpdateList(ThisMod.PlayerHand1!);
            return Task.CompletedTask;
        }
        private ScoreBoardXF? _thisScore;
        private BaseHandXF<PickelCardGameCardInformation, ts, DeckOfCardsXF<PickelCardGameCardInformation>>? _playerHand;
        private SeveralPlayersTrickXF<EnumSuitList, PickelCardGameCardInformation, ts, DeckOfCardsXF<PickelCardGameCardInformation>, PickelCardGamePlayerItem>? _trick1;
        private PickelCardGameMainGameClass? _mainGame;
        private BidControl? _bid1;
        private StackLayout? _bidStack;
        private BaseHandXF<PickelCardGameCardInformation, ts, DeckOfCardsXF<PickelCardGameCardInformation>>? _bidHand;
        protected override void ComplexStartControls(Grid thisGrid)
        {
            _bidStack = new StackLayout();
            thisGrid.Children.Add(_bidStack);
        }
        private void PopulateScores(ScoreBoardXF thisScore)
        {
            thisScore.AddColumn("Suit Desired", true, nameof(PickelCardGamePlayerItem.SuitDesired));
            thisScore.AddColumn("Bid", false, nameof(PickelCardGamePlayerItem.BidAmount));
            thisScore.AddColumn("Won", false, nameof(PickelCardGamePlayerItem.TricksWon));
            thisScore.AddColumn("Score", false, nameof(PickelCardGamePlayerItem.TotalScore));
        }
        protected override async Task AfterGameButtonAsync()
        {
            _mainGame = OurContainer!.Resolve<PickelCardGameMainGameClass>();
            StackLayout thisStack = new StackLayout(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            _thisScore = new ScoreBoardXF();
            _playerHand = new BaseHandXF<PickelCardGameCardInformation, ts, DeckOfCardsXF<PickelCardGameCardInformation>>();
            _trick1 = new SeveralPlayersTrickXF<EnumSuitList, PickelCardGameCardInformation, ts, DeckOfCardsXF<PickelCardGameCardInformation>, PickelCardGamePlayerItem>();
            _bidHand = new BaseHandXF<PickelCardGameCardInformation, ts, DeckOfCardsXF<PickelCardGameCardInformation>>();
            _bid1 = new BidControl();
            Binding thisBind = new Binding(nameof(PickelCardGameViewModel.BiddingScreenVisible));
            _bidStack!.SetBinding(IsVisibleProperty, thisBind);
            _bidStack.Children.Add(_bid1);
            _bidStack.Children.Add(_bidHand);
            SimpleLabelGridXF bidInfo = new SimpleLabelGridXF();
            bidInfo.AddRow("Turn", nameof(PickelCardGameViewModel.NormalTurn));
            _bidStack!.Children.Add(bidInfo.GetContent);
            GameButton!.HorizontalOptions = LayoutOptions.Center;
            GameButton.VerticalOptions = LayoutOptions.Center;
            RoundButton!.HorizontalOptions = LayoutOptions.Center;
            RoundButton.VerticalOptions = LayoutOptions.Center;
            thisStack.Children.Add(GameButton);
            thisStack.Children.Add(RoundButton);//most has rounds.
            _thisScore = new ScoreBoardXF();
            PopulateScores(_thisScore);
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(PickelCardGameViewModel.NormalTurn));
            firstInfo.AddRow("Trump", nameof(PickelCardGameViewModel.TrumpSuit)); //if trump is not needed, then comment.
            firstInfo.AddRow("Status", nameof(PickelCardGameViewModel.Status));
            thisStack.Children.Add(_trick1);
            MainGrid!.Children.Add(thisStack);
            thisStack.Children.Add(_playerHand);
            thisStack.Children.Add(firstInfo.GetContent);
            thisStack.Children.Add(_thisScore);
            AddRestoreCommand(thisStack); //i think.  if a stack can't be used, rethink.
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterNonSavedClasses<PickelCardGameViewModel>();
            OurContainer!.RegisterType<DeckViewModel<PickelCardGameCardInformation>>(true);
            OurContainer.RegisterType<BasicGameLoader<PickelCardGamePlayerItem, PickelCardGameSaveInfo>>();
            OurContainer.RegisterType<RegularCardsBasicShuffler<PickelCardGameCardInformation>>();
            OurContainer.RegisterSingleton<IRegularAceCalculator, RegularAceHighCalculator>(); //most of the time, ace is high for trick taking games.
            OurContainer.RegisterSingleton<IDeckCount, CustomDeck>(); //i think this is best this time.
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(ts.TagUsed);
            OurContainer.RegisterType<SeveralPlayersTrickViewModel<EnumSuitList, PickelCardGameCardInformation, PickelCardGamePlayerItem, PickelCardGameSaveInfo>>();
            OurContainer.RegisterType<StandardWidthHeight>();
            OurContainer.RegisterType<StandardPickerSizeClass>(); //forgot this.
        }
    }
}