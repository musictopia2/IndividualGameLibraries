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
using PaydayCP.Data;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicControlsAndWindowsCore.Controls.NavigationControls;
using PaydayCP.ViewModels;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //usually needs this as well for grid helpers.
using BasicGamingUIWPFLibrary.Helpers;
using System.Windows;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using BasicGamingUIWPFLibrary.BasicControls.Misc;
using BasicGameFrameworkLibrary.Dice;
using PaydayCP.Graphics;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGamingUIWPFLibrary.BasicControls.GameFrames;
using BasicGamingUIWPFLibrary.GameGraphics.GamePieces;
using PaydayCP.Cards;
using BasicGameFrameworkLibrary.DrawableListsObservable;

namespace PaydayWPF.Views
{
    public class PaydayMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>, IHandle<NewTurnEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly PaydayGameContainer _gameContainer;
        private readonly PaydayVMData _model;
        private readonly GameBoardWPF _board;
        private readonly ScoreBoardWPF _score;
        private readonly PawnPiecesWPF<EnumColorChoice>? _currentPiece;
        private readonly BaseHandWPF<DealCard, CardGraphicsCP, DealCardWPF>? _dealList;

        public PaydayMainView(IEventAggregator aggregator,
            TestOptions test,
            GameBoardGraphicsCP graphicsCP,
            IGamePackageRegister register,
            PaydayGameContainer gameContainer,
            PaydayVMData model
            )
        {
            StackPanel mainStack = new StackPanel();
            _aggregator = aggregator;
            _gameContainer = gameContainer;
            _model = model;
            _aggregator.Subscribe(this);
            _board = new GameBoardWPF();
            register.RegisterControl(_board.Element, "main");
            graphicsCP.LinkBoard();
            _score = new ScoreBoardWPF();
            _score.AddColumn("Money", true, nameof(PaydayPlayerItem.MoneyHas), useCurrency: true, rightMargin: 10);
            _score.AddColumn("Loans", true, nameof(PaydayPlayerItem.Loans), useCurrency: true, rightMargin: 10);
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Main Turn", nameof(PaydayMainViewModel.NormalTurn));
            firstInfo.AddRow("Other Turn", nameof(PaydayMainViewModel.OtherLabel));
            firstInfo.AddRow("Progress", nameof(PaydayMainViewModel.MonthLabel));
            firstInfo.AddRow("Status", nameof(PaydayMainViewModel.Status));
            //firstInfo.AddRow("Instructions", nameof(PaydayMainViewModel.Instructions));
            var firstContent = firstInfo.GetContent;
            StackPanel tempStack = new StackPanel();
            AddVerticalLabelGroup("Instructions", nameof(PaydayMainViewModel.Instructions), tempStack);
            ScrollViewer tempScroll = new ScrollViewer();
            tempScroll.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            tempScroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
            tempScroll.Content = tempStack;

            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer()
                {
                    Name = nameof(PaydayMainViewModel.RestoreScreen)
                };
            }
            StackPanel firstStack = new StackPanel();
            firstStack.Orientation = Orientation.Horizontal;
            firstStack.Children.Add(_board);
            mainStack.Children.Add(firstStack);
            firstStack.Margin = new Thickness(3, 3, 3, 3);
            Grid rightGrid = new Grid();


            firstStack.Children.Add(rightGrid);
            AddPixelRow(rightGrid, 225);
            AddLeftOverRow(rightGrid, 1);
            AddAutoColumns(rightGrid, 1);
            Grid grid1 = new Grid();
            AddControlToGrid(rightGrid, grid1, 0, 0);
            AddAutoRows(grid1, 1);
            AddPixelColumn(grid1, 200);
            AddPixelColumn(grid1, 150);
            AddPixelColumn(grid1, 200);
            AddLeftOverColumn(grid1, 1);
            StackPanel stack1 = new StackPanel();
            AddControlToGrid(grid1, stack1, 0, 0);

            ParentSingleUIContainer mailPile = new ParentSingleUIContainer()
            {
                Name = nameof(PaydayMainViewModel.MailPileScreen)
            };
            ParentSingleUIContainer dealPile = new ParentSingleUIContainer()
            {
                Name = nameof(PaydayMainViewModel.DealPileScreen)
            };
            stack1.Children.Add(mailPile);
            stack1.Children.Add(dealPile);

            ParentSingleUIContainer roller = new ParentSingleUIContainer()
            {
                Name = nameof(PaydayMainViewModel.RollerScreen)
            };

            stack1.Children.Add(roller);
            AddControlToGrid(grid1, tempScroll, 0, 1); // instructions on card
            AddControlToGrid(grid1, firstContent, 0, 2);
            AddControlToGrid(grid1, _score, 0, 3);
            Grid grid2 = new Grid();
            AddControlToGrid(rightGrid, grid2, 1, 0);
            AddAutoRows(grid2, 1);
            AddAutoColumns(grid2, 1);
            AddLeftOverColumn(grid2, 1);
            StackPanel finalStack = new StackPanel();
            _dealList = new BaseHandWPF<DealCard, CardGraphicsCP, DealCardWPF>();
            _dealList.Height = 500;
            _dealList.HandType = HandObservable<DealCard>.EnumHandList.Vertical;
            _dealList.HorizontalAlignment = HorizontalAlignment.Left;

            finalStack.Children.Add(_dealList);
            AddControlToGrid(grid2, finalStack, 0, 0);
            _currentPiece = new PawnPiecesWPF<EnumColorChoice>();
            _currentPiece.Visibility = Visibility.Collapsed; // until proven to need it
            _currentPiece.Width = 80;
            _currentPiece.Height = 80;
            _currentPiece.Margin = new Thickness(5, 5, 5, 5);
            _currentPiece.Init();
            finalStack.Children.Add(_currentPiece);

            ParentSingleUIContainer mailList = new ParentSingleUIContainer()
            {
                Name = nameof(PaydayMainViewModel.MailListScreen)
            };

            AddControlToGrid(grid2, mailList, 0, 1);


            ParentSingleUIContainer extras = new ParentSingleUIContainer()
            {
                Name = nameof(PaydayMainViewModel.DealOrBuyScreen)
            };
            AddControlToGrid(grid2, extras, 0, 1);

            extras = new ParentSingleUIContainer()
            {
                Name = nameof(PaydayMainViewModel.ChooseDealScreen)
            };
            AddControlToGrid(grid2, extras, 0, 1);
            extras = new ParentSingleUIContainer()
            {
                Name = nameof(PaydayMainViewModel.LotteryScreen)
            };
            AddControlToGrid(grid2, extras, 0, 1);
            extras = new ParentSingleUIContainer()
            {
                Name = nameof(PaydayMainViewModel.PlayerScreen)
            };
            AddControlToGrid(grid2, extras, 0, 1);



            if (restoreP != null)
            {
                //todo:  figure out where to place the restore ui if there is a restore option.
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }
            Content = mainStack;

        }

        void IHandle<NewTurnEventModel>.Handle(NewTurnEventModel message)
        {
            _currentPiece!.MainColor = _gameContainer.SingleInfo!.Color.ToColor(); //not hooking up to bindings this time.
            _currentPiece.Visibility = Visibility.Visible;
        }

        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {

            GamePackageViewModelBinder.ManuelElements.Clear(); //often times i have to add manually.

            _score!.LoadLists(_gameContainer.SaveRoot.PlayerList);
            _board.LoadBoard();
            _dealList!.LoadList(_model.CurrentDealList!, "");

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
