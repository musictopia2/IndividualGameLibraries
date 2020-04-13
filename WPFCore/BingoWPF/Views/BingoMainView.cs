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
using BingoCP.Data;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicControlsAndWindowsCore.Controls.NavigationControls;
using BingoCP.ViewModels;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //usually needs this as well for grid helpers.
using System.Windows;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using BasicGamingUIWPFLibrary.Helpers;

namespace BingoWPF.Views
{
    public class BingoMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly BingoBoardWPF _board = new BingoBoardWPF();
        
        public BingoMainView(IEventAggregator aggregator,
            TestOptions test
            )
        {
            _aggregator = aggregator;
			_aggregator.Subscribe(this);
            StackPanel mainStack = new StackPanel();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer()
                {
                    Name = nameof(BingoMainViewModel.RestoreScreen)
                };
            }
            

            _board.Margin = new Thickness(5);
            Grid grid = new Grid();
            AddAutoColumns(grid, 2);
            StackPanel tempStack = new StackPanel();
            SimpleLabelGrid secondInfo = new SimpleLabelGrid();
            secondInfo.FontSize = 40;
            secondInfo.AddRow("Number Called", nameof(BingoMainViewModel.NumberCalled));
            tempStack.Children.Add(secondInfo.GetContent);
            tempStack.Children.Add(_board);
            AddControlToGrid(grid!, tempStack, 0, 0);
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Status", nameof(BingoMainViewModel.Status));
            mainStack.Children.Add(firstInfo.GetContent);
            AddControlToGrid(grid, mainStack, 0, 1);
            var endButton = GetGamingButton("Bingo", nameof(BingoMainViewModel.BingoAsync)); // its bingo instead.
            endButton.HorizontalAlignment = HorizontalAlignment.Left;
            mainStack.Children.Add(endButton);

            if (restoreP != null)
            {
                //todo:  figure out where to place the restore ui if there is a restore option.
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }
            Content = grid;

        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {
            //if i have to clear and manually add controls, will do.  not sure if we have to add controls manually or not.
            GamePackageViewModelBinder.ManuelElements.Clear();

            BingoSaveInfo thisSave = cons!.Resolve<BingoSaveInfo>(); //usually needs this part for multiplayer games.
            _board!.CreateControls(thisSave.BingoBoard);
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
