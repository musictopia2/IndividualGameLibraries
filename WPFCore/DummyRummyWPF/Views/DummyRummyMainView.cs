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
using DummyRummyCP.Data;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicControlsAndWindowsCore.Controls.NavigationControls;
using DummyRummyCP.ViewModels;
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
using DummyRummyCP.Logic;

namespace DummyRummyWPF.Views
{
    public class DummyRummyMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly DummyRummyVMData _model;
        private readonly BaseDeckWPF<RegularRummyCard, ts, DeckOfCardsWPF<RegularRummyCard>> _deckGPile;
        private readonly BasePileWPF<RegularRummyCard, ts, DeckOfCardsWPF<RegularRummyCard>> _discardGPile;

        private readonly ScoreBoardWPF _score;
        private readonly BaseHandWPF<RegularRummyCard, ts, DeckOfCardsWPF<RegularRummyCard>> _playerHandWPF;

        private readonly TempRummySetsWPF<EnumSuitList, EnumColorList, RegularRummyCard, ts, DeckOfCardsWPF<RegularRummyCard>> _tempG;
        private readonly MainRummySetsWPF<EnumSuitList, EnumColorList, RegularRummyCard, ts, DeckOfCardsWPF<RegularRummyCard>, DummySet, SavedSet> _mainG;


        public DummyRummyMainView(IEventAggregator aggregator,
            TestOptions test,
            DummyRummyVMData model
            )
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);

            _deckGPile = new BaseDeckWPF<RegularRummyCard, ts, DeckOfCardsWPF<RegularRummyCard>>();
            _discardGPile = new BasePileWPF<RegularRummyCard, ts, DeckOfCardsWPF<RegularRummyCard>>();
            _score = new ScoreBoardWPF();
            _playerHandWPF = new BaseHandWPF<RegularRummyCard, ts, DeckOfCardsWPF<RegularRummyCard>>();
            _tempG = new TempRummySetsWPF<EnumSuitList, EnumColorList, RegularRummyCard, ts, DeckOfCardsWPF<RegularRummyCard>>();
            _mainG = new MainRummySetsWPF<EnumSuitList, EnumColorList, RegularRummyCard, ts, DeckOfCardsWPF<RegularRummyCard>, DummySet, SavedSet>();
            StackPanel mainStack = new StackPanel();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer()
                {
                    Name = nameof(DummyRummyMainViewModel.RestoreScreen)
                };
            }
            Grid finalGrid = new Grid();
            AddLeftOverRow(finalGrid, 20); // has to be this way because of scoreboard.
            AddLeftOverRow(finalGrid, 80);
            mainStack.Children.Add(finalGrid);
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
            var button = GetGamingButton("Lay" + Constants.vbCrLf + "Down", nameof(DummyRummyMainViewModel.LayDownSetsAsync));
            StackPanel tempStack = new StackPanel();
            tempStack.Orientation = Orientation.Horizontal;
            tempStack.Children.Add(button);
            button = GetGamingButton("Back", nameof(DummyRummyMainViewModel.Back));
            button.FontSize -= 4;
            tempStack.Children.Add(button);
            AddControlToGrid(firstGrid, tempStack, 0, 1);
            _score.AddColumn("Cards Left", true, nameof(DummyRummyPlayerItem.ObjectCount)); //very common.
            _score.AddColumn("Current Score", true, nameof(DummyRummyPlayerItem.CurrentScore));
            _score.AddColumn("Total Score", true, nameof(DummyRummyPlayerItem.TotalScore));
            AddControlToGrid(firstGrid, _score, 0, 4);
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(DummyRummyMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(DummyRummyMainViewModel.Status));
            firstInfo.AddRow("Up To", nameof(DummyRummyMainViewModel.UpTo));
            AddControlToGrid(firstGrid, firstInfo.GetContent, 0, 3);
            AddControlToGrid(finalGrid, firstGrid, 0, 0); // i think
            _tempG.Height = 700;
            StackPanel thirdStack = new StackPanel();
            thirdStack.Orientation = Orientation.Horizontal;
            thirdStack.Children.Add(_tempG);
            _mainG.Height = 700; // try this way.
            thirdStack.Children.Add(_mainG);
            AddControlToGrid(finalGrid, thirdStack, 1, 0); // i think

            _deckGPile.Margin = new Thickness(5, 5, 5, 5);

            _discardGPile.Margin = new Thickness(5, 5, 5, 5);


            if (restoreP != null)
            {
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }
            Content = mainStack;

        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {

            GamePackageViewModelBinder.ManuelElements.Clear(); //often times i have to add manually.

            DummyRummySaveInfo save = cons!.Resolve<DummyRummySaveInfo>(); //usually needs this part for multiplayer games.

            _score!.LoadLists(save.PlayerList);
            _playerHandWPF!.LoadList(_model.PlayerHand1!, ts.TagUsed); // i think
            _discardGPile!.Init(_model.Pile1!, ts.TagUsed); // may have to be here (well see)
            _discardGPile.StartListeningDiscardPile(); // its the main one.

            _deckGPile!.Init(_model.Deck1!, ts.TagUsed); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();

            _tempG!.Init(_model!.TempSets!, ts.TagUsed);
            _mainG!.Init(_model!.MainSets!, ts.TagUsed);

            return this.RefreshBindingsAsync(_aggregator);
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
