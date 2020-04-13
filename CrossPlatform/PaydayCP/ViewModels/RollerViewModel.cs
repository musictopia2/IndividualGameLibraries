using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.Dice;
using BasicGameFrameworkLibrary.ViewModelInterfaces;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using PaydayCP.Data;
using PaydayCP.Logic;
using System.Threading.Tasks;

namespace PaydayCP.ViewModels
{
    [InstanceGame]
    public class RollerViewModel : Screen, IBlankGameVM
    {
        private readonly PaydayMainGameClass _mainGame;
        private readonly StandardRollProcesses<SimpleDice, PaydayPlayerItem> _roller;

        //this is when we can roll
        public RollerViewModel(CommandContainer commandContainer, PaydayMainGameClass mainGame, StandardRollProcesses<SimpleDice, PaydayPlayerItem> roller)
        {
            CommandContainer = commandContainer;
            _mainGame = mainGame;
            _roller = roller;
        }

        public CommandContainer CommandContainer { get; set; }
        public bool CanRollDice
        {
            get
            {
                EnumStatus gameStatus = _mainGame!.SaveRoot!.GameStatus;
                return gameStatus == EnumStatus.Starts || gameStatus == EnumStatus.RollCharity ||
                    gameStatus == EnumStatus.RollLottery || gameStatus == EnumStatus.RollRadio;
            }
        }
        [Command(EnumCommandCategory.Plain)]
        public async Task RollDiceAsync()
        {
            await _roller.RollDiceAsync(); //hopefully this simple.
        }

    }
}