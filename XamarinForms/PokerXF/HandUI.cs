using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.DataClasses;
using BasicGamingUIXFLibrary.GameGraphics.Cards;
using BasicGamingUIXFLibrary.Helpers;
using BasicXFControlsAndPages.Helpers;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using PokerCP.Data;
using PokerCP.ViewModels;
using System;
using System.Collections.Specialized;
using System.Reflection;
using Xamarin.Forms;
using static BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;

namespace PokerXF
{
    public class HandUI : ContentView, IHandle<PokerCP.EventModels.NewCardEventModel>
    {
        private CustomBasicCollection<DisplayCard>? _thisList;
        private Grid? _thisGrid;
        private PokerMainViewModel? _thisMod;
        public HandUI(IEventAggregator aggregator)
        {
            aggregator.Subscribe(this);
            _aggregator = aggregator;
        }
        private DeckOfCardsXF<PokerCardInfo>? FindControl(int index)
        {
            foreach (View? thisCon in _thisGrid!.Children)
            {
                if (Grid.GetRow(thisCon) == 0 && Grid.GetColumn(thisCon) == index)
                    return thisCon as DeckOfCardsXF<PokerCardInfo>;
            }
            throw new BasicBlankException("No control found");
        }
        private Binding GetCommandBinding(string path)
        {
            Binding ThisBind = new Binding(path);
            ThisBind.Source = _thisMod;
            return ThisBind;
        }
        private void PopulateControls()
        {
            _thisGrid!.Children.Clear(); // i think
            if (_thisList!.Count == 0)
                return;
            if (_thisList.Count != 5)
                throw new BasicBlankException("Must have 5 cards for poker hand.");
            int x = 0;
            foreach (var display in _thisList)
            {
                var graphics = GetNewCard(display);
                graphics.CommandParameter = display;
                GridHelper.AddControlToGrid(_thisGrid, graphics, 0, x);
                var thisLabel = SharedUIFunctions.GetDefaultLabel();
                if (ScreenUsed != EnumScreen.SmallPhone)
                    thisLabel.FontSize = 20; // can always be adjusted as needed
                else
                    thisLabel.FontSize = 12;
                thisLabel.HorizontalOptions = LayoutOptions.Center;
                thisLabel.FontAttributes = FontAttributes.Bold;
                thisLabel.BindingContext = display;
                thisLabel.SetBinding(Label.TextProperty, new Binding(nameof(DisplayCard.Text)));
                GridHelper.AddControlToGrid(_thisGrid, thisLabel, 1, x);
                x += 1;
            }
        }
        private DeckOfCardsXF<PokerCardInfo> GetNewCard(DisplayCard thisPoker)
        {
            DeckOfCardsXF<PokerCardInfo> thisCard = new DeckOfCardsXF<PokerCardInfo>();
            thisCard.SendSize(ts.TagUsed, thisPoker.CurrentCard);
            thisCard.Command = _command!;
            return thisCard;
        }
        private void ThisList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            PopulateControls();
        }

        void IHandle<PokerCP.EventModels.NewCardEventModel>.Handle(PokerCP.EventModels.NewCardEventModel message)
        {
            if (_thisList!.Count == 0)
                return;
            var newCard = _thisList[message.Index];
            var thisControl = FindControl(message.Index);
            thisControl!.BindingContext = newCard.CurrentCard; //hopefully this is enough (?)
        }

        private PlainCommand? _command;
        private readonly IEventAggregator _aggregator;

        public void Init(PokerMainViewModel thisMod)
        {
            _thisMod = thisMod;
            Type type = _thisMod!.GetType();
            MethodInfo? method = type.GetMethod(nameof(PokerMainViewModel.HoldUnhold));
            PropertyInfo? fun = type.GetProperty(nameof(PokerMainViewModel.CanHoldUnhold));
            if (method == null)
            {
                throw new BasicBlankException("Method not found for hand.  Rethink");
            }
            if (fun == null)
            {
                throw new BasicBlankException("Function not found for hand.  Rethink");
            }
            _command = new PlainCommand(_thisMod, method, fun, _thisMod.CommandContainer);
            _thisGrid = new Grid();
            GridHelper.AddAutoColumns(_thisGrid, 5);
            GridHelper.AddAutoRows(_thisGrid, 2);
            _thisList = thisMod.PokerList;
            _thisList.CollectionChanged += ThisList_CollectionChanged;
            Content = _thisGrid;
            PopulateControls();

        }
    }
}