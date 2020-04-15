using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGamingUIXFLibrary.BasicControls.GameFrames;
using BasicGamingUIXFLibrary.GameGraphics.Cards;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using CommonBasicStandardLibraries.Messenging;
using PickelCardGameCP.Cards;
using PickelCardGameCP.Data;
using System.Threading.Tasks;
using Xamarin.Forms;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;

namespace PickelCardGameXF.Views
{
    public class PickelBidView : CustomControlBase, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;

        public PickelBidView(PickelCardGameVMData model, IEventAggregator aggregator)
        {
            StackLayout stack = new StackLayout();
            BidControl bid = new BidControl();
            bid.LoadLists(model);
            BaseHandXF<PickelCardGameCardInformation, ts, DeckOfCardsXF<PickelCardGameCardInformation>> hand = new BaseHandXF<PickelCardGameCardInformation, ts, DeckOfCardsXF<PickelCardGameCardInformation>>();
            stack.Children.Add(bid);
            stack.Children.Add(hand);
            hand.LoadList(model.PlayerHand1, ts.TagUsed);

            Content = stack;
            _aggregator = aggregator;
            _aggregator.Subscribe(this);
        }
        protected override Task TryActivateAsync()
        {
            return this.RefreshBindingsAsync(_aggregator);
        }
        protected override Task TryCloseAsync()
        {
            _aggregator.Unsubscribe(this);
            return base.TryCloseAsync();
        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {
            return Task.CompletedTask;
        }

    }
}
