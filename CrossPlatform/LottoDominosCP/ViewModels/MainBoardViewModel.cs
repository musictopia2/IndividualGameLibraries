using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.ViewModelInterfaces;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using LottoDominosCP.Logic;
namespace LottoDominosCP.ViewModels
{
    [InstanceGame]
    public class MainBoardViewModel : Screen, IBlankGameVM
    {
        public MainBoardViewModel(CommandContainer commandContainer, LottoDominosMainGameClass mainGame)
        {
            if (mainGame.SaveRoot.GameStatus != Data.EnumStatus.NormalPlay)
            {
                throw new BasicBlankException("Can't load the board view model when the status is not even normal play.  Rethink");
            }
            CommandContainer = commandContainer;
        }
        //maybe nothing else is needed for this one.
        public CommandContainer CommandContainer { get; set; }
    }
}
