using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.SolitaireClasses.Cards;
using BasicGameFrameworkLibrary.SolitaireClasses.PileObservable;
using BasicGamingUIXFLibrary.BasicControls.MultipleFrameContainers;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicGamingUIXFLibrary.BasicControls.SingleCardFrames;
using BasicGamingUIXFLibrary.GameGraphics.Cards;
using CarpetSolitaireCP.Logic;
using CarpetSolitaireCP.ViewModels;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using static BasicXFControlsAndPages.Helpers.GridHelper; //since i use the grid a lot too.
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;

namespace CarpetSolitaireXF.Views
{
    public class CarpetSolitaireMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;

        private readonly BaseDeckXF<SolitaireCard, ts, DeckOfCardsXF<SolitaireCard>> _deckGPile;
        private readonly BasePileXF<SolitaireCard, ts, DeckOfCardsXF<SolitaireCard>> _discardGPile;
        private readonly BasicMultiplePilesXF<SolitaireCard, ts, DeckOfCardsXF<SolitaireCard>> _main; //if something change here.
        private readonly BasicMultiplePilesXF<SolitaireCard, ts, DeckOfCardsXF<SolitaireCard>> _discard = new BasicMultiplePilesXF<SolitaireCard, ts, DeckOfCardsXF<SolitaireCard>>();
        public CarpetSolitaireMainView(IEventAggregator aggregator)
        {
            _aggregator = aggregator;
            _aggregator.Subscribe(this);
            _deckGPile = new BaseDeckXF<SolitaireCard, ts, DeckOfCardsXF<SolitaireCard>>();

            _deckGPile.Margin = new Thickness(5, 5, 5, 5);
            _deckGPile.HorizontalOptions = LayoutOptions.Start;
            _deckGPile.VerticalOptions = LayoutOptions.Start;
            _discardGPile = new BasePileXF<SolitaireCard, ts, DeckOfCardsXF<SolitaireCard>>();
            _discardGPile.Margin = new Thickness(5);
            _discardGPile.HorizontalOptions = LayoutOptions.Start;
            _discardGPile.VerticalOptions = LayoutOptions.Start;

            StackLayout stack = new StackLayout();
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile);
            _main = new BasicMultiplePilesXF<SolitaireCard, ts, DeckOfCardsXF<SolitaireCard>>();
            _main.Margin = new Thickness(10, 5, 5, 5);
            stack.Children.Add(otherStack);
            var autoBut = GetGamingButton("Auto Make Move", nameof(CarpetSolitaireMainViewModel.AutoMoveAsync));
            autoBut.HorizontalOptions = LayoutOptions.Start;
            autoBut.VerticalOptions = LayoutOptions.Start;
            var scoresAlone = new SimpleLabelGridXF();
            scoresAlone.AddRow("Score", nameof(CarpetSolitaireMainViewModel.Score));
            var tempGrid = scoresAlone.GetContent;
            tempGrid.WidthRequest = 50; //sometimes i am forced to do this 
            _deckGPile.Margin = new Thickness(5, 5, 5, 5);
            _discardGPile.Margin = new Thickness(5, 5, 5, 5);
            Grid grid = new Grid();
            AddAutoColumns(grid, 2);
            stack.Children.Add(tempGrid);
            stack.Children.Add(autoBut);
            AddControlToGrid(grid, stack, 0, 0);
            stack = new StackLayout();
            stack.Children.Add(_main);
            stack.Children.Add(_discard);
            AddControlToGrid(grid, stack, 0, 1);
            Content = grid;
        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {
            CarpetSolitaireMainViewModel model = (CarpetSolitaireMainViewModel)BindingContext;
            var tempWaste = (WastePiles)model.WastePiles1!;
            var tempMain = (MainPilesCP)model.MainPiles1!;
            _main.Init(tempMain.Piles, ts.TagUsed);

            _discard.Init(tempWaste.Discards!, ts.TagUsed);

            return Task.CompletedTask;
        }


        object IUIView.DataContext
        {
            get => BindingContext;
            set => BindingContext = value;
        }
        Task IUIView.TryActivateAsync()
        {
            CarpetSolitaireMainViewModel model = (CarpetSolitaireMainViewModel)BindingContext;
            _deckGPile.Init(model.DeckPile, ts.TagUsed);
            _discardGPile.Init(model.MainDiscardPile, ts.TagUsed);
            return Task.CompletedTask;
        }

        Task IUIView.TryCloseAsync()
        {
            _aggregator.Unsubscribe(this); //looks like you have to unsubscribe each time. now.
            return Task.CompletedTask;
        }
    }
}