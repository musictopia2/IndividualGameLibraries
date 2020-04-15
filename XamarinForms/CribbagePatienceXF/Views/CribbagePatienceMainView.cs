using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGamingUIXFLibrary.BasicControls.GameFrames;
using BasicGamingUIXFLibrary.BasicControls.SingleCardFrames;
using BasicGamingUIXFLibrary.GameGraphics.Cards;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using CribbagePatienceCP.Data;
using CribbagePatienceCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using static BasicXFControlsAndPages.Helpers.GridHelper; //since i use the grid a lot too.
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;

namespace CribbagePatienceXF.Views
{
    public class CribbagePatienceMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;

        private readonly BaseDeckXF<CribbageCard, ts, DeckOfCardsXF<CribbageCard>> _deckGPile;

        private readonly ScoreHandCribUI _hand1Score;
        private readonly ScoreHandCribUI _hand2Score;
        private readonly ScoreHandCribUI _cribScore;
        private readonly ScoreSummaryUI _score1;
        private readonly BaseHandXF<CribbageCard, ts, DeckOfCardsXF<CribbageCard>> _yourHand;
        private readonly BaseHandXF<CribbageCard, ts, DeckOfCardsXF<CribbageCard>> _cribHand;
        private readonly BasePileXF<CribbageCard, ts, DeckOfCardsXF<CribbageCard>> _startCard;
        private readonly Grid _grid;


        private void SetMargins(View element)
        {
            element.Margin = new Thickness(5);
        }

        public CribbagePatienceMainView(IEventAggregator aggregator)
        {
            _aggregator = aggregator;
            _aggregator.Subscribe(this);
            _deckGPile = new BaseDeckXF<CribbageCard, ts, DeckOfCardsXF<CribbageCard>>();
            StackLayout stack = new StackLayout();
            stack.Children.Add(_deckGPile);

            _deckGPile.Margin = new Thickness(5, 5, 5, 5);
            _deckGPile.HorizontalOptions = LayoutOptions.Start;
            _deckGPile.VerticalOptions = LayoutOptions.Start;

            _hand1Score = new ScoreHandCribUI();
            _hand2Score = new ScoreHandCribUI();
            _cribScore = new ScoreHandCribUI();
            _score1 = new ScoreSummaryUI();
            _yourHand = new BaseHandXF<CribbageCard, ts, DeckOfCardsXF<CribbageCard>>();
            _cribHand = new BaseHandXF<CribbageCard, ts, DeckOfCardsXF<CribbageCard>>();
            SetMargins(_score1);
            SetMargins(_hand1Score);
            SetMargins(_hand2Score);
            SetMargins(_cribScore);
            SetMargins(_cribHand);
            SetMargins(_yourHand); //i think should be this too.
            Grid grid = new Grid();
            AddLeftOverRow(grid, 40);
            AddLeftOverRow(grid, 60);
            AddAutoRows(grid, 1);
            AddAutoColumns(grid, 1);
            AddLeftOverColumn(grid, 40);
            AddLeftOverColumn(grid, 40);
            AddControlToGrid(grid, stack, 2, 0);
            Grid.SetColumnSpan(stack, 3);
            stack.Orientation = StackOrientation.Horizontal;
            var cributton = GetGamingButton("To Crib", nameof(CribbagePatienceMainViewModel.CribAsync));
            var continueButton = GetGamingButton("Continue", nameof(CribbagePatienceMainViewModel.Continue)); //to experiment
            stack.Children.Add(cributton);
            stack.Children.Add(continueButton);
            StackLayout otherStack = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal
            };
            stack = new StackLayout();
            otherStack.Children.Add(_deckGPile);
            _startCard = new BasePileXF<CribbageCard, ts, DeckOfCardsXF<CribbageCard>>();
            otherStack.Children.Add(_startCard);
            stack.Children.Add(otherStack);
            stack.Children.Add(_yourHand);
            AddControlToGrid(grid, stack, 1, 0);
            AddControlToGrid(grid, _cribHand, 0, 0);
            AddControlToGrid(grid, _score1, 0, 2);
            _grid = grid;

            Content = grid;


        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {
            CribbagePatienceMainViewModel model = (CribbagePatienceMainViewModel)BindingContext;
            var hand1 = model.GetScoreHand(EnumHandCategory.Hand1);
            var hand2 = model.GetScoreHand(EnumHandCategory.Hand2);
            var crib = model.GetScoreHand(EnumHandCategory.Crib);
            _hand1Score.Init(hand1);
            _hand2Score.Init(hand2);
            _cribScore.Init(crib);
            _cribHand.LoadList(model.TempCrib, ts.TagUsed);
            _yourHand.LoadList(model.Hand1, ts.TagUsed);
            _score1.Init(model);
            return Task.CompletedTask;
        }


        object IUIView.DataContext
        {
            get => BindingContext;
            set => BindingContext = value;
        }
        Task IUIView.TryActivateAsync()
        {
            CribbagePatienceMainViewModel model = (CribbagePatienceMainViewModel)BindingContext;
            _deckGPile.Init(model.DeckPile, ts.TagUsed);
            if (model.StartPile == null)
            {
                throw new BasicBlankException("Start card was never set.  Rethink");
            }
            _startCard.Init(model.StartPile, ts.TagUsed);

            var (row, column) = model.GetRowColumn(EnumHandCategory.Hand1);
            AddControlToGrid(_grid, _hand1Score, row, column);
            var hand2D = model.GetRowColumn(EnumHandCategory.Hand2);
            AddControlToGrid(_grid, _hand2Score, hand2D.row, hand2D.column);
            var cribD = model.GetRowColumn(EnumHandCategory.Crib);
            AddControlToGrid(_grid, _cribScore, cribD.row, cribD.column);
            return Task.CompletedTask;
        }

        Task IUIView.TryCloseAsync()
        {
            _aggregator.Unsubscribe(this); //looks like you have to unsubscribe each time. now.
            return Task.CompletedTask;
        }
    }
}
