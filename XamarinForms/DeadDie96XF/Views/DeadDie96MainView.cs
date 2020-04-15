using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.Dice;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIXFLibrary.BasicControls.GameFrames;
using BasicGamingUIXFLibrary.BasicControls.Misc;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicGamingUIXFLibrary.Helpers;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using DeadDie96CP.Data;
using DeadDie96CP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;

namespace DeadDie96XF.Views
{
    public class DeadDie96MainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly DeadDie96VMData _model;
        readonly ScoreBoardXF _score;
        readonly DiceListControlXF<TenSidedDice> _diceControl; //i think.
        public DeadDie96MainView(IEventAggregator aggregator,
            TestOptions test, DeadDie96VMData model
            )
        {
            _aggregator = aggregator;
            _aggregator.Subscribe(this);
            _model = model;
            StackLayout mainStack = new StackLayout();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer(nameof(DeadDie96MainViewModel.RestoreScreen));
            }


            var thisRoll = GetGamingButton("Roll Dice", nameof(DeadDie96MainViewModel.RollDiceAsync));
            _diceControl = new DiceListControlXF<TenSidedDice>();
            mainStack.Children.Add(_diceControl);
            mainStack.Children.Add(thisRoll);

            _score = new ScoreBoardXF();
            _score.AddColumn("Current Score", true, nameof(DeadDie96PlayerItem.CurrentScore));
            _score.AddColumn("Total Score", true, nameof(DeadDie96PlayerItem.TotalScore));
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
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
            return Task.CompletedTask;
        }
    }
}
