using BasicControlsAndWindowsCore.Controls.NavigationControls;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using BasicGamingUIWPFLibrary.Helpers;
using CheckersCP.Logic;
using CheckersCP.ViewModels;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows;
using System.Windows.Controls;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.

namespace CheckersWPF.Views
{
    public class CheckersMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly IGamePackageRegister _register;
        private readonly GameBoardWPF _board = new GameBoardWPF();
        public CheckersMainView(IEventAggregator aggregator,
            TestOptions test, IGamePackageRegister register,
            GameBoardGraphicsCP graphics)
        {
            _aggregator = aggregator;
            _register = register;
            _aggregator.Subscribe(this);
            StackPanel mainStack = new StackPanel();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer()
                {
                    Name = nameof(CheckersMainViewModel.RestoreScreen)
                };
            }
            _register.RegisterControl(_board.Element, "main");
            graphics.LinkBoard(); //this is always needed now.
            _board.Margin = new Thickness(3);
            var endButton = GetGamingButton("End Turn", nameof(CheckersMainViewModel.EndTurnAsync));
            endButton.HorizontalAlignment = HorizontalAlignment.Left;
            Button other = GetGamingButton("Show Tie", nameof(CheckersMainViewModel.TieAsync));
            other.HorizontalAlignment = HorizontalAlignment.Left;
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(CheckersMainViewModel.NormalTurn));
            firstInfo.AddRow("Instructions", nameof(CheckersMainViewModel.Instructions));
            firstInfo.AddRow("Status", nameof(CheckersMainViewModel.Status));
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(_board);
            mainStack.Children.Add(otherStack);
            StackPanel tempStack = new StackPanel();
            tempStack.Orientation = Orientation.Horizontal;
            tempStack.Children.Add(endButton);
            tempStack.Children.Add(other);
            StackPanel finals = new StackPanel();
            finals.Children.Add(tempStack);
            finals.Children.Add(firstInfo.GetContent);
            otherStack.Children.Add(finals);
            if (restoreP != null)
            {
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }
            Content = mainStack;
        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {
            GamePackageViewModelBinder.ManuelElements.Clear(); //often times i have to add manually.
            _board.LoadBoard();
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
