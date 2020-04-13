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
using AgnesSolitaireCP.Data;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //since i use the grid a lot too.
using BasicGamingUIWPFLibrary.BasicControls.SingleCardFrames;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;
using BasicGamingUIWPFLibrary.GameGraphics.Cards;
using System.Windows;
using AgnesSolitaireCP.ViewModels;
using BasicGameFrameworkLibrary.SolitaireClasses.Cards;
using BasicGamingUIWPFLibrary.BasicControls.MultipleFrameContainers;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using BasicGamingUIWPFLibrary.BasicControls.SolitaireClasses;
using AgnesSolitaireCP.Logic;
using BasicGameFrameworkLibrary.SolitaireClasses.PileObservable;

namespace AgnesSolitaireWPF.Views
{
    public class AgnesSolitaireMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;

        private readonly BaseDeckWPF<SolitaireCard, ts, DeckOfCardsWPF<SolitaireCard>> _deckGPile;
        private readonly BasicMultiplePilesWPF<SolitaireCard, ts, DeckOfCardsWPF<SolitaireCard>> _main; //if something change here.
        private readonly SolitairePilesWPF _waste;
        public AgnesSolitaireMainView(IEventAggregator aggregator)
        {
            _aggregator = aggregator;
			_aggregator.Subscribe(this);
            _deckGPile = new BaseDeckWPF<SolitaireCard, ts, DeckOfCardsWPF<SolitaireCard>>();
            _deckGPile.Margin = new Thickness(5, 5, 5, 5);
            _deckGPile.HorizontalAlignment = HorizontalAlignment.Left;
            _deckGPile.VerticalAlignment = VerticalAlignment.Top;


            StackPanel stack = new StackPanel();

            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            _main = new BasicMultiplePilesWPF<SolitaireCard, ts, DeckOfCardsWPF<SolitaireCard>>();
            _main.Margin = new Thickness(10, 5, 5, 5);
            stack.Children.Add(otherStack);
            otherStack.Children.Add(_main);

            var autoBut = GetGamingButton("Auto Make Move", nameof(AgnesSolitaireMainViewModel.AutoMoveAsync));
            autoBut.HorizontalAlignment = HorizontalAlignment.Left;

            //not sure where to place it.
            //it probably varies from game to game.
            var scoresAlone = new SimpleLabelGrid();
            scoresAlone.AddRow("Score", nameof(AgnesSolitaireMainViewModel.Score));
            scoresAlone.AddRow("Starting Number", nameof(AgnesSolitaireMainViewModel.StartingNumber));
            var tempGrid = scoresAlone.GetContent;

            //not sure where to place.
            _waste = new SolitairePilesWPF();
            //not sure where to place
            //needs to init.  however, needs a waste viewmodel to hook to.  the interface does not require to necessarily use it.
            //sometimes its more discard piles.
            StackPanel tempStack = new StackPanel();
            otherStack.Children.Add(tempStack);
            tempStack.Children.Add(tempGrid);
            tempStack.Children.Add(autoBut);
            stack.Children.Add(_waste);

            Content = stack; //if not doing this, rethink.
        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {
            AgnesSolitaireMainViewModel model = (AgnesSolitaireMainViewModel)DataContext;
            var tempWaste = (WastePiles)model.WastePiles1!;
            _waste.Init(tempWaste.Piles);
            var tempMain = (MainPilesCP)model.MainPiles1!;
            _main.Init(tempMain.Piles, ts.TagUsed);
            return Task.CompletedTask;
        }

        

        Task IUIView.TryActivateAsync()
        {
            AgnesSolitaireMainViewModel model = (AgnesSolitaireMainViewModel)DataContext;
            _deckGPile.Init(model.DeckPile, ts.TagUsed);

            return Task.CompletedTask;
        }

        Task IUIView.TryCloseAsync()
        {
            _aggregator.Unsubscribe(this); //looks like you have to unsubscribe each time. now.
            return Task.CompletedTask;
        }
    }
}
