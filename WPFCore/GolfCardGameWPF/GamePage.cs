using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BaseGPXWindowsAndControlsCore.BaseWindows;
using BaseGPXWindowsAndControlsCore.BasicControls.GameFrames;
using BaseGPXWindowsAndControlsCore.BasicControls.MultipleFrameContainers;
using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using BaseGPXWindowsAndControlsCore.BasicControls.SingleCardFrames;
using BaseGPXWindowsAndControlsCore.GameGraphics.Cards;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using BasicGameFramework.RegularDeckOfCards;
using GolfCardGameCP;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //just in case
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace GolfCardGameWPF
{
    public class GamePage : MultiPlayerWindow<GolfCardGameViewModel, GolfCardGamePlayerItem, GolfCardGameSaveInfo>
    {
        public GamePage(IStartUp starts, EnumGamePackageMode mode) //this means something needs to put into here.
        {
            BuildXAML(starts, mode);
        }
        public override Task HandleAsync(LoadEventModel message)
        {
            GolfCardGameSaveInfo saveRoot = OurContainer!.Resolve<GolfCardGameSaveInfo>();
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _discardGPile!.Init(ThisMod!.Pile1!, ts.TagUsed); // may have to be here (well see)
            _discardGPile.StartListeningDiscardPile(); // its the main one.
            _deckGPile!.Init(ThisMod.Deck1!, ts.TagUsed); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();
            _otherPileWPF!.Init(ThisMod.Pile2!, ts.TagUsed);
            _hiddenWPF!.Init(ThisMod.HiddenCards1!, ts.TagUsed);
            _beginWPF!.LoadList(ThisMod.Beginnings1!, ts.TagUsed);
            _golfWPF!.LoadList(ThisMod.GolfHand1!, ts.TagUsed);
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            GolfCardGameSaveInfo saveRoot = OurContainer!.Resolve<GolfCardGameSaveInfo>();
            _thisScore!.UpdateLists(saveRoot.PlayerList);
            _deckGPile!.UpdateDeck(ThisMod!.Deck1!);
            _discardGPile!.UpdatePile(ThisMod.Pile1!);
            _otherPileWPF!.UpdatePile(ThisMod.Pile2!);
            _hiddenWPF!.UpdateLists(ThisMod.HiddenCards1!);
            _beginWPF!.UpdateList(ThisMod.Beginnings1!);
            _golfWPF!.UpdateList(ThisMod.GolfHand1!);
            return Task.CompletedTask;
        }
        private BaseDeckWPF<RegularSimpleCard, ts, DeckOfCardsWPF<RegularSimpleCard>>? _deckGPile;
        private BasePileWPF<RegularSimpleCard, ts, DeckOfCardsWPF<RegularSimpleCard>>? _discardGPile;
        private ScoreBoardWPF? _thisScore;
        private BasicMultiplePilesWPF<RegularSimpleCard, ts, DeckOfCardsWPF<RegularSimpleCard>>? _hiddenWPF;
        private BasePileWPF<RegularSimpleCard, ts, DeckOfCardsWPF<RegularSimpleCard>>? _otherPileWPF;
        private CardBoardWPF<RegularSimpleCard, ts, DeckOfCardsWPF<RegularSimpleCard>>? _beginWPF;
        private CardBoardWPF<RegularSimpleCard, ts, DeckOfCardsWPF<RegularSimpleCard>>? _golfWPF;
        protected async override void AfterGameButton()
        {
            StackPanel thisStack = new StackPanel(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            _deckGPile = new BaseDeckWPF<RegularSimpleCard, ts, DeckOfCardsWPF<RegularSimpleCard>>();
            _discardGPile = new BasePileWPF<RegularSimpleCard, ts, DeckOfCardsWPF<RegularSimpleCard>>();
            _thisScore = new ScoreBoardWPF();
            _hiddenWPF = new BasicMultiplePilesWPF<RegularSimpleCard, ts, DeckOfCardsWPF<RegularSimpleCard>>();
            _otherPileWPF = new BasePileWPF<RegularSimpleCard, ts, DeckOfCardsWPF<RegularSimpleCard>>();
            _beginWPF = new CardBoardWPF<RegularSimpleCard, ts, DeckOfCardsWPF<RegularSimpleCard>>();
            _golfWPF = new CardBoardWPF<RegularSimpleCard, ts, DeckOfCardsWPF<RegularSimpleCard>>();
            GameButton!.HorizontalAlignment = HorizontalAlignment.Center;
            GameButton.VerticalAlignment = VerticalAlignment.Center;
            RoundButton!.HorizontalAlignment = HorizontalAlignment.Center;
            RoundButton.VerticalAlignment = VerticalAlignment.Center;
            thisStack.Children.Add(RoundButton);
            thisStack.Children.Add(GameButton);
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            var thisBut = GetGamingButton("Knock", nameof(GolfCardGameViewModel.KnockedCommand));
            var thisBind = GetVisibleBinding(nameof(GolfCardGameViewModel.KnockedVisible)); // hopefully its this simple (?)
            thisBut.SetBinding(VisibilityProperty, thisBind);
            otherStack.Children.Add(thisBut);
            thisStack.Children.Add(otherStack);
            otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(_beginWPF);
            thisBut = GetGamingButton("Choose First Cards", nameof(GolfCardGameViewModel.ChooseFirstCardsCommand));
            thisBind = GetVisibleBinding(nameof(GolfCardGameViewModel.ChooseFirstCardsVisible));
            thisBut.SetBinding(Button.VisibilityProperty, thisBind);
            otherStack.Children.Add(thisBut);
            thisStack.Children.Add(otherStack);
            thisStack.Children.Add(_hiddenWPF);
            otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(_golfWPF);
            otherStack.Children.Add(_otherPileWPF);
            thisStack.Children.Add(otherStack);
            _thisScore = new ScoreBoardWPF();
            _thisScore.UseAbbreviationForTrueFalse = true;
            Grid finalGrid = new Grid();
            AddLeftOverColumn(finalGrid, 40);
            AddLeftOverColumn(finalGrid, 60); // this is for scoreboard
            _thisScore.AddColumn("Knocked", false, nameof(GolfCardGamePlayerItem.Knocked), useTrueFalse: true); // well see how this work.  hopefully this simple.
            _thisScore.AddColumn("1 Changed", false, nameof(GolfCardGamePlayerItem.FirstChanged), useTrueFalse: true);
            _thisScore.AddColumn("2 Changed", false, nameof(GolfCardGamePlayerItem.SecondChanged), useTrueFalse: true);
            _thisScore.AddColumn("Previous Score", false, nameof(GolfCardGamePlayerItem.PreviousScore), rightMargin: 20);
            _thisScore.AddColumn("Total Score", false, nameof(GolfCardGamePlayerItem.TotalScore), rightMargin: 20);
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(GolfCardGameViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(GolfCardGameViewModel.Status));
            firstInfo.AddRow("Round", nameof(GolfCardGameViewModel.Round));
            firstInfo.AddRow("Instructions", nameof(GolfCardGameViewModel.Instructions));
            thisStack.Children.Add(finalGrid);
            AddControlToGrid(finalGrid, firstInfo.GetContent, 0, 0);
            AddControlToGrid(finalGrid, _thisScore, 0, 1);
            MainGrid!.Children.Add(thisStack);
            _deckGPile.Margin = new Thickness(5, 5, 5, 5);
            _discardGPile.Margin = new Thickness(5, 5, 5, 5);
            AddRestoreCommand(thisStack); //i think.  if a stack can't be used, rethink.
            await FinishUpAsync();
        }
        //protected override void RegisterTests()
        //{
        //    OurContainer!.RegisterType<TestConfig>();
        //}
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterType<BasicGameLoader<GolfCardGamePlayerItem, GolfCardGameSaveInfo>>(); //i think basic game loader gets done here still.
            OurContainer!.RegisterCommonRegularCards<GolfCardGameViewModel, RegularSimpleCard>(customDeck: true);
            OurContainer.RegisterSingleton<IRegularAceCalculator, RegularLowAceCalculator>();
            OurContainer.RegisterSingleton<IDeckCount, GolfDeck>();
        }
    }
}