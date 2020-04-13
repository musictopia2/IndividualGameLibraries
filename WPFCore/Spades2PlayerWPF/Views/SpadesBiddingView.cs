using BasicGamingUIWPFLibrary.BasicControls.ChoicePickers;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using Spades2PlayerCP.Data;
using Spades2PlayerCP.ViewModels;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.

namespace Spades2PlayerWPF.Views
{
    public class SpadesBiddingView : BaseFrameWPF, IUIView
    {
        public SpadesBiddingView(Spades2PlayerVMData model)
        {
            NumberChooserWPF firstBid = new NumberChooserWPF();
            firstBid.Columns = 7;
            Text = "Bid Info";
            var rect = ThisFrame.GetControlArea();
            StackPanel stack = new StackPanel();
            SetUpMarginsOnParentControl(stack, rect);
            firstBid.Margin = new Thickness(3, 3, 3, 3);
            firstBid.LoadLists(model.Bid1!);
            stack.Children.Add(firstBid);
            Button button = GetGamingButton("Place Bid", nameof(SpadesBiddingViewModel.BidAsync));
            stack.Children.Add(button);
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
