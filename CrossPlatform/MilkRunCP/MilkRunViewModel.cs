using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.DIContainers;
using BasicGameFramework.MainViewModels;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace MilkRunCP
{
    public class MilkRunViewModel : BasicCardGamesVM<MilkRunCardInformation, MilkRunPlayerItem, MilkRunMainGameClass>
    {
        public MilkRunViewModel(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }
        protected override bool CanEnableDeck()
        {
            return MainGame!.SaveRoot!.CardsDrawn != 2;
        }
        protected override bool CanEnablePile1()
        {
            return true;
        }

        protected override async Task ProcessDiscardClickedAsync()
        {
            int newDeck = PlayerHand1!.ObjectSelected();
            if (newDeck > 0)
            {
                if (MainGame!.SaveRoot!.CardsDrawn < 2)
                {
                    await ShowGameMessageAsync("Sorry, must draw the 2 cards first before discarding");
                    return;
                }
                if (newDeck == MainGame.PreviousCard)
                {
                    await ShowGameMessageAsync("Cannot discard the same card you picked up");
                    return;
                }
                await MainGame.SendDiscardMessageAsync(newDeck);
                await MainGame.DiscardAsync(newDeck);
                return;
            }
            if (MainGame!.SaveRoot!.DrawnFromDiscard == true)
            {
                await ShowGameMessageAsync("Sorry, you already picked up one card from discard.  Cannot pickup another one.  If you want to discard, then choose a card to discard");
                return;
            }
            if (Pile1!.PileEmpty())
            {
                await ShowGameMessageAsync("Sorry, there is no card to pickup from the discard.");
                return;
            }
            await MainGame.PickupFromDiscardAsync();
        }
        public override bool CanEnableAlways()
        {
            return true;
        }
        protected override void EndInit()
        {
            base.EndInit(); //must do this too.
            Deck1!.NeverAutoDisable = true; //if you want reshuffling, use this.  otherwise, comment or delete.
            PlayerHand1!.Maximum = 8;
        }
    }
}