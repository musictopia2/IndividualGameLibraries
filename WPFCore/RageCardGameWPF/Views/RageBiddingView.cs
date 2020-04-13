using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGamingUIWPFLibrary.BasicControls.ChoicePickers;
using BasicGamingUIWPFLibrary.BasicControls.GameFrames;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using RageCardGameCP.Cards;
using RageCardGameCP.Data;
using RageCardGameCP.ViewModels;
using System.Threading.Tasks;
using System.Windows.Controls;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions;
namespace RageCardGameWPF.Views
{
    public class RageBiddingView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;

        public RageBiddingView(IEventAggregator aggregator, RageCardGameVMData model, RageCardGameGameContainer gameContainer)
        {
            _aggregator = aggregator;
            _aggregator.Subscribe(this);
            StackPanel stack = new StackPanel();
            NumberChooserWPF bid = new NumberChooserWPF();
            BaseHandWPF<RageCardGameCardInformation, RageCardGameGraphicsCP, CardGraphicsWPF> hand = new BaseHandWPF<RageCardGameCardInformation, RageCardGameGraphicsCP, CardGraphicsWPF>();
            ScoreBoardWPF score = new ScoreBoardWPF();
            RageCardGameMainView.PopulateScores(score);
            Button button = GetGamingButton("Submit", nameof(RageBiddingViewModel.BidAsync));
            SimpleLabelGrid bidInfo = new SimpleLabelGrid();
            bidInfo.AddRow("Trump", nameof(RageBiddingViewModel.TrumpSuit));
            bidInfo.AddRow("Turn", nameof(RageBiddingViewModel.NormalTurn));
            stack.Children.Add(bid);
            stack.Children.Add(button);
            stack.Children.Add(hand);
            stack.Children.Add(bidInfo.GetContent);
            stack.Children.Add(score);
            Content = stack;
            score!.LoadLists(gameContainer.SaveRoot.PlayerList);
            hand!.LoadList(model.PlayerHand1!, "");
            bid!.LoadLists(model.Bid1);
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
