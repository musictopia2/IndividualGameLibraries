using BasicGamingUIXFLibrary.BasicControls.ChoicePickers;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using SkuckCardGameCP.Data;
using SkuckCardGameCP.ViewModels;
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions;

namespace SkuckCardGameXF.Views
{
    public class SkuckBiddingView : FrameUIViewXF
    {
        public SkuckBiddingView(SkuckCardGameVMData model)
        {
            Text = "Bid Info";
            Grid grid = new Grid();
            StackLayout thisStack = new StackLayout();
            SetUpMarginsOnParentControl(thisStack);
            NumberChooserXF bid = new NumberChooserXF();
            bid.Columns = 9;
            bid.LoadLists(model.Bid1!);
            thisStack.Children.Add(bid);
            Button button = GetGamingButton("Place Bid", nameof(SkuckBiddingViewModel.BidAsync));
            thisStack.Children.Add(button);
            grid.Children.Add(ThisDraw);
            grid.Children.Add(thisStack);
            Content = grid;
        }



    }
}
