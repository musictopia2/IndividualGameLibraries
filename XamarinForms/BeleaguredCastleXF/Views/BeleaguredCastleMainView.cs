using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.SolitaireClasses.Cards;
using BasicGameFrameworkLibrary.SolitaireClasses.PileObservable;
using BasicGamingUIXFLibrary.BasicControls.MultipleFrameContainers;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicGamingUIXFLibrary.GameGraphics.Cards;
using BeleaguredCastleCP.Logic;
using BeleaguredCastleCP.ViewModels;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;

namespace BeleaguredCastleXF.Views
{
    public class BeleaguredCastleMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;

        private readonly BasicMultiplePilesXF<SolitaireCard, ts, DeckOfCardsXF<SolitaireCard>> _main; //if something change here.
        private readonly CustomWasteUI _waste;
        public BeleaguredCastleMainView(IEventAggregator aggregator)
        {
            _aggregator = aggregator;
            _aggregator.Subscribe(this);

            StackLayout stack = new StackLayout();
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            _main = new BasicMultiplePilesXF<SolitaireCard, ts, DeckOfCardsXF<SolitaireCard>>();
            _main.Margin = new Thickness(10, 5, 5, 5);
            var autoBut = GetGamingButton("Auto Make Move", nameof(BeleaguredCastleMainViewModel.AutoMoveAsync));
            autoBut.HorizontalOptions = LayoutOptions.Center;
            autoBut.VerticalOptions = LayoutOptions.Center;
            var scoresAlone = new SimpleLabelGridXF();
            scoresAlone.AddRow("Score", nameof(BeleaguredCastleMainViewModel.Score));
            var tempGrid = scoresAlone.GetContent;
            _waste = new CustomWasteUI();
            stack.Children.Add(_waste);
            stack.Children.Add(otherStack);

            otherStack.Children.Add(tempGrid);
            otherStack.Children.Add(autoBut);
            Content = stack; //if not doing this, rethink.
        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {
            BeleaguredCastleMainViewModel model = (BeleaguredCastleMainViewModel)BindingContext;
            var tempMain = (MainPilesCP)model.MainPiles1!;
            _main.Init(tempMain.Piles, ts.TagUsed);
            var tempWaste = (WastePiles)model.WastePiles1!;
            _waste.Init(tempWaste.Piles, _main);
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