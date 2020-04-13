using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.ViewModelInterfaces;
using BuncoDiceGameCP.Logic;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace BuncoDiceGameCP.ViewModels
{
    [InstanceGame]
    public class EndGameViewModel : Screen, IBlankGameVM
    {
        private readonly BuncoDiceGameMainGameClass _game;
        [Command(EnumCommandCategory.Plain)]
        public async Task EndGameAsync()
        {
            await _game.PossibleNewGameAsync();
        }

        public EndGameViewModel(CommandContainer container, BuncoDiceGameMainGameClass game)
        {
            CommandContainer = container;
            _game = game;
            CommandContainer.ManuelFinish = false;
            CommandContainer.IsExecuting = false;
        }
        public CommandContainer CommandContainer { get; set; }
    }
}
