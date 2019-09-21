using BasicGameFramework.CommandClasses; //often times we will need commands.
using BasicGameFramework.Dice;
using BasicGameFramework.DIContainers;
using BasicGameFramework.MainViewModels;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace BuncoDiceGameCP
{
    public class BuncoDiceGameViewModel : SimpleGameVM, IDiceEvent<SimpleDice>
    {
        public DiceCup<SimpleDice>? ThisCup; //needs to be public so it can hook into ui.

        #region Properties
        private bool _CanEndTurn;
        public bool CanEndTurn
        {
            get
            {
                return _CanEndTurn;
            }

            set
            {
                if (SetProperty(ref _CanEndTurn, value) == true)
                {
                }
            }
        }

        private bool _AlreadyReceivedBunco;
        public bool AlreadyReceivedBunco
        {
            get
            {
                return _AlreadyReceivedBunco;
            }

            set
            {
                if (SetProperty(ref _AlreadyReceivedBunco, value) == true)
                {
                }
            }
        }

        #endregion

        #region Commands

        public BasicGameCommand? RollCommand { get; set; }
        public BasicGameCommand? BuncoCommand { get; set; }
        public BasicGameCommand? Has21Command { get; set; }
        public BasicGameCommand? EndTurnCommand { get; set; }

        #endregion

        public BuncoDiceGameMainGameClass? MainGame; //i think
        private BuncoDiceGameSaveInfo? _saveRoot;
        public BuncoDiceGameViewModel(ISimpleUI tempUI, IGamePackageResolver tempC) : base(tempUI, tempC) { } //if you have other interface definitions, add here

        internal void LoadCup(BuncoDiceGameSaveInfo saveRoot, bool autoResume)
        {
            if (ThisCup != null) //oppposite of what i intended.
                return;
            ThisCup = new DiceCup<SimpleDice>(saveRoot.DiceList, this);
            ThisCup.HowManyDice = 3;
            ThisCup.ShowDiceListAlways = true;
            ThisCup.MainContainer = MainContainer;
            _saveRoot = saveRoot;
            if (autoResume == false)
                ThisCup.ClearDice(); //was needed for mobile.
            else
            {
                ThisCup.CanShowDice = true; //i think
            }
        }
        private bool _isNewGame = true;

        public override async Task StartNewGameAsync()
        {
            NewGameVisible = false; //i think
            if (_isNewGame == true)
                await MainGame!.StartGameAsync();
            else
                await MainGame!.ProcessNewRoundAsync();
            _isNewGame = false; //at the end, no longer new game.
        }

        public override void Init()
        {
            MainGame = MainContainer!.Resolve<BuncoDiceGameMainGameClass>();
            RollCommand = new BasicGameCommand(this, async items =>
            {
                CommandContainer!.ManuelFinish = false;
                await HumanRollAsync();
            }, Items =>
            {
                return !CanEndTurn;
            }, this, CommandContainer!);
            BuncoCommand = new BasicGameCommand(this, async items =>
            {
                await HumanBuncoAsync();
            }, Items =>
            {
                if (CanEndTurn == true)
                    return false;
                if (AlreadyReceivedBunco == true)
                    return false;
                return _saveRoot!.HasRolled;
            }, this, CommandContainer!);
            Has21Command = new BasicGameCommand(this, async items =>
            {
                await Human21Async();
                CommandContainer!.IsExecuting = false;
            }, Items =>
            {
                if (CanEndTurn == true)
                    return false;
                if (_saveRoot!.HasRolled == false)
                    return false;
                if (MainGame.CurrentPlayer!.Table == 1)
                    return true;
                return false;
            }, this, CommandContainer!);
            EndTurnCommand = new BasicGameCommand(this, async items =>
            {
                await MainGame.EndTurnAsync();
            }, Items =>
            {
                return CanEndTurn;
            }, this, CommandContainer!);
            CommandContainer!.IsExecuting = true; //has to be put that its executing to begin with.
            CommandContainer.ManuelFinish = true; //has to manually be done.
        }
        public Task DiceClicked(SimpleDice thisDice)
        {
            throw new NotImplementedException("Should not have clicked any dice.  This would neven been ran");
        }

        # region Processes For Commmands
        private async Task HumanRollAsync()
        {
            await MainGame!.RollDiceAsync();
            int whatScore;
            whatScore = MainGame.ScoreRoll();
            if (whatScore == 0)
            {
                CanEndTurn = true;
                return;
            }
            MainGame.UpdateScores(whatScore);
            _saveRoot!.HasRolled = true;
        }
        private async Task Human21Async()
        {
            CommandContainer!.ManuelFinish = false;
            if (MainGame!.CurrentPlayer!.Points < 21)
            {
                await ThisMessage.ShowMessageBox("Sorry, you do not have 21 points.  Therefore, yuo cannot end the round");
                return;
            }
            if (MainGame.CurrentPlayer.Table > 1)
            {
                await ThisMessage.ShowMessageBox("Sorry, you cannot end this round because you are not hosting this round.");
                return;
            }
            await MainGame.FinishRoundAsync();
        }
        private async Task HumanBuncoAsync()
        {
            CommandContainer!.ManuelFinish = false;
            if (MainGame!.DidHaveBunco() == false)
            {
                await ThisMessage.ShowMessageBox("Sorry, there is no bunco here");
                return;
            }
            MainGame.UpdateScores(16);
            MainGame.ReceivedBunco();
            _saveRoot!.ThisStats.YourPoints = MainGame.CurrentPlayer!.Points;
            _saveRoot.ThisStats.Buncos = MainGame.CurrentPlayer.Buncos;
            if (MainGame.CurrentPlayer.Table == 1)
            {
                await MainGame.FinishRoundAsync(); // because a bunco has been received and you are hosting.  if you are not hosting, then round does not end right away.
                return;
            }
            AlreadyReceivedBunco = true;
        }

        #endregion
    }
}