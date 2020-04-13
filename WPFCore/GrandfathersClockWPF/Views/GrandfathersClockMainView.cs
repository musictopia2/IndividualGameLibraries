using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.SolitaireClasses.Cards;
using BasicGamingUIWPFLibrary.BasicControls.MultipleFrameContainers;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using BasicGamingUIWPFLibrary.BasicControls.SolitaireClasses;
using BasicGamingUIWPFLibrary.GameGraphics.Cards;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using GrandfathersClockCP.Logic;
using GrandfathersClockCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows;
using System.Windows.Controls;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;

namespace GrandfathersClockWPF.Views
{
    public class GrandfathersClockMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;

        private readonly BasicMultiplePilesWPF<SolitaireCard, ts, DeckOfCardsWPF<SolitaireCard>> _main; //if something change here.
        private readonly MainClockWPF _clock;
        private readonly SolitairePilesWPF _piles;
        public GrandfathersClockMainView(IEventAggregator aggregator)
        {
            _aggregator = aggregator;
            _aggregator.Subscribe(this);


            StackPanel stack = new StackPanel();

            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            _main = new BasicMultiplePilesWPF<SolitaireCard, ts, DeckOfCardsWPF<SolitaireCard>>();
            _main.Margin = new Thickness(10, 5, 5, 5);
            var autoBut = GetGamingButton("Auto Make Move", nameof(GrandfathersClockMainViewModel.AutoMoveAsync));
            autoBut.HorizontalAlignment = HorizontalAlignment.Left;
            autoBut.VerticalAlignment = VerticalAlignment.Top;

            var scoresAlone = new SimpleLabelGrid();
            scoresAlone.AddRow("Score", nameof(GrandfathersClockMainViewModel.Score));
            var tempGrid = scoresAlone.GetContent;
            var thisWaste = new SolitairePilesWPF();
            _piles = thisWaste;
            var thisClock = new MainClockWPF();
            _clock = thisClock;
            Grid thisGrid = new Grid();
            thisGrid.Width = 800;
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
            GrandfathersClockMainViewModel model = (GrandfathersClockMainViewModel)DataContext;
            var tempWaste = (WastePiles)model.WastePiles1!;
            _piles.Init(tempWaste.Piles);
            var tempMain = (CustomMain)model.MainPiles1!;
            _clock.LoadControls(tempMain);
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
