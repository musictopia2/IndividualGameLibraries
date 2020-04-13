using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.Extensions;
using BasicGameFrameworkLibrary.MultiplayerClasses.MainViewModels;
using BasicGameFrameworkLibrary.NetworkingClasses.Extensions;
using BasicGameFrameworkLibrary.TestUtilities;
using SorryCP.Data;
using SorryCP.Logic;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
//i think this is the most common things i like to do
namespace SorryCP.ViewModels
{
    [InstanceGame]
    public class SorryMainViewModel : SimpleBoardGameVM
    {
        private readonly SorryMainGameClass _mainGame; //if we don't need, delete.
        private readonly SorryGameContainer _gameContainer;
        private readonly GameBoardProcesses _gameBoard;

        public override bool CanEndTurn()
        {
            bool rets = base.CanEndTurn();
            if (rets == false)
                return false;
            return !_gameBoard.HasRequiredMove;
        }

        public SorryMainViewModel(CommandContainer commandContainer,
            SorryMainGameClass mainGame,
            SorryVMData model,
            BasicData basicData,
            TestOptions test,
            IGamePackageResolver resolver,
            SorryGameContainer gameContainer,
            GameBoardProcesses gameBoard
            )
            : base(commandContainer, mainGame, model, basicData, test, resolver)
        {
            _mainGame = mainGame;
            _gameContainer = gameContainer;
            _gameBoard = gameBoard;
            _gameContainer.SpaceClickedAsync = MakeMoveAsync;
            _gameContainer.DrawClickAsync = DrawAsync;
            _gameContainer.HomeClickedAsync = HomeAsync;
        }

        private async Task MakeMoveAsync(int space)
        {
            if (_gameBoard.IsValidMove(space) == false)
            {
                return;
            }
            if (_gameContainer.CanSendMessage())
            {
                await _gameContainer.Network!.SendMoveAsync(space);
            }
            await _mainGame.MakeMoveAsync(space);
        }

        private async Task DrawAsync()
        {
            if (_gameContainer.CanSendMessage())
            {
                await _gameContainer.Network!.SendDrawAsync();
            }
            await _mainGame.DrawCardAsync();
        }
        private async Task HomeAsync(EnumColorChoice color)
        {
            if (_gameBoard.CanGoHome(color) == false)
            {
                return;
            }
            if (_gameContainer.CanSendMessage())
            {
                await _gameContainer.Network!.SendMoveAsync(100);
            }
            await _mainGame.MakeMoveAsync(100);
        }

        private string _cardDetails = "";
        [VM]
        public string CardDetails
        {
            get { return _cardDetails; }
            set
            {
                if (SetProperty(ref _cardDetails, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
    }
}