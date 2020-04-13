using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.SolitaireClasses.Cards;
using BasicGamingUIWPFLibrary.BasicControls.SingleCardFrames;
using BasicGamingUIWPFLibrary.BasicControls.SolitaireClasses;
using BasicGamingUIWPFLibrary.GameGraphics.Cards;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows;
using System.Windows.Controls;
using TriangleSolitaireCP.ViewModels;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace TriangleSolitaireWPF.Views
{
    public class TriangleSolitaireMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;

        private readonly BaseDeckWPF<SolitaireCard, ts, DeckOfCardsWPF<SolitaireCard>> _deckGPile;
        private readonly BasePileWPF<SolitaireCard, ts, DeckOfCardsWPF<SolitaireCard>> _discardGPile;
        private readonly TriangleWPF _triangle;
        public TriangleSolitaireMainView(IEventAggregator aggregator)
        {
            _aggregator = aggregator;
            _aggregator.Subscribe(this);
            _deckGPile = new BaseDeckWPF<SolitaireCard, ts, DeckOfCardsWPF<SolitaireCard>>();
            _discardGPile = new BasePileWPF<SolitaireCard, ts, DeckOfCardsWPF<SolitaireCard>>();
            _triangle = new TriangleWPF();
            StackPanel stack = new StackPanel();

            _deckGPile.Margin = new Thickness(5, 5, 5, 5);
            _deckGPile.HorizontalAlignment = HorizontalAlignment.Left;
            _deckGPile.VerticalAlignment = VerticalAlignment.Top;

            _discardGPile.Margin = new Thickness(5);
            _discardGPile.HorizontalAlignment = HorizontalAlignment.Left;
            _discardGPile.VerticalAlignment = VerticalAlignment.Top;
            StackPanel other = new StackPanel()
            {
                Orientation = Orientation.Horizontal
            };
            other.Children.Add(_deckGPile);
            other.Children.Add(_discardGPile);
            other.Children.Add(_triangle);
            stack.Children.Add(other);
            Content = stack; //if not doing this, rethink.
        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {
            return Task.CompletedTask;
        }



        Task IUIView.TryActivateAsync()
        {
            TriangleSolitaireMainViewModel model = (TriangleSolitaireMainViewModel)DataContext;
            _deckGPile.Init(model.DeckPile, ts.TagUsed);
            _discardGPile.Init(model.Pile1, ts.TagUsed);
            _triangle.Init(model.Triangle1);
            return Task.CompletedTask;
        }

        Task IUIView.TryCloseAsync()
        {
            _aggregator.Unsubscribe(this); //looks like you have to unsubscribe each time. now.
            return Task.CompletedTask;
        }
    }
}
