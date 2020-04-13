using BasicGamingUIWPFLibrary.BasicControls.ChoicePickers;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using RookCP.Data;
using RookCP.ViewModels;
using System.Threading.Tasks;
using System.Windows.Controls;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.

namespace RookWPF.Views
{
    public class RookBiddingView : BaseFrameWPF, IUIView
    {

        public RookBiddingView(RookVMData model)
        {
            NumberChooserWPF firstBid = new NumberChooserWPF();
            firstBid.Columns = 7;
            Text = "Bid Info";
            var rect = ThisFrame.GetControlArea();
            StackPanel stack = new StackPanel();
            SetUpMarginsOnParentControl(stack, rect);
            firstBid.LoadLists(model.Bid1!);
            stack.Children.Add(firstBid);
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            stack.Children.Add(otherStack);
            Button button = GetGamingButton("Place Bid", nameof(RookBiddingViewModel.BidAsync));
            otherStack.Children.Add(button);
            button = GetGamingButton("Pass", nameof(RookBiddingViewModel.PassAsync));
            otherStack.Children.Add(button);
            Grid grid = new Grid();
            grid.Children.Add(ThisDraw);
            grid.Children.Add(stack);
            Content = grid;
        }

        Task IUIView.TryActivateAsync()
        {
            return Task.CompletedTask;
        }

        Task IUIView.TryCloseAsync()
        {
            return Task.CompletedTask;
        }
    }
}
