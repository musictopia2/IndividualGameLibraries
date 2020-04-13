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
using CarpetSolitaireCP.Data;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //since i use the grid a lot too.
using BasicGamingUIWPFLibrary.BasicControls.SingleCardFrames;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;
using BasicGamingUIWPFLibrary.GameGraphics.Cards;
using System.Windows;
using CarpetSolitaireCP.ViewModels;
using BasicGameFrameworkLibrary.SolitaireClasses.Cards;
using BasicGamingUIWPFLibrary.BasicControls.MultipleFrameContainers;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using BasicGamingUIWPFLibrary.BasicControls.SolitaireClasses;
using CarpetSolitaireCP.Logic;
using BasicGameFrameworkLibrary.SolitaireClasses.PileObservable;

namespace CarpetSolitaireWPF.Views
{
    public class CarpetSolitaireMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        //TODO:  has to build the main page.  no need for new game since that is completely separated out into another class.
        private readonly IEventAggregator _aggregator;

        private readonly BaseDeckWPF<SolitaireCard, ts, DeckOfCardsWPF<SolitaireCard>> _deckGPile;
        private readonly BasePileWPF<SolitaireCard, ts, DeckOfCardsWPF<SolitaireCard>> _discardGPile;
        private readonly BasicMultiplePilesWPF<SolitaireCard, ts, DeckOfCardsWPF<SolitaireCard>> _main; //if something change here.
        private readonly BasicMultiplePilesWPF<SolitaireCard, ts, DeckOfCardsWPF<SolitaireCard>>  _discard = new BasicMultiplePilesWPF<SolitaireCard, ts, DeckOfCardsWPF<SolitaireCard>>();
        public CarpetSolitaireMainView(IEventAggregator aggregator)
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
            _main = new BasicMultiplePilesWPF<SolitaireCard, ts, DeckOfCardsWPF<SolitaireCard>>();
            _main.Margin = new Thickness(10, 5, 5, 5);
            stack.Children.Add(otherStack);
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile);
            var autoBut = GetGamingButton("Auto Make Move", nameof(CarpetSolitaireMainViewModel.AutoMoveAsync));
            //not sure where to place it.
            //it probably varies from game to game.
            var scoresAlone = new SimpleLabelGrid();
            scoresAlone.AddRow("Score", nameof(CarpetSolitaireMainViewModel.Score));
            var tempGrid = scoresAlone.GetContent;
            
            _deckGPile.Margin = new Thickness(5, 5, 5, 5);
            _discardGPile.Margin = new Thickness(5, 5, 5, 5);
            Grid grid = new Grid();
            AddAutoColumns(grid, 2);
            stack.Children.Add(tempGrid);
            stack.Children.Add(autoBut);
            AddControlToGrid(grid, stack, 0, 0);
            stack = new StackPanel();
            stack.Children.Add(_main);
            stack.Children.Add(_discard);
            AddControlToGrid(grid, stack, 0, 1);
            Content = grid;
        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {



            //todo:  most of the time needs this.  if in a case its not needed, then delete then.
            CarpetSolitaireMainViewModel model = (CarpetSolitaireMainViewModel)DataContext;
            var tempWaste = (WastePiles)model.WastePiles1!;
            var tempMain = (MainPilesCP)model.MainPiles1!;
            _main.Init(tempMain.Piles, ts.TagUsed);

            _discard.Init(tempWaste.Discards!, ts.TagUsed);

            return Task.CompletedTask;
        }

        

        Task IUIView.TryActivateAsync()
        {
            CarpetSolitaireMainViewModel model = (CarpetSolitaireMainViewModel)DataContext;
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
