using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.ViewModelInterfaces;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using System.Threading.Tasks;
using UnoCP.Data;
using UnoCP.Logic;

namespace UnoCP.ViewModels
{
    [InstanceGame]
    public class SayUnoViewModel : Screen, IBlankGameVM
    {
        private readonly ISayUnoProcesses _processes;
        private readonly UnoVMData _model;

        //this will hold the timer.
        //only the player who needs it will show it.

        public SayUnoViewModel(CommandContainer commandContainer, ISayUnoProcesses processes, UnoVMData model)
        {
            CommandContainer = commandContainer;
            _processes = processes;
            _model = model;
            _model.Stops.TimeUp += Stops_TimeUp;
            CommandContainer.ExecutingChanged += CommandContainer_ExecutingChanged;
        }
        //private bool _unoProcessing;
        private void CommandContainer_ExecutingChanged()
        {
            if (CommandContainer.IsExecuting)
            {
                return;
            }
            NotifyOfCanExecuteChange(nameof(CanUno)); //i think
            //if (_unoProcessing)
            //{
            //    return;
            //}
            _model.Stops.StartTimer();
        }

        private async void Stops_TimeUp()
        {
            CommandContainer!.ManuelFinish = true;
            CommandContainer.IsExecuting = true; //now its executing.
            //_unoProcessing = false;
            await _processes.ProcessUnoAsync(false);
        }
        protected override Task TryCloseAsync()
        {
            _model.Stops.TimeUp -= Stops_TimeUp;
            CommandContainer.ExecutingChanged -= CommandContainer_ExecutingChanged;
            return base.TryCloseAsync();
        }
        public CommandContainer CommandContainer { get; set; }
        public bool CanUno => true;
        [Command(EnumCommandCategory.Plain)]
        public async Task SayUnoAsync()
        {
            //_unoProcessing = false;
            _model.Stops!.PauseTimer();
            await _processes!.ProcessUnoAsync(true);
        }

    }
}
