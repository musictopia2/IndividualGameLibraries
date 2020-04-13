using BasicControlsAndWindowsCore.Controls.NavigationControls;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.Dice;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIWPFLibrary.BasicControls.Misc;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using BasicGamingUIWPFLibrary.Helpers;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using SnakesAndLaddersCP.Data;
using SnakesAndLaddersCP.Logic;
using SnakesAndLaddersCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows;
using System.Windows.Controls;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.

namespace SnakesAndLaddersWPF.Views
{
    public class SnakesAndLaddersMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>, IHandle<NewTurnEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly DiceListControlWPF<SimpleDice> _diceControl;
        private readonly GamePieceWPF _pieceTurn;
        private readonly GameboardWPF _privateBoard;
        private readonly SnakesAndLaddersMainGameClass _mainGame; //hopefully this works.
        private readonly SnakesAndLaddersVMData _model;

        public SnakesAndLaddersMainView(IEventAggregator aggregator,
            TestOptions test, SnakesAndLaddersMainGameClass mainGame,
            SnakesAndLaddersVMData model
            )
        {
            _aggregator = aggregator;
            _mainGame = mainGame;
            _model = model;
            _aggregator.Subscribe(this);
            StackPanel mainStack = new StackPanel();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer()
                {
                    Name = nameof(SnakesAndLaddersMainViewModel.RestoreScreen)
                };
            }
            _privateBoard = new GameboardWPF();
            _privateBoard.Margin = new Thickness(5, 5, 5, 5);
            _privateBoard.HorizontalAlignment = HorizontalAlignment.Left;
            _privateBoard.VerticalAlignment = VerticalAlignment.Top;
            mainStack.Children.Add(_privateBoard);
            var thisRoll = GetGamingButton("Roll Dice", nameof(SnakesAndLaddersMainViewModel.RollDiceAsync));
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            _diceControl = new DiceListControlWPF<SimpleDice>();
            otherStack.Children.Add(thisRoll);
            _pieceTurn = new GamePieceWPF();
            _pieceTurn.Width = 80;
            _pieceTurn.Height = 80; // try this way.
            _pieceTurn.Margin = new Thickness(10, 0, 0, 0);
            _pieceTurn.NeedsSubscribe = false; // won't notify in this case.  just let them know when new turn.  otherwise, when this number changes, it will trigger for the gameboard (which is not good)
            _pieceTurn.Init();
            otherStack.Children.Add(_pieceTurn);
            otherStack.Children.Add(_diceControl);
            mainStack.Children.Add(otherStack);
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(SnakesAndLaddersMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(SnakesAndLaddersMainViewModel.Status));
            mainStack.Children.Add(firstInfo.GetContent);
            if (restoreP != null)
            {
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }
            Content = mainStack;

        }

        void IHandle<NewTurnEventModel>.Handle(NewTurnEventModel message)
        {
            _pieceTurn!.Index = _mainGame!.SaveRoot!.PlayOrder.WhoTurn;
            if (_mainGame.SingleInfo!.SpaceNumber == 0)
                _pieceTurn.Number = _mainGame.SaveRoot.PlayOrder.WhoTurn; // i think needs to be this so something can show up.
            else
                _pieceTurn.Number = _mainGame.SingleInfo.SpaceNumber;
        }

        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {

            GamePackageViewModelBinder.ManuelElements.Clear(); //often times i have to add manually.


            _diceControl!.LoadDiceViewModel(_model.Cup!);
            _privateBoard.LoadBoard();
            return this.RefreshBindingsAsync(_aggregator);
        }



        Task IUIView.TryActivateAsync()
        {
            return Task.CompletedTask;
        }

        Task IUIView.TryCloseAsync()
        {
            _aggregator.Unsubscribe(this);
            return Task.CompletedTask;
        }
    }
}
