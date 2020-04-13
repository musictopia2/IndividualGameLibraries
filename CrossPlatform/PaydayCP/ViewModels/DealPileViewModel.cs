using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.ViewModelInterfaces;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using PaydayCP.Data;
using PaydayCP.Logic;
using System.Threading.Tasks;

namespace PaydayCP.ViewModels
{
    [InstanceGame]
    public class DealPileViewModel : Screen, IBlankGameVM
    {

        public DealPileViewModel(CommandContainer commandContainer)
        {
            CommandContainer = commandContainer;
        }

        public CommandContainer CommandContainer { get; set; }


    }
}