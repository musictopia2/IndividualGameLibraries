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
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using YachtRaceCP.Data;
using YachtRaceCP.ViewModels;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;

namespace YachtRaceXF.Views
{
    public class YachtRaceMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly YachtRaceVMData _model;
        readonly ScoreBoardXF _score;
        readonly DiceListControlXF<SimpleDice> _diceControl; //i think.
        public YachtRaceMainView(IEventAggregator aggregator,
            TestOptions test, YachtRaceVMData model
            )
        {
            _aggregator = aggregator;
            _aggregator.Subscribe(this);
            _model = model;
            StackLayout mainStack = new StackLayout();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer(nameof(YachtRaceMainViewModel.RestoreScreen));
            }


            var thisRoll = GetGamingButton("Roll Dice", nameof(YachtRaceMainViewModel.RollDiceAsync));
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            _diceControl = new DiceListControlXF<SimpleDice>();
            mainStack.Children.Add(_diceControl);
            otherStack.Children.Add(thisRoll);
            var endButton = GetGamingButton("Has 5 Of A Kind", nameof(YachtRaceMainViewModel.FiveKindAsync));
            endButton.HorizontalOptions = LayoutOptions.Start;
            otherStack.Children.Add(endButton);
            mainStack.Children.Add(otherStack);
            _score = new ScoreBoardXF();
            _score.AddColumn("Time", true, nameof(YachtRacePlayerItem.Time));
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(YachtRaceMainViewModel.NormalTurn)); // there is no roll number needed for this game.
            firstInfo.AddRow("Status", nameof(YachtRaceMainViewModel.Status));
            firstInfo.AddRow("Error Message", nameof(YachtRaceMainViewModel.ErrorMessage));


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
            YachtRaceSaveInfo save = cons!.Resolve<YachtRaceSaveInfo>(); //usually needs this part for multiplayer games.
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
