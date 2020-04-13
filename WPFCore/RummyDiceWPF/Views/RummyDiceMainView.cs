using BasicControlsAndWindowsCore.Controls.NavigationControls;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIWPFLibrary.BasicControls.GameFrames;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using BasicGamingUIWPFLibrary.Helpers;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using RummyDiceCP.Data;
using RummyDiceCP.Logic;
using RummyDiceCP.ViewModels;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows;
using System.Windows.Controls;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.

namespace RummyDiceWPF.Views
{
    public class RummyDiceMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly RummyDiceMainGameClass _mainGame;
        readonly CustomBasicList<RummyDiceHandWPF> _tempList = new CustomBasicList<RummyDiceHandWPF>();
        private readonly ScoreBoardWPF _thisScore;
        private readonly RummyDiceListWPF _diceControl;
        public RummyDiceMainView(IEventAggregator aggregator,
            TestOptions test,
            RummyDiceMainGameClass mainGame
            )
        {
            _aggregator = aggregator;
            _mainGame = mainGame;
            _aggregator.Subscribe(this);
            StackPanel mainStack = new StackPanel();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer()
                {
                    Name = nameof(RummyDiceMainViewModel.RestoreScreen)
                };
            }
            StackPanel tempSets = new StackPanel();
            RummyDiceHandWPF thisTemp = new RummyDiceHandWPF();
            thisTemp.Index = 1;
            tempSets.Children.Add(thisTemp);
            _tempList!.Add(thisTemp); //forgot this too.
            thisTemp = new RummyDiceHandWPF();
            thisTemp.Index = 2;
            tempSets.Children.Add(thisTemp);
            _tempList.Add(thisTemp);

            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(tempSets);
            _thisScore = new ScoreBoardWPF();
            otherStack.Children.Add(_thisScore);
            mainStack.Children.Add(otherStack);
            SimpleLabelGrid otherScore = new SimpleLabelGrid();
            otherScore.AddRow("Roll", nameof(RummyDiceMainViewModel.RollNumber), new RollConverter());
            otherScore.AddRow("Score", nameof(RummyDiceMainViewModel.Score));
            var tempGrid = otherScore.GetContent;
            tempGrid.VerticalAlignment = VerticalAlignment.Bottom; // try this way
            otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(tempGrid);
            Button thisBut;
            thisBut = GetGamingButton("Put Back", nameof(RummyDiceMainViewModel.BoardAsync));
            thisBut.Margin = new Thickness(5, 0, 0, 0);
            otherStack.Children.Add(thisBut);
            mainStack.Children.Add(otherStack);
            otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            _thisScore.AddColumn("Score Round", false, nameof(RummyDicePlayerItem.ScoreRound));
            _thisScore.AddColumn("Score Game", false, nameof(RummyDicePlayerItem.ScoreGame));
            _thisScore.AddColumn("Phase", false, nameof(RummyDicePlayerItem.Phase));
            _thisScore.VerticalAlignment = VerticalAlignment.Top;
            otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            mainStack.Children.Add(otherStack);
            _diceControl = new RummyDiceListWPF();
            thisBut = GetGamingButton("Roll", nameof(RummyDiceMainViewModel.RollAsync));
            thisBut.HorizontalAlignment = HorizontalAlignment.Left;
            otherStack.Children.Add(thisBut);
            otherStack.Children.Add(_diceControl);
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(RummyDiceMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(RummyDiceMainViewModel.Status));
            firstInfo.AddRow("Phase", nameof(RummyDiceMainViewModel.CurrentPhase));
            otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            StackPanel bottomStack = new StackPanel();
            otherStack.Children.Add(bottomStack);
            var endButton = GetGamingButton("End Turn", nameof(RummyDiceMainViewModel.EndTurnAsync));
            endButton.HorizontalAlignment = HorizontalAlignment.Left;
            endButton.Margin = new Thickness(5, 0, 0, 20);
            bottomStack.Children.Add(endButton);
            thisBut = GetGamingButton("Score Hand", nameof(RummyDiceMainViewModel.CheckAsync));
            thisBut.Margin = new Thickness(5, 0, 0, 0);
            bottomStack.Children.Add(thisBut);
            otherStack.Children.Add(firstInfo.GetContent);
            mainStack.Children.Add(otherStack);


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
            if (_mainGame!.TempSets!.Count == 0)
                throw new BasicBlankException("You need to have 2 sets, not none for tempsets");
            _mainGame.TempSets.ForEach(tempCP =>
            {
                RummyDiceHandWPF thisTemp = _tempList.Single(items => items.Index == tempCP.Index);
                thisTemp.LoadList(tempCP, _mainGame);
            });
            _diceControl!.LoadDiceViewModel(_mainGame);
            _thisScore!.LoadLists(_mainGame.PlayerList!); // i think

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
