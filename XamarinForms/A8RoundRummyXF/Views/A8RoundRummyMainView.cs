using A8RoundRummyCP.Cards;
using A8RoundRummyCP.Data;
using A8RoundRummyCP.ViewModels;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIXFLibrary.BasicControls.GameFrames;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicGamingUIXFLibrary.BasicControls.SingleCardFrames;
using BasicGamingUIXFLibrary.Helpers;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using static BasicXFControlsAndPages.Helpers.GridHelper;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;

namespace A8RoundRummyXF.Views
{
    public class A8RoundRummyMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly A8RoundRummyVMData _model;
        private readonly BaseDeckXF<A8RoundRummyCardInformation, A8RoundRummyGraphicsCP, CardGraphicsXF> _deckGPile;
        private readonly BasePileXF<A8RoundRummyCardInformation, A8RoundRummyGraphicsCP, CardGraphicsXF> _discardGPile;

        private readonly ScoreBoardXF _score;
        private readonly BaseHandXF<A8RoundRummyCardInformation, A8RoundRummyGraphicsCP, CardGraphicsXF> _playerHandWPF;
        private readonly RoundUI _roundControl;
        private readonly A8RoundRummyGameContainer _gameContainer;
        public A8RoundRummyMainView(IEventAggregator aggregator,
            TestOptions test,
            A8RoundRummyVMData model,
            A8RoundRummyGameContainer gameContainer
            )
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);
            _gameContainer = gameContainer;
            _deckGPile = new BaseDeckXF<A8RoundRummyCardInformation, A8RoundRummyGraphicsCP, CardGraphicsXF>();
            _discardGPile = new BasePileXF<A8RoundRummyCardInformation, A8RoundRummyGraphicsCP, CardGraphicsXF>();
            _score = new ScoreBoardXF();
            _playerHandWPF = new BaseHandXF<A8RoundRummyCardInformation, A8RoundRummyGraphicsCP, CardGraphicsXF>();
            _roundControl = new RoundUI();
            StackLayout mainStack = new StackLayout();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer(nameof(A8RoundRummyMainViewModel.RestoreScreen));
            }

            Grid grid2 = new Grid();
            AddLeftOverColumn(grid2, 65);
            AddLeftOverColumn(grid2, 40); // can adjust as needed
            AddControlToGrid(grid2, mainStack, 0, 0);
            AddControlToGrid(grid2, _roundControl, 0, 1);



            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            var thisBut = GetGamingButton("Go Out", nameof(A8RoundRummyMainViewModel.GoOutAsync));
            otherStack.Children.Add(thisBut);
            mainStack.Children.Add(_playerHandWPF);
            _score.AddColumn("Cards Left", true, nameof(A8RoundRummyPlayerItem.ObjectCount)); //very common.
            _score.AddColumn("Total Score", true, nameof(A8RoundRummyPlayerItem.TotalScore));
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(A8RoundRummyMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(A8RoundRummyMainViewModel.Status));
            firstInfo.AddRow("Next", nameof(A8RoundRummyMainViewModel.NextTurn));
            mainStack.Children.Add(firstInfo.GetContent);
            mainStack.Children.Add(otherStack);
            mainStack.Children.Add(_score);


            _deckGPile.Margin = new Thickness(5, 5, 5, 5);

            _discardGPile.Margin = new Thickness(5, 5, 5, 5);

            if (restoreP != null)
            {
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }
            Content = grid2;
        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {
            GamePackageViewModelBinder.ManuelElements.Clear(); //often times i have to add manually.

            A8RoundRummySaveInfo save = cons!.Resolve<A8RoundRummySaveInfo>(); //usually needs this part for multiplayer games.

            _score!.LoadLists(save.PlayerList);
            _playerHandWPF!.LoadList(_model.PlayerHand1!, ""); // i think
            _discardGPile!.Init(_model.Pile1!, ""); // may have to be here (well see)
            _discardGPile.StartListeningDiscardPile(); // its the main one.

            _deckGPile!.Init(_model.Deck1!, ""); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();
            _roundControl.Init(_gameContainer);
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
            _playerHandWPF.Dispose(); //at least will help improve performance.
            return Task.CompletedTask;
        }
    }
}
