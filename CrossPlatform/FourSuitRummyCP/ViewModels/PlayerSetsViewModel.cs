using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.ViewModelInterfaces;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;

namespace FourSuitRummyCP.ViewModels
{
    [InstanceGame]
    public class PlayerSetsViewModel : Screen, IBlankGameVM
    {
        public PlayerSetsViewModel(CommandContainer commandContainer)
        {
            CommandContainer = commandContainer;
        }
        public CommandContainer CommandContainer { get; set; }
    }
}
