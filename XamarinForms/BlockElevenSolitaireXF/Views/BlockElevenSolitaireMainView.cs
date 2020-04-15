using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.SolitaireClasses.Cards;
using BasicGamingUIXFLibrary.BasicControls.MultipleFrameContainers;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicGamingUIXFLibrary.GameGraphics.Cards;
using BlockElevenSolitaireCP.Logic;
using BlockElevenSolitaireCP.ViewModels;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;

namespace BlockElevenSolitaireXF.Views
{
    public class BlockElevenSolitaireMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;

        private readonly BasicMultiplePilesXF<SolitaireCard, ts, DeckOfCardsXF<SolitaireCard>> _waste;
        public BlockElevenSolitaireMainView(IEventAggregator aggregator)
        {
            _aggregator = aggregator;
            _aggregator.Subscribe(this);

            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            var scoresAlone = new SimpleLabelGridXF();
            scoresAlone.AddRow("Score", nameof(BlockElevenSolitaireMainViewModel.Score));
            scoresAlone.AddRow("Cards Left", nameof(BlockElevenSolitaireMainViewModel.CardsLeft));
            var tempGrid = scoresAlone.GetContent;
            _waste = new BasicMultiplePilesXF<SolitaireCard, ts, DeckOfCardsXF<SolitaireCard>>();

            otherStack.Children.Add(_waste);
            otherStack.Children.Add(tempGrid);

            Content = otherStack; //if not doing this, rethink.
        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {
            BlockElevenSolitaireMainViewModel model = (BlockElevenSolitaireMainViewModel)BindingContext;
            var tempWaste = (WastePiles)model.WastePiles1!;
            _waste.Init(tempWaste.Discards!, ts.TagUsed);

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
