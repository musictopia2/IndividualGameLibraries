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
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using OpetongCP.Data;
using OpetongCP.Logic;
using OpetongCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using static BasicXFControlsAndPages.Helpers.GridHelper;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;

namespace OpetongXF.Views
{
    public class OpetongMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly OpetongVMData _model;
        private readonly BaseDeckXF<RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>> _deckGPile;

        private readonly ScoreBoardXF _score;
        private readonly BaseHandXF<RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>> _playerHandWPF;

        private readonly TempRummySetsXF<EnumSuitList, EnumColorList, RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>> _tempG;
        private readonly MainRummySetsXF<EnumSuitList, EnumColorList, RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>, RummySet, SavedSet> _mainG;
        private readonly CardBoardXF<RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>> _poolG;

        public OpetongMainView(IEventAggregator aggregator,
            TestOptions test,
            OpetongVMData model
            )
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);

            _deckGPile = new BaseDeckXF<RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>>();
            _score = new ScoreBoardXF();
            _playerHandWPF = new BaseHandXF<RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>>();
            _tempG = new TempRummySetsXF<EnumSuitList, EnumColorList, RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>>();
            _mainG = new MainRummySetsXF<EnumSuitList, EnumColorList, RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>, RummySet, SavedSet>();
            _poolG = new CardBoardXF<RegularRummyCard, ts, DeckOfCardsXF<RegularRummyCard>>();

            StackLayout mainStack = new StackLayout();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer(nameof(OpetongMainViewModel.RestoreScreen));
            }

            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_deckGPile);

            Grid finalGrid = new Grid();
            AddLeftOverRow(finalGrid, 50);
            AddAutoRows(finalGrid, 1);
            AddLeftOverRow(finalGrid, 50);

            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_poolG);
            otherStack.Children.Add(_tempG);
            Button button = GetGamingButton($"Lay Down{Constants.vbCrLf}Single Set", nameof(OpetongMainViewModel.PlaySetAsync));
            otherStack.Children.Add(button);
            StackLayout tempStack = new StackLayout();



            mainStack.Children.Add(otherStack);
            _score.AddColumn("Cards Left", true, nameof(OpetongPlayerItem.ObjectCount)); //very common.
            _score.AddColumn("Sets Played", true, nameof(OpetongPlayerItem.SetsPlayed));
            _score.AddColumn("Score", true, nameof(OpetongPlayerItem.TotalScore));



            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(OpetongMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(OpetongMainViewModel.Status));
            firstInfo.AddRow("Instructions", nameof(OpetongMainViewModel.Instructions));
            tempStack.Children.Add(_score);
            tempStack.Children.Add(firstInfo.GetContent);
            otherStack.Children.Add(tempStack);
            AddControlToGrid(finalGrid, otherStack, 0, 0);
            AddControlToGrid(finalGrid, _playerHandWPF, 1, 0);
            AddControlToGrid(finalGrid, _mainG, 2, 0);



            _deckGPile.Margin = new Thickness(5, 5, 5, 5);


            if (restoreP != null)
            {
                //todo:  figure out where to place the restore ui if there is a restore option.
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }
            Content = finalGrid;
        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {
            GamePackageViewModelBinder.ManuelElements.Clear(); //often times i have to add manually.

            OpetongSaveInfo save = cons!.Resolve<OpetongSaveInfo>(); //usually needs this part for multiplayer games.

            _score!.LoadLists(save.PlayerList);
            _playerHandWPF!.LoadList(_model.PlayerHand1!, ts.TagUsed); // i think

            _deckGPile!.Init(_model.Deck1!, ts.TagUsed); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();
            _tempG!.Init(_model.TempSets!, ts.TagUsed);
            _mainG!.Init(_model.MainSets!, ts.TagUsed);
            _poolG!.LoadList(_model.Pool1!, ts.TagUsed);
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
