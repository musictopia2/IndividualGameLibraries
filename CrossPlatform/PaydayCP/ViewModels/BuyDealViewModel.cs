using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.ViewModels;
using PaydayCP.Data;
using PaydayCP.Logic;
using System.Threading.Tasks;

namespace PaydayCP.ViewModels
{
    public class BuyDealViewModel : BasicSubmitViewModel
    {
        private readonly PaydayVMData _model;
        private readonly IBuyProcesses _processes;

        public BuyDealViewModel(CommandContainer commandContainer, PaydayVMData model, IBuyProcesses processes) : base(commandContainer)
        {
            _model = model;
            _processes = processes;
        }

        public override bool CanSubmit => _model.CurrentDealList.ObjectSelected() > 0; //hopefully this simple.

        public override Task SubmitAsync() //hopefully this simple.
        {
            return _processes.BuyerSelectedAsync(_model.CurrentDealList.ObjectSelected());
        }
    }
}
