using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicGamingUIXFLibrary.Helpers;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using BowlingDiceGameCP.Logic;
using BowlingDiceGameCP.ViewModels;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.

namespace BowlingDiceGameXF.Views
{
    public class BowlingDiceGameMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly IGamePackageResolver _resolver;
        private readonly DiceListXF _diceBoard;
        private readonly BowlingCompleteScoresheetXF _completeBoard;

        public BowlingDiceGameMainView(IEventAggregator aggregator,
            TestOptions test, IGamePackageResolver resolver
            )
        {
            _aggregator = aggregator;
            _resolver = resolver;
            _aggregator.Subscribe(this);
            StackLayout mainStack = new StackLayout();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer(nameof(BowlingDiceGameMainViewModel.RestoreScreen));
            }
            _completeBoard = new BowlingCompleteScoresheetXF();
            mainStack.Children.Add(_completeBoard);
            StackLayout tempStack = new StackLayout();
            tempStack.Orientation = StackOrientation.Horizontal;
            var button = GetGamingButton("Roll", nameof(BowlingDiceGameMainViewModel.RollAsync));
            _diceBoard = new DiceListXF();
            tempStack.Children.Add(_diceBoard);
            tempStack.Children.Add(button); // the roll dice should be on the right side
            mainStack.Children.Add(tempStack); // hopefully thissimple

            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(BowlingDiceGameMainViewModel.NormalTurn));
            firstInfo.AddRow("Frame", nameof(BowlingDiceGameMainViewModel.WhatFrame)); // i think so we know what frame its on.
            firstInfo.AddRow("Status", nameof(BowlingDiceGameMainViewModel.Status));
            mainStack.Children.Add(firstInfo.GetContent);

            button = GetGamingButton("Continue", nameof(BowlingDiceGameMainViewModel.ContinueTurnAsync));
            button.HorizontalOptions = LayoutOptions.Start;
            mainStack.Children.Add(button);
            var endButton = GetGamingButton("End Turn", nameof(BowlingDiceGameMainViewModel.EndTurnAsync));
            endButton.HorizontalOptions = LayoutOptions.Start;
            mainStack.Children.Add(endButton);

            if (restoreP != null)
            {
                //todo:  figure out where to place the restore ui if there is a restore option.
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }
            GamePackageViewModelBinder.ManuelElements.Clear(); //often times i have to add manually.
            var mainGame = _resolver.Resolve<BowlingDiceGameMainGameClass>();
            _completeBoard.LoadPlayerScores();
            _diceBoard.LoadBoard(mainGame.DiceBoard!.DiceList);
            Content = mainStack;

        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {
            return Task.CompletedTask;
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
