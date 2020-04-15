using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIXFLibrary.BasicControls.GameFrames;
using BasicGamingUIXFLibrary.BasicControls.Misc;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicGamingUIXFLibrary.BasicControls.SingleCardFrames;
using BasicGamingUIXFLibrary.GameGraphics.Cards;
using BasicGamingUIXFLibrary.Helpers;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using MonasteryCardGameCP.Data;
using MonasteryCardGameCP.Logic;
using MonasteryCardGameCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BasicXFControlsAndPages.Helpers.GridHelper;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;

namespace MonasteryCardGameXF.Views
{
    public class MonasteryCardGameMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly MonasteryCardGameVMData _model;
        private readonly BaseDeckXF<MonasteryCardInfo, ts, DeckOfCardsXF<MonasteryCardInfo>> _deckGPile;
        private readonly BasePileXF<MonasteryCardInfo, ts, DeckOfCardsXF<MonasteryCardInfo>> _discardGPile;

        private readonly ScoreBoardXF _score;
        private readonly BaseHandXF<MonasteryCardInfo, ts, DeckOfCardsXF<MonasteryCardInfo>> _playerHandWPF;

        private readonly TempRummySetsXF<EnumSuitList, EnumColorList, MonasteryCardInfo, ts, DeckOfCardsXF<MonasteryCardInfo>> _tempG;
        private readonly MainRummySetsXF<EnumSuitList, EnumColorList, MonasteryCardInfo, ts, DeckOfCardsXF<MonasteryCardInfo>, RummySet, SavedSet> _mainG;
        private readonly MissionUI _missionXF;

        public MonasteryCardGameMainView(IEventAggregator aggregator,
            TestOptions test,
            MonasteryCardGameVMData model
            )
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);

            _deckGPile = new BaseDeckXF<MonasteryCardInfo, ts, DeckOfCardsXF<MonasteryCardInfo>>();
            _discardGPile = new BasePileXF<MonasteryCardInfo, ts, DeckOfCardsXF<MonasteryCardInfo>>();
            _score = new ScoreBoardXF();
            _playerHandWPF = new BaseHandXF<MonasteryCardInfo, ts, DeckOfCardsXF<MonasteryCardInfo>>();
            _tempG = new TempRummySetsXF<EnumSuitList, EnumColorList, MonasteryCardInfo, ts, DeckOfCardsXF<MonasteryCardInfo>>();
            _mainG = new MainRummySetsXF<EnumSuitList, EnumColorList, MonasteryCardInfo, ts, DeckOfCardsXF<MonasteryCardInfo>, RummySet, SavedSet>();
            _missionXF = new MissionUI();
            Grid finalGrid = new Grid();

            AddAutoRows(finalGrid, 1);
            AddLeftOverRow(finalGrid, 1);
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer(nameof(MonasteryCardGameMainViewModel.RestoreScreen));
            }

            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            _score.UseAbbreviationForTrueFalse = true;
            _score.AddColumn("Cards", false, nameof(MonasteryCardGamePlayerItem.ObjectCount));
            _score.AddColumn("Done", false, nameof(MonasteryCardGamePlayerItem.FinishedCurrentMission), useTrueFalse: true);
            int x;
            for (x = 1; x <= 9; x++)
                _score.AddColumn("M" + x, false, "Mission" + x + "Completed", useTrueFalse: true);
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(MonasteryCardGameMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(MonasteryCardGameMainViewModel.Status));


            Grid firstGrid = new Grid();
            AddLeftOverColumn(firstGrid, 35);
            AddAutoColumns(firstGrid, 1);
            AddLeftOverColumn(firstGrid, 15);
            AddLeftOverColumn(firstGrid, 30);
            AddControlToGrid(firstGrid, _score, 0, 3); // use 3 instead of 4 here.
            AddControlToGrid(firstGrid, otherStack, 0, 1);
            AddControlToGrid(firstGrid, _playerHandWPF, 0, 0);

            AddControlToGrid(firstGrid, firstInfo.GetContent, 0, 2);
            AddControlToGrid(finalGrid, firstGrid, 0, 0);
            _tempG.Divider = 1.1;
            Grid bottomGrid = new Grid();
            AddAutoColumns(bottomGrid, 1);
            AddLeftOverColumn(bottomGrid, 35);
            AddLeftOverColumn(bottomGrid, 65);
            AddControlToGrid(bottomGrid, _tempG, 0, 0);
            AddControlToGrid(bottomGrid, _mainG, 0, 1);
            AddControlToGrid(bottomGrid, _missionXF, 0, 2);
            AddControlToGrid(finalGrid, bottomGrid, 1, 0);


            _deckGPile.Margin = new Thickness(5, 5, 5, 5);

            _discardGPile.Margin = new Thickness(5, 5, 5, 5);

            if (restoreP != null)
            {
                otherStack.Children.Add(restoreP); //default add to grid but does not have to.
            }
            Content = finalGrid;
        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {
            GamePackageViewModelBinder.ManuelElements.Clear(); //often times i have to add manually.

            MonasteryCardGameSaveInfo save = cons!.Resolve<MonasteryCardGameSaveInfo>(); //usually needs this part for multiplayer games.

            _score!.LoadLists(save.PlayerList);
            _playerHandWPF!.LoadList(_model.PlayerHand1!, ts.TagUsed); // i think
            _discardGPile!.Init(_model.Pile1!, ts.TagUsed); // may have to be here (well see)
            _discardGPile.StartListeningDiscardPile(); // its the main one.

            _deckGPile!.Init(_model.Deck1!, ts.TagUsed); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();

            _tempG!.Init(_model!.TempSets!, ts.TagUsed);
            _mainG!.Init(_model.MainSets!, ts.TagUsed);
            return _missionXF!.InitAsync(_model, this, _aggregator);
        }


        object IUIView.DataContext
        {
            get => BindingContext;
            set => BindingContext = value;
        }
        Task IUIView.TryActivateAsync()
        {
            return Task.CompletedTask;
        }

        Task IUIView.TryCloseAsync()
        {
            _missionXF.Dispose();
            _tempG.Dispose();
            _mainG.Dispose();
            return Task.CompletedTask;
        }
    }
}
