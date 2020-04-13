using BasicControlsAndWindowsCore.Controls.NavigationControls;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.Dice;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIWPFLibrary.BasicControls.GameFrames;
using BasicGamingUIWPFLibrary.BasicControls.Misc;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using BasicGamingUIWPFLibrary.BasicControls.SingleCardFrames;
using BasicGamingUIWPFLibrary.Helpers;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using FillOrBustCP.Cards;
using FillOrBustCP.Data;
using FillOrBustCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows;
using System.Windows.Controls;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;

namespace FillOrBustWPF.Views
{
    public class FillOrBustMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly FillOrBustVMData _model;
        private readonly BaseDeckWPF<FillOrBustCardInformation, FillOrBustGraphicsCP, CardGraphicsWPF> _deckGPile;
        private readonly BasePileWPF<FillOrBustCardInformation, FillOrBustGraphicsCP, CardGraphicsWPF> _discardGPile;

        private readonly ScoreBoardWPF _score;
        private readonly DiceListControlWPF<SimpleDice> _diceControl;
        public FillOrBustMainView(IEventAggregator aggregator,
            TestOptions test,
            FillOrBustVMData model
            )
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);

            _deckGPile = new BaseDeckWPF<FillOrBustCardInformation, FillOrBustGraphicsCP, CardGraphicsWPF>();
            _discardGPile = new BasePileWPF<FillOrBustCardInformation, FillOrBustGraphicsCP, CardGraphicsWPF>();
            _score = new ScoreBoardWPF();
            _diceControl = new DiceListControlWPF<SimpleDice>();
            StackPanel mainStack = new StackPanel();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer()
                {
                    Name = nameof(FillOrBustMainViewModel.RestoreScreen)
                };
            }


            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            mainStack.Children.Add(otherStack);
            _score.AddColumn("Current Score", true, nameof(FillOrBustPlayerItem.CurrentScore), rightMargin: 10);
            _score.AddColumn("Total Score", true, nameof(FillOrBustPlayerItem.TotalScore), rightMargin: 10);
            otherStack.Children.Add(_score);
            mainStack.Children.Add(_diceControl);

            otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            mainStack.Children.Add(otherStack);
            var button = GetGamingButton("Roll Dice", nameof(FillOrBustMainViewModel.RollDiceAsync));
            button.Margin = new Thickness(0, 0, 5, 0);
            otherStack.Children.Add(button);
            button = GetGamingButton("Remove Dice", nameof(FillOrBustMainViewModel.ChooseDiceAsync));
            button.Margin = new Thickness(0, 0, 5, 0);
            otherStack.Children.Add(button);
            var endButton = GetGamingButton("End Turn", nameof(FillOrBustMainViewModel.EndTurnAsync));
            otherStack.Children.Add(endButton);
            SimpleLabelGrid tempInfo = new SimpleLabelGrid();
            tempInfo.AddRow("Temporary Score", nameof(FillOrBustMainViewModel.TempScore));
            tempInfo.AddRow("Score", nameof(FillOrBustMainViewModel.DiceScore));
            otherStack.Children.Add(tempInfo.GetContent);

            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(FillOrBustMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(FillOrBustMainViewModel.Status));

            mainStack.Children.Add(firstInfo.GetContent);


            _deckGPile.Margin = new Thickness(5, 5, 5, 5);

            _discardGPile.Margin = new Thickness(5, 5, 5, 5);


            if (restoreP != null)
            {
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }
            Content = mainStack;

        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {

            GamePackageViewModelBinder.ManuelElements.Clear(); //often times i have to add manually.

            FillOrBustSaveInfo save = cons!.Resolve<FillOrBustSaveInfo>(); //usually needs this part for multiplayer games.

            _score!.LoadLists(save.PlayerList);
            _discardGPile!.Init(_model.Pile1!, ""); // may have to be here (well see)
            _discardGPile.StartListeningDiscardPile(); // its the main one.

            _deckGPile!.Init(_model.Deck1!, ""); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();
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
