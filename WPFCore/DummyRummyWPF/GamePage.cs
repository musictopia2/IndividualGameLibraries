using BasicGameFramework.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BaseGPXWindowsAndControlsCore.BaseWindows;
using BaseGPXWindowsAndControlsCore.BasicControls.GameFrames;
using BaseGPXWindowsAndControlsCore.BasicControls.Misc;
using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using BaseGPXWindowsAndControlsCore.BasicControls.SingleCardFrames;
using BaseGPXWindowsAndControlsCore.GameGraphics.Cards;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using BasicGameFramework.RegularDeckOfCards;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using DummyRummyCP;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //just in case
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace DummyRummyWPF
{
    public class GamePage : MultiPlayerWindow<DummyRummyViewModel, DummyRummyPlayerItem, DummyRummySaveInfo>
    {
        public GamePage(IStartUp starts, EnumGamePackageMode mode) //this means something needs to put into here.
        {
            BuildXAML(starts, mode);
        }
        public override Task HandleAsync(LoadEventModel message)
        {
            DummyRummySaveInfo saveRoot = OurContainer!.Resolve<DummyRummySaveInfo>();
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
            DummyRummySaveInfo saveRoot = OurContainer!.Resolve<DummyRummySaveInfo>();
            _thisScore!.UpdateLists(saveRoot.PlayerList);
            _playerHandWPF!.UpdateList(ThisMod!.PlayerHand1!);
            _deckGPile!.UpdateDeck(ThisMod.Deck1!);
            _discardGPile!.UpdatePile(ThisMod.Pile1!);
            _tempG!.Update(ThisMod!.TempSets!);
            _mainG!.Update(ThisMod!.MainSets!);
            return Task.CompletedTask;
        }
        private BaseDeckWPF<RegularRummyCard, ts, DeckOfCardsWPF<RegularRummyCard>>? _deckGPile;
        private BasePileWPF<RegularRummyCard, ts, DeckOfCardsWPF<RegularRummyCard>>? _discardGPile;
        private ScoreBoardWPF? _thisScore;
        private BaseHandWPF<RegularRummyCard, ts, DeckOfCardsWPF<RegularRummyCard>>? _playerHandWPF;
        private TempRummySetsWPF<EnumSuitList, EnumColorList, RegularRummyCard, ts, DeckOfCardsWPF<RegularRummyCard>>? _tempG;
        private MainRummySetsWPF<EnumSuitList, EnumColorList, RegularRummyCard, ts, DeckOfCardsWPF<RegularRummyCard>, DummySet, SavedSet>? _mainG;
        protected async override void AfterGameButton()
        {
            StackPanel thisStack = new StackPanel(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            _deckGPile = new BaseDeckWPF<RegularRummyCard, ts, DeckOfCardsWPF<RegularRummyCard>>();
            _discardGPile = new BasePileWPF<RegularRummyCard, ts, DeckOfCardsWPF<RegularRummyCard>>();
            _playerHandWPF = new BaseHandWPF<RegularRummyCard, ts, DeckOfCardsWPF<RegularRummyCard>>();
            _tempG = new TempRummySetsWPF<EnumSuitList, EnumColorList, RegularRummyCard, ts, DeckOfCardsWPF<RegularRummyCard>>();
            _mainG = new MainRummySetsWPF<EnumSuitList, EnumColorList, RegularRummyCard, ts, DeckOfCardsWPF<RegularRummyCard>, DummySet, SavedSet>();
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
            AddLeftOverColumn(firstGrid, 40); // 50 was too much.  if there is scrolling, i guess okay.
            AddLeftOverColumn(firstGrid, 10); // for buttons (can change if necessary)
            AddAutoColumns(firstGrid, 1); // maybe 1 (well see)
            AddLeftOverColumn(firstGrid, 15); // for other details
            AddLeftOverColumn(firstGrid, 30); // for scoreboard
            _deckGPile.HorizontalAlignment = HorizontalAlignment.Left;
            _deckGPile.VerticalAlignment = VerticalAlignment.Top;
            _discardGPile.HorizontalAlignment = HorizontalAlignment.Left;
            _discardGPile.VerticalAlignment = VerticalAlignment.Top;
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            AddControlToGrid(firstGrid, otherStack, 0, 2); // i think
            AddControlToGrid(firstGrid, _playerHandWPF, 0, 0); // i think
            var thisBut = GetGamingButton("Lay" + Constants.vbCrLf + "Down", nameof(DummyRummyViewModel.LayDownSetsCommand));
            StackPanel tempStack = new StackPanel();
            tempStack.Orientation = Orientation.Horizontal;
            tempStack.Children.Add(thisBut);
            thisBut = GetGamingButton("Back", nameof(DummyRummyViewModel.BackCommand));
            tempStack.Children.Add(thisBut);
            AddControlToGrid(firstGrid, tempStack, 0, 1);
            _thisScore = new ScoreBoardWPF();
            _thisScore.AddColumn("Cards Left", true, nameof(DummyRummyPlayerItem.ObjectCount)); //very common.
            _thisScore.AddColumn("Current Score", true, nameof(DummyRummyPlayerItem.CurrentScore));
            _thisScore.AddColumn("Total Score", true, nameof(DummyRummyPlayerItem.TotalScore));
            AddControlToGrid(firstGrid, _thisScore, 0, 4);
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(DummyRummyViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(DummyRummyViewModel.Status));
            firstInfo.AddRow("Up To", nameof(DummyRummyViewModel.UpTo));
            AddControlToGrid(firstGrid, firstInfo.GetContent, 0, 3);
            AddControlToGrid(finalGrid, firstGrid, 0, 0); // i think
            _tempG.Height = 700;
            StackPanel thirdStack = new StackPanel();
            thirdStack.Orientation = Orientation.Horizontal;
            thirdStack.Children.Add(_tempG);
            _mainG.Height = 700; // try this way.
            thirdStack.Children.Add(_mainG);
            AddControlToGrid(finalGrid, thirdStack, 1, 0); // i think
            MainGrid!.Children.Add(thisStack);
            _deckGPile.Margin = new Thickness(5, 5, 5, 5);
            _discardGPile.Margin = new Thickness(5, 5, 5, 5);
            AddRestoreCommand(thisStack); //usually to this.  can be to another control if needed.
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterType<BasicGameLoader<DummyRummyPlayerItem, DummyRummySaveInfo>>(); //i think basic game loader gets done here still.
            OurContainer!.RegisterCommonRegularCards<DummyRummyViewModel, RegularRummyCard>(registerCommonProportions: false);
            OurContainer.RegisterSingleton<IProportionImage, SmallDrawableProportion>(ts.TagUsed);
        }
    }
}