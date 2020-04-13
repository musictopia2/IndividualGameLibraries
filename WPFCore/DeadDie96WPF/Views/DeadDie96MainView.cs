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
using DeadDie96CP.Data;
using DeadDie96CP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows.Controls;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;

namespace DeadDie96WPF.Views
{
    public class DeadDie96MainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly DeadDie96VMData _model;
        readonly ScoreBoardWPF _score;
        readonly DiceListControlWPF<TenSidedDice> _diceControl; //i think.

        public DeadDie96MainView(IEventAggregator aggregator,
            TestOptions test, DeadDie96VMData model
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
                    Name = nameof(DeadDie96MainViewModel.RestoreScreen)
                };
            }

            var thisRoll = GetGamingButton("Roll Dice", nameof(DeadDie96MainViewModel.RollDiceAsync));
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            _diceControl = new DiceListControlWPF<TenSidedDice>();
            otherStack.Children.Add(_diceControl);
            otherStack.Children.Add(thisRoll);
            mainStack.Children.Add(otherStack);
            _score = new ScoreBoardWPF();
            _score.AddColumn("Current Score", true, nameof(DeadDie96PlayerItem.CurrentScore));
            _score.AddColumn("Total Score", true, nameof(DeadDie96PlayerItem.TotalScore));
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(DeadDie96MainViewModel.NormalTurn)); // there is no roll number needed for this game.
            firstInfo.AddRow("Status", nameof(DeadDie96MainViewModel.Status));


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
            DeadDie96SaveInfo save = cons!.Resolve<DeadDie96SaveInfo>(); //usually needs this part for multiplayer games.
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
