using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.ViewModelInterfaces;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;

namespace PaydayCP.ViewModels
{
    [InstanceGame]
    public class MailListViewModel : Screen, IBlankGameVM
    {
        public MailListViewModel(CommandContainer commandContainer)
        {
            CommandContainer = commandContainer;
        }
        public CommandContainer CommandContainer { get; set; }
    }
}
