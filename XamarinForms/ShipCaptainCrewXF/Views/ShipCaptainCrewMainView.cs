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
using ShipCaptainCrewCP.Data;
using ShipCaptainCrewCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;

namespace ShipCaptainCrewXF.Views
{
    public class ShipCaptainCrewMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly ShipCaptainCrewVMData _model;
        readonly ScoreBoardXF _score;
        readonly DiceListControlXF<SimpleDice> _diceControl; //i think.
        public ShipCaptainCrewMainView(IEventAggregator aggregator,
            TestOptions test, ShipCaptainCrewVMData model
            )
        {
            _aggregator = aggregator;
            _aggregator.Subscribe(this);
            _model = model;
            StackLayout mainStack = new StackLayout();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer(nameof(ShipCaptainCrewMainViewModel.RestoreScreen));
            }


            var thisRoll = GetGamingButton("Roll Dice", nameof(ShipCaptainCrewMainViewModel.RollDiceAsync));
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            _diceControl = new DiceListControlXF<SimpleDice>();
            otherStack.Children.Add(_diceControl);
            otherStack.Children.Add(thisRoll);
            mainStack.Children.Add(otherStack);
            _score = new ScoreBoardXF();
            _score.UseAbbreviationForTrueFalse = false;
            _score.AddColumn("Score", true, nameof(ShipCaptainCrewPlayerItem.Score));
            _score.AddColumn("Out", true, nameof(ShipCaptainCrewPlayerItem.WentOut), useTrueFalse: true);
            _score.AddColumn("Wins", true, nameof(ShipCaptainCrewPlayerItem.Wins));

            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(ShipCaptainCrewMainViewModel.NormalTurn)); // there is no roll number needed for this game.
            firstInfo.AddRow("Roll", nameof(ShipCaptainCrewMainViewModel.RollNumber)); //if you don't need, it comment it out.
            firstInfo.AddRow("Status", nameof(ShipCaptainCrewMainViewModel.Status));

            //this is just a starting point.

            mainStack.Children.Add(firstInfo.GetContent);
            mainStack.Children.Add(_score);

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
            ShipCaptainCrewSaveInfo save = cons!.Resolve<ShipCaptainCrewSaveInfo>(); //usually needs this part for multiplayer games.
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
