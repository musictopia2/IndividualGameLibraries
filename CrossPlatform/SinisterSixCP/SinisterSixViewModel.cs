using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommandClasses; //often times we will need commands.
using BasicGameFramework.Dice;
using BasicGameFramework.DIContainers;
using BasicGameFramework.MainViewModels;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System.Linq;
namespace SinisterSixCP
{
    public class SinisterSixViewModel : DiceGamesVM<EightSidedDice, SinisterSixPlayerItem, SinisterSixMainGameClass>
    {
        private int _MaxRolls;

        public int MaxRolls
        {
            get { return _MaxRolls; }
            set
            {
                if (SetProperty(ref _MaxRolls, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        public SinisterSixViewModel(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }
        protected override void EndInit()
        {
            base.EndInit(); //must do this too.
            RemoveDiceCommmand = new BasicGameCommand(this, async items =>
            {
                if (CanRemoveSelectedDice() == false)
                {
                    await ShowGameMessageAsync("Cannot remove dice that does not equal 6");
                    return;
                };
                await MainGame!.RemoveSelectedDiceAsync();
            }, items => CanEndTurn(), this, CommandContainer!);
        }
        protected override bool CanEnableDice()
        {
            return true; //if you can enable dice, change the routine.
        }
        private bool CanRemoveSelectedDice()
        {
            var thisList = ThisCup!.DiceList.GetSelectedItems();
            return thisList.Sum(Items => Items.Value) == 6;
        }
        public override bool CanEndTurn()
        {
            return RollNumber > 0;
        }
        protected override bool CanRollDice()
        {
            return MainGame!.SaveRoot!.RollNumber <= MainGame.SaveRoot.MaxRolls;
        }
        public BasicGameCommand? RemoveDiceCommmand { get; set; }
        protected override void FinishCup()
        {
            ThisCup!.HowManyDice = 6;
            ThisCup.ShowHold = false;
            ThisCup.Visible = true; //i think.
        }
    }
}