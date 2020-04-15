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
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using static BasicXFControlsAndPages.Helpers.GridHelper;
using CommonBasicStandardLibraries.Messenging;
using BasicGameFrameworkLibrary.BasicEventModels;
using ChinazoCP.Data;
using Xamarin.Forms;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using BasicGameFrameworkLibrary.TestUtilities;
using ChinazoCP.ViewModels;
using BasicGamingUIXFLibrary.Helpers;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;
using BasicGamingUIXFLibrary.BasicControls.SingleCardFrames;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGamingUIXFLibrary.GameGraphics.Cards;
using BasicGamingUIXFLibrary.BasicControls.GameFrames;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicGamingUIXFLibrary.BasicControls.Misc;
using ChinazoCP.Logic;

namespace ChinazoXF.Views
{
    public class ChinazoMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        //TODO:  has to build the main page.  no need for new game since that is completely separated out into another class.
        private readonly IEventAggregator _aggregator;
        private readonly ChinazoVMData _model;
        private readonly BaseDeckXF<ChinazoCard, ts, DeckOfCardsXF<ChinazoCard>> _deckGPile;
        private readonly BasePileXF<ChinazoCard, ts, DeckOfCardsXF<ChinazoCard>> _discardGPile;

        private readonly ScoreBoardXF _score;
        private readonly BaseHandXF<ChinazoCard, ts, DeckOfCardsXF<ChinazoCard>> _playerHandWPF;
        private readonly TempRummySetsXF<EnumSuitList, EnumColorList, ChinazoCard, ts, DeckOfCardsXF<ChinazoCard>> _tempG;
        private readonly MainRummySetsXF<EnumSuitList, EnumColorList, ChinazoCard, ts, DeckOfCardsXF<ChinazoCard>, PhaseSet, SavedSet> _mainG;

        public ChinazoMainView(IEventAggregator aggregator,
            TestOptions test,
            ChinazoVMData model
            )
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);

            _deckGPile = new BaseDeckXF<ChinazoCard, ts, DeckOfCardsXF<ChinazoCard>>();
            _discardGPile = new BasePileXF<ChinazoCard, ts, DeckOfCardsXF<ChinazoCard>>();
            _score = new ScoreBoardXF();
            _playerHandWPF = new BaseHandXF<ChinazoCard, ts, DeckOfCardsXF<ChinazoCard>>();
            _tempG = new TempRummySetsXF<EnumSuitList, EnumColorList, ChinazoCard, ts, DeckOfCardsXF<ChinazoCard>>();
            _mainG = new MainRummySetsXF<EnumSuitList, EnumColorList, ChinazoCard, ts, DeckOfCardsXF<ChinazoCard>, PhaseSet, SavedSet>();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer(nameof(ChinazoMainViewModel.RestoreScreen));
            }

            Grid finalGrid = new Grid();
            AddAutoRows(finalGrid, 1); // has to be this way because of scoreboard.
            AddLeftOverRow(finalGrid, 1);

            Grid firstGrid = new Grid();
            AddLeftOverColumn(firstGrid, 60);
            AddAutoColumns(firstGrid, 1);
            AddLeftOverColumn(firstGrid, 30);
            AddLeftOverColumn(firstGrid, 20);


            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            AddControlToGrid(firstGrid, otherStack, 0, 1);
            StackLayout firstStack = new StackLayout();
            firstStack.Children.Add(_playerHandWPF);
            StackLayout secondStack = new StackLayout();
            secondStack.Orientation = StackOrientation.Horizontal;
            firstStack.Children.Add(secondStack);

            var button = GetSmallerButton("Pass", nameof(ChinazoMainViewModel.PassAsync));
            secondStack.Children.Add(button);
            button = GetSmallerButton("Take", nameof(ChinazoMainViewModel.TakeAsync));
            secondStack.Children.Add(button);
            button = GetSmallerButton("Lay Down Sets", nameof(ChinazoMainViewModel.FirstSetsAsync));
            firstStack.Children.Add(button);
            AddControlToGrid(firstGrid, firstStack, 0, 0);

            _score.AddColumn("Cards Left", false, nameof(ChinazoPlayerItem.ObjectCount)); //very common.
            _score.AddColumn("Current Score", false, nameof(ChinazoPlayerItem.CurrentScore), rightMargin: 5);
            _score.AddColumn("Total Score", false, nameof(ChinazoPlayerItem.TotalScore));
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(ChinazoMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(ChinazoMainViewModel.Status));
            firstInfo.AddRow("Other Turn", nameof(ChinazoMainViewModel.OtherLabel));
            firstInfo.AddRow("Phase", nameof(ChinazoMainViewModel.PhaseData));


            AddControlToGrid(firstGrid, firstInfo.GetContent, 0, 2);
            AddControlToGrid(firstGrid, _score, 0, 3);
            AddControlToGrid(finalGrid, firstGrid, 0, 0);
            StackLayout thirdStack = new StackLayout();
            thirdStack.Orientation = StackOrientation.Horizontal;
            thirdStack.Children.Add(_tempG);
            thirdStack.Children.Add(_mainG);
            AddControlToGrid(finalGrid, thirdStack, 1, 0);

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
