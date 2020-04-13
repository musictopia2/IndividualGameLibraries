using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.ViewModelInterfaces;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using LifeBoardGameCP.Data;
using LifeBoardGameCP.Logic;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace LifeBoardGameCP.ViewModels
{
    [InstanceGame]
    public class SpinnerViewModel : Screen, IBlankGameVM
    {
        private readonly ISpinnerProcesses _processes;
        private readonly LifeBoardGameGameContainer _gameContainer;

        public SpinnerViewModel(CommandContainer commandContainer,
            ISpinnerProcesses processes,
            LifeBoardGameGameContainer gameContainer
            )
        {
            CommandContainer = commandContainer;
            _processes = processes;
            _gameContainer = gameContainer;
        }
        //hopefully no need for submit command here.
        public CommandContainer CommandContainer { get; set; }
        public bool CanSpin => _gameContainer.CanSpin;
        [Command(EnumCommandCategory.Plain)]
        public async Task SpinAsync()
        {
            await _processes.StartSpinningAsync();
        }
    }
}