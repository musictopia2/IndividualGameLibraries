using BasicControlsAndWindowsCore.Controls.NavigationControls;
using BasicGameFrameworkLibrary.BasicEventModels;
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
using RoundsCardGameCP.Cards;
using RoundsCardGameCP.Data;
using RoundsCardGameCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows;
using System.Windows.Controls;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;

namespace RoundsCardGameWPF.Views
{
    public class RoundsCardGameMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly RoundsCardGameVMData _model;
        private readonly BaseDeckWPF<RoundsCardGameCardInformation, ts, DeckOfCardsWPF<RoundsCardGameCardInformation>> _deckGPile;
        private readonly BasePileWPF<RoundsCardGameCardInformation, ts, DeckOfCardsWPF<RoundsCardGameCardInformation>> _discardGPile;

        private readonly ScoreBoardWPF _score;
        private readonly BaseHandWPF<RoundsCardGameCardInformation, ts, DeckOfCardsWPF<RoundsCardGameCardInformation>> _playerHandWPF;

        private readonly TwoPlayerTrickWPF<EnumSuitList, RoundsCardGameCardInformation, ts, DeckOfCardsWPF<RoundsCardGameCardInformation>> _trick1;


        public RoundsCardGameMainView(IEventAggregator aggregator,
            TestOptions test,
            RoundsCardGameVMData model
            )
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);

            _deckGPile = new BaseDeckWPF<RoundsCardGameCardInformation, ts, DeckOfCardsWPF<RoundsCardGameCardInformation>>();
            _discardGPile = new BasePileWPF<RoundsCardGameCardInformation, ts, DeckOfCardsWPF<RoundsCardGameCardInformation>>();
            _score = new ScoreBoardWPF();
            _playerHandWPF = new BaseHandWPF<RoundsCardGameCardInformation, ts, DeckOfCardsWPF<RoundsCardGameCardInformation>>();

            _trick1 = new TwoPlayerTrickWPF<EnumSuitList, RoundsCardGameCardInformation, ts, DeckOfCardsWPF<RoundsCardGameCardInformation>>();


            StackPanel mainStack = new StackPanel();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer()
                {
                    Name = nameof(RoundsCardGameMainViewModel.RestoreScreen)
                };
            }


            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            mainStack.Children.Add(otherStack);
            _score.AddColumn("# In Hand", true, nameof(RoundsCardGamePlayerItem.ObjectCount));
            _score.AddColumn("Tricks Won", true, nameof(RoundsCardGamePlayerItem.TricksWon));
            _score.AddColumn("Rounds Won", true, nameof(RoundsCardGamePlayerItem.RoundsWon));
            _score.AddColumn("Points", true, nameof(RoundsCardGamePlayerItem.CurrentPoints));
            _score.AddColumn("Total Score", true, nameof(RoundsCardGamePlayerItem.TotalScore));
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(RoundsCardGameMainViewModel.NormalTurn));
            firstInfo.AddRow("Trump", nameof(RoundsCardGameMainViewModel.TrumpSuit));
            firstInfo.AddRow("Status", nameof(RoundsCardGameMainViewModel.Status));
            mainStack.Children.Add(_trick1);
            mainStack.Children.Add(_playerHandWPF);
            mainStack.Children.Add(firstInfo.GetContent);

            mainStack.Children.Add(_score);


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

            RoundsCardGameSaveInfo save = cons!.Resolve<RoundsCardGameSaveInfo>(); //usually needs this part for multiplayer games.

            _score!.LoadLists(save.PlayerList);
            _playerHandWPF!.LoadList(_model.PlayerHand1!, ts.TagUsed); // i think
            _discardGPile!.Init(_model.Pile1!, ts.TagUsed); // may have to be here (well see)
            _discardGPile.StartListeningDiscardPile(); // its the main one.

            _deckGPile!.Init(_model.Deck1!, ts.TagUsed); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();
            _trick1!.Init(_model.TrickArea1!, ts.TagUsed);
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
