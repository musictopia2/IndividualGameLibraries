using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.MainViewModels;
using BasicGameFrameworkLibrary.TestUtilities;
using CheckersCP.Data;
using CheckersCP.Logic;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
//i think this is the most common things i like to do
namespace CheckersCP.ViewModels
{
    [InstanceGame]
    public class CheckersMainViewModel : SimpleBoardGameVM
    {
        private readonly CheckersMainGameClass _mainGame; //if we don't need, delete.
        private readonly BasicData _basicData;

        public CheckersMainViewModel(CommandContainer commandContainer,
            CheckersMainGameClass mainGame,
            CheckersVMData model,
            BasicData basicData,
            TestOptions test,
            IGamePackageResolver resolver
            )
            : base(commandContainer, mainGame, model, basicData, test, resolver)
        {
            _mainGame = mainGame;
            _basicData = basicData;
        }
        //anything else needed is here.

        public override bool CanEndTurn()
        {
            return _mainGame.SaveRoot.GameStatus == EnumGameStatus.PossibleTie;
        }
        public bool CanTie
        {
            get
            {
                if (_mainGame.SaveRoot.SpaceHighlighted > 0)
                {
                    return false;
                }
                return _mainGame.SaveRoot.ForcedToMove == false;
            }
        }
        [Command(EnumCommandCategory.Game)]
        public async Task TieAsync()
        {
            if (_basicData.MultiPlayer)
            {
                await _mainGame.Network!.SendAllAsync("possibletie");
            }
            CommandContainer.ManuelFinish = true;
            await _mainGame.ProcessTieAsync();
        }
    }
}