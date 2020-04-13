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
using PassOutDiceGameCP.Data;
using PassOutDiceGameCP.Logic;
using PassOutDiceGameCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows;
using System.Windows.Controls;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
namespace PassOutDiceGameWPF.Views
{
    public class PassOutDiceGameMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly PassOutDiceGameVMData _model;
        readonly DiceListControlWPF<SimpleDice> _diceControl; //i think.
        private readonly CompleteGameBoard<GameBoardGraphicsCP> _board = new CompleteGameBoard<GameBoardGraphicsCP>();

        public PassOutDiceGameMainView(IEventAggregator aggregator,
            TestOptions test,
            PassOutDiceGameVMData model,
            GameBoardGraphicsCP graphicsCP,
            IGamePackageRegister register
            )
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);
            StackPanel mainStack = new StackPanel();
            ParentSingleUIContainer? restoreP = null;
            register.RegisterControl(_board.ThisElement, ""); //hopefully okay.
            graphicsCP.LinkBoard();
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer()
                {
                    Name = nameof(PassOutDiceGameMainViewModel.RestoreScreen)
                };
            }
            StackPanel finalStack = new StackPanel();
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            _diceControl = new DiceListControlWPF<SimpleDice>();

            otherStack.Children.Add(finalStack);
            otherStack.Children.Add(_board);


            var endButton = GetGamingButton("End Turn", nameof(PassOutDiceGameMainViewModel.EndTurnAsync));
            endButton.HorizontalAlignment = HorizontalAlignment.Left;
            mainStack.Children.Add(otherStack);


            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(PassOutDiceGameMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(PassOutDiceGameMainViewModel.Status));


            finalStack.Children.Add(firstInfo.GetContent);
            finalStack.Children.Add(endButton);
            finalStack.Children.Add(_diceControl);


            if (restoreP != null)
            {
                //todo:  figure out where to place the restore ui if there is a restore option.
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
