using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.ViewModels;
using PaydayCP.Data;
using PaydayCP.Logic;
using System.Threading.Tasks;

namespace PaydayCP.ViewModels
{
    public class LotteryViewModel : BasicSubmitViewModel
    {
        private readonly PaydayVMData _model;
        private readonly ILotteryProcesses _processes;

        public LotteryViewModel(CommandContainer commandContainer, PaydayVMData model, ILotteryProcesses processes) : base(commandContainer)
        {
            _model = model;
            _processes = processes;
        }

        public override bool CanSubmit => _model.PopUpChosen != "";

        public override Task SubmitAsync()
        {
            return _processes.ProcessLotteryAsync();
        }
    }
}
