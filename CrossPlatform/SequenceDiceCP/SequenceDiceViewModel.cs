using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommandClasses; //often times we will need commands.
using BasicGameFramework.DIContainers;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using BasicGameFramework.MainViewModels;
using BasicGameFramework.NetworkingClasses.Extensions;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
namespace SequenceDiceCP
{
    public class SequenceDiceViewModel : BoardDiceGameVM<EnumColorChoice, CheckerChoiceCP<EnumColorChoice>,
        SequenceDicePlayerItem, SequenceDiceMainGameClass, SpaceInfoCP>
    {
        public SequenceDiceViewModel(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }
        protected override bool CanEnableDice()
        {
            return false;
        }
        protected override bool CanRollDice()
        {
            return MainGame!.SaveRoot!.GameStatus != EnumGameStatusList.MovePiece;
        }

        protected override void FinishCup()
        {
            ThisCup!.HowManyDice = 2; //most board games are only one dice.  can increase if necessary
            ThisCup.Visible = true;
        }
        public BasicGameCommand<SpaceInfoCP>? SpaceCommand { get; set; }
        protected override void EndInit()
        {
            base.EndInit();
            SpaceCommand = new BasicGameCommand<SpaceInfoCP>(this, async thisSpace =>
            {
                if (MainGame!.SaveRoot!.GameBoard.CanMakeMove(thisSpace) == false)
                {
                    await ShowGameMessageAsync("Illegal Move");
                    return;
                }
                if (ThisData!.MultiPlayer == true)
                    await ThisNet!.SendMoveAsync(thisSpace);
                await MainGame!.MakeMoveAsync(thisSpace);
            }, items =>
            {
                if (MainGame!.DidChooseColors == false)
                    return false;
                return MainGame.SaveRoot!.GameStatus != EnumGameStatusList.RollDice;
            }, this, CommandContainer!);
        }
    }
}