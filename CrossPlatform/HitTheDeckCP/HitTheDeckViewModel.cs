using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommandClasses; //often times we will need commands.
using BasicGameFramework.DIContainers;
using BasicGameFramework.MainViewModels;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace HitTheDeckCP
{
    public class HitTheDeckViewModel : BasicCardGamesVM<HitTheDeckCardInformation, HitTheDeckPlayerItem, HitTheDeckMainGameClass>
    {
        private string _NextPlayer = "";

        public string NextPlayer
        {
            get { return _NextPlayer; }
            set
            {
                if (SetProperty(ref _NextPlayer, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        public HitTheDeckViewModel(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }
        private bool NeedsSpecial
        {
            get
            {
                var thisCard = Pile1!.GetCardInfo();
                return thisCard.Instructions == EnumInstructionList.Cut || thisCard.Instructions == EnumInstructionList.Flip;
            }
        }
        protected override bool CanEnableDeck()
        {
            if (NeedsSpecial == true)
                return false; //otherwise, can't compile.
            if (MainGame!.SingleInfo!.MainHandList.Any(items => MainGame.CanPlay(items.Deck)))
                return false;
            return !MainGame.AlreadyDrew;
        }
        protected override bool CanEnablePile1()
        {
            return !NeedsSpecial;
        }
        protected override async Task ProcessDiscardClickedAsync()
        {
            int newDeck = PlayerHand1!.ObjectSelected();
            if (newDeck == 0)
            {
                await ShowGameMessageAsync("Sorry, must select a card first");
                return;
            }
            if (MainGame!.CanPlay(newDeck) == false)
            {
                await ShowGameMessageAsync("Illegal move");
                return;
            }
            await MainGame.ProcessPlayAsync(newDeck);
        }
        public override bool CanEnableAlways()
        {
            return true;
        }
        public override bool CanEndTurn()
        {
            var thisCard = Pile1!.GetCardInfo();
            if (thisCard.Instructions == EnumInstructionList.Flip || thisCard.Instructions == EnumInstructionList.Cut)
                return false;
            if (MainGame!.SingleInfo!.MainHandList.Any(items => MainGame.CanPlay(items.Deck)))
                return false;
            return MainGame.AlreadyDrew;
        }
        public BasicGameCommand? CutDeckCommand { get; set; }
        public BasicGameCommand? FlipDeckCommand { get; set; }
        protected override void EndInit()
        {
            base.EndInit(); //must do this too.
            Deck1!.NeverAutoDisable = true; //if you want reshuffling, use this.  otherwise, comment or delete.
            CutDeckCommand = new BasicGameCommand(this, async items =>
            {
                await MainGame!.CutDeckAsync();
            }, items =>
            {
                var thisCard = Pile1!.GetCardInfo();
                return thisCard.Instructions == EnumInstructionList.Cut;
            }, this, CommandContainer!);
            FlipDeckCommand = new BasicGameCommand(this, async items =>
            {
                if (ThisData!.MultiPlayer == true)
                    await ThisNet!.SendAllAsync("flipdeck");
                await MainGame!.FlipDeckAsync();
            }, items =>
            {
                var thisCard = Pile1!.GetCardInfo();
                return thisCard.Instructions == EnumInstructionList.Flip;
            }, this, CommandContainer!);
        }
    }
}