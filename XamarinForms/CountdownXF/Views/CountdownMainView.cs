using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIXFLibrary.BasicControls.Misc;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicGamingUIXFLibrary.Helpers;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using CountdownCP.Data;
using CountdownCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.

namespace CountdownXF.Views
{
    public class CountdownMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly CountdownVMData _model;
        readonly DiceListControlXF<CountdownDice> _diceControl; //i think.
        private readonly StackLayout _playerStack = new StackLayout();

        private bool _didLoad = false;
        private readonly CustomBasicList<PlayerBoardXF> _boardList = new CustomBasicList<PlayerBoardXF>();
        private readonly CountdownGameContainer _gameContainer;

        public CountdownMainView(IEventAggregator aggregator,
            TestOptions test, CountdownVMData model,
            CountdownGameContainer gameContainer
            )
        {
            _aggregator = aggregator;
            _aggregator.Subscribe(this);
            _model = model;
            _gameContainer = gameContainer;
            StackLayout mainStack = new StackLayout();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer(nameof(CountdownMainViewModel.RestoreScreen));
            }
            mainStack.Children.Add(_playerStack);

            var thisRoll = GetGamingButton("Roll Dice", nameof(CountdownMainViewModel.RollDiceAsync));
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            _diceControl = new DiceListControlXF<CountdownDice>();
            otherStack.Children.Add(_diceControl);
            otherStack.Children.Add(thisRoll);
            var endButton = GetGamingButton("End Turn", nameof(CountdownMainViewModel.EndTurnAsync));
            endButton.HorizontalOptions = LayoutOptions.Start;
            var otherButton = GetGamingButton("Show Hints", nameof(CountdownMainViewModel.Hint));
            otherButton.HorizontalOptions = LayoutOptions.Start;

            otherStack.Children.Add(endButton);
            otherStack.Children.Add(otherButton);
            mainStack.Children.Add(otherStack);

            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(CountdownMainViewModel.NormalTurn)); // there is no roll number needed for this game.
            firstInfo.AddRow("Round", nameof(CountdownMainViewModel.Round)); //if you don't need, it comment it out.
            firstInfo.AddRow("Status", nameof(CountdownMainViewModel.Status));


            mainStack.Children.Add(firstInfo.GetContent);

            if (restoreP != null)
            {
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }
            Content = mainStack;
        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {
            if (_didLoad)
            {
                return Task.CompletedTask;
            }
            GamePackageViewModelBinder.ManuelElements.Clear(); //often times i have to add manually.
            _boardList.Clear();
            _diceControl!.LoadDiceViewModel(_model.Cup!);

            _didLoad = true;
            _playerStack.Orientation = StackOrientation.Horizontal;
            _playerStack.Margin = new Thickness(5, 5, 5, 5);
            //self goes on top.
            //however, if its pass and play, then 1 then 2.
            CountdownPlayerItem player;
            if (_gameContainer.BasicData.MultiPlayer == false)
            {
                player = _gameContainer.PlayerList![1];
            }
            else
            {
                player = _gameContainer.PlayerList!.GetSelf();
            }
            PlayerBoardXF board = new PlayerBoardXF();
            _boardList.Add(board);
            board.Margin = new Thickness(3, 3, 3, 3);
            board.LoadBoard(player, _gameContainer);
            _playerStack.Children.Add(board);
            if (player.Id == 1)
                player = _gameContainer.PlayerList[2];
            else
                player = _gameContainer.PlayerList[1];
            board = new PlayerBoardXF();
            board.Margin = new Thickness(3, 3, 3, 3);
            board.LoadBoard(player, _gameContainer);
            _playerStack.Children.Add(board);
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
            _boardList.ForEach(x => x.Dispose());
            return Task.CompletedTask;
        }
    }
}
