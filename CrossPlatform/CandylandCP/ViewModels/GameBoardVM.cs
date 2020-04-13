using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.CommandClasses;
using CandylandCP.Logic;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
//i think this is the most common things i like to do
namespace CandylandCP.ViewModels
{
    [SingletonGame]
    [AutoReset]
    public class GameBoardVM
    {

        public GameBoardVM(CandylandMainGameClass mainGame, CommandContainer command)
        {
            _mainGame = mainGame;
            _command = command;
        }

        private readonly CandylandMainGameClass _mainGame;
        private readonly CommandContainer _command;

        public async Task CastleAsync()
        {
            _command.IsExecuting = true;
            await _mainGame.GameOverAsync();
        }

        
        public async Task MakeMoveAsync(int space)
        {
            _command.IsExecuting = true;
            await _mainGame!.MakeMoveAsync(space);
        }


    }
}
