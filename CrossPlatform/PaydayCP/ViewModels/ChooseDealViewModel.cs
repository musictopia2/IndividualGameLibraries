using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.ViewModels;
using PaydayCP.Data;
using PaydayCP.Logic;
using System.Threading.Tasks;

namespace PaydayCP.ViewModels
{
    public class ChooseDealViewModel : BasicSubmitViewModel
    {
        private readonly PaydayVMData _model;
        private readonly IDealProcesses _processes;

        //only tablets would need this one.
        public ChooseDealViewModel(CommandContainer commandContainer, PaydayVMData model, IDealProcesses processes) : base(commandContainer)
        {
            _model = model;
            _processes = processes;
        }

        public override bool CanSubmit => _model.PopUpChosen != "";

        public override Task SubmitAsync()
        {
            return _processes.ChoseWhetherToPurchaseDealAsync();
        }
    }
}
