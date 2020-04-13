using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.ViewModels;
using CommonBasicStandardLibraries.Exceptions;
using LifeBoardGameCP.Data;
using LifeBoardGameCP.Logic;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace LifeBoardGameCP.ViewModels
{
    public class TradeSalaryViewModel : BasicSubmitViewModel
    {
        private readonly LifeBoardGameGameContainer _gameContainer;
        private readonly LifeBoardGameVMData _model;
        private readonly ITradeSalaryProcesses _processes;

        //iffy 
        public TradeSalaryViewModel(
            CommandContainer commandContainer,
            LifeBoardGameGameContainer gameContainer,
            LifeBoardGameVMData model,
            ITradeSalaryProcesses processes
            ) : base(commandContainer)
        {
            if (gameContainer.SaveRoot.GameStatus != EnumWhatStatus.NeedTradeSalary)
            {
                throw new BasicBlankException("Was not trading salary.  Therefore, should not have loaded the trade salary view model.  Rethink");
            }

            _gameContainer = gameContainer;
            _model = model;
            _processes = processes;
            _model.PlayerChosen = ""; //i think
            //reset the other part.
        }

        public override bool CanSubmit => _model.PlayerChosen != "";

        public override Task SubmitAsync()
        {
            return _processes.TradedSalaryAsync(_model.PlayerChosen);
        }
        [Command(EnumCommandCategory.Plain)]
        public Task EndTurnAsync()
        {
            return _gameContainer.EndTurnAsync!.Invoke();
        }
    }
}
