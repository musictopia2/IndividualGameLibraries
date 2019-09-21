using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.DIContainers;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.MainViewModels;
using BasicGameFramework.RegularDeckOfCards;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace OldMaidCP
{
    public class OldMaidViewModel : BasicCardGamesVM<RegularSimpleCard, OldMaidPlayerItem, OldMaidMainGameClass>
    {
        public OldMaidViewModel(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }
        protected override bool CanEnableDeck()
        {
            return false;
        }
        protected override bool CanEnablePile1()
        {
            return true;
        }
        protected override async Task ProcessDiscardClickedAsync()
        {
            var thisCol = PlayerHand1!.ListSelectedObjects();
            if (thisCol.Count != 2)
            {
                await ShowGameMessageAsync("Must select 2 cards to throw away");
                return;
            }
            if (MainGame!.IsValidMove(thisCol) == false)
            {
                await ShowGameMessageAsync("Illegal move");
            }
            await MainGame.ProcessPlayAsync(thisCol.First().Deck, thisCol.Last().Deck);
        }
        public override bool CanEnableAlways()
        {
            return true;
        }
        public override bool CanEndTurn() => MainGame!.SaveRoot!.AlreadyChoseOne || MainGame.SaveRoot.RemovePairs;
        public HandViewModel<RegularSimpleCard>? OpponentCards1;
        public OldMaidPlayerItem? OtherPlayer;
        protected override void EndInit()
        {
            base.EndInit(); //must do this too.
            Deck1!.NeverAutoDisable = false;
            OpponentCards1 = new HandViewModel<RegularSimpleCard>(this);
            OpponentCards1.AutoSelect = HandViewModel<RegularSimpleCard>.EnumAutoType.None;
            PlayerHand1!.AutoSelect = HandViewModel<RegularSimpleCard>.EnumAutoType.SelectAsMany;
            OpponentCards1.Text = "Opponent Cards";
            OpponentCards1.SendEnableProcesses(this, () => MainGame!.SaveRoot!.RemovePairs == false && MainGame.SaveRoot.AlreadyChoseOne == false);
            OpponentCards1.ObjectClickedAsync += OpponentCards1_ObjectClickedAsync;
        }
        private async Task OpponentCards1_ObjectClickedAsync(RegularSimpleCard thisObject, int index)
        {
            await MainGame!.SelectCardAsync(thisObject.Deck);
        }
    }
}