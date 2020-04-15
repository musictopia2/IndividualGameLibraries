using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGamingUIXFLibrary.Helpers;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using MastermindCP.Data;
using MastermindCP.EventModels;
using MastermindCP.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using static BasicXFControlsAndPages.Helpers.GridHelper;
namespace MastermindXF.Views
{
    public class GameBoardView : ContentView, IUIView, IHandleAsync<ScrollToGuessEventModel>, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private GameBoardViewModel? _gameBoard;
        private readonly ScrollView _thisScroll = new ScrollView();
        private readonly Grid _grid = new Grid();

        public GameBoardView(IEventAggregator aggregator)
        {
            _aggregator = aggregator;
            _aggregator.Subscribe(this);
            _thisScroll.Content = _grid;
            AddAutoColumns(_grid, 2);
            WidthRequest = 820; //i think.
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

        async Task IHandleAsync<ScrollToGuessEventModel>.HandleAsync(ScrollToGuessEventModel message)
        {
            PopulateControls(); //i think this is needed too this time.

            //looks like i can no longer scroll to proper one.
            //with the new way, it won't allow the other processes to run.

            //Guess guess = message.Guess;
            //var list = _gameBoard!.GuessList;
            //if (guess.Equals(list.First()))
            //{
            //    await _thisScroll!.ScrollToAsync(0, 0, false);
            //}
            //else if (guess.Equals(list.Last()))
            //    await (_thisScroll!.ScrollToAsync(0, _thisScroll.HeightRequest, false));
            //else
            //{
            //    int index = list!.IndexOf(guess);
            //    if (index <= 6)
            //        await _thisScroll!.ScrollToAsync(0, 0, false);
            //    else
            //        await (_thisScroll!.ScrollToAsync(0, _thisScroll.HeightRequest, false));
            //}
            await this.RefreshBindingsAsync(_aggregator);
        }

        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {
            PopulateControls();
            return this.RefreshBindingsAsync(_aggregator);
        }

        Task IUIView.TryActivateAsync()
        {
            _gameBoard = (GameBoardViewModel)BindingContext;
            return Task.CompletedTask;
        }

        Task IUIView.TryCloseAsync()
        {
            return Task.CompletedTask;
        }
        object IUIView.DataContext
        {
            get => BindingContext;
            set => BindingContext = value;
        }
    }
}