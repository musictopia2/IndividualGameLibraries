using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommandClasses; //often times we will need commands.
using BasicGameFramework.DIContainers;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using BasicGameFramework.MainViewModels;
using BasicGameFramework.NetworkingClasses.Extensions;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
namespace AggravationCP
{
    public class AggravationViewModel : BoardDiceGameVM<EnumColorChoice, MarblePiecesCP<EnumColorChoice>,
        AggravationPlayerItem, AggravationMainGameClass, int>
    {
        internal GameBoardProcesses? GameBoard1;
        private int _TempSpace; //was going to do later but i guess its okay to do now.
        public int TempSpace
        {
            get { return _TempSpace; }
            set
            {
                if (SetProperty(ref _TempSpace, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        public AggravationViewModel(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }
        protected override bool CanEnableDice()
        {
            return false;
        }
        protected override bool CanRollDice()
        {
            return MainGame!.SaveRoot!.DiceNumber == 0;
        }

        protected override void FinishCup()
        {
            ThisCup!.HowManyDice = 1; //most board games are only one dice.  can increase if necessary
            ThisCup.Visible = true;
        }
        public BasicGameCommand<int>? SpaceCommand { get; set; }
        protected override void EndInit()
        {
            base.EndInit(); //must do this too.
            SpaceCommand = new BasicGameCommand<int>(this, async thisSpace =>
            {
                if (ThisData!.MultiPlayer)
                    await ThisNet!.SendMoveAsync(thisSpace);
                await MainGame!.MakeMoveAsync(thisSpace); //this simple i think.
            }, thisspace =>
            {
                if (MainGame!.DidChooseColors == false)
                    return false;
                return MainGame.SaveRoot!.DiceNumber > 0;
            }, this, CommandContainer!);
            GameBoard1 = MainContainer!.Resolve<GameBoardProcesses>();
        }
    }
}