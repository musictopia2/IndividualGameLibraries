using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.ViewModelInterfaces;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using CrazyEightsCP.Data;
using CrazyEightsCP.Logic;
using System.Threading.Tasks;

namespace CrazyEightsCP.ViewModels
{
    [InstanceGame]
    public class ChooseSuitViewModel : Screen, IBlankGameVM
    {
        private readonly CrazyEightsVMData _model;
        private readonly ISuitProcesses _processes;

        public ChooseSuitViewModel(CommandContainer commandContainer, CrazyEightsVMData model, ISuitProcesses processes)
        {
            CommandContainer = commandContainer;
            _model = model;
            _processes = processes;
            _model.SuitChooser.ItemClickedAsync += SuitChooser_ItemClickedAsync;
        }

        private Task SuitChooser_ItemClickedAsync(BasicGameFrameworkLibrary.RegularDeckOfCards.EnumSuitList piece)
        {
            return _processes.SuitChosenAsync(piece);
        }

        public CommandContainer CommandContainer { get; set; }
        protected override Task TryCloseAsync()
        {
            _model.SuitChooser.ItemClickedAsync -= SuitChooser_ItemClickedAsync;
            return base.TryCloseAsync();
        }
    }
}
