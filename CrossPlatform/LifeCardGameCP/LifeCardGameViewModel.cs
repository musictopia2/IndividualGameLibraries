using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommandClasses; //often times we will need commands.
using BasicGameFramework.DIContainers;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.MainViewModels;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMHelpers;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace LifeCardGameCP
{
    public class LifeCardGameViewModel : BasicCardGamesVM<LifeCardGameCardInformation, LifeCardGamePlayerItem, LifeCardGameMainGameClass>
    {
        private bool _OtherVisible;
        public bool OtherVisible
        {
            get { return _OtherVisible; }
            set
            {
                if (SetProperty(ref _OtherVisible, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private string _OtherText = "";
        public string OtherText
        {
            get { return _OtherText; }
            set
            {
                if (SetProperty(ref _OtherText, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        public LifeCardGameViewModel(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }
        protected override bool CanEnableDeck()
        {
            return false;
        }
        protected override bool CanEnablePile1()
        {
            return CurrentPile!.PileEmpty();
        }
        protected override async Task ProcessDiscardClickedAsync()
        {
            int newDeck = PlayerHand1!.ObjectSelected();
            if (newDeck == 0)
            {
                await ShowGameMessageAsync("Sorry, must select a card first");
                return;
            }
            if (ThisData!.MultiPlayer)
                await ThisNet!.SendAllAsync("discard", newDeck);
            await MainGame!.DiscardAsync(newDeck);
        }
        public override bool CanEnableAlways()
        {
            return true;
        }
        public PileViewModel<LifeCardGameCardInformation>? CurrentPile;
        public Command? YearsPassedCommand { get; set; }
        public BasicGameCommand? PlayCardCommand { get; set; }
        public BasicGameCommand? OtherCommand { get; set; }
        protected override void EndInit()
        {
            base.EndInit(); //must do this too.
            Deck1!.NeverAutoDisable = true; //if you want reshuffling, use this.  otherwise, comment or delete.
            CurrentPile = new PileViewModel<LifeCardGameCardInformation>(ThisE!, this);
            PlayerHand1!.Maximum = 5;
            CurrentPile.SendEnableProcesses(this, () => false);
            CurrentPile.Visible = true;
            CurrentPile.Text = "Current Card";
            MainGame!.OtherPile = CurrentPile; //hopefully this is okay this time.
            YearsPassedCommand = new Command(async items => await ShowGameMessageAsync($"{MainGame.SaveRoot!.YearsPassed()} passed.  Once it reaches 60; the game is over"), items => true, this);
            PlayCardCommand = new BasicGameCommand(this, async items =>
            {
                int decks = PlayerHand1.ObjectSelected();
                if (decks == 0)
                {
                    await ShowGameMessageAsync("Must choose a card to play");
                    return;
                }
                var thisCard = MainGame.SingleInfo!.MainHandList.GetSpecificItem(decks);
                if (MainGame.CanPlayCard(thisCard) == false)
                {
                    await ShowGameMessageAsync("Illegal Move");
                    return;
                }
                if (ThisData!.MultiPlayer)
                    await ThisNet!.SendAllAsync("playcard", decks);
                CommandContainer!.ManuelFinish = true;
                await MainGame.PlayCardAsync(thisCard);

            }, items => CurrentPile.PileEmpty(), this, CommandContainer!);
            OtherCommand = new BasicGameCommand(this, async items =>
            {
                (int yours, int others) = CardsChosen();
                if (IsValidMove(yours, others) == false)
                {
                    await ShowGameMessageAsync("Illegal Move");
                    return;
                }
                if (yours == 0 && others == 0)
                    throw new BasicBlankException("Must have chosen at least one");
                int decks;
                if (yours == 0)
                    decks = others;
                else if (others == 0)
                    decks = yours;
                else
                    decks = 0;
                if (decks > 0)
                {
                    if (ThisData!.MultiPlayer)
                        await ThisNet!.SendAllAsync("cardchosen", decks);
                    var tempCard = MainGame.DeckList!.GetSpecificItem(decks);
                    CommandContainer!.ManuelFinish = true;
                    await MainGame.ChoseSingleCardAsync(tempCard);
                    return;
                }
                if (ThisData!.MultiPlayer)
                {
                    TradeCard thisTrade = new TradeCard();
                    thisTrade.YourCard = yours;
                    thisTrade.OtherCard = others;
                    await ThisNet!.SendAllAsync("cardstraded", thisTrade);
                }
                var yourCard = MainGame.DeckList!.GetSpecificItem(yours);
                var opponentCard = MainGame.DeckList.GetSpecificItem(others);
                CommandContainer!.ManuelFinish = true;
                await MainGame.TradeCardsAsync(yourCard, opponentCard);
            }, items => OtherVisible, this, CommandContainer!);
            CommandContainer!.ExecutingChanged += CommandContainer_ExecutingChanged;
        }
        private void CommandContainer_ExecutingChanged()
        {
            MainGame!.PlayerList!.EnableLifeStories(MainGame, !CommandContainer!.IsExecuting); //i think
        }
        private (int yours, int others) CardsChosen()
        {
            int yours = MainGame!.SingleInfo!.LifeStory!.ObjectSelected();
            int others = MainGame.ThisGlobal!.OtherCardSelected();
            return (yours, others);
        }
        private bool IsValidMove(int yours, int others)
        {
            var thisCard = CurrentPile!.GetCardInfo();
            LifeCardGameCardInformation otherCard;
            if (thisCard.Action == EnumAction.Lawsuit || thisCard.Action == EnumAction.DonateToCharity)
            {
                if (others > 0)
                    throw new BasicBlankException("Should have only chosen your own card");

                if (yours == 0)
                    return false;
                otherCard = MainGame!.DeckList!.GetSpecificItem(yours);
                if (thisCard.Action == EnumAction.Lawsuit)
                    return otherCard.SpecialCategory != EnumSpecialCardCategory.Marriage && otherCard.Points >= 30;
                return otherCard.Category == EnumFirstCardCategory.Wealth && otherCard.Points > 5 && otherCard.SpecialCategory != EnumSpecialCardCategory.Passport;
            }
            if (thisCard.Action == EnumAction.MixUpAtVets || thisCard.Action == EnumAction.MovingHouse || thisCard.Action == EnumAction.CareerSwap)

                return yours > 0 && others > 0;
            if (yours > 0)
                throw new BasicBlankException("Should have only been allowed to choose another player");
            return others > 0;
        }
    }
}