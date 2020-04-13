using System;
using System.Text;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using System.Linq;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using fs = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.FileHelpers;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
using System.Windows.Controls;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using CommonBasicStandardLibraries.Messenging;
using BasicGameFrameworkLibrary.BasicEventModels;
using MinesweeperCP.Data;
using static BasicControlsAndWindowsCore.Helpers.GridHelper;
using MinesweeperCP.Logic;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using MinesweeperCP.ViewModels;
using System.Windows.Data;
using System.Windows;

namespace MinesweeperWPF.Views
{
    public class MinesweeperMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        //TODO:  has to build the main page.  no need for new game since that is completely separated out into another class.
        private readonly IEventAggregator _aggregator;
        private readonly GameboardWPF _gameboard;
        public MinesweeperMainView(IEventAggregator aggregator, MinesweeperMainGameClass game)
        {
            _aggregator = aggregator;
			_aggregator.Subscribe(this);


            //parts needed:
            //grid
            //gameboard
            //column 1 is gameboard
            //column 2 is the following:
            //stack panel
            //first part is label grid with mines needed, mines left, level chosen
            //plus command button to toggle unflip mines and flag mines


            Grid grid = new Grid();
            AddLeftOverColumn(grid, 1); //well see how this works (?)
            AddLeftOverColumn(grid, 1);
            GameboardWPF gameboard = new GameboardWPF(game);
            AddControlToGrid(grid, gameboard, 0, 0);
            _gameboard = gameboard;
            
            //TestControl test = new TestControl(aggregator, game);
            //AddControlToGrid(grid, test, 0, 0);

            StackPanel stack = new StackPanel();
            SimpleLabelGrid thisLab = new SimpleLabelGrid();
            thisLab.AddRow("Mines Needed", nameof(MinesweeperMainViewModel.HowManyMinesNeeded));
            thisLab.AddRow("Mines Left", nameof(MinesweeperMainViewModel.NumberOfMinesLeft));
            thisLab.AddRow("Level Chosen", nameof(MinesweeperMainViewModel.LevelChosen)); //hopefully this simple (?)

            //i do like the buttons first.
            Button button = GetGamingButton("", nameof(MinesweeperMainViewModel.ChangeFlag));
            Binding binding = new Binding(nameof(MinesweeperMainViewModel.IsFlagging));
            IValueConverter converter = new ToggleNameConverter();
            binding.Converter = converter;
            button.SetBinding(ContentProperty, binding);

            binding = new Binding(nameof(MinesweeperMainViewModel.IsFlagging));
            converter = new ToggleColorConverter();
            binding.Converter = converter;
            button.SetBinding(BackgroundProperty, binding);

            stack.Children.Add(button);
            button.Margin = new Thickness(5);
            button.HorizontalAlignment = HorizontalAlignment.Left;
            button.VerticalAlignment = VerticalAlignment.Top;
            stack.Children.Add(thisLab.GetContent);
            AddControlToGrid(grid, stack, 0, 1);
            Content = grid;
        }
        async Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {


            _gameboard.Init();


            //MinesweeperSaveInfo thisSave = cons!.Resolve<MinesweeperSaveInfo>();

            await this.RefreshBindingsAsync(_aggregator);

            await _gameboard.StartUpAsync();
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
