using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommandClasses; //often times we will need commands.
using BasicGameFramework.DIContainers;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using BasicGameFramework.MainViewModels;
using BasicGameFramework.NetworkingClasses.Extensions;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
namespace CheckersCP
{
    public class CheckersViewModel : SimpleBoardGameVM<EnumColorChoice, CheckerChoiceCP<EnumColorChoice>, CheckersPlayerItem, CheckersMainGameClass, int>
    {
        public CheckersViewModel(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }
        public override bool CanEndTurn()
        {
            return MainGame!.SaveRoot!.GameStatus == EnumGameStatus.PossibleTie;
        }
        public GameBoardProcesses? GameBoard1;

        public BasicGameCommand<int>? SpaceCommand { get; set; }
        public BasicGameCommand? TieCommand { get; set; }
        protected override void EndInit()
        {
            base.EndInit(); //must do this too.
            SpaceCommand = new BasicGameCommand<int>(this, async space =>
            {
                if (GameBoard1!.IsValidMove(space) == false)
                    return;
                if (ThisData!.MultiPlayer == true)
                    await ThisNet!.SendMoveAsync(GameBoardGraphicsCP.GetRealIndex(space, true));
                CommandContainer!.ManuelFinish = true;
                await GameBoard1!.MakeMoveAsync(space);
            }, items =>
            {
                return MainGame!.SaveRoot!.GameStatus == EnumGameStatus.None && MainGame.DidChooseColors == true;
            }, this, CommandContainer!);

            TieCommand = new BasicGameCommand(this, async items =>
            {
                if (ThisData!.MultiPlayer == true)
                    await ThisNet!.SendAllAsync("possibletie");
                CommandContainer!.ManuelFinish = true; //i think this is needed.
                await MainGame!.ProcessTieAsync();
            }, items =>
            {
                if (MainGame!.DidChooseColors == false)
                    return false;
                if (MainGame!.SaveRoot!.SpaceHighlighted > 0)
                    return false;
                return MainGame.SaveRoot.ForcedToMove == false;
            }, this, CommandContainer!);

            GameBoard1 = MainContainer!.Resolve<GameBoardProcesses>();
        }
    }
}