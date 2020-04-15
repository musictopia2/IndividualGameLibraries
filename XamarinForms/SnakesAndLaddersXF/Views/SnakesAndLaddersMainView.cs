using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.Dice;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.DataClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIXFLibrary.BasicControls.Misc;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicGamingUIXFLibrary.Helpers;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using SnakesAndLaddersCP.Data;
using SnakesAndLaddersCP.Logic;
using SnakesAndLaddersCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.

namespace SnakesAndLaddersXF.Views
{
    public class SnakesAndLaddersMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>, IHandle<NewTurnEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly DiceListControlXF<SimpleDice> _diceControl;
        private readonly GamePieceXF _pieceTurn;
        private readonly GameboardXF _privateBoard;
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
            StackLayout mainStack = new StackLayout();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer(nameof(SnakesAndLaddersMainViewModel.RestoreScreen));
            }

            _privateBoard = new GameboardXF();
            _privateBoard.Margin = new Thickness(5, 5, 5, 5);
            _privateBoard.HorizontalOptions = LayoutOptions.Start;
            _privateBoard.VerticalOptions = LayoutOptions.Start;
            mainStack.Children.Add(_privateBoard);
            var thisRoll = GetGamingButton("Roll Dice", nameof(SnakesAndLaddersMainViewModel.RollDiceAsync));
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            _diceControl = new DiceListControlXF<SimpleDice>();
            otherStack.Children.Add(thisRoll);
            _pieceTurn = new GamePieceXF();
            if (ScreenUsed == EnumScreen.SmallPhone)
            {
                _pieceTurn.WidthRequest = 40;
                _pieceTurn.HeightRequest = 40;
            }
            else
            {
                _pieceTurn.WidthRequest = 90;
                _pieceTurn.HeightRequest = 90;
            }
            _pieceTurn.Margin = new Thickness(10, 0, 0, 0);
            _pieceTurn.NeedsSubscribe = false; // won't notify in this case.  just let them know when new turn.  otherwise, when this number changes, it will trigger for the gameboard (which is not good)
            _pieceTurn.Init();
            otherStack.Children.Add(_pieceTurn);
            otherStack.Children.Add(_diceControl);
            mainStack.Children.Add(otherStack);
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(SnakesAndLaddersMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(SnakesAndLaddersMainViewModel.Status));
            mainStack.Children.Add(firstInfo.GetContent);

            if (restoreP != null)
            {
                //todo:  figure out where to place the restore ui if there is a restore option.
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }
            Content = mainStack;
        }
        public void Handle(NewTurnEventModel message)
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
