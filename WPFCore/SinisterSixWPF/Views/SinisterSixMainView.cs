using BasicControlsAndWindowsCore.Controls.NavigationControls;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.Dice;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIWPFLibrary.BasicControls.GameFrames;
using BasicGamingUIWPFLibrary.BasicControls.Misc;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using BasicGamingUIWPFLibrary.Helpers;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using SinisterSixCP.Data;
using SinisterSixCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows;
using System.Windows.Controls;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;

namespace SinisterSixWPF.Views
{
    public class SinisterSixMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly SinisterSixVMData _model;
        readonly ScoreBoardWPF _score;
        readonly DiceListControlWPF<EightSidedDice> _diceControl; //i think.

        public SinisterSixMainView(IEventAggregator aggregator,
            TestOptions test, SinisterSixVMData model
            )
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);
            StackPanel mainStack = new StackPanel();

            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer()
                {
                    Name = nameof(SinisterSixMainViewModel.RestoreScreen)
                };
            }

            var thisRoll = GetGamingButton("Roll Dice", nameof(SinisterSixMainViewModel.RollDiceAsync));
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            _diceControl = new DiceListControlWPF<EightSidedDice>();
            otherStack.Children.Add(_diceControl);
            otherStack.Children.Add(thisRoll);
            var endButton = GetGamingButton("End Turn", nameof(SinisterSixMainViewModel.EndTurnAsync));
            endButton.HorizontalAlignment = HorizontalAlignment.Left;
            otherStack.Children.Add(endButton);
            mainStack.Children.Add(otherStack);
            var otherButton = GetGamingButton("Remove Selected Dice", nameof(SinisterSixMainViewModel.RemoveDiceAsync));
            otherButton.HorizontalAlignment = HorizontalAlignment.Left;
            mainStack.Children.Add(otherButton);
            _score = new ScoreBoardWPF();
            _score.AddColumn("Score", true, nameof(SinisterSixPlayerItem.Score));
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(SinisterSixMainViewModel.NormalTurn)); // there is no roll number needed for this game.
            firstInfo.AddRow("Roll", nameof(SinisterSixMainViewModel.RollNumber)); //if you don't need, it comment it out.
            firstInfo.AddRow("Status", nameof(SinisterSixMainViewModel.Status));


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
            SinisterSixSaveInfo save = cons!.Resolve<SinisterSixSaveInfo>(); //usually needs this part for multiplayer games.
            _score!.LoadLists(save.PlayerList);
            _diceControl!.LoadDiceViewModel(_model.Cup!);
            return this.RefreshBindingsAsync(_aggregator);
        }



        Task IUIView.TryActivateAsync()
        {
            return Task.CompletedTask;
        }

        Task IUIView.TryCloseAsync()
        {
            return Task.CompletedTask;
        }
    }
}
