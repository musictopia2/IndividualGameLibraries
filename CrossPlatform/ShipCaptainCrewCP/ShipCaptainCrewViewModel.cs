using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.Dice;
using BasicGameFramework.DIContainers;
using BasicGameFramework.MainViewModels;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
namespace ShipCaptainCrewCP
{
    public class ShipCaptainCrewViewModel : DiceGamesVM<SimpleDice, ShipCaptainCrewPlayerItem, ShipCaptainCrewMainGameClass>
    {
        public ShipCaptainCrewViewModel(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }
        protected override void EndInit()
        {
            base.EndInit(); //must do this too.
        }
        protected override bool CanEnableDice()
        {
            return true; //if you can enable dice, change the routine.
        }
        public override bool CanEndTurn()
        {
            return false; //if you can't or has extras, do here.
        }
        protected override bool CanRollDice()
        {
            return true;
        }
        protected override void FinishCup()
        {
            ThisCup!.HowManyDice = 5; //you specify how many dice here.
            ThisCup.Visible = true; //i think.
            ThisCup.ShowHold = true;
        }
    }
}