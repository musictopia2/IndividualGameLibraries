using BasicControlsAndWindowsCore.Controls.NavigationControls;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.Dice;
using BasicGameFrameworkLibrary.Dominos;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIWPFLibrary.BasicControls.GameFrames;
using BasicGamingUIWPFLibrary.BasicControls.Misc;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using BasicGamingUIWPFLibrary.GameGraphics.Dominos;
using BasicGamingUIWPFLibrary.Helpers;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using DiceDominosCP.Data;
using DiceDominosCP.Logic;
using DiceDominosCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows;
using System.Windows.Controls;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Dominos.DominosCP;

namespace DiceDominosWPF.Views
{
    public class DiceDominosMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly DiceDominosVMData _model;
        private readonly GameBoardCP _gameBoard;
        readonly ScoreBoardWPF _score;
        readonly DiceListControlWPF<SimpleDice> _diceControl; //i think.
        private readonly CardBoardWPF<SimpleDominoInfo, ts, DominosWPF<SimpleDominoInfo>> _gameBoard1;

        public DiceDominosMainView(IEventAggregator aggregator,
            TestOptions test, DiceDominosVMData model,
            GameBoardCP gameBoard
            )
        {
            _aggregator = aggregator;
            _model = model;
            _gameBoard = gameBoard;
            _aggregator.Subscribe(this);
            StackPanel mainStack = new StackPanel();

            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer()
                {
                    Name = nameof(DiceDominosMainViewModel.RestoreScreen)
                };
            }
            _gameBoard1 = new CardBoardWPF<SimpleDominoInfo, ts, DominosWPF<SimpleDominoInfo>>();
            mainStack.Children.Add(_gameBoard1);
            var thisRoll = GetGamingButton("Roll Dice", nameof(DiceDominosMainViewModel.RollDiceAsync));
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            _diceControl = new DiceListControlWPF<SimpleDice>();
            otherStack.Children.Add(_diceControl);
            otherStack.Children.Add(thisRoll);
            var endButton = GetGamingButton("End Turn", nameof(DiceDominosMainViewModel.EndTurnAsync));
            endButton.HorizontalAlignment = HorizontalAlignment.Left;
            mainStack.Children.Add(otherStack);
            mainStack.Children.Add(endButton);
            _score = new ScoreBoardWPF();
            //anything you need for the scoreboard.
            _score.AddColumn("Dominos Won", true, nameof(DiceDominosPlayerItem.DominosWon), rightMargin: 10);
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(DiceDominosMainViewModel.NormalTurn)); // there is no roll number needed for this game.
            firstInfo.AddRow("Status", nameof(DiceDominosMainViewModel.Status));
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
            DiceDominosSaveInfo save = cons!.Resolve<DiceDominosSaveInfo>(); //usually needs this part for multiplayer games.
            _score!.LoadLists(save.PlayerList);
            _diceControl!.LoadDiceViewModel(_model.Cup!);
            _gameBoard1!.LoadList(_gameBoard, ts.TagUsed);
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
