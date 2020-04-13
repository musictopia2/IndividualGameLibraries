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
using Rummy500CP.Data;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicControlsAndWindowsCore.Controls.NavigationControls;
using Rummy500CP.ViewModels;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //usually needs this as well for grid helpers.
using BasicGamingUIWPFLibrary.Helpers;
using System.Windows;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;
using BasicGamingUIWPFLibrary.BasicControls.SingleCardFrames;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGamingUIWPFLibrary.GameGraphics.Cards;
using BasicGamingUIWPFLibrary.BasicControls.GameFrames;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using Rummy500CP.Logic;
using BasicGameFrameworkLibrary.DrawableListsObservable;

namespace Rummy500WPF.Views
{
    public class Rummy500MainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly Rummy500VMData _model;
        private readonly BaseDeckWPF<RegularRummyCard, ts, DeckOfCardsWPF<RegularRummyCard>> _deckGPile;

        private readonly ScoreBoardWPF _score;
        private readonly BaseHandWPF<RegularRummyCard, ts, DeckOfCardsWPF<RegularRummyCard>> _playerHandWPF;

        private readonly BaseHandWPF<RegularRummyCard, ts, DeckOfCardsWPF<RegularRummyCard>> _discardRummy;
        private readonly MainRummySetsWPF<EnumSuitList, EnumColorList, RegularRummyCard, ts, DeckOfCardsWPF<RegularRummyCard>, RummySet, SavedSet> _mainG;


        public Rummy500MainView(IEventAggregator aggregator,
            TestOptions test,
            Rummy500VMData model
            )
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);

            _deckGPile = new BaseDeckWPF<RegularRummyCard, ts, DeckOfCardsWPF<RegularRummyCard>>();
            _score = new ScoreBoardWPF();
            _playerHandWPF = new BaseHandWPF<RegularRummyCard, ts, DeckOfCardsWPF<RegularRummyCard>>();
            _playerHandWPF.Margin = new Thickness(5, 5, 5, 5);
            _playerHandWPF.HorizontalAlignment = HorizontalAlignment.Stretch;
            _discardRummy = new BaseHandWPF<RegularRummyCard, ts, DeckOfCardsWPF<RegularRummyCard>>();
            _mainG = new MainRummySetsWPF<EnumSuitList, EnumColorList, RegularRummyCard, ts, DeckOfCardsWPF<RegularRummyCard>, RummySet, SavedSet>();


            StackPanel mainStack = new StackPanel();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer()
                {
                    Name = nameof(Rummy500MainViewModel.RestoreScreen)
                };
            }
            Grid finalGrid = new Grid();
            AddAutoColumns(finalGrid, 1);
            AddLeftOverColumn(finalGrid, 1);
            AddControlToGrid(finalGrid, mainStack, 0, 1);
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(Rummy500MainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(Rummy500MainViewModel.Status));

            _score.AddColumn("Cards Left", false, nameof(Rummy500PlayerItem.ObjectCount)); //very common.
            _score.AddColumn("Points Played", false, nameof(Rummy500PlayerItem.PointsPlayed));
            _score.AddColumn("Cards Played", false, nameof(Rummy500PlayerItem.CardsPlayed));
            _score.AddColumn("Score Current", false, nameof(Rummy500PlayerItem.CurrentScore));
            _score.AddColumn("Score Total", false, nameof(Rummy500PlayerItem.TotalScore));
            otherStack.Children.Add(_score);
            otherStack.Children.Add(firstInfo.GetContent);
            mainStack.Children.Add(_playerHandWPF);
            Button button;
            button = GetGamingButton("Discard Current", nameof(Rummy500MainViewModel.DiscardCurrentAsync));
            otherStack.Children.Add(button);
            button = GetGamingButton("Create New Rummy Set", nameof(Rummy500MainViewModel.CreateSetAsync));
            otherStack.Children.Add(button);
            mainStack.Children.Add(otherStack);
            _mainG.Divider = 1.3;
            _mainG.Height = 550; // i think
            mainStack.Children.Add(_mainG);
            _discardRummy.Divider = 1.7;
            _discardRummy.HandType = HandObservable<RegularRummyCard>.EnumHandList.Vertical;
            _discardRummy.HorizontalAlignment = HorizontalAlignment.Left;
            _discardRummy.VerticalAlignment = VerticalAlignment.Stretch;
            AddControlToGrid(finalGrid, _discardRummy, 0, 0);
            _deckGPile.Margin = new Thickness(5, 5, 5, 5);



            if (restoreP != null)
            {
                //todo:  figure out where to place the restore ui if there is a restore option.
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }
            Content = finalGrid;

        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {

            GamePackageViewModelBinder.ManuelElements.Clear(); //often times i have to add manually.

            Rummy500SaveInfo save = cons!.Resolve<Rummy500SaveInfo>(); //usually needs this part for multiplayer games.

            _score!.LoadLists(save.PlayerList);
            _playerHandWPF!.LoadList(_model.PlayerHand1!, ts.TagUsed); // i think

            _deckGPile!.Init(_model.Deck1!, ts.TagUsed); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();

            _discardRummy!.LoadList(_model.DiscardList1!, ts.TagUsed);
            _mainG!.Init(_model.MainSets1!, ts.TagUsed);

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
