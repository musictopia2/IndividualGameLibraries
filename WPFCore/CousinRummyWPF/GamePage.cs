using AndyCristinaGamePackageCP.CommonProportionClasses;
using AndyCristinaGamePackageCP.ExtensionClasses;
using BaseGPXWindowsAndControlsCore.BaseWindows;
using BaseGPXWindowsAndControlsCore.BasicControls.GameFrames;
using BaseGPXWindowsAndControlsCore.BasicControls.Misc;
using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using BaseGPXWindowsAndControlsCore.BasicControls.SingleCardFrames;
using BaseGPXWindowsAndControlsCore.GameGraphics.Cards;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using BasicGameFramework.RegularDeckOfCards;
using CousinRummyCP;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //just in case
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace CousinRummyWPF
{
    public class GamePage : MultiPlayerWindow<CousinRummyViewModel, CousinRummyPlayerItem, CousinRummySaveInfo>
    {
        public GamePage(IStartUp starts, EnumGamePackageMode mode) //this means something needs to put into here.
        {
            BuildXAML(starts, mode);
        }
        public override Task HandleAsync(LoadEventModel message)
        {
            CousinRummySaveInfo saveRoot = OurContainer!.Resolve<CousinRummySaveInfo>();
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _playerHandWPF!.LoadList(ThisMod!.PlayerHand1!, ts.TagUsed); // i think
            _discardGPile!.Init(ThisMod.Pile1!, ts.TagUsed); // may have to be here (well see)
            _discardGPile.StartListeningDiscardPile(); // its the main one.
            _deckGPile!.Init(ThisMod.Deck1!, ts.TagUsed); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();
            _tempG!.Init(ThisMod.TempSets!, ts.TagUsed);
            _mainG!.Init(ThisMod.MainSets!, ts.TagUsed);
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            CousinRummySaveInfo saveRoot = OurContainer!.Resolve<CousinRummySaveInfo>();
            _thisScore!.UpdateLists(saveRoot.PlayerList);
            _playerHandWPF!.UpdateList(ThisMod!.PlayerHand1!);
            _deckGPile!.UpdateDeck(ThisMod.Deck1!);
            _discardGPile!.UpdatePile(ThisMod.Pile1!);
            _tempG!.Update(ThisMod.TempSets!);
            _mainG!.Update(ThisMod.MainSets!);
            return Task.CompletedTask;
        }
        private BaseDeckWPF<RegularRummyCard, ts, DeckOfCardsWPF<RegularRummyCard>>? _deckGPile;
        private BasePileWPF<RegularRummyCard, ts, DeckOfCardsWPF<RegularRummyCard>>? _discardGPile;
        private ScoreBoardWPF? _thisScore;
        private BaseHandWPF<RegularRummyCard, ts, DeckOfCardsWPF<RegularRummyCard>>? _playerHandWPF;
        private TempRummySetsWPF<EnumSuitList, EnumColorList, RegularRummyCard, ts, DeckOfCardsWPF<RegularRummyCard>>? _tempG;
        private MainRummySetsWPF<EnumSuitList, EnumColorList, RegularRummyCard, ts, DeckOfCardsWPF<RegularRummyCard>, PhaseSet, SavedSet>? _mainG;
        protected async override void AfterGameButton()
        {
            StackPanel thisStack = new StackPanel(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            _deckGPile = new BaseDeckWPF<RegularRummyCard, ts, DeckOfCardsWPF<RegularRummyCard>>();
            _discardGPile = new BasePileWPF<RegularRummyCard, ts, DeckOfCardsWPF<RegularRummyCard>>();
            _thisScore = new ScoreBoardWPF();
            _playerHandWPF = new BaseHandWPF<RegularRummyCard, ts, DeckOfCardsWPF<RegularRummyCard>>();
            _tempG = new TempRummySetsWPF<EnumSuitList, EnumColorList, RegularRummyCard, ts, DeckOfCardsWPF<RegularRummyCard>>();
            _mainG = new MainRummySetsWPF<EnumSuitList, EnumColorList, RegularRummyCard, ts, DeckOfCardsWPF<RegularRummyCard>, PhaseSet, SavedSet>();
            GameButton!.HorizontalAlignment = HorizontalAlignment.Center;
            GameButton.VerticalAlignment = VerticalAlignment.Center;
            RoundButton!.HorizontalAlignment = HorizontalAlignment.Center;
            RoundButton.VerticalAlignment = VerticalAlignment.Center;
            thisStack.Children.Add(GameButton);
            thisStack.Children.Add(RoundButton);
            Grid buyGrid = new Grid();
            AddAutoColumns(buyGrid, 2);
            AddAutoRows(buyGrid, 2);
            Button thisBut;
            thisBut = GetGamingButton("Pass", nameof(CousinRummyViewModel.PassCommand));
            AddControlToGrid(buyGrid, thisBut, 0, 0);
            thisBut = GetGamingButton("Buy", nameof(CousinRummyViewModel.BuyCommand));
            AddControlToGrid(buyGrid, thisBut, 0, 1);
            Grid gameGrid = new Grid();
            AddLeftOverColumn(gameGrid, 1); // try that
            AddAutoColumns(gameGrid, 1);
            AddAutoRows(gameGrid, 1);
            AddPixelRow(gameGrid, 450);
            _deckGPile.HorizontalAlignment = HorizontalAlignment.Left;
            _deckGPile.VerticalAlignment = VerticalAlignment.Top;
            AddControlToGrid(buyGrid, _deckGPile, 1, 0);
            _discardGPile.HorizontalAlignment = HorizontalAlignment.Left;
            _discardGPile.VerticalAlignment = VerticalAlignment.Top;
            AddControlToGrid(buyGrid, _discardGPile, 1, 1);
            StackPanel otherStack = new StackPanel();
            otherStack.Children.Add(_playerHandWPF);
            thisBut = GetGamingButton("Lay Down Initial Sets", nameof(CousinRummyViewModel.InitSetsCommand));
            otherStack.Children.Add(thisBut);
            thisBut = GetGamingButton("Lay Down Other Sets", nameof(CousinRummyViewModel.OtherSetsCommand)); // i think its othersets commands (?)
            otherStack.Children.Add(thisBut);
            AddControlToGrid(gameGrid, otherStack, 0, 0);
            _tempG.Height = 400;
            AddControlToGrid(gameGrid, _tempG, 0, 1);
            AddControlToGrid(gameGrid, _mainG, 1, 0);
            Grid.SetColumnSpan(_mainG, 2);
            _thisScore = new ScoreBoardWPF();
            _thisScore.AddColumn("Cards Left", false, nameof(CousinRummyPlayerItem.ObjectCount)); //very common.
            _thisScore.AddColumn("Tokens Left", false, nameof(CousinRummyPlayerItem.TokensLeft));
            _thisScore.AddColumn("Current Score", false, nameof(CousinRummyPlayerItem.CurrentScore), rightMargin: 5);
            _thisScore.AddColumn("Total Score", false, nameof(CousinRummyPlayerItem.TotalScore));
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Normal Turn", nameof(CousinRummyViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(CousinRummyViewModel.Status));
            firstInfo.AddRow("Other Turn", nameof(CousinRummyViewModel.OtherLabel));
            firstInfo.AddRow("Phase", nameof(CousinRummyViewModel.PhaseData));
            var tempStack = new StackPanel();
            tempStack.Orientation = Orientation.Horizontal;
            tempStack.Children.Add(_thisScore);
            tempStack.Children.Add(buyGrid);
            tempStack.Children.Add(firstInfo.GetContent);
            otherStack.Children.Add(tempStack);
            thisStack.Children.Add(gameGrid);
            MainGrid!.Children.Add(thisStack);
            _deckGPile.Margin = new Thickness(5, 5, 5, 5);
            _discardGPile.Margin = new Thickness(5, 5, 5, 5);
            AddRestoreCommand(thisStack); //usually to this.  can be to another control if needed.
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterType<BasicGameLoader<CousinRummyPlayerItem, CousinRummySaveInfo>>(); //i think basic game loader gets done here still.
            OurContainer!.RegisterCommonRegularCards<CousinRummyViewModel, RegularRummyCard>(registerCommonProportions: false, customDeck: true);
            OurContainer.RegisterSingleton<IDeckCount, CustomDeck>(); //forgot to use a custom deck for this one.
            OurContainer.RegisterSingleton<IRegularAceCalculator, RegularAceHighCalculator>();
            OurContainer.RegisterSingleton<IProportionImage, SmallDrawableProportion>(ts.TagUsed);
        }
    }
}