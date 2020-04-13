using BasicControlsAndWindowsCore.Controls.NavigationControls;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using MancalaCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows.Controls;

namespace MancalaWPF.Views
{
    public class MancalaMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly IGamePackageRegister _register;
        readonly GameBoardWPF _board = new GameBoardWPF();
        public MancalaMainView(IEventAggregator aggregator,
            TestOptions test,
            IGamePackageRegister register
            )
        {
            _aggregator = aggregator;
            _register = register;
            _register.RegisterControl(_board.Element, "");
            _aggregator.Subscribe(this);

            StackPanel mainStack = new StackPanel();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer()
                {
                    Name = nameof(MancalaMainViewModel.RestoreScreen)
                };
            }

            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(MancalaMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(MancalaMainViewModel.Status));
            firstInfo.AddRow("Instructions", nameof(MancalaMainViewModel.Instructions));
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            mainStack.Children.Add(otherStack);
            otherStack.Children.Add(_board);
            otherStack.Children.Add(firstInfo.GetContent);

            if (restoreP != null)
            {
                //todo:  figure out where to place the restore ui if there is a restore option.
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }
            Content = mainStack;

        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {
            _board.LoadBoard();
            return Task.CompletedTask;
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
