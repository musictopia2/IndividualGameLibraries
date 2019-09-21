using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommandClasses; //often times we will need commands.
using BasicGameFramework.DIContainers;
using BasicGameFramework.MainViewModels;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
namespace CountdownCP
{
    public class CountdownViewModel : DiceGamesVM<CountdownDice, CountdownPlayerItem, CountdownMainGameClass>
    {
        public static bool ShowHints { get; set; } //decided to place under the view model.  since i can't do in globals anymore. make it static.  i can risk it.
        public CountdownViewModel(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }

        protected override bool CanEnableDice()
        {
            return false; //if you can enable dice, change the routine.
        }
        public override bool CanEndTurn()
        {
            return true; //if you can't or has extras, do here.
        }
        protected override bool CanRollDice()
        {
            return false;
        }
        protected override void FinishCup()
        {
            ThisCup!.HowManyDice = 2; //you specify how many dice here.
            ThisCup.Visible = true; //i think.
        }
        private int _Round; //this is needed because the game has to end at some point no matter what even if tie.

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
        public BasicGameCommand<SimpleNumber>? SpaceCommand { get; set; }
        public BasicGameCommand? HintCommand { get; set; }
        protected override void EndInit()
        {
            base.EndInit(); //must do this too.
            SpaceCommand = new BasicGameCommand<SimpleNumber>(this, async thisNumber =>
            {
                if (ThisData!.MultiPlayer == true)
                    await ThisNet!.SendAllAsync("choosenumber", thisNumber.Value);
                await MainGame!.ProcessChosenNumberAsync(thisNumber);
            }, items =>
            {
                return true;
            }, this, CommandContainer!);
            HintCommand = new BasicGameCommand(this, items =>
            {
                //has to repaint some board.  refer to candyland for ideas  this happens alot.
                ShowHints = true; //other players don't have to know about this.
                ThisE!.RepaintBoard();
            }, items => true, this, CommandContainer!);
            CommandContainer!.ExecutingChanged += CommandContainer_ExecutingChanged;
        }
        private void CommandContainer_ExecutingChanged()
        {
            if (ThisCup == null)
                return;
            if (ThisCup.Visible == false)
                return;
            ThisE!.RepaintBoard(); //because what you see can be different.
        }
    }
}