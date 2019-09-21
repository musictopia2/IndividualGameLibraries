using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommandClasses; //often times we will need commands.
using BasicGameFramework.DIContainers;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using BasicGameFramework.MainViewModels;
using BasicGameFramework.NetworkingClasses.Extensions;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System.Linq;
namespace BackgammonCP
{
    public class BackgammonViewModel : BoardDiceGameVM<EnumColorChoice, CheckerChoiceCP<EnumColorChoice>,
        BackgammonPlayerItem, BackgammonMainGameClass, int>
    {
        public GameBoardProcesses? GameBoard1;
        private int _MovesMade;
        public int MovesMade
        {
            get { return _MovesMade; }
            set
            {
                if (SetProperty(ref _MovesMade, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private string _LastStatus = "";
        public string LastStatus
        {
            get { return _LastStatus; }
            set
            {
                if (SetProperty(ref _LastStatus, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        public override bool CanEndTurn()
        {
            return MainGame!.SaveRoot!.GameStatus == EnumGameStatus.EndingTurn;
        }
        public BackgammonViewModel(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }
        protected override bool CanEnableDice()
        {
            return false;
        }
        protected override bool CanRollDice()
        {
            return false; //does it automatically;
        }
        protected override void FinishCup()
        {
            ThisCup!.HowManyDice = 2; //most board games are only one dice.  can increase if necessary
            ThisCup.Visible = true;
        }
        public BasicGameCommand<int>? SpaceCommand { get; set; }
        public BasicGameCommand? UndoMovesCommand { get; set; }
        protected override void EndInit()
        {
            base.EndInit(); //must do this too.
            UndoMovesCommand = new BasicGameCommand(this, async items =>
            {
                if (ThisData!.MultiPlayer)
                    await ThisNet!.SendAllAsync("undomove");
                await GameBoard1!.UndoAllMovesAsync();
            }, items =>
            {
                if (MainGame!.SaveRoot!.SpaceHighlighted > -1 || MainGame.DidChooseColors == false)
                    return false;
                return MainGame.SaveRoot.GameStatus == EnumGameStatus.EndingTurn || MainGame.SaveRoot.MadeAtLeastOneMove;
            }, this, CommandContainer!);
            SpaceCommand = new BasicGameCommand<int>(this, async space =>
            {
                if (GameBoard1!.IsValidMove(space) == false)
                    return;
                if (ThisData!.MultiPlayer == true)
                    await ThisNet!.SendMoveAsync(GameBoard1.GetReversedID(space));
                await GameBoard1.MakeMoveAsync(space);
            }, items =>
            {
                if (MainGame!.ThisGlobal!.MoveInProgress)
                    return false;
                if (MainGame.ThisGlobal!.MoveList.Count == 0)
                    return false;
                if (MainGame.SaveRoot!.GameStatus == EnumGameStatus.EndingTurn || MainGame.DidChooseColors == false)
                    return false;
                return true;
            }, this, CommandContainer!);
            GameBoard1 = MainContainer!.Resolve<GameBoardProcesses>();
        }
    }
}