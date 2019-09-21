using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommandClasses; //often times we will need commands.
using BasicGameFramework.Dice;
using BasicGameFramework.DIContainers;
using BasicGameFramework.MainViewModels;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
namespace RollEmCP
{
    public class RollEmViewModel : DiceGamesVM<SimpleDice, RollEmPlayerItem, RollEmMainGameClass>
    {
        private int _Round;
        public int Round
        {
            get { return _Round; }
            set
            {
                if (SetProperty(ref _Round, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        public RollEmViewModel(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }
        public BasicGameCommand<int>? SpaceCommand { get; set; }
        public GameBoardProcesses? GameBoard1;
        protected override void EndInit()
        {
            base.EndInit(); //must do this too.
            GameBoard1 = MainContainer!.Resolve<GameBoardProcesses>(); //taking a chance here. hopefully it pays off.
            SpaceCommand = new BasicGameCommand<int>(this, async index =>
            {
                if (GameBoard1.CanMakeMove(index) == false)
                {
                    if (GameBoard1.HadRecent == true)
                    {
                        if (ThisData!.MultiPlayer == true)
                            await ThisNet!.SendAllAsync("clearrecent");
                        await ShowGameMessageAsync("Illegal Move");
                        GameBoard1.ClearRecent(true);
                        await MainGame!.ContinueTurnAsync();
                    }
                    return;
                }
                await MainGame!.MakeMoveAsync(index);
            }, items =>
            {
                return CanEndTurn();
            }, this, CommandContainer!);
            CommandContainer!.ExecutingChanged += CommandContainer_ExecutingChanged;
        }
        private void CommandContainer_ExecutingChanged()
        {
            MainGame!.RepaintBoard();
        }
        protected override bool CanEnableDice()
        {
            return false; //if you can enable dice, change the routine.
        }
        public override bool CanEndTurn()
        {
            return MainGame!.SaveRoot!.GameStatus != EnumStatusList.NeedRoll;
        }
        protected override bool CanRollDice()
        {
            return MainGame!.SaveRoot!.GameStatus == EnumStatusList.NeedRoll;
        }
        protected override void FinishCup()
        {
            ThisCup!.HowManyDice = 2; //you specify how many dice here.
            ThisCup.Visible = true; //i think.
        }
    }
}