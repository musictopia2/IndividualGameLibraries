using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommandClasses; //often times we will need commands.
using BasicGameFramework.DIContainers;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.Extensions;
using BasicGameFramework.MainViewModels;
using BasicGameFramework.RegularDeckOfCards;
using BasicGameFramework.SpecializedGameTypes.RummyClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace Pinochle2PlayerCP
{
    public class Pinochle2PlayerViewModel : TrickGamesVM<EnumSuitList, Pinochle2PlayerCardInformation, Pinochle2PlayerPlayerItem, Pinochle2PlayerMainGameClass>
    {
        public Pinochle2PlayerViewModel(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }
        protected override bool CanEnableDeck()
        {
            return false; //otherwise, can't compile.
        }
        protected override bool CanEnablePile1()
        {
            if (MainGame!.SaveRoot!.ChooseToMeld == false)
                return false;
            return Pile1!.GetCardInfo().Value != EnumCardValueList.Nine && MainGame.SingleInfo!.MainHandList.Any(items => items.Value == EnumCardValueList.Nine && items.Suit == MainGame.SaveRoot.TrumpSuit);
        }
        protected override async Task ProcessDiscardClickedAsync()
        {
            int fromTemps = TempSets!.HowManySelectedObjects;
            int hands = PlayerHand1!.HowManySelectedObjects;
            if (hands == 0 && fromTemps == 0)
            {
                await ShowGameMessageAsync("Must choose a card from hand in order to exchange for the top card");
                return;
            }
            if (hands + fromTemps > 1)
            {
                await ShowGameMessageAsync("Must choose only one card from hand to exchange");
                return;
            }
            Pinochle2PlayerCardInformation thisCard;
            if (fromTemps > 0)
            {
                thisCard = TempSets.GetSelectedObject;
            }
            else
            {
                thisCard = MainGame!.SingleInfo!.MainHandList.GetSpecificItem(PlayerHand1.ObjectSelected());
            }

            if (thisCard.Value > EnumCardValueList.Nine)
            {
                await ShowGameMessageAsync("Must choose a nine to exchange");
                return;
            }
            if (thisCard.Suit != MainGame!.SaveRoot!.TrumpSuit)
            {
                await ShowGameMessageAsync("Must choose the nine of the trump suit in order to exchange");
                return;
            }
            if (ThisData!.MultiPlayer)
                await ThisNet!.SendAllAsync("exchangediscard", thisCard.Deck);
            await MainGame.ExchangeDiscardAsync(thisCard.Deck);
        }
        protected override bool AlwaysEnableHand()
        {
            return false;
        }
        protected override bool CanEnableHand()
        {
            return true;
        }
        public HandViewModel<Pinochle2PlayerCardInformation>? YourMelds;
        public HandViewModel<Pinochle2PlayerCardInformation>? OpponentMelds;
        public TempSetsViewModel<EnumSuitList, EnumColorList, Pinochle2PlayerCardInformation>? TempSets;
        public ScoreGuideViewModel? Guide1;
        public BasicGameCommand? MeldCommand { get; set; }
        public override bool CanEndTurn()
        {
            if (PlayerHand1!.HasSelectedObject() || YourMelds!.HasSelectedObject() || TempSets!.HasSelectedObject)
                return false;
            return MainGame!.SaveRoot!.ChooseToMeld;
        }
        protected override void EndInit()
        {
            base.EndInit(); //must do this too.  most trick taking games does not reshuffle.
            TempSets = new TempSetsViewModel<EnumSuitList, EnumColorList, Pinochle2PlayerCardInformation>();
            TempSets.HowManySets = 2;
            TempSets.Init(this);
            TempSets.SetClickedAsync += TempSets_SetClickedAsync;
            Deck1!.DrawInCenter = true;
            MeldCommand = new BasicGameCommand(this, async items =>
            {
                if (PlayerHand1!.HasSelectedObject() == false && TempSets.HowManySelectedObjects == 0)
                {
                    await ShowGameMessageAsync("Must choose at least one card from hand in order to meld");
                    return;
                }
                var completeList = PlayerHand1.ListSelectedObjects();
                DeckRegularDict<Pinochle2PlayerCardInformation> otherList = new DeckRegularDict<Pinochle2PlayerCardInformation>();
                if (YourMelds!.HasSelectedObject())
                {
                    otherList = YourMelds.ListSelectedObjects();
                    completeList.AddRange(otherList);
                }
                var thisMeld = MainGame!.GetMeldFromList(completeList);
                if (thisMeld.ClassAValue == EnumClassA.None && thisMeld.ClassBValue == EnumClassB.None && thisMeld.ClassCValue == EnumClassC.None)
                {
                    await ShowGameMessageAsync("There is no meld combinations here");
                    return;
                }
                if (YourMelds.HasSelectedObject())
                {
                    foreach (var thisCard in otherList)
                    {
                        var tempMeld = MainGame.GetMeldFromCard(thisCard);
                        if (tempMeld.ClassAValue == EnumClassA.Dix)
                            throw new BasicBlankException("Should have caught the problem with using dix earlier");
                        if (tempMeld.ClassBValue == thisMeld.ClassBValue && thisMeld.ClassBValue > EnumClassB.None)
                        {
                            await ShowGameMessageAsync("Cannot reuse class b for class b");
                            return;
                        }
                        if (tempMeld.ClassCValue == thisMeld.ClassCValue && thisMeld.ClassCValue > EnumClassC.None)
                        {
                            await ShowGameMessageAsync("Cannot reuse a pinochle for a pinochle");
                            return;
                        }
                        if (tempMeld.ClassAValue <= thisMeld.ClassAValue && thisMeld.ClassAValue > EnumClassA.None)
                        {
                            await ShowGameMessageAsync("Cannot download class A to get more points or to create another of same value");
                            return;
                        }
                    }
                }
                var deckList = completeList.GetDeckListFromObjectList();
                if (ThisData!.MultiPlayer)
                    await ThisNet!.SendAllAsync("meld", deckList);
                await MainGame.MeldAsync(deckList);
            }, items => MainGame!.SaveRoot!.ChooseToMeld, this, CommandContainer!);
            YourMelds = new HandViewModel<Pinochle2PlayerCardInformation>(this);
            YourMelds.Text = "Yours";
            YourMelds.SendEnableProcesses(this, () =>
            {
                if (MainGame!.SaveRoot!.ChooseToMeld)
                {
                    YourMelds.AutoSelect = HandViewModel<Pinochle2PlayerCardInformation>.EnumAutoType.SelectAsMany;
                    return true;
                }
                YourMelds.AutoSelect = HandViewModel<Pinochle2PlayerCardInformation>.EnumAutoType.SelectOneOnly;
                return Pile1!.PileEmpty() == false && MainGame.SaveRoot.MeldList.Any(items => items.Player == MainGame.WhoTurn && items.CardList.Count > 0);
            });
            YourMelds.AutoSelect = HandViewModel<Pinochle2PlayerCardInformation>.EnumAutoType.SelectOneOnly;
            OpponentMelds = new HandViewModel<Pinochle2PlayerCardInformation>(this);
            OpponentMelds.Text = "Opponents";
            OpponentMelds.SendEnableProcesses(this, () => false);
            YourMelds.Maximum = 8;
            OpponentMelds.Maximum = 8;
            YourMelds.IgnoreMaxRules = true;
            OpponentMelds.IgnoreMaxRules = true;
            Guide1 = new ScoreGuideViewModel();
            CommandContainer!.ExecutingChanged += CommandContainer_ExecutingChanged;
        }
        private void CommandContainer_ExecutingChanged()
        {
            if (CommandContainer!.IsExecuting)
                return;
            if (MainGame!.SaveRoot!.ChooseToMeld)
                PlayerHand1!.AutoSelect = HandViewModel<Pinochle2PlayerCardInformation>.EnumAutoType.SelectAsMany;
            else
                PlayerHand1!.AutoSelect = HandViewModel<Pinochle2PlayerCardInformation>.EnumAutoType.SelectOneOnly;
        }
        public override bool CanEnableAlways()
        {
            return true;
        }
        private bool _isProcessing;
        private Task TempSets_SetClickedAsync(int index)
        {
            if (_isProcessing == true)
                return Task.CompletedTask;
            _isProcessing = true;
            var TempList = PlayerHand1!.ListSelectedObjects(true);
            TempSets!.AddCards(index, TempList);
            _isProcessing = false;
            return Task.CompletedTask;
        }
    }
}