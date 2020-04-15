using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.SolitaireClasses.Cards;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicGamingUIXFLibrary.BasicControls.SingleCardFrames;
using BasicGamingUIXFLibrary.GameGraphics.Cards;
using CaptiveQueensSolitaireCP.ViewModels;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;

namespace CaptiveQueensSolitaireXF.Views
{
    public class CaptiveQueensSolitaireMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;

        private readonly BaseDeckXF<SolitaireCard, ts, DeckOfCardsXF<SolitaireCard>> _deckGPile;
        private readonly BasePileXF<SolitaireCard, ts, DeckOfCardsXF<SolitaireCard>> _discardGPile;
        private readonly MainUI _main; //if something change here.
        public CaptiveQueensSolitaireMainView(IEventAggregator aggregator)
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
            stack.Children.Add(otherStack);
            _main = new MainUI();
            _main.Margin = new Thickness(10, 5, 5, 5);
            var autoBut = GetGamingButton("Auto Make Move", nameof(CaptiveQueensSolitaireMainViewModel.AutoMoveAsync));
            var scoresAlone = new SimpleLabelGridXF();
            scoresAlone.AddRow("Score", nameof(CaptiveQueensSolitaireMainViewModel.Score));
            scoresAlone.AddRow("First Start Number", nameof(CaptiveQueensSolitaireMainViewModel.FirstNumber));
            scoresAlone.AddRow("Second Start Number", nameof(CaptiveQueensSolitaireMainViewModel.SecondNumber));
            var tempGrid = scoresAlone.GetContent;
            _deckGPile.Margin = new Thickness(5, 5, 5, 5);
            _discardGPile.Margin = new Thickness(5, 5, 5, 5);
            stack.Children.Add(tempGrid);

            StackLayout finalStack = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal
            };
            Content = finalStack;
            finalStack.Children.Add(stack);
            finalStack.Children.Add(_main);
        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {
            _main.Init();

            return Task.CompletedTask;
        }


        object IUIView.DataContext
        {
            get => BindingContext;
            set => BindingContext = value;
        }
        Task IUIView.TryActivateAsync()
        {
            CaptiveQueensSolitaireMainViewModel model = (CaptiveQueensSolitaireMainViewModel)BindingContext;
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
