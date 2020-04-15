using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGamingUIXFLibrary.BasicControls.ChoicePickers;
using BasicGamingUIXFLibrary.BasicControls.GameFrames;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using CommonBasicStandardLibraries.Messenging;
using RageCardGameCP.Cards;
using RageCardGameCP.Data;
using RageCardGameCP.ViewModels;
using System.Threading.Tasks;
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.

namespace RageCardGameXF.Views
{
    public class RageBiddingView : CustomControlBase, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;

        public RageBiddingView(IEventAggregator aggregator, RageCardGameVMData model, RageCardGameGameContainer gameContainer)
        {
            _aggregator = aggregator;
            _aggregator.Subscribe(this);
            StackLayout stack = new StackLayout();
            NumberChooserXF bid = new NumberChooserXF();
            BaseHandXF<RageCardGameCardInformation, RageCardGameGraphicsCP, CardGraphicsXF> hand = new BaseHandXF<RageCardGameCardInformation, RageCardGameGraphicsCP, CardGraphicsXF>();
            ScoreBoardXF score = new ScoreBoardXF();
            RageCardGameMainView.PopulateScores(score);
            Button button = GetGamingButton("Submit", nameof(RageBiddingViewModel.BidAsync));
            SimpleLabelGridXF bidInfo = new SimpleLabelGridXF();
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

        protected override Task TryCloseAsync()
        {
            _aggregator.Unsubscribe(this);
            return base.TryCloseAsync();
        }


    }
}
