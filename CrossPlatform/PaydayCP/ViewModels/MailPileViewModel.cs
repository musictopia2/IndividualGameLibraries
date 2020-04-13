using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.ViewModelInterfaces;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;

namespace PaydayCP.ViewModels
{
    [InstanceGame]
    public class MailPileViewModel : Screen, IBlankGameVM
    {
        public MailPileViewModel(CommandContainer commandContainer)
        {
            CommandContainer = commandContainer;
            //this should hold the mail pile.
            //hopefully nothing else is needed (?)
        }

        public CommandContainer CommandContainer { get; set; }
    }
}
