using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.ChooserClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.Extensions;
using BasicGameFrameworkLibrary.ViewModelInterfaces;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using PokerCP.Data;
using PokerCP.Logic;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using BasicGameFrameworkLibrary.BasicEventModels;
namespace PokerCP.ViewModels
{
    [InstanceGame]
    public class PokerMainViewModel : Screen,
        IBasicEnableProcess,
        IBlankGameVM,
        IAggregatorContainer
    {
        private readonly IEventAggregator _aggregator;
        private readonly PokerMainGameClass _mainGame;

        public CustomBasicCollection<DisplayCard> PokerList => GlobalClass.PokerList;

        public DeckObservablePile<PokerCardInfo> DeckPile { get; set; }

        public DeckRegularDict<PokerCardInfo> GetCardList => PokerList.Select(items => items.CurrentCard).ToRegularDeckDict();

        public int SpotsToFill
        {
            get
            {
                if (PokerList.Count == 0)
                    return 5;
                return PokerList.Count(items => items.WillHold == false);
            }
        }

        public void PopulateNewCards(IDeckDict<PokerCardInfo> thisList)
        {
            DisplayCard thisDisplay;
            if (PokerList.Count == 0)
            {
                CustomBasicList<DisplayCard> newList = new CustomBasicList<DisplayCard>();
                if (thisList.Count != 5)
                    throw new BasicBlankException("Must have 5 cards for the poker hand");
                thisList.ForEach(thisCard =>
                {
                    thisDisplay = new DisplayCard();
                    thisDisplay.CurrentCard = thisCard;
                    newList.Add(thisDisplay);
                });
                PokerList.ReplaceRange(newList);
                return;
            }
            var tempList = PokerList.Where(items => items.WillHold == false).ToCustomBasicList();
            if (tempList.Count != thisList.Count)
                throw new BasicBlankException("Mismatch for populating new cards");
            int x = 0;
            tempList.ForEach(temps =>
            {
                var thisCard = thisList[x];
                temps.CurrentCard = thisCard;
                x++;
            });
        }

        private int _betAmount = 5;

        public int BetAmount
        {
            get { return _betAmount; }
            set
            {
                if (SetProperty(ref _betAmount, value))
                {
                    //can decide what to do when property changes
                }

            }
        }

        private decimal _winnings;

        public decimal Winnings
        {
            get { return _winnings; }
            set
            {
                if (SetProperty(ref _winnings, value))
                {
                    //can decide what to do when property changes
                }

            }
        }

        private string _handLabel = "";

        public string HandLabel
        {
            get { return _handLabel; }
            set
            {
                if (SetProperty(ref _handLabel, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private decimal _money;

        public decimal Money
        {
            get { return _money; }
            set
            {
                if (SetProperty(ref _money, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _round;

        public int Round
        {
            get { return _round; }
            set
            {
                if (SetProperty(ref _round, value))
                {
                    //can decide what to do when property changes
                }

            }
        }

        public bool BetPlaced { get; set; }
        public bool IsRoundOver { get; set; }

        public NumberPicker Bet1; //decided to have the numberpicker here.  because not only for new game but new round.
        //could have splitted it but decided not to this time.

        public bool CanHoldUnhold
        {
            get
            {
                if (PokerList.Count == 0)
                {
                    return false;
                }
                return !IsRoundOver;
            }
        }

        

        [Command(EnumCommandCategory.Plain)]
        public void HoldUnhold(DisplayCard display)
        {
            if (display == null)
            {
                throw new BasicBlankException("The holdunhold showed nothing.  Rethink");
            }
            display.WillHold = !display.WillHold;
        }
        public bool CanNewRound => IsRoundOver;
        [Command(EnumCommandCategory.Plain)]
        public async Task NewRoundAsync()
        {
            await _mainGame.NewRoundAsync();
        }
        public PokerMainViewModel(IEventAggregator aggregator,
            CommandContainer commandContainer,
            IGamePackageResolver resolver
            )
        {
            GlobalClass.PokerList.Clear(); //can't be brand new  that could cause the connection to break too.
            _aggregator = aggregator; //this time no new game.  just close out when you are done now.
            Round = 1;
            CommandContainer = commandContainer;
            CommandContainer.ExecutingChanged += CommandContainer_ExecutingChanged; //hopefully no problem (?)
            DeckPile = resolver.ReplaceObject<DeckObservablePile<PokerCardInfo>>();
            DeckPile.DeckClickedAsync += DeckPile_DeckClickedAsync;
            DeckPile.NeverAutoDisable = true;
            DeckPile.SendEnableProcesses(this, () =>
            {
                return !IsRoundOver;
            });

            Bet1 = new NumberPicker(commandContainer, resolver);
            Bet1.SendEnableProcesses(this, () =>
            {
                return !BetPlaced;
            });
            Bet1.LoadNumberList(new CustomBasicList<int>() { 5, 10, 25 });
            Bet1.SelectNumberValue(5); //something else has to set to large (?)
            Bet1.ChangedNumberValueAsync += Bet1_ChangedNumberValueAsync;

            _mainGame = resolver.ReplaceObject<PokerMainGameClass>(); //hopefully this works.  means you have to really rethink.
        }

        private async Task DeckPile_DeckClickedAsync()
        {
            _mainGame!.DrawFromDeck();
            await Task.CompletedTask;
        }

        private async void CommandContainer_ExecutingChanged()
        {
            if (CommandContainer.IsExecuting)
                return;
            //code to run when its not busy.

            if (_mainGame.GameGoing)
                await _mainGame.SaveStateAsync();
        }
        private Task Bet1_ChangedNumberValueAsync(int chosen)
        {
            BetAmount = chosen;
            return Task.CompletedTask;
        }
        public CommandContainer CommandContainer { get; set; }

        IEventAggregator IAggregatorContainer.Aggregator => _aggregator;

        public bool CanEnableBasics()
        {
            return true; //because maybe you can't enable it.
        }
        protected override async Task ActivateAsync()
        {
            await base.ActivateAsync();
            await _mainGame.NewGameAsync(this);
            await _aggregator.SendLoadAsync();

        }
        protected override Task TryCloseAsync()
        {
            CommandContainer.ExecutingChanged -= CommandContainer_ExecutingChanged;
            return base.TryCloseAsync();
        }
    }
}
