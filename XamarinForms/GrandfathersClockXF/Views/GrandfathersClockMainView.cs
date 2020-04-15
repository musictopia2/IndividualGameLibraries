using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.SolitaireClasses.Cards;
using BasicGamingUIXFLibrary.BasicControls.MultipleFrameContainers;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicGamingUIXFLibrary.BasicControls.SolitaireClasses;
using BasicGamingUIXFLibrary.GameGraphics.Cards;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using GrandfathersClockCP.Logic;
using GrandfathersClockCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;

namespace GrandfathersClockXF.Views
{
    public class GrandfathersClockMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;

        private readonly BasicMultiplePilesXF<SolitaireCard, ts, DeckOfCardsXF<SolitaireCard>> _main; //if something change here.
        private readonly MainClockXF _clock;
        private readonly SolitairePilesXF _piles;
        public GrandfathersClockMainView(IEventAggregator aggregator)
        {
            _aggregator = aggregator;
            _aggregator.Subscribe(this);

            StackLayout stack = new StackLayout();
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            _main = new BasicMultiplePilesXF<SolitaireCard, ts, DeckOfCardsXF<SolitaireCard>>();
            _main.Margin = new Thickness(10, 5, 5, 5);
            var autoBut = GetSmallerButton("Auto Make Move", nameof(GrandfathersClockMainViewModel.AutoMoveAsync));
            autoBut.HorizontalOptions = LayoutOptions.Start;
            autoBut.VerticalOptions = LayoutOptions.Start;
            var scoresAlone = new SimpleLabelGridXF();
            scoresAlone.AddRow("Score", nameof(GrandfathersClockMainViewModel.Score));
            var tempGrid = scoresAlone.GetContent;
            var thisWaste = new SolitairePilesXF();
            _piles = thisWaste;
            var thisClock = new MainClockXF();
            thisClock.Margin = new Thickness(0, 60, 0, 0);
            _clock = thisClock;
            Grid thisGrid = new Grid();
            otherStack.Children.Add(thisGrid);
            otherStack.Children.Add(thisWaste);
            otherStack.Children.Add(tempGrid);
            stack.Children.Add(autoBut);
            thisGrid.Children.Add(stack);
            thisGrid.Children.Add(thisClock);
            Content = otherStack;

        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {
            GrandfathersClockMainViewModel model = (GrandfathersClockMainViewModel)BindingContext;
            var tempWaste = (WastePiles)model.WastePiles1!;
            _piles.Init(tempWaste.Piles);
            var tempMain = (CustomMain)model.MainPiles1!;
            _clock.LoadControls(tempMain);
            return this.RefreshBindingsAsync(_aggregator);
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
