using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.Dice;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.Dominos;
using BasicGameFrameworkLibrary.MultiplayerClasses.MainViewModels;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using DiceDominosCP.Data;
using DiceDominosCP.Logic;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace DiceDominosCP.ViewModels
{
    [InstanceGame]
    public class DiceDominosMainViewModel : DiceGamesVM<SimpleDice>
    {
        private readonly DiceDominosMainGameClass _mainGame; //if we don't need, delete.
        private readonly GameBoardCP _gameBoard;

        public DiceDominosMainViewModel(CommandContainer commandContainer,
            DiceDominosMainGameClass mainGame,
            DiceDominosVMData viewModel,
            BasicData basicData,
            TestOptions test,
            IGamePackageResolver resolver,
            IStandardRollProcesses roller,
            DiceDominosGameContainer gameContainer,
            GameBoardCP gameBoard
            )
            : base(commandContainer, mainGame, viewModel, basicData, test, resolver, roller)
        {
            _mainGame = mainGame;
            _gameBoard = gameBoard;
            gameContainer.DominoClickedAsync = DominoClickedAsync;
            gameBoard.SendEnableProcesses(this, (() => _mainGame.SaveRoot.HasRolled));
        }
        //hopefully choose number not needed (?)
        private async Task DominoClickedAsync(SimpleDominoInfo thisDomino)
        {
            if (_gameBoard.IsValidMove(thisDomino.Deck) == false)
            {
                await UIPlatform.ShowMessageAsync("Illegal Move");
                return;
            }
            await _mainGame.MakeMoveAsync(thisDomino.Deck);
        }

        //anything else needed is here.
        protected override bool CanEnableDice()
        {
            if (_mainGame!.SaveRoot!.HasRolled == false || _mainGame.SaveRoot.DidHold == true)
                return false;
            return true;
        }
        public override bool CanEndTurn()
        {
            return _mainGame!.SaveRoot!.HasRolled;
        }
        public override bool CanRollDice()
        {
            return !_mainGame!.SaveRoot!.HasRolled;
        }

    }
}