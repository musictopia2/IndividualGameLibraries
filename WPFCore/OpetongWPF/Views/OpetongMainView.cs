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
using OpetongCP.Data;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicControlsAndWindowsCore.Controls.NavigationControls;
using OpetongCP.ViewModels;
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
using OpetongCP.Logic;

namespace OpetongWPF.Views
{
    public class OpetongMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly OpetongVMData _model;
        private readonly BaseDeckWPF<RegularRummyCard, ts, DeckOfCardsWPF<RegularRummyCard>> _deckGPile;

        private readonly ScoreBoardWPF _score;
        private readonly BaseHandWPF<RegularRummyCard, ts, DeckOfCardsWPF<RegularRummyCard>> _playerHandWPF;

        private readonly TempRummySetsWPF<EnumSuitList, EnumColorList, RegularRummyCard, ts, DeckOfCardsWPF<RegularRummyCard>> _tempG;
        private readonly MainRummySetsWPF<EnumSuitList, EnumColorList, RegularRummyCard, ts, DeckOfCardsWPF<RegularRummyCard>, RummySet, SavedSet> _mainG;
        private readonly CardBoardWPF<RegularRummyCard, ts, DeckOfCardsWPF<RegularRummyCard>> _poolG;

        public OpetongMainView(IEventAggregator aggregator,
            TestOptions test,
            OpetongVMData model
            )
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);

            _deckGPile = new BaseDeckWPF<RegularRummyCard, ts, DeckOfCardsWPF<RegularRummyCard>>();
            _score = new ScoreBoardWPF();
            _playerHandWPF = new BaseHandWPF<RegularRummyCard, ts, DeckOfCardsWPF<RegularRummyCard>>();

            _tempG = new TempRummySetsWPF<EnumSuitList, EnumColorList, RegularRummyCard, ts, DeckOfCardsWPF<RegularRummyCard>>();
            _mainG = new MainRummySetsWPF<EnumSuitList, EnumColorList, RegularRummyCard, ts, DeckOfCardsWPF<RegularRummyCard>, RummySet, SavedSet>();
            _poolG = new CardBoardWPF<RegularRummyCard, ts, DeckOfCardsWPF<RegularRummyCard>>();


            StackPanel mainStack = new StackPanel();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer()
                {
                    Name = nameof(OpetongMainViewModel.RestoreScreen)
                };
            }


            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_poolG);
            _poolG.HorizontalAlignment = HorizontalAlignment.Left;
            _poolG.VerticalAlignment = VerticalAlignment.Top;
            _tempG.Height = 350;
            otherStack.Children.Add(_tempG);
            var button = GetGamingButton("Lay Down Single Set", nameof(OpetongMainViewModel.PlaySetAsync));
            otherStack.Children.Add(button);
            mainStack.Children.Add(otherStack);
            _score.AddColumn("Cards Left", true, nameof(OpetongPlayerItem.ObjectCount)); //very common.
            _score.AddColumn("Sets Played", true, nameof(OpetongPlayerItem.SetsPlayed));
            _score.AddColumn("Score", true, nameof(OpetongPlayerItem.TotalScore));
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(OpetongMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(OpetongMainViewModel.Status));
            firstInfo.AddRow("Instructions", nameof(OpetongMainViewModel.Instructions));

            StackPanel finalStack = new StackPanel();
            finalStack.Children.Add(_score);
            finalStack.Children.Add(firstInfo.GetContent);
            otherStack.Children.Add(finalStack);
            mainStack.Children.Add(_playerHandWPF);
            mainStack.Children.Add(_mainG);


            _deckGPile.Margin = new Thickness(5, 5, 5, 5);



            if (restoreP != null)
            {
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }
            Content = mainStack;

        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {

            GamePackageViewModelBinder.ManuelElements.Clear(); //often times i have to add manually.

            OpetongSaveInfo save = cons!.Resolve<OpetongSaveInfo>(); //usually needs this part for multiplayer games.

            _score!.LoadLists(save.PlayerList);
            _playerHandWPF!.LoadList(_model.PlayerHand1!, ts.TagUsed); // i think

            _deckGPile!.Init(_model.Deck1!, ts.TagUsed); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();
            _tempG!.Init(_model.TempSets!, ts.TagUsed);
            _mainG!.Init(_model.MainSets!, ts.TagUsed);
            _poolG!.LoadList(_model.Pool1!, ts.TagUsed);
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
