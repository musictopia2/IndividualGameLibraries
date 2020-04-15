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
using PokerCP.Data;
using static BasicXFControlsAndPages.Helpers.GridHelper; //since i use the grid a lot too.
using BasicGamingUIXFLibrary.BasicControls.SingleCardFrames;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;
using BasicGamingUIXFLibrary.GameGraphics.Cards;
using PokerCP.ViewModels;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicXFControlsAndPages.Converters;

namespace PokerXF.Views
{
    public class PokerMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        //TODO:  has to build the main page.  no need for new game since that is completely separated out into another class.
        private readonly IEventAggregator _aggregator;

        private readonly BaseDeckXF<PokerCardInfo, ts, DeckOfCardsXF<PokerCardInfo>> _deckGPile;
        private readonly BetUI _bet;
        private readonly HandUI _hand;
        public PokerMainView(IEventAggregator aggregator)
        {
            _aggregator = aggregator;
            _aggregator.Subscribe(this);
            _deckGPile = new BaseDeckXF<PokerCardInfo, ts, DeckOfCardsXF<PokerCardInfo>>();
            StackLayout stack = new StackLayout();
            stack.Children.Add(_deckGPile);

            _deckGPile.Margin = new Thickness(5, 5, 5, 5);
            _deckGPile.HorizontalOptions = LayoutOptions.Start;
            _deckGPile.VerticalOptions = LayoutOptions.Start;

            StackLayout otherStack = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal
            };

            otherStack.Children.Add(_deckGPile);
            _bet = new BetUI();
            _bet.Margin = new Thickness(40, 5, 10, 10);
            otherStack.Children.Add(_bet);
            var tempButton = GetSmallerButton("New Round", nameof(PokerMainViewModel.NewRoundAsync)); //maybe smallerbutton does not work

            //MakeGameButtonSmaller(tempButton);
            tempButton.HorizontalOptions = LayoutOptions.Start;
            tempButton.VerticalOptions = LayoutOptions.Start;
            otherStack.Children.Add(tempButton);
            SimpleLabelGridXF thisLabel = new SimpleLabelGridXF();
            CurrencyConverter thisConvert = new CurrencyConverter();
            thisLabel.AddRow("Money", nameof(PokerMainViewModel.Money), thisConvert);
            thisLabel.AddRow("Round", nameof(PokerMainViewModel.Round));
            thisLabel.AddRow("Winnings", nameof(PokerMainViewModel.Winnings), thisConvert);
            thisLabel.AddRow("Hand", nameof(PokerMainViewModel.HandLabel));
            stack.Children.Add(otherStack);
            stack.Children.Add(thisLabel.GetContent);

            _hand = new HandUI(_aggregator);
            _hand.Margin = new Thickness(5);
            stack.Children.Add(_hand);


            Content = stack; //if not doing this, rethink.

        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {
            PokerMainViewModel model = (PokerMainViewModel)BindingContext;
            _hand.Init(model);
            _bet.Init(model);
            return Task.CompletedTask;
        }


        object IUIView.DataContext
        {
            get => BindingContext;
            set => BindingContext = value;
        }
        Task IUIView.TryActivateAsync()
        {
            PokerMainViewModel model = (PokerMainViewModel)BindingContext;
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
