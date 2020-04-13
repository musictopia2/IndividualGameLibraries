using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using BasicGamingUIWPFLibrary.BasicControls.SingleCardFrames;
using BasicGamingUIWPFLibrary.GameGraphics.Cards;
using BladesOfSteelCP.Data;
using BladesOfSteelCP.ViewModels;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;

namespace BladesOfSteelWPF.Views
{
    public class FaceoffView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly BaseDeckWPF<RegularSimpleCard, ts, DeckOfCardsWPF<RegularSimpleCard>> _deck;
        private readonly BladesOfSteelVMData _model;
        private readonly IEventAggregator _aggregator;
        private readonly BasePileWPF<RegularSimpleCard, ts, DeckOfCardsWPF<RegularSimpleCard>> _yourFace;
        private readonly BasePileWPF<RegularSimpleCard, ts, DeckOfCardsWPF<RegularSimpleCard>> _opponentFace;

        public FaceoffView(BladesOfSteelVMData model, IEventAggregator aggregator)
        {
            aggregator.Subscribe(this);
            _yourFace = new BasePileWPF<RegularSimpleCard, ts, DeckOfCardsWPF<RegularSimpleCard>>();
            _opponentFace = new BasePileWPF<RegularSimpleCard, ts, DeckOfCardsWPF<RegularSimpleCard>>();

            StackPanel stack = new StackPanel();
            _deck = new BaseDeckWPF<RegularSimpleCard, ts, DeckOfCardsWPF<RegularSimpleCard>>();
            _model = model;
            _aggregator = aggregator;
            _deck.HorizontalAlignment = HorizontalAlignment.Left;
            _deck.VerticalAlignment = VerticalAlignment.Top;
            stack.Children.Add(_deck);
            SimpleLabelGrid firsts = new SimpleLabelGrid();
            firsts.AddRow("Instructions", nameof(FaceoffViewModel.Instructions));
            stack.Children.Add(firsts.GetContent);

            StackPanel other = new StackPanel()
            {
                Orientation = Orientation.Horizontal
            };
            stack.Children.Add(other);
            other.Children.Add(_yourFace);
            other.Children.Add(_opponentFace);
            _yourFace.HorizontalAlignment = HorizontalAlignment.Left;
            _yourFace.VerticalAlignment = VerticalAlignment.Top;
            _opponentFace.HorizontalAlignment = HorizontalAlignment.Left;
            _yourFace.VerticalAlignment = VerticalAlignment.Top;
            _yourFace.Margin = new Thickness(5);
            _opponentFace.Margin = new Thickness(5);
            Content = stack;
        }

        

        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {
            _deck.Init(_model.Deck1, ts.TagUsed);
            _yourFace.Init(_model.YourFaceOffCard, ts.TagUsed);
            _opponentFace.Init(_model.OpponentFaceOffCard, ts.TagUsed);
            return Task.CompletedTask;
        }

        Task IUIView.TryActivateAsync()
        {
            return Task.CompletedTask;
        }

        Task IUIView.TryCloseAsync()
        {
            _aggregator.Unsubscribe(this);
            return Task.CompletedTask;
        }
    }
}
