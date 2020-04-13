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
using CommonBasicStandardLibraries.Messenging;
using PokerCP.EventModels;
using BasicGamingUIWPFLibrary.GameGraphics.Cards;
using PokerCP.Data;
using PokerCP.ViewModels;
using System.Windows;
using System.Windows.Data;
using BasicControlsAndWindowsCore.Helpers;
using BasicGamingUIWPFLibrary.Helpers;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Cards.DeckOfCardsCP;
using System.Collections.Specialized;
using System.Reflection;
using BasicGameFrameworkLibrary.CommandClasses;
using PokerWPF.Views;
using BasicGameFrameworkLibrary.BasicEventModels;
using System.Threading;

namespace PokerWPF
{
    public class HandUI : UserControl, IHandle<PokerCP.EventModels.NewCardEventModel>
    {
        private CustomBasicCollection<DisplayCard>? _thisList;
        private Grid? _thisGrid;
        private PokerMainViewModel? _thisMod;
        private DeckOfCardsWPF<PokerCardInfo>? FindControl(int index)
        {
            foreach (UIElement? thisCon in _thisGrid!.Children)
            {
                if (Grid.GetRow(thisCon) == 0 && Grid.GetColumn(thisCon) == index)
                    return thisCon as DeckOfCardsWPF<PokerCardInfo>;
            }
            throw new BasicBlankException("No control found");
        }

        public HandUI(IEventAggregator aggregator)
        {
            aggregator.Subscribe(this);
            _aggregator = aggregator;
            //_view = view;
        }
        private void PopulateControls()
        {
            _thisGrid!.Children.Clear(); // i think
            if (_thisList!.Count == 0)
                return;
            if (_thisList.Count != 5)
                throw new BasicBlankException("Must have 5 cards for poker hand.");
            int x = 0;
            foreach (var display in GlobalClass.PokerList)
            {
                var graphics = GetNewCard(display);
                graphics.CommandParameter = display;
                //var button = new Button()
                //{
                //    Name = nameof(PokerMainViewModel.HoldUnhold),
                //    Content = "Test"
                //};
                //GridHelper.AddControlToGrid(_thisGrid, button, 0, x);
                GridHelper.AddControlToGrid(_thisGrid, graphics, 0, x);
                var label = SharedUIFunctions.GetDefaultLabel();
                label.FontSize = 20; // can always be adjusted as needed
                label.HorizontalAlignment = HorizontalAlignment.Center;
                label.FontWeight = FontWeights.Bold;
                label.DataContext = display;
                label.SetBinding(TextBlock.TextProperty, new Binding(nameof(DisplayCard.Text)));
                GridHelper.AddControlToGrid(_thisGrid, label, 1, x);
                x += 1;
            }
        }
        private DeckOfCardsWPF<PokerCardInfo> GetNewCard(DisplayCard thisPoker)
        {
            DeckOfCardsWPF<PokerCardInfo> thisCard = new DeckOfCardsWPF<PokerCardInfo>();
            thisCard.SendSize(ts.TagUsed, thisPoker.CurrentCard);


            //thisCard.Name = nameof(PokerMainViewModel.HoldUnhold);

            

            thisCard.Command = _command!;
            return thisCard;
        }

        private void ThisList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            PopulateControls();
            //await _view.RefreshBindingsAsync(_aggregator);
        }
        private PlainCommand? _command;
        private readonly IEventAggregator _aggregator;
        //private readonly PokerMainView _view;

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

        void IHandle<PokerCP.EventModels.NewCardEventModel>.Handle(PokerCP.EventModels.NewCardEventModel message)
        {
            if (_thisList!.Count == 0)
                return;
            var newCard = _thisList[message.Index];
            var thisControl = FindControl(message.Index);
            thisControl!.DataContext = newCard.CurrentCard; //hopefully this is enough (?)
        }
    }
}
