using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.ViewModelInterfaces;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using SkuckCardGameCP.Data;
using SkuckCardGameCP.Logic;
using System.Threading.Tasks;

namespace SkuckCardGameCP.ViewModels
{
    [InstanceGame]
    public class SkuckChoosePlayViewModel : Screen, IBlankGameVM
    {
        private readonly SkuckCardGameGameContainer _gameContainer;
        private readonly IPlayChoiceProcesses _processes;

        public SkuckChoosePlayViewModel(
            CommandContainer commandContainer,
            SkuckCardGameGameContainer gameContainer,
            IPlayChoiceProcesses processes)
        {
            CommandContainer = commandContainer;
            _gameContainer = gameContainer;
            _processes = processes;
        }
        public CommandContainer CommandContainer { get; set; }
        [Command(EnumCommandCategory.Plain)]
        public async Task FirstPlayAsync(EnumChoiceOption choice)
        {
            if (choice == EnumChoiceOption.None)
                throw new BasicBlankException("Not Supported");
            if (choice == EnumChoiceOption.Play)
            {
                if (_gameContainer.BasicData!.MultiPlayer == true)
                    await _gameContainer.Network!.SendAllAsync("choosetoplay");
                await _processes!.ChooseToPlayAsync();
                return;
            }
            if (choice == EnumChoiceOption.Pass)
            {
                if (_gameContainer.BasicData!.MultiPlayer == true)
                    await _gameContainer.Network!.SendAllAsync("choosetopass");
                await _processes!.ChooseToPassAsync();
                return;
            }
        }
    }
}