using BasicControlsAndWindowsCore.Controls.NavigationControls;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using BasicGamingUIWPFLibrary.Helpers;
using BowlingDiceGameCP.Logic;
using BowlingDiceGameCP.ViewModels;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows;
using System.Windows.Controls;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.

namespace BowlingDiceGameWPF.Views
{
    public class BowlingDiceGameMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly IGamePackageResolver _resolver;
        private readonly DiceListWPF _diceBoard;
        private readonly BowlingCompleteScoresheetWPF _completeBoard;

        public BowlingDiceGameMainView(IEventAggregator aggregator,
            TestOptions test, IGamePackageResolver resolver
            )
        {
            _aggregator = aggregator;
            _resolver = resolver;
            _aggregator.Subscribe(this);
            StackPanel mainStack = new StackPanel();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer()
                {
                    Name = nameof(BowlingDiceGameMainViewModel.RestoreScreen)
                };
            }
            _completeBoard = new BowlingCompleteScoresheetWPF();
            mainStack.Children.Add(_completeBoard);
            StackPanel tempStack = new StackPanel();
            tempStack.Orientation = Orientation.Horizontal;
            var button = GetGamingButton("Roll", nameof(BowlingDiceGameMainViewModel.RollAsync));
            _diceBoard = new DiceListWPF();
            tempStack.Children.Add(_diceBoard);
            tempStack.Children.Add(button); // the roll dice should be on the right side
            mainStack.Children.Add(tempStack); // hopefully thissimple

            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(BowlingDiceGameMainViewModel.NormalTurn));
            firstInfo.AddRow("Frame", nameof(BowlingDiceGameMainViewModel.WhatFrame)); // i think so we know what frame its on.
            firstInfo.AddRow("Status", nameof(BowlingDiceGameMainViewModel.Status));
            mainStack.Children.Add(firstInfo.GetContent);

            button = GetGamingButton("Continue", nameof(BowlingDiceGameMainViewModel.ContinueTurnAsync));
            button.HorizontalAlignment = HorizontalAlignment.Left;
            mainStack.Children.Add(button);
            var endButton = GetGamingButton("End Turn", nameof(BowlingDiceGameMainViewModel.EndTurnAsync));
            endButton.HorizontalAlignment = HorizontalAlignment.Left;
            mainStack.Children.Add(endButton);

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
            var mainGame = _resolver.Resolve<BowlingDiceGameMainGameClass>();
            _completeBoard.LoadPlayerScores();
            _diceBoard.LoadBoard(mainGame.DiceBoard!.DiceList);
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
