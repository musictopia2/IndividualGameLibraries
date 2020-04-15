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
using CousinRummyCP.Data;
using Xamarin.Forms;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using BasicGameFrameworkLibrary.TestUtilities;
using CousinRummyCP.ViewModels;
using BasicGamingUIXFLibrary.Helpers;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;
using BasicGamingUIXFLibrary.BasicControls.SingleCardFrames;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGamingUIXFLibrary.GameGraphics.Cards;
using BasicGamingUIXFLibrary.BasicControls.GameFrames;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicGamingUIXFLibrary.BasicControls.Misc;
using CousinRummyCP.Logic;

namespace CousinRummyXF.Views
{
    public class CousinRummyMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        //TODO:  has to build the main page.  no need for new game since that is completely separated out into another class.
        private readonly IEventAggregator _aggregator;
        private readonly CousinRummyVMData _model;
        private readonly BaseDeckXF<RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>> _deckGPile;
        private readonly BasePileXF<RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>> _discardGPile;

        private readonly ScoreBoardXF _score;
        private readonly BaseHandXF<RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>> _playerHandWPF;
        private readonly TempRummySetsXF<EnumSuitList, EnumColorList, RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>> _tempG;
        private readonly MainRummySetsXF<EnumSuitList, EnumColorList, RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>, PhaseSet, SavedSet> _mainG;

        public CousinRummyMainView(IEventAggregator aggregator,
            TestOptions test,
            CousinRummyVMData model
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
            _mainG = new MainRummySetsXF<EnumSuitList, EnumColorList, RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>, PhaseSet, SavedSet>();

            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer(nameof(CousinRummyMainViewModel.RestoreScreen));
            }

            Grid buyGrid = new Grid();
            AddAutoColumns(buyGrid, 2);
            AddAutoRows(buyGrid, 1);
            AddPixelRow(buyGrid, 100);
            var button = GetSmallerButton("Pass", nameof(CousinRummyMainViewModel.PassAsync));
            AddControlToGrid(buyGrid, button, 0, 0);
            button = GetSmallerButton("Buy", nameof(CousinRummyMainViewModel.BuyAsync));
            AddControlToGrid(buyGrid, button, 0, 1);
            AddControlToGrid(buyGrid, _deckGPile, 1, 0);
            AddControlToGrid(buyGrid, _discardGPile, 1, 1);

            Grid gameGrid = new Grid();
            AddLeftOverRow(gameGrid, 45); // try that
            AddAutoRows(gameGrid, 1);
            AddLeftOverRow(gameGrid, 30);
            var otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_tempG);
            AddControlToGrid(gameGrid, otherStack, 0, 0);

            _score.AddColumn("Cards Left", false, nameof(CousinRummyPlayerItem.ObjectCount)); //very common.
            _score.AddColumn("Tokens Left", false, nameof(CousinRummyPlayerItem.TokensLeft));
            _score.AddColumn("Current Score", false, nameof(CousinRummyPlayerItem.CurrentScore), rightMargin: 5);
            _score.AddColumn("Total Score", false, nameof(CousinRummyPlayerItem.TotalScore));
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Normal Turn", nameof(CousinRummyMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(CousinRummyMainViewModel.Status));
            firstInfo.AddRow("Other Turn", nameof(CousinRummyMainViewModel.OtherLabel));
            firstInfo.AddRow("Phase", nameof(CousinRummyMainViewModel.PhaseData));


            otherStack.Children.Add(_score);
            Grid bottomGrid = new Grid();
            AddLeftOverColumn(bottomGrid, 30);
            AddLeftOverColumn(bottomGrid, 70);
            otherStack = new StackLayout();
            button = GetSmallerButton("Lay Down Initial Sets", nameof(CousinRummyMainViewModel.FirstSetsAsync));
            otherStack.Children.Add(button);
            button = GetSmallerButton("Lay Down Other Sets", nameof(CousinRummyMainViewModel.OtherSetsAsync)); // i think its othersets commands (?)
            otherStack.Children.Add(button);
            AddControlToGrid(bottomGrid, otherStack, 0, 0);
            AddControlToGrid(bottomGrid, _mainG, 0, 1);
            AddControlToGrid(gameGrid, bottomGrid, 2, 0);
            otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            _playerHandWPF.HorizontalOptions = LayoutOptions.StartAndExpand;
            otherStack.Children.Add(buyGrid);
            StackLayout tempStack = new StackLayout();
            tempStack.Children.Add(_playerHandWPF);
            tempStack.Children.Add(firstInfo.GetContent);
            otherStack.Children.Add(tempStack);
            AddControlToGrid(gameGrid, otherStack, 1, 0);

            _deckGPile.Margin = new Thickness(5, 5, 5, 5);

            _discardGPile.Margin = new Thickness(5, 5, 5, 5);

            if (restoreP != null)
            {
                otherStack.Children.Add(restoreP); //default add to grid but does not have to.
            }
            Content = gameGrid;
        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {
            GamePackageViewModelBinder.ManuelElements.Clear(); //often times i have to add manually.

            CousinRummySaveInfo save = cons!.Resolve<CousinRummySaveInfo>(); //usually needs this part for multiplayer games.

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
