using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.ViewModelInterfaces;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using Spades2PlayerCP.Logic;
using System.Threading.Tasks;

namespace Spades2PlayerCP.ViewModels
{
    [InstanceGame]
    public class SpadesBeginningViewModel : Screen, IBlankGameVM
    {
        private readonly Spades2PlayerMainGameClass _mainGame;

        public SpadesBeginningViewModel(CommandContainer commandContainer, Spades2PlayerMainGameClass mainGame)
        {
            CommandContainer = commandContainer;
            _mainGame = mainGame;
        }

        public CommandContainer CommandContainer { get; set; }
        [Command(EnumCommandCategory.Plain)]
        public async Task TakeCardAsync()
        {
            if (_mainGame.BasicData!.MultiPlayer == true)
                await _mainGame.Network!.SendAllAsync("acceptcard");
            await _mainGame.AcceptCardAsync();
        }

    }
}
