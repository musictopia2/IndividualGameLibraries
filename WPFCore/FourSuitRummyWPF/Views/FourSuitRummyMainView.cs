using BasicControlsAndWindowsCore.Controls.NavigationControls;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIWPFLibrary.BasicControls.GameFrames;
using BasicGamingUIWPFLibrary.BasicControls.Misc;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using BasicGamingUIWPFLibrary.BasicControls.SingleCardFrames;
using BasicGamingUIWPFLibrary.GameGraphics.Cards;
using BasicGamingUIWPFLibrary.Helpers;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using FourSuitRummyCP.Data;
using FourSuitRummyCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows;
using System.Windows.Controls;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //usually needs this as well for grid helpers.
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;

namespace FourSuitRummyWPF.Views
{
    public class FourSuitRummyMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly FourSuitRummyVMData _model;
        private readonly BaseDeckWPF<RegularRummyCard, ts, DeckOfCardsWPF<RegularRummyCard>> _deckGPile;
        private readonly BasePileWPF<RegularRummyCard, ts, DeckOfCardsWPF<RegularRummyCard>> _discardGPile;

        private readonly ScoreBoardWPF _score;
        private readonly BaseHandWPF<RegularRummyCard, ts, DeckOfCardsWPF<RegularRummyCard>> _playerHandWPF;
        private readonly TempRummySetsWPF<EnumSuitList, EnumColorList, RegularRummyCard, ts, DeckOfCardsWPF<RegularRummyCard>> _tempG;
        public FourSuitRummyMainView(IEventAggregator aggregator,
            TestOptions test,
            FourSuitRummyVMData model
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
            StackPanel mainStack = new StackPanel();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer()
                {
                    Name = nameof(FourSuitRummyMainViewModel.RestoreScreen)
                };
            }

            Grid finalGrid = new Grid();
            AddLeftOverRow(finalGrid, 20); // has to be this way because of scoreboard.
            AddLeftOverRow(finalGrid, 80);
            mainStack.Children.Add(finalGrid);



            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.

            Grid firstGrid = new Grid();
            AddLeftOverColumn(firstGrid, 60); // 50 was too much.  if there is scrolling, i guess okay.
            AddLeftOverColumn(firstGrid, 10); // for buttons (can change if necessary)
            AddAutoColumns(firstGrid, 1); // maybe 1 (well see)
            AddLeftOverColumn(firstGrid, 15); // for other details
            AddLeftOverColumn(firstGrid, 30); // for scoreboard
            _deckGPile.HorizontalAlignment = HorizontalAlignment.Left;
            _deckGPile.VerticalAlignment = VerticalAlignment.Top;
            _discardGPile.HorizontalAlignment = HorizontalAlignment.Left;
            _discardGPile.VerticalAlignment = VerticalAlignment.Top;
            AddControlToGrid(firstGrid, otherStack, 0, 2); // i think
            AddControlToGrid(firstGrid, _playerHandWPF, 0, 0); // i think
            var thisBut = GetGamingButton("Play Sets", nameof(FourSuitRummyMainViewModel.PlaySetsAsync));
            AddControlToGrid(firstGrid, thisBut, 0, 1);
            _score.AddColumn("Cards Left", true, nameof(FourSuitRummyPlayerItem.ObjectCount)); //very common.
            _score.AddColumn("Total Score", true, nameof(FourSuitRummyPlayerItem.TotalScore));
            AddControlToGrid(firstGrid, _score, 0, 4);
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(FourSuitRummyMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(FourSuitRummyMainViewModel.Status));
            AddControlToGrid(firstGrid, firstInfo.GetContent, 0, 3);
            AddControlToGrid(finalGrid, firstGrid, 0, 0); // i think
            _tempG.Height = 700;
            StackPanel thirdStack = new StackPanel();
            thirdStack.Orientation = Orientation.Horizontal;
            thirdStack.Children.Add(_tempG);
            Grid setGrid = new Grid();

            AddLeftOverColumn(setGrid, 50);
            AddLeftOverColumn(setGrid, 50);
            AddPixelRow(setGrid, 700); // i think this one needs that.
            thirdStack.Children.Add(setGrid);

            ParentSingleUIContainer parent = new ParentSingleUIContainer()
            {
                Name = nameof(FourSuitRummyMainViewModel.YourSetsScreen)
            };
            AddControlToGrid(setGrid, parent, 0, 0);
            parent = new ParentSingleUIContainer()
            {
                Name = nameof(FourSuitRummyMainViewModel.OpponentSetsScreen)
            };
            AddControlToGrid(setGrid, parent, 0, 1);

            AddControlToGrid(finalGrid, thirdStack, 1, 0); // i think


            _deckGPile.Margin = new Thickness(5, 5, 5, 5);

            _discardGPile.Margin = new Thickness(5, 5, 5, 5);


            if (restoreP != null)
            {
                //todo:  figure out where to place the restore ui if there is a restore option.
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }
            Content = mainStack;

        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {

            GamePackageViewModelBinder.ManuelElements.Clear(); //often times i have to add manually.

            FourSuitRummySaveInfo save = cons!.Resolve<FourSuitRummySaveInfo>(); //usually needs this part for multiplayer games.

            _score!.LoadLists(save.PlayerList);
            _playerHandWPF!.LoadList(_model.PlayerHand1!, ts.TagUsed); // i think
            _discardGPile!.Init(_model.Pile1!, ts.TagUsed); // may have to be here (well see)
            _discardGPile.StartListeningDiscardPile(); // its the main one.

            _deckGPile!.Init(_model.Deck1!, ts.TagUsed); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();
            _tempG.Init(_model.TempSets, ts.TagUsed);
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
