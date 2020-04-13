using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.ViewModelInterfaces;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using HuseHeartsCP.Data;
using HuseHeartsCP.Logic;
using System.Threading.Tasks;

namespace HuseHeartsCP.ViewModels
{
    [InstanceGame]
    public class MoonViewModel : Screen, IBlankGameVM
    {
        private readonly HuseHeartsMainGameClass _mainGame;

        public MoonViewModel(CommandContainer commandContainer, HuseHeartsMainGameClass mainGame)
        {
            CommandContainer = commandContainer;
            _mainGame = mainGame;
        }
        public CommandContainer CommandContainer { get; set; }
        [Command(EnumCommandCategory.Plain)]
        public async Task MoonAsync(EnumMoonOptions option)
        {
            switch (option)
            {
                case EnumMoonOptions.GiveSelfMinus:
                    if (_mainGame.BasicData!.MultiPlayer == true)
                        await _mainGame.Network!.SendAllAsync("takepointsaway");
                    await _mainGame!.GiveSelfMinusPointsAsync();
                    break;
                case EnumMoonOptions.GiveEverybodyPlus:
                    if (_mainGame.BasicData!.MultiPlayer == true)
                        await _mainGame.Network!.SendAllAsync("givepointseverybodyelse");
                    await _mainGame!.GiveEverybodyElsePointsAsync();
                    break;
                default:
                    throw new BasicBlankException("Not Supported");
            }
        }

    }
}
