using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.ViewModelInterfaces;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using System.Threading.Tasks;
using XactikaCP.Data;
using XactikaCP.Logic;

namespace XactikaCP.ViewModels
{
    [InstanceGame]
    public class XactikaBidViewModel : Screen, IBlankGameVM
    {
        private readonly XactikaVMData _model;
        private readonly XactikaGameContainer _gameContainer;
        private readonly IBidProcesses _processes;

        public XactikaBidViewModel(CommandContainer commandContainer,
            XactikaVMData model,
            XactikaGameContainer gameContainer,
            IBidProcesses processes
            )
        {
            CommandContainer = commandContainer;
            _model = model;
            _gameContainer = gameContainer;
            _processes = processes;
        }

        public CommandContainer CommandContainer { get; set; }
        public bool CanBid => _model.BidChosen > -1;
        [Command(EnumCommandCategory.Plain)]
        public async Task BidAsync()
        {
            if (_gameContainer.BasicData.MultiPlayer)
            {
                await _gameContainer.Network!.SendAllAsync("bid", _model.BidChosen);
            }
            await _processes.ProcessBidAsync();
        }
    }
}
