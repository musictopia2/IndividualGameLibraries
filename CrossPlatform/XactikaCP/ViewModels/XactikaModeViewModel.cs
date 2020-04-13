using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.ViewModelInterfaces;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using System.Threading.Tasks;
using XactikaCP.Data;
using XactikaCP.Logic;

namespace XactikaCP.ViewModels
{
    [InstanceGame]
    public class XactikaModeViewModel : Screen, IBlankGameVM
    {
        private readonly XactikaVMData _model;
        private readonly XactikaGameContainer _gameContainer;
        private readonly IModeProcesses _processes;

        public XactikaModeViewModel(CommandContainer commandContainer,
            XactikaVMData model,
            XactikaGameContainer gameContainer,
            IModeProcesses processes
            )
        {
            CommandContainer = commandContainer;
            _model = model;
            _gameContainer = gameContainer;
            _processes = processes;
        }

        public CommandContainer CommandContainer { get; set; }
        public bool CanMode => _model.ModeChosen != EnumGameMode.None;
        [Command(EnumCommandCategory.Plain)]
        public async Task ModeAsync()
        {
            //if (_model.ModeChosen == EnumGameMode.None)
            //{
            //    await UIPlatform.ShowMessageAsync("Must choose mode");
            //    return;
            //}
            if (_gameContainer.BasicData.MultiPlayer)
            {
                await _gameContainer.Network!.SendAllAsync("gameoptionchosen", _model.ModeChosen);
            }
            await _processes.ProcessGameOptionChosenAsync(_model.ModeChosen, true);
        }
    }
}
