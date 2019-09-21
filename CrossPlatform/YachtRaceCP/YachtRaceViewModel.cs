using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommandClasses; //often times we will need commands.
using BasicGameFramework.Dice;
using BasicGameFramework.DIContainers;
using BasicGameFramework.MainViewModels;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.Misc;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace YachtRaceCP
{
    public class YachtRaceViewModel : DiceGamesVM<SimpleDice, YachtRacePlayerItem, YachtRaceMainGameClass>
    {
        internal Timer? GameTimer;
        public YachtRaceViewModel(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }
        private string _ErrorMessage = "";

        public string ErrorMessage
        {
            get { return _ErrorMessage; }
            set
            {
                if (SetProperty(ref _ErrorMessage, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        public BasicGameCommand? FiveKindCommand { get; set; }
        protected override void EndInit()
        {
            base.EndInit(); //must do this too.
            CommandContainer!.ExecutingChanged += CommandContainer_ExecutingChanged;
            GameTimer = new Timer();
            FiveKindCommand = new BasicGameCommand(this, async items =>
            {
                if (MainGame!.HasYahtzee() == false)
                {
                    ErrorMessage = "You do not have 5 of a kind";
                    await Task.Delay(200); //this time, needs this.
                    ErrorMessage = "";
                    return;
                }
                GameTimer.StopTimer();
                float howLong = GameTimer.TimeElapsed;
                if (howLong == 0)
                    throw new BasicBlankException("Time cannot be 0");
                if (ThisData!.MultiPlayer == true)
                    await ThisNet!.SendAllAsync("fivekind", howLong);
                await MainGame.ProcessFiveKindAsync(howLong);
            }, items => MainGame!.HasRolled, this, CommandContainer);
        }
        protected override bool CanEnableDice()
        {
            return true; //if you can enable dice, change the routine.
        }
        public override bool CanEndTurn()
        {
            return false; //if you can't or has extras, do here.
        }
        protected override bool CanRollDice()
        {
            return true;
        }
        protected override void FinishCup()
        {
            ThisCup!.HowManyDice = 5; //you specify how many dice here.
            ThisCup.Visible = true; //i think.
            ThisCup.ShowHold = true;
        }
        private void CommandContainer_ExecutingChanged()
        {
            if (MainGame!.HasRolled == true && CommandContainer!.IsExecuting == true)
            {
                try
                {
                    GameTimer!.PauseTimer();
                }
                catch
                {

                }
            }
            else if (CommandContainer!.IsExecuting == false && MainGame.HasRolled == true)
                GameTimer!.ContinueTimer();
        }
    }
}