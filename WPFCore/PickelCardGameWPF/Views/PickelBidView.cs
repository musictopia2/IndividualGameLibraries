using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGamingUIWPFLibrary.BasicControls.GameFrames;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using BasicGamingUIWPFLibrary.GameGraphics.Cards;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using PickelCardGameCP.Cards;
using PickelCardGameCP.Data;
using PickelCardGameCP.ViewModels;
using System.Threading.Tasks;
using System.Windows.Controls;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;

namespace PickelCardGameWPF.Views
{
    public class PickelBidView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;

        public PickelBidView(PickelCardGameVMData model, IEventAggregator aggregator )
        {
            StackPanel stack = new StackPanel();
            BidControl bid = new BidControl();
            bid.LoadLists(model);
            BaseHandWPF<PickelCardGameCardInformation, ts, DeckOfCardsWPF<PickelCardGameCardInformation>> hand = new BaseHandWPF<PickelCardGameCardInformation, ts, DeckOfCardsWPF<PickelCardGameCardInformation>>();
            stack.Children.Add(bid);
            stack.Children.Add(hand);

            hand.LoadList(model.PlayerHand1, ts.TagUsed);

            Content = stack;
            _aggregator = aggregator;
            _aggregator.Subscribe(this);
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
            _aggregator.Unsubscribe(this);
            return Task.CompletedTask;
        }
    }
}
