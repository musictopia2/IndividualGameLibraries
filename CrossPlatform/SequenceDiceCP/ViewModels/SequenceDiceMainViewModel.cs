using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.Dice;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.MainViewModels;
using BasicGameFrameworkLibrary.NetworkingClasses.Extensions;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using SequenceDiceCP.Data;
using SequenceDiceCP.Logic;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace SequenceDiceCP.ViewModels
{
    [InstanceGame]
    public class SequenceDiceMainViewModel : BoardDiceGameVM
    {
        private readonly SequenceDiceMainGameClass _mainGame; //if we don't need, delete.

        public SequenceDiceMainViewModel(CommandContainer commandContainer,
            SequenceDiceMainGameClass mainGame,
            SequenceDiceVMData model,
            BasicData basicData,
            TestOptions test,
            IGamePackageResolver resolver,
            IStandardRollProcesses roller
            )
            : base(commandContainer, mainGame, model, basicData, test, resolver, roller)
        {
            _mainGame = mainGame;
        }
        //anything else needed is here.
        public override bool CanRollDice()
        {
            return _mainGame.SaveRoot!.GameStatus != EnumGameStatusList.MovePiece;
        }
        public override async Task RollDiceAsync() //if any changes, do here.
        {
            await base.RollDiceAsync();
        }
        public bool CanMakeMove => _mainGame.SaveRoot.GameStatus == EnumGameStatusList.MovePiece;
        [Command(EnumCommandCategory.Game)]
        public async Task MakeMoveAsync(SpaceInfoCP space)
        {
            if (_mainGame.SaveRoot.GameBoard.CanMakeMove(space) == false)
            {
                await UIPlatform.ShowMessageAsync("Illegal Move");
                return;
            }
            if (_mainGame.BasicData.MultiPlayer)
            {
                await _mainGame.Network!.SendMoveAsync(space);
            }
            await _mainGame.MakeMoveAsync(space);
        }

    }
}