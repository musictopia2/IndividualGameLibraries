using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIXFLibrary.BasicControls.GameFrames;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicGamingUIXFLibrary.GameGraphics.GamePieces;
using BasicGamingUIXFLibrary.Helpers;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using PaydayCP.Cards;
using PaydayCP.Data;
using PaydayCP.Graphics;
using PaydayCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using static BasicXFControlsAndPages.Helpers.GridHelper;

namespace PaydayXF.Views
{
    public class PaydayMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>, IHandle<NewTurnEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly PaydayGameContainer _gameContainer;
        private readonly PaydayVMData _model;
        private readonly GameBoardXF _board;
        private readonly ScoreBoardXF _score;
        private readonly PawnPiecesXF<EnumColorChoice>? _currentPiece;
        private readonly BaseHandXF<DealCard, CardGraphicsCP, DealCardXF>? _dealList;

        public PaydayMainView(IEventAggregator aggregator,
            TestOptions test,
            GameBoardGraphicsCP graphicsCP,
            IGamePackageRegister register,
            PaydayGameContainer gameContainer,
            PaydayVMData model
            )
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);
            StackLayout mainStack = new StackLayout();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer(nameof(PaydayMainViewModel.RestoreScreen));
            }

            _aggregator = aggregator;
            _gameContainer = gameContainer;
            _model = model;
            _aggregator.Subscribe(this);
            _board = new GameBoardXF();
            register.RegisterControl(_board.Element, "main");
            graphicsCP.LinkBoard();
            _score = new ScoreBoardXF();
            _score.AddColumn("Money", true, nameof(PaydayPlayerItem.MoneyHas), useCurrency: true, rightMargin: 10);
            _score.AddColumn("Loans", true, nameof(PaydayPlayerItem.Loans), useCurrency: true, rightMargin: 10);






            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Main Turn", nameof(PaydayMainViewModel.NormalTurn));
            firstInfo.AddRow("Other Turn", nameof(PaydayMainViewModel.OtherLabel));
            firstInfo.AddRow("Progress", nameof(PaydayMainViewModel.MonthLabel));
            firstInfo.AddRow("Status", nameof(PaydayMainViewModel.Status));


            var firstContent = firstInfo.GetContent;
            StackLayout tempStack = new StackLayout();
            AddVerticalLabelGroup("Instructions", nameof(PaydayMainViewModel.Instructions), tempStack);
            ScrollView tempScroll = new ScrollView();
            tempScroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Never;

            tempScroll.Content = tempStack;

            StackLayout firstStack = new StackLayout();
            firstStack.Orientation = StackOrientation.Horizontal;
            firstStack.Children.Add(_board);
            mainStack.Children.Add(firstStack);
            firstStack.Margin = new Thickness(3, 3, 3, 0);
            Grid rightGrid = new Grid();

            //this is very iffy.  lots of adjustments may be needed.
            //it only works for large tablets now anyways.


            firstStack.Children.Add(rightGrid);
            AddPixelRow(rightGrid, 225);
            AddLeftOverRow(rightGrid, 1);
            AddAutoColumns(rightGrid, 1);
            Grid grid1 = new Grid();
            AddControlToGrid(rightGrid, grid1, 0, 0);
            AddAutoRows(grid1, 1);
            AddPixelColumn(grid1, 200);
            AddPixelColumn(grid1, 130);
            AddPixelColumn(grid1, 250);
            //AddLeftOverColumn(grid1, 1);
            StackLayout stack1 = new StackLayout();
            AddControlToGrid(grid1, stack1, 0, 0);

            ParentSingleUIContainer mailPile = new ParentSingleUIContainer(nameof(PaydayMainViewModel.MailPileScreen));
            ParentSingleUIContainer dealPile = new ParentSingleUIContainer(nameof(PaydayMainViewModel.DealPileScreen));
            stack1.Children.Add(mailPile);
            stack1.Children.Add(dealPile);

            ParentSingleUIContainer roller = new ParentSingleUIContainer(nameof(PaydayMainViewModel.RollerScreen));

            stack1.Children.Add(roller);
            AddControlToGrid(grid1, tempScroll, 0, 1); // instructions on card
            //StackLayout endStack = new StackLayout();
            //endStack.Children.Add(_score);
            //endStack.Children.Add(firstContent);


            AddControlToGrid(grid1, _score, 0, 2);
            //AddControlToGrid(grid1, firstContent, 0, 2);
            //AddControlToGrid(grid1, _score, 0, 3);
            Grid grid2 = new Grid();
            AddControlToGrid(rightGrid, grid2, 1, 0);
            AddAutoRows(grid2, 1);
            AddAutoColumns(grid2, 1);
            AddLeftOverColumn(grid2, 1);
            StackLayout finalStack = new StackLayout();
            _dealList = new BaseHandXF<DealCard, CardGraphicsCP, DealCardXF>();
            _dealList.HeightRequest = 500;
            _dealList.HandType = HandObservable<DealCard>.EnumHandList.Vertical;
            _dealList.HorizontalOptions = LayoutOptions.Start;

            finalStack.Children.Add(_dealList);
            AddControlToGrid(grid2, finalStack, 0, 0);
            _currentPiece = new PawnPiecesXF<EnumColorChoice>();
            _currentPiece.IsVisible = false; // until proven to need it
            _currentPiece.WidthRequest = 70;
            _currentPiece.HeightRequest = 70;
            _currentPiece.Margin = new Thickness(5, 5, 5, 5);
            _currentPiece.Init();
            finalStack.Children.Add(_currentPiece);

            ParentSingleUIContainer mailList = new ParentSingleUIContainer(nameof(PaydayMainViewModel.MailListScreen));

            AddControlToGrid(grid2, mailList, 0, 1);

            

            ParentSingleUIContainer extras = new ParentSingleUIContainer(nameof(PaydayMainViewModel.DealOrBuyScreen));
            AddControlToGrid(grid2, extras, 0, 1);

            extras = new ParentSingleUIContainer(nameof(PaydayMainViewModel.BuyDealScreen));
            AddControlToGrid(grid2, extras, 0, 1);

            extras = new ParentSingleUIContainer(nameof(PaydayMainViewModel.ChooseDealScreen));
            AddControlToGrid(grid2, extras, 0, 1);
            extras = new ParentSingleUIContainer(nameof(PaydayMainViewModel.LotteryScreen));
            AddControlToGrid(grid2, extras, 0, 1);
            extras = new ParentSingleUIContainer(nameof(PaydayMainViewModel.PlayerScreen));
            AddControlToGrid(grid2, extras, 0, 1);
            //_score.Margin = new Thickness(0, -150, 0, 0);
            mainStack.Children.Add(firstContent);

            if (restoreP != null)
            {
                //todo:  figure out where to place the restore ui if there is a restore option.
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }
            Content = mainStack;
        }


        object IUIView.DataContext
        {
            get => BindingContext;
            set => BindingContext = value;
        }
        void IHandle<NewTurnEventModel>.Handle(NewTurnEventModel message)
        {
            _currentPiece!.MainColor = _gameContainer.SingleInfo!.Color.ToColor(); //not hooking up to bindings this time.
            _currentPiece.IsVisible = true;
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
