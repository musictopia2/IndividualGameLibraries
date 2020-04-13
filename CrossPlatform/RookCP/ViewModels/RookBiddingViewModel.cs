using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.ViewModelInterfaces;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using RookCP.Data;
using RookCP.Logic;
using System.Threading.Tasks;

namespace RookCP.ViewModels
{
    [InstanceGame]
    public class RookBiddingViewModel : Screen, IBlankGameVM
    {
        private readonly RookVMData _model;
        private readonly IBidProcesses _processes;

        public RookBiddingViewModel(CommandContainer commandContainer,
            RookVMData model,
            IBidProcesses processes
            )
        {
            CommandContainer = commandContainer;
            _model = model;
            _processes = processes;
        }
        public CommandContainer CommandContainer { get; set; }
        public bool CanBid => _model.BidChosen > -1;
        [Command(EnumCommandCategory.Plain)]
        public async Task BidAsync()
        {
            await _processes.ProcessBidAsync();
        }
        public bool CanPass => _model.CanPass;
        [Command(EnumCommandCategory.Plain)]
        public async Task PassAsync()
        {
            await _processes.PassBidAsync();
        }

    }
}
