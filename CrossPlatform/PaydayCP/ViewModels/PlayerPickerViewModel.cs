using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.ViewModels;
using PaydayCP.Data;
using PaydayCP.Logic;
using System.Threading.Tasks;

namespace PaydayCP.ViewModels
{
    public class PlayerPickerViewModel : BasicSubmitViewModel
    {
        private readonly PaydayVMData _model;
        private readonly IChoosePlayerProcesses _processes;

        public PlayerPickerViewModel(CommandContainer commandContainer, PaydayVMData model, IChoosePlayerProcesses processes) : base(commandContainer)
        {
            _model = model;
            _processes = processes;
        }



        protected override Task ActivateAsync()
        {
            //i think that something else will load the list.
            return base.ActivateAsync();
        }
        protected override Task TryCloseAsync()
        {
            return base.TryCloseAsync();
        }

        public override bool CanSubmit => _model.PopUpChosen != ""; //hopefully this simple.

        public override Task SubmitAsync()
        {
            //this is choosing player for either mad money or pay a neighbor.
            return _processes.ProcessChosenPlayerAsync();
        }
    }
}
