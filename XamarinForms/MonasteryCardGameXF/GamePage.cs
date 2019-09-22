using BasicGameFramework.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.Interfaces;;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Pages;
using BaseGPXPagesAndControlsXF.BasicControls.GameFrames;
using BaseGPXPagesAndControlsXF.BasicControls.Misc;
using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using BaseGPXPagesAndControlsXF.BasicControls.SingleCardFrames;
using BaseGPXPagesAndControlsXF.GameGraphics.Cards;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using BasicGameFramework.RegularDeckOfCards;
using MonasteryCardGameCP;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BasicXFControlsAndPages.Helpers.GridHelper; //just in case
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace MonasteryCardGameXF
{
    public class GamePage : MultiPlayerPage<MonasteryCardGameViewModel, MonasteryCardGamePlayerItem, MonasteryCardGameSaveInfo>
    {
        public GamePage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode) { }
        public override Task HandleAsync(LoadEventModel message)
        {
            MonasteryCardGameSaveInfo saveRoot = OurContainer!.Resolve<MonasteryCardGameSaveInfo>();
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _playerHand!.LoadList(ThisMod!.PlayerHand1!, ts.TagUsed); // i think
            _discardGPile!.Init(ThisMod.Pile1!, ts.TagUsed); // may have to be here (well see)
            _discardGPile.StartListeningDiscardPile(); // its the main one.
            _deckGPile!.Init(ThisMod.Deck1!, ts.TagUsed); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();
            _tempG!.Init(ThisMod!.TempSets!, ts.TagUsed);
            _mainG!.Init(ThisMod.MainSets!, ts.TagUsed);
            _mission!.Init(ThisMod);
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            MonasteryCardGameSaveInfo saveRoot = OurContainer!.Resolve<MonasteryCardGameSaveInfo>();
            _thisScore!.UpdateLists(saveRoot.PlayerList);
            _playerHand!.UpdateList(ThisMod!.PlayerHand1!);
            _deckGPile!.UpdateDeck(ThisMod.Deck1!);
            _discardGPile!.UpdatePile(ThisMod.Pile1!);
            _tempG!.Update(ThisMod!.TempSets!);
            _mainG!.Update(ThisMod.MainSets!);
            return Task.CompletedTask;
        }
        private BaseDeckXF<MonasteryCardInfo, ts, DeckOfCardsXF<MonasteryCardInfo>>? _deckGPile;
        private BasePileXF<MonasteryCardInfo, ts, DeckOfCardsXF<MonasteryCardInfo>>? _discardGPile;
        private ScoreBoardXF? _thisScore;
        private BaseHandXF<MonasteryCardInfo, ts, DeckOfCardsXF<MonasteryCardInfo>>? _playerHand;
        private TempRummySetsXF<EnumSuitList, EnumColorList, MonasteryCardInfo, ts, DeckOfCardsXF<MonasteryCardInfo>>? _tempG;
        private MainRummySetsXF<EnumSuitList, EnumColorList, MonasteryCardInfo, ts, DeckOfCardsXF<MonasteryCardInfo>, RummySet, SavedSet>? _mainG;
        private MissionUI? _mission;
        protected override async Task AfterGameButtonAsync()
        {
            BasicSetUp();
            //anything needed for ui is here.
            _deckGPile = new BaseDeckXF<MonasteryCardInfo, ts, DeckOfCardsXF<MonasteryCardInfo>>();
            _discardGPile = new BasePileXF<MonasteryCardInfo, ts, DeckOfCardsXF<MonasteryCardInfo>>();
            _thisScore = new ScoreBoardXF();
            _playerHand = new BaseHandXF<MonasteryCardInfo, ts, DeckOfCardsXF<MonasteryCardInfo>>();
            _tempG = new TempRummySetsXF<EnumSuitList, EnumColorList, MonasteryCardInfo, ts, DeckOfCardsXF<MonasteryCardInfo>>();
            _mainG = new MainRummySetsXF<EnumSuitList, EnumColorList, MonasteryCardInfo, ts, DeckOfCardsXF<MonasteryCardInfo>, RummySet, SavedSet>();
            _mission = new MissionUI();
            Grid finalGrid = new Grid();
            MainGrid!.Children.Add(finalGrid);
            AddAutoRows(finalGrid, 3);
            Grid firstGrid = new Grid();
            AddLeftOverColumn(firstGrid, 35);
            AddAutoColumns(firstGrid, 1);
            AddLeftOverColumn(firstGrid, 15);
            AddLeftOverColumn(firstGrid, 30);
            RoundButton!.HorizontalOptions = LayoutOptions.Center;
            RoundButton.VerticalOptions = LayoutOptions.Center;
            GameButton!.HorizontalOptions = LayoutOptions.Center;
            GameButton.VerticalOptions = LayoutOptions.Center;
            AddControlToGrid(finalGrid, GameButton, 0, 0);
            AddControlToGrid(finalGrid, RoundButton, 0, 0);
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            AddControlToGrid(firstGrid, otherStack, 0, 1);
            AddControlToGrid(firstGrid, _playerHand, 0, 0);
            _thisScore.UseAbbreviationForTrueFalse = true;
            _thisScore.AddColumn("Cards", false, nameof(MonasteryCardGamePlayerItem.ObjectCount));
            _thisScore.AddColumn("Done", false, nameof(MonasteryCardGamePlayerItem.FinishedCurrentMission), useTrueFalse: true);
            int x;
            for (x = 1; x <= 9; x++)
                _thisScore.AddColumn("M" + x, false, "Mission" + x + "Completed", useTrueFalse: true);
            AddControlToGrid(firstGrid, _thisScore, 0, 3); // use 3 instead of 4 here.
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(MonasteryCardGameViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(MonasteryCardGameViewModel.Status));
            AddControlToGrid(firstGrid, firstInfo.GetContent, 0, 2);
            AddControlToGrid(finalGrid, firstGrid, 1, 0);
            _tempG.Divider = 1.1;
            Grid bottomGrid = new Grid();
            AddAutoColumns(bottomGrid, 1);
            AddLeftOverColumn(bottomGrid, 35);
            AddLeftOverColumn(bottomGrid, 65);
            AddControlToGrid(bottomGrid, _tempG, 0, 0);
            AddControlToGrid(bottomGrid, _mainG, 0, 1);
            AddControlToGrid(bottomGrid, _mission, 0, 2);
            AddControlToGrid(finalGrid, bottomGrid, 2, 0);
            _deckGPile.Margin = new Thickness(5, 5, 5, 5);
            _discardGPile.Margin = new Thickness(5, 5, 5, 5);
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