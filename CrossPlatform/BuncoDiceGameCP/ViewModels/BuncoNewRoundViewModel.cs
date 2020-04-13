using System;
using System.Text;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using System.Linq;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using fs = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.FileHelpers;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using BasicGameFrameworkLibrary.ViewModelInterfaces;
using BasicGameFrameworkLibrary.CommandClasses;
using CommonBasicStandardLibraries.Messenging;
using BuncoDiceGameCP.EventModels;
using BasicGameFrameworkLibrary.Attributes;
//i think this is the most common things i like to do
namespace BuncoDiceGameCP.ViewModels
{
    [InstanceGame]
    public class BuncoNewRoundViewModel : Screen, IBlankGameVM
    {
        private readonly IEventAggregator _aggregator;

        public BuncoNewRoundViewModel(CommandContainer commandContainer, IEventAggregator aggregator)
        {
            CommandContainer = commandContainer;
            _aggregator = aggregator;
            commandContainer.ManuelFinish = false;
            commandContainer.IsExecuting = false;
        }
        public CommandContainer CommandContainer { get; set; }
        [Command(EnumCommandCategory.Plain)]
        public async Task NewRoundAsync()
        {
            await _aggregator.PublishAsync(new ChoseNewRoundEventModel());
        }
    }
}
