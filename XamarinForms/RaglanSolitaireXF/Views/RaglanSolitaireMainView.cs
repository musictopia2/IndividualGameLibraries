using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.SolitaireClasses.Cards;
using BasicGameFrameworkLibrary.SolitaireClasses.PileObservable;
using BasicGamingUIXFLibrary.BasicControls.GameFrames;
using BasicGamingUIXFLibrary.BasicControls.MultipleFrameContainers;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicGamingUIXFLibrary.BasicControls.SolitaireClasses;
using BasicGamingUIXFLibrary.GameGraphics.Cards;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using RaglanSolitaireCP.Logic;
using RaglanSolitaireCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;

namespace RaglanSolitaireXF.Views
{
    public class RaglanSolitaireMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        //TODO:  has to build the main page.  no need for new game since that is completely separated out into another class.
        private readonly IEventAggregator _aggregator;

        private readonly BaseHandXF<SolitaireCard, ts, DeckOfCardsXF<SolitaireCard>> _stock = new BaseHandXF<SolitaireCard, ts, DeckOfCardsXF<SolitaireCard>>();
        private readonly BasicMultiplePilesXF<SolitaireCard, ts, DeckOfCardsXF<SolitaireCard>> _main; //if something change here.
        private readonly SolitairePilesXF _waste;
        public RaglanSolitaireMainView(IEventAggregator aggregator)
        {
            _aggregator = aggregator;
            _aggregator.Subscribe(this);

            StackLayout stack = new StackLayout();
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            _main = new BasicMultiplePilesXF<SolitaireCard, ts, DeckOfCardsXF<SolitaireCard>>();
            _main.Margin = new Thickness(10, 5, 5, 5);
            stack.Children.Add(otherStack);
            var autoBut = GetSmallerButton("Auto Make Move", nameof(RaglanSolitaireMainViewModel.AutoMoveAsync));
            var scoresAlone = new SimpleLabelGridXF();
            scoresAlone.AddRow("Score", nameof(RaglanSolitaireMainViewModel.Score));
            var tempGrid = scoresAlone.GetContent;
            _waste = new SolitairePilesXF();
            otherStack.Children.Add(_main);
            otherStack.Children.Add(_stock);
            otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            StackLayout finalStack = new StackLayout();
            otherStack.Children.Add(finalStack);
            stack.Children.Add(otherStack);
            otherStack.Children.Add(_waste);
            finalStack.Children.Add(tempGrid);
            finalStack.Children.Add(autoBut);

            Content = stack; //if not doing this, rethink.

        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {
            RaglanSolitaireMainViewModel model = (RaglanSolitaireMainViewModel)BindingContext;
            var tempWaste = (WastePiles)model.WastePiles1!;
            _waste.Init(tempWaste.Piles);
            var tempMain = (MainPilesCP)model.MainPiles1!;
            _main.Init(tempMain.Piles, ts.TagUsed);
            _stock.LoadList(model.Stock1!, ts.TagUsed);
            return Task.CompletedTask;
        }


        object IUIView.DataContext
        {
            get => BindingContext;
            set => BindingContext = value;
        }
        Task IUIView.TryActivateAsync()
        {
            return Task.CompletedTask;
        }

        Task IUIView.TryCloseAsync()
        {
            _aggregator.Unsubscribe(this); //looks like you have to unsubscribe each time. now.
            return Task.CompletedTask;
        }
    }
}
