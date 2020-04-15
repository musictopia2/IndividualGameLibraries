using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicGamingUIXFLibrary.BasicControls.SingleCardFrames;
using BasicGamingUIXFLibrary.GameGraphics.Cards;
using CommonBasicStandardLibraries.Messenging;
using Spades2PlayerCP.Cards;
using Spades2PlayerCP.Data;
using Spades2PlayerCP.ViewModels;
using System.Threading.Tasks;
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;

namespace Spades2PlayerXF.Views
{
    public class SpadesBeginningView : FrameUIViewXF
    {
        private readonly BaseDeckXF<Spades2PlayerCardInformation, ts, DeckOfCardsXF<Spades2PlayerCardInformation>> _deckGPile;
        private readonly BasePileXF<Spades2PlayerCardInformation, ts, DeckOfCardsXF<Spades2PlayerCardInformation>> _discardGPile;
        private readonly BasePileXF<Spades2PlayerCardInformation, ts, DeckOfCardsXF<Spades2PlayerCardInformation>> _other;
        private readonly Spades2PlayerVMData _model;
        private readonly IEventAggregator _aggregator;

        public SpadesBeginningView(Spades2PlayerVMData model, IEventAggregator aggregator)
        {
            StackLayout stack = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal
            };
            _deckGPile = new BaseDeckXF<Spades2PlayerCardInformation, ts, DeckOfCardsXF<Spades2PlayerCardInformation>>();
            _discardGPile = new BasePileXF<Spades2PlayerCardInformation, ts, DeckOfCardsXF<Spades2PlayerCardInformation>>();
            _other = new BasePileXF<Spades2PlayerCardInformation, ts, DeckOfCardsXF<Spades2PlayerCardInformation>>();
            stack.Children.Add(_deckGPile);
            stack.Children.Add(_discardGPile);
            var button = GetGamingButton("Take Card", nameof(SpadesBeginningViewModel.TakeCardAsync));
            stack.Children.Add(button);
            stack.Children.Add(_other);
            _deckGPile.Margin = new Thickness(5, 5, 5, 5);

            _discardGPile.Margin = new Thickness(5, 5, 5, 5);
            _other.Margin = new Thickness(5);

            Content = stack;
            _model = model;
            _aggregator = aggregator;
        }
        protected override Task TryActivateAsync()
        {
            _discardGPile!.Init(_model.Pile1!, ts.TagUsed); // may have to be here (well see)
            _discardGPile.StartListeningDiscardPile(); // its the main one.

            _deckGPile!.Init(_model.Deck1!, ts.TagUsed); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();

            _other.Init(_model.OtherPile!, ts.TagUsed);
            _other.StartAnimationListener("otherpile");
            
            return this.RefreshBindingsAsync(_aggregator);
        }
        protected override Task TryCloseAsync()
        {
            _other.StopListening();
            _deckGPile.StopListening();
            _discardGPile.StopListening();
            return base.TryCloseAsync();
        }
    }
}
