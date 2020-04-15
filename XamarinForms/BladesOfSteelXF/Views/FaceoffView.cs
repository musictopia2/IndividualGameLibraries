using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicGamingUIXFLibrary.BasicControls.SingleCardFrames;
using BasicGamingUIXFLibrary.GameGraphics.Cards;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using BladesOfSteelCP.Data;
using BladesOfSteelCP.ViewModels;
using CommonBasicStandardLibraries.Messenging;
using System.Threading.Tasks;
using Xamarin.Forms;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;

namespace BladesOfSteelXF.Views
{
    public class FaceoffView : CustomControlBase, IHandleAsync<LoadEventModel>
    {
        private readonly BaseDeckXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>> _deck;
        private readonly BladesOfSteelVMData _model;
        private readonly IEventAggregator _aggregator;
        private readonly BasePileXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>> _yourFace;
        private readonly BasePileXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>> _opponentFace;

        public FaceoffView(BladesOfSteelVMData model, IEventAggregator aggregator)
        {
            aggregator.Subscribe(this);
            _yourFace = new BasePileXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>>();
            _opponentFace = new BasePileXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>>();

            StackLayout stack = new StackLayout();
            _deck = new BaseDeckXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>>();
            _model = model;
            _aggregator = aggregator;
            _deck.HorizontalOptions = LayoutOptions.Start;
            _deck.VerticalOptions = LayoutOptions.Start;
            stack.Children.Add(_deck);
            SimpleLabelGridXF firsts = new SimpleLabelGridXF();
            firsts.AddRow("Instructions", nameof(FaceoffViewModel.Instructions));
            stack.Children.Add(firsts.GetContent);

            StackLayout other = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal
            };
            stack.Children.Add(other);
            other.Children.Add(_yourFace);
            other.Children.Add(_opponentFace);
            _yourFace.HorizontalOptions = LayoutOptions.Start;
            _yourFace.VerticalOptions = LayoutOptions.Start;
            _opponentFace.HorizontalOptions = LayoutOptions.Start;
            _yourFace.VerticalOptions = LayoutOptions.Start;
            _yourFace.Margin = new Thickness(5);
            _opponentFace.Margin = new Thickness(5);
            Content = stack;
        }
        protected override Task TryCloseAsync()
        {
            _aggregator.Unsubscribe(this);
            return base.TryCloseAsync();
        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {
            _deck.Init(_model.Deck1, ts.TagUsed);
            _yourFace.Init(_model.YourFaceOffCard, ts.TagUsed);
            _opponentFace.Init(_model.OpponentFaceOffCard, ts.TagUsed);
            return Task.CompletedTask;
        }


    }
}