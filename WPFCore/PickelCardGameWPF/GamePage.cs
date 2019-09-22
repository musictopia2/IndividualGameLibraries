using BasicGameFramework.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BaseGPXWindowsAndControlsCore.BaseWindows;
using BaseGPXWindowsAndControlsCore.BasicControls.ChoicePickers;
using BaseGPXWindowsAndControlsCore.BasicControls.GameFrames;
using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
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
using PickelCardGameCP;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace PickelCardGameWPF
{
    public class GamePage : MultiPlayerWindow<PickelCardGameViewModel, PickelCardGamePlayerItem, PickelCardGameSaveInfo>
    {
        public GamePage(IStartUp starts, EnumGamePackageMode mode) //this means something needs to put into here.
        {
            BuildXAML(starts, mode);
        }
        public override Task HandleAsync(LoadEventModel message)
        {
            var temps = OurContainer!.Resolve<SeveralPlayersTrickViewModel<EnumSuitList, PickelCardGameCardInformation, PickelCardGamePlayerItem, PickelCardGameSaveInfo>>();
            PickelCardGameSaveInfo saveRoot = OurContainer!.Resolve<PickelCardGameSaveInfo>();
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _playerHandWPF!.LoadList(ThisMod!.PlayerHand1!, ts.TagUsed); // i think
            _trick1!.Init(_mainGame!.TrickArea1!, temps, ts.TagUsed);
            _bidHand!.LoadList(ThisMod.PlayerHand1!, ts.TagUsed);
            _bid1!.LoadLists(ThisMod);
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            PickelCardGameSaveInfo saveRoot = OurContainer!.Resolve<PickelCardGameSaveInfo>();
            _thisScore!.UpdateLists(saveRoot.PlayerList);
            _playerHandWPF!.UpdateList(ThisMod!.PlayerHand1!);
            _trick1!.Update(_mainGame!.TrickArea1!);
            _bidHand!.UpdateList(ThisMod.PlayerHand1!);
            return Task.CompletedTask;
        }

        private ScoreBoardWPF? _thisScore;
        private BaseHandWPF<PickelCardGameCardInformation, ts, DeckOfCardsWPF<PickelCardGameCardInformation>>? _playerHandWPF;
        private SeveralPlayersTrickWPF<EnumSuitList, PickelCardGameCardInformation, ts, DeckOfCardsWPF<PickelCardGameCardInformation>, PickelCardGamePlayerItem>? _trick1;
        private PickelCardGameMainGameClass? _mainGame;
        private BidControl? _bid1;
        private StackPanel? _bidStack;
        private BaseHandWPF<PickelCardGameCardInformation, ts, DeckOfCardsWPF<PickelCardGameCardInformation>>? _bidHand;
        protected override void ComplexStartControls(Grid thisGrid)
        {
            _bidStack = new StackPanel();
            thisGrid.Children.Add(_bidStack);
        }
        private void PopulateScores(ScoreBoardWPF thisScore)
        {
            thisScore.AddColumn("Suit Desired", true, nameof(PickelCardGamePlayerItem.SuitDesired));
            thisScore.AddColumn("Bid Amount", false, nameof(PickelCardGamePlayerItem.BidAmount));
            thisScore.AddColumn("Tricks Won", false, nameof(PickelCardGamePlayerItem.TricksWon));
            thisScore.AddColumn("Current Score", false, nameof(PickelCardGamePlayerItem.CurrentScore));
            thisScore.AddColumn("Total Score", false, nameof(PickelCardGamePlayerItem.TotalScore));
        }
        protected async override void AfterGameButton()
        {
            _mainGame = OurContainer!.Resolve<PickelCardGameMainGameClass>();
            StackPanel thisStack = new StackPanel(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            _thisScore = new ScoreBoardWPF();
            _playerHandWPF = new BaseHandWPF<PickelCardGameCardInformation, ts, DeckOfCardsWPF<PickelCardGameCardInformation>>();
            _trick1 = new SeveralPlayersTrickWPF<EnumSuitList, PickelCardGameCardInformation, ts, DeckOfCardsWPF<PickelCardGameCardInformation>, PickelCardGamePlayerItem>();
            _bidHand = new BaseHandWPF<PickelCardGameCardInformation, ts, DeckOfCardsWPF<PickelCardGameCardInformation>>();
            _bid1 = new BidControl();
            Binding thisBind = GetVisibleBinding(nameof(PickelCardGameViewModel.BiddingScreenVisible));
            _bidStack!.SetBinding(VisibilityProperty, thisBind);
            _bidStack.Children.Add(_bid1);
            _bidStack.Children.Add(_bidHand);
            SimpleLabelGrid bidInfo = new SimpleLabelGrid();
            bidInfo.AddRow("Turn", nameof(PickelCardGameViewModel.NormalTurn));
            _bidStack!.Children.Add(bidInfo.GetContent);
            GameButton!.HorizontalAlignment = HorizontalAlignment.Center;
            GameButton.VerticalAlignment = VerticalAlignment.Center;
            RoundButton!.HorizontalAlignment = HorizontalAlignment.Center;
            RoundButton.VerticalAlignment = VerticalAlignment.Center;
            thisStack.Children.Add(GameButton);
            thisStack.Children.Add(RoundButton); //most has rounds.
            _thisScore = new ScoreBoardWPF();
            PopulateScores(_thisScore);
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(PickelCardGameViewModel.NormalTurn));
            firstInfo.AddRow("Trump", nameof(PickelCardGameViewModel.TrumpSuit)); //if trump is not needed, then comment.
            firstInfo.AddRow("Status", nameof(PickelCardGameViewModel.Status));
            thisStack.Children.Add(_trick1);
            MainGrid!.Children.Add(thisStack);
            thisStack.Children.Add(_playerHandWPF);
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
        }
    }
}