using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommandClasses; //often times we will need commands.
using BasicGameFramework.DIContainers;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using BasicGameFramework.MainViewModels;
using BasicGameFramework.NetworkingClasses.Extensions;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
namespace PassOutDiceGameCP
{
    public class PassOutDiceGameViewModel : BoardDiceGameVM<EnumColorChoice, CheckerChoiceCP<EnumColorChoice>,
        PassOutDiceGamePlayerItem, PassOutDiceGameMainGameClass, int>
    {
        public PassOutDiceGameViewModel(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }
        protected override bool CanEnableDice()
        {
            return false;
        }
        protected override bool CanRollDice()
        {
            return false;
        }
        protected override void FinishCup()
        {
            ThisCup!.HowManyDice = 2; //most board games are only one dice.  can increase if necessary
            ThisCup.Visible = true;
        }
        public BasicGameCommand<int>? SpaceCommand { get; set; }
        public GameBoardProcesses? GameBoard1;
        protected override void EndInit()
        {
            base.EndInit(); //must do this too.
            SpaceCommand = new BasicGameCommand<int>(this, async items =>
            {
                if (GameBoard1!.IsValidMove(items) == false)
                {
                    await ShowGameMessageAsync("Illegal Move");
                    return;
                }
                if (ThisData!.MultiPlayer == true)
                    await ThisNet!.SendMoveAsync(items);
                await MainGame!.MakeMoveAsync(items);
            }, items =>
            {
                return true;
            }, this, CommandContainer!);
            GameBoard1 = MainContainer!.Resolve<GameBoardProcesses>(); //taking a chance here. hopefully it pays off.
        }
    }
}