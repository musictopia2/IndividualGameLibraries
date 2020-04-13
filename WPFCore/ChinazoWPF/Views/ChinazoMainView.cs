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
using ChinazoCP.Data;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicControlsAndWindowsCore.Controls.NavigationControls;
using ChinazoCP.ViewModels;
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
using ChinazoCP.Logic;

namespace ChinazoWPF.Views
{
    public class ChinazoMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly ChinazoVMData _model;
        private readonly BaseDeckWPF<ChinazoCard, ts, DeckOfCardsWPF<ChinazoCard>> _deckGPile;
        private readonly BasePileWPF<ChinazoCard, ts, DeckOfCardsWPF<ChinazoCard>> _discardGPile;

        private readonly ScoreBoardWPF _score;
        private readonly BaseHandWPF<ChinazoCard, ts, DeckOfCardsWPF<ChinazoCard>> _playerHandWPF;
        private readonly TempRummySetsWPF<EnumSuitList, EnumColorList, ChinazoCard, ts, DeckOfCardsWPF<ChinazoCard>> _tempG;
        private readonly MainRummySetsWPF<EnumSuitList, EnumColorList, ChinazoCard, ts, DeckOfCardsWPF<ChinazoCard>, PhaseSet, SavedSet> _mainG;
        public ChinazoMainView(IEventAggregator aggregator,
            TestOptions test,
            ChinazoVMData model
            )
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);

            _deckGPile = new BaseDeckWPF<ChinazoCard, ts, DeckOfCardsWPF<ChinazoCard>>();
            _discardGPile = new BasePileWPF<ChinazoCard, ts, DeckOfCardsWPF<ChinazoCard>>();
            _score = new ScoreBoardWPF();
            _playerHandWPF = new BaseHandWPF<ChinazoCard, ts, DeckOfCardsWPF<ChinazoCard>>();
            _tempG = new TempRummySetsWPF<EnumSuitList, EnumColorList, ChinazoCard, ts, DeckOfCardsWPF<ChinazoCard>>();
            _mainG = new MainRummySetsWPF<EnumSuitList, EnumColorList, ChinazoCard, ts, DeckOfCardsWPF<ChinazoCard>, PhaseSet, SavedSet>();



            StackPanel mainStack = new StackPanel();

            Grid finalGrid = new Grid();
            AddLeftOverRow(finalGrid, 20); // has to be this way because of scoreboard.
            AddLeftOverRow(finalGrid, 80);
            mainStack.Children.Add(finalGrid);

            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer()
                {
                    Name = nameof(ChinazoMainViewModel.RestoreScreen)
                };
            }

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
            var button = GetGamingButton("Pass", nameof(ChinazoMainViewModel.PassAsync));
            secondStack.Children.Add(button);
            button = GetGamingButton("Take", nameof(ChinazoMainViewModel.TakeAsync));
            secondStack.Children.Add(button);
            button = GetGamingButton("Lay Down Sets", nameof(ChinazoMainViewModel.FirstSetsAsync));
            firstStack.Children.Add(button);
            AddControlToGrid(firstGrid, firstStack, 0, 1);
            _score.AddColumn("Cards Left", false, nameof(ChinazoPlayerItem.ObjectCount)); //very common.
            _score.AddColumn("Current Score", false, nameof(ChinazoPlayerItem.CurrentScore), rightMargin: 5);
            _score.AddColumn("Total Score", false, nameof(ChinazoPlayerItem.TotalScore));
            AddControlToGrid(firstGrid, _score, 0, 4);
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(ChinazoMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(ChinazoMainViewModel.Status));
            firstInfo.AddRow("Other Turn", nameof(ChinazoMainViewModel.OtherLabel));
            firstInfo.AddRow("Phase", nameof(ChinazoMainViewModel.PhaseData));
            AddControlToGrid(firstGrid, firstInfo.GetContent, 0, 3);
            AddControlToGrid(finalGrid, firstGrid, 0, 0); // i think
            _tempG.Height = 700;
            StackPanel thirdStack = new StackPanel();
            thirdStack.Orientation = Orientation.Horizontal;
            thirdStack.Children.Add(_tempG);
            thirdStack.Children.Add(_mainG);
            AddControlToGrid(finalGrid, thirdStack, 1, 0);


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

            ChinazoSaveInfo save = cons!.Resolve<ChinazoSaveInfo>(); //usually needs this part for multiplayer games.

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
