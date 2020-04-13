using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.Dice;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.MainViewModels;
using BasicGameFrameworkLibrary.NetworkingClasses.Extensions;
using BasicGameFrameworkLibrary.TestUtilities;
using PassOutDiceGameCP.Data;
using PassOutDiceGameCP.Logic;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace PassOutDiceGameCP.ViewModels
{
    [InstanceGame]
    public class PassOutDiceGameMainViewModel : BoardDiceGameVM
    {
        private readonly PassOutDiceGameMainGameClass _mainGame; //if we don't need, delete.
        private readonly GameBoardProcesses _gameBoard;

        public PassOutDiceGameMainViewModel(CommandContainer commandContainer,
            PassOutDiceGameMainGameClass mainGame,
            PassOutDiceGameVMData model,
            BasicData basicData,
            TestOptions test,
            IGamePackageResolver resolver,
            IStandardRollProcesses roller,
            PassOutDiceGameGameContainer gameContainer,
            GameBoardProcesses gameBoard
            )
            : base(commandContainer, mainGame, model, basicData, test, resolver, roller)
        {
            _mainGame = mainGame;
            _gameBoard = gameBoard;
            gameContainer.MakeMoveAsync = MakeMoveAsync;
        }
        private async Task MakeMoveAsync(int space)
        {
            if (_gameBoard.IsValidMove(space) == false)
            {
                return;
            }
            if (_mainGame.BasicData.MultiPlayer)
            {
                await _mainGame.Network!.SendMoveAsync(space);
            }
            await _mainGame.MakeMoveAsync(space);
        }
        //anything else needed is here.
        public override bool CanRollDice()
        {
            return false;
        }
        public override async Task RollDiceAsync() //if any changes, do here.
        {
            await base.RollDiceAsync();
        }
    }
}