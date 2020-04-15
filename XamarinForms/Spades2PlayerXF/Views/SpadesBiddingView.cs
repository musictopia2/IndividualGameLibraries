using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGamingUIXFLibrary.BasicControls.ChoicePickers;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using CommonBasicStandardLibraries.Messenging;
using Spades2PlayerCP.Data;
using Spades2PlayerCP.ViewModels;
using System.Threading.Tasks;
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions;

namespace Spades2PlayerXF.Views
{
    public class SpadesBiddingView : FrameUIViewXF
    {
        private readonly IEventAggregator _aggregator;

        public SpadesBiddingView(Spades2PlayerVMData model, IEventAggregator aggregator)
        {
            NumberChooserXF firstBid = new NumberChooserXF();
            firstBid.Columns = 7;
            //firstBid.Rows = 2;
            Text = "Bid Info";
            StackLayout stack = new StackLayout();
            SetUpMarginsOnParentControl(stack);
            firstBid.Margin = new Thickness(3, 3, 3, 3);
            firstBid.LoadLists(model.Bid1!);
            stack.Children.Add(firstBid);
            Button button = GetSmallerButton("Place Bid", nameof(SpadesBiddingViewModel.BidAsync));
            button.Margin = new Thickness(3, -60, 3, 3);
            stack.Children.Add(button);
            Grid grid = new Grid();
            grid.Children.Add(ThisDraw);
            grid.Children.Add(stack);
            Content = grid;
            _aggregator = aggregator;
        }
        protected override Task TryActivateAsync()
        {
            return this.RefreshBindingsAsync(_aggregator);
        }

    }
}
