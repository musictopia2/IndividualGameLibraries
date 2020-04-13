using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.ViewModels;
using CommonBasicStandardLibraries.Exceptions;
using LifeBoardGameCP.Data;
using LifeBoardGameCP.Logic;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace LifeBoardGameCP.ViewModels
{
    public class StealTilesViewModel : BasicSubmitViewModel
    {
        private readonly LifeBoardGameGameContainer _gameContainer;
        private readonly LifeBoardGameVMData _model;
        private readonly IStolenTileProcesses _processes;

        public StealTilesViewModel(
            CommandContainer commandContainer,
            LifeBoardGameGameContainer gameContainer,
            LifeBoardGameVMData model,
            IStolenTileProcesses processes
            ) : base(commandContainer)
        {
            if (gameContainer.SaveRoot.GameStatus != EnumWhatStatus.NeedStealTile)
            {
                throw new BasicBlankException("Was not stealing tiles");
            }
            _gameContainer = gameContainer;
            _model = model;
            _processes = processes;
            _model.PlayerChosen = ""; //i think
        }

        public override bool CanSubmit => _model.PlayerChosen != "";

        public override Task SubmitAsync()
        {
            return _processes.TilesStolenAsync(_model.PlayerChosen);
        }
        [Command(EnumCommandCategory.Plain)]
        public Task EndTurnAsync()
        {
            return _gameContainer.EndTurnAsync!.Invoke();
        }
    }
}