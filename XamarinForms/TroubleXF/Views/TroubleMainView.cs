using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.Dice;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIXFLibrary.BasicControls.GameBoards;
using BasicGamingUIXFLibrary.BasicControls.Misc;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicGamingUIXFLibrary.Helpers;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using TroubleCP.Data;
using TroubleCP.Graphics;
using TroubleCP.ViewModels;
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.

namespace TroubleXF.Views
{
    public class TroubleMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly TroubleVMData _model;
        private readonly GameBoardGraphicsCP _graphicsCP;
        readonly DiceListControlXF<SimpleDice> _diceControl; //i think.
        readonly CompleteGameBoardXF<GameBoardGraphicsCP> _board = new CompleteGameBoardXF<GameBoardGraphicsCP>();
        private readonly Grid _tempGrid = new Grid();
        private readonly StackLayout _tempStack = new StackLayout();
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
            StackLayout mainStack = new StackLayout();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer(nameof(TroubleMainViewModel.RestoreScreen));
            }
            _tempGrid.Margin = new Thickness(5, 5, 5, 5);
            _diceControl = new DiceListControlXF<SimpleDice>();
            _diceControl.Margin = new Thickness(10, 0, 0, 0);

            var endButton = GetGamingButton("End Turn", nameof(TroubleMainViewModel.EndTurnAsync));
            endButton.HorizontalOptions = LayoutOptions.Start;


            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(TroubleMainViewModel.NormalTurn));
            firstInfo.AddRow("Instructions", nameof(TroubleMainViewModel.Instructions));
            firstInfo.AddRow("Status", nameof(TroubleMainViewModel.Status));


            mainStack.Children.Add(_tempGrid);
            mainStack.Children.Add(firstInfo.GetContent);
            _tempStack.Children.Add(_diceControl);
            _tempStack.InputTransparent = true; //maybe this will be okay.
            _tempGrid.Children.Add(_board);


            if (restoreP != null)
            {
                //todo:  figure out where to place the restore ui if there is a restore option.
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }
            Content = mainStack;
        }
        async Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {
            GamePackageViewModelBinder.ManuelElements.Clear(); //often times i have to add manually.

            _diceControl!.LoadDiceViewModel(_model.Cup!);
            _board.LoadBoard();
            await this.RefreshBindingsAsync(_aggregator);
            _board.AttemptRepaint(); //try this now.
        }

        private void PositionDice()
        {
            var pos = _graphicsCP.RecommendedPointForDice;
            _tempStack.Margin = new Thickness(pos.X, pos.Y, 0, 0);
            _tempGrid.Children.Add(_tempStack); //hopefully this simple.
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
            _aggregator.Unsubscribe(this);
            return Task.CompletedTask;
        }
    }
}
