using BackgammonCP.Data;
using BackgammonCP.Graphics;
using BackgammonCP.ViewModels;
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
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using static BasicXFControlsAndPages.Helpers.GridHelper;

namespace BackgammonXF.Views
{
    public class BackgammonMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly BackgammonVMData _model;
        readonly DiceListControlXF<SimpleDice> _diceControl; //i think.
        private readonly CompleteGameBoardXF<GameBoardGraphicsCP> _board = new CompleteGameBoardXF<GameBoardGraphicsCP>();

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
            register.RegisterControl(_board.ThisElement, "main");
            graphicsCP.LinkBoard();
            StackLayout mainStack = new StackLayout();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer(nameof(BackgammonMainViewModel.RestoreScreen));
            }


            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            _diceControl = new DiceListControlXF<SimpleDice>();
            otherStack.Children.Add(_board);
            Grid tempGrid = new Grid();
            AddLeftOverRow(tempGrid, 5);
            AddLeftOverRow(tempGrid, 1);
            AddControlToGrid(tempGrid, _diceControl, 1, 0);
            otherStack.Children.Add(tempGrid);

            StackLayout finalStack = new StackLayout();
            StackLayout tempStack = new StackLayout();
            tempStack.Orientation = StackOrientation.Horizontal;

            var endButton = GetSmallerButton("End Turn", nameof(BackgammonMainViewModel.EndTurnAsync));
            endButton.HorizontalOptions = LayoutOptions.Start;

            tempStack.Children.Add(endButton);



            Button other = GetSmallerButton("Undo All Moves", nameof(BackgammonMainViewModel.UndoMoveAsync));
            tempStack.Children.Add(other);

            mainStack.Children.Add(otherStack);


            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(BackgammonMainViewModel.NormalTurn));
            firstInfo.AddRow("Game Status", nameof(BackgammonMainViewModel.Status));
            firstInfo.AddRow("Moves Made", nameof(BackgammonMainViewModel.MovesMade));
            firstInfo.AddRow("Last Status", nameof(BackgammonMainViewModel.LastStatus));
            firstInfo.AddRow("Instructions", nameof(BackgammonMainViewModel.Instructions));


            finalStack.Children.Add(tempStack);
            AddControlToGrid(tempGrid, finalStack, 0, 0);
            finalStack.Children.Add(firstInfo.GetContent);


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
