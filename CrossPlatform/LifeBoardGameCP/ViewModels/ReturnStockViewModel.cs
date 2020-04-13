using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.ViewModels;
using CommonBasicStandardLibraries.Exceptions;
using LifeBoardGameCP.Data;
using LifeBoardGameCP.Logic;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace LifeBoardGameCP.ViewModels
{
    public class ReturnStockViewModel : BasicSubmitViewModel
    {
        private readonly LifeBoardGameGameContainer _gameContainer;
        private readonly LifeBoardGameVMData _model;
        private readonly IReturnStockProcesses _processes;

        public ReturnStockViewModel(CommandContainer commandContainer,
            LifeBoardGameGameContainer gameContainer,
            LifeBoardGameVMData model,
            IReturnStockProcesses processes
            ) : base(commandContainer)
        {
            _gameContainer = gameContainer;
            _model = model;
            _processes = processes;
            if (_gameContainer.SaveRoot.GameStatus != EnumWhatStatus.NeedReturnStock)
            {
                throw new BasicBlankException("Does not even need to return stock.  Rethink");
            }
        }

        public override bool CanSubmit => _model.HandList.ObjectSelected() > 0;

        public override Task SubmitAsync()
        {
            return _processes.StockReturnedAsync(_model.HandList.ObjectSelected());
        }
    }
}
