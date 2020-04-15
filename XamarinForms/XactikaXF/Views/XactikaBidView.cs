using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGamingUIXFLibrary.BasicControls.ChoicePickers;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using CommonBasicStandardLibraries.Messenging;
using System.Threading.Tasks;
using XactikaCP.Data;
using XactikaCP.ViewModels;
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
namespace XactikaXF.Views
{
    public class XactikaBidView : FrameUIViewXF
    {
        private readonly IEventAggregator _aggregator;

        public XactikaBidView(XactikaVMData model, IEventAggregator aggregator)
        {
            NumberChooserXF firstBid = new NumberChooserXF();
            firstBid.Rows = 2;
            firstBid.Columns = 5;
            Text = "Bid Info";
            StackLayout stack = new StackLayout();
            SetUpMarginsOnParentControl(stack);
            firstBid.Margin = new Thickness(3, 3, 3, 3);
            firstBid.LoadLists(model.Bid1!);
            stack.Children.Add(firstBid);
            Button button = GetGamingButton("Place Bid", nameof(XactikaBidViewModel.BidAsync));
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
