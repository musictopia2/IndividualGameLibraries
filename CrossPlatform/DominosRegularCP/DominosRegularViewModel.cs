using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.DIContainers;
using BasicGameFramework.Dominos;
using BasicGameFramework.MainViewModels;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
namespace DominosRegularCP
{
    public class DominosRegularViewModel : DominoGamesVM<SimpleDominoInfo, DominosRegularPlayerItem, DominosRegularMainGameClass>
    {
        public GameBoardCP? GameBoard1;
        public DominosRegularViewModel(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }
        protected override bool CanEnableBoneYard()
        {
            return true;
        }
        public override bool CanEndTurn()
        {
            if (BoneYard!.HasBone())
            {
                return BoneYard.HasDrawn();
            }
            return true;
        }
        protected override void EndInit()
        {
            base.EndInit(); //best to keep this to be safe.
            GameBoard1 = new GameBoardCP(this);
            PlayerHand1!.Maximum = 8;
            PlayerHand1.IgnoreMaxRules = true; //so after 8, scrollbars.
        }
    }
}