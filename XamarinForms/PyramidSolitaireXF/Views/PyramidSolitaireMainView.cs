using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.SolitaireClasses.Cards;
using BasicGamingUIXFLibrary.BasicControls.GameFrames;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicGamingUIXFLibrary.BasicControls.SingleCardFrames;
using BasicGamingUIXFLibrary.BasicControls.SolitaireClasses;
using BasicGamingUIXFLibrary.GameGraphics.Cards;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using PyramidSolitaireCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;

namespace PyramidSolitaireXF.Views
{
    public class PyramidSolitaireMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        //TODO:  has to build the main page.  no need for new game since that is completely separated out into another class.
        private readonly IEventAggregator _aggregator;

        private readonly BaseDeckXF<SolitaireCard, ts, DeckOfCardsXF<SolitaireCard>> _deckGPile;

        private readonly BasePileXF<SolitaireCard, ts, DeckOfCardsXF<SolitaireCard>> _discardGPile;
        private readonly BasePileXF<SolitaireCard, ts, DeckOfCardsXF<SolitaireCard>> _currentCard;
        private readonly CardBoardXF<SolitaireCard, ts, DeckOfCardsXF<SolitaireCard>> _playerBoard;
        private readonly TriangleXF _triangle;
        public PyramidSolitaireMainView(IEventAggregator aggregator)
        {
            _aggregator = aggregator;
            _aggregator.Subscribe(this);
            _deckGPile = new BaseDeckXF<SolitaireCard, ts, DeckOfCardsXF<SolitaireCard>>();
            _discardGPile = new BasePileXF<SolitaireCard, ts, DeckOfCardsXF<SolitaireCard>>();
            _discardGPile.Margin = new Thickness(5);
            _discardGPile.HorizontalOptions = LayoutOptions.Start;
            _discardGPile.VerticalOptions = LayoutOptions.Start;

            StackLayout stack = new StackLayout();
            _currentCard = new BasePileXF<SolitaireCard, ts, DeckOfCardsXF<SolitaireCard>>();
            _playerBoard = new CardBoardXF<SolitaireCard, ts, DeckOfCardsXF<SolitaireCard>>();
            _triangle = new TriangleXF();
            var playButton = GetGamingButton("Play Selected Cards", nameof(PyramidSolitaireMainViewModel.PlaySelectedCardsAsync));
            stack.Children.Add(_deckGPile);

            _deckGPile.Margin = new Thickness(5, 5, 5, 5);
            _deckGPile.HorizontalOptions = LayoutOptions.Start;
            _deckGPile.VerticalOptions = LayoutOptions.Start;


            StackLayout other = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal
            };

            other.Children.Add(_deckGPile);
            other.Children.Add(_discardGPile);
            other.Children.Add(_currentCard);
            other.Children.Add(_triangle);
            stack.Children.Add(other);
            stack.Children.Add(_playerBoard);

            playButton.HorizontalOptions = LayoutOptions.Start;
            playButton.VerticalOptions = LayoutOptions.Start;
            stack.Children.Add(playButton);
            var thisLabel = new SimpleLabelGridXF();
            thisLabel.AddRow("Score", nameof(PyramidSolitaireMainViewModel.Score));
            stack.Children.Add(thisLabel.GetContent);

            Content = stack; //if not doing this, rethink.
        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {
            return Task.CompletedTask;
        }


        object IUIView.DataContext
        {
            get => BindingContext;
            set => BindingContext = value;
        }
        Task IUIView.TryActivateAsync()
        {
            PyramidSolitaireMainViewModel model = (PyramidSolitaireMainViewModel)BindingContext;
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
