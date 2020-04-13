using BakersDozenSolitaireCP.Logic;
using BakersDozenSolitaireCP.ViewModels;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.SolitaireClasses.Cards;
using BasicGameFrameworkLibrary.SolitaireClasses.PileObservable;
using BasicGamingUIWPFLibrary.BasicControls.MultipleFrameContainers;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using BasicGamingUIWPFLibrary.BasicControls.SolitaireClasses;
using BasicGamingUIWPFLibrary.GameGraphics.Cards;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows;
using System.Windows.Controls;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;

namespace BakersDozenSolitaireWPF.Views
{
    public class BakersDozenSolitaireMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        //TODO:  has to build the main page.  no need for new game since that is completely separated out into another class.
        private readonly IEventAggregator _aggregator;

        private readonly BasicMultiplePilesWPF<SolitaireCard, ts, DeckOfCardsWPF<SolitaireCard>> _main; //if something change here.
        private readonly SolitairePilesWPF _waste;
        public BakersDozenSolitaireMainView(IEventAggregator aggregator)
        {
            _aggregator = aggregator;
            _aggregator.Subscribe(this);


            StackPanel stack = new StackPanel();

            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            _main = new BasicMultiplePilesWPF<SolitaireCard, ts, DeckOfCardsWPF<SolitaireCard>>();
            _main.Margin = new Thickness(10, 5, 5, 5);
            stack.Children.Add(otherStack);

            var autoBut = GetGamingButton("Auto Make Move", nameof(BakersDozenSolitaireMainViewModel.AutoMoveAsync));
            autoBut.HorizontalAlignment = HorizontalAlignment.Left;

            //not sure where to place it.
            //it probably varies from game to game.
            var scoresAlone = new SimpleLabelGrid();
            scoresAlone.AddRow("Score", nameof(BakersDozenSolitaireMainViewModel.Score));
            var tempGrid = scoresAlone.GetContent;
            //not sure where to place.
            _waste = new SolitairePilesWPF();

            StackPanel finalStack = new StackPanel();
            otherStack.Children.Add(finalStack);
            otherStack.Children.Add(_main);
            finalStack.Children.Add(tempGrid);
            finalStack.Children.Add(autoBut);
            stack.Children.Add(_waste);

            //not sure where to place
            //needs to init.  however, needs a waste viewmodel to hook to.  the interface does not require to necessarily use it.
            //sometimes its more discard piles.
            var miscDiscard = new BasicMultiplePilesWPF<SolitaireCard, ts, DeckOfCardsWPF<SolitaireCard>>();
            //if this is being used, then needs to hook to a viewmodel that is associated with this one.


            Content = stack; //if not doing this, rethink.
        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {



            BakersDozenSolitaireMainViewModel model = (BakersDozenSolitaireMainViewModel)DataContext;
            var tempWaste = (WastePiles)model.WastePiles1!;
            _waste.Init(tempWaste.Piles);
            var tempMain = (MainPilesCP)model.MainPiles1!;
            _main.Init(tempMain.Piles, ts.TagUsed);

            return this.RefreshBindingsAsync(_aggregator);
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
