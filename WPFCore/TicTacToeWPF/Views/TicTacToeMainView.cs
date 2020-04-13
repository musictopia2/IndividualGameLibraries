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
using TicTacToeCP.Data;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicControlsAndWindowsCore.Controls.NavigationControls;
using TicTacToeCP.ViewModels;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //usually needs this as well for grid helpers.
using TicTacToeCP.Logic;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using BasicGamingUIWPFLibrary.Helpers;

namespace TicTacToeWPF.Views
{
    public class TicTacToeMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;

        private readonly GameBoardWPF _board;

        public TicTacToeMainView(IEventAggregator aggregator,
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
                    Name = nameof(TicTacToeMainViewModel.RestoreScreen)
                };
            }
            _board = new GameBoardWPF();
            mainStack.Children.Add(_board);
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(TicTacToeMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(TicTacToeMainViewModel.Status)); // this may have to show the status to begin with (?)
            mainStack.Children.Add(firstInfo.GetContent);
            if (restoreP != null)
            {
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }
            Content = mainStack;
        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {



            TicTacToeSaveInfo thisSave = cons!.Resolve<TicTacToeSaveInfo>(); //usually needs this part for multiplayer games.

            TicTacToeGraphicsCP tempBoard = cons.Resolve<TicTacToeGraphicsCP>();
            tempBoard.SpaceSize = 250;
            GamePackageViewModelBinder.ManuelElements.Clear();
            _board.CreateControls(thisSave.GameBoard);
            //hopefully no update needed.
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
