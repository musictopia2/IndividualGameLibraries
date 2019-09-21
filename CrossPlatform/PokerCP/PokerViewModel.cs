using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.ChooserClasses;
using BasicGameFramework.CommandClasses; //its common to have command classes.
using BasicGameFramework.DIContainers;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.Extensions;
using BasicGameFramework.MainViewModels;
using BasicGameFramework.ViewModelInterfaces;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace PokerCP
{
    public class PokerViewModel : SimpleGameVM, ISoloCardGameVM<PokerCardInfo>
    {
        public readonly CustomBasicCollection<DisplayCard> PokerList = new CustomBasicCollection<DisplayCard>(); //actually its display not pokercard.

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

        private int _BetAmount = 5;

        public int BetAmount
        {
            get { return _BetAmount; }
            set
            {
                if (SetProperty(ref _BetAmount, value))
                {
                    //can decide what to do when property changes
                }

            }
        }

        private decimal _Winnings;

        public decimal Winnings
        {
            get { return _Winnings; }
            set
            {
                if (SetProperty(ref _Winnings, value))
                {
                    //can decide what to do when property changes
                }

            }
        }

        private string _HandLabel = "";

        public string HandLabel
        {
            get { return _HandLabel; }
            set
            {
                if (SetProperty(ref _HandLabel, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private decimal _Money;

        public decimal Money
        {
            get { return _Money; }
            set
            {
                if (SetProperty(ref _Money, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _Round;

        public int Round
        {
            get { return _Round; }
            set
            {
                if (SetProperty(ref _Round, value))
                {
                    //can decide what to do when property changes
                }

            }
        }

        public bool BetPlaced { get; set; }
        public bool IsRoundOver { get; set; }

        public NumberPicker? Bet1;


        public PokerViewModel(ISimpleUI tempUI, IGamePackageResolver tempC) : base(tempUI, tempC)
        {
        }

        public DeckViewModel<PokerCardInfo>? DeckPile { get; set; }

        public async Task DeckClicked()
        {
            _mainGame!.DrawFromDeck();
            await Task.CompletedTask;
        }
        private PokerGameClass? _mainGame;
        public PlainCommand<DisplayCard>? HoldUnHoldCardCommand { get; set; }
        public PlainCommand? NewRoundCommand { get; set; }
        public override void Init()
        {
            _mainGame = MainContainer!.Resolve<PokerGameClass>();
            DeckPile = MainContainer.Resolve<DeckViewModel<PokerCardInfo>>(); //i think.
            CommandContainer!.ExecutingChanged += CommandContainer_ExecutingChanged;
            DeckPile.NeverAutoDisable = true; //if it needs to autoreshuffle. do this.  if you don't want to allow reshuffling set to false.
            DeckPile.SendEnableProcesses(this, () =>
            {
                return !IsRoundOver;
            });
            Bet1 = new NumberPicker(this);
            Bet1.Visible = true;
            Bet1.SendEnableProcesses(this, () =>
            {
                return !BetPlaced;
            });
            Bet1.LoadNumberList(new CustomBasicList<int>() { 5, 10, 25 });
            Bet1.SelectNumberValue(5); //something else has to set to large (?)
            Bet1.ChangedNumberValueAsync += Bet1_ChangedNumberValueAsync;
            HoldUnHoldCardCommand = new PlainCommand<DisplayCard>(thisDisplay =>
            {
                thisDisplay.WillHold = !thisDisplay.WillHold;
            }, items =>
            {
                if (PokerList.Count == 0)
                    return false;
                return !IsRoundOver;
            }, this, CommandContainer);
            NewRoundCommand = new PlainCommand(async items =>
            {
                await _mainGame.NewRoundAsync();
            }, items => IsRoundOver, this, CommandContainer);
        }

        private Task Bet1_ChangedNumberValueAsync(int Chosen)
        {
            BetAmount = Chosen;
            return Task.CompletedTask;
        }

        private async void CommandContainer_ExecutingChanged()
        {
            if (CommandContainer!.IsExecuting)
                return;
            //code to run when its not busy.

            if (_mainGame!.GameGoing)
                await _mainGame.SaveStateAsync();
        }

        public override async Task StartNewGameAsync()
        {
            await _mainGame!.NewGameAsync();
            NewGameVisible = true; //most of the time, will be visible.  if i am wrong, rethink.
        }
        public override bool CanEnableBasics() //since a person can do new game but still do other things.
        {
            return true;
        }
    }
}