using BasicControlsAndWindowsCore.Controls.NavigationControls;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIWPFLibrary.BasicControls.GameFrames;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using BasicGamingUIWPFLibrary.Helpers;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using ConnectTheDotsCP.Data;
using ConnectTheDotsCP.Graphics;
using ConnectTheDotsCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows.Controls;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;

namespace ConnectTheDotsWPF.Views
{
    public class ConnectTheDotsMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        readonly GameBoardWPF _board = new GameBoardWPF();
        private readonly ScoreBoardWPF _score;
        public ConnectTheDotsMainView(IEventAggregator aggregator,
            TestOptions test, GameBoardGraphicsCP graphicsCP, IGamePackageRegister register
            )
        {
            _aggregator = aggregator;
            _aggregator.Subscribe(this);
            StackPanel mainStack = new StackPanel();
            register.RegisterControl(_board.Element, "");
            graphicsCP.LinkBoard();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer()
                {
                    Name = nameof(ConnectTheDotsMainViewModel.RestoreScreen)
                };
            }
            _score = new ScoreBoardWPF();
            _score.AddColumn("Score", true, nameof(ConnectTheDotsPlayerItem.Score));


            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(ConnectTheDotsMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(ConnectTheDotsMainViewModel.Status));
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(_board);
            mainStack.Children.Add(otherStack);
            StackPanel finalStack = new StackPanel();
            finalStack.Children.Add(firstInfo.GetContent);
            finalStack.Children.Add(_score);
            otherStack.Children.Add(finalStack);

            if (restoreP != null)
            {
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }
            Content = mainStack;
        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {
            GamePackageViewModelBinder.ManuelElements.Clear(); //often times i have to add manually.
            ConnectTheDotsSaveInfo save = cons!.Resolve<ConnectTheDotsSaveInfo>(); //usually needs this part for multiplayer games.
            _score.LoadLists(save.PlayerList);
            _board.LoadBoard();
            return this.RefreshBindingsAsync(_aggregator);
        }

        Task IUIView.TryActivateAsync()
        {
            return Task.CompletedTask;
        }

        Task IUIView.TryCloseAsync()
        {
            _aggregator.Unsubscribe(this);
            return Task.CompletedTask;
        }
    }
}