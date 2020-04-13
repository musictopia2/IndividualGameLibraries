using A8RoundRummyCP.Cards;
using A8RoundRummyCP.Data;
using A8RoundRummyCP.ViewModels;
using BasicControlsAndWindowsCore.Controls.NavigationControls;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIWPFLibrary.BasicControls.GameFrames;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using BasicGamingUIWPFLibrary.BasicControls.SingleCardFrames;
using BasicGamingUIWPFLibrary.Helpers;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows;
using System.Windows.Controls;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //usually needs this as well for grid helpers.
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;

namespace A8RoundRummyWPF.Views
{
    public class A8RoundRummyMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly A8RoundRummyVMData _model;
        private readonly A8RoundRummyGameContainer _gameContainer;
        private readonly BaseDeckWPF<A8RoundRummyCardInformation, A8RoundRummyGraphicsCP, CardGraphicsWPF> _deckGPile;
        private readonly BasePileWPF<A8RoundRummyCardInformation, A8RoundRummyGraphicsCP, CardGraphicsWPF> _discardGPile;

        private readonly ScoreBoardWPF _score;
        private readonly BaseHandWPF<A8RoundRummyCardInformation, A8RoundRummyGraphicsCP, CardGraphicsWPF> _playerHandWPF;
        private readonly RoundUI _roundControl;
        public A8RoundRummyMainView(IEventAggregator aggregator,
            TestOptions test,
            A8RoundRummyVMData model,
            A8RoundRummyGameContainer gameContainer
            )
        {
            _aggregator = aggregator;
            _model = model;
            _gameContainer = gameContainer;
            _aggregator.Subscribe(this);

            _deckGPile = new BaseDeckWPF<A8RoundRummyCardInformation, A8RoundRummyGraphicsCP, CardGraphicsWPF>();
            _discardGPile = new BasePileWPF<A8RoundRummyCardInformation, A8RoundRummyGraphicsCP, CardGraphicsWPF>();
            _score = new ScoreBoardWPF();
            _playerHandWPF = new BaseHandWPF<A8RoundRummyCardInformation, A8RoundRummyGraphicsCP, CardGraphicsWPF>();
            _roundControl = new RoundUI();

            StackPanel mainStack = new StackPanel();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer()
                {
                    Name = nameof(A8RoundRummyMainViewModel.RestoreScreen)
                };
            }
            Grid grid2 = new Grid();
            AddLeftOverColumn(grid2, 60);
            AddLeftOverColumn(grid2, 40); // can adjust as needed
            AddControlToGrid(grid2, mainStack, 0, 0);
            _roundControl = new RoundUI();
            AddControlToGrid(grid2, _roundControl, 0, 1);
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            var thisBut = GetGamingButton("Go Out", nameof(A8RoundRummyMainViewModel.GoOutAsync));
            otherStack.Children.Add(thisBut);
            mainStack.Children.Add(_playerHandWPF);
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(A8RoundRummyMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(A8RoundRummyMainViewModel.Status));
            firstInfo.AddRow("Next", nameof(A8RoundRummyMainViewModel.NextTurn));
            mainStack.Children.Add(firstInfo.GetContent);
            mainStack.Children.Add(otherStack);
            _score.AddColumn("Cards Left", true, nameof(A8RoundRummyPlayerItem.ObjectCount)); //very common.
            _score.AddColumn("Total Score", true, nameof(A8RoundRummyPlayerItem.TotalScore));
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
