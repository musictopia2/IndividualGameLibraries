using BasicControlsAndWindowsCore.Controls.NavigationControls;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.Dice;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIWPFLibrary.BasicControls.GameFrames;
using BasicGamingUIWPFLibrary.BasicControls.Misc;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using BasicGamingUIWPFLibrary.Helpers;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using RollEmCP.Data;
using RollEmCP.Logic;
using RollEmCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows;
using System.Windows.Controls;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;

namespace RollEmWPF.Views
{
    public class RollEmMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly RollEmVMData _model;
        readonly ScoreBoardWPF _score;
        readonly DiceListControlWPF<SimpleDice> _diceControl; //i think.
        readonly GameBoardWPF _board = new GameBoardWPF();
        public RollEmMainView(IEventAggregator aggregator,
            TestOptions test, RollEmVMData model,
            GameBoardGraphicsCP graphicsCP,
            IGamePackageRegister register
            )
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);
            StackPanel mainStack = new StackPanel();
            register.RegisterControl(_board.Element, "");
            graphicsCP.LinkBoard();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer()
                {
                    Name = nameof(RollEmMainViewModel.RestoreScreen)
                };
            }
            StackPanel tempStack = new StackPanel();
            mainStack.Children.Add(tempStack);
            tempStack.Orientation = Orientation.Horizontal;
            tempStack.Children.Add(_board);
            var thisRoll = GetGamingButton("Roll Dice", nameof(RollEmMainViewModel.RollDiceAsync));
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            _diceControl = new DiceListControlWPF<SimpleDice>();
            otherStack.Children.Add(_diceControl);
            otherStack.Children.Add(thisRoll);
            var endButton = GetGamingButton("End Turn", nameof(RollEmMainViewModel.EndTurnAsync));
            endButton.HorizontalAlignment = HorizontalAlignment.Left;
            otherStack.Children.Add(endButton);
            mainStack.Children.Add(otherStack);
            _score = new ScoreBoardWPF();
            _score.AddColumn("Score Round", true, nameof(RollEmPlayerItem.ScoreRound));
            _score.AddColumn("Score Game", true, nameof(RollEmPlayerItem.ScoreGame));
            tempStack.Children.Add(_score);
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(RollEmMainViewModel.NormalTurn)); // there is no roll number needed for this game.
            firstInfo.AddRow("Round", nameof(RollEmMainViewModel.Round)); //if you don't need, it comment it out.
            firstInfo.AddRow("Status", nameof(RollEmMainViewModel.Status));
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
            RollEmSaveInfo save = cons!.Resolve<RollEmSaveInfo>(); //usually needs this part for multiplayer games.
            _score!.LoadLists(save.PlayerList);
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
            _board.Dispose();
            return Task.CompletedTask;
        }
    }
}
