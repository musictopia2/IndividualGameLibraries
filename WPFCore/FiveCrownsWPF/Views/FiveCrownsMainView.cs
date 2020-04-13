using BasicControlsAndWindowsCore.Controls.NavigationControls;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIWPFLibrary.BasicControls.GameFrames;
using BasicGamingUIWPFLibrary.BasicControls.Misc;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using BasicGamingUIWPFLibrary.BasicControls.SingleCardFrames;
using BasicGamingUIWPFLibrary.Helpers;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using FiveCrownsCP.Cards;
using FiveCrownsCP.Data;
using FiveCrownsCP.Logic;
using FiveCrownsCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows;
using System.Windows.Controls;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //usually needs this as well for grid helpers.
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;

namespace FiveCrownsWPF.Views
{
    public class FiveCrownsMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly FiveCrownsVMData _model;
        private readonly BaseDeckWPF<FiveCrownsCardInformation, FiveCrownsGraphicsCP, CardGraphicsWPF> _deckGPile;
        private readonly BasePileWPF<FiveCrownsCardInformation, FiveCrownsGraphicsCP, CardGraphicsWPF> _discardGPile;

        private readonly ScoreBoardWPF _score;
        private readonly BaseHandWPF<FiveCrownsCardInformation, FiveCrownsGraphicsCP, CardGraphicsWPF> _playerHandWPF;

        private readonly TempRummySetsWPF<EnumSuitList, EnumColorList, FiveCrownsCardInformation, FiveCrownsGraphicsCP, CardGraphicsWPF> _tempG;
        private readonly MainRummySetsWPF<EnumSuitList, EnumColorList, FiveCrownsCardInformation, FiveCrownsGraphicsCP, CardGraphicsWPF, PhaseSet, SavedSet> _mainG;

        public FiveCrownsMainView(IEventAggregator aggregator,
            TestOptions test,
            FiveCrownsVMData model
            )
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);

            _deckGPile = new BaseDeckWPF<FiveCrownsCardInformation, FiveCrownsGraphicsCP, CardGraphicsWPF>();
            _discardGPile = new BasePileWPF<FiveCrownsCardInformation, FiveCrownsGraphicsCP, CardGraphicsWPF>();
            _score = new ScoreBoardWPF();
            _playerHandWPF = new BaseHandWPF<FiveCrownsCardInformation, FiveCrownsGraphicsCP, CardGraphicsWPF>();
            _tempG = new TempRummySetsWPF<EnumSuitList, EnumColorList, FiveCrownsCardInformation, FiveCrownsGraphicsCP, CardGraphicsWPF>();
            _mainG = new MainRummySetsWPF<EnumSuitList, EnumColorList, FiveCrownsCardInformation, FiveCrownsGraphicsCP, CardGraphicsWPF, PhaseSet, SavedSet>();

            StackPanel mainStack = new StackPanel();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer()
                {
                    Name = nameof(FiveCrownsMainViewModel.RestoreScreen)
                };
            }


            Grid finalGrid = new Grid();
            AddLeftOverRow(finalGrid, 20); // has to be this way because of scoreboard.
            AddLeftOverRow(finalGrid, 80);
            mainStack.Children.Add(finalGrid);
            Grid firstGrid = new Grid();
            AddLeftOverColumn(firstGrid, 40); // 50 was too much.  if there is scrolling, i guess okay.
            AddLeftOverColumn(firstGrid, 10); // for buttons (can change if necessary)
            AddAutoColumns(firstGrid, 1); // maybe 1 (well see)
            AddLeftOverColumn(firstGrid, 15); // for other details
            AddLeftOverColumn(firstGrid, 30); // for scoreboard
            _deckGPile.HorizontalAlignment = HorizontalAlignment.Left;
            _deckGPile.VerticalAlignment = VerticalAlignment.Top;
            _discardGPile.HorizontalAlignment = HorizontalAlignment.Left;
            _discardGPile.VerticalAlignment = VerticalAlignment.Top;
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            AddControlToGrid(firstGrid, otherStack, 0, 2); // i think
            AddControlToGrid(firstGrid, _playerHandWPF, 0, 0); // i think
            var thisBut = GetGamingButton("Lay" + Constants.vbCrLf + "Down", nameof(FiveCrownsMainViewModel.LayDownSetsAsync));
            StackPanel tempStack = new StackPanel();
            tempStack.Orientation = Orientation.Horizontal;
            tempStack.Children.Add(thisBut);
            thisBut = GetGamingButton("Back", nameof(FiveCrownsMainViewModel.Back));
            thisBut.FontSize -= 4;
            tempStack.Children.Add(thisBut);
            AddControlToGrid(firstGrid, tempStack, 0, 1);
            _score.AddColumn("Cards Left", true, nameof(FiveCrownsPlayerItem.ObjectCount)); //very common.
            _score.AddColumn("Current Score", true, nameof(FiveCrownsPlayerItem.CurrentScore));
            _score.AddColumn("Total Score", true, nameof(FiveCrownsPlayerItem.TotalScore));
            AddControlToGrid(firstGrid, _score, 0, 4);
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(FiveCrownsMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(FiveCrownsMainViewModel.Status));
            firstInfo.AddRow("Up To", nameof(FiveCrownsMainViewModel.UpTo));
            AddControlToGrid(firstGrid, firstInfo.GetContent, 0, 3);
            AddControlToGrid(finalGrid, firstGrid, 0, 0); // i think
            _tempG.Height = 700;
            StackPanel thirdStack = new StackPanel();
            thirdStack.Orientation = Orientation.Horizontal;
            thirdStack.Children.Add(_tempG);
            _mainG.Height = 700; // try this way.
            thirdStack.Children.Add(_mainG);
            AddControlToGrid(finalGrid, thirdStack, 1, 0); // i think


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

            FiveCrownsSaveInfo save = cons!.Resolve<FiveCrownsSaveInfo>(); //usually needs this part for multiplayer games.

            _score!.LoadLists(save.PlayerList);
            _playerHandWPF!.LoadList(_model.PlayerHand1!, ""); // i think
            _discardGPile!.Init(_model.Pile1!, ""); // may have to be here (well see)
            _discardGPile.StartListeningDiscardPile(); // its the main one.

            _deckGPile!.Init(_model.Deck1!, ""); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();
            _tempG!.Init(_model!.TempSets!, "");
            _mainG!.Init(_model!.MainSets!, "");
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
