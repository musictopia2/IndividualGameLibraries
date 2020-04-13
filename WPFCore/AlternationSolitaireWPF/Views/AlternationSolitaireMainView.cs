using System;
using System.Text;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using System.Linq;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using fs = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.FileHelpers;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
using System.Windows.Controls;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using CommonBasicStandardLibraries.Messenging;
using BasicGameFrameworkLibrary.BasicEventModels;
using AlternationSolitaireCP.Data;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //since i use the grid a lot too.
using BasicGamingUIWPFLibrary.BasicControls.SingleCardFrames;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;
using BasicGamingUIWPFLibrary.GameGraphics.Cards;
using System.Windows;
using AlternationSolitaireCP.ViewModels;
using BasicGameFrameworkLibrary.SolitaireClasses.Cards;
using BasicGamingUIWPFLibrary.BasicControls.MultipleFrameContainers;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using BasicGamingUIWPFLibrary.BasicControls.SolitaireClasses;
using AlternationSolitaireCP.Logic;
using BasicGameFrameworkLibrary.SolitaireClasses.PileObservable;

namespace AlternationSolitaireWPF.Views
{
    public class AlternationSolitaireMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        //TODO:  has to build the main page.  no need for new game since that is completely separated out into another class.
        private readonly IEventAggregator _aggregator;

        private readonly BaseDeckWPF<SolitaireCard, ts, DeckOfCardsWPF<SolitaireCard>> _deckGPile;
        private readonly BasePileWPF<SolitaireCard, ts, DeckOfCardsWPF<SolitaireCard>> _discardGPile;
        private readonly BasicMultiplePilesWPF<SolitaireCard, ts, DeckOfCardsWPF<SolitaireCard>> _main; //if something change here.
        private readonly SolitairePilesWPF _waste;
        public AlternationSolitaireMainView(IEventAggregator aggregator)
        {
            _aggregator = aggregator;
			_aggregator.Subscribe(this);
            _deckGPile = new BaseDeckWPF<SolitaireCard, ts, DeckOfCardsWPF<SolitaireCard>>();
            _deckGPile.Margin = new Thickness(5, 5, 5, 5);
            _deckGPile.HorizontalAlignment = HorizontalAlignment.Left;
            _deckGPile.VerticalAlignment = VerticalAlignment.Top;


            _discardGPile = new BasePileWPF<SolitaireCard, ts, DeckOfCardsWPF<SolitaireCard>>();
            _discardGPile.Margin = new Thickness(5);
            _discardGPile.HorizontalAlignment = HorizontalAlignment.Left;
            _discardGPile.VerticalAlignment = VerticalAlignment.Top;


            StackPanel stack = new StackPanel();

            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile);
            _main = new BasicMultiplePilesWPF<SolitaireCard, ts, DeckOfCardsWPF<SolitaireCard>>();
            _main.Margin = new Thickness(100, 5, 5, 5);
            stack.Children.Add(otherStack);
            var autoBut = GetGamingButton("Auto Make Move", nameof(AlternationSolitaireMainViewModel.AutoMoveAsync));
            //not sure where to place it.
            //it probably varies from game to game.
            var scoresAlone = new SimpleLabelGrid();
            scoresAlone.AddRow("Score", nameof(AlternationSolitaireMainViewModel.Score));
            var tempGrid = scoresAlone.GetContent;
            //not sure where to place.
            _waste = new SolitairePilesWPF();

            StackPanel tempStack = new StackPanel();
            otherStack.Children.Add(tempStack);
            tempStack.Children.Add(tempGrid);
            tempStack.Children.Add(autoBut);
            otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            stack.Children.Add(otherStack);
            otherStack.Children.Add(_waste);
            otherStack.Children.Add(_main);


            Content = stack; //if not doing this, rethink.
        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {



            AlternationSolitaireSaveInfo thisSave = cons!.Resolve<AlternationSolitaireSaveInfo>();
            //todo:  most of the time needs this.  if in a case its not needed, then delete then.
            AlternationSolitaireMainViewModel model = (AlternationSolitaireMainViewModel)DataContext;
            var tempWaste = (WastePiles)model.WastePiles1!;
            _waste.Init(tempWaste.Piles);
            var tempMain = (MainPilesCP)model.MainPiles1!;
            _main.Init(tempMain.Piles, ts.TagUsed);

            return this.RefreshBindingsAsync(_aggregator);
        }

        

        Task IUIView.TryActivateAsync()
        {
            AlternationSolitaireMainViewModel model = (AlternationSolitaireMainViewModel)DataContext;
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
