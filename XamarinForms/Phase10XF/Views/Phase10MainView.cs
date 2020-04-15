using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.ColorCards;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIXFLibrary.BasicControls.GameFrames;
using BasicGamingUIXFLibrary.BasicControls.Misc;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicGamingUIXFLibrary.BasicControls.SingleCardFrames;
using BasicGamingUIXFLibrary.Helpers;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using Phase10CP.Cards;
using Phase10CP.Data;
using Phase10CP.SetClasses;
using Phase10CP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using static BasicXFControlsAndPages.Helpers.GridHelper;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;

namespace Phase10XF.Views
{
    public class Phase10MainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly Phase10VMData _model;
        private readonly BaseDeckXF<Phase10CardInformation, Phase10GraphicsCP, CardGraphicsXF> _deckGPile;
        private readonly BasePileXF<Phase10CardInformation, Phase10GraphicsCP, CardGraphicsXF> _discardGPile;

        private readonly ScoreBoardXF _score;
        private readonly BaseHandXF<Phase10CardInformation, Phase10GraphicsCP, CardGraphicsXF> _playerHandWPF;

        private readonly TempRummySetsXF<EnumColorTypes, EnumColorTypes, Phase10CardInformation, Phase10GraphicsCP, CardGraphicsXF> _tempG;
        private readonly MainRummySetsXF<EnumColorTypes, EnumColorTypes,
            Phase10CardInformation, Phase10GraphicsCP, CardGraphicsXF, PhaseSet, SavedSet> _mainG;

        public Phase10MainView(IEventAggregator aggregator,
            TestOptions test,
            Phase10VMData model
            )
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);

            _deckGPile = new BaseDeckXF<Phase10CardInformation, Phase10GraphicsCP, CardGraphicsXF>();
            _discardGPile = new BasePileXF<Phase10CardInformation, Phase10GraphicsCP, CardGraphicsXF>();
            _score = new ScoreBoardXF();
            _playerHandWPF = new BaseHandXF<Phase10CardInformation, Phase10GraphicsCP, CardGraphicsXF>();
            _tempG = new TempRummySetsXF<EnumColorTypes, EnumColorTypes, Phase10CardInformation, Phase10GraphicsCP, CardGraphicsXF>();
            _mainG = new MainRummySetsXF<EnumColorTypes, EnumColorTypes, Phase10CardInformation, Phase10GraphicsCP, CardGraphicsXF, PhaseSet, SavedSet>();

            Grid finalGrid = new Grid();
            AddAutoRows(finalGrid, 1); // has to be this way because of scoreboard.
            AddLeftOverRow(finalGrid, 1);


            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer(nameof(Phase10MainViewModel.RestoreScreen));
            }

            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.

            _score.AddColumn("Score", true, nameof(Phase10PlayerItem.TotalScore));
            _score.AddColumn("Cards Left", true, nameof(Phase10PlayerItem.ObjectCount));
            _score.AddColumn("Phase", true, nameof(Phase10PlayerItem.Phase));
            _score.AddColumn("Skipped", true, nameof(Phase10PlayerItem.MissNextTurn), useTrueFalse: true);
            _score.AddColumn("Completed", true, nameof(Phase10PlayerItem.Completed), useTrueFalse: true);

            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(Phase10MainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(Phase10MainViewModel.Status));
            firstInfo.AddRow("Phase", nameof(Phase10MainViewModel.CurrentPhase));


            Grid firstGrid = new Grid();
            AddLeftOverColumn(firstGrid, 40); // 50 was too much.  if there is scrolling, i guess okay.
            AddAutoColumns(firstGrid, 1); // maybe 1 (well see)
            AddLeftOverColumn(firstGrid, 15); // for other details
            AddLeftOverColumn(firstGrid, 30); // for scoreboard
            AddControlToGrid(firstGrid, otherStack, 0, 1);
            StackLayout firstStack = new StackLayout();
            firstStack.Children.Add(_playerHandWPF);
            var thisBut = GetSmallerButton("Complete" + Constants.vbCrLf + "Phase", nameof(Phase10MainViewModel.CompletePhaseAsync));
            firstStack.Children.Add(thisBut);
            AddControlToGrid(firstGrid, firstStack, 0, 0);
            AddControlToGrid(firstGrid, firstInfo.GetContent, 0, 2);
            AddControlToGrid(firstGrid, _score, 0, 3);
            AddControlToGrid(finalGrid, firstGrid, 0, 0);
            _tempG.Divider = 1.1;
            StackLayout thirdStack = new StackLayout();
            thirdStack.Orientation = StackOrientation.Horizontal;
            thirdStack.Children.Add(_tempG);
            thirdStack.Children.Add(_mainG);
            AddControlToGrid(finalGrid, thirdStack, 1, 0);





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

            Phase10SaveInfo save = cons!.Resolve<Phase10SaveInfo>(); //usually needs this part for multiplayer games.

            _score!.LoadLists(save.PlayerList);
            _playerHandWPF!.LoadList(_model.PlayerHand1!, ""); // i think
            _discardGPile!.Init(_model.Pile1!, ""); // may have to be here (well see)
            _discardGPile.StartListeningDiscardPile(); // its the main one.

            _deckGPile!.Init(_model.Deck1!, ""); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();
            _mainG!.Init(_model.MainSets!, "");
            _tempG!.Init(_model.TempSets!, "");
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
            _tempG.Dispose();
            _mainG.Dispose();
            return Task.CompletedTask;
        }
    }
}
