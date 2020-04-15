using AggravationCP.Data;
using AggravationCP.Graphics;
using AggravationCP.ViewModels;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.Dice;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.DataClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIXFLibrary.BasicControls.GameBoards;
using BasicGamingUIXFLibrary.BasicControls.Misc;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicGamingUIXFLibrary.GameGraphics.GamePieces;
using BasicGamingUIXFLibrary.Helpers;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.

namespace AggravationXF.Views
{
    public class AggravationMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>, IHandle<NewTurnEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly AggravationVMData _model;
        private readonly AggravationGameContainer _gameContainer;
        readonly DiceListControlXF<SimpleDice> _diceControl; //i think.
        readonly CompleteGameBoardXF<GameBoardGraphicsCP> _board = new CompleteGameBoardXF<GameBoardGraphicsCP>();
        private readonly MarblePiecesXF<EnumColorChoice> _ourPiece;
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
            _ourPiece = new MarblePiecesXF<EnumColorChoice>();
            _ourPiece.Margin = new Thickness(3, 3, 3, 3);
            _ourPiece.HorizontalOptions = LayoutOptions.Start;
            _ourPiece.VerticalOptions = LayoutOptions.Start;
            _ourPiece.Init(); //i think.
            StackLayout mainStack = new StackLayout();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer(nameof(AggravationMainViewModel.RestoreScreen));
            }


            Button thisRoll;
            if (ScreenUsed == EnumScreen.SmallPhone)
                thisRoll = GetSmallerButton("Roll Dice", nameof(AggravationMainViewModel.RollDiceAsync));
            else
            {
                thisRoll = GetGamingButton("Roll Dice", nameof(AggravationMainViewModel.RollDiceAsync));
                thisRoll.FontSize += 20;
            }

            thisRoll.Margin = new Thickness(3, 3, 0, 0);
            thisRoll.HorizontalOptions = LayoutOptions.Start;
            thisRoll.VerticalOptions = LayoutOptions.Start;

            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            _diceControl = new DiceListControlXF<SimpleDice>();
            otherStack.Spacing = 2;
            var endButton = GetGamingButton("End Turn", nameof(AggravationMainViewModel.EndTurnAsync));
            endButton.HorizontalOptions = LayoutOptions.Start;






            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(AggravationMainViewModel.NormalTurn));
            firstInfo.AddRow("Instructions", nameof(AggravationMainViewModel.Instructions));
            firstInfo.AddRow("Status", nameof(AggravationMainViewModel.Status));



            otherStack.Children.Add(_ourPiece);
            otherStack.Children.Add(thisRoll);
            otherStack.Children.Add(_diceControl);
            mainStack.Children.Add(otherStack);
            mainStack.Children.Add(_board);
            mainStack.Children.Add(firstInfo.GetContent);


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
            _ourPiece!.MainColor = _gameContainer.SingleInfo!.Color.ToColor();
            _diceControl!.LoadDiceViewModel(_model.Cup!);
            _board.LoadBoard();
            return this.RefreshBindingsAsync(_aggregator);
        }
        void IHandle<NewTurnEventModel>.Handle(NewTurnEventModel message)
        {
            _ourPiece!.MainColor = _gameContainer.SingleInfo!.Color.ToColor();
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
