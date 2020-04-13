using System;
using System.Text;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using System.Linq;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using fs = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.FileHelpers;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
using System.Windows.Controls;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using CommonBasicStandardLibraries.Messenging;
using BasicGameFrameworkLibrary.BasicEventModels;
using MonasteryCardGameCP.Data;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicControlsAndWindowsCore.Controls.NavigationControls;
using MonasteryCardGameCP.ViewModels;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //usually needs this as well for grid helpers.
using BasicGamingUIWPFLibrary.Helpers;
using System.Windows;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;
using BasicGamingUIWPFLibrary.BasicControls.SingleCardFrames;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGamingUIWPFLibrary.GameGraphics.Cards;
using BasicGamingUIWPFLibrary.BasicControls.GameFrames;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using BasicGamingUIWPFLibrary.BasicControls.Misc;
using MonasteryCardGameCP.Logic;

namespace MonasteryCardGameWPF.Views
{
    public class MonasteryCardGameMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        //TODO:  has to build the main page.  no need for new game since that is completely separated out into another class.
        private readonly IEventAggregator _aggregator;
        private readonly MonasteryCardGameVMData _model;
        private readonly BaseDeckWPF<MonasteryCardInfo, ts, DeckOfCardsWPF<MonasteryCardInfo>> _deckGPile;
        private readonly BasePileWPF<MonasteryCardInfo, ts, DeckOfCardsWPF<MonasteryCardInfo>> _discardGPile;

        private readonly ScoreBoardWPF _score;
        private readonly BaseHandWPF<MonasteryCardInfo, ts, DeckOfCardsWPF<MonasteryCardInfo>> _playerHandWPF;

        private readonly TempRummySetsWPF<EnumSuitList, EnumColorList, MonasteryCardInfo, ts, DeckOfCardsWPF<MonasteryCardInfo>> _tempG;
        private readonly MainRummySetsWPF<EnumSuitList, EnumColorList, MonasteryCardInfo, ts, DeckOfCardsWPF<MonasteryCardInfo>, RummySet, SavedSet> _mainG;
        private readonly MissionUI _missionWPF;

        public MonasteryCardGameMainView(IEventAggregator aggregator,
            TestOptions test,
            MonasteryCardGameVMData model
            )
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);

            _deckGPile = new BaseDeckWPF<MonasteryCardInfo, ts, DeckOfCardsWPF<MonasteryCardInfo>>();
            _discardGPile = new BasePileWPF<MonasteryCardInfo, ts, DeckOfCardsWPF<MonasteryCardInfo>>();
            _score = new ScoreBoardWPF();
            _playerHandWPF = new BaseHandWPF<MonasteryCardInfo, ts, DeckOfCardsWPF<MonasteryCardInfo>>();

            _tempG = new TempRummySetsWPF<EnumSuitList, EnumColorList, MonasteryCardInfo, ts, DeckOfCardsWPF<MonasteryCardInfo>>();
            _mainG = new MainRummySetsWPF<EnumSuitList, EnumColorList, MonasteryCardInfo, ts, DeckOfCardsWPF<MonasteryCardInfo>, RummySet, SavedSet>();
            _missionWPF = new MissionUI();


            StackPanel mainStack = new StackPanel();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer()
                {
                    Name = nameof(MonasteryCardGameMainViewModel.RestoreScreen)
                };
            }
            Grid finalGrid = new Grid();
            AddLeftOverRow(finalGrid, 20); // has to be this way because of scoreboard.
            AddLeftOverRow(finalGrid, 80);
            mainStack.Children.Add(finalGrid);
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
            _score.UseAbbreviationForTrueFalse = true;
            _score.AddColumn("Cards Left", false, nameof(MonasteryCardGamePlayerItem.ObjectCount));
            _score.AddColumn("Finished Mission", false, nameof(MonasteryCardGamePlayerItem.FinishedCurrentMission), useTrueFalse: true);
            int x;
            for (x = 1; x <= 9; x++)
                _score.AddColumn("Mission" + x, false, "Mission" + x + "Completed", useTrueFalse: true);
            AddControlToGrid(firstGrid, _score, 0, 3); // use 3 instead of 4 here.
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(MonasteryCardGameMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(MonasteryCardGameMainViewModel.Status));
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

            _deckGPile.Margin = new Thickness(5, 5, 5, 5);

            _discardGPile.Margin = new Thickness(5, 5, 5, 5);


            if (restoreP != null)
            {
                //todo:  figure out where to place the restore ui if there is a restore option.
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }
            Content = mainStack;

        }
        async Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
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
            await _missionWPF!.InitAsync(_model, this, _aggregator);

            //await this.RefreshBindingsAsync(_aggregator);
        }

        

        Task IUIView.TryActivateAsync()
        {
            return Task.CompletedTask;
        }

        Task IUIView.TryCloseAsync()
        {
            return Task.CompletedTask;
        }
    }
}
