using System;
using System.Text;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using System.Linq;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using fs = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.FileHelpers;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
using System.Windows.Controls;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using CommonBasicStandardLibraries.Messenging;
using BasicGameFrameworkLibrary.BasicEventModels;
using SequenceDiceCP.Data;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicControlsAndWindowsCore.Controls.NavigationControls;
using SequenceDiceCP.ViewModels;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //usually needs this as well for grid helpers.
using BasicGamingUIWPFLibrary.Helpers;
using System.Windows;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using BasicGamingUIWPFLibrary.BasicControls.Misc;
using BasicGameFrameworkLibrary.Dice;

namespace SequenceDiceWPF.Views
{
    public class SequenceDiceMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly SequenceDiceVMData _model;
        readonly DiceListControlWPF<SimpleDice> _diceControl; //i think.
        private readonly GameBoardWPF _board;

        public SequenceDiceMainView(IEventAggregator aggregator,
            TestOptions test,
            SequenceDiceVMData model
            )
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);
            _board = new GameBoardWPF();
            _board.Margin = new Thickness(3);

            StackPanel mainStack = new StackPanel();
            ParentSingleUIContainer? restoreP = null;
            StackPanel tempStack = new StackPanel();
            tempStack.Orientation = Orientation.Horizontal;
            mainStack.Children.Add(tempStack);
            tempStack.Children.Add(_board);
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer()
                {
                    Name = nameof(SequenceDiceMainViewModel.RestoreScreen)
                };
            }

            var thisRoll = GetGamingButton("Roll Dice", nameof(SequenceDiceMainViewModel.RollDiceAsync));
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            _diceControl = new DiceListControlWPF<SimpleDice>();
            otherStack.Children.Add(_diceControl);
            otherStack.Children.Add(thisRoll);

            mainStack.Children.Add(otherStack);

            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(SequenceDiceMainViewModel.NormalTurn));
            firstInfo.AddRow("Instructions", nameof(SequenceDiceMainViewModel.Instructions));
            firstInfo.AddRow("Status", nameof(SequenceDiceMainViewModel.Status));
            //if we need to put to main, just change to main (?)
            tempStack.Children.Add(firstInfo.GetContent);
            if (restoreP != null)
            {
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
