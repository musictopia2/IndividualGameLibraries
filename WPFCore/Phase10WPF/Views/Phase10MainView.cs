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
using Phase10CP.Data;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicControlsAndWindowsCore.Controls.NavigationControls;
using Phase10CP.ViewModels;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //usually needs this as well for grid helpers.
using BasicGamingUIWPFLibrary.Helpers;
using System.Windows;
using BasicGamingUIWPFLibrary.BasicControls.SingleCardFrames;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGamingUIWPFLibrary.GameGraphics.Cards;
using BasicGamingUIWPFLibrary.BasicControls.GameFrames;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using Phase10CP.Cards;
using BasicGamingUIWPFLibrary.BasicControls.Misc;
using BasicGameFrameworkLibrary.ColorCards;
using Phase10CP.SetClasses;
using BasicGameFrameworkLibrary.DrawableListsObservable;

namespace Phase10WPF.Views
{
    public class Phase10MainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly Phase10VMData _model;
        private readonly BaseDeckWPF<Phase10CardInformation, Phase10GraphicsCP, CardGraphicsWPF> _deckGPile;
        private readonly BasePileWPF<Phase10CardInformation, Phase10GraphicsCP, CardGraphicsWPF> _discardGPile;

        private readonly ScoreBoardWPF _score;
        private readonly BaseHandWPF<Phase10CardInformation, Phase10GraphicsCP, CardGraphicsWPF> _playerHandWPF;

        private readonly TempRummySetsWPF<EnumColorTypes, EnumColorTypes, Phase10CardInformation, Phase10GraphicsCP, CardGraphicsWPF> _tempG;
        private readonly MainRummySetsWPF<EnumColorTypes, EnumColorTypes,
            Phase10CardInformation, Phase10GraphicsCP, CardGraphicsWPF, PhaseSet, SavedSet> _mainG;
        public Phase10MainView(IEventAggregator aggregator,
            TestOptions test,
            Phase10VMData model
            )
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);
            _tempG = new TempRummySetsWPF<EnumColorTypes, EnumColorTypes, Phase10CardInformation, Phase10GraphicsCP, CardGraphicsWPF>();
            _mainG = new MainRummySetsWPF<EnumColorTypes, EnumColorTypes, Phase10CardInformation, Phase10GraphicsCP, CardGraphicsWPF, PhaseSet, SavedSet>();
            _deckGPile = new BaseDeckWPF<Phase10CardInformation, Phase10GraphicsCP, CardGraphicsWPF>();
            _discardGPile = new BasePileWPF<Phase10CardInformation, Phase10GraphicsCP, CardGraphicsWPF>();
            _score = new ScoreBoardWPF();
            _playerHandWPF = new BaseHandWPF<Phase10CardInformation, Phase10GraphicsCP, CardGraphicsWPF>();

            StackPanel mainStack = new StackPanel();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer()
                {
                    Name = nameof(Phase10MainViewModel.RestoreScreen)
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
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            AddControlToGrid(firstGrid, otherStack, 0, 2); // i think
            _playerHandWPF.HandType = HandObservable<Phase10CardInformation>.EnumHandList.Horizontal;
            AddControlToGrid(firstGrid, _playerHandWPF, 0, 0); // i think
            var thisBut = GetGamingButton("Complete" + Constants.vbCrLf + "Phase", nameof(Phase10MainViewModel.CompletePhaseAsync));
            AddControlToGrid(firstGrid, thisBut, 0, 1);
            _score.AddColumn("Score", true, nameof(Phase10PlayerItem.TotalScore));
            _score.AddColumn("Cards Left", true, nameof(Phase10PlayerItem.ObjectCount));
            _score.AddColumn("Phase", true, nameof(Phase10PlayerItem.Phase));
            _score.AddColumn("Skipped", true, nameof(Phase10PlayerItem.MissNextTurn), useTrueFalse: true);
            _score.AddColumn("Completed", true, nameof(Phase10PlayerItem.Completed), useTrueFalse: true);
            AddControlToGrid(firstGrid, _score, 0, 4);
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(Phase10MainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(Phase10MainViewModel.Status));
            firstInfo.AddRow("Phase", nameof(Phase10MainViewModel.CurrentPhase));
            AddControlToGrid(firstGrid, firstInfo.GetContent, 0, 3);
            AddControlToGrid(finalGrid, firstGrid, 0, 0); // i think
            _tempG.Height = 700;
            _tempG.Divider = 1.3;
            StackPanel thirdStack = new StackPanel();
            thirdStack.Orientation = Orientation.Horizontal;
            _mainG.Height = 700; // try this way.
            thirdStack.Children.Add(_tempG);
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
