using BasicGamingUIXFLibrary.BasicControls.ChoicePickers;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using RookCP.Data;
using RookCP.ViewModels;
using System.Threading.Tasks;
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions;

namespace RookXF.Views
{
    public class RookBiddingView : FrameUIViewXF
    {
        private readonly RookVMData _model;
        readonly NumberChooserXF _firstBid;
        public RookBiddingView(RookVMData model)
        {
            NumberChooserXF firstBid = new NumberChooserXF();
            firstBid.Columns = 7;
            Text = "Bid Info";
            StackLayout stack = new StackLayout();
            SetUpMarginsOnParentControl(stack);
            stack.Children.Add(firstBid);
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            stack.Children.Add(otherStack);
            Button button = GetGamingButton("Place Bid", nameof(RookBiddingViewModel.BidAsync));
            otherStack.Children.Add(button);
            button = GetGamingButton("Pass", nameof(RookBiddingViewModel.PassAsync));
            otherStack.Children.Add(button);
            Grid grid = new Grid();
            grid.Children.Add(ThisDraw);
            grid.Children.Add(stack);
            Content = grid;
            _firstBid = firstBid;
            _model = model;
        }

        protected override Task TryActivateAsync()
        {
            _firstBid.LoadLists(_model.Bid1!);
            return base.TryActivateAsync();
        }


    }
}
