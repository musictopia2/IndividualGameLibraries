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
using TroubleCP.Data;
using TroubleCP.Graphics;
using TroubleCP.ViewModels;

namespace TroubleWPF.Views
{
    public class TroubleMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly TroubleVMData _model;
        private readonly GameBoardGraphicsCP _graphicsCP;
        readonly DiceListControlWPF<SimpleDice> _diceControl; //i think.
        readonly CompleteGameBoard<GameBoardGraphicsCP> _board = new CompleteGameBoard<GameBoardGraphicsCP>();
        private readonly Grid _tempGrid = new Grid();
        private readonly StackPanel _tempStack = new StackPanel();
        public TroubleMainView(IEventAggregator aggregator,
            TestOptions test,
            TroubleVMData model,
            TroubleGameContainer gameContainer,
            GameBoardGraphicsCP graphicsCP,
            IGamePackageRegister register
            )
        {
            _aggregator = aggregator;
            _model = model;
            _graphicsCP = graphicsCP;
            _aggregator.Subscribe(this);
            gameContainer.PositionDice = PositionDice;
            register.RegisterControl(_board.ThisElement, "");
            graphicsCP.LinkBoard();
            StackPanel mainStack = new StackPanel();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer()
                {
                    Name = nameof(TroubleMainViewModel.RestoreScreen)
                };
            }

            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            _diceControl = new DiceListControlWPF<SimpleDice>();
            mainStack.Children.Add(otherStack);


            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(TroubleMainViewModel.NormalTurn));
            firstInfo.AddRow("Instructions", nameof(TroubleMainViewModel.Instructions));
            firstInfo.AddRow("Status", nameof(TroubleMainViewModel.Status));
            otherStack.Children.Add(_tempGrid);
            otherStack.Children.Add(firstInfo.GetContent);
            _tempGrid.Margin = new Thickness(5);

            _tempStack.Children.Add(_diceControl);
            _tempGrid.Children.Add(_board);

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


        private void PositionDice()
        {
            var pos = _graphicsCP.RecommendedPointForDice;
            _tempStack.Margin = new Thickness(pos.X, pos.Y, 0, 0);
            _tempGrid.Children.Add(_tempStack); //hopefully this simple.
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