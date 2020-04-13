using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.SolitaireClasses.Cards;
using BasicGamingUIWPFLibrary.BasicControls.GameFrames;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using BasicGamingUIWPFLibrary.BasicControls.SingleCardFrames;
using BasicGamingUIWPFLibrary.BasicControls.SolitaireClasses;
using BasicGamingUIWPFLibrary.GameGraphics.Cards;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using PyramidSolitaireCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows;
using System.Windows.Controls;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;

namespace PyramidSolitaireWPF.Views
{
    public class PyramidSolitaireMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        //TODO:  has to build the main page.  no need for new game since that is completely separated out into another class.
        private readonly IEventAggregator _aggregator;

        private readonly BaseDeckWPF<SolitaireCard, ts, DeckOfCardsWPF<SolitaireCard>> _deckGPile;
        private readonly BasePileWPF<SolitaireCard, ts, DeckOfCardsWPF<SolitaireCard>> _discardGPile;
        private readonly BasePileWPF<SolitaireCard, ts, DeckOfCardsWPF<SolitaireCard>> _currentCard;
        private readonly CardBoardWPF<SolitaireCard, ts, DeckOfCardsWPF<SolitaireCard>> _playerBoard;
        private readonly TriangleWPF _triangle;
        public PyramidSolitaireMainView(IEventAggregator aggregator)
        {
            _aggregator = aggregator;
            _aggregator.Subscribe(this);
            _deckGPile = new BaseDeckWPF<SolitaireCard, ts, DeckOfCardsWPF<SolitaireCard>>();
            _discardGPile = new BasePileWPF<SolitaireCard, ts, DeckOfCardsWPF<SolitaireCard>>();
            _discardGPile.Margin = new Thickness(5);
            _discardGPile.HorizontalAlignment = HorizontalAlignment.Left;
            _discardGPile.VerticalAlignment = VerticalAlignment.Top;
            StackPanel stack = new StackPanel();
            _currentCard = new BasePileWPF<SolitaireCard, ts, DeckOfCardsWPF<SolitaireCard>>();
            _playerBoard = new CardBoardWPF<SolitaireCard, ts, DeckOfCardsWPF<SolitaireCard>>();
            _triangle = new TriangleWPF();
            var playButton = GetGamingButton("Play Selected Cards", nameof(PyramidSolitaireMainViewModel.PlaySelectedCardsAsync));

            _deckGPile.Margin = new Thickness(5, 5, 5, 5);
            _deckGPile.HorizontalAlignment = HorizontalAlignment.Left;
            _deckGPile.VerticalAlignment = VerticalAlignment.Top;

            StackPanel other = new StackPanel()
            {
                Orientation = Orientation.Horizontal
            };

            other.Children.Add(_deckGPile);
            other.Children.Add(_discardGPile);
            other.Children.Add(_currentCard);
            other.Children.Add(_triangle);
            stack.Children.Add(other);
            stack.Children.Add(_playerBoard);

            playButton.HorizontalAlignment = HorizontalAlignment.Left;
            playButton.VerticalAlignment = VerticalAlignment.Top;
            stack.Children.Add(playButton);
            var thisLabel = new SimpleLabelGrid();
            thisLabel.AddRow("Score", nameof(PyramidSolitaireMainViewModel.Score));
            stack.Children.Add(thisLabel.GetContent);

            Content = stack; //if not doing this, rethink.
        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {
            return Task.CompletedTask;
        }



        Task IUIView.TryActivateAsync()
        {
            PyramidSolitaireMainViewModel model = (PyramidSolitaireMainViewModel)DataContext;
            _deckGPile.Init(model.DeckPile, ts.TagUsed);
            _discardGPile.Init(model.Discard, ts.TagUsed);
            _currentCard.Init(model.CurrentPile, ts.TagUsed);
            _playerBoard.LoadList(model.PlayList1, ts.TagUsed);
            _triangle.Init(model.GameBoard1!);
            return Task.CompletedTask;
        }

        Task IUIView.TryCloseAsync()
        {
            _aggregator.Unsubscribe(this); //looks like you have to unsubscribe each time. now.
            return Task.CompletedTask;
        }
    }
}
