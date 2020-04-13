using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.ViewModels;
using System.Threading.Tasks;
using XactikaCP.Data;
using XactikaCP.Logic;

namespace XactikaCP.ViewModels
{
    public class XactikaSubmitShapeViewModel : BasicSubmitViewModel
    {
        private readonly XactikaVMData _model;
        private readonly IShapeProcesses _processes;
        private readonly XactikaGameContainer _gameContainer;
        public override string Text => "Choose Shape";
        public XactikaSubmitShapeViewModel(CommandContainer commandContainer,
            XactikaVMData model,
            IShapeProcesses processes,
            XactikaGameContainer gameContainer
            ) : base(commandContainer)
        {
            _model = model;
            _processes = processes;
            _gameContainer = gameContainer;
        }

        public override bool CanSubmit => _model.ShapeChosen != EnumShapes.None;

        public override async Task SubmitAsync()
        {
            if (_gameContainer.BasicData.MultiPlayer)
            {
                await _gameContainer.Network!.SendAllAsync("shapeused", _model.ShapeChosen);
            }
            await _processes.ShapeChosenAsync(_model.ShapeChosen);
        }

    }
}
