using BasicGamingUIWPFLibrary.BasicControls.ChoicePickers;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using SkuckCardGameCP.Data;
using SkuckCardGameCP.ViewModels;
using System.Threading.Tasks;
using System.Windows.Controls;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.

namespace SkuckCardGameWPF.Views
{
    public class SkuckBiddingView : BaseFrameWPF, IUIView
    {
        public SkuckBiddingView(SkuckCardGameVMData model)
        {
            Text = "Bid Info";
            Grid grid = new Grid();
            var rect = ThisFrame.GetControlArea();
            StackPanel thisStack = new StackPanel();
            SetUpMarginsOnParentControl(thisStack, rect);
            NumberChooserWPF bid = new NumberChooserWPF();
            bid.Columns = 9;
            bid.LoadLists(model.Bid1!);
            thisStack.Children.Add(bid);
            Button button = GetGamingButton("Place Bid", nameof(SkuckBiddingViewModel.BidAsync));
            thisStack.Children.Add(button);
            grid.Children.Add(ThisDraw);
            grid.Children.Add(thisStack);
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
