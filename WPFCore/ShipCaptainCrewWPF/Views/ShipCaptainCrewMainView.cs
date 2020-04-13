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
using ShipCaptainCrewCP.Data;
using ShipCaptainCrewCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows.Controls;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;

namespace ShipCaptainCrewWPF.Views
{
    public class ShipCaptainCrewMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly ShipCaptainCrewVMData _model;
        readonly ScoreBoardWPF _score;
        readonly DiceListControlWPF<SimpleDice> _diceControl; //i think.

        public ShipCaptainCrewMainView(IEventAggregator aggregator,
            TestOptions test, ShipCaptainCrewVMData model
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
                    Name = nameof(ShipCaptainCrewMainViewModel.RestoreScreen)
                };
            }

            var thisRoll = GetGamingButton("Roll Dice", nameof(ShipCaptainCrewMainViewModel.RollDiceAsync));
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            _diceControl = new DiceListControlWPF<SimpleDice>();
            otherStack.Children.Add(_diceControl);
            otherStack.Children.Add(thisRoll);
            mainStack.Children.Add(otherStack);
            _score = new ScoreBoardWPF();
            _score.UseAbbreviationForTrueFalse = false;
            _score.AddColumn("Score", true, nameof(ShipCaptainCrewPlayerItem.Score));
            _score.AddColumn("Out", true, nameof(ShipCaptainCrewPlayerItem.WentOut), useTrueFalse: true);
            _score.AddColumn("Wins", true, nameof(ShipCaptainCrewPlayerItem.Wins));
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(ShipCaptainCrewMainViewModel.NormalTurn)); // there is no roll number needed for this game.
            firstInfo.AddRow("Roll", nameof(ShipCaptainCrewMainViewModel.RollNumber)); //if you don't need, it comment it out.
            firstInfo.AddRow("Status", nameof(ShipCaptainCrewMainViewModel.Status));




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
            ShipCaptainCrewSaveInfo save = cons!.Resolve<ShipCaptainCrewSaveInfo>(); //usually needs this part for multiplayer games.
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
