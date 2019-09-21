using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.Dice;
using BasicGameFramework.DIContainers;
using BasicGameFramework.MainViewModels;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
namespace A21DiceGameCP
{
    public class A21DiceGameViewModel : DiceGamesVM<SimpleDice, A21DiceGamePlayerItem, A21DiceGameMainGameClass>
    {
        public A21DiceGameViewModel(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }
        protected override bool CanEnableDice()
        {
            return false; //if you can enable dice, change the routine.
        }
        public override bool CanEndTurn()
        {
            return MainGame!.SingleInfo!.NumberOfRolls > 0;
        }
        protected override bool CanRollDice()
        {
            return base.CanRollDice(); //anything you need to figure out if you can roll is here.
        }
        protected override void FinishCup()
        {
            ThisCup!.HowManyDice = 1; //you specify how many dice here.
            ThisCup.Visible = true; //i think.
        }
    }
}