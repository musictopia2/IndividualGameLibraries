using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.Dice;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIXFLibrary.BasicControls.Misc;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicGamingUIXFLibrary.Helpers;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using SequenceDiceCP.Data;
using SequenceDiceCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;

namespace SequenceDiceXF.Views
{
    public class SequenceDiceMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly SequenceDiceVMData _model;
        readonly DiceListControlXF<SimpleDice> _diceControl; //i think.
        private readonly GameBoardXF _board;
        public SequenceDiceMainView(IEventAggregator aggregator,
            TestOptions test,
            SequenceDiceVMData model
            )
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);
            _board = new GameBoardXF();
            _board.Margin = new Thickness(2);
            StackLayout mainStack = new StackLayout();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer(nameof(SequenceDiceMainViewModel.RestoreScreen));
            }

            mainStack.Children.Add(_board);


            var thisRoll = GetGamingButton("Roll Dice", nameof(SequenceDiceMainViewModel.RollDiceAsync));
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            _diceControl = new DiceListControlXF<SimpleDice>();
            otherStack.Children.Add(_diceControl);
            otherStack.Children.Add(thisRoll);
            mainStack.Children.Add(otherStack);


            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(SequenceDiceMainViewModel.NormalTurn));
            firstInfo.AddRow("Instructions", nameof(SequenceDiceMainViewModel.Instructions));
            firstInfo.AddRow("Status", nameof(SequenceDiceMainViewModel.Status));
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

            SequenceDiceSaveInfo save = cons!.Resolve<SequenceDiceSaveInfo>(); //usually needs this part for multiplayer games.
            _diceControl!.LoadDiceViewModel(_model.Cup!);
            _board.CreateControls(save.GameBoard);
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
            _aggregator.Unsubscribe(this);
            return Task.CompletedTask;
        }
    }
}
