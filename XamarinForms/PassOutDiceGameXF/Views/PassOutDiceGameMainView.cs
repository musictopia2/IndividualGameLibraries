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
using PassOutDiceGameCP.Data;
using PassOutDiceGameCP.Logic;
using PassOutDiceGameCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using static BasicXFControlsAndPages.Helpers.GridHelper;

namespace PassOutDiceGameXF.Views
{
    public class PassOutDiceGameMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly PassOutDiceGameVMData _model;
        readonly DiceListControlXF<SimpleDice> _diceControl; //i think.
        private readonly CompleteGameBoardXF<GameBoardGraphicsCP> _board = new CompleteGameBoardXF<GameBoardGraphicsCP>();
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
            StackLayout mainStack = new StackLayout();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer(nameof(PassOutDiceGameMainViewModel.RestoreScreen));
            }
            register.RegisterControl(_board.ThisElement, ""); //hopefully okay.
            graphicsCP.LinkBoard();


            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            _diceControl = new DiceListControlXF<SimpleDice>();
            otherStack.Children.Add(_diceControl);

            Grid grid = new Grid();
            AddLeftOverColumn(grid, 1);
            AddAutoColumns(grid, 1);
            AddControlToGrid(grid, _board, 0, 1);
            AddControlToGrid(grid, mainStack, 0, 0);

            StackLayout finalStack = new StackLayout();
            otherStack.Children.Add(finalStack);
            var endButton = GetGamingButton("End Turn", nameof(PassOutDiceGameMainViewModel.EndTurnAsync));
            endButton.HorizontalOptions = LayoutOptions.Start;
            otherStack.Children.Add(endButton);
            mainStack.Children.Add(otherStack);


            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(PassOutDiceGameMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(PassOutDiceGameMainViewModel.Status));

            finalStack.Children.Add(firstInfo.GetContent);
            finalStack.Children.Add(endButton);
            finalStack.Children.Add(_diceControl);



            if (restoreP != null)
            {
                //todo:  figure out where to place the restore ui if there is a restore option.
                finalStack.Children.Add(restoreP); //default add to grid but does not have to.
            }
            Content = grid;
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
