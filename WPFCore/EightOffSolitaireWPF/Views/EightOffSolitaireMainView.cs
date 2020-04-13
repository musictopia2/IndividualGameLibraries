using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.SolitaireClasses.Cards;
using BasicGameFrameworkLibrary.SolitaireClasses.PileObservable;
using BasicGamingUIWPFLibrary.BasicControls.GameFrames;
using BasicGamingUIWPFLibrary.BasicControls.MultipleFrameContainers;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using BasicGamingUIWPFLibrary.BasicControls.SolitaireClasses;
using BasicGamingUIWPFLibrary.GameGraphics.Cards;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using EightOffSolitaireCP.Logic;
using EightOffSolitaireCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows;
using System.Windows.Controls;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;

namespace EightOffSolitaireWPF.Views
{
    public class EightOffSolitaireMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;

        private readonly BasicMultiplePilesWPF<SolitaireCard, ts, DeckOfCardsWPF<SolitaireCard>> _main; //if something change here.
        private readonly SolitairePilesWPF _waste;
        private readonly BaseHandWPF<SolitaireCard, ts, DeckOfCardsWPF<SolitaireCard>> _reserve = new BaseHandWPF<SolitaireCard, ts, DeckOfCardsWPF<SolitaireCard>>();
        public EightOffSolitaireMainView(IEventAggregator aggregator)
        {
            _aggregator = aggregator;
            _aggregator.Subscribe(this);


            StackPanel stack = new StackPanel();

            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            _main = new BasicMultiplePilesWPF<SolitaireCard, ts, DeckOfCardsWPF<SolitaireCard>>();
            _main.Margin = new Thickness(10, 5, 5, 5);
            var autoBut = GetGamingButton("Auto Make Move", nameof(EightOffSolitaireMainViewModel.AutoMoveAsync));
            var scoresAlone = new SimpleLabelGrid();
            scoresAlone.AddRow("Score", nameof(EightOffSolitaireMainViewModel.Score));
            var tempGrid = scoresAlone.GetContent;
            _waste = new SolitairePilesWPF();


            var reserveButton = GetGamingButton("Reserve Card", nameof(EightOffSolitaireMainViewModel.AddToReserveAsync));
            otherStack.Children.Add(_main);
            otherStack.Children.Add(_reserve);
            stack.Children.Add(otherStack);
            otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Margin = new Thickness(0, 5, 0, 0);
            stack.Children.Add(otherStack);

            StackPanel finalStack = new StackPanel();
            otherStack.Children.Add(_waste);
            otherStack.Children.Add(finalStack);
            finalStack.Children.Add(reserveButton);
            finalStack.Children.Add(autoBut);
            finalStack.Children.Add(tempGrid);
            Content = stack; //if not doing this, rethink.
        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {

            EightOffSolitaireMainViewModel model = (EightOffSolitaireMainViewModel)DataContext;
            var tempWaste = (WastePiles)model.WastePiles1!;
            _waste.Init(tempWaste.Piles);
            var tempMain = (MainPilesCP)model.MainPiles1!;
            _main.Init(tempMain.Piles, ts.TagUsed);
            _reserve.LoadList(model.ReservePiles1, ts.TagUsed);
            return Task.CompletedTask;
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
