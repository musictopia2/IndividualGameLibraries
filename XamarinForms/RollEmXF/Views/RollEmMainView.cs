using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.Dice;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIXFLibrary.BasicControls.GameFrames;
using BasicGamingUIXFLibrary.BasicControls.Misc;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicGamingUIXFLibrary.Helpers;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using RollEmCP.Data;
using RollEmCP.Logic;
using RollEmCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;

namespace RollEmXF.Views
{
    public class RollEmMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly RollEmVMData _model;
        readonly ScoreBoardXF _score;
        readonly DiceListControlXF<SimpleDice> _diceControl; //i think.
        readonly GameBoardXF _board = new GameBoardXF();
        public RollEmMainView(IEventAggregator aggregator,
            TestOptions test, RollEmVMData model,
            GameBoardGraphicsCP graphicsCP,
            IGamePackageRegister register
            )
        {
            _aggregator = aggregator;
            _aggregator.Subscribe(this);
            _model = model;
            register.RegisterControl(_board.Element, "");
            graphicsCP.LinkBoard();
            StackLayout mainStack = new StackLayout();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer(nameof(RollEmMainViewModel.RestoreScreen));
            }





            var thisRoll = GetGamingButton("Roll Dice", nameof(RollEmMainViewModel.RollDiceAsync));
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            _diceControl = new DiceListControlXF<SimpleDice>();
            mainStack.Children.Add(_board);


            mainStack.Children.Add(_diceControl);



            otherStack.Children.Add(thisRoll);
            var endButton = GetGamingButton("End Turn", nameof(RollEmMainViewModel.EndTurnAsync));
            endButton.HorizontalOptions = LayoutOptions.Start;
            otherStack.Children.Add(endButton);
            mainStack.Children.Add(otherStack);
            _score = new ScoreBoardXF();
            _score.AddColumn("Score Round", true, nameof(RollEmPlayerItem.ScoreRound));
            _score.AddColumn("Score Game", true, nameof(RollEmPlayerItem.ScoreGame));



            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(RollEmMainViewModel.NormalTurn)); // there is no roll number needed for this game.
            firstInfo.AddRow("Round", nameof(RollEmMainViewModel.Round)); //if you don't need, it comment it out.
            firstInfo.AddRow("Status", nameof(RollEmMainViewModel.Status));


            mainStack.Children.Add(firstInfo.GetContent);
            mainStack.Children.Add(_score);


            if (restoreP != null)
            {
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }
            Content = mainStack;
        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {

            GamePackageViewModelBinder.ManuelElements.Clear(); //often times i have to add manually.
            RollEmSaveInfo save = cons!.Resolve<RollEmSaveInfo>(); //usually needs this part for multiplayer games.
            _score!.LoadLists(save.PlayerList);
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
            _board.Dispose();
            return Task.CompletedTask;
        }
    }
}
