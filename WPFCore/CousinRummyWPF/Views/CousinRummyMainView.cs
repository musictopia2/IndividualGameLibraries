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
using CousinRummyCP.Data;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicControlsAndWindowsCore.Controls.NavigationControls;
using CousinRummyCP.ViewModels;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //usually needs this as well for grid helpers.
using BasicGamingUIWPFLibrary.Helpers;
using System.Windows;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;
using BasicGamingUIWPFLibrary.BasicControls.SingleCardFrames;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGamingUIWPFLibrary.GameGraphics.Cards;
using BasicGamingUIWPFLibrary.BasicControls.GameFrames;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using BasicGamingUIWPFLibrary.BasicControls.Misc;
using CousinRummyCP.Logic;

namespace CousinRummyWPF.Views
{
    public class CousinRummyMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        //TODO:  has to build the main page.  no need for new game since that is completely separated out into another class.
        private readonly IEventAggregator _aggregator;
        private readonly CousinRummyVMData _model;
        private readonly BaseDeckWPF<RegularRummyCard, ts, DeckOfCardsWPF<RegularRummyCard>> _deckGPile;
        private readonly BasePileWPF<RegularRummyCard, ts, DeckOfCardsWPF<RegularRummyCard>> _discardGPile;

        private readonly ScoreBoardWPF _score;
        private readonly BaseHandWPF<RegularRummyCard, ts, DeckOfCardsWPF<RegularRummyCard>> _playerHandWPF;
        private readonly TempRummySetsWPF<EnumSuitList, EnumColorList, RegularRummyCard, ts, DeckOfCardsWPF<RegularRummyCard>> _tempG;
        private readonly MainRummySetsWPF<EnumSuitList, EnumColorList, RegularRummyCard, ts, DeckOfCardsWPF<RegularRummyCard>, PhaseSet, SavedSet> _mainG;
        public CousinRummyMainView(IEventAggregator aggregator,
            TestOptions test,
            CousinRummyVMData model
            )
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);

            _deckGPile = new BaseDeckWPF<RegularRummyCard, ts, DeckOfCardsWPF<RegularRummyCard>>();
            _discardGPile = new BasePileWPF<RegularRummyCard, ts, DeckOfCardsWPF<RegularRummyCard>>();
            _score = new ScoreBoardWPF();
            _playerHandWPF = new BaseHandWPF<RegularRummyCard, ts, DeckOfCardsWPF<RegularRummyCard>>();

            _tempG = new TempRummySetsWPF<EnumSuitList, EnumColorList, RegularRummyCard, ts, DeckOfCardsWPF<RegularRummyCard>>();
            _mainG = new MainRummySetsWPF<EnumSuitList, EnumColorList, RegularRummyCard, ts, DeckOfCardsWPF<RegularRummyCard>, PhaseSet, SavedSet>();


            StackPanel mainStack = new StackPanel();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer()
                {
                    Name = nameof(CousinRummyMainViewModel.RestoreScreen)
                };
            }

            Grid buyGrid = new Grid();
            AddAutoColumns(buyGrid, 2);
            AddAutoRows(buyGrid, 2);
            Button button;
            button = GetGamingButton("Pass", nameof(CousinRummyMainViewModel.PassAsync));
            AddControlToGrid(buyGrid, button, 0, 0);
            button = GetGamingButton("Buy", nameof(CousinRummyMainViewModel.BuyAsync));
            AddControlToGrid(buyGrid, button, 0, 1);
            Grid gameGrid = new Grid();
            AddLeftOverColumn(gameGrid, 1); // try that
            AddAutoColumns(gameGrid, 1);
            AddAutoRows(gameGrid, 1);
            AddPixelRow(gameGrid, 450);
            _deckGPile.HorizontalAlignment = HorizontalAlignment.Left;
            _deckGPile.VerticalAlignment = VerticalAlignment.Top;
            AddControlToGrid(buyGrid, _deckGPile, 1, 0);
            _discardGPile.HorizontalAlignment = HorizontalAlignment.Left;
            _discardGPile.VerticalAlignment = VerticalAlignment.Top;
            AddControlToGrid(buyGrid, _discardGPile, 1, 1);
            StackPanel otherStack = new StackPanel();
            otherStack.Children.Add(_playerHandWPF);
            button = GetGamingButton("Lay Down Initial Sets", nameof(CousinRummyMainViewModel.FirstSetsAsync));
            otherStack.Children.Add(button);
            button = GetGamingButton("Lay Down Other Sets", nameof(CousinRummyMainViewModel.OtherSetsAsync)); // i think its othersets commands (?)
            otherStack.Children.Add(button);
            AddControlToGrid(gameGrid, otherStack, 0, 0);
            _tempG.Height = 400;
            AddControlToGrid(gameGrid, _tempG, 0, 1);
            AddControlToGrid(gameGrid, _mainG, 1, 0);
            Grid.SetColumnSpan(_mainG, 2);
            _score.AddColumn("Cards Left", false, nameof(CousinRummyPlayerItem.ObjectCount)); //very common.
            _score.AddColumn("Tokens Left", false, nameof(CousinRummyPlayerItem.TokensLeft));
            _score.AddColumn("Current Score", false, nameof(CousinRummyPlayerItem.CurrentScore), rightMargin: 5);
            _score.AddColumn("Total Score", false, nameof(CousinRummyPlayerItem.TotalScore));
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Normal Turn", nameof(CousinRummyMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(CousinRummyMainViewModel.Status));
            firstInfo.AddRow("Other Turn", nameof(CousinRummyMainViewModel.OtherLabel));
            firstInfo.AddRow("Phase", nameof(CousinRummyMainViewModel.PhaseData));
            var tempStack = new StackPanel();
            tempStack.Orientation = Orientation.Horizontal;
            tempStack.Children.Add(_score);
            tempStack.Children.Add(buyGrid);
            tempStack.Children.Add(firstInfo.GetContent);
            otherStack.Children.Add(tempStack);
            mainStack.Children.Add(gameGrid);



            _deckGPile.Margin = new Thickness(5, 5, 5, 5);

            _discardGPile.Margin = new Thickness(5, 5, 5, 5);


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

            CousinRummySaveInfo save = cons!.Resolve<CousinRummySaveInfo>(); //usually needs this part for multiplayer games.

            _score!.LoadLists(save.PlayerList);
            _playerHandWPF!.LoadList(_model.PlayerHand1!, ts.TagUsed); // i think
            _discardGPile!.Init(_model.Pile1!, ts.TagUsed); // may have to be here (well see)
            _discardGPile.StartListeningDiscardPile(); // its the main one.

            _deckGPile!.Init(_model.Deck1!, ts.TagUsed); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();
            _tempG!.Init(_model!.TempSets!, ts.TagUsed);
            _mainG!.Init(_model!.MainSets!, ts.TagUsed);
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
