using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.ColorCards;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.ViewModelInterfaces;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using System.Threading.Tasks;
using UnoCP.Data;
using UnoCP.Logic;

namespace UnoCP.ViewModels
{
    [InstanceGame]
    public class ChooseColorViewModel : Screen, IBlankGameVM
    {
        private readonly UnoVMData _model;
        private readonly IChooseColorProcesses _processes;

        public ChooseColorViewModel(CommandContainer commandContainer, UnoVMData model, IChooseColorProcesses processes)
        {
            CommandContainer = commandContainer;
            _model = model;
            _processes = processes;
            _model.ColorPicker.ItemClickedAsync += ColorPicker_ItemClickedAsync;
        }
        protected override Task TryCloseAsync()
        {
            _model.ColorPicker.ItemClickedAsync -= ColorPicker_ItemClickedAsync;
            return base.TryCloseAsync();
        }
        private async Task ColorPicker_ItemClickedAsync(EnumColorTypes piece)
        {
            await _processes!.ColorChosenAsync(piece);
        }

        public CommandContainer CommandContainer { get; set; }
    }
}
