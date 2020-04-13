using BackgammonCP.Data;
using BackgammonCP.Graphics;
using BackgammonCP.ViewModels;
using BasicControlsAndWindowsCore.Controls.NavigationControls;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.Dice;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIWPFLibrary.BasicControls.GameBoards;
using BasicGamingUIWPFLibrary.BasicControls.Misc;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using BasicGamingUIWPFLibrary.Helpers;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows;
using System.Windows.Controls;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.

namespace BackgammonWPF.Views
{
    public class BackgammonMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly BackgammonVMData _model;
        readonly DiceListControlWPF<SimpleDice> _diceControl; //i think.
        private readonly CompleteGameBoard<GameBoardGraphicsCP> _board = new CompleteGameBoard<GameBoardGraphicsCP>();

        public BackgammonMainView(IEventAggregator aggregator,
            TestOptions test,
            BackgammonVMData model,
            GameBoardGraphicsCP graphicsCP,
            IGamePackageRegister register
            )
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);
            StackPanel mainStack = new StackPanel();
            ParentSingleUIContainer? restoreP = null;
            register.RegisterControl(_board.ThisElement, "main");
            graphicsCP.LinkBoard();
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer()
                {
                    Name = nameof(BackgammonMainViewModel.RestoreScreen)
                };
            }

            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            _diceControl = new DiceListControlWPF<SimpleDice>();

            otherStack.Children.Add(_board);
            StackPanel finalStack = new StackPanel();
            StackPanel tempStack = new StackPanel();
            tempStack.Orientation = Orientation.Horizontal;

            var endButton = GetGamingButton("End Turn", nameof(BackgammonMainViewModel.EndTurnAsync));
            endButton.HorizontalAlignment = HorizontalAlignment.Left;
            mainStack.Children.Add(otherStack);
            Button other = GetGamingButton("Undo All Moves", nameof(BackgammonMainViewModel.UndoMoveAsync));



            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(BackgammonMainViewModel.NormalTurn));
            firstInfo.AddRow("Game Status", nameof(BackgammonMainViewModel.Status));
            firstInfo.AddRow("Moves Made", nameof(BackgammonMainViewModel.MovesMade));
            firstInfo.AddRow("Last Status", nameof(BackgammonMainViewModel.LastStatus));
            firstInfo.AddRow("Instructions", nameof(BackgammonMainViewModel.Instructions));
            tempStack.Children.Add(endButton);
            tempStack.Children.Add(other);
            tempStack.Children.Add(_diceControl);
            finalStack.Children.Add(tempStack);
            finalStack.Children.Add(firstInfo.GetContent);
            otherStack.Children.Add(finalStack);
            //if we need to put to main, just change to main (?)

            if (restoreP != null)
            {
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }
            Content = mainStack;

        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {

            GamePackageViewModelBinder.ManuelElements.Clear(); //often times i have to add manually.

            _diceControl!.LoadDiceViewModel(_model.Cup!);
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
