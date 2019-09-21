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
using MonasteryCardGameCP;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //just in case
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace MonasteryCardGameWPF
{
    public class GamePage : MultiPlayerWindow<MonasteryCardGameViewModel, MonasteryCardGamePlayerItem, MonasteryCardGameSaveInfo>
    {
        public GamePage(IStartUp starts, EnumGamePackageMode mode) //this means something needs to put into here.
        {
            BuildXAML(starts, mode);
        }
        public override Task HandleAsync(LoadEventModel message)
        {
            MonasteryCardGameSaveInfo saveRoot = OurContainer!.Resolve<MonasteryCardGameSaveInfo>();
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _playerHandWPF!.LoadList(ThisMod!.PlayerHand1!, ts.TagUsed); // i think
            _discardGPile!.Init(ThisMod.Pile1!, ts.TagUsed); // may have to be here (well see)
            _discardGPile.StartListeningDiscardPile(); // its the main one.
            _deckGPile!.Init(ThisMod.Deck1!, ts.TagUsed); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();
            _tempG!.Init(ThisMod!.TempSets!, ts.TagUsed);
            _mainG!.Init(ThisMod.MainSets!, ts.TagUsed);
            _missionWPF!.Init(ThisMod);
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            MonasteryCardGameSaveInfo saveRoot = OurContainer!.Resolve<MonasteryCardGameSaveInfo>();
            _thisScore!.UpdateLists(saveRoot.PlayerList);
            _playerHandWPF!.UpdateList(ThisMod!.PlayerHand1!);
            _deckGPile!.UpdateDeck(ThisMod.Deck1!);
            _discardGPile!.UpdatePile(ThisMod.Pile1!);
            _tempG!.Update(ThisMod!.TempSets!);
            _mainG!.Update(ThisMod.MainSets!);
            return Task.CompletedTask;
        }
        private BaseDeckWPF<MonasteryCardInfo, ts, DeckOfCardsWPF<MonasteryCardInfo>>? _deckGPile;
        private BasePileWPF<MonasteryCardInfo, ts, DeckOfCardsWPF<MonasteryCardInfo>>? _discardGPile;
        private ScoreBoardWPF? _thisScore;
        private BaseHandWPF<MonasteryCardInfo, ts, DeckOfCardsWPF<MonasteryCardInfo>>? _playerHandWPF;
        private TempRummySetsWPF<EnumSuitList, EnumColorList, MonasteryCardInfo, ts, DeckOfCardsWPF<MonasteryCardInfo>>? _tempG;
        private MainRummySetsWPF<EnumSuitList, EnumColorList, MonasteryCardInfo, ts, DeckOfCardsWPF<MonasteryCardInfo>, RummySet, SavedSet>? _mainG;
        private MissionUI? _missionWPF;
        protected async override void AfterGameButton()
        {
            StackPanel thisStack = new StackPanel(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            _deckGPile = new BaseDeckWPF<MonasteryCardInfo, ts, DeckOfCardsWPF<MonasteryCardInfo>>();
            _discardGPile = new BasePileWPF<MonasteryCardInfo, ts, DeckOfCardsWPF<MonasteryCardInfo>>();
            _thisScore = new ScoreBoardWPF();
            _playerHandWPF = new BaseHandWPF<MonasteryCardInfo, ts, DeckOfCardsWPF<MonasteryCardInfo>>();
            _tempG = new TempRummySetsWPF<EnumSuitList, EnumColorList, MonasteryCardInfo, ts, DeckOfCardsWPF<MonasteryCardInfo>>();
            _mainG = new MainRummySetsWPF<EnumSuitList, EnumColorList, MonasteryCardInfo, ts, DeckOfCardsWPF<MonasteryCardInfo>, RummySet, SavedSet>();
            _missionWPF = new MissionUI();
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
            AddControlToGrid(firstGrid, otherStack, 0, 1);
            AddControlToGrid(firstGrid, _playerHandWPF, 0, 0);
            _thisScore.UseAbbreviationForTrueFalse = true;
            _thisScore.AddColumn("Cards Left", false, nameof(MonasteryCardGamePlayerItem.ObjectCount));
            _thisScore.AddColumn("Finished Mission", false, nameof(MonasteryCardGamePlayerItem.FinishedCurrentMission), useTrueFalse: true);
            int x;
            for (x = 1; x <= 9; x++)
                _thisScore.AddColumn("Mission" + x, false, "Mission" + x + "Completed", useTrueFalse: true);
            AddControlToGrid(firstGrid, _thisScore, 0, 3); // use 3 instead of 4 here.
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(MonasteryCardGameViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(MonasteryCardGameViewModel.Status));
            AddControlToGrid(firstGrid, firstInfo.GetContent, 0, 2);
            AddControlToGrid(finalGrid, firstGrid, 0, 0);
            _tempG.Height = 700;
            _mainG.Height = 700; //i think.
            Grid bottomGrid = new Grid();
            AddAutoColumns(bottomGrid, 1);
            AddLeftOverColumn(bottomGrid, 40);
            AddLeftOverColumn(bottomGrid, 60); // most important is the last one.  can adjust as needed though.   especially on tablets
            AddControlToGrid(bottomGrid, _tempG, 0, 0);
            AddControlToGrid(bottomGrid, _mainG, 0, 1);
            AddControlToGrid(bottomGrid, _missionWPF, 0, 2);
            AddControlToGrid(finalGrid, bottomGrid, 1, 0);
            MainGrid!.Children.Add(thisStack);
            _deckGPile.Margin = new Thickness(5, 5, 5, 5);
            _discardGPile.Margin = new Thickness(5, 5, 5, 5);
            AddRestoreCommand(thisStack); //usually to this.  can be to another control if needed.
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterType<BasicGameLoader<MonasteryCardGamePlayerItem, MonasteryCardGameSaveInfo>>(); //i think basic game loader gets done here still.
            OurContainer!.RegisterCommonRegularCards<MonasteryCardGameViewModel, MonasteryCardInfo>(customDeck: true, registerCommonProportions: false);
            OurContainer.RegisterSingleton<IProportionImage, SmallDrawableProportion>(ts.TagUsed);
            OurContainer.RegisterSingleton<IDeckCount, CustomDeck>();
            OurContainer.RegisterSingleton<IRegularAceCalculator, RegularLowAceCalculator>();
        }
    }
}