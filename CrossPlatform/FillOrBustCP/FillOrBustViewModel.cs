using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommandClasses; //often times we will need commands.
using BasicGameFramework.Dice;
using BasicGameFramework.DIContainers;
using BasicGameFramework.MainViewModels;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace FillOrBustCP
{
    public class FillOrBustViewModel : BasicCardGamesVM<FillOrBustCardInformation, FillOrBustPlayerItem, FillOrBustMainGameClass>
        , IDiceEvent<SimpleDice>
    {
        private int _DiceScore;

        public int DiceScore
        {
            get { return _DiceScore; }
            set
            {
                if (SetProperty(ref _DiceScore, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _TempScore;

        public int TempScore
        {
            get { return _TempScore; }
            set
            {
                if (SetProperty(ref _TempScore, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private string _Instructions = "";

        public string Instructions
        {
            get { return _Instructions; }
            set
            {
                if (SetProperty(ref _Instructions, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        public FillOrBustViewModel(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }
        protected override bool CanEnableDeck()
        {
            return MainGame!.SaveRoot!.GameStatus == EnumGameStatusList.DrawCard ||
                MainGame.SaveRoot.GameStatus == EnumGameStatusList.ChoosePlay ||
                MainGame.SaveRoot.GameStatus == EnumGameStatusList.ChooseDraw;
        }
        protected override bool CanEnablePile1()
        {
            return false;
        }
        protected override async Task ProcessDiscardClickedAsync()
        {
            await Task.CompletedTask;
        }
        public override bool CanEnableAlways()
        {
            return true;
        }
        public override bool CanEndTurn()
        {
            if (MainGame!.SaveRoot!.GameStatus == EnumGameStatusList.EndTurn ||
                MainGame.SaveRoot.GameStatus == EnumGameStatusList.ChooseRoll ||
                MainGame.SaveRoot.GameStatus == EnumGameStatusList.ChooseDraw)
                return true;
            return false;
        }
        public BasicGameCommand? RollDiceCommand { get; set; }
        public BasicGameCommand? ChooseDiceCommand { get; set; }
        public DiceCup<SimpleDice>? ThisCup { get; set; }

        public void LoadCup(FillOrBustSaveInfo saveRoot, bool autoResume)
        {
            ThisCup = new DiceCup<SimpleDice>(saveRoot.DiceList, this);
            ThisCup.SendEnableProcesses(this, () =>
            {
                return MainGame!.SaveRoot!.GameStatus == EnumGameStatusList.ChooseDice;
            });
            if (autoResume == true)
            {
                ThisCup.CanShowDice = true;
            }
            else
                ThisCup.HowManyDice = 6;

        }
        protected override void EndInit()
        {
            base.EndInit(); //must do this too.
            Deck1!.NeverAutoDisable = true; //if you want reshuffling, use this.  otherwise, comment or delete.
            RollDiceCommand = new BasicGameCommand(this, async items =>
            {
                await MainGame!.ThisRoll!.RollDiceAsync(); //use this.
            }, items =>
            {
                if (MainGame!.SaveRoot!.GameStatus == EnumGameStatusList.RollDice ||
                MainGame.SaveRoot.GameStatus == EnumGameStatusList.ChooseRoll ||
                MainGame.SaveRoot.GameStatus == EnumGameStatusList.ChoosePlay)
                    return true;
                return false;
            }, this, CommandContainer!);
            ChooseDiceCommand = new BasicGameCommand(this, async items =>
            {
                int score = MainGame!.CalculateScore();
                if (score == 0)
                {
                    await ShowGameMessageAsync("Sorry, you must choose at least one scoring dice");
                    return;
                }
                if (MainGame.ThisData!.MultiPlayer == true)
                    await MainGame.ThisNet!.SendAllAsync("updatescore", score);
                await MainGame.AddToTempAsync(score);
            }, items =>
            {
                return MainGame!.SaveRoot!.GameStatus == EnumGameStatusList.ChooseDice;
            }, this, CommandContainer!);
        }
        public async Task DiceClicked(SimpleDice thisDice)
        {
            await MainGame!.ThisRoll!.SelectUnSelectDiceAsync(thisDice.Index); // i think
        }
    }
}