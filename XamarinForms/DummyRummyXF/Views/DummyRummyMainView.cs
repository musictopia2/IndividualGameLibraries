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
using DummyRummyCP.Data;
using Xamarin.Forms;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using BasicGameFrameworkLibrary.TestUtilities;
using DummyRummyCP.ViewModels;
using BasicGamingUIXFLibrary.Helpers;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;
using BasicGamingUIXFLibrary.BasicControls.SingleCardFrames;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGamingUIXFLibrary.GameGraphics.Cards;
using BasicGamingUIXFLibrary.BasicControls.GameFrames;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicGamingUIXFLibrary.BasicControls.Misc;
using DummyRummyCP.Logic;

namespace DummyRummyXF.Views
{
    public class DummyRummyMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly DummyRummyVMData _model;
        private readonly BaseDeckXF<RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>> _deckGPile;
        private readonly BasePileXF<RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>> _discardGPile;

        private readonly ScoreBoardXF _score;
        private readonly BaseHandXF<RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>> _playerHandWPF;

        private readonly TempRummySetsXF<EnumSuitList, EnumColorList, RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>> _tempG;
        private readonly MainRummySetsXF<EnumSuitList, EnumColorList, RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>, DummySet, SavedSet> _mainG;

        public DummyRummyMainView(IEventAggregator aggregator,
            TestOptions test,
            DummyRummyVMData model
            )
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);
            _tempG = new TempRummySetsXF<EnumSuitList, EnumColorList, RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>>();
            _mainG = new MainRummySetsXF<EnumSuitList, EnumColorList, RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>, DummySet, SavedSet>();
            _deckGPile = new BaseDeckXF<RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>>();
            _discardGPile = new BasePileXF<RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>>();
            _score = new ScoreBoardXF();
            _playerHandWPF = new BaseHandXF<RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>>();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer(nameof(DummyRummyMainViewModel.RestoreScreen));
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





            _score.AddColumn("Cards Left", true, nameof(DummyRummyPlayerItem.ObjectCount)); //very common.
            _score.AddColumn("Current Score", true, nameof(DummyRummyPlayerItem.CurrentScore));
            _score.AddColumn("Total Score", true, nameof(DummyRummyPlayerItem.TotalScore));
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(DummyRummyMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(DummyRummyMainViewModel.Status));
            firstInfo.AddRow("Up To", nameof(DummyRummyMainViewModel.UpTo));

            AddControlToGrid(firstGrid, otherStack, 0, 1);


            StackLayout firstStack = new StackLayout();
            firstStack.Children.Add(_playerHandWPF);
            StackLayout secondStack = new StackLayout();
            secondStack.Orientation = StackOrientation.Horizontal;
            firstStack.Children.Add(secondStack);
            var button = GetSmallerButton("Lay Down", nameof(DummyRummyMainViewModel.LayDownSetsAsync));
            firstStack.Children.Add(button);
            button = GetSmallerButton("Back", nameof(DummyRummyMainViewModel.Back));
            firstStack.Children.Add(button);

            AddControlToGrid(firstGrid, firstStack, 0, 0);

            AddControlToGrid(firstGrid, _score, 0, 3);
            AddControlToGrid(finalGrid, firstGrid, 1, 0);

            AddControlToGrid(firstGrid, firstInfo.GetContent, 0, 2);
            _tempG.Divider = 1.1;
            StackLayout thirdStack = new StackLayout();
            thirdStack.Orientation = StackOrientation.Horizontal;
            thirdStack.Children.Add(_tempG);
            thirdStack.Children.Add(_mainG);
            AddControlToGrid(finalGrid, thirdStack, 2, 0); // i think

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
