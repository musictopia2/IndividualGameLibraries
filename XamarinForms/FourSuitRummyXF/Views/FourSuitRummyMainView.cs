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
using FourSuitRummyCP.Data;
using FourSuitRummyCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using static BasicXFControlsAndPages.Helpers.GridHelper;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;

namespace FourSuitRummyXF.Views
{
    public class FourSuitRummyMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly FourSuitRummyVMData _model;
        private readonly BaseDeckXF<RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>> _deckGPile;
        private readonly BasePileXF<RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>> _discardGPile;

        private readonly ScoreBoardXF _score;
        private readonly BaseHandXF<RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>> _playerHandWPF;
        private readonly TempRummySetsXF<EnumSuitList, EnumColorList, RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>> _tempG;

        //private readonly Grid _setGrid;

        public FourSuitRummyMainView(IEventAggregator aggregator,
            TestOptions test,
            FourSuitRummyVMData model
            )
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);

            _deckGPile = new BaseDeckXF<RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>>();
            _discardGPile = new BasePileXF<RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>>();
            _score = new ScoreBoardXF();
            _playerHandWPF = new BaseHandXF<RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>>();
            _tempG = new TempRummySetsXF<EnumSuitList, EnumColorList, RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>>();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer(nameof(FourSuitRummyMainViewModel.RestoreScreen));
            }

            Grid finalGrid = new Grid();
            AddAutoRows(finalGrid, 3);
            Grid firstGrid = new Grid();
            AddLeftOverColumn(firstGrid, 40);
            AddAutoColumns(firstGrid, 1);
            AddLeftOverColumn(firstGrid, 15);
            AddLeftOverColumn(firstGrid, 30);





            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.

            AddControlToGrid(firstGrid, otherStack, 0, 1);
            StackLayout firstStack = new StackLayout();
            firstStack.Children.Add(_playerHandWPF);
            var thisBut = GetGamingButton("Play Sets", nameof(FourSuitRummyMainViewModel.PlaySetsAsync));
            firstStack.Children.Add(thisBut);
            AddControlToGrid(firstGrid, firstStack, 0, 0);

            _score.AddColumn("Cards Left", true, nameof(FourSuitRummyPlayerItem.ObjectCount)); //very common.
            _score.AddColumn("Total Score", true, nameof(FourSuitRummyPlayerItem.TotalScore));
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(FourSuitRummyMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(FourSuitRummyMainViewModel.Status));

            AddControlToGrid(firstGrid, firstInfo.GetContent, 0, 2);

            AddControlToGrid(firstGrid, _score, 0, 3);
            AddControlToGrid(finalGrid, firstGrid, 1, 0);
            StackLayout thirdStack = new StackLayout();
            thirdStack.Orientation = StackOrientation.Horizontal;
            thirdStack.Children.Add(_tempG);
            //_setGrid = new Grid();
            //thirdStack.Children.Add(_setGrid);
            Grid setgrid = new Grid();
            thirdStack.Children.Add(setgrid);
            AddControlToGrid(finalGrid, thirdStack, 2, 0);
            AddLeftOverColumn(setgrid, 50);
            AddLeftOverColumn(setgrid, 50);



            
            //var fins = new ParentSingleUIContainer(nameof(FourSuitRummyMainViewModel.OpponentSetsScreen));
            //AddControlToGrid(_setGrid, fins, 0, 1);
            ParentSingleUIContainer parent = new ParentSingleUIContainer(nameof(FourSuitRummyMainViewModel.YourSetsScreen));

            AddControlToGrid(setgrid, parent, 0, 0);
            parent = new ParentSingleUIContainer(nameof(FourSuitRummyMainViewModel.OpponentSetsScreen));
            AddControlToGrid(setgrid, parent, 0, 1);

            //StackLayout temps = new StackLayout();
            //temps.Orientation = StackOrientation.Horizontal;
            //temps.Children.Add(parent);
            //temps.Children.Add(fins);
            //_setGrid.Children.Add(temps);



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
            return Task.CompletedTask;
        }
    }
}
