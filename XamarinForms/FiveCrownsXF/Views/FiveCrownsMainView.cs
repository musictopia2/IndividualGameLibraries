using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIXFLibrary.BasicControls.GameFrames;
using BasicGamingUIXFLibrary.BasicControls.Misc;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicGamingUIXFLibrary.BasicControls.SingleCardFrames;
using BasicGamingUIXFLibrary.Helpers;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using FiveCrownsCP.Cards;
using FiveCrownsCP.Data;
using FiveCrownsCP.Logic;
using FiveCrownsCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using static BasicXFControlsAndPages.Helpers.GridHelper;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;

namespace FiveCrownsXF.Views
{
    public class FiveCrownsMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly FiveCrownsVMData _model;
        private readonly BaseDeckXF<FiveCrownsCardInformation, FiveCrownsGraphicsCP, CardGraphicsXF> _deckGPile;
        private readonly BasePileXF<FiveCrownsCardInformation, FiveCrownsGraphicsCP, CardGraphicsXF> _discardGPile;

        private readonly ScoreBoardXF _score;
        private readonly BaseHandXF<FiveCrownsCardInformation, FiveCrownsGraphicsCP, CardGraphicsXF> _playerHandWPF;

        private readonly TempRummySetsXF<EnumSuitList, EnumColorList, FiveCrownsCardInformation, FiveCrownsGraphicsCP, CardGraphicsXF> _tempG;
        private readonly MainRummySetsXF<EnumSuitList, EnumColorList, FiveCrownsCardInformation, FiveCrownsGraphicsCP, CardGraphicsXF, PhaseSet, SavedSet> _mainG;


        public FiveCrownsMainView(IEventAggregator aggregator,
            TestOptions test,
            FiveCrownsVMData model
            )
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);

            _deckGPile = new BaseDeckXF<FiveCrownsCardInformation, FiveCrownsGraphicsCP, CardGraphicsXF>();
            _discardGPile = new BasePileXF<FiveCrownsCardInformation, FiveCrownsGraphicsCP, CardGraphicsXF>();
            _score = new ScoreBoardXF();
            _playerHandWPF = new BaseHandXF<FiveCrownsCardInformation, FiveCrownsGraphicsCP, CardGraphicsXF>();
            _tempG = new TempRummySetsXF<EnumSuitList, EnumColorList, FiveCrownsCardInformation, FiveCrownsGraphicsCP, CardGraphicsXF>();
            _mainG = new MainRummySetsXF<EnumSuitList, EnumColorList, FiveCrownsCardInformation, FiveCrownsGraphicsCP, CardGraphicsXF, PhaseSet, SavedSet>();

            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer(nameof(FiveCrownsMainViewModel.RestoreScreen));
            }

            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            _score.AddColumn("Cards Left", true, nameof(FiveCrownsPlayerItem.ObjectCount)); //very common.
            _score.AddColumn("Current Score", true, nameof(FiveCrownsPlayerItem.CurrentScore));
            _score.AddColumn("Total Score", true, nameof(FiveCrownsPlayerItem.TotalScore));
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(FiveCrownsMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(FiveCrownsMainViewModel.Status));
            firstInfo.AddRow("Up To", nameof(FiveCrownsMainViewModel.UpTo));


            Grid finalGrid = new Grid();
            AddAutoRows(finalGrid, 1);
            AddLeftOverRow(finalGrid, 1);
            Grid firstGrid = new Grid();
            AddLeftOverColumn(firstGrid, 40);
            AddAutoColumns(firstGrid, 1);
            AddLeftOverColumn(firstGrid, 15);
            AddLeftOverColumn(firstGrid, 30);

            var thisBut = GetSmallerButton("Lay Down", nameof(FiveCrownsMainViewModel.LayDownSetsAsync));


            AddControlToGrid(firstGrid, otherStack, 0, 1);
            StackLayout firstStack = new StackLayout();
            firstStack.Children.Add(_playerHandWPF);
            StackLayout secondStack = new StackLayout();
            secondStack.Orientation = StackOrientation.Horizontal;
            firstStack.Children.Add(secondStack);
            firstStack.Children.Add(thisBut);
            thisBut = GetSmallerButton("Back", nameof(FiveCrownsMainViewModel.Back));
            firstStack.Children.Add(thisBut);

            AddControlToGrid(firstGrid, firstStack, 0, 0);
            AddControlToGrid(firstGrid, _score, 0, 3);
            AddControlToGrid(finalGrid, firstGrid, 0, 0);
            AddControlToGrid(firstGrid, firstInfo.GetContent, 0, 2);
            _tempG.Divider = 1.1;
            StackLayout thirdStack = new StackLayout();
            thirdStack.Orientation = StackOrientation.Horizontal;
            thirdStack.Children.Add(_tempG);
            thirdStack.Children.Add(_mainG);
            AddControlToGrid(finalGrid, thirdStack, 1, 0); // i think
            _deckGPile.Margin = new Thickness(5, 5, 5, 5);

            _discardGPile.Margin = new Thickness(5, 5, 5, 5);

            if (restoreP != null)
            {
                otherStack.Children.Add(restoreP); //default add to grid but does not have to.
            }
            Content = finalGrid;
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
            _playerHandWPF.Dispose(); //at least will help improve performance.
            _mainG.Dispose();
            _tempG.Dispose();
            return Task.CompletedTask;
        }
    }
}
