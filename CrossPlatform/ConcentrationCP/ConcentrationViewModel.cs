using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.DIContainers;
using BasicGameFramework.MainViewModels;
using BasicGameFramework.RegularDeckOfCards;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace ConcentrationCP
{
    public class ConcentrationViewModel : BasicCardGamesVM<RegularSimpleCard, ConcentrationPlayerItem, ConcentrationMainGameClass>
    {
        public ConcentrationViewModel(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }
        public GameBoardClass? GameBoard1;
        protected override bool CanEnableDeck()
        {
            return false;
        }
        protected override bool CanEnablePile1()
        {
            return false;
        }
        protected override async Task ProcessDiscardClickedAsync()
        {
            //if we have anything, will be here.
            await Task.CompletedTask;
        }
        public override bool CanEnableAlways()
        {
            return true;
        }
        protected override void EndInit()
        {
            base.EndInit(); //must do this too.
            Deck1!.NeverAutoDisable = true; //if you want reshuffling, use this.  otherwise, comment or delete.
            GameBoard1 = new GameBoardClass(this); //by this time, you already did the main game class.  if i am wrong, will get overflow issue.
            GameBoard1.PileClickedAsync += GameBoard1_PileClickedAsync;
        }
        private async Task GameBoard1_PileClickedAsync(int Index, BasicGameFramework.MultiplePilesViewModels.BasicPileInfo<RegularSimpleCard> ThisPile)
        {
            await MainGame!.SelectCardAsync(ThisPile.ThisObject.Deck);
        }
    }
}