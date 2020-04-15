using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.GameGraphicsCP.GamePieces;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGamingUIXFLibrary.BasicControls.ChoicePickers;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicGamingUIXFLibrary.GameGraphics.GamePieces;
using CommonBasicStandardLibraries.Messenging;
using SkuckCardGameCP.Data;
using SkuckCardGameCP.ViewModels;
using System.Threading.Tasks;
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions;
namespace SkuckCardGameWPF.Views
{
    public class SkuckSuitView : FrameUIViewXF
    {
        private readonly SkuckCardGameVMData _model;
        private readonly IEventAggregator _aggregator;
        readonly EnumPickerXF<DeckPieceCP, DeckPieceXF, EnumSuitList> _suit;
        public SkuckSuitView(SkuckCardGameVMData model, IEventAggregator aggregator)
        {
            Text = "Trump Info";
            Grid grid = new Grid();
            StackLayout thisStack = new StackLayout();
            SetUpMarginsOnParentControl(thisStack);
            EnumPickerXF<DeckPieceCP, DeckPieceXF, EnumSuitList> thisSuit = new EnumPickerXF<DeckPieceCP, DeckPieceXF, EnumSuitList>();
            thisStack.Children.Add(thisSuit);
            Button thisBut = GetGamingButton("Choose Trump Suit", nameof(SkuckSuitViewModel.TrumpAsync));
            thisStack.Children.Add(thisBut);
            grid.Children.Add(ThisDraw);
            grid.Children.Add(thisStack);
            _suit = thisSuit;
            Content = grid;
            _model = model;
            _aggregator = aggregator;
        }

        protected override Task TryActivateAsync()
        {
            _suit.LoadLists(_model.Suit1!); //try this way.
            return this.RefreshBindingsAsync(_aggregator);
        }
        protected override Task TryCloseAsync()
        {
            _suit.Dispose();
            return base.TryCloseAsync();
        }
    }
}