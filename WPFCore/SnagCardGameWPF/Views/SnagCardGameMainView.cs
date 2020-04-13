using BasicControlsAndWindowsCore.Controls.NavigationControls;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIWPFLibrary.BasicControls.GameFrames;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using BasicGamingUIWPFLibrary.BasicControls.SingleCardFrames;
using BasicGamingUIWPFLibrary.BasicControls.TrickUIs;
using BasicGamingUIWPFLibrary.GameGraphics.Cards;
using BasicGamingUIWPFLibrary.Helpers;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using SnagCardGameCP.Cards;
using SnagCardGameCP.Data;
using SnagCardGameCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows;
using System.Windows.Controls;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //usually needs this as well for grid helpers.
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;

namespace SnagCardGameWPF.Views
{
    public class SnagCardGameMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly SnagCardGameVMData _model;
        private readonly BaseDeckWPF<SnagCardGameCardInformation, ts, DeckOfCardsWPF<SnagCardGameCardInformation>> _deckGPile;
        private readonly BasePileWPF<SnagCardGameCardInformation, ts, DeckOfCardsWPF<SnagCardGameCardInformation>> _discardGPile;

        private readonly ScoreBoardWPF _score;
        private readonly BaseHandWPF<SnagCardGameCardInformation, ts, DeckOfCardsWPF<SnagCardGameCardInformation>> _playerHandWPF;

        private readonly SeveralPlayersTrickWPF<EnumSuitList, SnagCardGameCardInformation, ts, DeckOfCardsWPF<SnagCardGameCardInformation>, SnagCardGamePlayerItem> _trick1;

        private readonly BaseHandWPF<SnagCardGameCardInformation, ts, DeckOfCardsWPF<SnagCardGameCardInformation>> _bar1;
        private readonly BaseHandWPF<SnagCardGameCardInformation, ts, DeckOfCardsWPF<SnagCardGameCardInformation>> _human1;
        private readonly BaseHandWPF<SnagCardGameCardInformation, ts, DeckOfCardsWPF<SnagCardGameCardInformation>> _opponent1;


        public SnagCardGameMainView(IEventAggregator aggregator,
            TestOptions test,
            SnagCardGameVMData model
            )
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);

            _deckGPile = new BaseDeckWPF<SnagCardGameCardInformation, ts, DeckOfCardsWPF<SnagCardGameCardInformation>>();
            _discardGPile = new BasePileWPF<SnagCardGameCardInformation, ts, DeckOfCardsWPF<SnagCardGameCardInformation>>();
            _score = new ScoreBoardWPF();
            _playerHandWPF = new BaseHandWPF<SnagCardGameCardInformation, ts, DeckOfCardsWPF<SnagCardGameCardInformation>>();

            _trick1 = new SeveralPlayersTrickWPF<EnumSuitList, SnagCardGameCardInformation, ts, DeckOfCardsWPF<SnagCardGameCardInformation>, SnagCardGamePlayerItem>();

            _bar1 = new BaseHandWPF<SnagCardGameCardInformation, ts, DeckOfCardsWPF<SnagCardGameCardInformation>>();
            _human1 = new BaseHandWPF<SnagCardGameCardInformation, ts, DeckOfCardsWPF<SnagCardGameCardInformation>>();
            _opponent1 = new BaseHandWPF<SnagCardGameCardInformation, ts, DeckOfCardsWPF<SnagCardGameCardInformation>>();
            _bar1.HandType = HandObservable<SnagCardGameCardInformation>.EnumHandList.Vertical;
            _bar1.ExtraControlSpace = 20;


            StackPanel mainStack = new StackPanel();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer()
                {
                    Name = nameof(SnagCardGameMainViewModel.RestoreScreen)
                };
            }


            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;


            StackPanel tempStack = new StackPanel();
            otherStack.Children.Add(_trick1);
            tempStack.Children.Add(_human1);
            tempStack.Children.Add(_opponent1);
            otherStack.Children.Add(tempStack);
            mainStack.Children.Add(otherStack);
            _score.AddColumn("Cards Won", true, nameof(SnagCardGamePlayerItem.CardsWon));
            _score.AddColumn("Current Points", true, nameof(SnagCardGamePlayerItem.CurrentPoints));
            _score.AddColumn("Total Points", true, nameof(SnagCardGamePlayerItem.TotalPoints));
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(SnagCardGameMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(SnagCardGameMainViewModel.Status));
            firstInfo.AddRow("Instructions", nameof(SnagCardGameMainViewModel.Instructions));
            mainStack.Children.Add(_playerHandWPF);
            mainStack.Children.Add(firstInfo.GetContent);
            mainStack.Children.Add(_score);
            Grid otherGrid = new Grid();
            AddLeftOverColumn(otherGrid, 20);
            AddLeftOverColumn(otherGrid, 80); // can always be adjusted
            AddControlToGrid(otherGrid, _bar1, 0, 0);
            AddControlToGrid(otherGrid, mainStack, 0, 1);


            _deckGPile.Margin = new Thickness(5, 5, 5, 5);

            _discardGPile.Margin = new Thickness(5, 5, 5, 5);


            if (restoreP != null)
            {
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }
            Content = otherGrid;

        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {

            GamePackageViewModelBinder.ManuelElements.Clear(); //often times i have to add manually.

            SnagCardGameSaveInfo save = cons!.Resolve<SnagCardGameSaveInfo>(); //usually needs this part for multiplayer games.

            _score!.LoadLists(save.PlayerList);
            _playerHandWPF!.LoadList(_model.PlayerHand1!, ts.TagUsed); // i think
            _discardGPile!.Init(_model.Pile1!, ts.TagUsed); // may have to be here (well see)
            _discardGPile.StartListeningDiscardPile(); // its the main one.

            _deckGPile!.Init(_model.Deck1!, ts.TagUsed); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();
            _trick1!.Init(_model.TrickArea1!, _model.TrickArea1, ts.TagUsed);

            _bar1!.LoadList(_model.Bar1!, ts.TagUsed);
            _human1!.LoadList(_model.Human1!, ts.TagUsed);
            _opponent1!.LoadList(_model.Opponent1!, ts.TagUsed);

            return this.RefreshBindingsAsync(_aggregator);
        }



        Task IUIView.TryActivateAsync()
        {
            return Task.CompletedTask;
        }

        Task IUIView.TryCloseAsync()
        {
            _trick1.Dispose();
            return Task.CompletedTask;
        }
    }
}
