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
using TriangleSolitaireCP.Data;
using static BasicXFControlsAndPages.Helpers.GridHelper; //since i use the grid a lot too.
using BasicGamingUIXFLibrary.BasicControls.SingleCardFrames;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;
using BasicGamingUIXFLibrary.GameGraphics.Cards;
using TriangleSolitaireCP.ViewModels;
using BasicGameFrameworkLibrary.SolitaireClasses.Cards;
using BasicGamingUIXFLibrary.BasicControls.SolitaireClasses;

namespace TriangleSolitaireXF.Views
{
    public class TriangleSolitaireMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        //TODO:  has to build the main page.  no need for new game since that is completely separated out into another class.
        private readonly IEventAggregator _aggregator;

        private readonly BaseDeckXF<SolitaireCard, ts, DeckOfCardsXF<SolitaireCard>> _deckGPile;
        private readonly BasePileXF<SolitaireCard, ts, DeckOfCardsXF<SolitaireCard>> _discardGPile;
        private readonly TriangleXF _triangle;
        public TriangleSolitaireMainView(IEventAggregator aggregator)
        {
            _aggregator = aggregator;
            _aggregator.Subscribe(this);
            _deckGPile = new BaseDeckXF<SolitaireCard, ts, DeckOfCardsXF<SolitaireCard>>();
            _discardGPile = new BasePileXF<SolitaireCard, ts, DeckOfCardsXF<SolitaireCard>>();
            _triangle = new TriangleXF();
            StackLayout stack = new StackLayout();
            stack.Children.Add(_deckGPile);

            _deckGPile.Margin = new Thickness(5, 5, 5, 5);
            _deckGPile.HorizontalOptions = LayoutOptions.Start;
            _deckGPile.VerticalOptions = LayoutOptions.Start;

            _discardGPile.Margin = new Thickness(5);
            _discardGPile.HorizontalOptions = LayoutOptions.Start;
            _discardGPile.VerticalOptions = LayoutOptions.Start;
            StackLayout other = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal
            };
            other.Children.Add(_deckGPile);
            other.Children.Add(_discardGPile);
            other.Children.Add(_triangle);
            stack.Children.Add(other);


            Content = stack; //if not doing this, rethink.
        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {
            return Task.CompletedTask;
        }


        object IUIView.DataContext
        {
            get => BindingContext;
            set => BindingContext = value;
        }
        Task IUIView.TryActivateAsync()
        {
            TriangleSolitaireMainViewModel model = (TriangleSolitaireMainViewModel)BindingContext;
            _deckGPile.Init(model.DeckPile, ts.TagUsed);
            _discardGPile.Init(model.Pile1, ts.TagUsed);
            _triangle.Init(model.Triangle1);
            return Task.CompletedTask;
        }

        Task IUIView.TryCloseAsync()
        {
            _aggregator.Unsubscribe(this); //looks like you have to unsubscribe each time. now.
            return Task.CompletedTask;
        }
    }
}
