using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommandClasses; //often times we will need commands.
using BasicGameFramework.DIContainers;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.Extensions;
using BasicGameFramework.MainViewModels;
using BasicGameFramework.RegularDeckOfCards;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace SixtySix2PlayerCP
{
    public class SixtySix2PlayerViewModel : TrickGamesVM<EnumSuitList, SixtySix2PlayerCardInformation, SixtySix2PlayerPlayerItem, SixtySix2PlayerMainGameClass>
    {
        private int _BonusPoints;

        public int BonusPoints
        {
            get { return _BonusPoints; }
            set
            {
                if (SetProperty(ref _BonusPoints, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        public SixtySix2PlayerViewModel(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }
        protected override bool CanEnableDeck()
        {
            return false; //otherwise, can't compile.
        }
        protected override bool CanEnablePile1()
        {
            return MainGame!.CanExchangeForDiscard();
        }
        protected override async Task ProcessDiscardClickedAsync()
        {
            if (MainGame!.CanExchangeForDiscard() == false)
                throw new BasicBlankException("Should have been disabled because cannot exchange for discard");
            int howMany = PlayerHand1!.HowManySelectedObjects;
            if (howMany == 0)
            {
                await ShowGameMessageAsync("Must choose a card to exchange");
                return;
            }
            if (howMany > 1)
            {
                await ShowGameMessageAsync("Cannot choose more than one card to exchange");
                return;
            }
            int decks = PlayerHand1!.ObjectSelected();
            var thisCard = MainGame!.SingleInfo!.MainHandList.GetSpecificItem(decks);
            if (thisCard.Value > EnumCardValueList.Nine)
            {
                await ShowGameMessageAsync("Must choose a nine to exchange");
                return;
            }
            if (thisCard.Suit != MainGame.SaveRoot!.TrumpSuit)
            {
                await ShowGameMessageAsync("Must choose the nine of the trump suit in order to exchange");
                return;
            }
            if (ThisData!.MultiPlayer)
                await ThisNet!.SendAllAsync("exchangediscard", thisCard.Deck);
            await MainGame!.ExchangeDiscardAsync(thisCard.Deck);
        }
        public CustomBasicList<ScoreValuePair> GetDescriptionList()
        {
            return new CustomBasicList<ScoreValuePair>()
            {
                new ScoreValuePair("Marriage In Trumps (K, Q announced)", 40),
                new ScoreValuePair("Marriage In Any Other Suit (K, Q announced)", 20),
                new ScoreValuePair("Each Ace", 11),
                new ScoreValuePair("Each 10", 10),
                new ScoreValuePair("Each King", 4),
                new ScoreValuePair("Each Queen", 3),
                new ScoreValuePair("Each Jack", 2),
                new ScoreValuePair("Last Trick", 10)
            };
        }
        public HandViewModel<SixtySix2PlayerCardInformation>? Marriage1;
        public BasicGameCommand? AnnounceMarriageCommand { get; set; }
        public BasicGameCommand? GoOutCommand { get; set; }
        protected override void EndInit()
        {
            base.EndInit(); //must do this too.  most trick taking games does not reshuffle.
            Deck1!.DrawInCenter = true;
            PlayerHand1!.Maximum = 6;
            Marriage1 = new HandViewModel<SixtySix2PlayerCardInformation>(this);
            Marriage1.Visible = false; //i guess it starts out as false
            Marriage1.Maximum = 2;
            Marriage1.Text = "Cards For Marriage";
            CommandContainer!.ExecutingChanged += CommandContainer_ExecutingChanged;
            AnnounceMarriageCommand = new BasicGameCommand(this, async items =>
            {
                int howMany = PlayerHand1.HowManySelectedObjects;
                if (howMany != 2)
                {
                    await ShowGameMessageAsync("Must choose 2 cards");
                    return;
                }
                var thisList = PlayerHand1.ListSelectedObjects();
                var thisMarriage = MainGame!.WhichMarriage(thisList);
                if (thisMarriage == EnumMarriage.None)
                {
                    await ShowGameMessageAsync("This is not a valid marrige");
                    return;
                }
                if (MainGame.CanShowMarriage(thisMarriage) == false)
                {
                    await ShowGameMessageAsync("Cannot show marriage because the points will put you over 66 points.");
                    return;
                }
                var tempList = thisList.GetDeckListFromObjectList();
                if (ThisData!.MultiPlayer)
                    await ThisNet!.SendAllAsync("announcemarriage", tempList);
                await MainGame.AnnounceMarriageAsync(tempList);
            }, items => MainGame!.Trick1!.IsLead && MainGame.SaveRoot!.CardsForMarriage.Count == 0, this, CommandContainer);
            GoOutCommand = new BasicGameCommand(this, async items =>
            {
                if (ThisData!.MultiPlayer)
                    await ThisNet!.SendAllAsync("goout");
                await MainGame!.GoOutAsync();
            }, items =>
            {
                if (MainGame!.CanAnnounceMarriageAtBeginning == true || MainGame.Trick1!.IsLead && MainGame.SaveRoot!.CardsForMarriage.Count == 0)
                    return MainGame.Trick1!.IsLead;
                return false;
            }, this, CommandContainer);
        }
        private void CommandContainer_ExecutingChanged()
        {
            if (CommandContainer!.IsExecuting)
                return; //for now, has to be this way.
            if (AnnounceMarriageCommand!.CanExecute(null!))
                PlayerHand1!.AutoSelect = HandViewModel<SixtySix2PlayerCardInformation>.EnumAutoType.SelectAsMany;
            else
                PlayerHand1!.AutoSelect = HandViewModel<SixtySix2PlayerCardInformation>.EnumAutoType.SelectOneOnly;
        }
        public override bool CanEnableAlways()
        {
            return true;
        }
    }
}