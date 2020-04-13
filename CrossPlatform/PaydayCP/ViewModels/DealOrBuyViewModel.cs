using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.ViewModels;
using PaydayCP.Data;
using PaydayCP.Logic;
using System.Threading.Tasks;

namespace PaydayCP.ViewModels
{
    public class DealOrBuyViewModel : BasicSubmitViewModel
    {
        private readonly PaydayVMData _model;
        private readonly IDealBuyChoiceProcesses _processes;

        public DealOrBuyViewModel(CommandContainer commandContainer, PaydayVMData model, IDealBuyChoiceProcesses processes) : base(commandContainer)
        {
            _model = model;
            _processes = processes;
        }

        public override bool CanSubmit => _model.PopUpChosen != "";

        public override Task SubmitAsync()
        {
            return _processes.ChoseDealOrBuyAsync();
        }
    }
}
