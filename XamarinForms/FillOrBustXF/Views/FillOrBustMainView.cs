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
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using static BasicXFControlsAndPages.Helpers.GridHelper;
using CommonBasicStandardLibraries.Messenging;
using BasicGameFrameworkLibrary.BasicEventModels;
using FillOrBustCP.Data;
using Xamarin.Forms;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using BasicGameFrameworkLibrary.TestUtilities;
using FillOrBustCP.ViewModels;
using BasicGamingUIXFLibrary.Helpers;
using BasicGamingUIXFLibrary.BasicControls.SingleCardFrames;
using BasicGamingUIXFLibrary.GameGraphics.Cards;
using BasicGamingUIXFLibrary.BasicControls.GameFrames;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using FillOrBustCP.Cards;
using BasicGamingUIXFLibrary.BasicControls.Misc;
using BasicGameFrameworkLibrary.Dice;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.DataClasses;
using static BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;

namespace FillOrBustXF.Views
{
    public class FillOrBustMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly FillOrBustVMData _model;
        private readonly BaseDeckXF<FillOrBustCardInformation, FillOrBustGraphicsCP, CardGraphicsXF> _deckGPile;
        private readonly BasePileXF<FillOrBustCardInformation, FillOrBustGraphicsCP, CardGraphicsXF> _discardGPile;

        private readonly ScoreBoardXF _score;
        private readonly DiceListControlXF<SimpleDice> _diceControl;
        public FillOrBustMainView(IEventAggregator aggregator,
            TestOptions test,
            FillOrBustVMData model
            )
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);

            _deckGPile = new BaseDeckXF<FillOrBustCardInformation, FillOrBustGraphicsCP, CardGraphicsXF>();
            _discardGPile = new BasePileXF<FillOrBustCardInformation, FillOrBustGraphicsCP, CardGraphicsXF>();
            _score = new ScoreBoardXF();
            _diceControl = new DiceListControlXF<SimpleDice>();
            StackLayout mainStack = new StackLayout();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer(nameof(FillOrBustMainViewModel.RestoreScreen));
            }

            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.

            Grid firstGrid = new Grid();
            AddAutoRows(firstGrid, 1);
            AddAutoColumns(firstGrid, 1);
            AddLeftOverColumn(firstGrid, 1);
            AddControlToGrid(firstGrid, otherStack, 0, 0);

            
            SimpleLabelGridXF tempInfo = new SimpleLabelGridXF();
            tempInfo.AddRow("Temporary Score", nameof(FillOrBustMainViewModel.TempScore));
            tempInfo.AddRow("Score", nameof(FillOrBustMainViewModel.DiceScore));
            otherStack.Children.Add(tempInfo.GetContent);

            _score.AddColumn("Current Score", true, nameof(FillOrBustPlayerItem.CurrentScore), rightMargin: 10);
            _score.AddColumn("Total Score", true, nameof(FillOrBustPlayerItem.TotalScore), rightMargin: 10);
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(FillOrBustMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(FillOrBustMainViewModel.Status));

            if (ScreenUsed != EnumScreen.SmallPhone)
            {
                firstInfo.AddRow("Instructions", nameof(FillOrBustMainViewModel.Instructions));
            }

            Grid finGrid = new Grid();
            AddAutoColumns(finGrid, 1);
            AddAutoRows(finGrid, 2);
            AddLeftOverColumn(finGrid, 1);


            AddControlToGrid(finGrid, _score, 0, 0);
            otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            var button = GetSmallerButton("Roll Dice", nameof(FillOrBustMainViewModel.RollDiceAsync));
            button.Margin = new Thickness(0, 0, 5, 0);
            otherStack.Children.Add(button);
            button = GetSmallerButton("Remove Dice", nameof(FillOrBustMainViewModel.ChooseDiceAsync));
            button.Margin = new Thickness(0, 0, 5, 0);
            otherStack.Children.Add(button);
            var endButton = GetSmallerButton("End Turn", nameof(FillOrBustMainViewModel.EndTurnAsync));
            otherStack.Children.Add(endButton);
            otherStack.Children.Add(tempInfo.GetContent);
            var temps = firstInfo.GetContent;
            AddControlToGrid(finGrid, temps, 1, 0);
            Grid.SetColumnSpan(temps, 2);
            AddControlToGrid(firstGrid, finGrid, 0, 1);
            mainStack.Children.Add(firstGrid);
            mainStack.Children.Add(_diceControl);
            mainStack.Children.Add(otherStack);


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
