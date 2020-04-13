using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGamingUIWPFLibrary.Helpers;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using MastermindCP.Data;
using MastermindCP.EventModels;
using MastermindCP.ViewModels;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows;
using System.Windows.Controls;
using static BasicControlsAndWindowsCore.Helpers.GridHelper;
namespace MastermindWPF.Views
{
    public class GameBoardView : UserControl, IUIView, IHandleAsync<ScrollToGuessEventModel>, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private GameBoardViewModel? _gameBoard;
        private readonly ScrollViewer _thisScroll = new ScrollViewer();
        private readonly Grid _grid = new Grid();
        public static readonly DependencyProperty TopProperty = DependencyProperty.Register("Top", typeof(double), typeof(GameBoardView), new PropertyMetadata());
        public static void SetTopProperty(DependencyObject item, double Value)
        {
            item.SetValue(TopProperty, Value);
        }

        public static double GetTopProperty(DependencyObject item)
        {
            return (double)item.GetValue(TopProperty);
        }

        public GameBoardView(IEventAggregator aggregator)
        {
            _aggregator = aggregator;
            _aggregator.Subscribe(this);
            _thisScroll.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            _thisScroll.Content = _grid;
            AddAutoColumns(_grid, 2);
            Width = 820; //i think.
            Content = _grid;
        }

        private void PopulateControls()
        {
            GamePackageViewModelBinder.ManuelElements.Clear();
            if (_gameBoard == null)
            {
                throw new BasicBlankException("Never had board to populate controls.  Rethink");
            }
            var list = _gameBoard.GuessList;
            _grid.Children.Clear();
            _grid.RowDefinitions.Clear();
            if (list.Count == 0)
            {
                throw new BasicBlankException("The guess list must be populated to begin with");
            }
            if (list.Count == 0)
                return;
            int x = 0;
            AddAutoRows(_grid, list.Count);
            list.ForEach(firstGuess =>
            {
                GuessUI thisUI = new GuessUI();
                AddControlToGrid(_grid, thisUI, x, 0);
                thisUI.Init(firstGuess);
                HintUI thisHint = new HintUI();
                thisHint.Margin = new Thickness(5, 0, 0, 0);
                AddControlToGrid(_grid, thisHint, x, 1);
                thisHint.Init(firstGuess);
                x++;
            });
        }

        Task IHandleAsync<ScrollToGuessEventModel>.HandleAsync(ScrollToGuessEventModel message)
        {
            if (_gameBoard == null)
            {
                throw new BasicBlankException("Gameboard was never set.  Rethink");
            }
            PopulateControls();
            Guess guess = message.Guess;
            var list = _gameBoard.GuessList;
            if (guess.Equals(list.First()))
                _thisScroll!.ScrollToTop();
            else if (guess.Equals(list.Last()))
                _thisScroll!.ScrollToBottom();
            else
            {
                int index = list!.IndexOf(guess);
                if (index <= 6)
                    _thisScroll!.ScrollToTop();
                else
                    _thisScroll!.ScrollToBottom();
            }

            return this.RefreshBindingsAsync(_aggregator);
        }

        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {
            PopulateControls();
            return this.RefreshBindingsAsync(_aggregator);
        }

        Task IUIView.TryActivateAsync()
        {
            _gameBoard = (GameBoardViewModel)DataContext;
            return Task.CompletedTask;
        }

        Task IUIView.TryCloseAsync()
        {
            return Task.CompletedTask;
        }
    }
}