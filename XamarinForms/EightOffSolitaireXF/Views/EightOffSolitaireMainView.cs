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
using EightOffSolitaireCP.Logic;
using EightOffSolitaireCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;

namespace EightOffSolitaireXF.Views
{
    public class EightOffSolitaireMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;

        private readonly BasicMultiplePilesXF<SolitaireCard, ts, DeckOfCardsXF<SolitaireCard>> _main; //if something change here.
        private readonly SolitairePilesXF _waste;
        private readonly BaseHandXF<SolitaireCard, ts, DeckOfCardsXF<SolitaireCard>> _reserve = new BaseHandXF<SolitaireCard, ts, DeckOfCardsXF<SolitaireCard>>();

        public EightOffSolitaireMainView(IEventAggregator aggregator)
        {
            _aggregator = aggregator;
            _aggregator.Subscribe(this);

            StackLayout stack = new StackLayout();
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            _main = new BasicMultiplePilesXF<SolitaireCard, ts, DeckOfCardsXF<SolitaireCard>>();
            _main.Margin = new Thickness(10, 5, 5, 5);
            stack.Children.Add(otherStack);
            var autoBut = GetGamingButton("Auto Make Move", nameof(EightOffSolitaireMainViewModel.AutoMoveAsync));
            var scoresAlone = new SimpleLabelGridXF();
            scoresAlone.AddRow("Score", nameof(EightOffSolitaireMainViewModel.Score));
            var tempGrid = scoresAlone.GetContent;
            _waste = new SolitairePilesXF();

            var reserveButton = GetGamingButton("Reserve Card", nameof(EightOffSolitaireMainViewModel.AddToReserveAsync));
            otherStack.Children.Add(_main);
            otherStack.Children.Add(_reserve);

            otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Margin = new Thickness(0, 5, 0, 0);
            stack.Children.Add(otherStack);

            StackLayout finalStack = new StackLayout();
            otherStack.Children.Add(_waste);
            otherStack.Children.Add(finalStack);
            finalStack.Children.Add(reserveButton);
            finalStack.Children.Add(autoBut);
            finalStack.Children.Add(tempGrid);


            Content = stack; //if not doing this, rethink.
        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {
            //todo:  most of the time needs this.  if in a case its not needed, then delete then.
            EightOffSolitaireMainViewModel model = (EightOffSolitaireMainViewModel)BindingContext;
            var tempWaste = (WastePiles)model.WastePiles1!;
            _waste.Init(tempWaste.Piles);
            var tempMain = (MainPilesCP)model.MainPiles1!;
            _main.Init(tempMain.Piles, ts.TagUsed);
            _reserve.LoadList(model.ReservePiles1, ts.TagUsed);
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
