using BasicGamingUIWPFLibrary.BasicControls.ChoicePickers;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using XactikaCP.Data;
using XactikaCP.ViewModels;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions;
namespace XactikaWPF.Views
{
    public class XactikaBidView : BaseFrameWPF, IUIView
    {
        public XactikaBidView(XactikaVMData model)
        {
            NumberChooserWPF firstBid = new NumberChooserWPF();
            firstBid.Rows = 2;
            firstBid.Columns = 5;
            Text = "Bid Info";
            var rect = ThisFrame.GetControlArea();
            StackPanel stack = new StackPanel();
            SetUpMarginsOnParentControl(stack, rect);
            firstBid.Margin = new Thickness(3, 3, 3, 3);
            firstBid.LoadLists(model.Bid1!);
            stack.Children.Add(firstBid);
            Button button = GetGamingButton("Place Bid", nameof(XactikaBidViewModel.BidAsync));
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
