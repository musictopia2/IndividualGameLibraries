using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.DIContainers;
using BasicGameFramework.MainViewModels;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace FlinchCP
{
    public class FlinchViewModel : BasicCardGamesVM<FlinchCardInformation, FlinchPlayerItem, FlinchMainGameClass>
    {
        public FlinchViewModel(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }
        private int _CardsToShuffle;

        public int CardsToShuffle
        {
            get { return _CardsToShuffle; }
            set
            {
                if (SetProperty(ref _CardsToShuffle, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        protected override bool CanEnableDeck()
        {
            return false; //otherwise, can't compile.
        }

        protected override bool CanEnablePile1()
        {
            return false; //otherwise, can't compile.
        }
        protected override async Task BeforeUnselectCardFromHandAsync()
        {
            MainGame!.UnselectAllCards();
            await Task.CompletedTask;
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
        public override bool CanEndTurn()
        {
            return MainGame!.SaveRoot!.GameStatus == EnumStatusList.FirstOne && MainGame.SaveRoot.PlayerFound == 0;
        }
        protected override void EndInit()
        {
            base.EndInit(); //must do this too.
            Deck1!.NeverAutoDisable = true; //if you want reshuffling, use this.  otherwise, comment or delete.
            PlayerHand1!.Maximum = 5;
        }
    }
}