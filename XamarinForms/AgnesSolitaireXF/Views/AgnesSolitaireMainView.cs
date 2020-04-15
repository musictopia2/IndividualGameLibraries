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
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using CommonBasicStandardLibraries.Messenging;
using BasicGameFrameworkLibrary.BasicEventModels;
using AgnesSolitaireCP.Data;
using static BasicXFControlsAndPages.Helpers.GridHelper; //since i use the grid a lot too.
using BasicGamingUIXFLibrary.BasicControls.SingleCardFrames;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;
using BasicGamingUIXFLibrary.GameGraphics.Cards;
using AgnesSolitaireCP.ViewModels;
using BasicGameFrameworkLibrary.SolitaireClasses.Cards;
using BasicGamingUIXFLibrary.BasicControls.MultipleFrameContainers;
using BasicGamingUIXFLibrary.BasicControls.SolitaireClasses;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using AgnesSolitaireCP.Logic;
using BasicGameFrameworkLibrary.SolitaireClasses.PileObservable;

namespace AgnesSolitaireXF.Views
{
    public class AgnesSolitaireMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        //TODO:  has to build the main page.  no need for new game since that is completely separated out into another class.
        private readonly IEventAggregator _aggregator;

        private readonly BaseDeckXF<SolitaireCard, ts, DeckOfCardsXF<SolitaireCard>> _deckGPile;
        private readonly BasicMultiplePilesXF<SolitaireCard, ts, DeckOfCardsXF<SolitaireCard>> _main; //if something change here.
        private readonly SolitairePilesXF _waste;
        public AgnesSolitaireMainView(IEventAggregator aggregator)
        {
            _aggregator = aggregator;
            _aggregator.Subscribe(this);
            _deckGPile = new BaseDeckXF<SolitaireCard, ts, DeckOfCardsXF<SolitaireCard>>();

            _deckGPile.Margin = new Thickness(5, 5, 5, 5);
            _deckGPile.HorizontalOptions = LayoutOptions.Start;
            _deckGPile.VerticalOptions = LayoutOptions.Start;

            StackLayout stack = new StackLayout();
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            _main = new BasicMultiplePilesXF<SolitaireCard, ts, DeckOfCardsXF<SolitaireCard>>();
            _main.Margin = new Thickness(10, 5, 5, 5); 
            otherStack.Children.Add(_main);
            stack.Children.Add(otherStack);
            var autoBut = GetGamingButton("Auto Make Move", nameof(AgnesSolitaireMainViewModel.AutoMoveAsync));
            autoBut.HorizontalOptions = LayoutOptions.Start;
            //not sure where to place it.
            //it probably varies from game to game.
            var scoresAlone = new SimpleLabelGridXF();
            scoresAlone.AddRow("Score", nameof(AgnesSolitaireMainViewModel.Score));
            scoresAlone.AddRow("Starting Number", nameof(AgnesSolitaireMainViewModel.StartingNumber));
            var tempGrid = scoresAlone.GetContent;
            //not sure where to place.
            _waste = new SolitairePilesXF();

            StackLayout tempStack = new StackLayout();
            otherStack.Children.Add(tempStack);
            tempStack.Children.Add(tempGrid);
            tempStack.Children.Add(autoBut);
            stack.Children.Add(_waste);

            //not sure where to place
            //needs to init.  however, needs a waste viewmodel to hook to.  the interface does not require to necessarily use it.
            //sometimes its more discard piles.
            //if this is being used, then needs to hook to a viewmodel that is associated with this one.
            _deckGPile.Margin = new Thickness(5, 5, 5, 5);


            Content = stack; //if not doing this, rethink.
        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {
            AgnesSolitaireMainViewModel model = (AgnesSolitaireMainViewModel)BindingContext;
            var tempWaste = (WastePiles)model.WastePiles1!;
            _waste.Init(tempWaste.Piles);
            var tempMain = (MainPilesCP)model.MainPiles1!;
            _main.Init(tempMain.Piles, ts.TagUsed);
            return Task.CompletedTask;
        }


        object IUIView.DataContext
        {
            get => BindingContext;
            set => BindingContext = value;
        }
        Task IUIView.TryActivateAsync()
        {
            AgnesSolitaireMainViewModel model = (AgnesSolitaireMainViewModel)BindingContext;
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
