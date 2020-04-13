using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.MainViewModels;
using BasicGameFrameworkLibrary.TestUtilities;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using TicTacToeCP.Data;
using TicTacToeCP.Logic;
namespace TicTacToeCP.ViewModels
{
    [InstanceGame]
    public class TicTacToeMainViewModel : BasicMultiplayerMainVM
    {
        private readonly TicTacToeMainGameClass _mainGame; //if we don't need, delete.

        public TicTacToeMainViewModel(CommandContainer commandContainer,
            TicTacToeMainGameClass mainGame,
            IViewModelData viewModel,
            BasicData basicData,
            TestOptions test,
            IGamePackageResolver resolver
            )
            : base(commandContainer, mainGame, viewModel, basicData, test, resolver)
        {
            _mainGame = mainGame;
        }
        public bool CanMakeMove(SpaceInfoCP space)
        {
            return space.Status == EnumSpaceType.Blank;
        }
        [Command(EnumCommandCategory.Game)]
        public async Task MakeMoveAsync(SpaceInfoCP space)
        {
            await _mainGame.MakeMoveAsync(space);
        }

    }
}