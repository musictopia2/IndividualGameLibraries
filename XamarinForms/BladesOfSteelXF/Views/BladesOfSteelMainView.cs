using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIXFLibrary.BasicControls.GameFrames;
using BasicGamingUIXFLibrary.BasicControls.SingleCardFrames;
using BasicGamingUIXFLibrary.GameGraphics.Cards;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using BladesOfSteelCP.Data;
using BladesOfSteelCP.ViewModels;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using static BasicXFControlsAndPages.Helpers.GridHelper;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;

namespace BladesOfSteelXF.Views
{
    public class BladesOfSteelMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly BladesOfSteelVMData _model;
        private readonly BaseDeckXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>> _deckGPile;
        private readonly BasePileXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>> _discardGPile;

        private readonly ScoreBoardXF _score;
        private readonly BaseHandXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>> _playerHandWPF;

        private readonly BaseHandXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>> _mainDefenseCards;
        private readonly BaseHandXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>> _opponentDefense;
        private readonly BaseHandXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>> _opponentAttack;
        private readonly BaseHandXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>> _yourDefense;
        private readonly BaseHandXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>> _yourAttack;
        public BladesOfSteelMainView(IEventAggregator aggregator,
            TestOptions test,
            BladesOfSteelVMData model,
            BladesOfSteelGameContainer gameContainer
            )
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);

            _deckGPile = new BaseDeckXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>>();
            _discardGPile = new BasePileXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>>();
            _score = new ScoreBoardXF();
            _playerHandWPF = new BaseHandXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>>();

            _mainDefenseCards = new BaseHandXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>>();
            _opponentDefense = new BaseHandXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>>();
            _opponentAttack = new BaseHandXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>>();
            _yourDefense = new BaseHandXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>>();
            _yourAttack = new BaseHandXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>>();
            ScoringGuideXF tempScore = new ScoringGuideXF();
            _score.AddColumn("Cards Left", true, nameof(BladesOfSteelPlayerItem.ObjectCount), rightMargin: 5);
            _score.AddColumn("Score", true, nameof(BladesOfSteelPlayerItem.Score), rightMargin: 5);

            StackLayout mainStack = new StackLayout();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer(nameof(BladesOfSteelMainViewModel.RestoreScreen));
            }



            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;

            otherStack.Children.Add(tempScore);
            otherStack.Children.Add(_score);
            mainStack.Children.Add(otherStack);

            otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            mainStack.Children.Add(otherStack);

            StackLayout firstStack = new StackLayout();
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
            otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile);
            AddControlToGrid(playerArea, otherStack, 0, 0);
            otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            mainStack.Children.Add(otherStack);
            var endButton = GetGamingButton("End Turn", nameof(BladesOfSteelMainViewModel.EndTurnAsync));
            endButton.HorizontalOptions = LayoutOptions.Start;
            endButton.VerticalOptions = LayoutOptions.Center;
            otherStack.Children.Add(endButton);
            var otherBut = GetGamingButton("Pass", nameof(BladesOfSteelMainViewModel.PassAsync));
            otherStack.Children.Add(otherBut);
            otherBut.HorizontalOptions = LayoutOptions.Start;
            otherBut.VerticalOptions = LayoutOptions.Center;
            otherStack.Children.Add(_playerHandWPF);
            _deckGPile.HorizontalOptions = LayoutOptions.Start;
            _deckGPile.VerticalOptions = LayoutOptions.Start;
            _discardGPile.HorizontalOptions = LayoutOptions.Start;
            _discardGPile.VerticalOptions = LayoutOptions.Start;

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

            return this.RefreshBindingsAsync(_aggregator); //this may be needed so it can hook up the buttons just in case not there first time.
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
            return Task.CompletedTask;
        }
    }
}
