using BasicControlsAndWindowsCore.Controls.NavigationControls;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIWPFLibrary.BasicControls.GameBoards;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using ChineseCheckersCP.Logic;
using ChineseCheckersCP.ViewModels;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows;
using System.Windows.Controls;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //usually needs this as well for grid helpers.
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.

namespace ChineseCheckersWPF.Views
{
    public class ChineseCheckersMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly CompleteGameBoard<GameBoardGraphicsCP> _board = new CompleteGameBoard<GameBoardGraphicsCP>();
        public ChineseCheckersMainView(IEventAggregator aggregator,
            TestOptions test, GameBoardGraphicsCP tempBoard, IGamePackageRegister register
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
                    Name = nameof(ChineseCheckersMainViewModel.RestoreScreen)
                };
            }
            register.RegisterControl(_board.ThisElement, "main");
            tempBoard.LinkBoard(); //i think
            var endButton = GetGamingButton("End Turn", nameof(ChineseCheckersMainViewModel.EndTurnAsync));
            endButton.HorizontalAlignment = HorizontalAlignment.Left;
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(ChineseCheckersMainViewModel.NormalTurn));
            firstInfo.AddRow("Instructions", nameof(ChineseCheckersMainViewModel.Instructions));
            firstInfo.AddRow("Status", nameof(ChineseCheckersMainViewModel.Status));

            _board.Margin = new Thickness(-10, 0, 0, 0);
            _board.HorizontalAlignment = HorizontalAlignment.Left;
            _board.VerticalAlignment = VerticalAlignment.Top;
            Grid tempGrid = new Grid();
            AddPixelColumn(tempGrid, 300);
            AddAutoColumns(tempGrid, 1);
            StackPanel tempStack = new StackPanel();
            tempStack.Children.Add(endButton);
            tempStack.Children.Add(firstInfo.GetContent);
            AddControlToGrid(tempGrid, tempStack, 0, 0);
            AddControlToGrid(tempGrid, _board, 0, 1);
            mainStack.Children.Add(tempGrid);
            if (restoreP != null)
            {
                //todo:  figure out where to place the restore ui if there is a restore option.
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }
            Content = mainStack;

        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {
            _board.LoadBoard(); //very iffy.
            return Task.CompletedTask;
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
