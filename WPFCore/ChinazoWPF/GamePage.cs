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
using ChinazoCP;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //just in case
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace ChinazoWPF
{
    public class GamePage : MultiPlayerWindow<ChinazoViewModel, ChinazoPlayerItem, ChinazoSaveInfo>
    {
        public GamePage(IStartUp starts, EnumGamePackageMode mode) //this means something needs to put into here.
        {
            BuildXAML(starts, mode);
        }
        public override Task HandleAsync(LoadEventModel message)
        {
            ChinazoSaveInfo saveRoot = OurContainer!.Resolve<ChinazoSaveInfo>();
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _playerHandWPF!.LoadList(ThisMod!.PlayerHand1!, ts.TagUsed); // i think
            _discardGPile!.Init(ThisMod.Pile1!, ts.TagUsed); // may have to be here (well see)
            _discardGPile.StartListeningDiscardPile(); // its the main one.
            _deckGPile!.Init(ThisMod.Deck1!, ts.TagUsed); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();
            _tempG!.Init(ThisMod!.TempSets!, ts.TagUsed);
            _mainG!.Init(ThisMod!.MainSets!, ts.TagUsed);
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            ChinazoSaveInfo saveRoot = OurContainer!.Resolve<ChinazoSaveInfo>();
            _thisScore!.UpdateLists(saveRoot.PlayerList);
            _playerHandWPF!.UpdateList(ThisMod!.PlayerHand1!);
            _deckGPile!.UpdateDeck(ThisMod.Deck1!);
            _discardGPile!.UpdatePile(ThisMod.Pile1!);
            _tempG!.Update(ThisMod!.TempSets!);
            _mainG!.Update(ThisMod!.MainSets!);
            return Task.CompletedTask;
        }
        private BaseDeckWPF<ChinazoCard, ts, DeckOfCardsWPF<ChinazoCard>>? _deckGPile;
        private BasePileWPF<ChinazoCard, ts, DeckOfCardsWPF<ChinazoCard>>? _discardGPile;
        private ScoreBoardWPF? _thisScore;
        private BaseHandWPF<ChinazoCard, ts, DeckOfCardsWPF<ChinazoCard>>? _playerHandWPF;
        private TempRummySetsWPF<EnumSuitList, EnumColorList, ChinazoCard, ts, DeckOfCardsWPF<ChinazoCard>>? _tempG;
        private MainRummySetsWPF<EnumSuitList, EnumColorList, ChinazoCard, ts, DeckOfCardsWPF<ChinazoCard>, PhaseSet, SavedSet>? _mainG;
        protected async override void AfterGameButton()
        {
            StackPanel thisStack = new StackPanel(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            _deckGPile = new BaseDeckWPF<ChinazoCard, ts, DeckOfCardsWPF<ChinazoCard>>();
            _discardGPile = new BasePileWPF<ChinazoCard, ts, DeckOfCardsWPF<ChinazoCard>>();
            _playerHandWPF = new BaseHandWPF<ChinazoCard, ts, DeckOfCardsWPF<ChinazoCard>>();
            _tempG = new TempRummySetsWPF<EnumSuitList, EnumColorList, ChinazoCard, ts, DeckOfCardsWPF<ChinazoCard>>();
            _mainG = new MainRummySetsWPF<EnumSuitList, EnumColorList, ChinazoCard, ts, DeckOfCardsWPF<ChinazoCard>, PhaseSet, SavedSet>();
            GameButton!.HorizontalAlignment = HorizontalAlignment.Center;
            GameButton.VerticalAlignment = VerticalAlignment.Center;
            RoundButton!.HorizontalAlignment = HorizontalAlignment.Center;
            RoundButton.VerticalAlignment = VerticalAlignment.Center;
            thisStack.Children.Add(GameButton);
            thisStack.Children.Add(RoundButton);
            Grid finalGrid = new Grid();
            AddLeftOverRow(finalGrid, 20); // has to be this way because of scoreboard.
            AddLeftOverRow(finalGrid, 80);
            thisStack.Children.Add(finalGrid);
            Grid firstGrid = new Grid();
            AddLeftOverColumn(firstGrid, 50); // 50 was too much.  if there is scrolling, i guess okay.
            AddLeftOverColumn(firstGrid, 10); // for buttons (can change if necessary)
            AddAutoColumns(firstGrid, 1); // maybe 1 (well see)
            AddLeftOverColumn(firstGrid, 20); // for other details
            AddLeftOverColumn(firstGrid, 20); // for scoreboard
            _deckGPile.HorizontalAlignment = HorizontalAlignment.Left;
            _deckGPile.VerticalAlignment = VerticalAlignment.Top;
            _discardGPile.HorizontalAlignment = HorizontalAlignment.Left;
            _discardGPile.VerticalAlignment = VerticalAlignment.Top;
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            AddControlToGrid(firstGrid, otherStack, 0, 2);
            AddControlToGrid(firstGrid, _playerHandWPF, 0, 0);
            StackPanel firstStack = new StackPanel();
            StackPanel secondStack = new StackPanel();
            secondStack.Orientation = Orientation.Horizontal;
            firstStack.Children.Add(secondStack);
            var thisBut = GetGamingButton("Pass", nameof(ChinazoViewModel.PassCommand));
            secondStack.Children.Add(thisBut);
            thisBut = GetGamingButton("Take", nameof(ChinazoViewModel.TakeCommand));
            secondStack.Children.Add(thisBut);
            thisBut = GetGamingButton("Lay Down Sets", nameof(ChinazoViewModel.InitSetsCommand));
            firstStack.Children.Add(thisBut);
            AddControlToGrid(firstGrid, firstStack, 0, 1);
            _thisScore = new ScoreBoardWPF();
            _thisScore.AddColumn("Cards Left", false, nameof(ChinazoPlayerItem.ObjectCount)); //very common.
            _thisScore.AddColumn("Current Score", false, nameof(ChinazoPlayerItem.CurrentScore), rightMargin: 5);
            _thisScore.AddColumn("Total Score", false, nameof(ChinazoPlayerItem.TotalScore));
            AddControlToGrid(firstGrid, _thisScore, 0, 4);
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(ChinazoViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(ChinazoViewModel.Status));
            firstInfo.AddRow("Other Turn", nameof(ChinazoViewModel.OtherLabel));
            firstInfo.AddRow("Phase", nameof(ChinazoViewModel.PhaseData));
            AddControlToGrid(firstGrid, firstInfo.GetContent, 0, 3);
            AddControlToGrid(finalGrid, firstGrid, 0, 0); // i think
            _tempG.Height = 700;
            StackPanel thirdStack = new StackPanel();
            thirdStack.Orientation = Orientation.Horizontal;
            thirdStack.Children.Add(_tempG);
            thirdStack.Children.Add(_mainG);
            AddControlToGrid(finalGrid, thirdStack, 1, 0);
            MainGrid!.Children.Add(thisStack);
            _deckGPile.Margin = new Thickness(5, 5, 5, 5);
            _discardGPile.Margin = new Thickness(5, 5, 5, 5);
            AddRestoreCommand(thisStack); //usually to this.  can be to another control if needed.
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterType<BasicGameLoader<ChinazoPlayerItem, ChinazoSaveInfo>>(); //i think basic game loader gets done here still.
            OurContainer!.RegisterCommonRegularCards<ChinazoViewModel, ChinazoCard>(registerCommonProportions: false, customDeck: true);
            OurContainer.RegisterSingleton<IDeckCount, CustomDeck>(); //forgot to use a custom deck for this one.
            OurContainer.RegisterSingleton<IRegularAceCalculator, RegularAceHighCalculator>();
            OurContainer.RegisterSingleton<IProportionImage, SmallDrawableProportion>(ts.TagUsed);
        }
    }
}