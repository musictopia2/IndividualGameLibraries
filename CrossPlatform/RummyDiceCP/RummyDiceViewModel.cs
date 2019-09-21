using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommandClasses; //often times we will need commands.
using BasicGameFramework.DIContainers;
using BasicGameFramework.MainViewModels;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
namespace RummyDiceCP
{
    public class RummyDiceViewModel : BasicMultiplayerVM<RummyDicePlayerItem, RummyDiceMainGameClass>
    {
        public RummyDiceViewModel(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }
        private int _RollNumber;

        public int RollNumber
        {
            get { return _RollNumber; }
            set
            {
                if (SetProperty(ref _RollNumber, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private string _CurrentPhase = "None";

        public string CurrentPhase
        {
            get { return _CurrentPhase; }
            set
            {
                if (SetProperty(ref _CurrentPhase, value))
                {
                    //can decide what to do when property changes
                }

            }
        }

        private int _Score;

        public int Score
        {
            get { return _Score; }
            set
            {
                if (SetProperty(ref _Score, value))
                {
                    //can decide what to do when property changes
                }

            }
        }

        public BasicGameCommand? BoardCommand { get; set; }
        public BasicGameCommand? RollCommand { get; set; }
        public BasicGameCommand? CheckCommand { get; set; }
        protected override void EndInit()
        {
            base.EndInit(); //must do this too.
            BoardCommand = new BasicGameCommand(this, async items =>
            {
                if (ThisData!.MultiPlayer == true)
                    await ThisNet!.SendAllAsync("boardclicked");
                await MainGame!.BoardProcessAsync();
            }, Items => true, this, CommandContainer!);
            RollCommand = new BasicGameCommand(this, async items =>
            {
                if (MainGame!.MainBoard1!.HasSelectedDice() == true)
                {
                    await ShowGameMessageAsync("Need to either unselect the dice or use them.");
                    return;
                }
                //CommandContainer.ManuelFinish = true;//to test.
                await MainGame.RollDiceAsync();
            }, items =>
            {
                return RollNumber < 4; //a converter will reduce by one.
            }, this, CommandContainer!);
            CheckCommand = new BasicGameCommand(this, async items =>
            {
                if (ThisData!.MultiPlayer == true)
                    await ThisNet!.SendAllAsync("calculate");
                await MainGame!.DoCalculateAsync();
            }, items => true, this, CommandContainer!);
        }
    }
}