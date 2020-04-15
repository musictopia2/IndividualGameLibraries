using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGamingUIXFLibrary.BasicControls.GameFrames;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicGamingUIXFLibrary.BasicControls.SingleCardFrames;
using BasicGamingUIXFLibrary.GameGraphics.Cards;
using BlackjackCP.Data;
using BlackjackCP.ViewModels;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using static BasicXFControlsAndPages.Helpers.GridHelper; //since i use the grid a lot too.
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;

namespace BlackjackXF.Views
{
    public class BlackjackMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;

        private readonly BaseDeckXF<BlackjackCardInfo, ts, DeckOfCardsXF<BlackjackCardInfo>> _deckGPile;
        private readonly BaseHandXF<BlackjackCardInfo, ts, DeckOfCardsXF<BlackjackCardInfo>> _humanHand;
        private readonly BaseHandXF<BlackjackCardInfo, ts, DeckOfCardsXF<BlackjackCardInfo>> _computerHand;
        public BlackjackMainView(IEventAggregator aggregator)
        {
            _aggregator = aggregator;
            _aggregator.Subscribe(this);
            _deckGPile = new BaseDeckXF<BlackjackCardInfo, ts, DeckOfCardsXF<BlackjackCardInfo>>();
            StackLayout stack = new StackLayout();
            stack.Spacing = 0;
            stack.Children.Add(_deckGPile);

            _deckGPile.Margin = new Thickness(5, 5, 5, 5);
            _deckGPile.HorizontalOptions = LayoutOptions.Start;
            _deckGPile.VerticalOptions = LayoutOptions.Start;
            Grid grid = new Grid();
            AddLeftOverColumn(grid, 30);
            AddLeftOverColumn(grid, 30);
            AddAutoColumns(grid, 2);
            AddLeftOverRow(grid, 40);
            AddLeftOverRow(grid, 50);
            SimpleLabelGridXF otherGrid = new SimpleLabelGridXF();
            otherGrid.AddRow("Wins", nameof(BlackjackMainViewModel.Wins));
            otherGrid.AddRow("Losses", nameof(BlackjackMainViewModel.Losses));
            otherGrid.AddRow("Draws", nameof(BlackjackMainViewModel.Draws));
            stack.Children.Add(otherGrid.GetContent);

            otherGrid = new SimpleLabelGridXF();
            otherGrid.AddRow("Human Points", nameof(BlackjackMainViewModel.HumanPoints));
            otherGrid.AddRow("Computer Points", nameof(BlackjackMainViewModel.ComputerPoints));
            stack.Children.Add(otherGrid.GetContent);

            grid.Children.Add(stack);

            stack = new StackLayout();


            BaseHandXF<BlackjackCardInfo, ts, DeckOfCardsXF<BlackjackCardInfo>> humanHand = new BaseHandXF<BlackjackCardInfo, ts, DeckOfCardsXF<BlackjackCardInfo>>();
            BaseHandXF<BlackjackCardInfo, ts, DeckOfCardsXF<BlackjackCardInfo>> computerHand = new BaseHandXF<BlackjackCardInfo, ts, DeckOfCardsXF<BlackjackCardInfo>>();
            humanHand.HandType = HandObservable<BlackjackCardInfo>.EnumHandList.Horizontal;
            computerHand.HandType = HandObservable<BlackjackCardInfo>.EnumHandList.Horizontal;
            humanHand.Margin = new Thickness(3, 3, 3, 10);
            computerHand.Margin = new Thickness(3, 3, 3, 10);
            _humanHand = humanHand;
            _computerHand = computerHand;

            AddControlToGrid(grid, stack, 1, 0);
            Grid.SetColumnSpan(stack, 2);

            var thisBut = GetGamingButton("Hit Me", nameof(BlackjackMainViewModel.HitAsync)); // hopefully margins set automatically (well find out)

            stack.Children.Add(thisBut);
            thisBut = GetGamingButton("Stay", nameof(BlackjackMainViewModel.StayAsync));
            stack.Children.Add(thisBut);
            StackLayout finalStack = new StackLayout();
            thisBut = GetGamingButton("Use Ace As One (1)", nameof(BlackjackMainViewModel.AceAsync));
            thisBut.CommandParameter = BlackjackMainViewModel.EnumAceChoice.Low;
            finalStack.Children.Add(thisBut);
            var thisBind = new Binding(nameof(BlackjackMainViewModel.NeedsAceChoice));
            finalStack.SetBinding(IsVisibleProperty, thisBind); // since its going to this stack, will make all based on this one.

            thisBut = GetGamingButton("Use Ace As Eleven (11)", nameof(BlackjackMainViewModel.AceAsync));
            thisBut.CommandParameter = BlackjackMainViewModel.EnumAceChoice.High;
            finalStack.Children.Add(thisBut);
            stack.Children.Add(finalStack);
            stack = new StackLayout();

            stack.Children.Add(computerHand);
            stack.Children.Add(humanHand);

            AddControlToGrid(grid, stack, 0, 1);


            Content = grid;
        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {

            BlackjackMainViewModel model = (BlackjackMainViewModel)BindingContext;
            _humanHand.LoadList(model.HumanStack!, ts.TagUsed);
            _computerHand.LoadList(model.ComputerStack!, ts.TagUsed);
            return Task.CompletedTask;
        }


        object IUIView.DataContext
        {
            get => BindingContext;
            set => BindingContext = value;
        }
        Task IUIView.TryActivateAsync()
        {
            BlackjackMainViewModel model = (BlackjackMainViewModel)BindingContext;
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
