using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.Dice;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIXFLibrary.BasicControls.GameBoards;
using BasicGamingUIXFLibrary.BasicControls.GameFrames;
using BasicGamingUIXFLibrary.BasicControls.Misc;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicGamingUIXFLibrary.BasicControls.SingleCardFrames;
using BasicGamingUIXFLibrary.GameGraphics.GamePieces;
using BasicGamingUIXFLibrary.Helpers;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using ClueBoardGameCP.Cards;
using ClueBoardGameCP.Data;
using ClueBoardGameCP.Graphics;
using ClueBoardGameCP.Logic;
using ClueBoardGameCP.ViewModels;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using static BasicXFControlsAndPages.Helpers.GridHelper;

namespace ClueBoardGameXF.Views
{
    public class ClueBoardGameMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>, IHandle<NewTurnEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly ClueBoardGameVMData _model;
        readonly DiceListControlXF<SimpleDice> _diceControl; //i think.
        private readonly CompleteGameBoardXF<GameBoardGraphicsCP> _board = new CompleteGameBoardXF<GameBoardGraphicsCP>();
        private readonly PawnPiecesXF<EnumColorChoice> _piece;
        private readonly BasePileXF<CardInfo, CardCP, CardXF> _pile;
        private readonly BaseHandXF<CardInfo, CardCP, CardXF> _hand;
        private readonly DetectiveNotebookXF _detective;
        private readonly PredictionAccusationXF _prediction;
        private readonly ClueBoardGameMainGameClass _mainGame;
        private readonly ClueBoardGameGameContainer _gameContainer;
        private readonly Button _predictionButton;
        private readonly Grid _details;
        private readonly Button _accusationButton;
        public ClueBoardGameMainView(IEventAggregator aggregator,
            TestOptions test,
            ClueBoardGameVMData model,
            GameBoardGraphicsCP graphicsCP,
            IGamePackageRegister register,
            ClueBoardGameMainGameClass mainGame,
            ClueBoardGameGameContainer gameContainer
            )
        {
            _mainGame = mainGame;
            _gameContainer = gameContainer;
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);
            register.RegisterControl(_board.ThisElement, "");
            graphicsCP.LinkBoard();
            _piece = new PawnPiecesXF<EnumColorChoice>();
            _piece.HeightRequest = 80;
            _piece.WidthRequest = 80;
            _piece.Init();
            _pile = new BasePileXF<CardInfo, CardCP, CardXF>();
            _hand = new BaseHandXF<CardInfo, CardCP, CardXF>();
            _detective = new DetectiveNotebookXF();
            _prediction = new PredictionAccusationXF();
            StackLayout mainStack = new StackLayout();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer(nameof(ClueBoardGameMainViewModel.RestoreScreen));
            }
            _predictionButton = GetSmallerButton("Predict", nameof(ClueBoardGameMainViewModel.MakePredictionAsync));
            _accusationButton = GetSmallerButton("Accusation", nameof(ClueBoardGameMainViewModel.MakeAccusationAsync));

            StackLayout firstStack = new StackLayout();
            firstStack.Orientation = StackOrientation.Horizontal;
            firstStack.Children.Add(_board);

            firstStack.Children.Add(_piece);
            mainStack.Children.Add(firstStack);
            Grid tempGrid = new Grid();
            AddLeftOverColumn(tempGrid, 50);
            AddLeftOverColumn(tempGrid, 50);
            AddAutoColumns(tempGrid, 1);
            mainStack.Children.Add(tempGrid);
            AddControlToGrid(tempGrid, _prediction, 0, 0);
            _board.HorizontalOptions = LayoutOptions.Start;
            _board.VerticalOptions = LayoutOptions.Start;
            Label label = new Label();
            label.FontSize = 30;
            label.TextColor = Color.White;
            label.FontAttributes = FontAttributes.Bold;
            label.SetBinding(Label.TextProperty, new Binding(nameof(ClueBoardGameMainViewModel.LeftToMove)));
            StackLayout tempStack = new StackLayout();

            var thisRoll = GetSmallerButton("Roll Dice", nameof(ClueBoardGameMainViewModel.RollDiceAsync));
            _diceControl = new DiceListControlXF<SimpleDice>();
            var endButton = GetSmallerButton("End Turn", nameof(ClueBoardGameMainViewModel.EndTurnAsync));
            endButton.HorizontalOptions = LayoutOptions.Start;
            tempStack.Children.Add(label);
            tempStack.Children.Add(thisRoll);
            tempStack.Children.Add(endButton);
            tempStack.Children.Add(_pile);
            tempStack.Children.Add(_diceControl);
            AddControlToGrid(tempGrid, _detective, 0, 1);
            AddControlToGrid(tempGrid, tempStack, 0, 2);
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(ClueBoardGameMainViewModel.NormalTurn));
            firstInfo.AddRow("Instructions", nameof(ClueBoardGameMainViewModel.Instructions));
            _details = firstInfo.GetContent;
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
            _pile!.Init(_model.Pile!, "");
            _hand!.LoadList(_model.HandList!, "");
            _mainGame!.PopulateDetectiveNoteBook(); //i think.
            _prediction!.LoadControls(_predictionButton, _accusationButton, _gameContainer);
            _detective!.LoadControls(_hand, _details, _gameContainer);
            return this.RefreshBindingsAsync(_aggregator);
        }

        void IHandle<NewTurnEventModel>.Handle(NewTurnEventModel message)
        {
            _piece!.MainColor = _mainGame!.SingleInfo!.Color.ToColor();
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
