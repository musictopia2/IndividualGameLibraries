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
using A21DiceGameCP.Data;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicControlsAndWindowsCore.Controls.NavigationControls;
using A21DiceGameCP.ViewModels;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //usually needs this as well for grid helpers.
using BasicGamingUIWPFLibrary.Helpers;
using System.Windows;
using BasicGamingUIWPFLibrary.BasicControls.GameFrames;
using BasicGamingUIWPFLibrary.BasicControls.Misc;
using BasicGameFrameworkLibrary.Dice;
using BasicGameFrameworkLibrary.MultiplayerClasses.MainViewModels;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;

namespace A21DiceGameWPF.Views
{
    public class A21DiceGameMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly A21DiceGameVMData _model;
        readonly ScoreBoardWPF _score;
        readonly DiceListControlWPF<SimpleDice> _diceControl; //i think.

        public A21DiceGameMainView(IEventAggregator aggregator,
            TestOptions test, A21DiceGameVMData model
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
                    Name = nameof(A21DiceGameMainViewModel.RestoreScreen)
                };
            }

            var thisRoll = GetGamingButton("Roll Dice", nameof(A21DiceGameMainViewModel.RollDiceAsync));
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            _diceControl = new DiceListControlWPF<SimpleDice>();
            otherStack.Children.Add(_diceControl);
            otherStack.Children.Add(thisRoll);
            var endButton = GetGamingButton("End Turn", nameof(A21DiceGameMainViewModel.EndTurnAsync));
            endButton.HorizontalAlignment = HorizontalAlignment.Left;
            otherStack.Children.Add(endButton);
            mainStack.Children.Add(otherStack);
            _score = new ScoreBoardWPF();
            //anything you need for the scoreboard.
            _score.AddColumn("# Of Rolls", true, nameof(A21DiceGamePlayerItem.NumberOfRolls));
            _score.AddColumn("Score", true, nameof(A21DiceGamePlayerItem.Score));
            _score.AddColumn("Was Tie", true, nameof(A21DiceGamePlayerItem.IsFaceOff), useTrueFalse: true);
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(A21DiceGameMainViewModel.NormalTurn)); // there is no roll number needed for this game.
            firstInfo.AddRow("Status", nameof(A21DiceGameMainViewModel.Status));




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
            A21DiceGameSaveInfo save = cons!.Resolve<A21DiceGameSaveInfo>(); //usually needs this part for multiplayer games.
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
