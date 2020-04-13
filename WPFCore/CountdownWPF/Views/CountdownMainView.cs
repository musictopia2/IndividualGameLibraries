using BasicControlsAndWindowsCore.Controls.NavigationControls;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIWPFLibrary.BasicControls.Misc;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using BasicGamingUIWPFLibrary.Helpers;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using CountdownCP.Data;
using CountdownCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows;
using System.Windows.Controls;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.

namespace CountdownWPF.Views
{
    public class CountdownMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly CountdownVMData _model;
        private readonly CountdownGameContainer _gameContainer;
        readonly DiceListControlWPF<CountdownDice> _diceControl; //i think.
        private readonly StackPanel _playerStack = new StackPanel();
        public CountdownMainView(IEventAggregator aggregator,
            TestOptions test, CountdownVMData model,
            CountdownGameContainer gameContainer
            )
        {
            _aggregator = aggregator;
            _model = model;
            _gameContainer = gameContainer;
            _aggregator.Subscribe(this);
            StackPanel mainStack = new StackPanel();

            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer()
                {
                    Name = nameof(CountdownMainViewModel.RestoreScreen)
                };
            }
            mainStack.Children.Add(_playerStack);
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            _diceControl = new DiceListControlWPF<CountdownDice>();
            otherStack.Children.Add(_diceControl);
            var endButton = GetGamingButton("End Turn", nameof(CountdownMainViewModel.EndTurnAsync));
            endButton.HorizontalAlignment = HorizontalAlignment.Left;
            otherStack.Children.Add(endButton);
            var otherButton = GetGamingButton("Show Hints", nameof(CountdownMainViewModel.Hint));
            otherButton.HorizontalAlignment = HorizontalAlignment.Left;
            otherStack.Children.Add(otherButton);
            mainStack.Children.Add(otherStack);

            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(CountdownMainViewModel.NormalTurn)); // there is no roll number needed for this game.
            firstInfo.AddRow("Round", nameof(CountdownMainViewModel.Round)); //if you don't need, it comment it out.
            firstInfo.AddRow("Status", nameof(CountdownMainViewModel.Status));


            mainStack.Children.Add(firstInfo.GetContent);



            if (restoreP != null)
            {
                //todo:  figure out where to place the restore ui if there is a restore option.
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }
            Content = mainStack;

        }
        private bool _didLoad = false;
        private readonly CustomBasicList<PlayerBoardWPF> _boardList = new CustomBasicList<PlayerBoardWPF>();
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
            _playerStack.Orientation = Orientation.Horizontal;
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
            PlayerBoardWPF board = new PlayerBoardWPF();
            _boardList.Add(board);
            board.Margin = new Thickness(3, 3, 3, 3);
            board.LoadBoard(player, _gameContainer);
            _playerStack.Children.Add(board);
            if (player.Id == 1)
                player = _gameContainer.PlayerList[2];
            else
                player = _gameContainer.PlayerList[1];
            board = new PlayerBoardWPF();
            board.Margin = new Thickness(3, 3, 3, 3);
            board.LoadBoard(player, _gameContainer);
            _playerStack.Children.Add(board);
            return this.RefreshBindingsAsync(_aggregator);
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
