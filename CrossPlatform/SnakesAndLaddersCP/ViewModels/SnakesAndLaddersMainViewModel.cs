using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.MainViewModels;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using SnakesAndLaddersCP.Logic;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace SnakesAndLaddersCP.ViewModels
{
    [InstanceGame]
    public class SnakesAndLaddersMainViewModel : BasicMultiplayerMainVM
    {
        private readonly SnakesAndLaddersMainGameClass _mainGame; //if we don't need, delete.

        public SnakesAndLaddersMainViewModel(CommandContainer commandContainer,
            SnakesAndLaddersMainGameClass mainGame,
            IViewModelData viewModel,
            BasicData basicData,
            TestOptions test,
            IGamePackageResolver resolver
            )
            : base(commandContainer, mainGame, viewModel, basicData, test, resolver)
        {
            _mainGame = mainGame;
        }
        //anything else needed is here.
        public bool CanRollDice => !_mainGame.SaveRoot.HasRolled;
        [Command(EnumCommandCategory.Game)]
        public async Task RollDiceAsync()
        {
            await _mainGame.Roll.RollDiceAsync();
        }
        public bool CanMakeMove(int space)
        {
            if (space == 0)
            {
                return false;
            }
            return _mainGame.SaveRoot.HasRolled;
        }

        [Command(EnumCommandCategory.Game)]
        public async Task MakeMoveAsync(int space)
        {
            if (_mainGame.GameBoard1.IsValidMove(space) == false)
            {
                await UIPlatform.ShowMessageAsync("Illegal Move");
                return;
            }
            await _mainGame.MakeMoveAsync(space);
        }
    }
}