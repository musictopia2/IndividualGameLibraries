using BasicControlsAndWindowsCore.BasicWindows.BasicConverters;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using BasicGamingUIWPFLibrary.BasicControls.SingleCardFrames;
using BasicGamingUIWPFLibrary.GameGraphics.Cards;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using PokerCP.Data;
using PokerCP.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows;
using System.Windows.Controls;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;

namespace PokerWPF.Views
{
    public class PokerMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        //TODO:  has to build the main page.  no need for new game since that is completely separated out into another class.
        private readonly IEventAggregator _aggregator;

        private readonly BaseDeckWPF<PokerCardInfo, ts, DeckOfCardsWPF<PokerCardInfo>> _deckGPile;
        private readonly BetUI _bet;
        private readonly HandUI _hand;
        public PokerMainView(IEventAggregator aggregator)
        {
            _aggregator = aggregator;
            _aggregator.Subscribe(this);
            _deckGPile = new BaseDeckWPF<PokerCardInfo, ts, DeckOfCardsWPF<PokerCardInfo>>();

            StackPanel stack = new StackPanel();

            _deckGPile.Margin = new Thickness(5, 5, 5, 5);
            _deckGPile.HorizontalAlignment = HorizontalAlignment.Left;
            _deckGPile.VerticalAlignment = VerticalAlignment.Top;
            StackPanel otherStack = new StackPanel()
            {
                Orientation = Orientation.Horizontal
            };

            otherStack.Children.Add(_deckGPile);
            _bet = new BetUI();
            _bet.Margin = new Thickness(60, 5, 10, 10);
            otherStack.Children.Add(_bet);
            var tempButton = GetGamingButton("New Round", nameof(PokerMainViewModel.NewRoundAsync));
            tempButton.HorizontalAlignment = HorizontalAlignment.Left;
            tempButton.VerticalAlignment = VerticalAlignment.Center;
            otherStack.Children.Add(tempButton);
            SimpleLabelGrid thisLabel = new SimpleLabelGrid();
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
            PokerMainViewModel model = (PokerMainViewModel)DataContext;
            _hand.Init(model);
            _bet.Init(model);
            return Task.CompletedTask;
            //return this.RefreshBindingsAsync(_aggregator);
        }



        Task IUIView.TryActivateAsync()
        {
            PokerMainViewModel model = (PokerMainViewModel)DataContext;
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
