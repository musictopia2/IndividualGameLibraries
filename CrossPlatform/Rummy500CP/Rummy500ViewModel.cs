using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommandClasses; //often times we will need commands.
using BasicGameFramework.DIContainers;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.Extensions;
using BasicGameFramework.MainViewModels;
using BasicGameFramework.RegularDeckOfCards;
using BasicGameFramework.SpecializedGameTypes.RummyClasses;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace Rummy500CP
{
    public class Rummy500ViewModel : BasicCardGamesVM<RegularRummyCard, Rummy500PlayerItem, Rummy500MainGameClass>
    {
        public Rummy500ViewModel(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }
        protected override bool CanEnableDeck()
        {
            return !MainGame!.AlreadyDrew;
        }
        protected override bool CanEnablePile1()
        {
            return false;
        }
        protected override async Task ProcessDiscardClickedAsync() //something else is being done instead.
        {
            await Task.CompletedTask;
        }
        public override bool CanEnableAlways()
        {
            return true;
        }
        public DiscardListCP? DiscardList1;
        public MainSetsViewModel<EnumSuitList, EnumColorList, RegularRummyCard, RummySet, SavedSet>? MainSets1;
        public BasicGameCommand? CreateSetCommand { get; set; }
        public BasicGameCommand? DiscardCurrentCommand { get; set; }
        protected override void EndInit()
        {
            base.EndInit(); //must do this too.
            Deck1!.NeverAutoDisable = true; //if you want reshuffling, use this.  otherwise, comment or delete.
            PlayerHand1!.AutoSelect = HandViewModel<RegularRummyCard>.EnumAutoType.SelectAsMany;
            MainSets1 = new MainSetsViewModel<EnumSuitList, EnumColorList, RegularRummyCard, RummySet, SavedSet>(this);
            DiscardList1 = new DiscardListCP(this);
            DiscardList1.Visible = true;
            MainSets1.SetClickedAsync += MainSets1_SetClickedAsync;
            DiscardList1.ObjectClickedAsync += DiscardList1_ObjectClickedAsync;
            if (ThisData!.IsXamarinForms == false)
                DiscardList1.BoardClickedAsync += DiscardList1_BoardClickedAsync; //not for xamarin forms.
            MainSets1.SendEnableProcesses(this, () => MainGame!.AlreadyDrew);
            Pile1!.Visible = false; //we have something else instead.
            CreateSetCommand = new BasicGameCommand(this, async items =>
            {
                var thisCol = PlayerHand1.ListSelectedObjects();
                if (thisCol.Count == PlayerHand1.HandList.Count)
                {
                    await ShowGameMessageAsync("Sorry, you must have one card left over to discard");
                    PlayerHand1.UnselectAllObjects();
                    return;
                }
                if (MainGame!.IsValidRummy(thisCol, out EnumWhatSets SetType, out bool Seconds) == false)
                {
                    await ShowGameMessageAsync("This is not a valid rummy");
                    PlayerHand1.UnselectAllObjects();
                    return;
                }
                if (thisCol.Count == 1)
                {
                    if (thisCol.Single().Deck == MainGame.PreviousCard)
                    {
                        await ShowGameMessageAsync("Sorry, since the last card left is the card picked up, then cannot put down the rummy");
                        return;
                    }
                }
                if (ThisData!.MultiPlayer == true)
                {
                    SendNewSet thisNew = new SendNewSet();
                    thisNew.DeckList = thisCol.GetDeckListFromObjectList();
                    thisNew.SetType = SetType;
                    thisNew.UseSecond = Seconds;
                    await ThisNet!.SendAllAsync("newset", thisNew);
                }
                await MainGame.CreateNewSetAsync(thisCol, SetType, Seconds);
            }, items =>
            {
                return MainGame!.AlreadyDrew;
            }, this, CommandContainer!);
            DiscardCurrentCommand = new BasicGameCommand(this, async items =>
            {
                await NewDiscardClickAsync(0);
            }, items =>
            {
                return MainGame!.AlreadyDrew;
            }, this, CommandContainer!
            );
        }
        private bool _didClickDiscard;
        private async Task DiscardList1_BoardClickedAsync()
        {
            if (ThisData!.IsXamarinForms == true)
            {
                if (DiscardList1!.HandList.Count == 0)
                    return;
            }
            if (_didClickDiscard == true) //maybe 
            {
                _didClickDiscard = false;
                return;
            }
            _didClickDiscard = true;
            await NewDiscardClickAsync(0);
        }
        private async Task DiscardList1_ObjectClickedAsync(RegularRummyCard thisObject, int index)
        {
            _didClickDiscard = true;
            await NewDiscardClickAsync(thisObject.Deck);
        }
        private async Task MainSets1_SetClickedAsync(int setNumber, int section, int deck)
        {
            var NewCol = PlayerHand1!.ListSelectedObjects();
            if (NewCol.Count == 0)
            {
                await ShowGameMessageAsync("There is no card selected");
                return;
            }
            if (NewCol.Count > 1)
            {
                await ShowGameMessageAsync("Only can expand one card at a time");
                return;
            }
            if (PlayerHand1.HandList.Count == 1)
            {
                await ShowGameMessageAsync("Sorry, must have a card left for discard");
                return;
            }
            var thisCard = NewCol.First();
            RummySet thisSet = MainSets1!.GetIndividualSet(setNumber);
            int pos = thisSet.PositionToPlay(thisCard);
            if (pos == 0)
            {
                await ShowGameMessageAsync("This cannot be used to expand upto");
                return;
            }
            var thisCol = MainSets1.SetList.ToCustomBasicList();
            int x = 0;
            int nums = 0;
            thisCol.ForEach(newSet =>
            {
                x++;
                if (newSet.Equals(thisSet))
                    nums = x;
            });
            if (nums == 0)
                throw new BasicBlankException("Cannot find the rummy set that matches");
            if (ThisData!.MultiPlayer == true)
            {
                SendAddSet thisSend = new SendAddSet();
                thisSend.Deck = thisCard.Deck;
                thisSend.Position = pos;
                thisSend.Index = nums;
                await ThisNet!.SendAllAsync("addtoset", thisSend);
            }
            await MainGame!.AddToSetAsync(nums, thisCard.Deck, pos);
        }
        private async Task NewDiscardClickAsync(int deck)
        {
            if (MainGame!.CanProcessDiscard(out bool pickUp, ref deck, out string message) == false)
            {
                await ShowGameMessageAsync(message);
                return;
            }
            if (pickUp == true)
            {
                var thisCol = DiscardList1!.DiscardListSelected(deck);
                if (thisCol.Count > 1)
                {
                    var newCol = MainGame.AppendDiscardList(thisCol);
                    if (MainGame.CardContainsRummy(deck, newCol) == false)
                    {
                        await ShowGameMessageAsync("Sorry, cannot pick up more than one card because either invalid rummy or no card left for discard");
                        return;
                    }
                }
                if (ThisData!.MultiPlayer == true)
                    await ThisNet!.SendAllAsync("pickupfromdiscard", deck);
                await MainGame.PickupFromDiscardAsync(deck);
                return;
            }
            await MainGame.SendDiscardMessageAsync(deck);
            await MainGame.DiscardAsync(deck);
        }
    }
}