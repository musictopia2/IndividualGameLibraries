using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.Dice;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.MainViewModels;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.Exceptions;
using System;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using YachtRaceCP.Data;
using YachtRaceCP.Logic;
namespace YachtRaceCP.ViewModels
{
    [InstanceGame]
    public class YachtRaceMainViewModel : DiceGamesVM<SimpleDice>
    {
        private readonly YachtRaceMainGameClass _mainGame; //if we don't need, delete.
        private readonly YachtRaceVMData _model;
        public YachtRaceMainViewModel(CommandContainer commandContainer,
            YachtRaceMainGameClass mainGame,
            YachtRaceVMData viewModel,
            BasicData basicData,
            TestOptions test,
            IGamePackageResolver resolver,
            IStandardRollProcesses roller
            )
            : base(commandContainer, mainGame, viewModel, basicData, test, resolver, roller)
        {
            _mainGame = mainGame;
            _model = viewModel;
            CommandContainer.ExecutingChanged += CommandContainer_ExecutingChanged;
        }

        private void CommandContainer_ExecutingChanged()
        {
            if (_mainGame.HasRolled && CommandContainer.IsExecuting)
            {
                try
                {
                    _model.GameTimer.PauseTimer();
                }
                catch (Exception)
                {

                    throw;
                }
            }
            else if (CommandContainer.IsExecuting == false && _mainGame.HasRolled)
            {
                _model.GameTimer.ContinueTimer();
            }
        }

        //anything else needed is here.
        protected override bool CanEnableDice()
        {
            return true; //if you can enable dice, change the routine.
        }
        public override bool CanEndTurn()
        {
            return false; //if you can't or has extras, do here.
        }
        public override bool CanRollDice()
        {
            return true;
        }
        private string _errorMessage = "";

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                if (SetProperty(ref _errorMessage, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        public bool CanFiveKind => _mainGame.HasRolled;
        [Command(EnumCommandCategory.Game)]
        public async Task FiveKindAsync()
        {
            if (_mainGame!.HasYahtzee() == false)
            {
                ErrorMessage = "You do not have 5 of a kind";
                await Task.Delay(200); //this time, needs this.
                ErrorMessage = "";
                return;
            }
            _model.GameTimer.StopTimer();
            float howLong = _model.GameTimer.TimeElapsed;
            if (howLong == 0)
                throw new BasicBlankException("Time cannot be 0");
            if (_mainGame.BasicData!.MultiPlayer == true)
            {
                await _mainGame.Network!.SendAllAsync("fivekind", howLong);
            }
            await _mainGame.ProcessFiveKindAsync(howLong);
        }

    }
}