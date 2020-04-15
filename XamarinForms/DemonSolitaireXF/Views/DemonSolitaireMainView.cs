using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.SolitaireClasses.Cards;
using BasicGameFrameworkLibrary.SolitaireClasses.PileObservable;
using BasicGamingUIXFLibrary.BasicControls.MultipleFrameContainers;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicGamingUIXFLibrary.BasicControls.SingleCardFrames;
using BasicGamingUIXFLibrary.BasicControls.SolitaireClasses;
using BasicGamingUIXFLibrary.GameGraphics.Cards;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using DemonSolitaireCP.Logic;
using DemonSolitaireCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using static BasicXFControlsAndPages.Helpers.GridHelper; //since i use the grid a lot too.
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;

namespace DemonSolitaireXF.Views
{
    public class DemonSolitaireMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;

        private readonly BaseDeckXF<SolitaireCard, ts, DeckOfCardsXF<SolitaireCard>> _deckGPile;
        private readonly BasePileXF<SolitaireCard, ts, DeckOfCardsXF<SolitaireCard>> _discardGPile;
        private readonly BasicMultiplePilesXF<SolitaireCard, ts, DeckOfCardsXF<SolitaireCard>> _main; //if something change here.
        private readonly SolitairePilesXF _waste;
        private readonly BaseDeckXF<SolitaireCard, ts, DeckOfCardsXF<SolitaireCard>> _heel = new BaseDeckXF<SolitaireCard, ts, DeckOfCardsXF<SolitaireCard>>();

        public DemonSolitaireMainView(IEventAggregator aggregator)
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
            _main = new BasicMultiplePilesXF<SolitaireCard, ts, DeckOfCardsXF<SolitaireCard>>();
            _main.Margin = new Thickness(10, 5, 5, 5);
            var autoBut = GetGamingButton("Auto Make Move", nameof(DemonSolitaireMainViewModel.AutoMoveAsync));
            var scoresAlone = new SimpleLabelGridXF();
            scoresAlone.AddRow("Score", nameof(DemonSolitaireMainViewModel.Score));
            scoresAlone.AddRow("Starting Number", nameof(DemonSolitaireMainViewModel.StartingNumber));
            var tempGrid = scoresAlone.GetContent;
            tempGrid.WidthRequest = 150;
            _waste = new SolitairePilesXF();
            _deckGPile.Margin = new Thickness(5, 5, 5, 5);
            _discardGPile.Margin = new Thickness(5, 5, 5, 5);
            _heel.HorizontalOptions = LayoutOptions.Start;
            _heel.VerticalOptions = LayoutOptions.Start;
            Grid grid = new Grid();
            AddAutoColumns(grid, 3);
            AddAutoRows(grid, 2);
            AddControlToGrid(grid, _deckGPile, 0, 0);
            AddControlToGrid(grid, _discardGPile, 0, 1);
            AddControlToGrid(grid, _main, 0, 2);
            AddControlToGrid(grid, _heel, 1, 1);
            AddControlToGrid(grid, _waste, 1, 2);
            stack.Children.Add(tempGrid);
            AddControlToGrid(grid, stack, 1, 0);
            _heel.Margin = new Thickness(0, 5, 0, 0);
            _waste.Margin = new Thickness(0, 25, 0, 0);
            stack.Margin = new Thickness(0, 5, 5, 0);
            Content = grid;
        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {
            DemonSolitaireMainViewModel model = (DemonSolitaireMainViewModel)BindingContext;
            var tempWaste = (WastePiles)model.WastePiles1!;
            _waste.Init(tempWaste.Piles);
            var tempMain = (MainPilesCP)model.MainPiles1!;
            _main.Init(tempMain.Piles, ts.TagUsed);
            return Task.CompletedTask;
        }


        object IUIView.DataContext
        {
            get => BindingContext;
            set => BindingContext = value;
        }
        Task IUIView.TryActivateAsync()
        {
            DemonSolitaireMainViewModel model = (DemonSolitaireMainViewModel)BindingContext;
            _deckGPile.Init(model.DeckPile, ts.TagUsed);
            _discardGPile.Init(model.MainDiscardPile, ts.TagUsed);
            _heel.Init(model.Heel1, ts.TagUsed);
            return Task.CompletedTask;
        }

        Task IUIView.TryCloseAsync()
        {
            _aggregator.Unsubscribe(this); //looks like you have to unsubscribe each time. now.
            return Task.CompletedTask;
        }
    }
}
