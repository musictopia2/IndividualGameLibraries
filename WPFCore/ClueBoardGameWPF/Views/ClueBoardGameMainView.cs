using BasicControlsAndWindowsCore.Controls.NavigationControls;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.Dice;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIWPFLibrary.BasicControls.GameBoards;
using BasicGamingUIWPFLibrary.BasicControls.GameFrames;
using BasicGamingUIWPFLibrary.BasicControls.Misc;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using BasicGamingUIWPFLibrary.BasicControls.SingleCardFrames;
using BasicGamingUIWPFLibrary.GameGraphics.GamePieces;
using BasicGamingUIWPFLibrary.Helpers;
using ClueBoardGameCP.Cards;
using ClueBoardGameCP.Data;
using ClueBoardGameCP.Graphics;
using ClueBoardGameCP.Logic;
using ClueBoardGameCP.ViewModels;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.

namespace ClueBoardGameWPF.Views
{
    public class ClueBoardGameMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>, IHandle<NewTurnEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly ClueBoardGameVMData _model;
        readonly DiceListControlWPF<SimpleDice> _diceControl; //i think.
        private readonly CompleteGameBoard<GameBoardGraphicsCP> _board = new CompleteGameBoard<GameBoardGraphicsCP>();
        private readonly PawnPiecesWPF<EnumColorChoice> _piece;
        private readonly BasePileWPF<CardInfo, CardCP, CardWPF> _pile;
        private readonly BaseHandWPF<CardInfo, CardCP, CardWPF> _hand;
        private readonly DetectiveNotebookWPF _detective;
        private readonly PredictionAccusationWPF _prediction;
        private readonly ClueBoardGameMainGameClass _mainGame;
        private readonly ClueBoardGameGameContainer _gameContainer;

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
            _piece = new PawnPiecesWPF<EnumColorChoice>();
            _piece.Height = 60;
            _piece.Width = 60;
            _piece.Init();
            _pile = new BasePileWPF<CardInfo, CardCP, CardWPF>();
            _hand = new BaseHandWPF<CardInfo, CardCP, CardWPF>();
            _detective = new DetectiveNotebookWPF();
            _prediction = new PredictionAccusationWPF();

            StackPanel mainStack = new StackPanel();
            StackPanel finalStack = new StackPanel()
            {
                Orientation = Orientation.Horizontal
            };

            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer()
                {
                    Name = nameof(ClueBoardGameMainViewModel.RestoreScreen)
                };
            }

            var thisRoll = GetGamingButton("Roll Dice", nameof(ClueBoardGameMainViewModel.RollDiceAsync));
            var preButton = GetGamingButton("Predict", nameof(ClueBoardGameMainViewModel.MakePredictionAsync));
            var accusationButton = GetGamingButton("Accusation", nameof(ClueBoardGameMainViewModel.MakeAccusationAsync));

            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            _diceControl = new DiceListControlWPF<SimpleDice>();
            var endButton = GetGamingButton("End Turn", nameof(ClueBoardGameMainViewModel.EndTurnAsync));
            endButton.HorizontalAlignment = HorizontalAlignment.Left;


            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(ClueBoardGameMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(ClueBoardGameMainViewModel.Status));

            StackPanel firstRowStack = new StackPanel();
            firstRowStack.Children.Add(_board);
            otherStack.Children.Add(_piece);
            otherStack.Children.Add(firstInfo.GetContent);
            firstRowStack.Children.Add(otherStack);
            finalStack.Children.Add(firstRowStack);
            StackPanel secondRowStack = new StackPanel();
            finalStack.Children.Add(secondRowStack);
            StackPanel thirdRowStack = new StackPanel();
            finalStack.Children.Add(thirdRowStack);
            otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(_hand);
            otherStack.Children.Add(_pile);
            secondRowStack.Children.Add(otherStack);
            _prediction.Margin = new Thickness(0, 0, 0, 30);
            secondRowStack.Children.Add(_prediction);
            secondRowStack.Children.Add(_detective);
            AddVerticalLabelGroup("Instructions", nameof(ClueBoardGameMainViewModel.Instructions), thirdRowStack);

            otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(_diceControl);
            TextBlock label = new TextBlock();
            label.FontSize = 100;
            label.Foreground = Brushes.White;
            label.FontWeight = FontWeights.Bold;
            label.SetBinding(TextBlock.TextProperty, new Binding(nameof(ClueBoardGameMainViewModel.LeftToMove)));
            otherStack.Children.Add(label);
            thirdRowStack.Children.Add(otherStack);
            thirdRowStack.Children.Add(thisRoll);

            thirdRowStack.Children.Add(preButton);
            thirdRowStack.Children.Add(accusationButton);
            thirdRowStack.Children.Add(endButton);
            var nextInfo = new SimpleLabelGrid();
            nextInfo.AddRow("Room", nameof(ClueBoardGameMainViewModel.CurrentRoomName));
            nextInfo.AddRow("Character", nameof(ClueBoardGameMainViewModel.CurrentCharacterName));
            nextInfo.AddRow("Weapon", nameof(ClueBoardGameMainViewModel.CurrentWeaponName));
            thirdRowStack.Children.Add(nextInfo.GetContent);
            mainStack.Children.Add(finalStack);

            if (restoreP != null)
            {
                //todo:  figure out where to place the restore ui if there is a restore option.
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }
            Content = mainStack;

        }

        void IHandle<NewTurnEventModel>.Handle(NewTurnEventModel message)
        {
            _piece!.MainColor = _mainGame!.SingleInfo!.Color.ToColor();
        }

        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {
            GamePackageViewModelBinder.ManuelElements.Clear(); //often times i have to add manually.
            _diceControl!.LoadDiceViewModel(_model.Cup!);
            _board.LoadBoard();
            _pile!.Init(_model.Pile!, "");
            _hand!.LoadList(_model.HandList!, "");
            _mainGame!.PopulateDetectiveNoteBook(); //i think.
            _detective!.LoadControls(_gameContainer);
            _prediction!.LoadControls(_gameContainer);
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
