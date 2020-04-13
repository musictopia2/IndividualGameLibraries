using System;
using System.Text;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using System.Linq;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using fs = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.FileHelpers;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
using System.Windows.Controls;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using CommonBasicStandardLibraries.Messenging;
using BasicGameFrameworkLibrary.BasicEventModels;
using AggravationCP.Data;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicControlsAndWindowsCore.Controls.NavigationControls;
using AggravationCP.ViewModels;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //usually needs this as well for grid helpers.
using BasicGamingUIWPFLibrary.Helpers;
using System.Windows;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using BasicGamingUIWPFLibrary.BasicControls.Misc;
using BasicGameFrameworkLibrary.Dice;
using BasicGamingUIWPFLibrary.BasicControls.GameBoards;
using AggravationCP.Graphics;
using BasicGamingUIWPFLibrary.GameGraphics.GamePieces;
using BasicGameFrameworkLibrary.DIContainers;

namespace AggravationWPF.Views
{
    public class AggravationMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>, IHandle<NewTurnEventModel>
    {
        //TODO:  has to build the main page.  no need for new game since that is completely separated out into another class.
        private readonly IEventAggregator _aggregator;
        private readonly AggravationVMData _model;
        private readonly AggravationGameContainer _gameContainer;
        readonly DiceListControlWPF<SimpleDice> _diceControl; //i think.
        readonly CompleteGameBoard<GameBoardGraphicsCP> _board = new CompleteGameBoard<GameBoardGraphicsCP>();
        private readonly MarblePiecesWPF<EnumColorChoice> _ourPiece;
        public AggravationMainView(IEventAggregator aggregator,
            TestOptions test,
            AggravationVMData model,
            AggravationGameContainer gameContainer,
            GameBoardGraphicsCP graphicsCP,
            IGamePackageRegister register
            )
        {
            _aggregator = aggregator;
            _model = model;
            _gameContainer = gameContainer;
            _aggregator.Subscribe(this);
            register.RegisterControl(_board.ThisElement, "");
            graphicsCP.LinkBoard();
            _ourPiece = new MarblePiecesWPF<EnumColorChoice>();
            _ourPiece.Width = 80;
            _ourPiece.Height = 80;
            _ourPiece.Init(); //i think.
            StackPanel mainStack = new StackPanel();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer()
                {
                    Name = nameof(AggravationMainViewModel.RestoreScreen)
                };
            }
            Grid tempGrid = new Grid();
            mainStack.Children.Add(tempGrid);
            var thisRoll = GetGamingButton("Roll Dice", nameof(AggravationMainViewModel.RollDiceAsync));
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            _diceControl = new DiceListControlWPF<SimpleDice>();


            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(AggravationMainViewModel.NormalTurn));
            firstInfo.AddRow("Instructions", nameof(AggravationMainViewModel.Instructions));
            firstInfo.AddRow("Status", nameof(AggravationMainViewModel.Status));
            //if we need to put to main, just change to main (?)

            StackPanel firstStack = new StackPanel();
            firstStack.Margin = new Thickness(3, 3, 3, 3);
            firstStack.Children.Add(firstInfo.GetContent);
            firstStack.Children.Add(_ourPiece);
            otherStack.Children.Add(thisRoll);
            otherStack.Children.Add(_diceControl);
            firstStack.Children.Add(otherStack);
            tempGrid.Children.Add(firstStack);
            tempGrid.Children.Add(_board);

            if (restoreP != null)
            {
                //todo:  figure out where to place the restore ui if there is a restore option.
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }
            Content = mainStack;

        }

        void IHandle<NewTurnEventModel>.Handle(NewTurnEventModel message)
        {
            _ourPiece!.MainColor = _gameContainer.SingleInfo!.Color.ToColor();
        }

        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {

            GamePackageViewModelBinder.ManuelElements.Clear(); //often times i have to add manually.
            _ourPiece!.MainColor = _gameContainer.SingleInfo!.Color.ToColor();
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