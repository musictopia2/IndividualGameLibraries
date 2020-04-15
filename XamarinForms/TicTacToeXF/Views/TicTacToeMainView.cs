using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.DataClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using TicTacToeCP.Data;
using TicTacToeCP.Logic;
using TicTacToeCP.ViewModels;
using Xamarin.Forms;
using static BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;

namespace TicTacToeXF.Views
{
    public class TicTacToeMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly GameBoardXF _board;
        public TicTacToeMainView(IEventAggregator aggregator,
            TestOptions test
            )
        {
            _aggregator = aggregator;
            _aggregator.Subscribe(this);
            StackLayout mainStack = new StackLayout();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer(nameof(TicTacToeMainViewModel.RestoreScreen));
            }
            _board = new GameBoardXF();
            mainStack.Children.Add(_board);
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
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
            if (ScreenUsed == EnumScreen.LargeTablet)
                tempBoard.SpaceSize = 250;
            else
                tempBoard.SpaceSize = 100; //can experiment.            GamePackageViewModelBinder.ManuelElements.Clear();
            _board.CreateControls(thisSave.GameBoard);
            //hopefully no update needed.
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
