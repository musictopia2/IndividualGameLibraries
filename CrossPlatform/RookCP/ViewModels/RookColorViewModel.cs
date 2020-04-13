using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.ColorCards;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.ViewModelInterfaces;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using RookCP.Data;
using RookCP.Logic;
using System.Threading.Tasks;

namespace RookCP.ViewModels
{
    [InstanceGame]
    public class RookColorViewModel : Screen, IBlankGameVM
    {
        private readonly RookVMData _model;
        private readonly ITrumpProcesses _processes;

        public RookColorViewModel(CommandContainer commandContainer,
            RookVMData model,
            ITrumpProcesses processes
            )
        {
            CommandContainer = commandContainer;
            _model = model;
            _processes = processes;
        }
        public CommandContainer CommandContainer { get; set; }
        public bool CanTrump => _model.ColorChosen != EnumColorTypes.None;
        [Command(EnumCommandCategory.Plain)]
        public async Task TrumpAsync()
        {
            await _processes.ProcessTrumpAsync();
        }
    }
}
