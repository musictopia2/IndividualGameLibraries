using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommandClasses; //often times we will need commands.
using BasicGameFramework.Dice;
using BasicGameFramework.DIContainers;
using BasicGameFramework.MainViewModels;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace SnakesAndLaddersCP
{
    public class SnakesAndLaddersViewModel : BasicMultiplayerVM<SnakesAndLaddersPlayerItem, SnakesAndLaddersMainGameClass>
        , IDiceEvent<SimpleDice>
    {
        public DiceCup<SimpleDice>? ThisCup { get; set; }
        public SnakesAndLaddersViewModel(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }
        public BasicGameCommand? RollDiceCommand { get; set; }
        public BasicGameCommand<int>? SpaceClickCommand { get; set; }
        public void LoadCup(SnakesAndLaddersSaveInfo saveRoot, bool autoResume)
        {
            ThisCup = new DiceCup<SimpleDice>(saveRoot.DiceList, this);
            ThisCup.SendEnableProcesses(this, () =>
            {
                return false; //because you can't click the dice.
            });
            ThisCup.HowManyDice = 1;
            if (autoResume == true && saveRoot.HasRolled == true)
            {
                ThisCup.CanShowDice = true;
                ThisCup.Visible = true;
            }
        }
        protected override void EndInit()
        {
            base.EndInit(); //must do this too.
            RollDiceCommand = new BasicGameCommand(this, async items =>
            {
                await MainGame!.ThisRoll!.RollDiceAsync(); //use this.
            }, items =>
            {
                return !MainGame!.SaveRoot!.HasRolled;
            }, this, CommandContainer!);
            SpaceClickCommand = new BasicGameCommand<int>(this, async numberClicked =>
            {
                if (MainGame!.GameBoard1!.IsValidMove(numberClicked) == false)
                {
                    await ShowGameMessageAsync("Illegal Move");
                    return;
                }
                await MainGame.MakeMoveAsync(numberClicked);
            }, numberClicked =>
            {
                if (numberClicked == 0)
                    return false;
                return MainGame!.SaveRoot!.HasRolled;
            }, this, CommandContainer!);
        }
        public Task DiceClicked(SimpleDice thisDice)
        {
            throw new BasicBlankException("You should not have been allowed to click the dice.  Could ignore.  Rethink");
        }
    }
}