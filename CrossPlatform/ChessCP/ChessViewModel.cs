using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommandClasses; //often times we will need commands.
using BasicGameFramework.DIContainers;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using BasicGameFramework.MainViewModels;
using BasicGameFramework.NetworkingClasses.Extensions;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
namespace ChessCP
{
    public class ChessViewModel : SimpleBoardGameVM<EnumColorChoice, CheckerChoiceCP<EnumColorChoice>, ChessPlayerItem, ChessMainGameClass, int>
    {
        public ChessViewModel(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }
        public BasicGameCommand<int>? SpaceCommand { get; set; }
        public BasicGameCommand? TieCommand { get; set; }
        public BasicGameCommand? UndoMovesCommand { get; set; }
        public GameBoardProcesses? GameBoard1;

        public override bool CanEndTurn()
        {
            return MainGame!.SaveRoot!.GameStatus == EnumGameStatus.PossibleTie || MainGame.SaveRoot.GameStatus == EnumGameStatus.EndingTurn;
        }
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
                await GameBoard1.MakeMoveAsync(space);
            }, items =>
            {
                return MainGame!.SaveRoot!.GameStatus == EnumGameStatus.None && MainGame.DidChooseColors == true;
            }, this, CommandContainer!);
            UndoMovesCommand = new BasicGameCommand(this, async items =>
            {
                if (ThisData!.MultiPlayer == true)
                    await ThisNet!.SendAllAsync("undomove");
                await GameBoard1!.UndoAllMovesAsync();
            }, items =>
            {
                return MainGame!.SaveRoot!.GameStatus == EnumGameStatus.EndingTurn;
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
                if (MainGame.SaveRoot!.SpaceHighlighted > 0)
                    return false;
                return MainGame.SaveRoot.GameStatus != EnumGameStatus.EndingTurn;
            }, this, CommandContainer!);

            GameBoard1 = MainContainer!.Resolve<GameBoardProcesses>();
        }
    }
}