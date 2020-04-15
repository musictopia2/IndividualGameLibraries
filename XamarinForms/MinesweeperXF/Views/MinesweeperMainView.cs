using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using MinesweeperCP.Logic;
using MinesweeperCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using static BasicXFControlsAndPages.Helpers.GridHelper;

namespace MinesweeperXF.Views
{
    public class MinesweeperMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly GameboardXF _gameboard;
        public MinesweeperMainView(IEventAggregator aggregator, MinesweeperMainGameClass game)
        {
            _aggregator = aggregator;
            _aggregator.Subscribe(this);

            Grid grid = new Grid();
            AddLeftOverColumn(grid, 1); //well see how this works (?)
            AddLeftOverColumn(grid, 1);
            GameboardXF gameboard = new GameboardXF(game);
            AddControlToGrid(grid, gameboard, 0, 0);
            _gameboard = gameboard;

            StackLayout stack = new StackLayout();
            SimpleLabelGridXF thisLab = new SimpleLabelGridXF();
            thisLab.AddRow("Mines Needed", nameof(MinesweeperMainViewModel.HowManyMinesNeeded));
            thisLab.AddRow("Mines Left", nameof(MinesweeperMainViewModel.NumberOfMinesLeft));
            thisLab.AddRow("Level Chosen", nameof(MinesweeperMainViewModel.LevelChosen)); //hopefully this simple (?)

            //i do like the buttons first.
            Button button = GetGamingButton("", nameof(MinesweeperMainViewModel.ChangeFlag));
            Binding binding = new Binding(nameof(MinesweeperMainViewModel.IsFlagging));
            IValueConverter converter = new ToggleNameConverter();
            binding.Converter = converter;
            button.SetBinding(Button.TextProperty, binding);

            binding = new Binding(nameof(MinesweeperMainViewModel.IsFlagging));
            converter = new ToggleColorConverter();
            binding.Converter = converter;
            button.SetBinding(BackgroundColorProperty, binding);

            stack.Children.Add(button);
            button.Margin = new Thickness(5);
            button.HorizontalOptions = LayoutOptions.Start;
            button.VerticalOptions = LayoutOptions.Start;
            stack.Children.Add(thisLab.GetContent);
            AddControlToGrid(grid, stack, 0, 1);
            Content = grid;
        }
        async Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {
            _gameboard.Init();

            await this.RefreshBindingsAsync(_aggregator);

            await _gameboard.StartUpAsync();
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
