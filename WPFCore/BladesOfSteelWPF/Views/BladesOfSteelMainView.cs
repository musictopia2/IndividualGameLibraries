using BasicControlsAndWindowsCore.Controls.NavigationControls;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIWPFLibrary.BasicControls.GameFrames;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using BasicGamingUIWPFLibrary.BasicControls.SingleCardFrames;
using BasicGamingUIWPFLibrary.GameGraphics.Cards;
using BladesOfSteelCP.Data;
using BladesOfSteelCP.ViewModels;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows;
using System.Windows.Controls;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions;
using static BasicControlsAndWindowsCore.Helpers.GridHelper;
namespace BladesOfSteelWPF.Views
{
    public class BladesOfSteelMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly BladesOfSteelVMData _model;
        private readonly BaseDeckWPF<RegularSimpleCard, ts, DeckOfCardsWPF<RegularSimpleCard>> _deckGPile;
        private readonly BasePileWPF<RegularSimpleCard, ts, DeckOfCardsWPF<RegularSimpleCard>> _discardGPile;

        private readonly ScoreBoardWPF _score;
        private readonly BaseHandWPF<RegularSimpleCard, ts, DeckOfCardsWPF<RegularSimpleCard>> _playerHandWPF;

        private readonly BaseHandWPF<RegularSimpleCard, ts, DeckOfCardsWPF<RegularSimpleCard>> _mainDefenseCards;
        private readonly BaseHandWPF<RegularSimpleCard, ts, DeckOfCardsWPF<RegularSimpleCard>> _opponentDefense;
        private readonly BaseHandWPF<RegularSimpleCard, ts, DeckOfCardsWPF<RegularSimpleCard>> _opponentAttack;
        private readonly BaseHandWPF<RegularSimpleCard, ts, DeckOfCardsWPF<RegularSimpleCard>> _yourDefense;
        private readonly BaseHandWPF<RegularSimpleCard, ts, DeckOfCardsWPF<RegularSimpleCard>> _yourAttack;



        public BladesOfSteelMainView(IEventAggregator aggregator,
            TestOptions test,
            BladesOfSteelVMData model,
            BladesOfSteelGameContainer gameContainer
            )
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);

            _deckGPile = new BaseDeckWPF<RegularSimpleCard, ts, DeckOfCardsWPF<RegularSimpleCard>>();
            _discardGPile = new BasePileWPF<RegularSimpleCard, ts, DeckOfCardsWPF<RegularSimpleCard>>();
            _score = new ScoreBoardWPF();
            _playerHandWPF = new BaseHandWPF<RegularSimpleCard, ts, DeckOfCardsWPF<RegularSimpleCard>>();

            _mainDefenseCards = new BaseHandWPF<RegularSimpleCard, ts, DeckOfCardsWPF<RegularSimpleCard>>();
            _opponentDefense = new BaseHandWPF<RegularSimpleCard, ts, DeckOfCardsWPF<RegularSimpleCard>>();
            _opponentAttack = new BaseHandWPF<RegularSimpleCard, ts, DeckOfCardsWPF<RegularSimpleCard>>();
            _yourDefense = new BaseHandWPF<RegularSimpleCard, ts, DeckOfCardsWPF<RegularSimpleCard>>();
            _yourAttack = new BaseHandWPF<RegularSimpleCard, ts, DeckOfCardsWPF<RegularSimpleCard>>();
            ScoringGuideWPF tempScore = new ScoringGuideWPF();
            _score.AddColumn("Cards Left", true, nameof(BladesOfSteelPlayerItem.ObjectCount), rightMargin: 5);
            _score.AddColumn("Score", true, nameof(BladesOfSteelPlayerItem.Score), rightMargin: 5);



            StackPanel mainStack = new StackPanel();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer()
                {
                    Name = nameof(BladesOfSteelMainViewModel.RestoreScreen)
                };
            }


            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;

            otherStack.Children.Add(tempScore);
            otherStack.Children.Add(_score);
            mainStack.Children.Add(otherStack);

            otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            mainStack.Children.Add(otherStack);

            StackPanel firstStack = new StackPanel();
            AddVerticalLabelGroup("Instructions", nameof(BladesOfSteelMainViewModel.Instructions), firstStack);
            otherStack.Children.Add(firstStack);
            Grid playerArea = new Grid();
            AddAutoColumns(playerArea, 3);
            AddAutoRows(playerArea, 2);
            _opponentDefense.Margin = new Thickness(0, 0, 0, 20);
            AddControlToGrid(playerArea, _opponentDefense, 0, 2);
            AddControlToGrid(playerArea, _opponentAttack, 0, 1);
            _opponentAttack.Margin = new Thickness(0, 0, 0, 20);
            AddControlToGrid(playerArea, _mainDefenseCards, 1, 0);
            AddControlToGrid(playerArea, _yourAttack, 1, 1);
            AddControlToGrid(playerArea, _yourDefense, 1, 2);
            mainStack.Children.Add(playerArea);
            otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile);
            AddControlToGrid(playerArea, otherStack, 0, 0);
            otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            mainStack.Children.Add(otherStack);
            var endButton = GetGamingButton("End Turn", nameof(BladesOfSteelMainViewModel.EndTurnAsync));
            endButton.HorizontalAlignment = HorizontalAlignment.Left;
            endButton.VerticalAlignment = VerticalAlignment.Center;
            otherStack.Children.Add(endButton);
            var otherBut = GetGamingButton("Pass", nameof(BladesOfSteelMainViewModel.PassAsync));
            otherStack.Children.Add(otherBut);
            otherBut.HorizontalAlignment = HorizontalAlignment.Left;
            otherBut.VerticalAlignment = VerticalAlignment.Center;
            otherStack.Children.Add(_playerHandWPF);
            _deckGPile.Margin = new Thickness(5, 5, 5, 5);
            _discardGPile.Margin = new Thickness(5, 5, 5, 5);
            _deckGPile.HorizontalAlignment = HorizontalAlignment.Left;
            _deckGPile.VerticalAlignment = VerticalAlignment.Top;
            _discardGPile.HorizontalAlignment = HorizontalAlignment.Left;
            _discardGPile.VerticalAlignment = VerticalAlignment.Top;

            if (restoreP != null)
            {
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }
            _score!.LoadLists(gameContainer.PlayerList!);
            _playerHandWPF!.LoadList(_model.PlayerHand1!, ts.TagUsed); // i think
            _discardGPile!.Init(_model.Pile1!, ts.TagUsed); // may have to be here (well see)
            _discardGPile.StartListeningDiscardPile(); // its the main one.

            _deckGPile!.Init(_model.Deck1!, ts.TagUsed); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();


            _mainDefenseCards!.LoadList(_model.MainDefense1!, ts.TagUsed);
            _yourAttack!.LoadList(_model.YourAttackPile!, ts.TagUsed);
            _yourDefense!.LoadList(_model.YourDefensePile!, ts.TagUsed);
            _opponentAttack!.LoadList(_model.OpponentAttackPile!, ts.TagUsed);
            _opponentDefense!.LoadList(_model.OpponentDefensePile!, ts.TagUsed);

            Content = mainStack;

        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {

            return Task.CompletedTask;
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
