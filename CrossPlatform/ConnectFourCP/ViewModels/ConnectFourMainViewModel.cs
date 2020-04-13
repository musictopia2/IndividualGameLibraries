using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.MainViewModels;
using BasicGameFrameworkLibrary.TestUtilities;
using ConnectFourCP.Data;
using ConnectFourCP.Logic;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace ConnectFourCP.ViewModels
{
    [InstanceGame]
    public class ConnectFourMainViewModel : SimpleBoardGameVM
    {
        private readonly ConnectFourMainGameClass _mainGame; //if we don't need, delete.

        public ConnectFourMainViewModel(CommandContainer commandContainer,
            ConnectFourMainGameClass mainGame,
            ConnectFourVMData model,
            BasicData basicData,
            TestOptions test,
            IGamePackageResolver resolver
            )
            : base(commandContainer, mainGame, model, basicData, test, resolver)
        {
            _mainGame = mainGame;
        }

        public bool CanColumn(SpaceInfoCP space) => !_mainGame.SaveRoot.GameBoard.IsFilled(space.Vector.Column);

        [Command(EnumCommandCategory.Game)]
        public async Task ColumnAsync(SpaceInfoCP space)
        {
            await _mainGame.MakeMoveAsync(space.Vector.Column);
        }
    }
}